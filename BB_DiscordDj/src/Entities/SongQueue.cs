﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BB_DiscordDj.src.Entities
{
    [DataContract]
    [Serializable]
    class SongQueue
    {
        [DataMember]
        protected List<PlayerSong> Queue;
        [DataMember]
        protected List<PlayerSong> OriginalSeq;

        public List<PlayerSong> QueueInstance
        {
            get => Queue;
        }

        public bool IsShuffled { get => (OriginalSeq == null) ? false : true; }

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

        public SongQueue(SongQueue other)
        {
            this.Queue = other.Queue;
            this.OriginalSeq = other.OriginalSeq;
        }

        public void AddSong(PlayerSong song)
        {
            Queue.Add(song);
            if (OriginalSeq != null)
                OriginalSeq.Add(song);
        }

        public PlayerSong GetSongByIdx(int idx)
        {
            return Queue[idx];
        }

        public bool HasNext(int idx)
        {
            return idx < Queue.Count ? true : false;
        }

        public void Shuffle()
        {
            if (OriginalSeq == null)
            {
                OriginalSeq = new List<PlayerSong>(Queue);
                Queue.Shuffle();
            }
            else
            {
                Queue = OriginalSeq;
                OriginalSeq = null;
            }
        }

        public void CleanQue()
        {
            this.Queue = new List<PlayerSong>();
            this.OriginalSeq = null;
        }

        public void RemoveAt(int position)
        {
            PlayerSong temp = Queue[position];
            Queue.Remove(temp);
            if(OriginalSeq != null)
                OriginalSeq.Remove(temp);
        }
       
    }

    static class ListExtension
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = rng.Next(i, list.Count);
                T value = list[i];
                list[i] = list[j];
                list[j] = value;
            }
        }
    }
}
