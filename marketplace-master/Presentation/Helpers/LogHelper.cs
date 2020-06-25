using System;
using System.IO;

namespace Presentation.Helpers
{
    public static class LogHelper
    {
        private const string fileName = "inamkt_log.txt";
        private static readonly string filePath = "C:\\inetpub\\logs\\LogFiles\\mkt_log.txt";

        public static void LogString(string s)
        {

#if !DEBUG
            return;
#endif
            try
            {
                if (!File.Exists(filePath))
                {
                    var stream = File.Create(filePath);
                    stream.Close();
                    stream.Dispose();
                }

                File.AppendAllLines(filePath, new string[] { $"\n{s}" });
            }
            catch (Exception)
            {

            }
        }
    }
}