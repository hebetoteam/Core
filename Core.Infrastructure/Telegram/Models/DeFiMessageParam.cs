using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.ViewModels.BotTelegram
{
    public class DeFiMessageParam
    {
        public string Title { get; set; }
        public decimal AmountBNB { get; set; }
        public decimal AmountUSD { get; set; }
        public decimal AmountToken { get; set; }
        public string Currency { get; set; }
        public string UserWallet { get; set; }
        public string SystemWallet { get; set; }
        public string Email { get; set; }
        public DateTime DepositAt { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
