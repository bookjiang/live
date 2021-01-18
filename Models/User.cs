using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    /// <summary>
    /// User类
    /// </summary>
    public class User
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public User()
        {

        }

        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string id_no { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        public int role { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string psd { get; set; }
        /// <summary>
        /// 状态 0表示禁言
        /// </summary>
        public int status { get; set; }
        
    }
}
