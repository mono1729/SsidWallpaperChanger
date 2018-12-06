using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SsidWallpaperChanger
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var mutexName = Application.ProductName;
            var mutex = new Mutex(false, mutexName);
            bool allocated = false;

            try
            {
                try
                {
                    allocated = mutex.WaitOne(0, false);
                }
                catch (AbandonedMutexException)
                {
                    allocated = true;
                }
                if (!allocated)
                {
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var mf = new Views.MainForm();
                Application.Run(mf);
            }
            finally
            {
                if (allocated)
                {
                    mutex.ReleaseMutex();
                }
                mutex.Close();
            }
        }
    }
}
