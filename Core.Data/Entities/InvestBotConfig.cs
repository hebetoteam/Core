using Core.Infrastructure.SharedKernel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities
{
    [Table("InvestBotConfig")]
    public class InvestBotConfig : DomainEntity<int>
    {
        public string ConfigName { get; set; }

        public string ConfigValue { get; set; }

        public string Description { get; set; }
    }
}
