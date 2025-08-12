using System;
using System.Windows.Threading;

namespace POL.WPF.DXControls.POLClock
{
    public partial class POLClockAnalog
    {
        public POLClockAnalog()
        {
            InitializeComponent();
            StartTimer();
        }

        private void StartTimer()
        {
            var myDispatcherTimer = new DispatcherTimer
            {Interval = new TimeSpan(0, 0, 0, 0, 1000)};
            myDispatcherTimer.Tick += Each_Tick;
            myDispatcherTimer.Start();
        }

        private void Each_Tick(object o, EventArgs sender)
        {
            var hourRotateValue = Convert.ToDouble(DateTime.Now.Hour.ToString());
            var minuteRotateValue = Convert.ToDouble(DateTime.Now.Minute.ToString());
            var secondRotateValue = Convert.ToDouble(DateTime.Now.Second.ToString());
            hourRotateValue = (hourRotateValue + minuteRotateValue/60)*30;
            minuteRotateValue = (minuteRotateValue + secondRotateValue/60)*6;
            secondRotateValue = Convert.ToDouble(DateTime.Now.Second.ToString())*6;
            SecondRotate.Angle = secondRotateValue;
            MinuteRotate.Angle = minuteRotateValue;
            HourRotate.Angle = hourRotateValue;
        }
    }
}
