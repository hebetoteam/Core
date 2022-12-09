using Core.Application.ViewModels.InvestTradingBot;
using Core.Data.Entities;
using Core.Utilities.Dtos;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IInvestTradingBotService
    {
        decimal GetDailyProfitRate();

        decimal GetChartProfitRate();

        decimal GetChartLoseRate();

        int GetChartIntervalValue();

        int GetChartProfitCountValue();

        int GetChartLoseCountValue();

        decimal GetMaxProfitConfig();

        PagedResult<InvestTradingBotProfitHistoryViewModel> GetAllProfitHistoryPaging(string keyword, int page, int pageSize);

        Task ProcessDailyProfitHistory();

        Task SendChannelTeleGroup(InvestProfitHistory history);

    }
}
