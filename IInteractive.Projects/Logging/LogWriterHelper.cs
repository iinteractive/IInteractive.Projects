using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace IInteractive.Projects.Logging
{
    public static class LogWriterHelper
    {
        public static void Write(this LogWriter writer, object message, string category, int priority, Exception exception)
        {
            writer.Write(message,
                    category,
                    priority,
                    new Dictionary<string, object>() { { "Exception", exception } });
        }

        public static void Error(this LogWriter writer, object message, Exception exception)
        {
            writer.Write(message,
                Category.Error, Priority.High, new Dictionary<string, object>() { { "Exception", exception } });
        }
    }
}
