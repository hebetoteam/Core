using Core.Application.Interfaces;
using Core.Application.ViewModels.Report;
using Core.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Areas.Admin.Controllers
{
    public class ReportController : BaseController
    {
        private static Dictionary<string, List<Guid>> _cacheDict;
        private static Dictionary<string, DateTime> _cacheTimeDict;
        private readonly object cachelock = new();

        private readonly UserManager<AppUser> _userManager;
        private readonly IReportService _reportService;
        private readonly IStakingService _stakingService;
        private readonly ITokenPriceHistoryService _tokenPriceHistoryService;
        private readonly ILogger<ReportController> _logger;
        private readonly IConfigService _configService;

        public ReportController(
            IConfigService configService,
            IReportService reportService,
            UserManager<AppUser> userManager,
            ILogger<ReportController> logger,
            ITokenPriceHistoryService tokenPriceHistoryService,
            IStakingService stakingService)
        {
            _configService = configService;
            _stakingService = stakingService;
            _logger = logger;
            _userManager = userManager;
            _reportService = reportService;
            _cacheTimeDict ??= new Dictionary<string, DateTime>();
            _cacheDict ??= new Dictionary<string, List<Guid>>();
            _tokenPriceHistoryService = tokenPriceHistoryService;
        }

        public IActionResult Index()
        {
            if (!IsAdmin)
                return Redirect("/login");

            return View();
        }



        [HttpGet]
        public IActionResult GetReportInfo(string fromDate, string toDate)
        {
            var model = _reportService.GetReportInfo(fromDate, toDate);
            return new OkObjectResult(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllTokenPrice()
        {
            var model = _tokenPriceHistoryService.GetAllPaging();
            return new OkObjectResult(new
            {
                Prices = model.Results.Select(x => x.Price).ToArray(),
                Dates = model.Results.Select(x => x.DateCreated.ToString("MM/dd/yy")).ToArray(),
                CurrentPrice = _configService.GetTokenPrice()
            });
        }

        public async Task<IActionResult> Staking()
        {
            var appUser = await _userManager.FindByIdAsync(CurrentUserId.ToString());

            var levelReferral = await _stakingService.ReferralStakingLevelSummary(CurrentUserId.ToString());

            var stakingMaxout = _stakingService.GetMaxout(CurrentUserId.ToString());

            return View(new ReportStakingSummaryViewModel
            {
                StakingAffiliateAmount = appUser.StakingAffiliateAmount,
                StakingAmount = appUser.StakingAmount,
                StakingMaxout = stakingMaxout,
                ReferralLevel = levelReferral
            });
        }


        public IActionResult SaleHistories()
        {
            if (!IsAdmin)
                return Redirect("/login");

            return View();
        }

        public IActionResult Leadership()
        {
            if (!IsLeader)
                return Redirect("/");

            return View();
        }


    }
}
