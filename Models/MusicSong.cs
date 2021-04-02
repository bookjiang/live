using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace live.Models
{
    public class MusicSong
    {
        public MusicSong()
        {

        }
        //用于添加歌曲的构造函数（不需要id）//
        public MusicSong(string name, string album, string artists, string lyric, string play_url, string cover_post)
        {
            
            this.name = name;
            this.album = album;
            this.artists = artists;
            this.lyric = lyric;
            this.play_url = play_url;
            this.cover_post = cover_post;

        }
        public MusicSong(int id,string name,string album,string artists,string lyric,string  play_url,string cover_post)
        {
            this.id = id;
            this.name = name;
            this.album = album;
            this.artists = artists;
            this.lyric = lyric;
            this.play_url = play_url;
            this.cover_post = cover_post;

        }
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string album { get; set; }
        public string artists { get; set; }  
        public string lyric { get; set; }
        public string play_url { get; set; }
        public string cover_post { get; set; }
    }
}
