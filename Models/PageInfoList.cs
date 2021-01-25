using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace live.Models
{
    public class PageInfoList
    {
        //public List<T> items = new List<T>();
        public PageInfoList()
        {
            this.count = 0;
            this.pageIndex = 0;
            this.pageSize = 5;
            this.items = "";
        }
        //public PageInfoList(List<T> items, int count, int pageIndex, int pageSize)
        //{
        //    this.pageIndex = pageIndex;
        //    this.pageSize = pageSize;
        //    this.items.AddRange(items);
        //    this.count = count;

        //}
        //public PageInfoList(int count,int pageIndex,int pageSize,object items)
        //{
        //    this.pageIndex = pageIndex;
        //    this.pageSize = pageSize;
        //    this.items = items;
        //    this.count = count;
        //}


        public int count { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public Object items { get; set; }
        //public List<T> items { get; set; }
        //public List<T> get()
        //{
        //    return this.items;
        //}
        //public void set(List<T> items)
        //{
        //    this.items.AddRange(items);

        //}

        //public void set(T items)
        //{
        //    this.items.Add(items);

        //}

    }
}
