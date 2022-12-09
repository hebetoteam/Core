using Core.Data.Enums;
using Core.Data.Interfaces;
using Core.Infrastructure.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities
{
    [Table("StakingAffiliates")]
    public class StakingAffiliate : DomainEntity<int>
    {

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public string Remarks { get; set; }

        [Required]
        public int StakingRewardId { get; set; }

        [ForeignKey("StakingRewardId")]
        public virtual StakingReward StakingReward { set; get; }

        [Required]
        public Guid AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { set; get; }
    }
}
