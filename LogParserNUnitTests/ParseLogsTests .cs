using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace LogParserNUnitTests
{
    public class ParseLogsTests
    {
        private string _source =
            Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "logs.txt");
        private StreamReader _reader
        {
            get
            {
                return new StreamReader(File.OpenRead(_source));
            }
        }

        [SetUp]
        public void Setup()
        {
            /*int l = "https://all_to_the_bottom.com/".Length;
            var a = new LogParser.LogParser().ParseLogs(_reader).Select(r => r.Url).Distinct().Select(s => s.Substring(l));
            using (var s = new StreamWriter(@"C:\Users\ShuAS\Desktop\urls.txt"))
            {
                foreach (var aa in a)
                {
                    s.WriteLine(aa);
                }
            }*/
        }

        [Test]
        public void ParseLogsListTest()
        {
            Assert.AreEqual(new LogParser.LogParser().ParseLogs(_reader).Count(), 24284);
        }
    }
}