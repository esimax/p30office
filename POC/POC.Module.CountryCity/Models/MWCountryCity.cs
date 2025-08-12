using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.CountryCity.Views;
using POL.DB.P30Office.GL;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.CountryCity.Models
{
    public class MWCountryCity : NotifyObjectBase, IRequestCloseViewModel
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private Window Owner { get; set; }
        private DBGLCountry Data { get; set; }
        private dynamic MainView { get; set; }
        private GridControl DynamicGrid { get; set; }
        private TableView DynamicTableView { get; set; }
        public bool AllowPhoneCode { get; set; }

        #region CTOR
        public MWCountryCity(Window owner, DBGLCountry data, object mainView)
        {
            Owner = owner;
            Data = data;
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitCommands();
            GetDynamicData();
            PopulateCityList();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "فهرست اطلاعات شهر ها"; }
        }
        #endregion
        #region CountryTitle
        public string CountryTitle
        {
            get { return Data.TitleXX; }
        }
        #endregion


        #region SearchText
        private string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                if (_SearchText == value)
                    return;
                _SearchText = value;
                RaisePropertyChanged("SearchText");
                PopulateCityList();
            }
        }
        #endregion

        #region CityList
        private XPCollection<DBGLCity> _CityList;
        public XPCollection<DBGLCity> CityList
        {
            get { return _CityList; }
            set
            {
                if (_CityList == value)
                    return;
                _CityList = value;
                RaisePropertyChanged("CityList");
            }
        }
        #endregion
        #region SelectedCity
        private DBGLCity _SelectedCity;
        public DBGLCity SelectedCity
        {
            get { return _SelectedCity; }
            set
            {
                if (ReferenceEquals(_SelectedCity, value))
                    return;
                _SelectedCity = value;
                RaisePropertyChanged("SelectedCity");
            }
        }
        #endregion





        #region [METHODS]

        private void InitCommands()
        {
            CommandNew = new RelayCommand(CityNew, () => true);
            CommandEdit = new RelayCommand(CityEdit, () => SelectedCity != null);
            CommandDelete = new RelayCommand(CityDelete, () => SelectedCity != null);
            CommandRefresh = new RelayCommand(CityRefresh, () => true);
            CommandPrint = new RelayCommand(CityPrint, () => true);
            CommandBestFitColumn = new RelayCommand(CityBestFitColumn, () => true);
            CommandClearSearchText = new RelayCommand(ClearSearchText, () => !string.IsNullOrWhiteSpace(SearchText));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp37 != "");
        }

        private void CityNew()
        {
            var w = new WCityAddEdit(Data, null, AllowPhoneCode) { Owner = Owner };
            if (w.ShowDialog() == true)
                CityRefresh();
        }
        private void CityEdit()
        {
            var w = new WCityAddEdit(Data, SelectedCity, AllowPhoneCode) { Owner = Owner };
            if (w.ShowDialog() == true)
                CityRefresh();
        }
        private void CityDelete()
        {
            var srh = DynamicGrid.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", Owner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGrid.GetRow(n) as DBGLCity).ToList();

            var failed = 0;
            var success = 0;
            POLProgressBox.Show("حذف اطلاعات", true, 0, srh.Count(), 1,
                w =>
                {
                    var dxs = ADatabase.GetNewSession();
                    foreach (var db in list)
                    {
                        try
                        {
                            if (w.NeedToCancel) return;
                            var db2 = DBGLCity.FindByOid(dxs, db.Oid);
                            w.AsyncSetText(1, db2.TitleXX);
                            db2.Delete();
                            db2.Save();
                            success++;
                        }
                        catch
                        {
                            failed++;
                        }
                    }
                }, null, Owner);
            POLMessageBox.ShowInformation(string.Format("گزارش حذف : {0}{0}موفقیت آمیز : {1}{0}بروز خطا : {2}", Environment.NewLine, success, failed), Owner);
            CityRefresh();
        }
        private void CityRefresh()
        {
            var i = DynamicTableView.TopRowIndex;
            var srhs = DynamicGrid.GetSelectedRowHandles();

            PopulateCityList();

            DynamicGrid.UnselectAll();
            srhs.ToList().ForEach(r => DynamicGrid.SelectItem(r));
            DynamicTableView.TopRowIndex = i;
        }
        private void CityBestFitColumn()
        {
            if (DynamicTableView == null) return;
            DynamicTableView.BestFitColumns();
        }
        private void ClearSearchText()
        {
            SearchText = string.Empty;
        }
        private void CityPrint()
        {
            var link = new PrintableControlLink(DynamicTableView);

            var preview = new DocumentPreview { Model = new LinkPreviewModel(link) };
            var v = (DataTemplate)Owner.FindResource("toolbarCustomization");
            var barManagerCustomizer = new TemplatedBarManagerController { Template = v };
            preview.BarManager.Controllers.Add(barManagerCustomizer);
            var previewWindow = new DocumentPreviewWindow
            {
                Owner = Owner,
                Content = preview,
                FlowDirection = HelperLocalize.ApplicationFlowDirection,
                FontFamily = new FontFamily(HelperLocalize.ApplicationFontName),
                FontSize = HelperLocalize.ApplicationFontSize,
                Title = "پیش نمایش",
            };
            preview.FlowDirection = FlowDirection.LeftToRight;


            link.ReportHeaderData = this;
            link.ReportHeaderTemplate = (DataTemplate)Owner.FindResource("reportHeaderTemplate");
            link.CreateDocument(true);
            previewWindow.ShowDialog();
        }



        private void PopulateCityList()
        {
            var ff = SearchText;
            if (string.IsNullOrWhiteSpace(ff))
                ff = string.Empty;
            ff = ff.Replace("*", "").Replace("%", "").Trim();
            HelperConvert.CorrectPersianBug(ref ff);
            XPCollection<DBGLCity> xpc = null;
            xpc = AllowPhoneCode ? DBGLCity.GetByCountryWithTeleCode(ADatabase.Dxs, Data, ff) : DBGLCity.GetByCountryWithoutTeleCode(ADatabase.Dxs, Data, ff);
            xpc.LoadAsync();
            CityList = xpc;
        }
        private void GetDynamicData()
        {
            try
            {
                DynamicGrid = MainView.GetDynamicGrid();
                DynamicTableView = DynamicGrid.View as TableView;
                AllowPhoneCode = MainView.AllowPhoneCode;

                DynamicGrid.MouseDoubleClick +=
                    (s, e) =>
                    {
                        var i = DynamicTableView.GetRowHandleByMouseEventArgs(e);
                        if (i < 0) return;
                        if (CommandEdit.CanExecute(null))
                            CommandEdit.Execute(null);
                        e.Handled = true;
                    };
            }
            catch
            {
            }
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp37);
        }
        #endregion



        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandPrint { get; set; }
        public RelayCommand CommandBestFitColumn { get; set; }
        public RelayCommand CommandClearSearchText { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IRequestCloseViewModel
        public event EventHandler<RequestCloseEventArgs> RequestClose;
        private void RaiseRequestClose(bool? dialogResult)
        {
            if (RequestClose != null)
                RequestClose.Invoke(this, new RequestCloseEventArgs { DialogResult = dialogResult });
        }
        #endregion
    }
}
