using Core.Data.Enums;
using Core.Data.Interfaces;
using Core.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities
{
    [Table("StakingRewards")]
    public class StakingReward : DomainEntity<int>
    {
        [Required]
        public decimal InterestRate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public int StakingId { get; set; }

        [ForeignKey("StakingId")]
        public virtual Staking Staking { set; get; }

        [Required]
        public Guid AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { set; get; }

        public virtual ICollection<StakingAffiliate> StakingAffiliates { get; set; }
    }
}
