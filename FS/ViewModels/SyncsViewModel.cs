using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;
using MvvmScarletToolkit.Commands;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Akavache;
using System;
using System.Linq;

namespace FS
{
    public sealed class SyncsViewModel : BusinessViewModelListBase<DirectoriesViewModel>
    {
        private const string _key = "FS.SyncsViewModel";
        private readonly IBlobCache _cache;

        private string _root;
        public string Root
        {
            get { return _root; }
            set { SetValue(ref _root, value); }
        }

        public ICommand AddCommand { get; }

        public SyncsViewModel(ICommandBuilder commandBuilder, IBlobCache cache)
            : base(commandBuilder)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));

            AddCommand = commandBuilder
                .Create(Add, CanAdd)
                .WithSingleExecution(CommandManager)
                .Build();
        }

        protected override Task RefreshInternal(CancellationToken token)
        {
            if (!IsLoaded)
            {
                Load();

                if (Root is null)
                {
                    Root = new DirectoryInfo(".").FullName;
                }
            }
            else
            {
                if (Root is null)
                {
                    Root = new DirectoryInfo(".").FullName;
                }
            }

            // + load data from storage
            return Task.CompletedTask;
        }

        private async Task Add(CancellationToken token)
        {
            await Add(new DirectoriesViewModel(CommandBuilder)
            {
                TargetDirectory = Root,
                Name = "new Sync" + (Count + 1),
                Id = Count + 1
            }).ConfigureAwait(false);

            if (SelectedItem is null)
            {
                SelectedItem = Items[0];
            }
            Save();
        }

        private bool CanAdd()
        {
            return !IsBusy && !(Root is null);
        }

        protected override Task UnloadInternal(CancellationToken token)
        {
            Save();
            return base.UnloadInternal(token);
        }

        private void Save()
        {
            _cache.InsertObject(_key, GetModel());
        }

        private void Load()
        {
            _cache.GetObject<SyncsModel>(_key).Subscribe(async model =>
            {
                Root = model.Root;

                foreach (var item in model.Items)
                {
                    var dModel = new DirectoriesViewModel(CommandBuilder)
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Root = item.Root,
                        TargetDirectory = item.TargetDirectory,
                        CopyEmptyDirectories = item.CopyEmptyDirectories,
                        CopyLeftOnlyFiles = item.CopyLeftOnlyFiles,
                        UpdateChangedFiles = item.UpdateChangedFiles,
                        DeleteSameFiles = item.DeleteSameFiles,
                        DeleteRightOnlyFiles = item.DeleteRightOnlyFiles,
                        DeleteRightOnlyDirectories = item.DeleteRightOnlyDirectories,
                        DeleteChangedFiles = item.DeleteChangedFiles,
                    };
                    await dModel.Excludes.AddRange(item.Excludes.Select(p => new Pattern(p.Value))).ConfigureAwait(false);
                    await dModel.Includes.AddRange(item.Includes.Select(p => new Pattern(p.Value))).ConfigureAwait(false);
                    await Add(dModel).ConfigureAwait(false);
                }
            });
        }

        private SyncsModel GetModel()
        {
            return new SyncsModel()
            {
                Root = Root,
                Items = Items.Select(p => new DirectoriesModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Root = p.Root,
                    TargetDirectory = p.TargetDirectory,
                    CopyEmptyDirectories = p.CopyEmptyDirectories,
                    CopyLeftOnlyFiles = p.CopyLeftOnlyFiles,
                    UpdateChangedFiles = p.UpdateChangedFiles,
                    DeleteSameFiles = p.DeleteSameFiles,
                    DeleteRightOnlyFiles = p.DeleteRightOnlyFiles,
                    DeleteRightOnlyDirectories = p.DeleteRightOnlyDirectories,
                    DeleteChangedFiles = p.DeleteChangedFiles,
                    Excludes = p.Excludes.Items.Select(o => new PatternModel()
                    {
                        Value = o.Value,
                    }).ToList(),
                    Includes = p.Includes.Items.Select(o => new PatternModel()
                    {
                        Value = o.Value,
                    }).ToList(),
                }).ToList(),
            };
        }
    }
}
