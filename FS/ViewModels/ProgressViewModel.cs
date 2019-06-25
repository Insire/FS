using MvvmScarletToolkit.Observables;
using System;

namespace FS
{
    public class ProgressViewModel : ObservableObject, IDisposable
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

        public ProgressViewModel(Progress<int> progress)
        {
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _progress.ProgressChanged += ProgressChanged;
        }

        private void ProgressChanged(object sender, int e)
        {
            Value += e;
        }

        public void Dispose()
        {
            _progress.ProgressChanged -= ProgressChanged;
        }
    }
}
