using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Report
{
    public class ReportSaleSummaryViewModel
    {
        public decimal TotalSale { get;set;}

        public decimal TotalSeedRound { get;set;}

        public decimal TotalAngelRound { get;set;}

        public decimal TotalPrivateRound { get;set;}

        public decimal TotalPublishRound { get;set;}


        public decimal TotalRefSeedRound { get; set; }

        public decimal TotalRefAngelRound { get; set; }

        public decimal TotalRefPrivateRound { get; set; }

        public decimal TotalRefPublishRound { get; set; }


        public decimal TotalSalebByBNB { get;set;}

        public decimal TotalSwap { get; set; }

        public decimal TotalSalebByBNBAffiliate { get; set; }

        public decimal TotalSwapAffiliate { get; set; }
    }
}
