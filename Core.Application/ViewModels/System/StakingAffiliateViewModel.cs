using Core.Data.Enums;
using System;

namespace Core.Application.ViewModels.System
{
    public class StakingAffiliateViewModel
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime DateCreated { get; set; }

        public string Remarks { get; set; }

        public int StakingRewardId { get; set; }

        public  StakingRewardViewModel StakingReward { set; get; }

        public string Sponsor { get; set; }

        public string AppUserName { get; set; }

        public Guid AppUserId { get; set; }

        public AppUserViewModel AppUser { set; get; }
    }
}
