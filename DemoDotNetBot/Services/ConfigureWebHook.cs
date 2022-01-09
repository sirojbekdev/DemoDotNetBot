using DemoDotNetBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace DemoDotNetBot.Services
{
    public class ConfigureWebHook : IHostedService
    {
        private readonly BotConfiguration _botConfig;
        private readonly ILogger<ConfigureWebHook> _logger;
        private readonly IServiceProvider _serviceProvider;
        public ConfigureWebHook(ILogger<ConfigureWebHook> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var webHookAddress = $@"{_botConfig.HostAddress}/bot/{_botConfig.Token}";

            _logger.LogInformation("Setting WebHook!");

            await botClient.SendTextMessageAsync(chatId: 1062074085, text: "Bot Starting");

            await botClient.SetWebhookAsync(url: webHookAddress, allowedUpdates: Array.Empty<UpdateType>(),cancellationToken:cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            _logger.LogInformation("Stopping WebHook!");

            await botClient.SendTextMessageAsync(chatId: 1062074085, text: "Bot Stopping");

        }
    }
}
