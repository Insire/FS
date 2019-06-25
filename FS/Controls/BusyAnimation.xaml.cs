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

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(BusyAnimation),
            new PropertyMetadata(default(string)));

        public BusyAnimation()
        {
            InitializeComponent();
        }
    }
}
