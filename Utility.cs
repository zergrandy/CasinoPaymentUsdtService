using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QuoteLineBot1
{
    public static class Utility
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void LogAndConsole(string bot, string msg)
        {
            msg = bot + "   " + msg;
            log.Info(msg);
            string dtNowStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Console.WriteLine($"{dtNowStr}   {msg}");
        }

        public static void LogAndConsole(string msg)
        {
            log.Info(msg);
            string dtNowStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Console.WriteLine($"{dtNowStr}   {msg}");
        }

    }
}