using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Discord;
using Discord.WebSocket;
using Discord.Audio;
using BB_DiscordDj.src.Entities;
using Microsoft.Extensions.Configuration;

namespace BB_DiscordDj.src.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private readonly IConfigurationRoot _config;
        private readonly AudioPlayer audioPlayer = new AudioPlayer(true);

        public AudioService(IConfigurationRoot config)
        {
            _config = config;
            audioPlayer.LoadConfig(_config);
        }

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {

            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
                return;
            if (target.Guild.Id != guild.Id)
                return;

            var audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
                await Console.Out.WriteLineAsync($"Connected to voice on {guild.Name}.");
        }

        public async Task LeaveAudio(IGuild guild)
        {
            if (ConnectedChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                await client.StopAsync();
                await Console.Out.WriteLineAsync($"Disconnected from voice on {guild.Name}.");
            }

        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, SocketMessage msg, int position)
        {
            // Pre-processing things
            await channel.SendMessageAsync("Обрабатываю музяку.");

            //

            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                if (position < 0)
                    await audioPlayer.Play(client, channel);
                else
                    await audioPlayer.Play(client, channel, position);
            }
        }

        public async Task AddSong(IMessageChannel channel, SocketMessage msg, String Url, String Name = "null", String Author = "null", String Album = "null")
        {
            await msg.DeleteAsync(new RequestOptions
            {
                RetryMode = RetryMode.AlwaysRetry
            });
            PlayerSong newOne = new PlayerSong(StorageType.Web, Url, Name, Author, Album);
            await audioPlayer.AddSong(newOne);

            var builder = new EmbedBuilder() { Color = new Color(224, 123, 22) };

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = Name = msg.Author.Username + " добавил в конец очереди: ",
                Value = newOne.ToString(),
                IsInline = false,
            });

            await channel.SendMessageAsync("", false, builder.Build());
        }

        public async Task Rewind(IMessageChannel channel)
        {
            await audioPlayer.Rewind();
            await channel.SendMessageAsync("Наматываем касету на карандаш...");
        }

        public async Task Stop(IGuild guild)
        {
            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                await audioPlayer.Stop();
            }
        }

        public async Task Next(IGuild guild, IMessageChannel channel)
        {
            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                await audioPlayer.Next(client, channel);
            }
        }

        public async Task Song(IMessageChannel channel)
        {
            await channel.SendMessageAsync("", false, audioPlayer.Song());
        }

        public async Task Prev(IGuild guild, IMessageChannel channel)
        {
            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                await audioPlayer.Prev(client, channel);
            }
        }

        public async Task Shuffle(IMessageChannel channel)
        {
            audioPlayer.Shuffle();
            if (!audioPlayer.IsShuffled)
                await channel.SendMessageAsync("Порядок восстановлен.");
            else
                await channel.SendMessageAsync("Шуфлим немножчк.");
        }

        public async Task GetList(IMessageChannel channel)
        {
            await channel.SendMessageAsync("", false, audioPlayer.GetList());
        }

        public async Task ClearQue(IMessageChannel channel)
        {
            audioPlayer.ClearQue();
            await channel.SendMessageAsync("Чисто, как орбит чистый плей-лист");
        }

        public async Task SavePlayList(IMessageChannel channel, SocketMessage msg, String playListName)
        {
            if (audioPlayer.TrySavePlayList(msg.Author as SocketUser, playListName))
            {
                await channel.SendMessageAsync("Понял, принял.");
            }
            else
            {
                await channel.SendMessageAsync("Не понял. Стэк исключения в консоли.");
            }
        }

        public async Task LoadPlayList(IMessageChannel channel, String playListName)
        {
            audioPlayer.TryLoadPlayList(playListName);
            await channel.SendMessageAsync("Подгружено.");
        }
    }
}
