using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace weixinSendto
{
    public class Log
    {
        static Log()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            System.IO.Directory.CreateDirectory(path);
        }
        public static void Write(string msg, bool export = true)
        {
            if (!export) return;
            try
            {
                Console.WriteLine(msg);

                string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                File.AppendAllText(file, "\r\n" + DateTime.Now.ToLongTimeString() + ":" + msg);
            }
            catch (Exception)
            {

            }

        }
        public static void Error(string msg)
        {
            try
            {
                Console.WriteLine(msg);

                string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", "Error_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                File.AppendAllText(file, "\r\n" + DateTime.Now.ToLongTimeString() + ":" + msg);
            }
            catch (Exception)
            {

            }

        }
    }
}