using Akavache;
using DryIoc;
using MvvmScarletToolkit;
using System.Windows;

namespace FS
{
#pragma warning disable IDISP002 // Dispose member.
#pragma warning disable IDISP006 // Implement IDisposable.
#pragma warning disable IDISP003 // Dispose previous before re-assigning.

    public sealed partial class App : Application
    {
        private IContainer _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _container = CompositionRoot.Compose();

            var datacontext = _container.Resolve(typeof(MainViewModel), ifUnresolved: IfUnresolved.Throw);

            var window = new MainWindow()
            {
                DataContext = datacontext,
            };
            window.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            await ExitService.Default.ShutDown().ConfigureAwait(false);
            BlobCache.Shutdown().Wait();

            _container?.Dispose();
        }
    }

#pragma warning restore IDISP003 // Dispose previous before re-assigning.
#pragma warning restore IDISP006 // Implement IDisposable.
#pragma warning restore IDISP002 // Dispose member.
}
