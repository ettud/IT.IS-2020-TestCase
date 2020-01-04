using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;

namespace LogBasePresenter.DatabaseModels
{
    public partial class Subnet
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(32)]
        [Column(TypeName = "bit(32)")]
        public BitArray Network { get; set; }
        [MaxLength(32)]
        [Column(TypeName = "bit(32)")]
        public BitArray Broadcast { get; set; }
        public Country Country { get; set; }
        public int CountryId { get; set; }

        public Subnet SetSubnetCidr(IPAddress zeroIp, int length)
        {
            var bytes = zeroIp.GetAddressBytes();
            Network = new BitArray(BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes);
            Broadcast = GetBroadcast(zeroIp, length);
            if (BitConverter.IsLittleEndian) //looks like postgres has another bytes order
            {
                Reverse(Network);
                Reverse(Broadcast);
            }
            return this;
        }

        private static BitArray GetBroadcast(IPAddress ip, int length)
        {
            uint c = 0;
            for (int j = 0; j < 4; j++) c = (c << 8) + ip.GetAddressBytes()[j];
            var result = new BitArray(32);
            var s = 1;
            for (int j = 0; j < 32; j++)
            {
                result[j] = (c & s) != 0;
                s = s << 1;
            }
            for (int i = 0; i < 32-length; i++)
            {
                result[i] = true;
            }

            return result;
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
