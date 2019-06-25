using GlobExpressions;
using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Commands;
using MvvmScarletToolkit.Observables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        }

        private async Task Add(CancellationToken token)
        {
            using (BusyStack.GetToken())
            {
                await Add(new Pattern(Content)).ConfigureAwait(false);
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
            var root = new DirectoryInfo(_directoriesViewModel.Root);

            return Items
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .Select(p => root.GlobDirectories(p.Value))
                .SelectMany(p => p)
                .Select(p => p.FullName);
        }
    }
}
