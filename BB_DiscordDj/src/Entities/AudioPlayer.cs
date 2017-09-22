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
        }

        private Stream GetStream()
        {
            return playerProcess.StandardOutput.BaseStream;
        }

        private bool SetupProcess()
        {
            String arguments;
            PlayerSong nextSong = queue.Next();

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
            if (!queue.HasNext())
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
            } while (queue.HasNext());

        }

        public async Task AddSong(PlayerSong song)
        {
            queue.AddSong(song);
        }

        public async Task Rewind()
        {
            queue.Rewind();
        }

        public async Task Stop()
        {
            if (playerProcess != null && !playerProcess.StandardOutput.EndOfStream)
            {
                playerProcess.StandardOutput.Close();
            }

        }

        public async Task Next(IAudioClient cl, IMessageChannel ch)
        {
            await Stop();
            System.Threading.Thread.Sleep(2000);
            await Play(cl,ch);
        }

        public string Song()
        {
            return queue.GetSong().ToString();
        }
    }
}
