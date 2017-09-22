using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BB_DiscordDj.src.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly IConfigurationRoot _config;

        public HelpModule(CommandService service, IConfigurationRoot config)
        {
            _service = service;
            _config = config;
        }

        [Command("help"),Alias("пэрэмоха")]
        public async Task HelpAsync()
        {
            string prefix = _config["prefix"];
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "Я хочу, что бы ты мной покомандовал_А"
            };

            foreach(var module in _service.Modules)
            {
                string description = null;
                foreach(var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()} : {cmd.Summary}\n\n";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x => 
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }
            await ReplyAsync("", false, builder.Build());
        }
    }
}
