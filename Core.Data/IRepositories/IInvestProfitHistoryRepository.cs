using Core.Data.Entities;
using Core.Infrastructure.Interfaces;

namespace Core.Data.IRepositories
{

    public interface IInvestProfitHistoryRepository : IRepository<InvestProfitHistory, int>
    {
        InvestProfitHistory GetPendingInvestProfitHistory();
    }
}
