using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Dapp
{
    public class DappInitializationParams
    {
        public decimal Amount { get; set; }

        public int Timeline { get;set;}

        [Required]
        public string Address { get; set; }
        public bool IsDevice { get; set; }
        public string AppUserId { get; set; }

        public string TokenCode { get;set;}
    }
}
