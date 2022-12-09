using Core.Infrastructure.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities
{
    [Table("TokenPriceHistories")]
    public class TokenPriceHistory : DomainEntity<int>
    {
        public decimal Price { get;set;}

        public DateTime DateCreated { get; set; }
    }
}
