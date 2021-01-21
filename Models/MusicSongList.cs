using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    public class MusicSongList
    {
        public MusicSongList()
        {

        }

        public int id { get; set; }
        public string name { get; set; }
        public string describe { get; set; }
        public string cover_url { get; set; }
        public int user_id { get; set; }

        public int status { get; set; }
        
        
    }
}
