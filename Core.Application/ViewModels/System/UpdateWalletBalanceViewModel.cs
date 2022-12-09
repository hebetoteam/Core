using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.System
{
    public class UpdateWalletBalanceViewModel
    {
        public decimal Amount { get; set; }
        public string WalletType { get; set; }
        public string UserId { get; set; }
        public int ActionType { get; set; }
    }
}
