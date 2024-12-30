using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgIO
{
    public static class Log
    {
        public static StringBuilder sbEvent = new StringBuilder();

        public static void EventWriter(string message)
        {
            sbEvent.Append(DateTime.Now.ToString("T"));
            sbEvent.Append("-> ");
            sbEvent.Append(message);
            sbEvent.Append("\r");
        }
    }
}
