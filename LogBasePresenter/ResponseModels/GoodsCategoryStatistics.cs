using System;
using System.Collections.Generic;
using System.Text;

namespace LogBasePresenter.ResponseModels
{
    public class GoodsCategoryStatistics
    {
        public string GoodsCategoryName { get; set; }
        public Dictionary<DateTime, int> Statistics { get; set; }

        public GoodsCategoryStatistics(string goodsCategoryName)
        {
            GoodsCategoryName = goodsCategoryName;
            Statistics = new Dictionary<DateTime, int>();
        }
    }
}
