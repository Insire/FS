using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FS.Sync
{
    public sealed class Log
    {
        private readonly List<Tuple<string, string>> _finalReportEntries = new List<Tuple<string, string>>();
        private readonly Action<string, ConsoleColor> _consoleWriteDelegate;

        public Log(Action<string, ConsoleColor> consoleWriteDelegate)
        {
            _consoleWriteDelegate = consoleWriteDelegate ?? throw new ArgumentNullException(nameof(consoleWriteDelegate));
        }

        public void Write(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            _consoleWriteDelegate(message, color);
        }

        public void WriteLine(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            _consoleWriteDelegate(message, color);
        }

        public void WriteError(string message, ConsoleColor color = ConsoleColor.Red)
        {
            _consoleWriteDelegate(message, color);
        }

        public void AddFinalReportEntry(string title, string elapsedTime)
        {
            _finalReportEntries.Add(Tuple.Create(title, elapsedTime));
        }

        public void PrintFinalReport()
        {
            var leftColumnWidth = _finalReportEntries.Max(e => e.Item1.Length) + 2;

            var sb = new StringBuilder();
            foreach (var entry in _finalReportEntries)
            {
                var message = (entry.Item1 + ":").PadRight(leftColumnWidth) + entry.Item2;
                sb.AppendLine(message);
            }

            Write(sb.ToString(), ConsoleColor.DarkGray);
        }

        public IDisposable MeasureTime(string operationTitle)
        {
            return new Disposable(this, operationTitle);
        }

        private class Disposable : IDisposable
        {
            private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
            private readonly string _title;
            private readonly Log _log;

            public Disposable(Log log, string operationTitle)
            {
                _log = log;
                _title = operationTitle;
            }

            public void Dispose()
            {
                var elapsedTime = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
                if (elapsedTime != "00:00:00.000")
                {
                    _log.AddFinalReportEntry(_title, elapsedTime);
                }
            }
        }
    }
}
