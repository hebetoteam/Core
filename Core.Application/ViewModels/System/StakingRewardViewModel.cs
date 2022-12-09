using Core.Data.Enums;
using System;
using System.Collections.Generic;

namespace Core.Application.ViewModels.System
{
    public class StakingRewardViewModel
    {
        public StakingRewardViewModel()
        {
            StakingAffiliates = new List<StakingAffiliateViewModel>();
        }

        public int Id { get; set; }

        public decimal InterestRate { get; set; }

        public decimal Amount { get; set; }

        public DateTime DateCreated { get; set; }

        public int StakingId { get; set; }

        public StakingPackage Package { get; set; }
        public string PackageName { get; set; }
        public decimal StakingAmount { get; set; }

        public string Sponsor { get; set; }

        public string AppUserName { get; set; }

        public Guid AppUserId { get; set; }

        public StakingViewModel Staking { set; get; }
        public AppUserViewModel AppUser { set; get; }
        public List<StakingAffiliateViewModel> StakingAffiliates { get; set; }
    }
}
