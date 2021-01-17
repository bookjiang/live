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

        }
        [Key]
        public int id { get; set; }
        public int auchor_id { get; set; }
        public int status { get; set; }
        public string catagory { get; set; }
        public string keyword { get; set; }

        public string url { get; set; }

    }
}


