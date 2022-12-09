using Core.Data.Enums;
using Core.Infrastructure.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities
{
    [Table("Airdrops")]
    public class Airdrop : DomainEntity<int>
    {
        public string UserTelegramChannel { get; set; }

        public string UserTelegramCommunity { get; set; }

        public string UserFacebook { get; set; }

        public AirdropStatus Status { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        [Required]
        public Guid AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { set; get; }
    }
}
