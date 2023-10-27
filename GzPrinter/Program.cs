using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace GzPrinter
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createNew;
            using (System.Threading.Mutex m = new System.Threading.Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    using (new Form1())
                    {
                        Application.Run();
                    }
                    //Application.Run(new Form1());
                }
                else
                {
                    MessageBox.Show("程序已打开!");
                }
            }

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            //Process current = Process.GetCurrentProcess();
            //var arr = Process.GetProcessesByName(current.ProcessName);
            //if (arr != null && arr.Length > 0)
            //{
            //    //Application.Exit();
            //    Environment.Exit(0);
            //    return;
            //}

            //Application.Run(new Form1());
        }
    }
}
