using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Transfer
{
    public class TransferTokenViewModel
    {
        public decimal Amount { get; set; }
        public string Sponsor { get; set; }
        public string Password { get; set; }

        public string TokenCode { get; set; }
    }

    public class TransferBalanceViewModel
    {
        public decimal Balance { get; set; }

        public decimal MinTransfer { get; set; }

        public decimal TransferFee { get; set; }
    }
}
