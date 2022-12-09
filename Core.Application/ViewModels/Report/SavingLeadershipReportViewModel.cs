using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Report
{
    public class SavingLeadershipReportViewModel
    {
        public Guid AppUserId { get; set; }

        public string Email { get; set; }

        public decimal Staking { get;set;}

        public decimal StakingAffiliate { get; set; }

        public decimal Total { get; set; }
    }
}
