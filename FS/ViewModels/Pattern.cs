using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Commands;
using MvvmScarletToolkit.Observables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FS
{
    [DebuggerDisplay("{Value}")]
    public sealed class Pattern : ViewModelBase
    {
        private readonly DirectoriesViewModel _directoriesViewModel;

        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetValue(ref _value, value); }
        }

        private string _preview;
        public string Preview
        {
            get { return _preview; }
            set { SetValue(ref _preview, value); }
        }

        private string _rawString;
        public string RawString
        {
            get { return _rawString; }
            private set { SetValue(ref _rawString, value); }
        }

        public ICommand UpdatePreviewCommand { get; }

        public Pattern(ICommandBuilder commandBuilder, IScarletCommandManager commandManager, DirectoriesViewModel directoriesViewModel, string value)
            : base(commandBuilder)
        {
            _directoriesViewModel = directoriesViewModel ?? throw new ArgumentNullException(nameof(directoriesViewModel));
            Value = value ?? throw new ArgumentNullException(nameof(value));

            UpdatePreviewCommand = commandBuilder
                .Create(UpdatePreview, CanUpdatePreview)
                .WithSingleExecution(commandManager)
                .Build();
        }

        public Task UpdatePreview(CancellationToken token)
        {
            return Task.Run(() =>
            {
                var builder = new StringBuilder();
                foreach (var item in _directoriesViewModel.GetDirectories(Get()))
                {
                    builder.Append(item);
                    builder.Append(Environment.NewLine);

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }

                Dispatcher.Invoke(() => RawString = Value.Replace("*", string.Empty).Replace("\\", string.Empty).Replace("/", string.Empty));
                Dispatcher.Invoke(() => Preview = builder.ToString());
            }, token);
        }

        private IEnumerable<Pattern> Get()
        {
            yield return this;
        }

        public bool CanUpdatePreview()
        {
            return true;
        }
    }
}
