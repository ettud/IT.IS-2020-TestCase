using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IpParser;
using Microsoft.VisualBasic.FileIO;

namespace IpParser
{
    public class GeoLite2Parser
    {
        private List<Country> _countries { get; set; }
        private List<Subnet> _subnets { get; set; }
        public ReadOnlyCollection<Country> Countries => _countries?.AsReadOnly();
        public ReadOnlyCollection<Subnet> Subnets => _subnets?.AsReadOnly();
        public GeoLite2Parser()
        {
            _countries = new List<Country>();
            _subnets = new List<Subnet>();
        }

        public void ParseCountries(StreamReader stream)
        {
            using (TextFieldParser csvParser = new TextFieldParser(stream))
            {
                csvParser.SetDelimiters(",");
                csvParser.HasFieldsEnclosedInQuotes = true;

                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();
                    if (int.TryParse(fields[0], out var id))
                    {
                        _countries.Add(new Country {Id = id, Name = fields[5]??""});
                    }
                }
            }
        }

        public void ParseIps(StreamReader stream)
        {
            using (TextFieldParser csvParser = new TextFieldParser(stream))
            {
                csvParser.SetDelimiters(",");
                csvParser.HasFieldsEnclosedInQuotes = true;

                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();
                    if (int.TryParse(fields[1], out var countryId))
                    {
                        string[] ipParts = fields[0].Split('/');
                        if (ipParts.Length == 2)
                        {
                            if (int.TryParse(ipParts[1], out var subnetLength))
                            {
                                if (IPAddress.TryParse(ipParts[0], out var zeroIp))
                                {
                                    var subnet = new Subnet();
                                    subnet.CountryId = countryId;
                                    subnet.ZeroIp = zeroIp;
                                    subnet.Length = subnetLength;
                                    _subnets.Add(subnet);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
