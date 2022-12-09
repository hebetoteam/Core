using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Dapp
{
    public class SaleInitializationParams
    {
        [Required]
        public string Address { get; set; }
        public bool IsDevice { get; set; }
        public string WalletType { get; set; }
        public string AppUserId { get; set; }
        public int TypeId { get;set;}
        public decimal BNBAmount { get; set; }
    }
}