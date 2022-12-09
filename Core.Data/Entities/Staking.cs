using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Enums;
using Core.Data.Interfaces;
using Core.Infrastructure.SharedKernel;

namespace Core.Data.Entities
{
    [Table("Stakings")]
    public class Staking : DomainEntity<int>
    {
        [Required]
        public StakingTimeLine TimeLine { get; set; }

        [Required]
        public StakingPackage Package { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        [Required]
        public decimal StakingAmount { get; set; }

        [Required]
        public int StakingTimes { get; set; }

        [Required]
        public string PublishKey { get; set; }

        [Required]
        public string PrivateKey { get; set; }

        public string TransactionHash { get; set; }


        [Required]
        public decimal ReceiveAmount { get; set; }

        [Required]
        public int ReceiveTimes { get; set; }

        [Required]
        public StakingType Type { get; set; }

        public DateTime ReceiveLatest { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public Guid AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { set; get; }

        public virtual ICollection<StakingReward> StakingRewards { get; set; }
    }
}
