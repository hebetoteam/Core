using Core.Data.IRepositories;
using Core.Application.Interfaces;
using Core.Data.Entities;
using Core.Infrastructure.Interfaces;
using Core.Infrastructure.Telegram;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using Core.Utilities.Dtos;
using Core.Application.ViewModels.InvestTradingBot;
using System.Linq;
using Core.Data.Enums;
using Core.Utilities.Extensions;
using System.Threading.Tasks;

namespace Core.Application.Implementation
{
    public class InvestTradingBotService : IInvestTradingBotService
    {
        private readonly IInvestBotConfigRepository _investBotConfigRepository;
        private readonly IInvestProfitHistoryRepository _investProfitHistoryRepository;

        private readonly ILogger<InvestTradingBotService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private readonly UserManager<AppUser> _userManager;
        private readonly IUserService _userService;
        private readonly IBlockChainService _blockChainService;
        private readonly ITicketTransactionService _ticketTransactionService;
        private readonly TelegramBotWrapper _botTelegramService;


        public InvestTradingBotService(IInvestBotConfigRepository investBotConfigRepository,
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            ILogger<InvestTradingBotService> logger,
             IUserService userService,
             IBlockChainService blockChainService,
             TelegramBotWrapper botTelegramService,


             ITicketTransactionService ticketTransactionService,
             IInvestProfitHistoryRepository investProfitHistoryRepository)
        {

            _investProfitHistoryRepository = investProfitHistoryRepository;
            _ticketTransactionService = ticketTransactionService;
            _botTelegramService = botTelegramService;
            _blockChainService = blockChainService;
            _investBotConfigRepository = investBotConfigRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
            _userService = userService;
        }


        public decimal GetDailyProfitRate()
        {
            var minProfit = Convert.ToDouble(_investBotConfigRepository.GetDecimalValueByConfigName("MIN_PROFIT"));

            var maxProfit = Convert.ToDouble(_investBotConfigRepository.GetDecimalValueByConfigName("MAX_PROFIT"));

            Random rand = new();

            var profitToday = (rand.NextDouble() * Math.Abs(maxProfit - minProfit)) + minProfit;

            if (profitToday > maxProfit)
                profitToday = maxProfit;

            return Convert.ToDecimal(Math.Round(profitToday, 2));
        }

        public decimal GetChartLoseRate()
        {
            var minLose = Convert.ToDouble(_investBotConfigRepository.GetDecimalValueByConfigName("CHART_MIN_LOSE"));

            var maxLose = Convert.ToDouble(_investBotConfigRepository.GetDecimalValueByConfigName("CHART_MAX_LOSE"));

            Random rand = new Random();

            var loseRate = (rand.NextDouble() * Math.Abs(maxLose - minLose)) + minLose;

            if (loseRate > maxLose)
            {
                loseRate = maxLose;
            }

            return Convert.ToDecimal(Math.Round(loseRate, 2));
        }

        public decimal GetChartProfitRate()
        {
            var minProfit = Convert.ToDouble(_investBotConfigRepository.GetDecimalValueByConfigName("CHART_MIN_PROFIT"));

            var maxProfit = Convert.ToDouble(_investBotConfigRepository.GetDecimalValueByConfigName("CHART_MAX_PROFIT"));

            Random rand = new Random();

            var profitRate = (rand.NextDouble() * Math.Abs(maxProfit - minProfit)) + minProfit;

            if (profitRate > maxProfit)
            {
                profitRate = maxProfit;
            }

            return Convert.ToDecimal(Math.Round(profitRate, 2));
        }

        public decimal GetMaxProfitConfig()
        {
            var maxProfit = Convert.ToDouble(_investBotConfigRepository.GetDecimalValueByConfigName("CHART_MAX_PROFIT"));
            return Convert.ToDecimal(maxProfit);
        }

        public int GetChartIntervalValue()
        {
            var interval = _investBotConfigRepository.GetIntValueByConfigName("CHART_REFRESH_TIME");

            if (interval == 0)
                interval = 10000;

            return interval;
        }

        public int GetChartLoseCountValue()
        {
            var interval = _investBotConfigRepository.GetIntValueByConfigName("CHART_LOSE_COUNT");

            return interval;
        }

        public int GetChartProfitCountValue()
        {
            var interval = _investBotConfigRepository.GetIntValueByConfigName("CHART_PROFIT_COUNT");

            return interval;
        }


