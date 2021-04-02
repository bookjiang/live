using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    public class Admin
    {
        public Admin()
        {


        }//test
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string tel { get; set; }
        public int role { get; set; }
        public string psd { get; set; }
    }
}
