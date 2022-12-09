using Core.Data.Enums;
using Core.Infrastructure.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities
{
    [Table("WalletTransactions")]
    public class WalletTransaction : DomainEntity<int>
    {
        public string TransactionHash { get;set;}

        public string AddressFrom { get; set; }

        public string AddressTo { get; set; }

        public decimal Amount { get; set; }

        public decimal Fee { get; set; }

        public decimal FeeAmount { get; set; }

        public decimal AmountReceive { get; set; }

        public WalletTransactionType Type { get; set; }

        public Unit Unit { get; set; }

        public string Remarks { get; set; }

        public DateTime DateCreated { get;set;}

        [Required]
        public Guid AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { set; get; }
    }
}
