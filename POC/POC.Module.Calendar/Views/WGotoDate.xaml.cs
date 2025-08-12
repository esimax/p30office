using System;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler;
using POL.Lib.Utils;


namespace POC.Module.Calendar.Views
{
    public partial class WGotoDate : DXWindow
    {
        public WGotoDate(SchedulerControl control, SchedulerViewRepository views, DateTime date, SchedulerViewType viewType)
        {
            InitializeComponent();
            DynamicSchedulerControl = control;
            DynamicViewRepository = views;
            DynamicSelectedDateTime = date;
            DynamicSchedulerViewType = viewType;

            Loaded += (s, e) =>
            {
                var model = new Models.MGotoDate(this);
                DataContext = model;
                firstFocused.Focus();
                HelperLocalize.SetLanguageToDefault();
            };
        }

        public SchedulerControl DynamicSchedulerControl { get; set; }
        public SchedulerViewRepository DynamicViewRepository { get; set; }
        public DateTime DynamicSelectedDateTime { get; set; }
        public SchedulerViewType DynamicSchedulerViewType { get; set; }
    }

}
