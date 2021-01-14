using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    public class ResultState
    {
        public ResultState()
        {
            success = false;
            message = " ";
            code = 0;
            value = "";
        }
        //返回操作成功与否
        public bool success { get; set; }

        //返回操作提示信息
        public string message { get; set; }

        //返回操作状态码，来约定不同状态码对应不同信息;默认0为错误操作
        public int code { get; set; }

        //返回结果对象
        public Object value { get; set; }
    }
}
