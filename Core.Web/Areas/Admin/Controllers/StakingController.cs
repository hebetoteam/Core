using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Interfaces;
using Core.Application.ViewModels.BlockChain;
using Core.Application.ViewModels.Common;
using Core.Application.ViewModels.System;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Extensions;
using Core.Services;
using Core.Utilities.Dtos;
using Core.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.Util;

namespace Core.Areas.Admin.Controllers
{
    public class StakingController : BaseController
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IBlockChainService _blockChainService;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly ILogger<StakingController> _logger;
        private readonly AddressUtil _addressUtil = new AddressUtil();
        private readonly IEmailSender _emailSender;
        private readonly IStakingService _stakingService;
        private readonly IStakingAffiliateService _stakingAffiliateService;
        private readonly IStakingRewardService _stakingRewardService;
        private readonly IConfiguration _configuration;

        public StakingController(
            IConfiguration configuration,
            IStakingRewardService stakingRewardService,
            IStakingAffiliateService stakingAffiliateService,
            IStakingService stakingService,
            IWalletTransactionService walletTransactionService,
            ILogger<StakingController> logger,
            UserManager<AppUser> userManager,
            IUserService userService,
            IEmailSender emailSender,
            IBlockChainService blockChainService)
        {
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
            _emailSender = emailSender;
            _configuration = configuration;
            _blockChainService = blockChainService;
            _stakingService = stakingService;
            _stakingAffiliateService = stakingAffiliateService;
            _stakingRewardService = stakingRewardService;
            _walletTransactionService = walletTransactionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var enumStakingPackages = ((StakingPackage[])Enum
                .GetValues(typeof(StakingPackage)))
                .Select(c => new EnumModel()
                {
                    Value = (int)c,
                    Name = c.GetDescription()
                }).ToList();

            ViewBag.PackageType = new SelectList(enumStakingPackages, "Value", "Name");

            return View();
        }

        [HttpGet]
        public IActionResult History()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            string appUserId = string.Empty;

            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() == "customer")
                appUserId = User.GetSpecificClaim("UserId");

            var model = _stakingService.GetAllPaging(keyword, appUserId, null, null, 0, page, pageSize);

            return new OkObjectResult(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Leaderboard()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetLeaderboardAllPaging(string keyword, int page, int pageSize)
        {
            var model = _stakingService.GetAllPaging(keyword, "", null, null, 0, page, pageSize);

            return new OkObjectResult(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetLeaderboard(int top)
        {
            var model = _stakingService.GetLeaderboard(top);

            return new OkObjectResult(model);
        }


        [HttpGet]
        public IActionResult Profit()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetProfitAllPaging(string keyword, int page, int pageSize)
        {
            string appUserId = string.Empty;

            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() == "customer")
                appUserId = User.GetSpecificClaim("UserId");

            var model = _stakingRewardService.GetAllPaging(keyword, appUserId, page, pageSize);

            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult Affiliate()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAffiliateAllPaging(string keyword, int page, int pageSize)
        {
            string appUserId = string.Empty;

            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() == "customer")
                appUserId = User.GetSpecificClaim("UserId");

            var model = _stakingAffiliateService.GetAllPaging(keyword, appUserId, page, pageSize);

            return new OkObjectResult(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyStaking(string modelJson)
        {
            _logger.LogInformation(modelJson);
            var stakingResult = await _stakingService.StakingToken(modelJson, CurrentUserId.ToString());
            return new OkObjectResult(stakingResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletBlance()
        {
            var userId = User.GetSpecificClaim("UserId");

            var appUser = await _userService.GetById(userId);

            var model = new WalletViewModel()
            {
                USDTAmount = appUser.USDTAmount,
                HBTAmount = appUser.HBTAmount
            };

            return new OkObjectResult(model);
        }

        public IActionResult StakingTransaction()
        {
            if (!IsAdmin)
                RedirectHome();


            return View();
        }

        [AllowAnonymous]
        public IActionResult ProcessDailyStakingProfit()
        {
            _logger.LogInformation("ProcessDailyStakingProfit - Begin");

            _stakingService.PaymentProfit();

            _logger.LogInformation("ProcessDailyStakingProfit - Complete");

            return new OkObjectResult(true);
        }
    }
}