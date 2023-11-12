using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp.Utilites
{
    public static class LogWriter
    {
        static object locker = new object();

        public static void Write(string message)
        {
            lock (locker)
            {
                var streamWriter = File.AppendText(AppConfig.LogFileName);
                streamWriter.WriteLine($"{DateTime.Now} => {message}");
                streamWriter.Close();
            }
        }
    }
}
