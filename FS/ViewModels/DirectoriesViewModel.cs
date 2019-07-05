using FS.Sync;
using MvvmScarletToolkit;
using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Commands;
using MvvmScarletToolkit.Observables;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FS
{
    public sealed class DirectoriesViewModel : BusinessViewModelListBase<DirectoryViewModel>
    {
        private readonly IProgress<int> _progress;
        private readonly ConcurrentCommandBase _syncCommand;

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

        private bool _respectLastAccessDateTime = true;
        public bool RespectLastAccessDateTime
        {
            get { return _respectLastAccessDateTime; }
            set { SetValue(ref _respectLastAccessDateTime, value); }
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

        private ProgressViewModel _progressViewModel;
        public ProgressViewModel ProgressViewModel
        {
            get { return _progressViewModel; }
            private set { SetValue(ref _progressViewModel, value); }
        }

        private string _targetDirectory;
        public string TargetDirectory
        {
            get { return _targetDirectory; }
            set { SetValue(ref _targetDirectory, value); }
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

        private bool _showLog;
        public bool ShowLog
        {
            get { return _showLog; }
            set { SetValue(ref _showLog, value); }
        }

        private TimeSpan _interval = TimeSpan.FromSeconds(30);
        public TimeSpan Interval
        {
            get { return _interval; }
            set { SetValue(ref _interval, value); }
        }

        private IntervalType _intervalType;
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

        public ICommand ExcludeCommand { get; }
        public ICommand StartTimedSynchronizationCommand { get; }
        public ICommand SyncCommand => _syncCommand;

        public DirectoriesViewModel(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
            var progress = new Progress<int>();
            _progress = progress;

            Log = new LogViewModel(commandBuilder);
            Excludes = new Patterns(commandBuilder, this);
            Includes = new Patterns(commandBuilder, this);
            ProgressViewModel = new ProgressViewModel(commandBuilder, progress, this);

            _syncCommand = commandBuilder
                .Create(Synchronize, CanSync)
                .WithSingleExecution(CommandManager)
                .WithBusyNotification(BusyStack)
                .WithCancellation()
                .Build();

            StartTimedSynchronizationCommand = commandBuilder
                .Create(StartTimedSynchronization, CanStartTimedSynchronization)
                .WithSingleExecution(CommandManager)
                .WithBusyNotification(BusyStack)
                .WithCancellation()
                .Build();

            ExcludeCommand = commandBuilder
                .Create(Exclude, CanExclude)
                .WithSingleExecution(CommandManager)
                .Build();
        }

        protected override async Task RefreshInternal(CancellationToken token)
        {
            var includes = Task.Run(() => Includes.GetDirectories().ToArray());
            var excludes = Task.Run(() => Excludes.GetDirectories().ToArray());

            await Task.WhenAll(includes, excludes).ConfigureAwait(false);

            await AddRange(includes.Result.Except(excludes.Result)
                    .Distinct()
                    .OrderBy(p => p)
                    .Select(p => new DirectoryViewModel(p))).ConfigureAwait(false);
        }

        private async Task Synchronize(CancellationToken token)
        {
            try
            {
                using (BusyStack.GetToken())
                {
                    await ProgressViewModel.Reset().ConfigureAwait(false);
                    await Log.Clear(token).ConfigureAwait(false);

                    Debug.WriteLine("Refreshing...");
                    await Refresh(token).ConfigureAwait(false);

                    Debug.WriteLine("Synchronizing...");
                    await Task.Run(() => Items.ForEach(p => SyncDirectory(p.FullPath, token)), token).ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        private void SyncDirectory(string path, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var log = new Log(async (message, color) => await Log.Add(new LogEntry()
            {
                Message = message,
                Color = color,
            }, token).ConfigureAwait(false));

            GuiLabs.FileUtilities.Sync.Directories(path, TargetDirectory, log, token, new Arguments()
            {
                CopyLeftOnlyFiles = CopyLeftOnlyFiles,
                DeleteChangedFiles = DeleteChangedFiles,
                DeleteRightOnlyFiles = DeleteRightOnlyFiles,
                DeleteSameFiles = DeleteSameFiles,
                UpdateChangedFiles = UpdateChangedFiles,
                RespectLastAccessDateTime = RespectLastAccessDateTime,
            });

            _progress.Report(1);
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
        }

        private async Task StartTimedSynchronization(CancellationToken token)
        {
            using (BusyStack.GetToken())
            {
                try
                {
                    await Dispatcher.Invoke(() => IsActive = true, token).ConfigureAwait(false);
                    do
                    {
                        await Synchronize(token).ConfigureAwait(false);
                        await UpdateExecution().ConfigureAwait(false);
                        await Task.Delay(Interval).ConfigureAwait(false);
                    } while (!token.IsCancellationRequested);
                }
                finally
                {
                    await Dispatcher.Invoke(() => IsActive = false).ConfigureAwait(false);
                    await UpdateExecution().ConfigureAwait(false);
                }
            }

            async Task UpdateExecution()
            {
                var nextExecution = DateTime.Now + Interval;

                await Dispatcher.Invoke(() => NextExecution = nextExecution).ConfigureAwait(false);
            }
        }

        private bool CanStartTimedSynchronization()
        {
            return CanSync();
        }

        private async Task Exclude(CancellationToken token)
        {
            var root = Root
                .TrimEnd('\\')
                .TrimEnd('/');

            var pathWithOutRoot = SelectedItem.FullPath
                .Replace(root, string.Empty)
                .Trim('\\')
                .Trim('/');

            await Excludes.Add(new Pattern($"**/{pathWithOutRoot}")).ConfigureAwait(false);
            await Remove().ConfigureAwait(false);
        }

        private bool CanExclude()
        {
            return !IsBusy
                && SelectedItem != null
                && !string.IsNullOrWhiteSpace(SelectedItem.FullPath)
                && !Excludes.Items.Any(p => p.Value.Equals(SelectedItem.FullPath));
        }
    }
}
