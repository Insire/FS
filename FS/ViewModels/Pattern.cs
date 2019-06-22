using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;

namespace FS
{
    public sealed class Pattern : ViewModelBase
    {
        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetValue(ref _value, value); }
        }

        public Pattern(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
        }
    }
}
