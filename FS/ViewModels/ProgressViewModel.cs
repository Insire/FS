using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using System;
using System.Threading.Tasks;

namespace FS
{
    public class ProgressViewModel : ViewModelBase
    {
        private readonly Progress<int> _progress;
        private readonly DirectoriesViewModel _directoriesViewModel;

        private int _minimum;
        public int Minimum
        {
            get { return _minimum; }
            set { SetValue(ref _minimum, value); }
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

        public ProgressViewModel(ICommandBuilder commandBuilder, Progress<int> progress, DirectoriesViewModel directoriesViewModel)
            : base(commandBuilder)
        {
            _directoriesViewModel = directoriesViewModel ?? throw new ArgumentNullException(nameof(directoriesViewModel));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _progress.ProgressChanged += ProgressChanged;
        }

        private async void ProgressChanged(object sender, int e)
        {
            await Dispatcher.Invoke(() => Value += e).ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _progress.ProgressChanged -= ProgressChanged;
            }

            base.Dispose(disposing);
        }

        public async Task Reset()
        {
            await Dispatcher.Invoke(() => Minimum = 0).ConfigureAwait(false);
            await Dispatcher.Invoke(() => Maximum = _directoriesViewModel.Items.Count).ConfigureAwait(false);
            await Dispatcher.Invoke(() => Value = 0).ConfigureAwait(false);
        }
    }
}
