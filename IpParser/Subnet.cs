using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IpParser
{
    public class Subnet
    {
        public int Id { get; set; }
        public IPAddress ZeroIp { get; set; }
        public int Length { get; set; }
        public int CountryId { get; set; }
    }
}
