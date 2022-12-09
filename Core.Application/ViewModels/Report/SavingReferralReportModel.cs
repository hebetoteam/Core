using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Report
{
    public class SavingReferralReportModel
    {
        public string Email { get;set;}

        public decimal Amount { get;set;}

        public DateTime CreatedOn { get;set;}

        public decimal Rate { get;set;}

    }
}
