using DryIoc;
using MvvmScarletToolkit;
using MvvmScarletToolkit.Abstractions;
using MvvmScarletToolkit.Observables;

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

            c.Register<IBusyStack, BusyStack>(Reuse.Transient);

            c.UseInstance(ScarletCommandBuilder.Default);
            c.UseInstance(ScarletCommandManager.Default);
            c.UseInstance(ScarletDispatcher.Default);
            c.UseInstance(ScarletExitService.Default);
            c.UseInstance(ScarletMessenger.Default);
            c.UseInstance(ScarletWeakEventManager.Default);

            return c;
        }
    }
}
