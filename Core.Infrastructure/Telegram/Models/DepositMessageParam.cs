using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.ViewModels.BotTelegram
{
    public class DepositMessageParam
    {
        public string Email { get; set; }
        public int? UserId { get; set; }
        public string SponsorEmail { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string WalletFrom { get; set; }
        public string WalletTo { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
