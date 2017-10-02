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

namespace BB_DiscordDj.src.Entities
{
    class AudioPlayer
    {
        private readonly SongQueue queue;
        private Process playerProcess = null;
        private int currentlyPlaying;

        public bool IsShuffled => queue.IsShuffled;

        public AudioPlayer(bool addWelcome)
        {
            if (addWelcome)
            {
                queue = new SongQueue(new PlayerSong(
                    StorageType.Web,
                    "https://www.youtube.com/watch?v=TVo_H8fAN9k"
                    ));
            }
            else
            {
                queue = new SongQueue();
            }
            currentlyPlaying = 0;
        }

        private Stream GetStream()
        {
            return playerProcess.StandardOutput.BaseStream;
        }

        private bool SetupProcess()
        {
            String arguments;
            PlayerSong nextSong = queue.GetSongByIdx(currentlyPlaying);

            switch (nextSong.Storage)
            {
                case StorageType.Local:
                    arguments = $"/C ffmpeg.exe -hide_banner -loglevel panic -i \"{nextSong.Url}\" -ac 2 -f s16le -ar 48000 pipe:1";
                    break;
                case StorageType.Web:
                    arguments = $"/C youtube-dl.exe -o - {nextSong.Url} | ffmpeg.exe -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1";
                    break;
                default:
                    throw new Exception("There is no such Storage Type");
            }

            playerProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });

            return true;
        }

        public async Task Play(IAudioClient client, IMessageChannel channel)
        {
            Stream clientStream = client.CreatePCMStream(AudioApplication.Music);
            if (!queue.HasNext(currentlyPlaying))
            {
                await channel.SendMessageAsync("Конец очереди.");
                return;
            }
            if (playerProcess != null && !playerProcess.HasExited)
            {
                return;
            }
            do
            {
                SetupProcess();
                await GetStream().CopyToAsync(clientStream);
                await clientStream.FlushAsync().ConfigureAwait(false);
                currentlyPlaying++;
            } while (queue.HasNext(currentlyPlaying));

        }

        public async Task AddSong(PlayerSong song)
        {
            queue.AddSong(song);
        }

        public async Task Rewind()
        {
            currentlyPlaying = 0;
        }

        public async Task Stop()
        {
            if (playerProcess != null && !playerProcess.StandardOutput.EndOfStream)
            {
                playerProcess.StandardOutput.Close(); // костыль, обязательно придумаю как по людски сделать, но не в первую очередь
            }

        }

        public async Task Next(IAudioClient cl, IMessageChannel ch)
        {
            await Stop();
            currentlyPlaying++;
            System.Threading.Thread.Sleep(2000);
            await Play(cl, ch);
        }

        public Embed Song()
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(19, 189, 33),
            };

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Сейчас заряжено: ",
                Value = queue.GetSongByIdx(currentlyPlaying).ToString(),
            });

            return builder.Build();
        }

        public async Task Prev(IAudioClient cl, IMessageChannel ch)
        {
            await Stop();
            currentlyPlaying--;
            System.Threading.Thread.Sleep(1900);
            await Play(cl, ch);
        }

        public void Shuffle()
        {
            queue.Shuffle();
        }


        public Embed GetList()
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(221, 34, 235),
            };

            String descriptionString = null;

            for (int i = 0; i < queue.QueueInstance.Count; i++)
            {
                if (i == currentlyPlaying) descriptionString += "__***";
                descriptionString += i + ") ";
                descriptionString += queue.QueueInstance[i].ToString();
                if (i == currentlyPlaying) descriptionString += "***__";
                descriptionString += "\n";
            }

            if (!string.IsNullOrWhiteSpace(descriptionString))
            {
                builder.AddField(x =>
                {
                    x.Name = "Список пиписок:";
                    x.Value = descriptionString;
                    x.IsInline = false;
                });
            }

            return builder.Build();
        }
    }
}
