using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    public class QueryParameters
    {
        public QueryParameters()
        {
            this.pageIndex = 0;
            this.pageSize = 5;
        }
        public QueryParameters(int pageSize,int pageIndex)
        {
            this.pageSize = pageSize;
            this.pageIndex = pageIndex;
        }

        public int pageSize { get; set; }
        public int pageIndex { get; set; }

    }
}
