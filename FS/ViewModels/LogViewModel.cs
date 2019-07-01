using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using System;
using System.Threading.Tasks;

namespace FS
{
    public sealed class LogViewModel : ViewModelListBase<LogEntry>
    {
        public LogViewModel(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
        }

        public override async Task Add(LogEntry item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using (BusyStack.GetToken())
            {
                await Dispatcher.Invoke(delegate
                {
                    _items.Insert(0, item);
                }).ConfigureAwait(continueOnCapturedContext: false);
                await Dispatcher.Invoke(delegate
                {
                    OnPropertyChanged(nameof(Count));
                }).ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}
