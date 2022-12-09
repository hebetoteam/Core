using Core.Data.Enums;
using System;

namespace Core.Application.ViewModels.System
{
    public class WithdrawViewModel
    {
        public Unit Unit { get; set; }
        public decimal Amount { get; set; }
        public string Password { get; set; }
        public string ReceiveAddress { get; set; }
    }
}
