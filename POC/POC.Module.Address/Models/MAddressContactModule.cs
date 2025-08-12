using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using POC.Module.Address.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Utils;
using POL.DB.P30Office;
using System.Windows.Media;

namespace POC.Module.Address.Models
{

    public class MAddressContactModule : NotifyObjectBase, IDisposable, IRefrashable
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private UserControl DynamicView { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private DBCTContact CurrentContact { get; set; }

        #region CTOR
        public MAddressContactModule(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

            AContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();

            AContactModule.OnSelectedContactChanged += AContactModule_OnSelectedContactChanged;

            InitCommands();
            GetDynamicData();

            IsViewFull = HelperSettingsClient.AddressViewIsFull;
            IsViewSimple = !IsViewFull;
            MainView.DynamicReorderColumns();
            DataRefresh();
        }
        #endregion

        void AContactModule_OnSelectedContactChanged(object sender, EventArgs e)
        {

            if (POCContactModuleItem.LasteSelectedVmType == null || POCContactModuleItem.LasteSelectedVmType != GetType())
            {
                RequiresRefresh = true;
                return;
            }
            if (ReferenceEquals(CurrentContact, (AContactModule.SelectedContact as DBCTContact)))
                return;

            DoRefresh();
        }

        #region WindowTitle
        public string WindowTitle
        {
            get { return "آدرس های " + ((DBCTContact)AContactModule.SelectedContact).Title; }
        }
        #endregion

        #region RootEnable
        public bool RootEnable { get { return AContactModule.SelectedContact != null; } }
        #endregion

        #region AddressList
        private XPCollection<DBCTAddress> _AddressList;
        public XPCollection<DBCTAddress> AddressList
        {
            get { return _AddressList; }
            set
            {
                _AddressList = value;
                RaisePropertyChanged("AddressList");
            }
        }
        #endregion
        #region FocusedAddress
        private DBCTAddress _FocusedAddress;
        public DBCTAddress FocusedAddress
        {
            get
            {
                return _FocusedAddress;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedAddress)) return;
                _FocusedAddress = value;
                RaisePropertyChanged("FocusedAddress");
                RaisePropertyChanged("SelectedMapImage");
            }
        }
        #endregion FocusedAddress
        #region SelectedMapImage
        public ImageSource SelectedMapImage
        {
            get
            {
                if (FocusedAddress == null) return null;
                if (FocusedAddress.Latitude < 0) return null;
                try
                {
                    var stream = new MemoryStream(FocusedAddress.MapImageByte);
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.EndInit();
                    return image;
                }
                catch
                {

                }
                return null;
            }
        }
        #endregion


        public bool IsViewFull { get; set; }
        public bool IsViewSimple { get; set; }

        private Guid CopyCutDataOid { get; set; }
        private bool IsCut { get; set; }




        #region [METHODS]
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicGridControl = MainView.DynamicGridControl;
            DynamicTableView = MainView.DynamicTableView;
            DynamicView = MainView as UserControl;
            DynamicGridControl.MouseDoubleClick +=
                (s, e) =>
                {
                    var i = DynamicTableView.GetRowHandleByMouseEventArgs(e);
                    if (i < 0) return;
                    if (CommandEdit.CanExecute(null))
                        CommandEdit.Execute(null);
                    e.Handled = true;
                };
        }
        private void PopulateAddressList()
        {
            CurrentContact = AContactModule.SelectedContact as DBCTContact;
            if (CurrentContact == null)
            {
                AddressList = null;
                return;
            }
            var xpc = DBCTAddress.GetByContact(ADatabase.Dxs, CurrentContact.Oid);
            AddressList = xpc;
        }
        private void InitCommands()
        {
            CommandNew = new RelayCommand(AddressNew, () => AContactModule.SelectedContact != null && AMembership.HasPermission(PCOPermissions.Contact_Address_Add));
            CommandEdit = new RelayCommand(AddressEdit, () => FocusedAddress != null && DynamicGridControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.Contact_Address_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => DynamicGridControl.SelectedItems.Count >= 1 && AMembership.HasPermission(PCOPermissions.Contact_Address_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);

            CommandCopyText = new RelayCommand(CopyText, () => FocusedAddress != null && AContactModule.SelectedContact != null && DynamicGridControl.VisibleRowCount > 0);
            CommandCopy = new RelayCommand(() =>
            {
                CopyCutDataOid = FocusedAddress.Oid;
                IsCut = false;
            }, () => FocusedAddress != null && AContactModule.SelectedContact != null && DynamicGridControl.VisibleRowCount > 0 && AMembership.HasPermission(PCOPermissions.Contact_Address_Copy));
            CommandCut = new RelayCommand(() =>
            {
                CopyCutDataOid = FocusedAddress.Oid;
                IsCut = true;
            }, () => FocusedAddress != null && AContactModule.SelectedContact != null && DynamicGridControl.VisibleRowCount > 0 && AMembership.HasPermission(PCOPermissions.Contact_Address_Cut));
            CommandPaste = new RelayCommand(PasteAddress, () => CopyCutDataOid != Guid.Empty && AContactModule.SelectedContact != null);

            CommandViewFull = new RelayCommand(ViewFull);
            CommandViewSimple = new RelayCommand(ViewSimple);

            CommandViewMap = new RelayCommand(ViewMap, () => FocusedAddress != null && FocusedAddress.Latitude >= 0);
            CommandAssignMap = new RelayCommand(AssignMap, () => FocusedAddress != null && FocusedAddress.Latitude < 0 && AMembership.HasPermission(PCOPermissions.Contact_Address_MapAdd));
            CommandClearMap = new RelayCommand(ClearMap, () => FocusedAddress != null && FocusedAddress.Latitude >= 0 && AMembership.HasPermission(PCOPermissions.Contact_Address_MapDelete));

            CommandPrint = new RelayCommand(AddressPrint, () => !DynamicGridControl.Columns["This"].Visible && AMembership.HasPermission(PCOPermissions.Contact_Address_Print));

            CommandSendSMS = new RelayCommand(SendSMS, () => true);

            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp06 != "");
        }

        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp06);
        }

        private void SendSMS()
        {
            if (FocusedAddress == null) return;
            APOCMainWindow.ShowSendSMS(DynamicOwner, EnumSelectionType.SelectedContact, null, FocusedAddress.Contact
                , null, null, null, new List<string>(), FocusedAddress.FullAddress);
        }

        private void CopyText()
        {
            Clipboard.SetText(FocusedAddress.FullAddress);
        }

        private void AddressNew()
        {
            var w = new WAddressAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void AddressEdit()
        {
            var w = new WAddressAddEdit(FocusedAddress) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("آدرس(های) انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBCTAddress).ToList();

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
                            var db2 = DBCTAddress.FindByOid(dxs, db.Oid);
                            w.AsyncSetText(1, db2.Address);
                            db2.Delete();
                            db2.Save();
                            success++;
                        }
                        catch
                        {
                            failed++;
                        }
                    }
                }, null, DynamicOwner);
            POLMessageBox.ShowInformation(string.Format("گزارش حذف : {0}{0}موفقیت آمیز : {1}{0}بروز خطا : {2}", Environment.NewLine, success, failed), DynamicOwner);
            DataRefresh();
        }
        private void DataRefresh()
        {
            PopulateAddressList();
            RaisePropertyChanged("SelectedMapImage");
            if (IsViewFull)
                Task.Factory.StartNew(
                    () =>
                    {
                        System.Threading.Thread.Sleep(500);
                        HelperUtils.DoDispatcher(() => MainView.DynamicBestFitColumn());
                    });
        }

        private void ViewFull()
        {
            IsViewFull = true;
            IsViewSimple = !IsViewFull;

            RaisePropertyChanged("IsViewFull");
            RaisePropertyChanged("IsViewSimple");

            POL.Lib.XOffice.HelperSettingsClient.AddressViewIsFull = IsViewFull;

            MainView.DynamicReorderColumns();
        }
        private void ViewSimple()
        {
            IsViewFull = false;
            IsViewSimple = !IsViewFull;

            RaisePropertyChanged("IsViewFull");
            RaisePropertyChanged("IsViewSimple");

            POL.Lib.XOffice.HelperSettingsClient.AddressViewIsFull = IsViewFull;
        }

        private void ViewMap()
        {
            var v = APOCMainWindow.ShowSelectPointOnMap(DynamicOwner, new MapLocationItem
                                                          {
                                                              Lat = FocusedAddress.Latitude,
                                                              Lon = FocusedAddress.Longitude,
                                                              ZoomLevel = FocusedAddress.ZoomLevel,
                                                          });
        }
        private void AssignMap()
        {
            var v = APOCMainWindow.ShowSelectPointOnMap(DynamicOwner, null);
            if (v == null) return;
            try
            {
                FocusedAddress.MapImageByte = v.ImageArray;
                FocusedAddress.Latitude = v.Lat;
                FocusedAddress.Longitude = v.Lon;
                FocusedAddress.ZoomLevel = v.ZoomLevel;
                FocusedAddress.Save();
                RaisePropertyChanged("SelectedMapImage");
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }
        private void ClearMap()
        {
            if (POLMessageBox.ShowQuestionYesNo("اطلاعات نقشه حذف شود؟", DynamicOwner) != MessageBoxResult.Yes) return;
            try
            {
                FocusedAddress.MapImageByte = null;
                FocusedAddress.Latitude = -1;
                FocusedAddress.Longitude = -1;
                FocusedAddress.ZoomLevel = -1;
                FocusedAddress.Save();
                RaisePropertyChanged("SelectedMapImage");
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }

        private void AddressPrint()
        {
            var report = new Reports.XtraReport1();
            report.Name = "ReportAddress";

            report.DataSource = DynamicGridControl.ItemsSource;
            var tool = new DevExpress.XtraReports.UI.ReportDesignTool(report);
            tool.ShowDesignerDialog();




        }
        private void PasteAddress()
        {
            if (!IsCut) 
            {
                #region Copy
                var dba1 = DBCTAddress.FindByOid(ADatabase.Dxs, CopyCutDataOid);
                if (dba1 == null)
                {
                    CopyCutDataOid = Guid.Empty;
                    return;
                }
                try
                {
                    var dba2 = new DBCTAddress(ADatabase.Dxs)
                    {
                        Address = dba1.Address,
                        Area = dba1.Area,
                        City = dba1.City,
                        Contact = (DBCTContact)AContactModule.SelectedContact,
                        Latitude = dba1.Latitude,
                        Longitude = dba1.Longitude,
                        MapImageByte = dba1.MapImageByte,
                        Note = dba1.Note,
                        POBox = dba1.POBox,
                        Title = dba1.Title,
                        ZipCode = dba1.ZipCode,
                        ZoomLevel = dba1.ZoomLevel,
                    };
                    dba2.Save();
                }
                catch (Exception ex)
                {
                    POLMessageBox.ShowError(ex.Message);
                    ALogger.Log("Exception : " + ex.ToString(), Category.Exception, Priority.Medium);
                }
                #endregion
            }
            else 
            {
                var dba1 = DBCTAddress.FindByOid(ADatabase.Dxs, CopyCutDataOid);
                if (dba1 == null)
                {
                    CopyCutDataOid = Guid.Empty;
                    return;
                }
                if (!ReferenceEquals(dba1.Contact, AContactModule.SelectedContact))
                {
                    try
                    {
                        dba1.Contact = (DBCTContact)AContactModule.SelectedContact;
                        dba1.Save();
                        CopyCutDataOid = Guid.Empty;
                    }
                    catch (Exception ex)
                    {
                        POLMessageBox.ShowError(ex.Message);
                        ALogger.Log("Exception : " + ex.ToString(), Category.Exception, Priority.Medium);
                    }
                }
            }
            DataRefresh();
        } 
        #endregion



        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandNewMulti { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }

        public RelayCommand CommandCopy { get; set; }
        public RelayCommand CommandCopyText { get; set; }
        public RelayCommand CommandCut { get; set; }
        public RelayCommand CommandPaste { get; set; }


        public RelayCommand CommandSendSMS { get; set; }
        

        public RelayCommand CommandViewSimple { get; set; }
        public RelayCommand CommandViewFull { get; set; }

        public RelayCommand CommandViewMap { get; set; }
        public RelayCommand CommandAssignMap { get; set; }
        public RelayCommand CommandClearMap { get; set; }

        public RelayCommand CommandPrint { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            AContactModule.OnSelectedContactChanged -= AContactModule_OnSelectedContactChanged;
            AddressList = null;
        }
        #endregion

        #region IRefrashable
        public void DoRefresh()
        {
            PopulateAddressList();
            RaisePropertyChanged("RootEnable");
            FocusedAddress = null;
        }

        public bool RequiresRefresh { get; set; }
        #endregion
    }
}
