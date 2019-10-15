using LiteDB;
using MvvmScarletToolkit;
using MvvmScarletToolkit.Observables;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FS
{
    public sealed class SyncsViewModel : BusinessViewModelListBase<DirectoriesViewModel>
    {
        private const string Key = "FS.SyncsViewModel";
        private readonly string _connectionString;

        private string _root;
        public string Root
        {
            get { return _root; }
            set { SetValue(ref _root, value); }
        }

        public ICommand AddCommand { get; }

        public ICommand CloneCommand { get; }

        public SyncsViewModel(ICommandBuilder commandBuilder)
            : base(commandBuilder)
        {
            var dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FS");
            Directory.CreateDirectory(dbFolder);
            _connectionString = Path.Combine(dbFolder, "FS.db");

            AddCommand = commandBuilder
                .Create(Add, CanAdd)
                .WithSingleExecution(CommandManager)
                .WithBusyNotification(BusyStack)
                .Build();

            CloneCommand = commandBuilder
                .Create(Clone, CanClone)
                .WithSingleExecution(CommandManager)
                .WithBusyNotification(BusyStack)
                .WithCancellation()
                .Build();
        }

        protected override async Task RefreshInternal(CancellationToken token)
        {
            if (!IsLoaded)
            {
                await Load().ConfigureAwait(false);

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
        }

        private async Task Clone(CancellationToken token)
        {
            var dModel = new DirectoriesViewModel(CommandBuilder)
            {
                Id = Count + 1,
                Name = SelectedItem.Name + " Clone " + (Count + 1),
                Root = SelectedItem.Root,
                Interval = SelectedItem.Interval,
                TargetDirectory = SelectedItem.TargetDirectory,
                CopyLeftOnlyFiles = SelectedItem.CopyLeftOnlyFiles,
                UpdateChangedFiles = SelectedItem.UpdateChangedFiles,
                DeleteSameFiles = SelectedItem.DeleteSameFiles,
                DeleteRightOnlyFiles = SelectedItem.DeleteRightOnlyFiles,
                DeleteChangedFiles = SelectedItem.DeleteChangedFiles,
                RespectLastAccessDateTime = SelectedItem.RespectLastAccessDateTime,
                ShowLog = SelectedItem.ShowLog,
            };

            await dModel.Includes.AddRange(SelectedItem.Includes.Items).ConfigureAwait(false);
            await dModel.Excludes.AddRange(SelectedItem.Excludes.Items).ConfigureAwait(false);

            await Add(dModel).ConfigureAwait(false);
        }

        private bool CanClone()
        {
            return !IsBusy && SelectedItem != null;
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
            using (var db = new LiteDatabase(_connectionString))
            {
                var models = db.GetCollection<SyncsModel>();
                var id = new BsonValue(Key);
                var model = models.FindById(id);
                if (model is null)
                {
                    models.Insert(id, GetModel());
                }
                else
                {
                    models.Update(id, GetModel());
                }
            }
        }

        private async Task Load()
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var models = db.GetCollection<SyncsModel>();

                var model = models.FindById(new BsonValue(Key));

                if (model is null)
                    return;

                Root = model.Root;

                foreach (var item in model.Items)
                {
                    var dModel = new DirectoriesViewModel(CommandBuilder)
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Root = item.Root,
                        Interval = item.Interval,
                        TargetDirectory = item.TargetDirectory,
                        CopyLeftOnlyFiles = item.CopyLeftOnlyFiles,
                        UpdateChangedFiles = item.UpdateChangedFiles,
                        DeleteSameFiles = item.DeleteSameFiles,
                        DeleteRightOnlyFiles = item.DeleteRightOnlyFiles,
                        DeleteChangedFiles = item.DeleteChangedFiles,
                        RespectLastAccessDateTime = item.RespectLastAccessDateTime,
                        ShowLog = item.ShowLog
                    };

                    await dModel.Excludes.AddRange(item.Excludes.Select(p => new Pattern(CommandBuilder, CommandManager, dModel, p.Value))).ConfigureAwait(false);
                    await dModel.Includes.AddRange(item.Includes.Select(p => new Pattern(CommandBuilder, CommandManager, dModel, p.Value))).ConfigureAwait(false);
                    await Add(dModel).ConfigureAwait(false);
                }
            }
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
                    Interval = p.Interval,
                    TargetDirectory = p.TargetDirectory,
                    CopyLeftOnlyFiles = p.CopyLeftOnlyFiles,
                    UpdateChangedFiles = p.UpdateChangedFiles,
                    DeleteSameFiles = p.DeleteSameFiles,
                    DeleteRightOnlyFiles = p.DeleteRightOnlyFiles,
                    DeleteChangedFiles = p.DeleteChangedFiles,
                    RespectLastAccessDateTime = p.RespectLastAccessDateTime,
                    ShowLog = p.ShowLog,
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
