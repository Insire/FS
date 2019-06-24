using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using MvvmScarletToolkit.Commands;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

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

        private bool CanAdd()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(Content);
        }

        protected override Task RefreshInternal(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public IEnumerable<string> GetDirectories()
        {
            return Items
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .Select(p => root.GlobDirectories(p.Value))
                .SelectMany(p => p)
                .Select(p => p.FullName);
        }
    }
}
