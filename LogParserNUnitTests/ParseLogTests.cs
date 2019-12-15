using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace LogParserNUnitTests
{
    public class ParseLogTests
    {
        private const string _source = "shop_api";
        private DateTime _time;
        private int _year => _time.Year;
        private int _month => _time.Month;
        private int _day => _time.Day;
        private int _hour => _time.Hour;
        private int _minute => _time.Minute;
        private int _second => _time.Second;
        private const string _id = "7QFE7A5L";
        private const string _ip = "240.179.220.134";
        private const string _url = "https://all_to_the_bottom.com/cart?goods_id=3&amount=1&cart_id=9623";
        private string _line => $"{_source}      | {_year}-{_month.ToString("00")}-{_day.ToString("00")} {_hour.ToString("00")}:{_minute.ToString("00")}:{_second.ToString("00")} [{_id}] INFO: {_ip} {_url}";

        [SetUp]
        public void Setup()
        {
            _time = new DateTime(2018, 8, 12, 11, 7, 39, DateTimeKind.Unspecified);
        }

        [Test]
        public void ParseLogTest()
        {
            Assert.NotNull(new LogParser.LogParser().ParseLog(_line));
        }

        [Test]
        public void IdEqualTest()
        {
            Assert.AreEqual(new LogParser.LogParser().ParseLog(_line).Id, _id);
        }

        [Test]
        public void SourceEqualTest()
        {
            Assert.AreEqual(new LogParser.LogParser().ParseLog(_line).Source, _source);
        }

        [Test]
        public void IpEqualTest()
        {
            Assert.AreEqual(new LogParser.LogParser().ParseLog(_line).Ip, _ip);
        }

        [Test]
        public void UrlEqualTest()
        {
            Assert.AreEqual(new LogParser.LogParser().ParseLog(_line).Url, _url);
        }

        [Test]
        public void RecordTimeEqualTest()
        {
            Assert.AreEqual(new LogParser.LogParser().ParseLog(_line).RecordTime, _time);
        }
    }
}