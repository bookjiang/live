using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    public class User
    {

        public User()
        {

        }
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string tel { get; set; }
        public string id_no { get; set; }
        public int role { get; set; }
        public string psd { get; set; }
    }
}
