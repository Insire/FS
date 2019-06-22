using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FS
{
    public sealed class SyncsViewModel : BusinessViewModelListBase<SyncViewModel>
    {
        private DirectoryInfo _root;
        /// <summary>
        /// default value for new syncs
        /// </summary>
        public DirectoryInfo Root
        {
            get { return _root; }
            set { SetValue(ref _root, value); }
        }

        public SyncsViewModel(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
        }

        protected override Task RefreshInternal(CancellationToken token)
        {
            // + load data from storage
            return Task.CompletedTask;
        }
    }
}
