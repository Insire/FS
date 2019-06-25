using MvvmScarletToolkit.Observables;

namespace FS
{
    public sealed class DirectoryViewModel : ObservableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }

        private string _fullPath;
        public string FullPath
        {
            get { return _fullPath; }
            private set { SetValue(ref _fullPath, value); }
        }

        public DirectoryViewModel(string fullPath)
        {
            FullPath = fullPath;
        }
    }
}
