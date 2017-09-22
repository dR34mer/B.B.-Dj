using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace BB_DiscordDj.src.Services
{
    class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;

        public StartupService(DiscordSocketClient discord, CommandService commands, IConfigurationRoot config)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
        }

        public async Task StartAsync()
        {
            string discordToken = _config["tokens:discord"];
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Otrkoy json config i vvedi token pzhlst. Sps.");

            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
    }
}
