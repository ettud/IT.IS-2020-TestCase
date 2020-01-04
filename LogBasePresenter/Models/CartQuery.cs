using System;
using System.Collections.Generic;
using System.Text;

namespace LogBasePresenter.Models
{
    public class CartQuery : AQuery
    {
        public int CartId { get; set; }
        public int GoodsAmount { get; set; }
        public int GoodsId { get; set; }
    }
}
