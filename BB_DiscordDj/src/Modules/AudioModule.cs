﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Discord.Commands;
using BB_DiscordDj.src.Services;

namespace BB_DiscordDj.src.Modules
{

    public class AudioModule : ModuleBase<ICommandContext>
    {
        private readonly AudioService _service;

        public AudioModule(AudioService service)
        {
            _service = service;
        }

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Присоединяет бота к вашему голосовому каналу.")]
        public async Task JoinCmd()
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }

        [Command("leave", RunMode = RunMode.Async)]
        [Summary("Прогоняет бота из канала. (Например за Казаха)")]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Начинает воспроизведение с текущей позиции в плейлисте.")]
        public async Task PlayCmd()
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, Context.Message as SocketMessage);
        }

        [Command("queue",RunMode=RunMode.Async)]
        [Summary("Добавляет музыку в конец очереди. \nПараметры в [] - опциональные, чисто информационные." +
            "\nПотом замутим, что бы инфа сохранялась (прям плейлистами), красивше буит." +
            " \nПрим. !queue www.utub.com/somesong [\"О кумысе\"] [Казах] [\"Плейлист Казахстана\"]" +
            "\nДоступные сайты можно посмотреть на гитхабе автора парсера" +
            "\n<https://github.com/rg3/youtube-dl/blob/master/docs/supportedsites.md>")]
        public async Task QueueCmd(string song, string songName = "null", string authorName = "null", string albumName = "null")
        {
            await _service.AddSong(Context.Channel, Context.Message as SocketMessage, song, songName, authorName, albumName);
        }

        [Command("rewind",RunMode=RunMode.Async)]
        [Summary("Отматывает касету в начало плейлиста.")]
        public async Task RewindCmd()
        {
            await _service.Rewind(Context.Channel);
        }

        [Command("stop",RunMode=RunMode.Async)]
        [Summary("Че-то сложное")]
        public async Task StopCmd()
        {
            await _service.Stop(Context.Guild);
        }

        [Command("next",RunMode=RunMode.Async)]
        [Summary("Включает следующий трэк.")]
        public async Task NextCmd()
        {
            await _service.Next(Context.Guild, Context.Channel);
        }

        [Command("song",RunMode=RunMode.Async)]
        [Summary("Напишет информацию о текущем трэке. Если это не Казах.")]
        public async Task SongCmd()
        {
            await _service.Song(Context.Channel);
        }
    }
}