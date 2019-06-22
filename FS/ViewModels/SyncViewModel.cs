using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FS
{
    public sealed class SyncViewModel : ViewModelBase, IDisposable
    {
        private DirectoryInfo _sourceDirctory;
        public DirectoryInfo TargetDirectory
        {
            get { return _sourceDirctory; }
            set { SetValue(ref _sourceDirctory, value); }
        }

        private DirectoryInfo _targetDirctory;
        public DirectoryInfo SourceDirectory
        {
            get { return _targetDirctory; }
            set { SetValue(ref _targetDirctory, value); }
        }

        public SyncViewModel(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
        }

        private Task Sync(CancellationToken token)
        {
            // + sync via Matcher
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // + move object back to solutionsviewmodel
        }
    }
}
