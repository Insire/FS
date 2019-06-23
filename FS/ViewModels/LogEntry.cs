using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using System;

namespace FS
{
    public sealed class LogEntry : ViewModelBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetValue(ref _message, value); }
        }

        private ConsoleColor _color;
        public ConsoleColor Color
        {
            get { return _color; }
            set { SetValue(ref _color, value); }
        }

        public LogEntry(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
        }
    }
}
