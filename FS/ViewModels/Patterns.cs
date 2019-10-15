using MvvmScarletToolkit;
using MvvmScarletToolkit.Observables;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FS
{
    [DebuggerDisplay("{Content} - {Count}")]
    public sealed class Patterns : BusinessViewModelListBase<Pattern>
    {
        private readonly DirectoriesViewModel _directoriesViewModel;

        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetValue(ref _content, value); }
        }

        public ICommand AddCommand { get; }

        public Patterns(ICommandBuilder commandBuilder, DirectoriesViewModel directoriesViewModel)
            : base(commandBuilder)
        {
            _directoriesViewModel = directoriesViewModel ?? throw new ArgumentNullException(nameof(directoriesViewModel));

            AddCommand = commandBuilder
                .Create(Add, CanAdd)
                .WithSingleExecution(CommandManager)
                .Build();

            Messenger.Subscribe<ViewModelListBaseSelectionChanged<Pattern>>(OnSelectionChanged, OnSelectionChangedReceived);
        }

        private async Task Add(CancellationToken token)
        {
            using (BusyStack.GetToken())
            {
                await Add(new Pattern(CommandBuilder, CommandManager, _directoriesViewModel, Content)).ConfigureAwait(false);
            }
        }

        private bool CanAdd()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(Content);
        }

        protected override Task RefreshInternal(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        private void OnSelectionChanged(ViewModelListBaseSelectionChanged<Pattern> messasge)
        {
            if (string.IsNullOrEmpty(messasge?.Content?.Value))
                return;

            Content = messasge.Content.Value;
        }

        private bool OnSelectionChangedReceived(ViewModelListBaseSelectionChanged<Pattern> messasge)
        {
            return messasge.Sender.Equals(this);
        }
    }
}
