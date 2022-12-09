using Core.Application.ViewModels.System;
using Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Application.ViewModels.Report
{
    public class ReportViewModel
    {
        public ReportViewModel()
        {
        }

        public int TotalMember { get; set; }
        public int TodayMember { get; set; }
        public int TotalMemberVerifyEmail { get; set; }
        public int TotalMemberInVerifyEmail { get; set; }

        public decimal TotalBNBDeposit { get; set; }
        public decimal TotalBNBWithdraw { get; set; }

        public decimal TotalUSDTDeposit { get; set; }
        public decimal TotalUSDTWithdraw { get; set; }

        public decimal TotalSavingUSDT { get; set; }

        public decimal TotalExchangeSaleUSDT { get;set;}



        public decimal TotalTransferBNB { get;set;}

        public decimal TotalTransferUSDT { get; set; }

        public decimal TotalReceivedTransferBNB { get; set; }

        public decimal TotalReceivedTransferUSDT { get; set; }
    }
}
