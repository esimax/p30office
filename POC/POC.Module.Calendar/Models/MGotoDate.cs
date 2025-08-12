using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Services;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Calendar.Models
{
    public class MGotoDate : NotifyObjectBase
    {
        private dynamic MainView { get; set; }
        public SchedulerControl DynamicSchedulerControl { get; set; }
        public SchedulerViewRepository DynamicViewRepository { get; set; }
        public DateTime DynamicSelectedDateTime { get; set; }
        public SchedulerViewType DynamicSchedulerViewType { get; set; }

        public MGotoDate(dynamic mainView)
        {
            MainView = mainView;
            InitDynamics();

            Title = SchedulerControlLocalizer.GetString(SchedulerControlStringId.Caption_GotoDate);
            SelectedDate = DynamicSelectedDateTime;
            PopulateActiveViews(DynamicViewRepository);
            InitCommand();
        }


        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (value == _Title)
                    return;

                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        #endregion

        #region SelectedDate
        private DateTime _SelectedDate;
        public DateTime SelectedDate
        {
            get { return _SelectedDate; }
            set
            {
                if (value == _SelectedDate)
                    return;

                _SelectedDate = value;
                RaisePropertyChanged("SelectedDate");
            }
        }
        #endregion
        #region ViewList
        private List<Tuple<string, SchedulerViewType>> _ViewList;
        public List<Tuple<string, SchedulerViewType>> ViewList
        {
            get { return _ViewList; }
            set
            {
                if (value == _ViewList)
                    return;

                _ViewList = value;
                RaisePropertyChanged("ViewList");
            }
        }
        #endregion
        #region SelectedView
        private Tuple<string, SchedulerViewType> _SelectedView;
        public Tuple<string, SchedulerViewType> SelectedView
        {
            get { return _SelectedView; }
            set
            {
                if (Equals(value, _SelectedView))
                    return;

                _SelectedView = value;
                RaisePropertyChanged("SelectedView");
            }
        }
        #endregion

        private void InitDynamics()
        {
            DynamicSchedulerControl = MainView.DynamicSchedulerControl;
            DynamicViewRepository = MainView.DynamicViewRepository;
            DynamicSelectedDateTime = MainView.DynamicSelectedDateTime;
            DynamicSchedulerViewType = MainView.DynamicSchedulerViewType;
        }
        private void PopulateActiveViews(SchedulerViewRepository views)
        {
            ViewList = new List<Tuple<string, SchedulerViewType>>();
            var count = views.Count;
            for (var i = 0; i < count; i++)
            {
                var base2 = views[i];
                if (base2.Enabled)
                {
                    ViewList.Add(new Tuple<string, SchedulerViewType>(base2.DisplayName, base2.Type));
                }
            }

            SelectedView = (from n in ViewList
                            where DynamicSchedulerViewType == n.Item2
                            select n).FirstOrDefault();
        }
        private void InitCommand()
        {
            CommandOK = new RelayCommand(OK);
        }

        private void OK()
        {
            var service = (IDateTimeNavigationService)DynamicSchedulerControl.GetService(typeof(IDateTimeNavigationService));
            if (service != null)
            {
                HelperUtils.Try(
                    () =>
                    {
                        service.GoToDate(SelectedDate, SelectedView.Item2);
                        MainView.DynamicSelectedDateTime = DynamicSelectedDateTime;
                        MainView.DynamicSchedulerViewType = DynamicSchedulerViewType;
                        MainView.DialogResult = true;
                        MainView.Close();
                    });
            }

        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion
    }
}
