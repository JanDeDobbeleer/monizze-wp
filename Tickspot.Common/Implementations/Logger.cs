using System;
using System.Diagnostics.Tracing;
using Tickspot.Common.Interfaces;
using Tickspot.Common.Model;

namespace Tickspot.Common.Implementations
{
    public class Logger : ILogger
    {
        private readonly LogSource _source = new LogSource();

        public Logger()
        {
            EventListener informationListener = new StorageFileEventListener("tickspot");
            try
            {
#if DEBUG
                informationListener.EnableEvents(_source, EventLevel.LogAlways);
#else
            informationListener.EnableEvents(_source, EventLevel.Error);
#endif
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Skipping creation, already exists");
            }
        }

        public void Debug(string message)
        {
            _source.Debug(message);
        }

        public void Info(string message)
        {
            _source.Info(message);
        }

        public void Warn(string message)
        {
            _source.Warn(message);
        }

        public void Error(string message, Exception e)
        {
            _source.Error(FormatStackTraceAndMessage(message, e));
        }

        public void Critical(string message)
        {
            _source.Critical(message);
        }

        public void Always(string message)
        {
            _source.Always(message);
        }

        private string FormatStackTraceAndMessage(string message, Exception e)
        {
            return message + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace;
        }
    }
}
