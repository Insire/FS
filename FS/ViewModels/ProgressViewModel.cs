using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using System;

namespace FS
{
    public class ProgressViewModel : ViewModelBase, IDisposable
    {
        private readonly Progress<int> _progress;

        private int _mininmum;
        public int Minimum
        {
            get { return _mininmum; }
            set { SetValue(ref _mininmum, value); }
        }

        private int _maximum;
        public int Maximum
        {
            get { return _maximum; }
            set { SetValue(ref _maximum, value); }
        }

        private int _value;
        public int Value
        {
            get { return _value; }
            set { SetValue(ref _value, value); }
        }

        public ProgressViewModel(ICommandBuilder commandBuilder, Progress<int> progress)
            : base(commandBuilder)
        {
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _progress.ProgressChanged += ProgressChanged;
        }

        private async void ProgressChanged(object sender, int e)
        {
            await Dispatcher.Invoke(() => Value += e).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _progress.ProgressChanged -= ProgressChanged;
        }
    }
}
