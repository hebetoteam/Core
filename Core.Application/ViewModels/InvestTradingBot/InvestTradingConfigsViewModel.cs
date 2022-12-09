using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Application.ViewModels.InvestTradingBot
{
    public class InvestTradingConfigsViewModel
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Name { get; set; }
        public string Value { get; set; }
        [StringLength(250)]
        [Required]
        public string Description { get; set; }
    }
}
