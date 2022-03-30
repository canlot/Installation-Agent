using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Logger
{
    public enum LogType
    {
        Info = 1,
        Warning = 2,
        Error = 4
    }
    public static class CommonLogger
    {
        public static string LoggerSource = "Installation Agent Service";
        public static string LoggerName = "Installation Agent";
        private static bool initialized = false;
        private static EventLog logger = new EventLog();
        public static void InitializeLogger()
        {
            if (!EventLog.SourceExists(LoggerSource))
            {
                EventLog.CreateEventSource(LoggerSource, LoggerName);
            }
            logger.Source = LoggerSource;
            logger.Log = LoggerName;
            initialized = true;
        }
        public static void LogEvent(string message, LogType type = LogType.Info)
        {
            if (!initialized)
                InitializeLogger();
            
            switch(type)
            {
                case LogType.Info:
                    logger.WriteEntry(message, EventLogEntryType.Information);
                    break;
                case LogType.Warning:
                    logger.WriteEntry(message, EventLogEntryType.Warning);
                    break;
                case LogType.Error:
                    logger.WriteEntry(message, EventLogEntryType.Error);
                    break;
            }

        }

        public static void LogInformation(string message)
        {

        }
        public static void LogDebug(string message)
        {

        }
        public static void LogWarning(string message)
        {

        }
        public static void LogError(string message)
        {

        }
    }
}
