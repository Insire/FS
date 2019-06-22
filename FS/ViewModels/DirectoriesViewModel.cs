using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS
{
    public sealed class DirectoriesViewModel : BusinessViewModelListBase<DirectoryViewModel>
    {
        private readonly SyncsViewModel _syncsViewModel;

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

        public DirectoriesViewModel(ICommandBuilder commandBuilder, SyncsViewModel syncsViewModel)
            : base(commandBuilder)
        {
            _syncsViewModel = syncsViewModel ?? throw new ArgumentNullException(nameof(syncsViewModel));
            Excludes = new Patterns(commandBuilder);
            Includes = new Patterns(commandBuilder);
        }

        protected override async Task RefreshInternal(CancellationToken token)
        {
            var includes = new HashSet<string>(Includes.Items.Where(p => !string.IsNullOrWhiteSpace(p.Value)).Select(p => p.Value).SelectMany(Globbing.Glob));
            var excludes = new HashSet<string>(Excludes.Items.Where(p => !string.IsNullOrWhiteSpace(p.Value)).Select(p => p.Value).SelectMany(Globbing.Glob));

            await AddRange(includes.Except(excludes).Select(p => new DirectoryViewModel(CommandBuilder, p)));
        }
    }
}
