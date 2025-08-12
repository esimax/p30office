using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POC.Module.Profile.Views;
using System.Windows;
using System.IO;
using POL.Lib.Common;

namespace POC.Module.Profile.Models
{
    public class MProfileReportSelect : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private DBCTProfileReport DynamicSelectedData { get; set; }

        #region CTOR
        public MProfileReportSelect(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitCommands();
            GetDynamicData();
            PopulateData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات فرم"; }
        }
        #endregion



        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (ReferenceEquals(_Title, value)) return;
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        #endregion


        #region ReportList
        private XPCollection<DBCTProfileReport> _ReportList;
        public XPCollection<DBCTProfileReport> ReportList
        {
            get { return _ReportList; }
            set
            {
                if (value == _ReportList)
                    return;

                _ReportList = value;
                RaisePropertyChanged("ReportList");
            }
        }
        #endregion

        #region SelectedReport
        private DBCTProfileReport _SelectedReport;
        public DBCTProfileReport SelectedReport
        {
            get { return _SelectedReport; }
            set
            {
                if (value == _SelectedReport)
                    return;

                _SelectedReport = value;
                RaisePropertyChanged("SelectedReport");

                UpdateColumns();
            }
        }

        private void UpdateColumns()
        {
            if (SelectedReport == null)
            {
                ColumnList = null;
                SelectedColumn = null;
            }
            else
            {
                try
                {
                    var sz = new XmlSerializer(typeof(MetaDataProfileReportPack));
                    var list = (MetaDataProfileReportPack)sz.Deserialize(new StringReader(SelectedReport.MetaData));

                    ColumnList = list.Items.ToList();
                    foreach (var col in ColumnList)
                    {
                        col.ProfileItem = DBCTProfileItem.FindByOid(ADatabase.Dxs, col.ProfileItemOid);
                        if (col.ProfileItem != null)
                            col.Image = HelperP30office.GetProfileItemImage(((DBCTProfileItem)col.ProfileItem).ItemType);
                    }
                    var c1 = ColumnList.Count;
                    ColumnList.RemoveAll(n => n.ProfileItem == null);
                    var c2 = ColumnList.Count;
                    if (c1 != c2)
                    {
                        UpdateSelectedReport();
                    }
                }
                catch
                {
                    ColumnList = null;
                }
            }
            SelectedColumn = null;
        }
        #endregion


        #region ColumnList
        private List<MetaDataProfileReportItem> _ColumnList;
        public List<MetaDataProfileReportItem> ColumnList
        {
            get { return _ColumnList; }
            set
            {
                if (value == _ColumnList)
                    return;

                _ColumnList = value;
                RaisePropertyChanged("ColumnList");
            }
        }
        #endregion

        #region SelectedColumn
        private MetaDataProfileReportItem _SelectedColumn;
        public MetaDataProfileReportItem SelectedColumn
        {
            get { return _SelectedColumn; }
            set
            {
                if (value == _SelectedColumn)
                    return;

                _SelectedColumn = value;
                RaisePropertyChanged("SelectedColumn");
            }
        }
        #endregion





        #region [METHODS]
        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => SelectedReport != null);
            CommandReportNew = new RelayCommand(ReportNew, () => true);
            CommandReportEdit = new RelayCommand(ReportEdit, () => SelectedReport != null);
            CommandReportDelete = new RelayCommand(ReportDelete, () => SelectedReport != null);

            CommandColumnNew = new RelayCommand(ColumnNew, () => SelectedReport != null);
            CommandColumnEdit = new RelayCommand(ColumnEdit, () => SelectedColumn != null);
            CommandColumnDelete = new RelayCommand(ColumnDelete, () => SelectedColumn != null);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp59 != "");
        }

        private void ReportNew()
        {
            var w = new WProfileReportAddEdit(null)
                        {
                            Owner = MainView
                        };
            if (w.ShowDialog() == true)
            {
                PopulateData();
            }
        }
        private void ReportEdit()
        {
            if (SelectedReport == null) return;
            var w = new WProfileReportAddEdit(SelectedReport)
            {
                Owner = MainView
            };
            if (w.ShowDialog() == true)
            {
                PopulateData();
            }
        }
        private void ReportDelete()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("گزارش انتخاب شده حذف شود؟", MainView);
            if (dr != MessageBoxResult.Yes) return;
            try
            {
                SelectedReport.Delete();
                PopulateData();
            }
            catch (Exception ex)
            {
                ALogger.Log(string.Format("MProfileReportSelect.ReportDelete Failed : {0}{1}", Environment.NewLine, ex), Category.Exception, Priority.Medium);
            }
        }

        private void ColumnNew()
        {
            var w = new WProfileReportColumnAddEdit(null)
            {
                Owner = MainView
            };
            if (w.ShowDialog() != true) return;
            if (w.DynamicSelectedData == null) return;
            if (_ColumnList == null)
                _ColumnList = new List<MetaDataProfileReportItem>();
            var maxOrder = _ColumnList.Any() ? _ColumnList.Max(n => n.Order) : 0;
            w.DynamicSelectedData.Order = maxOrder + 1;
            _ColumnList.Add(w.DynamicSelectedData);
            ColumnList = _ColumnList.OrderBy(n => n.Order).ToList();
            UpdateSelectedReport();
        }




        private void UpdateSelectedReport()
        {
            try
            {
                var sz = new XmlSerializer(typeof(MetaDataProfileReportPack));
                var sw = new StringWriter();
                sz.Serialize(sw, new MetaDataProfileReportPack { Items = ColumnList.ToArray() });
                SelectedReport.MetaData = sw.ToString();
                SelectedReport.Save();
            }
            catch
            {
                POLMessageBox.ShowError("بروز خطا در ثبت تغییرات.", MainView);
            }
        }
        private void ColumnEdit()
        {
            if (SelectedColumn == null) return;
            var w = new WProfileReportColumnAddEdit(SelectedColumn)
            {
                Owner = MainView
            };
            if (w.ShowDialog() != true) return;
            if (w.DynamicSelectedData == null) return;
            if (_ColumnList == null)
                _ColumnList = new List<MetaDataProfileReportItem>();
            w.DynamicSelectedData.Order = SelectedColumn.Order;
            _ColumnList.Remove(SelectedColumn);
            _ColumnList.Add(w.DynamicSelectedData);
            ColumnList = _ColumnList.OrderBy(n => n.Order).ToList();
            SelectedColumn = ColumnList.FirstOrDefault(n => n.Order == w.DynamicSelectedData.Order);
            UpdateSelectedReport();
        }
        private void ColumnDelete()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("ستون انتخاب شده حذف شود؟", MainView);
            if (dr != MessageBoxResult.Yes) return;

            ColumnList.Remove(SelectedColumn);
            UpdateSelectedReport();
            UpdateColumns();
            RaisePropertyChanged("ColumnList");
        }

        private void OK()
        {
            DynamicSelectedData = SelectedReport;
            MainView.DynamicSelectedData = DynamicSelectedData;
            MainView.DialogResult = true;
            MainView.Close();
        }

        private void PopulateData()
        {
            ReportList = DBCTProfileReport.GetAll(ADatabase.Dxs);
        }
        private void GetDynamicData()
        {
            DynamicSelectedData = MainView.DynamicSelectedData;
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp59);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandReportNew { get; set; }
        public RelayCommand CommandReportEdit { get; set; }
        public RelayCommand CommandReportDelete { get; set; }
        public RelayCommand CommandColumnNew { get; set; }
        public RelayCommand CommandColumnEdit { get; set; }
        public RelayCommand CommandColumnDelete { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }



}
