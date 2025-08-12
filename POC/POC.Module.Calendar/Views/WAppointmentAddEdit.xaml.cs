using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler;
using POL.Lib.Utils;


namespace POC.Module.Calendar.Views
{
    public partial class WAppointmentAddEdit : DXWindow
    {
        public WAppointmentAddEdit(SchedulerControl control, Appointment appointment)
        {
            InitializeComponent(); 
            DynamicSchedulerControl = control;
            DynamicAppointment = appointment;
            

            Loaded += (s, e) =>
            {
                var model = new Models.MGotoDate(this);
                DataContext = model;
                HelperLocalize.SetLanguageToDefault();
            };
        }

        public SchedulerControl DynamicSchedulerControl { get; set; }
        public Appointment DynamicAppointment { get; set; }
    }
}
