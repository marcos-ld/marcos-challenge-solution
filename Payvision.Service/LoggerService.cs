using Payvision.Domain.Interfaces;
using Serilog;
using System;
using System.IO;

namespace Payvision.Service
{
    public class LoggerService : ILoggerService
    {
        public LoggerService()
        {
            var logConfig = new LoggerConfiguration();

            logConfig = logConfig.MinimumLevel.Debug();

            string logDirectory = Path.Combine(Environment.CurrentDirectory, "logs");

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            Log.Logger = logConfig
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile($"{logDirectory}\\log.txt")
                .CreateLogger();
        }

        public void Debug(string msg)
        {
            Log.Debug(msg);
        }

        public void Info(string msg)
        {
            Log.Information(msg);
        }

        public void Error(Exception ex)
        {
            Log.Error(ex, "An exception was thrown");
        }

        public void Error(string msg, Exception ex)
        {
            Log.Error(ex, msg);
        }
    }
}
