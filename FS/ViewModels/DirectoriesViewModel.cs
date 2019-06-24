using FS.Sync;
using GlobExpressions;
using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Commands;
using MvvmScarletToolkit.Observables;
using System;
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
        private Timer _timer;

        private bool _copyLeftOnlyFiles = true;
        public bool CopyLeftOnlyFiles
        {
            get { return _copyLeftOnlyFiles; }
            set { SetValue(ref _copyLeftOnlyFiles, value); }
        }

        private bool _updateChangedFiles = true;
        public bool UpdateChangedFiles
        {
            get { return _updateChangedFiles; }
            set { SetValue(ref _updateChangedFiles, value); }
        }

        private bool _deleteRightOnlyFiles = true;
        public bool DeleteRightOnlyFiles
        {
            get { return _deleteRightOnlyFiles; }
            set { SetValue(ref _deleteRightOnlyFiles, value); }
        }

        private bool _copyEmptyDirectories = true;
        public bool CopyEmptyDirectories
        {
            get { return _copyEmptyDirectories; }
            set { SetValue(ref _copyEmptyDirectories, value); }
        }

        private bool _deleteRightOnlyDirectories = true;
        public bool DeleteRightOnlyDirectories
        {
            get { return _deleteRightOnlyDirectories; }
            set { SetValue(ref _deleteRightOnlyDirectories, value); }
        }

        private bool _deleteSameFiles;
        public bool DeleteSameFiles
        {
            get { return _deleteSameFiles; }
            set { SetValue(ref _deleteSameFiles, value); }
        }

        private bool _deleteChangedFiles;
        public bool DeleteChangedFiles
        {
            get { return _deleteChangedFiles; }
            set { SetValue(ref _deleteChangedFiles, value); }
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

        private LogViewModel _log;
        public LogViewModel Log
        {
            get { return _log; }
            private set { SetValue(ref _log, value); }
        }

        private string _sourceDirctory;
        public string TargetDirectory
        {
            get { return _sourceDirctory; }
            set { SetValue(ref _sourceDirctory, value); }
        }

        private string _root;
        public string Root
        {
            get { return _root ?? new DirectoryInfo(".").FullName; }
            set { SetValue(ref _root, value); }
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

        private TimeSpan _interval = TimeSpan.FromSeconds(30);
        public TimeSpan Interval
        {
            get { return _interval; }
            private set { SetValue(ref _interval, value); }
        }

        private IntervalType _intervalType = IntervalType.Seconds;
        public IntervalType IntervalType
        {
            get { return _intervalType; }
            set { SetValue(ref _intervalType, value, OnChanged: OnIntervalTypeChanged); }
        }

        private string _intervalInput = "30";
        public string IntervalInput
        {
            get { return _intervalInput; }
            set { SetValue(ref _intervalInput, value, OnChanged: OnIntervalInputChanged); }
        }

        private DateTime _nextExecution;
        public DateTime NextExecution
        {
            get { return _nextExecution; }
            private set { SetValue(ref _nextExecution, value); }
        }

        public ICommand SyncCommand { get; }
        public ICommand ToggleCommand { get; }

        public DirectoriesViewModel(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
            Excludes = new Patterns(commandBuilder, this);
            Includes = new Patterns(commandBuilder, this);
            Log = new LogViewModel(commandBuilder);

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
            var root = new DirectoryInfo(Root);

            var directories = Includes.GetDirectories();
            var includes = new HashSet<string>(Includes.GetDirectories());
            var excludes = new HashSet<string>(Excludes.GetDirectories());

            await AddRange(includes.Except(excludes).OrderBy(p => p).Select(p => new DirectoryViewModel(CommandBuilder, p))).ConfigureAwait(false);
        }

        private async Task Sync(CancellationToken token)
        {
            using (BusyStack.GetToken())
            {
                await Refresh(token).ConfigureAwait(false);
                await Log.Clear(token).ConfigureAwait(false);
                await Items.ForEachAsync(p => Task.Run(() => GuiLabs.FileUtilities.Sync.Directories(p.FullPath, TargetDirectory, new Arguments()
                {
                    CopyEmptyDirectories = CopyEmptyDirectories,
                    CopyLeftOnlyFiles = CopyLeftOnlyFiles,
                    DeleteChangedFiles = DeleteChangedFiles,
                    DeleteRightOnlyDirectories = DeleteRightOnlyDirectories,
                    DeleteRightOnlyFiles = DeleteRightOnlyFiles,
                    DeleteSameFiles = DeleteSameFiles,
                    UpdateChangedFiles = UpdateChangedFiles,
                }, new Log((message, color) => Log.Add(new LogEntry(CommandBuilder)
                {
                    Message = message,
                    Color = color,
                }))))).ConfigureAwait(false);
            }
        }

        private bool CanSync()
        {
            return !IsBusy
                && Count > 0
                && Includes.Count > 0
                && TargetDirectory.Length > 0;
        }

        private void OnIntervalTypeChanged()
        {
            UpdateTimer();
        }

        private void OnIntervalInputChanged()
        {
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            if (IntervalInput.Length <= 0)
                return;

            if (IntervalInput.Length > 3)
                return;

            if (!IntervalInput.All(c => char.IsDigit(c)))
                return;

            switch (IntervalType)
            {
                case IntervalType.Seconds:
                    Interval = TimeSpan.FromSeconds(double.Parse(IntervalInput));
                    break;

                case IntervalType.Minutes:
                    Interval = TimeSpan.FromMinutes(double.Parse(IntervalInput));
                    break;

                case IntervalType.Hours:
                    Interval = TimeSpan.FromHours(double.Parse(IntervalInput));
                    break;
            }

            if (_timer is null)
                return;

            _timer.Change(TimeSpan.Zero, Interval);
        }

        private async Task Toggle()
        {
            using (BusyStack.GetToken())
            {
                if (_timer is null)
                {
                    _timer = new Timer(Callback, null, TimeSpan.Zero, Interval);
                }
                else
                {
                    _timer.Dispose();
                    _timer = null;
                }

                await UpdateExecution().ConfigureAwait(false);
                await Dispatcher.Invoke(() => IsActive = !IsActive).ConfigureAwait(false);
            }

#pragma warning disable RCS1163 // Unused parameter.
            async void Callback(object state)
            {
                await Sync(CancellationToken.None).ConfigureAwait(false);
                await UpdateExecution().ConfigureAwait(false);
            }
#pragma warning restore RCS1163 // Unused parameter.

            async Task UpdateExecution()
            {
                var nextExecution = DateTime.Now + Interval;

                await Dispatcher.Invoke(() => NextExecution = nextExecution).ConfigureAwait(false);
            }
        }

        private bool CanToggle()
        {
            return !IsBusy;
        }
    }
}
