using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace live.Models
{
    public class MusicSong
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public int album { get; set; }
        public string artists { get; set; }
        public string lyric { get; set; }
        public string play_url { get; set; }
        public string cover_post { get; set; }
    }
}
