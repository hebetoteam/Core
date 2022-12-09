using Core.Application.Interfaces;
using Core.Application.ViewModels.BlockChain;
using Core.Application.ViewModels.Saving;
using Core.Application.ViewModels.Swap;
using Core.Application.ViewModels.Transfer;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Extensions;
using Core.Utilities.Constants;
using Core.Utilities.Dtos;
using Core.Utilities.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Areas.Admin.Controllers
{

    public class SwapController : BaseController
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<SwapController> _logger;
        private readonly IUserService _userService;
        private readonly IBlockChainService _blockChainService;
        private readonly IConfiguration _configuration;
        private readonly IConfigService _configService;

        public SwapController(
            IConfigService configService,
            IWalletTransactionService walletTransactionService,
            ILogger<SwapController> logger,
            UserManager<AppUser> userManager,
            IUserService userService,
            IBlockChainService blockChainService,
            IConfiguration configuration
            )
        {
            _configService = configService;
            _configuration = configuration;
            _blockChainService = blockChainService;
            _walletTransactionService = walletTransactionService;
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
        }


        public async Task<IActionResult> Index()
        {
            var appUser = await _userService.GetById(CurrentUserId.ToString());

            ViewBag.Enabled2FA = appUser.Enabled2FA;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Swap(
            [FromBody] SwapViewModel model, [FromQuery] string authenticatorCode)
        {
            try
            {
                var appUser = await _userManager.FindByIdAsync(CurrentUserId.ToString());
                if (appUser == null)
                    return new OkObjectResult(new GenericResult(false, "Account does not exist"));

                if (appUser.IsShowOff)
                    return new OkObjectResult(new GenericResult(false, "Account does not allow to swap"));

                var isMatched = await _userManager.CheckPasswordAsync(appUser, model.Password);
                if (!isMatched)
                    return new OkObjectResult(new GenericResult(false, "Wrong password"));

                if (appUser.TwoFactorEnabled)
                {
                    var isValid = await VerifyCode(authenticatorCode, _userManager, appUser);
                    if (!isValid)
                        return new OkObjectResult(new GenericResult(false, "Invalid authenticator code"));
                }

                if (model.Amount < CommonConstants.HBTMinSwap)
                {
                    return new OkObjectResult(new GenericResult(false,
                        $"Minimum swap {CommonConstants.HBTMinSwap} {CommonConstants.TOKEN_OUT_CODE}"));
                }

                if (model.Amount > appUser.HBTAmount)
                {
                    return new OkObjectResult(new GenericResult(false,
                        "Your balance is not enough to make a transaction"));
                }

                appUser.HBTAmount -= model.Amount;

                var swapFeeAmount = model.Amount * (CommonConstants.HBTFeeSwap / 100);

                var receivedAmount = model.Amount - swapFeeAmount;

                var tokenPrice = _configService.GetTokenPrice();

                var receivedUSDT = receivedAmount * tokenPrice;

                appUser.USDTAmount += receivedUSDT;

                var updateUserBalance = await _userManager.UpdateAsync(appUser);

                if (updateUserBalance.Succeeded)
                {
                    var txnHash = Guid.NewGuid().ToString("N");

                    _walletTransactionService.AddTransaction(
                        appUser.Id,
                        model.Amount,
                        receivedAmount,
                        WalletTransactionType.SwapFromHBT,
                        $"Wallet {Unit.HBT.GetDescription()}",
                        "System",
                        Unit.HBT,
                        CommonConstants.HBTFeeSwap,
                        swapFeeAmount,
                        txnHash,
                        $"Swap from {Unit.HBT.GetDescription()} " +
                        $"to {Unit.USDT.GetDescription()} of {appUser.Email}");

                    _walletTransactionService.AddTransaction(
                        appUser.Id,
                        receivedUSDT,
                        receivedUSDT,
                        WalletTransactionType.SwapToUSDT,
                        "System",
                        $"Wallet {Unit.USDT.GetDescription()}",
                        Unit.USDT,
                        0,
                        0,
                        txnHash,
                        $"Swap from {Unit.HBT.GetDescription()} " +
                        $"to {Unit.USDT.GetDescription()} of {appUser.Email}");

                    return new OkObjectResult(
                        new GenericResult(true, $"Successfully swap " +
                        $"from {Unit.HBT.GetDescription()} to {Unit.USDT.GetDescription()}"));
                }
                else
                {
                    return new OkObjectResult(new GenericResult(false,
                        string.Join(",", updateUserBalance.Errors.Select(x => x.Description))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwapWallet: {0}", ex.Message);
                return new OkObjectResult(new GenericResult(false, ex.Message));
            }
        }


        public async Task<IActionResult> GetBalance()
        {
            var appUser = await _userManager.FindByIdAsync(CurrentUserId.ToString());
            if (appUser == null)
            {
                return new OkObjectResult(new GenericResult(false, "Account does not exist"));
            }

            var tokenPrice = _configService.GetTokenPrice();

            var model = new SwapBalanceViewModel()
            {
                Balance = appUser.HBTAmount,
                MinSwap = CommonConstants.HBTMinSwap,
                SwapFee = CommonConstants.HBTFeeSwap,
                TokenPrice = tokenPrice
            };

            return new OkObjectResult(model);
        }

    }
}
