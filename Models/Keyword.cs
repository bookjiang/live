using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    public class Keyword
    {
        public Keyword()
        {

        }

        [Key]
        public int id { get; set; }
        public string keyword { get; set; }
        public string operation { get; set; }
    }
}
