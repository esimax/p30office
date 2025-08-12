using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.UI;
using DevExpress.XtraScheduler;
using POC.Module.Calendar.Models;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;

namespace POC.Module.Calendar.Views
{
    public partial class UAppointmentForm : UserControl
    {

        public UAppointmentForm(SchedulerControl control, Appointment appointment, Window mainWindow)
        {
            InitializeComponent();
            InnerSchedulerControl = control;
            InnerAppointment = appointment;
            ParentWindow = mainWindow;

            Controller = new P30OfficeAppointmentFormController(control, appointment);
            RecurrenceVisualController = new StandaloneRecurrenceVisualController(Controller);

            InitCommands();
            Loaded +=
                (s, e) =>
                {
                    SchedulerFormBehavior.SetTitle(this, "فرم ثبت رویداد");
                    HelperLocalize.SetLanguageToDefault();

                    Task.Factory.StartNew(
                        () =>
                        {
                            System.Threading.Thread.Sleep(300);
                            Application.Current.Dispatcher.Invoke(
                                DispatcherPriority.Normal,
                                new Action(
                                () =>
                                {
                                    firstFocused.Focus();
                                    firstFocused.SelectAll();
                                }));
                        });

                    if (!Controller.ShouldShowRecurrenceButton || !InnerAppointment.IsRecurring)
                    {
                        tabReccurence.Visibility = Visibility.Collapsed;
                    }
                    if (Controller.IsNewAppointment)
                    {
                        Controller.ViewPermissionType = 0;
                        Controller.EditPermissionType = 0;
                        Controller.DeletePermissionType = 0;
                    }
                    tabSecurity.Visibility = Controller.IsOwner ? Visibility.Visible : Visibility.Collapsed;
                };

            PopulatePermissionTypeList();
        }



        public static readonly DependencyProperty FirstDayOfWeekProperty;
        static UAppointmentForm()
        {
            FirstDayOfWeekProperty = DependencyPropertyHelper.RegisterProperty<RecurrenceForm, DayOfWeek>("FirstDayOfWeek", DayOfWeek.Saturday, (DevExpress.Xpf.Core.Native.DependencyPropertyChangedCallback<RecurrenceForm, DayOfWeek>)null);
        }







        private SchedulerControl InnerSchedulerControl { get; set; }
        private Appointment InnerAppointment { get; set; }
        private Window ParentWindow { get; set; }

        #region TimeEditMask
        public string TimeEditMask
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern; }
        }
        #endregion
        #region Controller
        public P30OfficeAppointmentFormController Controller { get; set; }
        #endregion
        #region CanEditRecurrence
        public bool CanEditRecurrence
        {
            get { return Controller == null || Controller.IsNewAppointment; }
        }
        #endregion
        #region RecurrenceVisualController
        public StandaloneRecurrenceVisualController RecurrenceVisualController { get; set; }
        #endregion
        #region FirstDayOfWeek
        public DayOfWeek FirstDayOfWeek
        {
            get
            {
                return (DayOfWeek)GetValue(FirstDayOfWeekProperty);
            }
            set
            {
                SetValue(FirstDayOfWeekProperty, value);
            }
        }
        #endregion
        #region ReadOnly
        public bool ReadOnly
        {
            get
            {
                return false;
            }
        }
        #endregion
        #region TimeZoneHelper
        public TimeZoneHelper TimeZoneHelper
        {
            get
            {
                return RecurrenceVisualController.Controller.TimeZoneHelper;
            }
        }
        #endregion
        #region PermissionTypeList
        public List<string> PermissionTypeList { get; set; }
        #endregion



        #region [METHODS]
        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (InnerAppointment.IsRecurring && Controller.IsNewAppointment)
                        RecurrenceVisualController.ApplyRecurrence();
                    InnerAppointment.HasReminder = true;
                    Controller.ApplyChanges();
                    SchedulerFormBehavior.Close(this, true);
                    ParentWindow.Activate();
                });
            CommandCancel = new RelayCommand(
                () =>
                {
                    SchedulerFormBehavior.Close(this, false);
                    ParentWindow.Activate();
                });
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp20 != "");
        }
        private void PopulatePermissionTypeList()
        {
            PermissionTypeList = new List<string> { "فقط خودم", "كاربر", "سطح دسترسی", "همه" };
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp20);
        } 
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCancel { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion




    }
}
