using System.Windows;
using System.Windows.Controls;
using POL.Lib.Interfaces;
using DevExpress.Xpf.Scheduler;

namespace POC.Module.Calendar.Views
{
    public partial class UCalendar : UserControl, IModuleRibbon
    {
        public UCalendar()
        {
            InitializeComponent();

            DynamicSchedulerControl.MonthView.HorizontalWeekCellStyle = (Style)this.FindResource("HorizontalWeekCellStyle");
            Loaded += (s, e) => scMain.Focus();
        }

        #region Implementation of IModuleRibbon

        public object GetRibbon()
        {
            return calendarRibbon;
        }

        public void UnloadChildRibbons()
        {
        }

        public void LoadChildRibbons()
        {
        }

        #endregion

        public SchedulerControl DynamicSchedulerControl
        {
            get { return scMain; }
        }
    }
}

