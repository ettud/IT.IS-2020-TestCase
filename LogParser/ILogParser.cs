using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogParser
{
    interface ILogParser
    {
        IEnumerable<LogRecord> ParseLogs(StreamReader stream);
        int ParseLogs(StreamReader stream, Action<LogRecord> onLogRecordParsed);
        LogRecord ParseLog(string str);
    }
}
