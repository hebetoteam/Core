using Core.Data.Entities;
using Core.Data.IRepositories;
using Core.Data.EF;
using System.Linq;

namespace Core.Data.EF.Repositories
{

    public class InvestBotConfigRepository : EFRepository<InvestBotConfig, int>, IInvestBotConfigRepository
    {
        public InvestBotConfigRepository(AppDbContext context) : base(context)
        {
        }

        public decimal GetDecimalValueByConfigName(string configName)
        {
            var config = FindAll(x=>x.ConfigName.Equals(configName)).SingleOrDefault();

            if (config==null)
                return 0;

            return decimal.Parse(config.ConfigValue);
        }

        public bool GetBoolValueByConfigName(string configName)
        {
            var config = FindAll(x => x.ConfigName.Equals(configName)).SingleOrDefault();

            if (config == null)
                return false;

            return bool.Parse(config.ConfigValue);
        }

        public int GetIntValueByConfigName(string configName)
        {
            var config = FindAll(x => x.ConfigName.Equals(configName)).SingleOrDefault();

            if (config == null)
                return 0;

            return int.Parse(config.ConfigValue);
        }
    }
}

