using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models.ReportsService
{
    public class GoodsCategoryResponseModel
    {
        public double Timespan { get; set; }
        public int Count { get; set; }

        public GoodsCategoryResponseModel(TimeSpan timespan, int count) : this(timespan.TotalMilliseconds, count) { }

        public GoodsCategoryResponseModel(double timespan, int count)
        {
            Timespan = timespan;
            Count = count;
        }
    }
}
