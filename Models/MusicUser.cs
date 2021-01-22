using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    /// <summary>
    /// 音乐 user类
    /// </summary>
    public class MusicUser
    {
        /// <summary>
        /// 构造
        /// </summary>
        public MusicUser()
        {
            this.role = 1;
        }

        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string tel { get; set; }
        public string id_no { get; set; }
        public string psd { get; set; }
        public int role { get; set; }//0：管理员 1：用户
    }
}
