using DryIoc;
using MvvmScarletToolkit;
using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Commands;
using MvvmScarletToolkit.Observables;
using System.ComponentModel;

namespace FS
{
    public static class CompositionRoot
    {
        internal static DryIoc.IContainer Compose()
        {
            var c = new DryIoc.Container();

            c.Register<DirectoriesViewModel>(Reuse.Singleton);
            c.Register<SyncsViewModel>(Reuse.Singleton);
            c.Register<MainViewModel>(Reuse.Singleton);
            c.Register<ICommandBuilder, CommandBuilder>(Reuse.Singleton);

            c.Register<IBusyStack, BusyStack>(Reuse.Transient);

            c.UseInstance(typeof(IScarletCommandManager), ScarletCommandManager.Default);
            c.UseInstance(typeof(IScarletDispatcher), ScarletDispatcher.Default);
            c.UseInstance(typeof(IExitService), ExitService.Default);
            c.UseInstance(typeof(IScarletMessenger), ScarletMessenger.Default);
            c.UseInstance(typeof(IWeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>), ScarletWeakEventManager.Default);

            return c;
        }
    }
}
