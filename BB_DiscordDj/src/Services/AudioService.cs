using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Audio;
using BB_DiscordDj.src.Entities;

namespace BB_DiscordDj.src.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private readonly AudioPlayer audioPlayer = new AudioPlayer(true);

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

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, SocketMessage msg)
        {
            // Pre-processing things
            await channel.SendMessageAsync("Обрабатываю музяку.");

            //

            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                await audioPlayer.Play(client, channel);
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
            await channel.SendMessageAsync(newOne.ToString() + ", добавлена в конец очереди, пользователем ***" + msg.Author.Username + "***");
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
            await channel.SendMessageAsync("Сейчас заряжено: " + audioPlayer.Song() + ".");
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
                await channel.SendMessageAsync("Шуфлим немножчк");
        }

        public async Task GetList(IMessageChannel channel)
        {

        }
    }
}
