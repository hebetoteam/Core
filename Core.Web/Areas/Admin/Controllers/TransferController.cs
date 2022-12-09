using Core.Application.Interfaces;
using Core.Application.ViewModels.BlockChain;
using Core.Application.ViewModels.System;
using Core.Application.ViewModels.Transfer;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Utilities.Constants;
using Core.Utilities.Dtos;
using Core.Utilities.Extensions;
using Core.Utilities.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Areas.Admin.Controllers
{
    public class TransferController : BaseController
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<TransferController> _logger;
        private readonly IUserService _userService;
        private readonly IBlockChainService _blockChainService;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly ITicketTransactionService _ticketTransactionService;
        public TransferController(
            ILogger<TransferController> logger,
            UserManager<AppUser> userManager,
            IUserService userService,
            IBlockChainService blockChainService,
            IWalletTransactionService walletTransactionService,
            ITicketTransactionService ticketTransactionService
            )
        {
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
            _blockChainService = blockChainService;
            _walletTransactionService = walletTransactionService;
            _ticketTransactionService = ticketTransactionService;
        }

        public async Task<IActionResult> Index()
        {
            var appUser = await _userService.GetById(CurrentUserId.ToString());

            ViewBag.Enabled2FA = appUser.Enabled2FA;

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(
            [FromBody] TransferViewModel model, [FromQuery] string authenticatorCode)
        {
            try
            {
                var appUser = await _userManager.FindByIdAsync(CurrentUserId.ToString());
                if (appUser == null)
                    return new OkObjectResult(new GenericResult(false, "Account does not exist"));

                if (appUser.IsShowOff)
                    return new OkObjectResult(new GenericResult(false, "Account does not allow to transfer"));

                var isMatched = await _userManager.CheckPasswordAsync(appUser, model.Password);
                if (!isMatched)
                    return new OkObjectResult(new GenericResult(false, "Wrong password"));

                if (appUser.TwoFactorEnabled)
                {
                    var isValid = await VerifyCode(authenticatorCode, _userManager, appUser);
                    if (!isValid)
                        return new OkObjectResult(new GenericResult(false, "Invalid authenticator code"));
                }

                var getTokenInfo = await GetBalance(model.Unit);

                if (!getTokenInfo.Success)
                    return new OkObjectResult(getTokenInfo);

                var tokenInfo = getTokenInfo.Data as TransferBalanceViewModel;

                if (model.Amount < tokenInfo.MinTransfer)
                    return new OkObjectResult(new GenericResult(false,
                        $"Minimum transfer {tokenInfo.MinTransfer} {model.Unit.GetDescription()}"));

                if (model.Amount > tokenInfo.Balance)
                    return new OkObjectResult(new GenericResult(false,
                        "Your balance is not enough to make a transaction"));

                if (string.IsNullOrEmpty(model.Sponsor) || model.Sponsor.Length <= 4)
                    return new OkObjectResult(new GenericResult(false, "Sponsor does not exists."));

                var sponsorString = model.Sponsor.GetRawSponsor();

                if (string.IsNullOrEmpty(sponsorString))
                    return new OkObjectResult(new GenericResult(false, "Sponsor does not exists."));

                var userSponsor = _userManager.Users.FirstOrDefault(x => x.Sponsor == sponsorString);
                if (userSponsor == null)
                    return new OkObjectResult(new GenericResult(false, "Sponsor does not exists."));

                var isStaked = _walletTransactionService.IsStaked(CurrentUserId);
                if (!isStaked)
                    return new OkObjectResult(new GenericResult(false, "Account should staking to withdraw"));

                var transferFeeAmount = model.Amount * (tokenInfo.TransferFee / 100);

                var receiveAmount = model.Amount - transferFeeAmount;

                if (model.Unit == Unit.HBT)
                {
                    appUser.HBTAmount -= model.Amount;
                    userSponsor.HBTAmount += receiveAmount;
                }
                else
                {
                    appUser.USDTAmount -= model.Amount;
                    userSponsor.USDTAmount += receiveAmount;
                }

                var updateFromUser = await _userManager.UpdateAsync(appUser);

                if (updateFromUser.Succeeded)
                {
                    var txnHash = Guid.NewGuid().ToString("N");

                    var txn1 = new WalletTransactionViewModel()
                    {
                        AddressFrom = $"Wallet {model.Unit.GetDescription()}",
                        AddressTo = userSponsor.Email,
                        Amount = model.Amount,
                        Fee = tokenInfo.TransferFee,
                        FeeAmount = transferFeeAmount,
                        AmountReceive = receiveAmount,
                        AppUserId = appUser.Id,
                        TransactionHash = txnHash,
                        DateCreated = DateTime.UtcNow,
                        Unit = model.Unit,
                        Type = WalletTransactionType.Transfer,
                        Remarks = $"Transfer from {appUser.Email} to {userSponsor.Email}",
                    };

                    _walletTransactionService.Add(txn1);
                    _walletTransactionService.Save();

                    var updateToUser = await _userManager.UpdateAsync(userSponsor);

                    if (updateToUser.Succeeded)
                    {
                        var txn2 = new WalletTransactionViewModel()
                        {
                            AddressFrom = appUser.Email,
                            AddressTo = $"Wallet {model.Unit.GetDescription()}",
                            Amount = model.Amount,
                            Fee = tokenInfo.TransferFee,
                            FeeAmount = transferFeeAmount,
                            AmountReceive = receiveAmount,
                            AppUserId = userSponsor.Id,
                            TransactionHash = txnHash,
                            DateCreated = DateTime.UtcNow,
                            Unit = model.Unit,
                            Type = WalletTransactionType.Received,
                            Remarks = $"Transfer from {appUser.Email} to {userSponsor.Email}",
                        };

                        _walletTransactionService.Add(txn2);
                        _walletTransactionService.Save();
                    }
                    else
                    {
                        return new OkObjectResult(new GenericResult(false,
                       string.Join(",", updateToUser.Errors.Select(x => x.Description))));
                    }


                    return new OkObjectResult(new GenericResult(true,
                        $"Transfer from Wallet {model.Unit.GetDescription()} to {userSponsor.Email} is successful"));
                }
                else
                {
                    return new OkObjectResult(new GenericResult(false,
                       string.Join(",", updateFromUser.Errors.Select(x => x.Description))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TransferWallet: {0}", ex.Message);
                return new OkObjectResult(new GenericResult(false, ex.Message));
            }
        }

        public async Task<GenericResult> GetBalance(Unit unit)
        {
            var appUser = await _userManager.FindByIdAsync(CurrentUserId.ToString());
            if (appUser == null)
            {
                return new GenericResult(false, "Account does not exist");
            }

            decimal balance = 0;
            decimal minTransfer = 0;
            decimal transferFee = 0;

            if (unit == Unit.HBT)
            {
                balance = appUser.HBTAmount;
                minTransfer = CommonConstants.HBTMinTransfer;
                transferFee = CommonConstants.HBTFeeTransfer;
            }
            else
            {
                balance = appUser.USDTAmount;
                minTransfer = CommonConstants.USDTMinTransfer;
                transferFee = CommonConstants.USDTFeeTransfer;
            }

            var model = new TransferBalanceViewModel()
            {
                Balance = balance,
                MinTransfer = minTransfer,
                TransferFee = transferFee,
            };

            return new GenericResult(true, model);
        }

        [HttpGet]
        public IActionResult GetSponsor(string sponsor)
        {
            try
            {
                if (string.IsNullOrEmpty(sponsor) || sponsor.Length <= 4)
                    return new OkObjectResult(new GenericResult(false, "Sponsor does not exists."));

                var sponsorString = sponsor.GetRawSponsor();

                if (string.IsNullOrEmpty(sponsorString))
                    return new OkObjectResult(new GenericResult(false, "Sponsor does not exists."));

                var userSponsor = _userManager.Users.FirstOrDefault(x => x.Sponsor.Equals(sponsorString));
                if (userSponsor == null)
                    return new OkObjectResult(new GenericResult(false, "Sponsor does not exists."));

                return new OkObjectResult(new GenericResult(true, userSponsor.Email));
            }
            catch (Exception ex)
            {
                _logger.LogError("GetWalletBlance {@0}", ex);
                return new OkObjectResult(new GenericResult(false, ex.Message));
            }
        }
    }
}
