using System;
using System.Windows;
using System.Windows.Threading;

namespace FS
{
    public partial class CountDown
    {
        public static readonly DependencyProperty RemainingTimeProperty = DependencyProperty.Register(
            nameof(RemainingTime),
            typeof(TimeSpan),
            typeof(CountDown),
            new PropertyMetadata(TimeSpan.Zero));

        public static readonly DependencyProperty DueTimeProperty = DependencyProperty.Register(
            nameof(DueTime),
            typeof(DateTime),
            typeof(CountDown),
            new PropertyMetadata(DateTime.Now, DueTimeChanged));

        private static void DueTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is CountDown countDown))
                return;

            countDown.SetupTimer();
        }

        private readonly DispatcherTimer _timer;

        public DateTime DueTime
        {
            get { return (DateTime)GetValue(DueTimeProperty); }
            set { SetValue(DueTimeProperty, value); }
        }

        public TimeSpan RemainingTime
        {
            get { return (TimeSpan)GetValue(RemainingTimeProperty); }
            private set { SetValue(RemainingTimeProperty, value); }
        }

        public CountDown()
        {
            InitializeComponent();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
        }

        private void SetupTimer()
        {
            _timer.Tick -= TimerTick;
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        private void TimerTick(object o, EventArgs e)
        {
            var now = DateTime.Now;
            var remainingTime = DueTime.Ticks - now.Ticks;

            if (remainingTime > 0)
            {
                RemainingTime = DueTime - now;
            }
            else
            {
                _timer.Stop();
                RemainingTime = TimeSpan.Zero;
            }
        }
    }
}
