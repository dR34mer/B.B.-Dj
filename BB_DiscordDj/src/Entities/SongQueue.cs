using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB_DiscordDj.src.Entities
{
    class SongQueue
    {
        List<PlayerSong> Queue;

        public SongQueue()
        {
            Queue = new List<PlayerSong>();
        }

        public SongQueue(PlayerSong welcome)
        {
            Queue = new List<PlayerSong>
            {
                welcome
            };
        }

        public void AddSong(PlayerSong song)
        {
            Queue.Add(song);
        }

        public PlayerSong GetSongByIdx(int idx)
        {
            return Queue[idx];
        }

        public bool HasNext(int idx)
        {
            return idx < Queue.Count ? true : false;
        }
    }
}
