using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.CountryCity.Views;
using POL.DB.P30Office.GL;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using Microsoft.Win32;
using POL.DB.P30Office;
using POL.WPF.DXControls;

namespace POC.Module.CountryCity.Models
{
    public class MCountryCity : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }



        private dynamic MainView { get; set; }
        private RibbonControl MainRibbonControl { get; set; }
        private GridControl DynamicGrid { get; set; }
        private TableView DynamicTableView { get; set; }
        private Window Owner { get; set; }


        #region CTOR
        public MCountryCity(object mainView)
        {
            MainView = mainView;

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitDynamics();
            PopulateCountryList();
            InitCommands();
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
                PopulateCountryList();
            }
        }
        #endregion

        #region CountryList
        private List<DBGLCountry> _CountryList;
        public List<DBGLCountry> CountryList
        {
            get { return _CountryList; }
            set
            {
                if (_CountryList == value)
                    return;
                _CountryList = value;
                RaisePropertyChanged("CountryList");
            }
        }
        #endregion
        #region SelectedCountry
        private DBGLCountry _SelectedCountry;
        public DBGLCountry SelectedCountry
        {
            get { return _SelectedCountry; }
            set
            {
                if (ReferenceEquals(_SelectedCountry, value))
                    return;
                _SelectedCountry = value;
                RaisePropertyChanged("SelectedCountry");

                if (_SelectedCountry != null)
                {
                    MapImage = HelperImage.ConvertImageByteToBitmapImage(_SelectedCountry.MapImage);
                    FlagImage = HelperImage.ConvertImageByteToBitmapImage(_SelectedCountry.FlagImage);
                }
                else
                {
                    MapImage = null;
                    FlagImage = null;
                }
            }
        }

        #endregion
        #region MapImage
        private BitmapImage _MapImage;
        public BitmapImage MapImage
        {
            get { return _MapImage; }
            set
            {
                if (_MapImage == value) return;
                _MapImage = value;
                RaisePropertyChanged("MapImage");

            }
        }
        #endregion
        #region FlagImage
        private BitmapImage _FlagImage;
        public BitmapImage FlagImage
        {
            get { return _FlagImage; }
            set
            {
                if (_FlagImage == value) return;
                _FlagImage = value;
                RaisePropertyChanged("FlagImage");
            }
        }

        #endregion


        #region [METHODS]
        private void PopulateCountryList()
        {
            var ff = SearchText;
            if (string.IsNullOrWhiteSpace(ff))
                ff = string.Empty;
            ff = ff.Replace("*", "").Replace("%", "").Trim();
            HelperConvert.CorrectPersianBug(ref ff);





            var xpc = from n in ACacheData.GetCountryList() select (DBGLCountry)n.Tag;
            if (!string.IsNullOrWhiteSpace(ff))
            {
                var code = 0;
                int.TryParse(ff, out code);
                xpc = xpc.Where(n =>
                                (n.TeleCode1 == code) ||
                                (n.TeleCode2 == code) ||
                                (n.TitleEn != null && n.TitleEn.Contains(ff)) ||
                                (n.TitleXX != null && n.TitleXX.Contains(ff)) ||
                                (n.CurrencyName != null && n.CurrencyName.Contains(ff)) ||
                                (n.CurrencyCode != null && n.CurrencyCode.Contains(ff)) ||
                                (n.CurrencySymbol != null && n.CurrencySymbol.Contains(ff))
                    );
            }
            CountryList = xpc.ToList();
        }
        private void InitCommands()
        {
            CommandCountryEdit = new RelayCommand(CountryEdit, () => SelectedCountry != null && AMembership.HasPermission(PCOPermissions.CountryCity_CountryEdit));
            CommandCountryDelete = new RelayCommand(CountryDelete, () => SelectedCountry != null && AMembership.HasPermission(PCOPermissions.CountryCity_CountryDelete));
            CommandCountryRefresh = new RelayCommand(() => Refresh(null), () => true);

            CommandCountryCity = new RelayCommand(CountryCity, () => SelectedCountry != null && AMembership.HasPermission(PCOPermissions.CountryCity_ManageCity));
            CommandCountryTele = new RelayCommand(CountryTele, () => SelectedCountry != null && AMembership.HasPermission(PCOPermissions.CountryCity_ManageTeleCode));
            CommandExtraCode = new RelayCommand(ExtraCode, () => AMembership.HasPermission(PCOPermissions.CountryCity_ManageTeleCode));

            CommandBestColumn = new RelayCommand(BestColumn, () => true);

            CommandExportXML = new RelayCommand(ExportXML, () => AMembership.HasPermission(PCOPermissions.CountryCity_ExportXML));

            CommandClearSearchText = new RelayCommand(ClearSearchText, () => !string.IsNullOrWhiteSpace(SearchText));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp35 != "");
        }



        private void InitDynamics()
        {
            try
            {
                Owner = MainView.DynamicOwner;
                MainRibbonControl = MainView.DynamicRibbonControl;
                DynamicGrid = MainView.DynamicDynamicGrid;
                DynamicTableView = DynamicGrid.View as TableView;

                DynamicGrid.MouseDoubleClick +=
                    (s, e) =>
                    {
                        var i = DynamicTableView.GetRowHandleByMouseEventArgs(e);
                        if (i < 0) return;
                        if (CommandCountryEdit.CanExecute(null))
                            CommandCountryEdit.Execute(null);
                        e.Handled = true;
                    };
            }
            catch
            {
            }
        }

        private void ExtraCode()
        {
            var w = new WManageExtraCode()
            {
                Owner = APOCMainWindow.GetWindow()
            };
            w.ShowDialog();
        }
        private void CountryTele()
        {
            var w = new WCountryCity(SelectedCountry, true)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            w.ShowDialog();
        }
        private void CountryCity()
        {
            var w = new WCountryCity(SelectedCountry, false)
                        {
                            Owner = APOCMainWindow.GetWindow()
                        };
            w.ShowDialog();
        }
        private void BestColumn()
        {
            if (DynamicTableView == null) return;
            DynamicTableView.BestFitColumns();
        }
        private void Refresh(DBGLCountry country)
        {

            var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            aCacheData.ForcePopulateCache(EnumCachDataType.Country, false, country);
            aCacheData.RaiseCacheChanged(EnumCachDataType.Country);

            PopulateCountryList();
        }
        private void CountryEdit()
        {
            var w = new WCountryEdit(SelectedCountry)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            if (w.ShowDialog() == true)
                Refresh(SelectedCountry);
        }
        private void CountryDelete()
        {
            if (SelectedCountry == null)
                return;
            var dbcountry = new XPQuery<DBGLCountry>(SelectedCountry.Session).First(n => n.Oid == SelectedCountry.Oid);
            var cityCount = new XPQuery<DBGLCity>(SelectedCountry.Session).Count(n => n.Country != null && n.Country.Oid == dbcountry.Oid);
            if (cityCount > 0)
            {
                POLMessageBox.ShowError("امكان حذف وجود ندارد. لطفا قبل از حذف كشور، شهر های آن را حذف كنید.", MainView);
                return;
            }
            var callCount = new XPQuery<DBCLCall>(SelectedCountry.Session).Count(n => n.Country != null && n.Country.Oid == SelectedCountry.Oid);
            if (callCount > 0)
            {
                POLMessageBox.ShowError(string.Format("امكان حذف وجود ندارد. در بخش تماس ها از این كشور استفاده شده است.{0}تعداد : {1}", Environment.NewLine, callCount), MainView);
                return;
            }
            try
            {
                POLMessageBox.ShowQuestionYesNo("كشور زیر حذف شود؟" + Environment.NewLine + dbcountry.TitleXX, Window.GetWindow(MainView));
                dbcountry.Delete();
                dbcountry.Save();
                Refresh(null);
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message, MainView);
            }
        }
        private void ExportXML()
        {
            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xml",
                Filter = "XML (*.xml)|*.xml",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = "CountryCity.xml"
            };
            if (sf.ShowDialog() != true) return;

            Exception exError = null;
            POLProgressBox.Show(2,
                w =>
                {
                    w.AsyncEnableCancel();
                    var dxsSrc = ADatabase.GetNewSession();
                    var dxsDest = new Session
                    {
                        ConnectionString = String.Format("XpoProvider=XmlDataSet;Data Source={0}", sf.FileName),
                    };

                    try
                    {
                        dxsDest.ClearDatabase();
                        var xpc1 = DBGLCountry.GetAll(dxsSrc);
                        foreach (var v in xpc1)
                        {
                            if (w.NeedToCancel) throw new Exception("عملیات توسط كاربر لغو شد.");

                            w.AsyncSetText(1, v.TitleXX);
                            w.AsyncSetText(2, string.Empty);
                            var dbc = new DBGLCountry(dxsDest)
                                              {
                                                  Code_FIPS104 = v.Code_FIPS104,
                                                  Code_ID = v.Code_ID,
                                                  CurrencyCode = v.CurrencyCode,
                                                  CurrencyName = v.CurrencyName,
                                                  CurrencySymbol = v.CurrencySymbol,
                                                  FlagImage = v.FlagImage,
                                                  Internet = v.Internet,
                                                  ISO2 = v.ISO2,
                                                  ISO3 = v.ISO3,
                                                  ISON = v.ISON,
                                                  MapImage = v.MapImage,
                                                  TeleCode1 = v.TeleCode1,
                                                  TeleCode2 = v.TeleCode2,
                                                  TeleCode = v.TeleCode,
                                                  TimeZone = v.TimeZone,
                                                  TitleEn = v.TitleEn,
                                                  TitleXX = v.TitleXX,
                                              };
                            dbc.Save();

                            var xpcCities = DBGLCity.GetByCountryAll(dxsSrc, v);
                            foreach (var c in xpcCities)
                            {
                                if (w.NeedToCancel) throw new Exception("عملیات توسط كاربر لغو شد.");
                                w.AsyncSetText(2, string.Format("{0} : {1}", c.StateTitle, c.TitleXX));
                                var dbcity = new DBGLCity(dxsDest)
                                                 {
                                                     Country = dbc,
                                                     HasTeleCode = c.HasTeleCode,
                                                     Latitude = c.Latitude,
                                                     Longitude = c.Longitude,
                                                     PhoneCode = c.PhoneCode,
                                                     StateTitle = c.StateTitle,
                                                     TimeZone = c.TimeZone,
                                                     TitleEn = c.TitleEn,
                                                     TitleXX = c.TitleXX,
                                                     PhoneLen = c.PhoneLen,
                                                 };
                                dbcity.Save();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        exError = ex;
                        HelperUtils.Try(dxsDest.Disconnect);
                    }
                },
                w =>
                {
                    if (exError != null)
                    {
                        ALogger.Log(exError.ToString(), Category.Warn, Priority.Medium);
                        POLMessageBox.ShowError(exError.Message, w);
                        return;
                    }
                    POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", w);
                },
                Owner);
        }

        private void ClearSearchText()
        {
            SearchText = string.Empty;
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp35);
        }
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandCountryEdit { get; set; }
        public RelayCommand CommandCountryDelete { get; set; }
        public RelayCommand CommandCountryRefresh { get; set; }
        public RelayCommand CommandCountryCity { get; set; }
        public RelayCommand CommandCountryTele { get; set; }
        public RelayCommand CommandExtraCode { get; set; }
        public RelayCommand CommandBestColumn { get; set; }
        public RelayCommand CommandExportXML { get; set; }
        public RelayCommand CommandClearSearchText { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}

