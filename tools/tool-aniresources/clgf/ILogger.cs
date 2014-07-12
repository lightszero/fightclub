using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clgf
{
        public enum LogLevel
        {
            Normal,
            Waring,
            System,
            Error,
        }
        public interface ILogger
        {
            void Log(LogLevel level, string text);
        }
}
