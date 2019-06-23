using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;

namespace FS
{
    public sealed class LogViewModel : ViewModelListBase<LogEntry>
    {
        public LogViewModel(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
        }
    }
}
