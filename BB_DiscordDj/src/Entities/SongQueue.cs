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
        private int currentlyPlaying;

        public SongQueue()
        {
            Queue = new List<PlayerSong>();
            currentlyPlaying = 0;
        }

        public SongQueue(PlayerSong welcome)
        {
            Queue = new List<PlayerSong>
            {
                welcome
            };
            currentlyPlaying = 0;
        }

        public void AddSong(PlayerSong song)
        {
            Queue.Add(song);
        }

        public PlayerSong GetSongByIdx(int idx)
        {
            return Queue[idx];
        }

        public PlayerSong Next()
        {
            if (currentlyPlaying < Queue.Count)
            {
                return Queue[currentlyPlaying++];
            }
            throw new Exception("Queue is at its end");
        }

        public bool HasNext()
        {
            return currentlyPlaying < Queue.Count;
        }
        public void Rewind()
        {
            this.currentlyPlaying = 0;
        }

        public PlayerSong GetSong()
        {
            return Queue[currentlyPlaying];
        }
    }
}
