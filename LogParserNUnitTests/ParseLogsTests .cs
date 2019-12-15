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
        private const string _source =
            "https://doc-14-9s-docs.googleusercontent.com/docs/securesc/ha0ro937gcuc7l7deffksulhg5h7mbp1/vd08jrmumon8il75cjatavubl68hda36/1576231200000/05763580352083269803/*/1-cRbNsHkG5q2ZQ1mpYm1Q5WeRGqX15VQ?e=download";
        private StreamReader _reader
        {
            get
            {
                var request = WebRequest.Create(_source);
                var response = request.GetResponse();
                return new StreamReader(response.GetResponseStream());
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