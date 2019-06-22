using MvvmScarletToolkit.Observables;

namespace FS
{
    public sealed class MainViewModel : ObservableObject
    {
        public DirectoriesViewModel Directories { get; }
        public SyncsViewModel Syncs { get; }

        public MainViewModel(DirectoriesViewModel solutions, SyncsViewModel syncs)
        {
            Directories = solutions;
            Syncs = syncs;
        }
    }
}
