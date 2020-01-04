using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LogBasePresenter.DatabaseModels;
using LogBasePresenter.Models;
using LogParser;
using Microsoft.AspNetCore.Mvc;
using WebServer.Models.ReportsService;

namespace WebServer.Services
{
    public class AdminService
    {
        private readonly LogBasePresenter.LogBasePresenter _logBasePresenter;
        public AdminService(LogBasePresenter.LogBasePresenter logBasePresenter)
        {
            _logBasePresenter = logBasePresenter;
        }

        public void ClearLogs()
        {
            _logBasePresenter.ClearLogRecords();
        }

        public void UploadLogs(IEnumerable<LogParser.LogRecord> records)
        {
            _logBasePresenter.AddLogRecords(records.Select(CastLogRecord));
        }

        public void ClearIps()
        {
            _logBasePresenter.ClearIps();
        }

        private static LogBasePresenter.DatabaseModels.LogRecord CastLogRecord(LogParser.LogRecord logRecord)
        {
            if (string.IsNullOrEmpty(logRecord.Id)) throw new ArgumentNullException("LogRecord id is null");
            var newRecord = new LogBasePresenter.DatabaseModels.LogRecord { RecordTime = logRecord.RecordTime };
            newRecord.Id = logRecord.Id;
            {
                newRecord.QueryDescription = AQuery.FromUrl(logRecord.Url);
            }
            return newRecord.SetIp(IPAddress.Parse(logRecord.Ip));
        }
    }
}
