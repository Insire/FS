using System;
using System.Windows;
using System.Windows.Threading;

namespace FS
{
    public partial class CountDown
    {
        /// <summary>Identifies the <see cref="RemainingTime"/> dependency property.</summary>
        public static readonly DependencyProperty RemainingTimeProperty = DependencyProperty.Register(
            nameof(RemainingTime),
            typeof(TimeSpan),
            typeof(CountDown),
            new PropertyMetadata(TimeSpan.Zero));

        /// <summary>Identifies the <see cref="DueTime"/> dependency property.</summary>
        public static readonly DependencyProperty DueTimeProperty = DependencyProperty.Register(
            nameof(DueTime),
            typeof(DateTime),
            typeof(CountDown),
            new PropertyMetadata(DateTime.Now, OnDueTimeChanged));

        private static void OnDueTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is CountDown countDown))
                return;

            countDown.OnDueTimeChanged();
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

        private void OnDueTimeChanged()
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
                SetCurrentValue(RemainingTimeProperty, DueTime - now);
            }
            else
            {
                _timer.Stop();
                SetCurrentValue(RemainingTimeProperty, TimeSpan.Zero);
            }
        }
    }
}
