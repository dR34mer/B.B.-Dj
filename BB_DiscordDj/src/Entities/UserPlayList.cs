﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;
using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BB_DiscordDj.src.Entities
{
    [Serializable]
    class UserPlayList : SongQueue
    {
        SocketUser socketUser;
        DateTime dateTime;
        String playListName;

        public UserPlayList() : base() { }

        public UserPlayList(SongQueue other,SocketUser user, String plName) : base(other)
        {
            dateTime = DateTime.Now;
            socketUser = user;
            playListName = plName;
        }

        public UserPlayList(String path)
        {
            UserPlayList other = null;
            using(FileStream fs = new FileStream(path, FileMode.Open))
            {
                try
                {
                    other = (UserPlayList)new BinaryFormatter().Deserialize(fs);
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            this.Queue = other.Queue;
            this.OriginalSeq = other.OriginalSeq;
            this.socketUser = other.socketUser;
            this.dateTime = other.dateTime;
            this.playListName = other.playListName;
        }

        public bool TrySave()
        {
            using (FileStream fs = new FileStream(playListName + ".dat", FileMode.Create))
            {
                try
                {
                    new BinaryFormatter().Serialize(fs, this);
                    return true;
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }
    }
}
