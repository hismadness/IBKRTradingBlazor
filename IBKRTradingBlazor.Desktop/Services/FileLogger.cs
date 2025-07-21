using System;
using System.IO;

namespace IBKRTradingBlazor.Desktop.Services
{
    public class FileLogger
    {
        private readonly string logFilePath;
        private readonly object lockObj = new object();

        public FileLogger()
        {
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            logFilePath = Path.Combine(logDir, $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        }

        public void Log(string message)
        {
            lock (lockObj)
            {
                File.AppendAllText(logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
        }
    }
} 