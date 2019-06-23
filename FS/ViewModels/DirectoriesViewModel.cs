using GuiLabs.FileUtilities;
using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Commands;
using MvvmScarletToolkit.Observables;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FS
{
    public sealed class DirectoriesViewModel : BusinessViewModelListBase<DirectoryViewModel>
    {
        private bool copyLeftOnlyFiles = true;
        public bool CopyLeftOnlyFiles
        {
            get { return copyLeftOnlyFiles; }
            set { SetValue(ref copyLeftOnlyFiles, value); }
        }

        private bool updateChangedFiles = true;
        public bool UpdateChangedFiles
        {
            get { return updateChangedFiles; }
            set { SetValue(ref updateChangedFiles, value); }
        }

        private bool deleteRightOnlyFiles = true;
        public bool DeleteRightOnlyFiles
        {
            get { return deleteRightOnlyFiles; }
            set { SetValue(ref deleteRightOnlyFiles, value); }
        }

        private bool copyEmptyDirectories = true;
        public bool CopyEmptyDirectories
        {
            get { return copyEmptyDirectories; }
            set { SetValue(ref copyEmptyDirectories, value); }
        }

        private bool deleteRightOnlyDirectories = true;
        public bool DeleteRightOnlyDirectories
        {
            get { return deleteRightOnlyDirectories; }
            set { SetValue(ref deleteRightOnlyDirectories, value); }
        }

        private bool deleteSameFiles;
        public bool DeleteSameFiles
        {
            get { return deleteSameFiles; }
            set { SetValue(ref deleteSameFiles, value); }
        }

        private bool deleteChangedFiles;
        public bool DeleteChangedFiles
        {
            get { return deleteChangedFiles; }
            set { SetValue(ref deleteChangedFiles, value); }
        }

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

        private string _sourceDirctory;
        public string TargetDirectory
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

        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            private set { SetValue(ref _isActive, value); }
        }

        public ICommand SyncCommand { get; }
        public ICommand ToggleCommand { get; }

        public DirectoriesViewModel(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
            Excludes = new Patterns(commandBuilder);
            Includes = new Patterns(commandBuilder);

            SyncCommand = commandBuilder
                .Create(Sync, CanSync)
                .WithSingleExecution(CommandManager)
                .Build();

            ToggleCommand = commandBuilder
                .Create(Toggle, CanToggle)
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
                await Items.ForEachAsync(p => Task.Run(() => GuiLabs.FileUtilities.Sync.Directories(p.FullPath, TargetDirectory, new Arguments()
                {
                    CopyEmptyDirectories = CopyEmptyDirectories,
                    CopyLeftOnlyFiles = CopyLeftOnlyFiles,
                    DeleteChangedFiles = DeleteChangedFiles,
                    DeleteRightOnlyDirectories = DeleteRightOnlyDirectories,
                    DeleteRightOnlyFiles = DeleteRightOnlyFiles,
                    DeleteSameFiles = DeleteSameFiles,
                    UpdateChangedFiles = UpdateChangedFiles,
                })));
            }
        }

        private bool CanSync()
        {
            return !IsBusy
                && Count > 0
                && Includes.Count > 0
                && TargetDirectory.Length > 0;
        }

        private Task Toggle()
        {
            return Dispatcher.Invoke(() => IsActive = !IsActive);
        }

        private bool CanToggle()
        {
            return !IsBusy;
        }
    }
}
