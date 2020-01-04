using System;
using System.Collections.Generic;
using System.Text;

namespace LogBasePresenter.Models
{
    public class PayQuery : AQuery
    {
        public int CartId { get; set; }
        public long UserId { get; set; }
    }
}
