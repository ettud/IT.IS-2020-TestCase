using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace LogBasePresenter.Models
{
    public class LogRecord
    {
        [Key]
        public string Id { get; set; }
        public DateTime RecordTime { get; set; }
        public IPAddress Ip { get; set; }
        public string Url { get; set; }
        public string UrlParameters { get; set; }
    }
}
