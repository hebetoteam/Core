using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Core.Infrastructure.Telegram
{
    public class TelegramBotQueueModel
    {
        public ITelegramBotClient BotClient { get; set; }
        public int Time { get; set; }
        public long FirstCallSendindMessageAt { get; set; }
    }

    public enum TelegramBotActionType
    {
        Withdraw,
        Deposit
    }
}
