using System.Windows;

namespace FS
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = CompositionRoot.Compose(this);
            var datacontext = container.Resolve(typeof(MainViewModel), ifUnresolved: DryIoc.IfUnresolved.Throw);

            var window = new MainWindow()
            {
                DataContext = datacontext,
            };
            window.Show();
        }
    }
}
