using System;
using System.Collections.Generic;
using System.Linq;
using LogBasePresenter.Models;
using Microsoft.AspNetCore.Mvc;
using WebServer.Models.ReportsService;

namespace WebServer.Services
{
    public interface IDateTimeService
    {
        DateTime FromJsGetTime(double jsTimestamp);
    }
    public class DateTimeService : IDateTimeService
    {
        public DateTime FromJsGetTime(double jsTimestamp)
        {
            return new DateTime(1970, 01, 01).AddMilliseconds(jsTimestamp);
        }
    }
}
