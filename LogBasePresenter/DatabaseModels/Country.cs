using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using LogBasePresenter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogBasePresenter.DatabaseModels
{
    public partial class Country
    {
        public Country()
        {
            Subnets = new HashSet<Subnet>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Subnet> Subnets { get; set; }
    }
}
