using Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.ViewModels.Staking
{
    public class BuyStakingViewModel
    {
        public Unit Unit { get; set; }
        public StakingPackage Package { get; set; }
        public decimal AmountPayment { get; set; }
    }
}
