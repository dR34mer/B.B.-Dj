using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace BB_DiscordDj.src.Entities
{
    [Serializable]
    [DataContract]
    class UserPlayList : SongQueue
    {
        [DataMember]
        String socketUser;
        [DataMember]
        DateTime creationDate;
        [DataMember]
        String playListName;

        public UserPlayList() : base() { }

        public UserPlayList(SongQueue other,String user, String plName) : base(other)
        {
            creationDate = DateTime.Now;
            socketUser = user;
            playListName = plName;
        }

        public UserPlayList(String path)
        {
            UserPlayList other = null;
            using(FileStream fs = new FileStream(Directory.GetCurrentDirectory() + @"\data\" + path + ".json", FileMode.Open))
            {
                try
                {
                    other = (UserPlayList)new DataContractJsonSerializer(typeof(UserPlayList)).ReadObject(fs);
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            this.Queue = other.Queue;
            this.OriginalSeq = other.OriginalSeq;
            this.socketUser = other.socketUser;
            this.creationDate = other.creationDate;
            this.playListName = other.playListName;
        }

        public bool TrySave()
        {
            String workingFolder = Directory.GetCurrentDirectory() + @"\data\";
            if (!Directory.Exists(workingFolder))
            {
                Directory.CreateDirectory(workingFolder);
            }
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(UserPlayList));
            using (FileStream fs2 = new FileStream(workingFolder + playListName + ".json", FileMode.Create))
            {
                try
                {
                    jsonFormatter.WriteObject(fs2, this);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }

    }
}
