using System;
using System.IO;

namespace CakeShop1.Helpers
{
    public static class Logger
    {
        private static string logPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

        public static void Log(string message)
        {
            try
            {
                File.AppendAllText(logPath,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    " - " + message + Environment.NewLine);
            }
            catch
            {
                // ne ruši app ako log failuje
            }
        }
    }
}