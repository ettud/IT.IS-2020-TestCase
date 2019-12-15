using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser
{
    public class LogRecord
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public DateTime RecordTime { get; set; }
        public string Ip { get; set; }
        public string Url { get; set; }
    }
}
