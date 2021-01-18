using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    public class Comment
    {
        public Comment()
        {

        }
        [Key]
        public int id { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public int video_id { get; set; }

        public string content { get; set; }
        public string video_name { get; set; }


    }
}
