using Core.Data.Entities;
using Core.Infrastructure.Interfaces;

namespace Core.Data.IRepositories
{
    public interface IInvestBotConfigRepository : IRepository<InvestBotConfig, int>
    {
        decimal GetDecimalValueByConfigName(string configName);

        int GetIntValueByConfigName(string configName);

        bool GetBoolValueByConfigName(string configName);
    }
}
