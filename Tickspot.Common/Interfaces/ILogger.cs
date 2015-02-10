using System;

namespace Tickspot.Common.Interfaces
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception e);
        void Critical(string message);
        void Always(string message);
    }
}
