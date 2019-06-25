using System;
using MvvmScarletToolkit.Observables;

namespace FS
{
    public sealed class Pattern : ObservableObject
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set { SetValue(ref _value, value); }
        }

        public Pattern(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
