using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    public class RecordVideo
    {
        public RecordVideo()
        {
            status = 0;
        }
        [Key]
        public int id { get; set; }
        public int anchor_id { get; set; }
        public int status { get; set; }
        public string category { get; set; }
        public string keyword { get; set; }

        public string url { get; set; }

        public long size { get; set; }

        public string  type { get; set; }
        public string  path { get; set; }
        public string createTime { get; set; }

    }
}


