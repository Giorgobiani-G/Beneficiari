using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Models
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager() { }
   
        public Pager(int totalItems,int page,int pageSize=5)
        {
            int totalpages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int currentpage = page;
            int startpage = currentpage - 5;
            int endpage = currentpage + 4;

            if (startpage<=0)
            {
                endpage = endpage - (startpage - 1);
                startpage = 1;
            }

            if (endpage>totalpages)
            {
                endpage = totalpages;
                if (endpage>10)
                {
                    startpage = endpage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentpage;
            PageSize = pageSize;
            TotalPages = totalpages;
            StartPage = startpage;
            EndPage = endpage;
        }
    }
}
