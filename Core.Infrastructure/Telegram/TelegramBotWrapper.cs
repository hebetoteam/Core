using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Core.Infrastructure.Telegram
{
    public class TelegramBotWrapper
    {
        private static List<TelegramBotQueueModel> _botClientsWithdraw;
        private static List<TelegramBotQueueModel> _botClientsDeposit;
        private readonly ILogger<TelegramBotClient> _logger;

        public TelegramBotWrapper(ILogger<TelegramBotClient> logger)
        {
            _logger = logger;
        }

        public List<TelegramBotQueueModel> GetClientBots(TelegramBotActionType type)
        {
            switch (type)
            {
                case TelegramBotActionType.Withdraw:
                    if (_botClientsWithdraw == null)
                    {
                        _botClientsWithdraw = TelegramBotHelper.Bots;
                    }

                    return _botClientsWithdraw;

                case TelegramBotActionType.Deposit:
                    if (_botClientsDeposit == null)
                    {
                        _botClientsDeposit = TelegramBotHelper.Bots;
                    }
                    return _botClientsDeposit;

                default:
                    return null;
            }
        }

        public Task SendMessageAsyncWithSendingBalance(
            TelegramBotActionType type, string message, ChatId chatId = null, long? groupIndentify = null)
        {
            try
            {
                return ExcuteWithFreeBotFirst(type, (bot) => TrySendMessageAsync(bot, message, chatId));
            }
            catch (Exception e)
            {
                _logger.LogError("BotTelegram: ", e);
                //ignore
                return Task.CompletedTask;
            }
        }

        public async Task<bool> TrySendMessageAsync(
            ITelegramBotClient botClient, string message, ChatId chatId = null, long? groupIndentify = null)
        {
            try
            {
                _logger.LogInformation("Send to Group Chat with message: {0}", message);
                await SendMessageAsync(botClient, message, chatId, groupIndentify);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to send Message: {0}", message);
                return false;
            }
        }

        public async Task SendMessageAsync(
            ITelegramBotClient botClient, string message, ChatId chatId = null, long? groupIndentify = null)
        {
            try
            {
                chatId = chatId ?? new ChatId(groupIndentify.Value);

                await botClient.SendChatActionAsync(chatId, ChatAction.Typing);

                var d = await botClient.SendTextMessageAsync(chatId, message, ParseMode.Html);
            }
            catch (Exception e)
            {

            }
            
        }

        private async Task ExcuteWithFreeBotFirst(
            TelegramBotActionType type, Func<ITelegramBotClient, Task> func)
        {
            var bots = GetClientBots(type);

            var wrapperBot = bots.OrderByDescending(b => b.Time).FirstOrDefault();

            await func(wrapperBot.BotClient);

            if (wrapperBot.Time == 0)
            {
                var ticks = DateTime.UtcNow.Ticks;

                wrapperBot.FirstCallSendindMessageAt = ticks;
            }

            wrapperBot.Time++;

            ResetWrapperBots(bots);
        }

        private void ResetWrapperBots(List<TelegramBotQueueModel> bots)
        {
            var ticks = DateTime.UtcNow.Ticks;

            //if over 60s then reset bot;
            var freeBots = bots.Where(b => ticks - b.FirstCallSendindMessageAt >= 600000000 && b.Time >= 1);

            foreach (var wrapperBot in freeBots)
            {
                wrapperBot.Time = 0;
            }
        }

        public string BuildReportDEPOSITInvestBotMessage(decimal profit,
            decimal startPrice,
            decimal endPrice,
            decimal margin,
            string typeName,
            bool isWin,
            decimal profitAmount,
            DateTime startTime,
            DateTime stopTime)
        {
            string msg = $"✅𝐁𝐎𝐓 𝐓𝐑𝐀𝐃𝐈𝐍𝐆 PROFIT  : \n" +
                $"Report Time : {DateTime.UtcNow:MM-dd-yyyy HH:mm:ss}\n" +
                
                $"Profit Percent: {profit:0.00}% \n" +
                $"Profit Amount : {profitAmount:0.00} USDT\n" +
                $"Start Price : {startPrice:0.00} 𝐔𝐒𝐃𝐓/BTC \n" +
                $"Stop Price : {endPrice:0.00} 𝐔𝐒𝐃𝐓/BTC \n" +
                $"Start Time : {startTime:MM-dd-yyyy HH:mm} \n" +
                $"Stop Time : {stopTime:MM-dd-yyyy HH:mm} \n" +
                $"Margin : {margin:0.00} \n" +
                $"Type : {typeName} \n" +
                $"Result : {(isWin?"WIN":"LOSE")} \n";

            return msg;

        }
    }
}
