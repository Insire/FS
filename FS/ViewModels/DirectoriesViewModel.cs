using GuiLabs.FileUtilities;
using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Commands;
using MvvmScarletToolkit.Observables;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FS
{
    public sealed class DirectoriesViewModel : BusinessViewModelListBase<DirectoryViewModel>
    {
        private Patterns _includes;
        public Patterns Includes
        {
            get { return _includes; }
            private set { SetValue(ref _includes, value); }
        }

        private Patterns _excludes;
        public Patterns Excludes
        {
            get { return _excludes; }
            private set { SetValue(ref _excludes, value); }
        }

        private DirectoryInfo _sourceDirctory;
        public DirectoryInfo TargetDirectory
        {
            get { return _sourceDirctory; }
            set { SetValue(ref _sourceDirctory, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }

        private int _id;
        public int Id
        {
            get { return _id; }
            set { SetValue(ref _id, value); }
        }

        public ICommand SyncCommand { get; }

        public DirectoriesViewModel(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
            Excludes = new Patterns(commandBuilder);
            Includes = new Patterns(commandBuilder);

            SyncCommand = commandBuilder
                .Create(Sync, CanSync)
                .WithSingleExecution(CommandManager)
                .Build();
        }

        protected override async Task RefreshInternal(CancellationToken token)
        {
            var includes = new HashSet<string>(Includes.Items.Where(p => !string.IsNullOrWhiteSpace(p.Value)).Select(p => p.Value).SelectMany(Globbing.GlobFolders));
            var excludes = new HashSet<string>(Excludes.Items.Where(p => !string.IsNullOrWhiteSpace(p.Value)).Select(p => p.Value).SelectMany(Globbing.GlobFolders));

            await AddRange(includes.Except(excludes).Select(p => new DirectoryViewModel(CommandBuilder, p)));
        }

        private async Task Sync(CancellationToken token)
        {
            using (BusyStack.GetToken())
            {
                await Refresh(token);
                await Items.ForEachAsync(p => Task.Run(() => FileSystem.CopyFile(p.FullPath, TargetDirectory.FullName, false)));
            }
        }

        private bool CanSync()
        {
            return !IsBusy
                && Count > 0
                && Includes.Count > 0
                && TargetDirectory != null;
        }
    }
}
