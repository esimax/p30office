using DevExpress.Xpf.Scheduler;
using DevExpress.Xpo;
using DevExpress.XtraScheduler;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Calendar.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace POC.Module.Calendar.Models
{
    public class MCalendar : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }


        private dynamic DynamicMainView { get; set; }
        private SchedulerControl DynamicSchedulerControl { get; set; }

        private readonly List<XPBaseObject> _ToDeleteList = new List<XPBaseObject>();



        public MCalendar(object mainView)
        {
            DynamicMainView = mainView;

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();

            AMembership.OnMembershipStatusChanged +=
                (s, e) =>
                {
                    if (e.Status == EnumMembershipStatus.AfterLogout || e.Status == EnumMembershipStatus.AfterLogin)
                    {
                        PopulateData();
                    }
                };
            InitDynamics();

            NavigatorDateTime = DateTime.Now;
            SwitchToDayView = true;
            InitCommands();

            PopulateData();
        }







        #region NavigatorDateTime
        private DateTime _NavigatorDateTime;
        public DateTime NavigatorDateTime
        {
            get { return _NavigatorDateTime; }
            set
            {
                if (value == _NavigatorDateTime)
                    return;

                _NavigatorDateTime = value;

                DynamicSchedulerControl.ActiveView.GotoTimeInterval(new TimeInterval(value, value.AddHours(23)));
                RaisePropertyChanged("NavigatorDateTime");
            }
        }
        #endregion

        #region SwitchToDayView
        public bool SwitchToDayView
        {
            get { return DynamicSchedulerControl.ActiveViewType == SchedulerViewType.Day; }
            set
            {
                if (value)
                    DynamicSchedulerControl.ActiveViewType = SchedulerViewType.Day;
            }
        }
        #endregion
        #region SwitchToWorkWeekView
        public bool SwitchToWorkWeekView
        {
            get { return DynamicSchedulerControl.ActiveViewType == SchedulerViewType.WorkWeek; }
            set
            {
                if (value)
                    DynamicSchedulerControl.ActiveViewType = SchedulerViewType.WorkWeek;
            }
        }
        #endregion
        #region SwitchToWeekView
        public bool SwitchToWeekView
        {
            get { return DynamicSchedulerControl.ActiveViewType == SchedulerViewType.Week; }
            set
            {
                if (value)
                    DynamicSchedulerControl.ActiveViewType = SchedulerViewType.Week;
            }
        }
        #endregion
        #region SwitchToMonthView
        public bool SwitchToMonthView
        {
            get { return DynamicSchedulerControl.ActiveViewType == SchedulerViewType.Month; }
            set
            {
                if (value)
                    DynamicSchedulerControl.ActiveViewType = SchedulerViewType.Month;
            }
        }
        #endregion
        #region SwitchToTimelineView
        public bool SwitchToTimelineView
        {
            get { return DynamicSchedulerControl.ActiveViewType == SchedulerViewType.Timeline; }
            set
            {
                if (value)
                    DynamicSchedulerControl.ActiveViewType = SchedulerViewType.Timeline;
            }
        }
        #endregion



        private void InitDynamics()
        {
            DynamicSchedulerControl = DynamicMainView.DynamicSchedulerControl;
            DynamicSchedulerControl.ActiveViewChanged += (s, e) => UpdateViewForBarCheckItem();
            DynamicSchedulerControl.GotoDateFormShowing +=
                (s, e) =>
                {
                    var w = new WGotoDate(DynamicSchedulerControl,
                                          e.Views, e.Date,
                                          e.SchedulerViewType) { Owner = Window.GetWindow(DynamicMainView) };
                    if (w.ShowDialog() != true)
                    {
                        NavigatorDateTime = w.DynamicSelectedDateTime;
                        DynamicSchedulerControl.ActiveViewType = w.DynamicSchedulerViewType;
                    }
                    var w2 = Window.GetWindow(DynamicMainView);
                    if (w2 != null)
                        w2.Activate();
                    e.Cancel = true;
                };

            DynamicSchedulerControl.EditAppointmentFormShowing +=
                (s, e) =>
                {
                    e.Form = new UAppointmentForm(DynamicSchedulerControl, e.Appointment, Window.GetWindow(DynamicMainView));
                    e.AllowResize = false;
                };

            DynamicSchedulerControl.Storage.RemindersCheckInterval = 3000;


            DynamicSchedulerControl.WorkDays.Clear();
            DynamicSchedulerControl.WorkDays.Add(WeekDays.Saturday);
            DynamicSchedulerControl.WorkDays.Add(WeekDays.Sunday);
            DynamicSchedulerControl.WorkDays.Add(WeekDays.Monday);
            DynamicSchedulerControl.WorkDays.Add(WeekDays.Tuesday);
            DynamicSchedulerControl.WorkDays.Add(WeekDays.Wednesday);
            DynamicSchedulerControl.WorkDays.Add(WeekDays.Thursday);
            DynamicSchedulerControl.OptionsView.FirstDayOfWeek = FirstDayOfWeek.Saturday;

            DynamicSchedulerControl.Views.MonthView.CompressWeekend = false;
            DynamicSchedulerControl.Views.MonthView.ShowWeekend = true;

            DynamicSchedulerControl.Storage.AppointmentsChanged += StorageModified;
            DynamicSchedulerControl.Storage.AppointmentsInserted += StorageModified;
            DynamicSchedulerControl.Storage.AppointmentDeleting += StorageAppointmentDeleting;
            DynamicSchedulerControl.Storage.AppointmentsDeleted += StorageAppointmentsDeleted;

            DynamicSchedulerControl.Storage.AppointmentStorage.Mappings =
                new AppointmentMapping(DynamicSchedulerControl.Storage.AppointmentStorage)
                    {
                        AppointmentId = "Oid",
                        Subject = "Subject",
                        Start = "StartTime",
                        End = "EndTime",
                        AllDay = "AllDay",
                        Label = "Label",
                        Status = "Status",
                        Description = "Description",
                        Type = "EventType",
                        RecurrenceInfo = "RecurrenceInfo",
                        ReminderInfo = "ReminderInfo",
                    };

            #region Related Objects
            DynamicSchedulerControl.Storage.AppointmentStorage.CustomFieldMappings.Add(
                    new SchedulerCustomFieldMapping
                    {
                        Member = "CategoryOid",
                        Name = "CategoryOid",
                    });
            DynamicSchedulerControl.Storage.AppointmentStorage.CustomFieldMappings.Add(
                new SchedulerCustomFieldMapping
                {
                    Member = "ContactOid",
                    Name = "ContactOid",
                });
            DynamicSchedulerControl.Storage.AppointmentStorage.CustomFieldMappings.Add(
                new SchedulerCustomFieldMapping
                {
                    Member = "ProfileItemOid",
                    Name = "ProfileItemOid",
                });
            #endregion

            #region Permissions
            DynamicSchedulerControl.Storage.AppointmentStorage.CustomFieldMappings.Add(
                    new SchedulerCustomFieldMapping
                        {
                            Member = "ViewPermissionType",
                            Name = "ViewPermissionType",
                            ValueType = FieldValueType.Integer
                        });
            DynamicSchedulerControl.Storage.AppointmentStorage.CustomFieldMappings.Add(
                new SchedulerCustomFieldMapping
                {
                    Member = "ViewOid",
                    Name = "ViewOid",
                });

            DynamicSchedulerControl.Storage.AppointmentStorage.CustomFieldMappings.Add(
                new SchedulerCustomFieldMapping
                    {
                        Member = "EditPermissionType",
                        Name = "EditPermissionType",
                        ValueType = FieldValueType.Integer
                    });
            DynamicSchedulerControl.Storage.AppointmentStorage.CustomFieldMappings.Add(
                new SchedulerCustomFieldMapping
                {
                    Member = "EditOid",
                    Name = "EditOid",
                });

            DynamicSchedulerControl.Storage.AppointmentStorage.CustomFieldMappings.Add(
                new SchedulerCustomFieldMapping
                    {
                        Member = "DeletePermissionType",
                        Name = "DeletePermissionType",
                        ValueType = FieldValueType.Integer
                    });
            DynamicSchedulerControl.Storage.AppointmentStorage.CustomFieldMappings.Add(
                new SchedulerCustomFieldMapping
                {
                    Member = "DeleteOid",
                    Name = "DeleteOid",
                });

            DynamicSchedulerControl.Storage.AppointmentStorage.CustomFieldMappings.Add(
                new SchedulerCustomFieldMapping
                {
                    Member = "UserCreated",
                    Name = "UserCreated",
                });
            #endregion

            DynamicSchedulerControl.Storage.AppointmentChanging +=
                (s, e) =>
                {
                    try
                    {
                        var apt = (Appointment)e.Object;
                        var user = Convert.ToString(apt.CustomFields["UserCreated"]);
                        var editOid = Guid.Parse(Convert.ToString(apt.CustomFields["EditOid"]));
                        var editType = Convert.ToInt32(apt.CustomFields["EditPermissionType"]);

                        if (AMembership.ActiveUser.UserName == "admin") return;
                        if (user == AMembership.ActiveUser.UserName) return;

                        if (editType == 1 && editOid == AMembership.ActiveUser.UserID)
                            return;
                        if (editType == 2 && AMembership.ActiveUser.RolesOid != null &&
                            AMembership.ActiveUser.RolesOid.Contains(editOid))
                            return;

                        e.Cancel = true;

                    }
                    catch
                    {
                        e.Cancel = true;
                    }
                };

            DynamicSchedulerControl.Storage.AppointmentDeleting +=
                (s, e) =>
                {
                    try
                    {
                        var apt = (Appointment)e.Object;

                        var user = Convert.ToString(apt.CustomFields["UserCreated"]);
                        var deleteOid = Guid.Parse(Convert.ToString(apt.CustomFields["DeleteOid"]));
                        var deleteType = Convert.ToInt32(apt.CustomFields["DeletePermissionType"]);

                        if (AMembership.ActiveUser.UserName == "admin") return;
                        if (user == AMembership.ActiveUser.UserName) return;

                        if (deleteType == 1 && deleteOid == AMembership.ActiveUser.UserID)
                            return;
                        if (deleteType == 2 && AMembership.ActiveUser.RolesOid != null &&
                            AMembership.ActiveUser.RolesOid.Contains(deleteOid))
                            return;

                        e.Cancel = true;
                    }
                    catch
                    {
                        e.Cancel = true;
                    }
                };
        }
        private void InitCommands()
        {
            CommandRefresh = new RelayCommand(Refresh);
            CommandNavigateViewBackward = new RelayCommand(NavigateViewBackward, () => true);
            CommandNavigateViewForward = new RelayCommand(NavigateViewForward, () => true);
            CommandGotoToday = new RelayCommand(GotoToday, () => true);
            CommandZoomIn = new RelayCommand(ZoomIn, () => true);
            CommandZoomOut = new RelayCommand(ZoomOut, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp21 != "");
        }

        private void Refresh()
        {
            DynamicSchedulerControl.Storage.AppointmentStorage.DataSource = null;
            PopulateData();
            DynamicSchedulerControl.UpdateLayout();
        }
        private void NavigateViewBackward()
        {
            HelperUtils.Try(() => { NavigatorDateTime = NavigatorDateTime.AddDays(-1); });
        }
        private void NavigateViewForward()
        {
            HelperUtils.Try(() => { NavigatorDateTime = NavigatorDateTime.AddDays(1); });
        }
        private void GotoToday()
        {
            HelperUtils.Try(() =>
            {
                NavigatorDateTime = DateTime.Now.Date;
            });
        }
        private void ZoomIn()
        {
            DynamicSchedulerControl.ActiveView.ZoomIn();
        }
        private void ZoomOut()
        {
            DynamicSchedulerControl.ActiveView.ZoomOut();
        }



        private void PopulateData()
        {
            if (!AMembership.IsAuthorized)
            {
                DynamicSchedulerControl.Storage.AppointmentStorage.DataSource = null;
            }
            else
            {
                AllData = DBCASchedule.GetByRolesAndUsers(ADatabase.Dxs, AMembership.ActiveUser.UserID, AMembership.ActiveUser.UserName, AMembership.ActiveUser.RolesOid);
                DynamicSchedulerControl.Storage.AppointmentStorage.DataSource = AllData;

            }
        }
        private void UpdateViewForBarCheckItem()
        {
            RaisePropertyChanged("SwitchToDayView");
            RaisePropertyChanged("SwitchToWorkWeekView");
            RaisePropertyChanged("SwitchToWeekView");
            RaisePropertyChanged("SwitchToMonthView");
            RaisePropertyChanged("SwitchToTimelineView");
        }
        private void StorageModified(object sender, PersistentObjectsEventArgs e)
        {
            foreach (Appointment apt in e.Objects)
            {
                var o = apt.GetSourceObject(DynamicSchedulerControl.Storage.GetCoreStorage()) as XPBaseObject;
                if (o != null)
                {
                    o.Save();
                    o.Save();
                }
            }
        }
        private void StorageAppointmentDeleting(object sender, PersistentObjectCancelEventArgs e)
        {
            var o = e.Object.GetSourceObject((SchedulerStorageBase)sender) as XPBaseObject;
            if (o != null)
            {
                _ToDeleteList.Add(o);
            }
        }
        private void StorageAppointmentsDeleted(object sender, PersistentObjectsEventArgs e)
        {
            if (_ToDeleteList.Count != 0)
            {
                foreach (var db in _ToDeleteList)
                {
                    db.Delete();
                    db.Save();
                }
                _ToDeleteList.Clear();
            }
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp21);
        }


        #region [COMMANDS]
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandNavigateViewBackward { get; set; }
        public RelayCommand CommandNavigateViewForward { get; set; }
        public RelayCommand CommandGotoToday { get; set; }
        public RelayCommand CommandZoomIn { get; set; }
        public RelayCommand CommandZoomOut { get; set; }

        public RelayCommand CommandSwitchToDayView { get; set; }
        public RelayCommand CommandSwitchToWorkWeekView { get; set; }
        public RelayCommand CommandSwitchToWeekView { get; set; }

        public RelayCommand CommandSwitchToMonthView { get; set; }
        public RelayCommand CommandSwitchToTimelineView { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion


        public XPCollection<DBCASchedule> AllData { get; set; }
    }
}
