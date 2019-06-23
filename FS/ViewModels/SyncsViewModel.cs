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

        private DirectoryInfo _root;
        public DirectoryInfo Root
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
                    Root = new DirectoryInfo(".");
                }
            }
            else
            {
                if (Root is null)
                {
                    Root = new DirectoryInfo(".");
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
            });

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
                Root = new DirectoryInfo(model.Root);

                foreach (var item in model.Items)
                {
                    var dModel = new DirectoriesViewModel(CommandBuilder)
                    {
                        Id = item.Id,
                        Name = item.Name,
                        TargetDirectory = new DirectoryInfo(item.TargetDirectory),
                    };
                    await dModel.Excludes.AddRange(item.Excludes.Select(p => new Pattern(CommandBuilder)
                    {
                        Value = p.Value
                    }));

                    await dModel.Includes.AddRange(item.Includes.Select(p => new Pattern(CommandBuilder)
                    {
                        Value = p.Value
                    }));
                    await Add(dModel);
                }
            });
        }

        private SyncsModel GetModel()
        {
            var model = new SyncsModel()
            {
                Root = Root.FullName,
                Items = Items.Select(p => new DirectoriesModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    TargetDirectory = p.TargetDirectory.FullName,
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

            return model;
        }
    }
}
