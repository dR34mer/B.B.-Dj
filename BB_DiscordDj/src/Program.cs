using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BB_DiscordDj.src.Services;

namespace BB_DiscordDj
{
    class Program
    {
        static void Main(string[] args)
            //=> Console.WriteLine(AppContext.BaseDirectory);
            => new Program().StartAsync().GetAwaiter().GetResult();

        private IConfigurationRoot _config;

        public async Task StartAsync()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json");
            _config = configBuilder.Build();

            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 100
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Verbose
                }))
                .AddSingleton<AudioService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()
                .AddSingleton<StartupService>()
                .AddSingleton<Random>()
                .AddSingleton(_config);

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<LoggingService>();
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<CommandHandler>();

            await Task.Delay(-1);
        }
    }
}
