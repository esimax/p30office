using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.LookUp;
using DevExpress.Xpf.LayoutControl;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Win32;
using POL.DB.P30Office.GL;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using Microsoft.Practices.ServiceLocation;
using DevExpress.Xpf.Core;
using POL.DB.P30Office;
using System.Collections.Generic;
using POL.WPF.DXControls;
using POL.Lib.Utils;

namespace POC.Module.Profile.Models
{
    public class MListEditor : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private Grid DynamicGrid { get; set; }
        private DBCTList DynamicList { get; set; }
        private object DynamicSelectedData { get; set; }
        private Type DynamicDBListType { get; set; }
        private Assembly DynamicDBListAssembly { get; set; }
        private DBCTContact DynamicDBContact { get; set; }

        private Style LableNormalStyle { get; set; }
        private ControlTemplate CountryPopupTemplate { get; set; }
        private ControlTemplate CityPopupTemplate { get; set; }


        public MListEditor(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();


            InitCommands();
            GetDynamicData();
            DataRefresh();
        }

        #region WindowTitle
        public string WindowTitle
        {
            get
            {
                var title = DynamicList.Title;
                var contact = DynamicDBContact.Title;
                return string.Format("اطلاعات {0} ({1})", title, contact);
            }
        }
        #endregion

        private Dictionary<Guid, byte[]> FileDataHolder = new Dictionary<Guid, byte[]>();


        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => true);
        }
        private void OK()
        {
            if (!DataSave()) return;
            MainView.DynamicSelectedData = DynamicSelectedData;
            MainView.DialogResult = true;
            MainView.Close();
        }

        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicGrid = MainView.DynamicGrid;
            DynamicList = MainView.DynamicList;
            DynamicSelectedData = MainView.DynamicSelectedData;
            DynamicDBListType = MainView.DynamicDBListType;
            DynamicDBListAssembly = MainView.DynamicDBListAssembly;
            DynamicDBContact = MainView.DynamicDBContact;

            LableNormalStyle = MainView.FindResource("LayoutItemLabelStyle");
            CountryPopupTemplate = MainView.FindResource("CountryPopupTemplate");
            CityPopupTemplate = MainView.FindResource("CityPopupTemplate");
        }
        private void DataRefresh()
        {
            DynamicGrid.Children.Clear();
            DynamicGrid.BeginInit();
            try
            {
                var flc = new FlowLayoutControl
                {
                    AllowItemMoving = false,
                    AllowAddFlowBreaksDuringItemMoving = false,
                    AllowLayerSizing = false,
                    AnimateItemMoving = true,
                    Orientation = Orientation.Vertical,
                    BreakFlowToFit = false,
                    StretchContent = true,
                    AllowMaximizedElementMoving = true,
                    MaximizedElementPosition = MaximizedElementPosition.Right,
                    ScrollBars = ScrollBars.Auto,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                DynamicGrid.Children.Add(flc);

                var items =
                    (from r in ACacheData.GetProfileItemList()
                     from g in r.ChildList
                     where ((DBCTProfileGroup)g.Tag).Oid == DynamicList.ProfileGroup.Oid
                     select g.ChildList.Select(m => (DBCTProfileItem)m.Tag).ToList()).FirstOrDefault();

                if (items != null)
                    items.ForEach(
                        item =>
                        {
                            var li = new LayoutItem
                            {
                                Label = item.Title,
                                LabelStyle = LableNormalStyle,
                                Margin = new Thickness(0, 3, 0, 3),
                                Tag = item,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                            };
                            li.Content = GetProfileVisualContent(item, li);
                            flc.Children.Add(li);
                        });
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.High);
            }
            DynamicGrid.EndInit();
        }
        private bool DataSave()
        {
            try
            {
                if (DynamicSelectedData == null)
                {
                    DynamicSelectedData = DynamicDBListAssembly.CreateInstance(DynamicList.TableName, false, BindingFlags.Default, null,
                                                             new object[] { ADatabase.Dxs }, null, null);
                    var pi = DynamicDBListType.GetProperty("Contact", typeof(DBCTContact));
                    pi.SetValue(DynamicSelectedData, DynamicDBContact, null);
                }
                var q = from flc in DynamicGrid.Children.Cast<FrameworkElement>()
                        where flc is FlowLayoutControl
                        from li in ((FlowLayoutControl)flc).Children.Cast<FrameworkElement>()
                        where
                            li is LayoutItem && li.Tag is DBCTProfileItem &&
                            ((LayoutItem)li).Content is FrameworkElement
                        select li;
                q.ToList().ForEach(GatherValue);

                ADatabase.Dxs.Save(DynamicSelectedData);
                return true;
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
                POLMessageBox.ShowWarning("برای حذف این پیغام خطا لطفا از برنامه خارج شده و مجددا اجرا كنید.", DynamicOwner);
            }
            return false;
        }

        private void GatherValue(FrameworkElement obj)
        {
            var li = (LayoutItem)obj;
            var item = (DBCTProfileItem)li.Tag;
            switch (item.ItemType)
            {
                case EnumProfileItemType.Bool:
                    GatherFromBool(li.Content, item);
                    break;
                case EnumProfileItemType.Double:
                    GatherFromDouble(li.Content, item);
                    break;
                case EnumProfileItemType.Country:
                    GatherFromCountry(li.Content, item);
                    break;
                case EnumProfileItemType.City:
                    GatherFromCity(li.Content, item);
                    break;
                case EnumProfileItemType.Location:
                    GatherFromLocation(li.Content, item);
                    break;
                case EnumProfileItemType.String:
                    GatherFromString(li.Content, item);
                    break;
                case EnumProfileItemType.Memo:
                    GatherFromMemo(li.Content, item);
                    break;
                case EnumProfileItemType.StringCombo:
                    GatherFromCombo(li.Content, item);
                    break;
                case EnumProfileItemType.StringCheckList:
                    GatherFromCheckList(li.Content, item);
                    break;
                case EnumProfileItemType.Color:
                    GatherFromColor(li.Content, item);
                    break;
                case EnumProfileItemType.File:
                    GatherFromFile(li.Content, item);
                    break;
                case EnumProfileItemType.Image:
                    GatherFromImage(li.Content, item);
                    break;
                case EnumProfileItemType.Date:
                    GatherFromDate(li.Content, item);
                    break;
                case EnumProfileItemType.Time:
                    GatherFromTime(li.Content, item);
                    break;
                case EnumProfileItemType.DateTime:
                    GatherFromDateTime(li.Content, item);
                    break;
                case EnumProfileItemType.Contact:
                    GatherFromContact(li.Content, item);
                    break;
                case EnumProfileItemType.List:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void GatherFromBool(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is CheckEdit)) return;
            var ui = (CheckEdit)uie;
            var val = ui.IsChecked == true ? 1 : (ui.IsChecked == false ? 0 : 2);
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(int));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromDouble(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is TextEdit)) return;
            var ui = (TextEdit)uie;
            var val = ui.EditValue == null ? 0 : Convert.ToDouble(ui.EditValue);
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(double));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromCountry(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is LookUpEdit)) return;
            var ui = (LookUpEdit)uie;
            var country = ui.EditValue as POL.DB.P30Office.GL.DBGLCountry;
            var val = country == null
                          ? new CountryStruct { CountOid = Guid.Empty, CountTitle = string.Empty }
                          : new CountryStruct { CountOid = country.Oid, CountTitle = country.TitleXX };
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(CountryStruct));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromCity(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is StackPanel)) return;
            var ui = (StackPanel)uie;
            var uicity = (from n in ui.Children.Cast<FrameworkElement>() where n.Name == "city" select n as LookUpEdit).FirstOrDefault();
            if (uicity != null)
            {
                var city = uicity.EditValue as DBGLCity;
                var val = city == null
                    ? new CityStruct() { CityOid = Guid.Empty, CityTitle = string.Empty }
                    : new CityStruct
                    {
                        CityOid = city.Oid,
                        CityTitle =
                            string.Format("{0}, {1} {2}", city.Country.TitleXX,
                                string.IsNullOrWhiteSpace(city.StateTitle) ? string.Empty : city.StateTitle + ",",
                                city.TitleXX)
                    };
                var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(CityStruct));
                pi.SetValue(DynamicSelectedData, val, null);
            }
        }
        private void GatherFromLocation(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is Grid)) return;
            var ui = (Grid)uie;
            var uilat = (from n in ui.Children.Cast<FrameworkElement>() where n.Name == "teLat" select n as TextEdit).FirstOrDefault();
            var uilon = (from n in ui.Children.Cast<FrameworkElement>() where n.Name == "teLong" select n as TextEdit).FirstOrDefault();

            if (uilat == null || uilon == null)
                return;

            var val = new LocationStruct { Lat = 0, Lon = 0, Zoom = 4, Note = string.Empty };
            HelperUtils.Try(() => { val.Lat = Convert.ToDouble(uilat.EditValue); });
            HelperUtils.Try(() => { val.Lon = Convert.ToDouble(uilon.EditValue); });
            HelperUtils.Try(() => { val.Zoom = Convert.ToInt32(ui.Tag); });
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(LocationStruct));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromString(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is TextEdit)) return;
            var ui = (TextEdit)uie;
            var val = ui.EditValue == null ? string.Empty : ui.EditValue.ToString();
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(string));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromMemo(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is TextEdit)) return;
            var ui = (TextEdit)uie;
            var val = ui.EditValue == null ? string.Empty : ui.EditValue.ToString();
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(string));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromCombo(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is TextEdit)) return;
            var ui = (TextEdit)uie;
            var val = ui.EditValue == null ? string.Empty : ui.EditValue.ToString();
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(string));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromCheckList(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is ListBoxEdit)) return;
            var ui = (ListBoxEdit)uie;
            var val = String.Join("|", (from n in ui.SelectedItems.Cast<string>() orderby n select n.Replace("|", "")).ToList());
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(string));
            pi.SetValue(DynamicSelectedData, val, null);
        }

        private void GatherFromColor(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is PopupColorEdit)) return;
            var ui = (PopupColorEdit)uie;
            var c = ((Color)ui.EditValue);
            var val = ((ulong)c.A * 256 * 256 * 256) + ((ulong)c.R * 256 * 256) + ((ulong)c.G * 256) + (ulong)c.B;
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(double));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromFile(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is StackPanel)) return;
            var ui = (StackPanel)uie;
            var uiFileName = (from n in ui.Children.Cast<FrameworkElement>() where n.Name == "beFileName" select n as ButtonEdit).FirstOrDefault();
            var uiNote = (from n in ui.Children.Cast<FrameworkElement>() where n.Name == "teNote" select n as TextEdit).FirstOrDefault();

            if (uiNote == null || uiFileName == null)
                return;

            var val = GetValueFile(item);
            var fn = uiFileName.EditValue == null ? string.Empty : uiFileName.EditValue.ToString();

            var hc = false;
            if (val.FileName != fn)
            {
                val.FileName = fn;
                hc = true;
            }

            val.Note = uiNote.EditValue == null ? string.Empty : uiNote.EditValue.ToString();


            if (FileDataHolder.ContainsKey(item.Oid))
            {
                {
                    var dbb = DBCTBytes.FindByOid(ADatabase.Dxs, val.ByteOid);
                    if (dbb != null)
                    {
                        dbb.Delete();
                        dbb.Save();
                        val.ByteOid = Guid.Empty;
                        val.Len = 0;
                    }
                }
                {
                    var dbb = new DBCTBytes(ADatabase.Dxs)
                                  {
                                      DataByte = FileDataHolder[item.Oid],
                                      Contact = DynamicDBContact,
                                      ByteDataType = EnumByteDataType.ListFile,
                                  };
                    dbb.Save();
                    val.ByteOid = dbb.Oid;
                    val.Len = FileDataHolder[item.Oid].Length;
                }
            }
            else
            {
                if (hc)
                {
                    var dbb = DBCTBytes.FindByOid(ADatabase.Dxs, val.ByteOid);
                    if (dbb != null)
                    {
                        dbb.Delete();
                        dbb.Save();
                    }
                    val.ByteOid = Guid.Empty;
                    val.Len = 0;
                }
            }


            if (!string.IsNullOrEmpty(val.FileName) && FileDataHolder.ContainsKey(item.Oid))
                val.Len = FileDataHolder[item.Oid].Length;
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(FileStruct));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromImage(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is Grid)) return;
            var ui = (Grid)uie;
            var uiImg = (from n in ui.Children.Cast<FrameworkElement>() where n.Name == "img" select n as ImageEdit).FirstOrDefault();
            var uiNote = (from n in ui.Children.Cast<FrameworkElement>() where n.Name == "note" select n as TextEdit).FirstOrDefault();

            if (uiImg == null || uiNote == null)
                return;

            var val = GetValueImage(item);
            val.Note = uiNote.EditValue == null ? string.Empty : uiNote.EditValue.ToString();
            var hc = false;

            var h = 0;
            var w = 0;
            if (uiImg.EditValue != null)
            {
                if (uiImg.EditValue is BitmapSource)
                {
                    var bs = uiImg.EditValue as BitmapSource;
                    w = (int)bs.Width;
                    h = (int)bs.Height;

                    if (val.Width != w) hc = true;
                    if (val.Height != h) hc = true;
                }

                if (uiImg.EditValue is byte[])
                {
                    var bb = uiImg.EditValue as byte[];
                    var bs = HelperImage.ConvertImageByteToBitmapImage(bb);
                    w = (int)bs.Width;
                    h = (int)bs.Height;

                    if (val.Width != w) hc = true;
                    if (val.Height != h) hc = true;
                }
            }
            else
            {
                if (val.ByteOid != Guid.Empty)
                    hc = true;
            }

            if (hc)
            {
                {
                    var dbb = DBCTBytes.FindByOid(ADatabase.Dxs, val.ByteOid);
                    if (dbb != null)
                    {
                        dbb.Delete();
                        dbb.Save();
                    }
                    val.ByteOid = Guid.Empty;
                    val.Height = 0;
                    val.Width = 0;
                }
                if (uiImg.EditValue is BitmapSource)
                {
                    var bs = uiImg.EditValue as BitmapSource;
                    var dbb = new DBCTBytes(ADatabase.Dxs);
                    dbb.DataByte = HelperImage.ConvertBitmapSourceToByteArrayAsPNG(bs);
                    dbb.Contact = DynamicDBContact;
                    dbb.ByteDataType = EnumByteDataType.ListImage;
                    dbb.Save();
                    val.ByteOid = dbb.Oid;
                    val.Height = h;
                    val.Width = w;
                }
                if (uiImg.EditValue is byte[])
                {
                    var bs = HelperImage.ConvertImageByteToBitmapImage(uiImg.EditValue as byte[]);
                    var dbb = new DBCTBytes(ADatabase.Dxs);
                    dbb.DataByte = HelperImage.ConvertBitmapSourceToByteArrayAsPNG(bs);
                    dbb.Contact = DynamicDBContact;
                    dbb.ByteDataType = EnumByteDataType.ListImage;
                    dbb.Save();
                    val.ByteOid = dbb.Oid;
                    val.Height = h;
                    val.Width = w;
                }
            }

            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(ImageStruct));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromDate(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is POL.WPF.DXControls.POLDateEdit.POLDateEdit)) return;
            var ui = (POL.WPF.DXControls.POLDateEdit.POLDateEdit)uie;
            var val = ui.DateEditValue.HasValue ? ui.DateEditValue.Value : DateTime.MinValue;
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(DateTime));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromTime(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is TextEdit)) return;
            var ui = (TextEdit)uie;
            var val = 0;
            if (ui.EditValue != null && !string.IsNullOrEmpty(ui.EditValue.ToString()))
            {
                var newVal = ui.EditValue.ToString();
                var ss = newVal.Split(':');
                val = Convert.ToInt32(ss[0]) * 100 + Convert.ToInt32(ss[1]);
            }
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(int));
            pi.SetValue(DynamicSelectedData, val, null);
        }
        private void GatherFromDateTime(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is Grid)) return;
            var ui = (Grid)uie;
            var uiDate = (from n in ui.Children.Cast<FrameworkElement>() where n.Name == "dateUI" select n as POL.WPF.DXControls.POLDateEdit.POLDateEdit).FirstOrDefault();
            var uiTime = (from n in ui.Children.Cast<FrameworkElement>() where n.Name == "timeUI" select n as TextEdit).FirstOrDefault();

            if (uiTime == null || uiDate == null) return;
            var datetime = DateTime.MinValue;
            if (uiDate.DateEditValue.HasValue && uiDate.DateEditValue.Value != DateTime.MinValue)
            {
                datetime = uiDate.DateEditValue.Value.Date;
            }

            if (uiTime.EditValue != null && !string.IsNullOrEmpty(uiTime.EditValue.ToString()))
            {
                var newVal = uiTime.EditValue.ToString();
                var ss = newVal.Split(':');
                var h = Convert.ToInt32(ss[0]);
                var m = Convert.ToInt32(ss[1]);
                datetime = new DateTime(datetime.Year, datetime.Month, datetime.Day, h, m, 0);
            }

            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(DateTime));
            pi.SetValue(DynamicSelectedData, datetime, null);
        }
        private void GatherFromContact(UIElement uie, DBCTProfileItem item)
        {
            if (!(uie is ButtonEdit)) return;
            var ui = (ButtonEdit)uie;

            var val = GetValueContact(item);
            HelperUtils.Try(() => { val.ContactOid = ((DBCTContact)ui.EditValue).Oid; });
            HelperUtils.Try(() => { val.Title = ((DBCTContact)ui.EditValue).Title; });

            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(ContactStruct));
            pi.SetValue(DynamicSelectedData, val, null);
        }

        private UIElement GetProfileVisualContent(DBCTProfileItem dbItem, LayoutItem li)
        {
            if (dbItem == null) return null;
            switch (dbItem.ItemType)
            {
                case EnumProfileItemType.Bool:
                    return GenerateContentForBool(dbItem, li);
                case EnumProfileItemType.Double:
                    return GenerateContentForDouble(dbItem, li);
                case EnumProfileItemType.Country:
                    return GenerateContentForCountry(dbItem, li);
                case EnumProfileItemType.City:
                    return GenerateContentForCity(dbItem, li);
                case EnumProfileItemType.Location:
                    return GenerateContentForLocation(dbItem, li);
                case EnumProfileItemType.String:
                    return GenerateContentForString(dbItem, li);
                case EnumProfileItemType.Memo:
                    return GenerateContentForMemo(dbItem, li);
                case EnumProfileItemType.StringCombo:
                    return GenerateContentForCombo(dbItem, li);
                case EnumProfileItemType.StringCheckList:
                    return GenerateContentForCheckList(dbItem, li);
                case EnumProfileItemType.Color:
                    return GenerateContentForColor(dbItem, li);
                case EnumProfileItemType.File:
                    return GenerateContentForFile(dbItem, li);
                case EnumProfileItemType.Image:
                    return GenerateContentForImage(dbItem, li);
                case EnumProfileItemType.Date:
                    return GenerateContentForDate(dbItem, li);
                case EnumProfileItemType.Time:
                    return GenerateContentForTime(dbItem, li);
                case EnumProfileItemType.DateTime:
                    return GenerateContentForDateTime(dbItem, li);
                case EnumProfileItemType.Contact:
                    return GenerateContentForContact(dbItem, li);
                case EnumProfileItemType.List:
                    return GenerateContentForList(dbItem, li);
            }
            return null;
        }
        private UIElement GenerateContentForBool(DBCTProfileItem item, LayoutItem li)
        {
            var rv = new CheckEdit
                       {
                           IsThreeState = item.Int1 == 1,
                           IsChecked = GetValueBool(item),
                           Tag = item,
                       };
            return rv;
        }
        private UIElement GenerateContentForDouble(DBCTProfileItem item, LayoutItem li)
        {
            var rv = new TextEdit
            {
                MaskType = MaskType.Numeric,
                Mask = item.String1,
                MaskUseAsDisplayFormat = true,
                FlowDirection = FlowDirection.LeftToRight,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                EditValue = GetValueDouble(item),
                Tag = item,
            };

            rv.LostFocus += (s, e) =>
            {
                var te = (TextEdit)s;
                var db = (DBCTProfileItem)te.Tag;
                var newVal = Convert.ToDouble(te.EditValue);
                var changed = false;
                if (db.Int1 > 0)
                {
                    if (db.Double1 > newVal)
                    {
                        newVal = db.Double1;
                        changed = true;
                    }
                }
                if (db.Int2 > 0)
                {
                    if (db.Double2 < newVal)
                    {
                        newVal = db.Double2;
                        changed = true;
                    }
                }
                if (changed)
                    te.EditValue = newVal;
            };

            return rv;
        }
        private UIElement GenerateContentForCountry(DBCTProfileItem item, LayoutItem li)
        {
            var rv = new LookUpEdit
            {
                IsTextEditable = false,
                StyleSettings = new SearchLookUpEditStyleSettings(),
                DisplayMember = "TitleXX",
                AutoPopulateColumns = false,
                IncrementalFiltering = true,
                ImmediatePopup = true,
                PopupMinWidth = 320,
                PopupContentTemplate = CountryPopupTemplate,
                ItemsSource = POL.DB.P30Office.GL.DBGLCountry.GetAll(ADatabase.Dxs),
                EditValue = POL.DB.P30Office.GL.DBGLCountry.FindByOid(ADatabase.Dxs, GetValueCountry(item).CountOid),
                Tag = item,
            };
            var bi = new ButtonInfo
            {
                GlyphKind = GlyphKind.Cancel,
            };
            rv.Buttons.Add(bi);
            bi.Click += (s, e)
                =>
                {
                    rv.SelectedItem = null;
                };
            return rv;
        }
        private UIElement GenerateContentForCity(DBCTProfileItem item, LayoutItem li)
        {
            var rv = new StackPanel { Tag = item };

            var cityStruct = GetValueCity(item);
            var city = POL.DB.P30Office.GL.DBGLCity.FindByOid(ADatabase.Dxs, cityStruct.CityOid);

            var lueCountry = new LookUpEdit
            {
                IsTextEditable = false,
                StyleSettings = new SearchLookUpEditStyleSettings(),
                DisplayMember = "TitleXX",
                AutoPopulateColumns = false,
                IncrementalFiltering = true,
                ImmediatePopup = true,
                PopupMinWidth = 370,
                PopupContentTemplate = CountryPopupTemplate,
                ItemsSource = POL.DB.P30Office.GL.DBGLCountry.GetAll(ADatabase.Dxs),
                Tag = item,
            };
            if (item.Int1 == 1)
            {
                lueCountry.EditValue = POL.DB.P30Office.GL.DBGLCountry.FindByISO3(ADatabase.Dxs, item.String1);
                lueCountry.IsEnabled = false;
            }
            else
            {
                lueCountry.EditValue = city == null ? null : city.Country;
            }
            rv.Children.Add(lueCountry);


            var lueCity = new LookUpEdit
            {
                IsTextEditable = false,
                StyleSettings = new SearchLookUpEditStyleSettings(),
                DisplayMember = "TitleXX",
                AutoPopulateColumns = false,
                IncrementalFiltering = true,
                ImmediatePopup = true,
                PopupMinWidth = 320,
                PopupContentTemplate = CityPopupTemplate,
                ItemsSource = POL.DB.P30Office.GL.DBGLCity.GetByCountryWithoutTeleCode(ADatabase.Dxs, lueCountry.EditValue as POL.DB.P30Office.GL.DBGLCountry, string.Empty),
                EditValue = city,
                Tag = item,
                Name = "city",
            };
            rv.Children.Add(lueCity);

            lueCountry.EditValueChanged += (s, e)
                =>
                {
                    var te = (LookUpEdit)s;
                    var sp = (StackPanel)te.Parent;
                    var uicity = (from n in sp.Children.Cast<FrameworkElement>() where n.Name == "city" select n as LookUpEdit).FirstOrDefault();
                    if (uicity == null) return;
                    uicity.EditValue = null;
                    uicity.ItemsSource = POL.DB.P30Office.GL.DBGLCity.
                        GetByCountryWithoutTeleCode(ADatabase.Dxs, te.EditValue as POL.DB.P30Office.GL.DBGLCountry,
                                                                                                  string.Empty);
                };


            var bi = new ButtonInfo
            {
                GlyphKind = GlyphKind.Cancel,
            };
            lueCity.Buttons.Add(bi);
            bi.Click += (s, e)
                =>
                {
                    lueCity.EditValue = null;
                };
            return rv;
        }
        private UIElement GenerateContentForLocation(DBCTProfileItem item, LayoutItem li)
        {
            var locStruct = GetValueLocation(item);
            var rv = new Grid { Tag = locStruct.Zoom };

            rv.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rv.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            var teLat = new TextEdit
            {
                MaskType = MaskType.Numeric,
                Mask = "n4",
                MaskUseAsDisplayFormat = true,
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = locStruct.Lat,
                Margin = new Thickness(0, 0, 0, 0),
                Tag = item,
                Name = "teLat",
            };
            teLat.SetValue(Grid.ColumnProperty, 0);
            teLat.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(teLat);


            var teLong = new TextEdit
            {
                MaskType = MaskType.Numeric,
                Mask = "n4",
                MaskUseAsDisplayFormat = true,
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = locStruct.Lon,
                Margin = new Thickness(6, 0, 6, 0),
                Tag = item,
                Name = "teLong",
            };
            teLong.SetValue(Grid.ColumnProperty, 1);
            teLong.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(teLong);


            var but = new Button
            {
                Margin = new Thickness(0, 0, 0, 0),
                Content = "+",
                Tag = item,
            };
            but.SetValue(Grid.ColumnProperty, 2);
            but.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(but);




            but.Click += (s, e)
                =>
            {
                var te = (Button)s;
                var gridUI = (Grid)te.Parent;
                var latUI = (from n in gridUI.Children.Cast<FrameworkElement>() where n.Name == "teLat" select n as TextEdit).First();
                var longUI = (from n in gridUI.Children.Cast<FrameworkElement>() where n.Name == "teLong" select n as TextEdit).First();

                var mapinfo = new MapLocationItem();
                HelperUtils.Try(() => { mapinfo.Lat = Convert.ToDouble(latUI.EditValue); });
                HelperUtils.Try(() => { mapinfo.Lon = Convert.ToDouble(longUI.EditValue); });
                HelperUtils.Try(() => { mapinfo.ZoomLevel = Convert.ToInt32(gridUI.Tag); });

                var mli = APOCMainWindow.ShowSelectPointOnMap(DynamicOwner, mapinfo);

                if (mli == null) return;
                latUI.EditValue = mli.Lat;
                longUI.EditValue = mli.Lon;
                gridUI.Tag = mli.ZoomLevel;







            };
            return rv;
        }
        private UIElement GenerateContentForString(DBCTProfileItem item, LayoutItem li)
        {
            var rv = new TextEdit
            {
                EditValue = GetValueString(item),
                Tag = item,
            };

            if (item.Int3 == 1)
            {
                rv.FlowDirection = FlowDirection.RightToLeft;
                rv.GotFocus += (s, e) => HelperLocalize.SetLanguageToRTL();
                rv.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
            }
            if (item.Int3 == 2)
            {
                rv.FlowDirection = FlowDirection.LeftToRight;
                rv.GotFocus += (s, e) => HelperLocalize.SetLanguageToLTR();
                rv.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
            }
            if (item.Int1 > 0)
                rv.MaxLength = item.Int1;
            return rv;
        }
        private UIElement GenerateContentForMemo(DBCTProfileItem item, LayoutItem li)
        {
            var rv = new TextEdit
            {
                EditValue = GetValueString(item),
                Height = (item.Int2 + 1) * 50,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                TextWrapping = TextWrapping.NoWrap,
                VerticalContentAlignment = VerticalAlignment.Top,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                AcceptsReturn = true,
                AcceptsTab = false,
                Tag = item,
            };

            if (item.Int3 == 1)
            {
                rv.FlowDirection = FlowDirection.RightToLeft;
                rv.GotFocus += (s, e) => HelperLocalize.SetLanguageToRTL();
                rv.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
            }
            if (item.Int3 == 2)
            {
                rv.FlowDirection = FlowDirection.LeftToRight;
                rv.GotFocus += (s, e) => HelperLocalize.SetLanguageToLTR();
                rv.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
            }
            if (item.Int1 > 0)
                rv.MaxLength = item.Int1;
            return rv;
        }
        private UIElement GenerateContentForCombo(DBCTProfileItem item, LayoutItem li)
        {
            var dataList =
               (from n in ACacheData.GetProfileTableList()
                where ((DBCTProfileTable)n.Tag).Oid == item.Guid1
                select n.ChildList).FirstOrDefault();


            var rv = new ComboBoxEdit()
            {
                IsTextEditable = false,
                EditValue = GetValueString(item),
                Tag = item,
                ItemsSource = dataList == null ? null : from n in dataList select n.Title,
                AllowSpinOnMouseWheel = false
            };

            if (dataList != null)
            {
                var vv = new List<string>();
                vv.Clear();
                foreach (var v1 in dataList)
                {
                    vv.Add(v1.Title);
                    if (v1.ChildList.Count > 0)
                    {
                        foreach (var v2 in v1.ChildList)
                        {
                            vv.Add(v1.Title + " / " + v2.Title);
                            if (v2.ChildList.Count > 0)
                            {
                                foreach (var v3 in v2.ChildList)
                                {
                                    vv.Add(v1.Title + " / " + v2.Title + " / " + v3.Title);

                                }
                            }
                        }
                    }

                }
                rv.ItemsSource = vv;
            }



            if (item.Int3 == 1)
            {
                rv.FlowDirection = FlowDirection.RightToLeft;
                rv.GotFocus += (s, e) => HelperLocalize.SetLanguageToRTL();
                rv.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
            }
            if (item.Int3 == 2)
            {
                rv.FlowDirection = FlowDirection.LeftToRight;
                rv.GotFocus += (s, e) => HelperLocalize.SetLanguageToLTR();
                rv.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
            }

            var bi1 = new ButtonInfo
            {
                GlyphKind = GlyphKind.Regular,
            };
            rv.Buttons.Add(bi1);
            bi1.Click += (s, e)
            =>
            {
                var db = DBCTProfileTable.FindByOid(ADatabase.Dxs, item.Guid1);
                var o = APOCMainWindow.ShowManageProfileTValue(DynamicOwner, db);

                dataList =
                (from n in ACacheData.GetProfileTableList()
                 where ((DBCTProfileTable)n.Tag).Oid == item.Guid1
                 select n.ChildList).FirstOrDefault();

                rv.ItemsSource = dataList == null ? null : from n in dataList select n.Title;

                if (dataList != null)
                {
                    var vv = new List<string>();
                    vv.Clear();
                    foreach (var v1 in dataList)
                    {
                        vv.Add(v1.Title);
                        if (v1.ChildList.Count > 0)
                        {
                            foreach (var v2 in v1.ChildList)
                            {
                                vv.Add(v1.Title + " / " + v2.Title);
                                if (v2.ChildList.Count > 0)
                                {
                                    foreach (var v3 in v2.ChildList)
                                    {
                                        vv.Add(v1.Title + " / " + v2.Title + " / " + v3.Title);

                                    }
                                }
                            }
                        }

                    }
                    rv.ItemsSource = vv;
                }



                if (o == null) return;
                var v = o as DBCTProfileTValue;
                if (v != null) rv.SelectedItem = v.Title;
            };

            var bi2 = new ButtonInfo
            {
                GlyphKind = GlyphKind.Cancel,
            };
            rv.Buttons.Add(bi2);
            bi2.Click += (s, e) => rv.EditValue = null;

            return rv;
        }
        private UIElement GenerateContentForCheckList(DBCTProfileItem item, LayoutItem li)
        {
            var dataList =
                (from n in ACacheData.GetProfileTableList()
                 where ((DBCTProfileTable)n.Tag).Oid == item.Guid1
                 select n.ChildList).FirstOrDefault();
            var rv = new ListBoxEdit()
            {
                StyleSettings = new CheckedListBoxEditStyleSettings(),
                SelectionMode = SelectionMode.Multiple,
                ShowCustomItems = false,
                Height = 150,
                Tag = item,
                ItemsSource = dataList == null ? null : from n in dataList select n.Title,
            };
            var val = GetValueString(item);
            var ss = string.IsNullOrEmpty(val) ? new string[] { } : val.Split(new[] { '|' });
            ss.ToList().ForEach(s => rv.SelectedItems.Add(s));


            if (item.Int3 == 1)
            {
                rv.FlowDirection = FlowDirection.RightToLeft;
                rv.GotFocus += (s, e) => HelperLocalize.SetLanguageToRTL();
                rv.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
            }
            if (item.Int3 == 2)
            {
                rv.FlowDirection = FlowDirection.LeftToRight;
                rv.GotFocus += (s, e) => HelperLocalize.SetLanguageToLTR();
                rv.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
            }





            return rv;
        }

        private UIElement GenerateContentForColor(DBCTProfileItem item, LayoutItem li)
        {
            var rv = new PopupColorEdit
            {
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = GetValueColor(item),
                Tag = item,
            };
            return rv;
        }
        private UIElement GenerateContentForFile(DBCTProfileItem item, LayoutItem li)
        {
            var rv = new StackPanel { Tag = item };
            var fileStruct = GetValueFile(item);
            var beFileName = new ButtonEdit
            {
                IsTextEditable = false,
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = fileStruct.FileName,
                AllowDefaultButton = false,
                Tag = item,
                Name = "beFileName",
            };
            rv.Children.Add(beFileName);

            var biOpen = new ButtonInfo
            {
                GlyphKind = GlyphKind.Regular,
                ToolTip = "بارگذاری فایل",
            };
            beFileName.Buttons.Add(biOpen);
            biOpen.Click += (s, e)
                =>
                {
                    var sf = new OpenFileDialog
                    {
                        CheckFileExists = true,
                        CheckPathExists = true,
                        Filter = "All Files (*.*)|*.*",
                        FilterIndex = 0,
                        RestoreDirectory = true,
                    };
                    if (sf.ShowDialog() != true) return;
                    var fi = new FileInfo(sf.FileName);
                    if (item.Int1 != 0)
                    {
                        var limit = (long)item.Int2 * (item.Int1 == 1 ? 1024 : 1024 * 1024);
                        if (fi.Length > limit)
                        {
                            POLMessageBox.ShowError(
                                string.Format("حجم فایل بیش از حد مجاز می باشد.{0}حد مجاز: {1}{0}اندازه فایل : {2}",
                                Environment.NewLine,
                                HelperConvert.ConvertToFileSizeFormat(limit),
                                HelperConvert.ConvertToFileSizeFormat(fi.Length)), DynamicOwner);
                            return;
                        }
                    }

                    try
                    {
                        if (FileDataHolder.ContainsKey(item.Oid))
                            FileDataHolder.Remove(item.Oid);

                        using (var f = fi.OpenRead())
                        {
                            var data = new byte[f.Length];
                            f.Read(data, 0, (int)f.Length);
                            FileDataHolder.Add(item.Oid, data);
                        }
                        beFileName.EditValue = Path.GetFileName(sf.FileName);
                    }
                    catch (Exception ex)
                    {
                        ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                        POLMessageBox.ShowError(ex.Message, DynamicOwner);
                    }
                };

            var biSave = new ButtonInfo
            {
                GlyphKind = GlyphKind.Down,
                ToolTip = "ذخیره فایل" + (fileStruct.Len > 0 ? string.Format("، حجم : {0}", HelperConvert.ConvertToFileSizeFormat(Convert.ToDecimal(fileStruct.Len))) : string.Empty),
            };
            beFileName.Buttons.Add(biSave);

            biSave.Click += (s, e)
                =>
            {
                if (fileStruct.ByteOid == Guid.Empty) return;
                var sf = new SaveFileDialog
                {
                    CheckFileExists = false,
                    CheckPathExists = true,
                    Filter = "All Files (*.*)|*.*",
                    FilterIndex = 0,
                    RestoreDirectory = true,
                    FileName = fileStruct.FileName,
                };
                if (sf.ShowDialog() != true) return;

                var bytes = DBCTBytes.FindByOid(ADatabase.Dxs, fileStruct.ByteOid);
                if (bytes == null)
                {
                    POLMessageBox.ShowError("اطلاعات فایل موجود نمی باشد.", DynamicOwner);
                    return;
                }

                try
                {
                    using (var f = File.Create(sf.FileName))
                    {
                        f.Write(bytes.DataByte, 0, bytes.DataByte.Length);
                    }
                    POLMessageBox.ShowInformation("فایل ذخیره شد.", DynamicOwner);
                }
                catch (Exception ex)
                {
                    ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                    POLMessageBox.ShowError(ex.Message, DynamicOwner);
                }
            };


            var teNote = new TextEdit
            {
                NullText = "توضیحات ...",
                Margin = new Thickness(0, 6, 0, 0),
                EditValue = fileStruct.Note,
                Tag = item,
                MaxLength = 128,
                Name = "teNote",
            };
            rv.Children.Add(teNote);

            var biClear = new ButtonInfo
            {
                GlyphKind = GlyphKind.Cancel,
                ToolTip = "حذف اطلاعات فایل",
            };
            beFileName.Buttons.Add(biClear);
            biClear.Click += (s, e)
                =>
                {
                    beFileName.EditValue = null;
                    teNote.EditValue = null;
                };

            beFileName.MouseDoubleClick +=
                (s, e) =>
                {
                    try
                    {
                        var dbfile = DBCTBytes.FindByOid(ADatabase.Dxs, fileStruct.ByteOid );
                        if (dbfile == null) return;
                        if (dbfile.DataByte == null) return;
                        if (dbfile.DataByte.Length == 0) return;

                        var dbc = (DBCTContact)ServiceLocator.Current.GetInstance<IPOCContactModule>().SelectedContact;
                        var path = string.Format("{0}\\{1}", ConstantGeneral.PathFileTemp, item.UnicCode);
                        var fn = Path.Combine(path, beFileName.Text);
                        Directory.CreateDirectory(path);
                        if (File.Exists(fn))
                            File.Delete(fn);

                        using (var f = File.Create(fn))
                        {
                            f.Write(dbfile.DataByte, 0, dbfile.DataByte.Length);
                        }

                        System.Diagnostics.Process.Start(fn);
                    }
                    catch (Exception ex)
                    {
                        var aLogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
                        aLogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                        POLMessageBox.ShowError(ex.Message);
                    }
                };
            return rv;
        }
        private UIElement GenerateContentForImage(DBCTProfileItem item, LayoutItem li)
        {
            var imageStruct = GetValueImage(item);

            var rv = new Grid { Tag = item };
            rv.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rv.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var dbb = DBCTBytes.FindByOid(ADatabase.Dxs, imageStruct.ByteOid);
            var img = new ImageEdit
            {
                Stretch = Stretch.UniformToFill,
                SnapsToDevicePixels = true,
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = dbb == null ? null : dbb.DataByte,
                Name = "img",
                Tag = item,
                ToolTip = (dbb == null ? "بدون اطلاعات" : string.Format("پهنا : {0}{1}ادتفاع : {2}", imageStruct.Width, Environment.NewLine, imageStruct.Height)),
            };

            img.SetValue(Grid.ColumnProperty, 0);
            img.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(img);


            var teNote = new TextEdit
            {
                NullText = "توضیحات ...",
                EditValue = imageStruct.Note,
                MaxLength = 128,
                Margin = new Thickness(0, 6, 0, 0),
                Tag = item,
                Name = "note",
            };
            teNote.SetValue(Grid.ColumnProperty, 0);
            teNote.SetValue(Grid.RowProperty, 1);
            rv.Children.Add(teNote);

            img.EditValueChanging += (s, e) =>
            {
                if (e.NewValue is BitmapSource)
                {
                    var bs = e.NewValue as BitmapSource;
                    var w = (int)bs.Width;
                    var h = (int)bs.Height;
                    if ((item.Int1 > 0 && w > item.Int1) || (item.Int2 > 0 && h > item.Int2))
                    {
                        POLMessageBox.ShowError(
                            string.Format("اندازه تصویر بیشتر از حد مجاز می باشد.{0}پهنای مجاز : {1}{0}ارتفاع مجاز : {2}",
                            Environment.NewLine, (item.Int1 > 0 ? item.Int1.ToString() : "نامحدود"),
                            (item.Int2 > 0 ? item.Int2.ToString(CultureInfo.InvariantCulture) : "نامحدود")),
                            DynamicOwner);
                        e.IsCancel = true;
                        e.Handled = true;
                    }
                }
            };
            return rv;
        }
        private UIElement GenerateContentForDate(DBCTProfileItem item, LayoutItem li)
        {
            var date = GetValueDate(item);
            var rv = new POL.WPF.DXControls.POLDateEdit.POLDateEdit
            {
                FlowDirection = FlowDirection.LeftToRight,
                DateEditValue = date,
                DateTimeType = GetDateTimeType(item.Int1),
                Tag = item,
            };
            return rv;
        }
        private UIElement GenerateContentForTime(DBCTProfileItem item, LayoutItem li)
        {
            var time = GetValueTime(item);

            var rv = new TextEdit
            {
                FlowDirection = FlowDirection.LeftToRight,
                Mask = @"([01]?[0-9]|2[0-3]):[0-5]\d",
                MaskType = MaskType.RegEx,
                MaskUseAsDisplayFormat = true,
                EditValue = string.Format("{0}:{1}", (int)(time / 100), (time % 100).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')),
                Tag = item,
            };


            return rv;
        }
        private UIElement GenerateContentForDateTime(DBCTProfileItem item, LayoutItem li)
        {
            var date = GetValueDateTime(item);

            var rv = new Grid { Tag = item, FlowDirection = FlowDirection.LeftToRight };
            rv.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var dateUI = new POL.WPF.DXControls.POLDateEdit.POLDateEdit
            {
                FlowDirection = FlowDirection.LeftToRight,
                DateEditValue = date,
                DateTimeType = GetDateTimeType(item.Int1),
                Tag = rv,
                Name = "dateUI",
            };
            dateUI.SetValue(Grid.ColumnProperty, 0);
            dateUI.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(dateUI);


            var timeUI = new TextEdit
            {
                FlowDirection = FlowDirection.LeftToRight,
                Mask = @"([01]?[0-9]|2[0-3]):[0-5]\d",
                MaskType = MaskType.RegEx,
                MaskUseAsDisplayFormat = true,
                EditValue = string.Format("{0}:{1}", date == null ? 0 : ((DateTime)date).Hour, date == null ? "00" : ((DateTime)date).Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')),
                Tag = rv,
                Name = "timeUI",
            };
            timeUI.SetValue(Grid.ColumnProperty, 1);
            timeUI.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(timeUI);
            return rv;
        }
        private UIElement GenerateContentForContact(DBCTProfileItem item, LayoutItem li)
        {
            var contactStruct = GetValueContact(item);

            var rv = new ButtonEdit()
            {
                IsTextEditable = false,
                AllowDefaultButton = false,
                Tag = item,
            };

            HelperUtils.Try(() => rv.EditValue = DBCTContact.FindByOid(ADatabase.Dxs, contactStruct.ContactOid));


            var biOpen = new ButtonInfo
            {
                GlyphKind = GlyphKind.Regular,
                ToolTip = "انتخاب پرونده",
            };
            rv.Buttons.Add(biOpen);

            biOpen.Click += (s, e)
                =>
                {
                    var cc = DBCTContactCat.FindByOid(ADatabase.Dxs, item.Guid1);
                    var o = APOCMainWindow.ShowSelectContact(DynamicOwner, cc);
                    if (o is DBCTContact)
                    {
                        var dbc = o as DBCTContact;
                        rv.EditValue = dbc;
                    }
                };

            var biClear = new ButtonInfo
            {
                GlyphKind = GlyphKind.Cancel,
                ToolTip = "حذف پرونده",
            };
            rv.Buttons.Add(biClear);
            biClear.Click += (s, e)
                =>
                {
                    rv.EditValue = null;
                };
            return rv;
        }
        private UIElement GenerateContentForList(DBCTProfileItem item, LayoutItem li)
        {
            var rv = new Button
            {
                Content = item.String1,
                Tag = item,
            };

            return rv;
        }

        private bool? GetValueBool(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                try
                {
                    var dv = Convert.ToInt32(item.DefaultValue);
                    switch (dv)
                    {
                        case 0:
                            return false;
                        case 1:
                            return true;
                    }
                }
                catch
                { }
                return null;
            }

            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(int));
            var val = (int)pi.GetValue(DynamicSelectedData, null);
            if (val == 0) return false;
            if (val == 1) return true;
            return null;
        }
        private double GetValueDouble(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                var d = 0.0;
                Double.TryParse(item.DefaultValue, out d);
                return d;
            }

            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(double));
            var val = (double)pi.GetValue(DynamicSelectedData, null);
            return val;
        }
        private CountryStruct GetValueCountry(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                var dbc = DBGLCountry.FindByISO3(ADatabase.Dxs, item.DefaultValue);
                if (dbc == null)
                    return new CountryStruct { CountOid = Guid.Empty, CountTitle = string.Empty };
                else
                    return new CountryStruct { CountOid = dbc.Oid, CountTitle = dbc.TitleXX };
            }
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(CountryStruct));
            var val = (CountryStruct)pi.GetValue(DynamicSelectedData, null);
            return val;
        }
        private CityStruct GetValueCity(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                var g = Guid.Empty;
                Guid.TryParse(item.DefaultValue, out g);
                var dbc = DBGLCity.FindByOid(ADatabase.Dxs, g);
                if (dbc == null)
                    return new CityStruct { CityOid = Guid.Empty, CityTitle = string.Empty };
                else
                    return new CityStruct { CityOid = g, CityTitle = dbc.TitleXX };
            }
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(CityStruct));
            var val = (CityStruct)pi.GetValue(DynamicSelectedData, null);
            return val;
        }
        private LocationStruct GetValueLocation(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                try
                {
                    var dv = item.DefaultValue;
                    var ss = dv.Split('|');
                    return new LocationStruct { Lat = Convert.ToDouble(ss[0]), Lon = Convert.ToDouble(ss[1]), Zoom = Convert.ToInt32(ss[2]), Note = string.Empty };
                }
                catch { }
                return new LocationStruct { Lat = 0, Lon = 0, Zoom = 4, Note = string.Empty };
            }
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(LocationStruct));
            var val = (LocationStruct)pi.GetValue(DynamicSelectedData, null);
            return val;
        }
        private string GetValueString(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                return item.DefaultValue;
            }

            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(string));
            var val = (string)pi.GetValue(DynamicSelectedData, null);
            return val;
        }
        private Color GetValueColor(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                try
                {
                    var val2 = Convert.ToInt64(item.DefaultValue);
                    var rv2 = Color.FromArgb((byte)(val2 >> 24), (byte)((val2 << 8) >> 24), (byte)((val2 << 16) >> 24),
                        (byte)((val2 << 24) >> 24));
                    return rv2;
                }
                catch
                {

                }
                return Colors.White;
            }

            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(double));
            var val = Convert.ToInt64(pi.GetValue(DynamicSelectedData, null));
            var rv = Color.FromArgb((byte)(val >> 24), (byte)((val << 8) >> 24), (byte)((val << 16) >> 24),
                                    (byte)((val << 24) >> 24));
            return rv;
        }
        private FileStruct GetValueFile(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null) return new FileStruct { ByteOid = Guid.Empty, FileName = string.Empty, Len = 0, Note = string.Empty };
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(FileStruct));
            var val = (FileStruct)pi.GetValue(DynamicSelectedData, null);
            return val;
        }
        private ImageStruct GetValueImage(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null) return new ImageStruct { ByteOid = Guid.Empty, Width = 0, Height = 0, Note = string.Empty };
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(ImageStruct));
            var val = (ImageStruct)pi.GetValue(DynamicSelectedData, null);
            return val;
        }
        private DateTime? GetValueDate(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                try
                {
                    if (item.Int2 == 0) 
                    {
                        return new DateTime(Convert.ToInt64(item.DefaultValue));
                    }
                    else 
                    {
                        return DateTime.Now.AddDays(item.Int3);
                    }
                }
                catch
                {
                }
                return null;
            }
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(DateTime));
            var val = (DateTime)pi.GetValue(DynamicSelectedData, null);
            if (val == DateTime.MinValue) return null;
            return val;
        }
        private int GetValueTime(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                try
                {
                    if (item.Int3 == 0) 
                    {
                        return Convert.ToInt32(item.DefaultValue);
                    }
                    else 
                    {
                        var int1 = DateTime.Now.Hour;
                        var int2 = DateTime.Now.Minute;
                        return int1 * 60 + int2;
                    }
                }
                catch { }
                return 0;
            }
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(int));
            var val = (int)pi.GetValue(DynamicSelectedData, null);
            return val;
        }
        private DateTime? GetValueDateTime(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                try
                {
                    if (item.Int3 == 0) 
                    {
                        return new DateTime(Convert.ToInt64(item.DefaultValue));
                    }
                    else 
                    {
                        return DateTime.Now;
                    }
                }
                catch { }
                return null;
            }
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(DateTime));
            var val = (DateTime)pi.GetValue(DynamicSelectedData, null);
            if (val == DateTime.MinValue) return null;
            return val;
        }
        private ContactStruct GetValueContact(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null)
            {
                var g = Guid.Empty;
                Guid.TryParse(item.DefaultValue, out g);
                var dbc = DBCTContact.FindByOid(ADatabase.Dxs, g);
                if (dbc == null)
                    return new ContactStruct { ContactOid = Guid.Empty, Title = string.Empty };
                else
                    return new ContactStruct { ContactOid = g, Title = dbc.Title };
            }
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(ContactStruct));
            var val = (ContactStruct)pi.GetValue(DynamicSelectedData, null);
            return val;
        }
        private ListStruct GetValueList(DBCTProfileItem item)
        {
            if (DynamicSelectedData == null) return new ListStruct { ListOid = Guid.Empty, Title = string.Empty };
            var pi = DynamicDBListType.GetProperty(string.Format("F{0}", item.UnicCode), typeof(ListStruct));
            var val = (ListStruct)pi.GetValue(DynamicSelectedData, null);
            return val;
        }


        private EnumCalendarType GetDateTimeType(int p)
        {
            if (p == 1) return EnumCalendarType.Shamsi;
            if (p == 2) return EnumCalendarType.Hijri;
            if (p == 3) return EnumCalendarType.Gregorian;
            return HelperLocalize.ApplicationCalendar;
        }
        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion
    }
}
