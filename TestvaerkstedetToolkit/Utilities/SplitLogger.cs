using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TestvaerkstedetToolkit.Services;

namespace TestvaerkstedetToolkit.Utilities
{
    /// <summary>
    /// Logger klasse til detaljeret operation logging
    /// Implementerer ISplitLogger interface for service integration
    /// </summary>
    public class SplitLogger : ISplitLogger
    {
        private readonly List<string> _logEntries = new List<string>();
        private readonly DateTime _startTime = DateTime.Now;

        public void LogInfo(string message)
        {
            var entry = $"[{DateTime.Now:HH:mm:ss}] INFO: {message}";
            _logEntries.Add(entry);
            System.Diagnostics.Debug.WriteLine(entry);
        }

        public void LogWarning(string message)
        {
            var entry = $"[{DateTime.Now:HH:mm:ss}] WARN: {message}";
            _logEntries.Add(entry);
            System.Diagnostics.Debug.WriteLine(entry);
        }

        public void LogError(string message)
        {
            var entry = $"[{DateTime.Now:HH:mm:ss}] ERROR: {message}";
            _logEntries.Add(entry);
            System.Diagnostics.Debug.WriteLine(entry);
        }

        public void SaveToFile(string logPath)
        {
            var logContent = new StringBuilder();

            logContent.AppendLine("==============================================================================");
            logContent.AppendLine("                       XML TABLE SPLIT OPERATION LOG");
            logContent.AppendLine("==============================================================================");
            logContent.AppendLine();
            logContent.AppendLine($"Operation startet: {_startTime:yyyy-MM-dd HH:mm:ss}");
            logContent.AppendLine($"Log genereret: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            logContent.AppendLine($"Total varighed: {DateTime.Now - _startTime:hh\\:mm\\:ss}");
            logContent.AppendLine($"System: {Environment.MachineName}");
            logContent.AppendLine($"Bruger: {Environment.UserName}");
            logContent.AppendLine();

            foreach (var entry in _logEntries)
            {
                logContent.AppendLine(entry);
            }

            logContent.AppendLine();
            logContent.AppendLine("==============================================================================");
            logContent.AppendLine("                               LOG AFSLUTTET");
            logContent.AppendLine("==============================================================================");

            File.WriteAllText(logPath, logContent.ToString(), Encoding.UTF8);
        }
    }
}