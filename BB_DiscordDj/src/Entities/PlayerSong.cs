﻿using System;

namespace BB_DiscordDj.src.Entities
{
    [Serializable]
    class PlayerSong
    {
        public StorageType Storage { get; }
        public String Author { get; }
        public String Album { get; }
        public String Name { get; }
        public String Url { get; }

        public PlayerSong(StorageType Storage, String Url, String Name = "null", String Author = "null", String Album = "null")
        {
            this.Storage = Storage;
            this.Url = Url;
            this.Name = Name;
            this.Author = Author;
            this.Album = Album;
        }

        public override string ToString()
        {
            String album;
            album = (Album == "null") ? "" : (" из альбома - " + Album + ",");
            return $"**{Author}**:**{Name}** {album} [источник]({Url})";
        }
    }
}
