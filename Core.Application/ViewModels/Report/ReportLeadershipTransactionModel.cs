using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Report
{
    public class ReportLeadershipTransactionModel
    {
        public string Email { get;set;}

        public string TypeRound { get;set;}

        public string Unit { get;set;}

        public DateTime CreatedOn { get; set; }

        public decimal Amount { get;set;}

        public decimal AmountUSDT { get;set;}

        public string Type { get;set;}

    }
}
