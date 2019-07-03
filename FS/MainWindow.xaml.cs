using Splat;
using Squirrel;
using System;
using System.Windows;

namespace FS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override async void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            using (var manager = new UpdateManager("D:\\Drop\\Nuget\\", "InFact.FileSync"))
            {
                await manager.UpdateApp().ConfigureAwait(false);
            }
        }
    }
}
