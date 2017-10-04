using System.Threading.Tasks;
using Discord;
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
        [Summary("Начинает воспроизведение с текущей позиции в плейлисте или с указанной после команды. \nПрим.: !play 11")]
        public async Task PlayCmd(int position = -42)
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, Context.Message as SocketMessage, position);
        }

        [Command("queue", RunMode = RunMode.Async)]
        [Summary("Добавляет музыку в конец очереди. " +
            " \nПрим. !queue www.utub.cum/somesong [\"О кумысе\"] [Казах] [\"Плейлист Казахстана\"]" +
            "\nДоступные сайты:" +
            "\n<https://github.com/rg3/youtube-dl/blob/master/docs/supportedsites.md>")]
        public async Task QueueCmd(string song, string songName = "null", string authorName = "null", string albumName = "null")
        {
            await _service.AddSong(Context.Channel, Context.Message as SocketMessage, song, songName, authorName, albumName);
        }

        [Command("rewind", RunMode = RunMode.Async)]
        [Summary("Отматывает касету в начало плейлиста.")]
        public async Task RewindCmd()
        {
            await _service.Rewind(Context.Channel);
        }

        [Command("stop", RunMode = RunMode.Async)]
        [Summary("Че-то сложное")]
        public async Task StopCmd()
        {
            await _service.Stop(Context.Guild);
        }

        [Command("next", RunMode = RunMode.Async)]
        [Summary("Включает следующий трэк.")]
        public async Task NextCmd()
        {
            await _service.Next(Context.Guild, Context.Channel);
        }

        [Command("prev", RunMode = RunMode.Async)]
        [Summary("Включает предыдущий трэк.")]
        public async Task PrevCmd()
        {
            await _service.Prev(Context.Guild, Context.Channel);
        }

        [Command("song", RunMode = RunMode.Async)]
        [Summary("Напишет информацию о текущем трэке. Если это не Казах.")]
        public async Task SongCmd()
        {
            await _service.Song(Context.Channel);
        }

        [Command("shuffle", RunMode = RunMode.Async)]
        [Summary("Включает/выключает случайный порядок воспроизведения.")]
        public async Task ShuffleCmd()
        {
            await _service.Shuffle(Context.Channel);
        }

        [Command("list", RunMode = RunMode.Async)]
        [Summary("Текущая обойма плеера.")]
        public async Task ListCmd()
        {
            await _service.GetList(Context.Channel);
        }

        [Command("clear", RunMode = RunMode.Async)]
        [Summary("Очистка очереди.")]
        public async Task ClearCmd()
        {
            await _service.ClearQue(Context.Channel);
        }

        [Command("save", RunMode = RunMode.Async)]
        [Summary("Сохраняет текущую очередь, как плейлист, после его можно загрузить с помощью !load. \n Прим.: !save \"Сраный казах\"")]
        public async Task SaveCmd(string playListName)
        {
            await _service.SavePlayList(Context.Channel, Context.Message as SocketMessage, playListName);
        }

        [Command("load", RunMode = RunMode.Async)]
        [Summary("Диджей заряжай пластинки, авария идет в могилки. Подгружает заранее сохраненный плейлист. \n Прим.: !load \"Жанна Фрисби\"")]
        public async Task LoadCmd(string playListName)
        {
            await _service.LoadPlayList(Context.Channel, playListName);
        }
    }
}
