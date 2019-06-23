using MvvmScarletToolkit.Observables;

namespace FS
{
    public sealed class MainViewModel : ObservableObject
    {
        public SyncsViewModel Syncs { get; }

        public MainViewModel(SyncsViewModel syncs)
        {
            Syncs = syncs;
        }
    }
}
