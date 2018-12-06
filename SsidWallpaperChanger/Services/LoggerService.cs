using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsidWallpaperChanger.Services
{
    public class LoggerService
    {
        // singleton
        private static LoggerService _instance = new LoggerService();
        public static LoggerService Instance { get { return _instance; } }

        private string _logFile;

        public LoggerService()
        {
            _logFile = ConfigurationManager.AppSettings["logfile"];
        }

        // Write log on DEBUG-mode only.
        public void WriteLog(string message)
        {
#if DEBUG
            if (string.IsNullOrEmpty(_logFile))
            {
                return;
            }
            using (var f = new StreamWriter(_logFile, append: true))
            {
                var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                f.WriteLine($"[{date}]:{message}");
            }
#endif
        }

    }
}
