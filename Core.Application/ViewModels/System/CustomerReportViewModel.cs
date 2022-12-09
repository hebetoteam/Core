using Core.Data.Enums;
using System;
using System.Collections.Generic;

namespace Core.Application.ViewModels.System
{
    public class CustomerReportViewModel
    {
        public CustomerReportViewModel()
        {
            UserRefers = new List<AppUserViewModel>();
        }

        public int MyLevel { get; set; }
        public bool IsActived { get; set; }
        public string CustomerId { get; set; }
        public decimal Balance { get; set; }
        public decimal InvestmentMoney { get; set; }
        public int TotalPeople { get; set; }
        public decimal TotalUplineFund { get; set; }
        public decimal RewardPoint { get; set; }
        public List<AppUserViewModel> UserRefers { get; set; }
    }
}
