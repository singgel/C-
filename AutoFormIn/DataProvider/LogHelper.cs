using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders
{
    public class LogHelper
    {
        public static void Write(Exception exception)
        {
            if (EventLog.SourceExists("CarConfigAssistant") == false)
            {
                EventLog.CreateEventSource("CarConfigAssistant", "应用程序");
            }
            StringBuilder innerException = new StringBuilder();
            Exception inner = exception.InnerException;
            while (inner != null)
            {
                innerException.Append(string.Format("\n\t{0}\n{1}\n\n", inner.Message, inner.StackTrace));
                inner = inner.InnerException;
            }
            if(innerException.Length == 0)
                innerException.Append("null");
            EventLog.WriteEntry("CarConfigAssistant", string.Format("{0}\nInnerException:{1}", exception.StackTrace, innerException), EventLogEntryType.Error);
        }
    }
}
