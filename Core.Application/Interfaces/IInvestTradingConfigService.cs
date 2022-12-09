using Core.Application.ViewModels.InvestTradingBot;
using Core.Utilities.Dtos;

namespace Core.Application.Interfaces
{
    public interface IInvestTradingConfigService
    {
        PagedResult<InvestTradingConfigsViewModel> GetAllPaging(string keyword, int pageIndex, int pageSize);

        InvestTradingConfigsViewModel GetById(int id);

        void SaveConfig(InvestTradingConfigsViewModel model);
    }
}
