using System;

namespace Core.Application.ViewModels.System
{
    public class WithdrawTokenViewModel
    {
        public decimal Amount { get; set; }
        public string AddressTo { get; set; }
        public string Password { get; set; }
    }
}
