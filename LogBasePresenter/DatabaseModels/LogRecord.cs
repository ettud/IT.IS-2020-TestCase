using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using LogBasePresenter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogBasePresenter.DatabaseModels
{
    public partial class LogRecord
    {
        [Key]
        public string Id { get; set; }
        public DateTime RecordTime { get; set; }
        public IPAddress Ip { get; set; }
        [MaxLength(32)]
        [Column(TypeName = "bit(32)")]
        public BitArray IpBit { get; set; }
        public AQuery QueryDescription { get; set; }
        public LogRecord SetIp(IPAddress ip)
        {
            Ip = ip;
            var bytes = ip.GetAddressBytes();
            IpBit = new BitArray(BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes);
            if (BitConverter.IsLittleEndian) //looks like postgres has another bytes order
            {
                Reverse(IpBit);
            }
            return this;
        }

        private static void Reverse(BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }
    }
}
