using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DemoDotNetBot.Services
{
    public class HandleUpdateService
    {
        private readonly ILogger<ConfigureWebHook> _logger;
        private readonly ITelegramBotClient _botClient;
        public HandleUpdateService(ILogger<ConfigureWebHook> logger, ITelegramBotClient botClient)
        {
            _botClient = botClient;
            _logger = logger;
        }
        public async Task EchoAsync(Update update)
        {
            var handler = update.Type switch
            {
                UpdateType.Message=> BotOnMessageRecieved(update.Message),
                UpdateType.CallbackQuery=> BotOnCallbackRecieved(update.CallbackQuery),
                _=> UnknownUpdateTypeHandler(update)
            };
            try
            {
                await handler;
            }
            catch (Exception ex)
            {
                await HandlerErrorAsync(ex);
            }
        }

        public Task HandlerErrorAsync(Exception ex)
        {
            var ErrorMessage = ex switch
            {
                ApiRequestException apiRequestException => $"API Error:\n {apiRequestException.ErrorCode}",
                _ => ex.ToString()
            };
            _logger.LogInformation($"Error is:{ ErrorMessage }");
            return Task.CompletedTask;
        }

        private Task UnknownUpdateTypeHandler(Update update)
        {
            _logger.LogInformation($"Unknown is:{ update.Type }");
            return Task.CompletedTask;
        }

        private async Task BotOnCallbackRecieved(CallbackQuery callbackQuery)
        {
            await _botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"Bot recieved a {callbackQuery.Data}");
        }

        private async Task BotOnMessageRecieved(Message message)
        {
            _logger.LogInformation($"Message is:{ message.Type }");
            await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Bot recieved a message");
        }
    }
}
