using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FS
{
    public class ProgressViewModel : ViewModelBase
    {
        private readonly Progress<int> _progress;
        private readonly DirectoriesViewModel _directoriesViewModel;
        private readonly IObservable<EventPattern<int>> _observable;
        private readonly IDisposable _disposable;

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

            _observable = Observable.FromEventPattern<int>(
                fsHandler => _progress.ProgressChanged += fsHandler,
                fsHandler => _progress.ProgressChanged -= fsHandler);

            _disposable = _observable
                .Publish(ps => ps.Buffer(() => ps.Throttle(TimeSpan.FromSeconds(1))))
                .Subscribe(x => ProgressChanged(x.Sum(p => p.EventArgs)));

            //_disposable = _observable
            //    .Window(TimeSpan.FromSeconds(1))
            //    .Count()
            //    .Subscribe(ProgressChanged);
        }

        private async void ProgressChanged(int e)
        {
            await Dispatcher.Invoke(() => Value += e).ConfigureAwait(false);
            Debug.WriteLine("ProgressChanged " + Value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposable.Dispose();
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
