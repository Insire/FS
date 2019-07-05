using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FS
{
    public sealed class LogViewModel : ViewModelListBase<LogEntry>
    {
        public LogViewModel(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
        }

        public async Task Add(LogEntry item, CancellationToken token)
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
                }, token).ConfigureAwait(continueOnCapturedContext: false);
                await Dispatcher.Invoke(delegate
                {
                    OnPropertyChanged(nameof(Count));
                }, token).ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}
