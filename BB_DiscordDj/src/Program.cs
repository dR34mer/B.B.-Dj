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
            => new BotBuilder().StartAsync().GetAwaiter().GetResult();
    }
}
