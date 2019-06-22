using DryIoc;
using MvvmScarletToolkit;
using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Commands;
using MvvmScarletToolkit.Observables;
using System.Windows.Threading;

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
            c.Register<IScarletCommandManager, ScarletCommandManager>(Reuse.Singleton);
            c.Register<ICommandBuilder, CommandBuilder>(Reuse.Singleton);

            c.UseInstance(app.Dispatcher);
            c.Register<IScarletDispatcher, ScarletDispatcher>(made: Made.Of(() => new ScarletDispatcher(Arg.Of<Dispatcher>())), reuse: Reuse.Singleton);
            c.Register<IBusyStack, BusyStack>(Reuse.Transient);

            return c;
        }
    }
}
