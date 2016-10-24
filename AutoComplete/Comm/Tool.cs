using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoComplete.Comm
{
    class Tool
    {
        private static readonly object flag = new object();
        public static void LogFile(string msg)
        {
            lock (flag)
            {
                var outputFile = new FileInfo(@"D:\workfile\AutoComplete\QA\6-4\log\log.txt");

                using (var writer = File.AppendText(@"D:\workfile\AutoComplete\QA\6-4\log\log.txt"))
                {
                    writer.WriteLine(msg);
                    writer.Flush();
                }
            }

        }
        public static void LogFile(string file,string msg)
        {
            lock (flag)
            {
                var outputFile = new FileInfo(file);
                using (var writer = File.AppendText(file))
                {
                    writer.WriteLine(msg);
                    writer.Flush();
                }
            }
        }

    }
}