        public async Task ProcessDailyProfitHistory()
        {
            var pendingProfit = _investProfitHistoryRepository.GetPendingInvestProfitHistory();

            if (pendingProfit == null)
            {
                InitialNewInvestProfit();

                return;
            }

            var stopPrice = _blockChainService.GetCurrentPrice("BTC", "USD");
            int type;
            if (pendingProfit.IsWin) // if true , close price > open price => buy ELSE SALE
            {
                if (stopPrice < pendingProfit.StartPrice)
                    type = (int)InvestProfitHistoryType.SELL;
                else
                    type = (int)InvestProfitHistoryType.BUY;
            }
            else
            {
                if (stopPrice < pendingProfit.StartPrice)
                    type = (int)InvestProfitHistoryType.BUY;
                else
                    type = (int)InvestProfitHistoryType.SELL;
            }

            decimal profit = (stopPrice - pendingProfit.StartPrice) * pendingProfit.Margin;

            pendingProfit.ProfitAmount = EnsurePositive(profit);

            pendingProfit.ProfitPercent = EnsurePositive((stopPrice - pendingProfit.StartPrice) / pendingProfit.StartPrice * 100);

            pendingProfit.StopPrice = stopPrice;

            pendingProfit.Type = (InvestProfitHistoryType)type;

            _investProfitHistoryRepository.Update(pendingProfit);

            _unitOfWork.Commit();

            InitialNewInvestProfit();

            await SendChannelTeleGroup(pendingProfit);

        }

        decimal EnsurePositive(decimal source)
        {
            if (source < 0)
            {
                return source * -1;
            }
            return source;
        }

        public InvestProfitHistory InitialNewInvestProfit()
        {
            var btcPrice = _blockChainService.GetCurrentPrice("BTC", "USD");

            var margin = _investBotConfigRepository.GetDecimalValueByConfigName("MARGIN");

            var isWin = _investBotConfigRepository.GetBoolValueByConfigName("IS_WIN");

            var obj = new InvestProfitHistory
            {
                DateCreated = DateTime.UtcNow,
                ProfitPercent = 0,
                Type = InvestProfitHistoryType.PENDING,
                StartPrice = btcPrice,
                Margin = margin,
                IsWin = isWin,
                StopPrice = 0,
                ProfitAmount = 0

            };

            _investProfitHistoryRepository.Add(obj);
            _unitOfWork.Commit();

            return obj;
        }

        public void SendDepositTeleGroup(InvestProfitHistory history)
        {
            var message = _botTelegramService.BuildReportDEPOSITInvestBotMessage
                (history.ProfitPercent,
                history.StartPrice,
                history.StopPrice,
                history.Margin,
                history.Type.GetDescription(),
                history.IsWin,
                history.ProfitAmount,
                history.DateCreated,
                DateTime.UtcNow
                );

            _botTelegramService.SendMessageAsyncWithSendingBalance(TelegramBotActionType.Deposit, message, TelegramBotHelper.OfficialGroup);
        }

        public async Task SendChannelTeleGroup(InvestProfitHistory history)
        {
            var message = _botTelegramService.BuildReportDEPOSITInvestBotMessage
                (history.ProfitPercent,
                history.StartPrice,
                history.StopPrice,
                history.Margin,
                history.Type.GetDescription(),
                history.IsWin,
                history.ProfitAmount,
                history.DateCreated,
                DateTime.UtcNow
                );

            await _botTelegramService.SendMessageAsyncWithSendingBalance(TelegramBotActionType.Deposit, message, TelegramBotHelper.OfficialGroup);

        }


        public PagedResult<InvestTradingBotProfitHistoryViewModel> GetAllProfitHistoryPaging(string keyword, int page, int pageSize)
        {
            var query = _investProfitHistoryRepository.FindAll(x => x.Type != InvestProfitHistoryType.PENDING);

            //if (!string.IsNullOrEmpty(keyword))
            //{
            //    query = query.Where(x => x.Title.Contains(keyword) || x.FullName.Contains(keyword)
            //    || x.Email.Contains(keyword) || x.Phone.Contains(keyword));
            //}

            int totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize)
                .Take(pageSize).Select(x => new InvestTradingBotProfitHistoryViewModel()
                {
                    Id = x.Id,
                    DateCreated = x.DateCreated,
                    StartPrice = x.StartPrice,
                    ProfitPercent = x.ProfitPercent,
                    Remarks = x.Remarks,
                    IsWin = x.IsWin,
                    Margin = x.Margin,
                    StopPrice = x.StopPrice,
                    Type = x.Type,
                    TypeName = x.Type.GetDescription(),
                    ProfitAmount = x.ProfitAmount,
                    Result = x.IsWin ? "WIN" : "LOSE"
                }).ToList();

            var paginationSet = new PagedResult<InvestTradingBotProfitHistoryViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }
    }
}
