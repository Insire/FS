using System.Windows;

namespace FS
{
    public partial class BusyAnimation
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>Identifies the <see cref="Text"/> dependency property.</summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(BusyAnimation),
            new PropertyMetadata(default(string)));

        public BusyAnimation()
        {
            InitializeComponent();
        }
    }
}
