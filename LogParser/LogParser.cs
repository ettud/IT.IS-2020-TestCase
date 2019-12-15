using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LogParser
{
    public class LogParser : ILogParser
    {
        public IEnumerable<LogRecord> ParseLogs(StreamReader stream)
        {
            List<LogRecord> logRecords = new List<LogRecord>();
            string line;
            while ((line = stream.ReadLine()) != null)
            {
                var logRecord = ParseLog(line);
                logRecords.Add(logRecord);
            }

            return logRecords;
        }

        public int ParseLogs(StreamReader stream, Action<LogRecord> onLogRecordParsed)
        {
            string line;
            int count = 0;
            while ((line = stream.ReadLine()) != null)
            {
                var logRecord = ParseLog(line);
                Task.Run(() => onLogRecordParsed?.Invoke(logRecord));
                count++;
            }
            return count;
        }

        public LogRecord ParseLog(string str)
        {
            //@gruber v2
            const string urlRegex = @"(?i)\b((?:[a-z][\w-]+:(?:/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))";
            const string ipRegex = @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"; //will valid also 999.999.999.999, but it's fine
            //EXAMPLE:
            //shop_api      | 2018-08-01 00:01:35 [YQ4WUDJV] INFO: 121.165.118.201 https://all_to_the_bottom.com/
            var regex = new Regex(
                @"([a-zA-Z0-9_]+)\s+\|\s+(\d\d\d\d)\-(\d\d)\-(\d\d)\s(\d\d):(\d\d):(\d\d)\s\[([A-Z0-9]+)\]\sINFO:\s(" +
                ipRegex + @")\s(" +
                urlRegex + ")");
            var regexResult = regex.Match(str);
            if (regexResult.Success)
            {
                int i = 1;
                var logRecord = new LogRecord();
                logRecord.Source = regexResult.Groups[i++].Value;
                {
                    var year = int.Parse(regexResult.Groups[i++].Value);
                    var month = int.Parse(regexResult.Groups[i++].Value);
                    var day = int.Parse(regexResult.Groups[i++].Value);
                    var hour = int.Parse(regexResult.Groups[i++].Value);
                    var minute = int.Parse(regexResult.Groups[i++].Value);
                    var second = int.Parse(regexResult.Groups[i++].Value);
                    logRecord.RecordTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
                }
                logRecord.Id = regexResult.Groups[i++].Value;
                logRecord.Ip = regexResult.Groups[i++].Value;
                logRecord.Url = regexResult.Groups[i++].Value;
                return logRecord;
            }
            else throw new Exception("Failed to parse log record");
        }
    }
}
