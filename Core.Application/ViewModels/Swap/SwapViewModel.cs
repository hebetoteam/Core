using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Swap
{
    public class SwapViewModel
    {
        public string Password { get; set; }
        public decimal Amount { get; set; }
    }

    public class SwapBalanceViewModel
    {
        public decimal Balance { get; set; }

        public decimal MinSwap { get; set; }

        public decimal SwapFee { get; set; }

        public decimal TokenPrice { get;set;}
    }
}
