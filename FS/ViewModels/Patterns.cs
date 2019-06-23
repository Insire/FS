using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using MvvmScarletToolkit.Commands;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;

namespace FS
{
    [DebuggerDisplay("{Content} - {Count}")]
    public sealed class Patterns : BusinessViewModelListBase<Pattern>
    {
        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetValue(ref _content, value); }
        }

        public ICommand AddCommand { get; }

        public Patterns(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
            AddCommand = commandBuilder
                .Create(Add, CanAdd)
                .WithSingleExecution(CommandManager)
                .Build();
        }

        private async Task Add(CancellationToken token)
        {
            using (BusyStack.GetToken())
            {
                await Add(new Pattern(CommandBuilder)
                {
                    Value = Content,
                });
            }
        }

        protected override Task UnloadInternal(CancellationToken token)
        {
            return base.UnloadInternal(token);
        }

        private bool CanAdd()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(Content);
        }

        protected override Task RefreshInternal(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
