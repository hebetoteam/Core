using Core.Application.Interfaces;
using Core.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using Core.Areas.Admin.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Core.Application.ViewModels.InvestTradingBot;
using System.Threading.Tasks;

namespace Core.Web.Areas.Admin.Controllers
{
    public class InvestTradingController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IInvestTradingBotService _investTradingBotService;
        private readonly IInvestTradingConfigService _investTradingConfigService;

        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<InvestTradingController> _logger;
        private readonly IConfiguration _configuration;
        private static Dictionary<string, Queue<int>> dictProfitCount;
        private static Dictionary<string, decimal> dictProfit;


        public InvestTradingController(
            IUserService userService,
            IInvestTradingBotService investTradingBotService,
            UserManager<AppUser> userManager,
            ILogger<InvestTradingController> logger,
            IConfiguration configuration,
            IInvestTradingConfigService investTradingConfigService)
        {
            _investTradingConfigService = investTradingConfigService;
            _configuration = configuration;
            _logger = logger;
            _userService = userService;
            _investTradingBotService = investTradingBotService;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewBag.ChartInterval = _investTradingBotService.GetChartIntervalValue();
            
            return View();
        }

        public IActionResult Config()
        {
            if (!IsAdmin)
                return Redirect("/login");

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllPaging()
        {
            if (!IsAdmin)
                return Redirect("/login");
            var model = _investTradingConfigService.GetAllPaging(string.Empty, 1, 30);
            return new ObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            if (!IsAdmin)
                return Redirect("/login");
            var model = _investTradingConfigService.GetById(id);

            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveConfig(InvestTradingConfigsViewModel congfigsVm)
        {
            if (!IsAdmin)
                return Redirect("/login");
            _logger.LogInformation($"SaveConfig:{congfigsVm.Name} - {congfigsVm.Value}");
            _investTradingConfigService.SaveConfig(congfigsVm);
            return new OkObjectResult(congfigsVm);

        }

        [HttpGet]
        public IActionResult GetInvestTradingChart()
        {
            var model = "";
            return new OkObjectResult(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetBotTradingPercent()
        {

            bool isProfit = IsProfitReturn();

            decimal rate;
            if (isProfit)
            {
                var profitRate = _investTradingBotService.GetChartProfitRate();
                rate = profitRate;
            }
            else
            {
                var loseRate = _investTradingBotService.GetChartLoseRate();
                rate = loseRate * (-1);
            }

            if (dictProfit.ContainsKey(CurrentUserId.ToString()))
                dictProfit[CurrentUserId.ToString()] = rate;
            else
                dictProfit.Add(CurrentUserId.ToString(), rate);

            InvestProfitRateModel resp = new InvestProfitRateModel
            {
                Rate = rate,
                RateString = $"{rate:N2}%"
            };

            return new ObjectResult(resp);
        }

        bool IsProfitReturn()
        {
            bool isProfit = true;

            if (dictProfitCount == null)
                dictProfitCount = new Dictionary<string, Queue<int>>();
            if (dictProfit == null)
                dictProfit = new Dictionary<string, decimal>();

            var profitCount = _investTradingBotService.GetChartProfitCountValue();

            var loseCount = _investTradingBotService.GetChartLoseCountValue();

            if (profitCount == 0 && loseCount == 0) // random
            {
                var r = new Random();

                int nextInt = r.Next();

                if (nextInt % 2 != 0)
                    return false;
                else
                    return true;

            }

            if (!dictProfitCount.ContainsKey(CurrentUserId.ToString()))
            {
                Queue<int> queue = new Queue<int>();

                dictProfitCount.Add(CurrentUserId.ToString(), queue);
            }

            if (dictProfitCount.ContainsKey(CurrentUserId.ToString()))
            {
                Queue<int> queue = dictProfitCount[CurrentUserId.ToString()];
                if (queue.Count == 0)
                {
                    for (int i = 1; i <= loseCount; i++)
                    {
                        queue.Enqueue(-1 * i);
                    }

                    for (int i = 1; i <= profitCount; i++)
                    {
                        queue.Enqueue(1 * i);
                    }
                }
                if (queue.Count > 0)
                {
                    var dequeue = queue.Dequeue();
                    if (dequeue > 0)
                        isProfit = true;
                    else
                        isProfit = false;

                    dictProfitCount[CurrentUserId.ToString()] = queue;
                }
            }



            return isProfit;
        }

        [AllowAnonymous]
        public async Task<string> ProcessInvestProfitDaily()
        {
            await _investTradingBotService.ProcessDailyProfitHistory();

            return "";
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllBotProfitHistoryPaging(string keyword, int page, int pageSize)
        {
            var model = _investTradingBotService.GetAllProfitHistoryPaging(keyword, page, pageSize);
            return new OkObjectResult(model);
        }
    }
}
