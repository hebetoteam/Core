using BeCoreApp.Data.Entities;
using BeCoreApp.Data.Enums;
using Core.Data.Enums;
using Core.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities
{
    [Table("AppUsers")]
    public class AppUser : IdentityUser<Guid>, IDateTracking, ISwitchable
    {
        public string Sponsor { get; set; }

        public Guid? ReferralId { get; set; }

        public string PublishKey { get; set; }

        public string PrivateKey { get; set; }

        public decimal USDTAmount { get; set; }

        public decimal HBTAmount { get; set; }

        public decimal StakingAmount { get; set; }

        public decimal StakingAffiliateAmount { get; set; }

        public StakingLevel StakingLevel { get; set; }

        public bool IsSystem { get; set; } = false;

        public Status Status { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public string ByCreated { get; set; }

        public string ByModified { get; set; }

        public virtual ICollection<Support> Supports { set; get; }
        public virtual ICollection<TicketTransaction> TicketTransactions { set; get; }
        public virtual ICollection<WalletTransaction> WalletTransactions { set; get; }
        public virtual ICollection<Airdrop> Airdrops { set; get; }

        public bool IsShowOff { get;set;}
    }
}
