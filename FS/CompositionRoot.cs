using Akavache;
using DryIoc;
using MvvmScarletToolkit;
using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Commands;
using MvvmScarletToolkit.Observables;

namespace FS
{
    public static class CompositionRoot
    {
        internal static IContainer Compose(App app)
        {
            var c = new Container();
            c.Register<DirectoriesViewModel>(Reuse.Singleton);
            c.Register<SyncsViewModel>(Reuse.Singleton);
            c.Register<MainViewModel>(Reuse.Singleton);
            c.Register<ICommandBuilder, CommandBuilder>(Reuse.Singleton);

            c.Register<IBusyStack, BusyStack>(Reuse.Transient);

            Registrations.Start("FS");
            c.UseInstance(typeof(IBlobCache), BlobCache.UserAccount);
            c.UseInstance(typeof(IScarletCommandManager), ScarletCommandManager.Default);
            c.UseInstance(typeof(IScarletDispatcher), ScarletDispatcher.Default);
            c.UseInstance(typeof(IExitService), ExitService.Default);

            return c;
        }
    }
}
