using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Data.Filtering;
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
    public class MWManageExtraCode : NotifyObjectBase, IRequestCloseViewModel
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private Window Owner { get; set; }
        private dynamic MainView { get; set; }
        private GridControl DynamicGrid { get; set; }
        private TableView DynamicTableView { get; set; }

        #region CTOR
        public MWManageExtraCode(Window owner, object mainView)
        {
            Owner = owner;
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitCommands();
            GetDynamicData();
            PopulateExtraCodeList();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "فهرست اطلاعات روستا/محله/دهستان/منطقه"; }
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
                PopulateExtraCodeList();
            }
        }
        #endregion

        #region ExtraCodeList
        private XPCollection<DBGLExtraCodes> _ExtraCodeList;
        public XPCollection<DBGLExtraCodes> ExtraCodeList
        {
            get { return _ExtraCodeList; }
            set
            {
                if (_ExtraCodeList == value)
                    return;
                _ExtraCodeList = value;
                RaisePropertyChanged("ExtraCodeList");
            }
        }
        #endregion
        #region SelectedExtraCode
        private DBGLExtraCodes _SelectedExtraCode;
        public DBGLExtraCodes SelectedExtraCode
        {
            get { return _SelectedExtraCode; }
            set
            {
                if (ReferenceEquals(_SelectedExtraCode, value))
                    return;
                _SelectedExtraCode = value;
                RaisePropertyChanged("SelectedExtraCode");
            }
        }
        #endregion






        #region [METHODS]
        private void InitCommands()
        {
            CommandNew = new RelayCommand(CityNew, () => true);
            CommandEdit = new RelayCommand(CityEdit, () => SelectedExtraCode != null);
            CommandDelete = new RelayCommand(CityDelete, () => SelectedExtraCode != null);
            CommandRefresh = new RelayCommand(CityRefresh, () => true);
            CommandPrint = new RelayCommand(CityPrint, () => true);
            CommandBestFitColumn = new RelayCommand(CityBestFitColumn, () => true);
            CommandClearSearchText = new RelayCommand(ClearSearchText, () => !string.IsNullOrWhiteSpace(SearchText));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp38 != "");
        }

        private void CityNew()
        {
            var w = new WExtraCodeAddEdit(null) { Owner = Owner };
            if (w.ShowDialog() == true)
                CityRefresh();
        }
        private void CityEdit()
        {
            var w = new WExtraCodeAddEdit(SelectedExtraCode) { Owner = Owner };
            if (w.ShowDialog() == true)
                CityRefresh();
        }
        private void CityDelete()
        {
            var srh = DynamicGrid.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", Owner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGrid.GetRow(n) as DBGLExtraCodes).ToList();

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
                            var db2 = DBGLExtraCodes.FindByOid(dxs, db.Oid);
                            w.AsyncSetText(1, db2.Title);
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

            PopulateExtraCodeList();

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



        private void PopulateExtraCodeList()
        {
            var ff = SearchText;
            if (string.IsNullOrWhiteSpace(ff))
                ff = string.Empty;
            ff = ff.Replace("*", "").Replace("%", "").Trim();
            HelperConvert.CorrectPersianBug(ref ff);
            XPCollection<DBGLExtraCodes> xpc = null;
            xpc = DBGLExtraCodes.GetAll(ADatabase.Dxs, new BinaryOperator("Title", "%" + ff + "%", BinaryOperatorType.Like));
            xpc.LoadAsync();
            ExtraCodeList = xpc;
        }
        private void GetDynamicData()
        {
            try
            {
                DynamicGrid = MainView.GetDynamicGrid();
                DynamicTableView = DynamicGrid.View as TableView;

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
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp38);
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
