using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Ribbon;
using Microsoft.Practices.Prism.Logging;
using POC.Module.Profile.Models;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using Microsoft.Practices.ServiceLocation;
using DevExpress.Xpf.Grid;
using System.Reflection;
using DevExpress.Data;
using POL.WPF.DXControls;
using POL.WPF.Controles.AttachProperties;


namespace POC.Module.Profile.Views
{
    public partial class WListViewer : DXRibbonWindow
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private DataTemplate ctBool { get; set; }

        private DataTemplate ctDouble_n0 { get; set; }
        private DataTemplate ctDouble_d { get; set; }
        private DataTemplate ctDouble_n2 { get; set; }
        private DataTemplate ctDouble_n6 { get; set; }
        private DataTemplate ctDouble_P0 { get; set; }
        private DataTemplate ctDouble_P2 { get; set; }

        private DataTemplate ctCountry { get; set; }
        private DataTemplate ctCity { get; set; }
        private DataTemplate ctLocation { get; set; }

        private DataTemplate ctString { get; set; }
        private DataTemplate ctStringRTL { get; set; }
        private DataTemplate ctStringLTR { get; set; }

        private DataTemplate ctMemo { get; set; }
        private DataTemplate ctMemoRTL { get; set; }
        private DataTemplate ctMemoLTR { get; set; }

        private DataTemplate ctCombo { get; set; }
        private DataTemplate ctComboRTL { get; set; }
        private DataTemplate ctComboLTR { get; set; }

        private DataTemplate ctCheckList { get; set; }
        private DataTemplate ctCheckListRTL { get; set; }
        private DataTemplate ctCheckListLTR { get; set; }

        private DataTemplate ctColor { get; set; }
        private DataTemplate ctFile { get; set; }
        private DataTemplate ctImage { get; set; }

        private DataTemplate ctDateDefault { get; set; }
        private DataTemplate ctDateShamsi { get; set; }
        private DataTemplate ctDateHijri { get; set; }
        private DataTemplate ctDateGregorian { get; set; }

        private DataTemplate ctTime { get; set; }

        private DataTemplate ctDateTimeDefault { get; set; }
        private DataTemplate ctDateTimeShamsi { get; set; }
        private DataTemplate ctDateTimeHijri { get; set; }
        private DataTemplate ctDateTimeGregorian { get; set; }

        private DataTemplate ctContact { get; set; }
        private DataTemplate ctList { get; set; }

        public WListViewer(DBCTProfileValue profileValue, bool canEdit)
        {
            DynamicDBProfileValue = profileValue;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
                ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

                if (DynamicDBProfileValue == null) return;
                if (DynamicDBProfileValue.ProfileItem == null) return;
                if (DynamicDBProfileValue.ProfileItem.ItemType != EnumProfileItemType.List) return;

                DynamicDBContact = DynamicDBProfileValue.Contact;
                DynamicDBList = DBCTList.FindByOid(ADatabase.Dxs, DynamicDBProfileValue.ProfileItem.Guid1);
                if (DynamicDBList == null) return;

                SetValue(APWindow.StateSizePositionProperty, DynamicDBList.TableName);
            }

            InitializeComponent();

            try
            {
                DynamicDBListAssembly = DynamicDBList.GetAssemblyOfListObject();

                Loaded += (s, e)
                          =>
                              {
                                  ctBool = (DataTemplate)FindResource("ctBool");
                                  ctDouble_n0 = (DataTemplate)FindResource("ctDouble_n0");
                                  ctDouble_d = (DataTemplate)FindResource("ctDouble_d");
                                  ctDouble_n2 = (DataTemplate)FindResource("ctDouble_n2");
                                  ctDouble_n6 = (DataTemplate)FindResource("ctDouble_n6");
                                  ctDouble_P0 = (DataTemplate)FindResource("ctDouble_P0");
                                  ctDouble_P2 = (DataTemplate)FindResource("ctDouble_P2");

                                  ctCountry = (DataTemplate)FindResource("ctCountry");
                                  ctCity = (DataTemplate)FindResource("ctCity");
                                  ctLocation = (DataTemplate)FindResource("ctLocation");

                                  ctString = (DataTemplate)FindResource("ctString");
                                  ctStringLTR = (DataTemplate)FindResource("ctStringLTR");
                                  ctStringRTL = (DataTemplate)FindResource("ctStringRTL");

                                  ctMemo = (DataTemplate)FindResource("ctMemo");
                                  ctMemoLTR = (DataTemplate)FindResource("ctMemoLTR");
                                  ctMemoRTL = (DataTemplate)FindResource("ctMemoRTL");

                                  ctCombo = (DataTemplate)FindResource("ctCombo");
                                  ctComboLTR = (DataTemplate)FindResource("ctComboLTR");
                                  ctComboRTL = (DataTemplate)FindResource("ctComboRTL");

                                  ctCheckList = (DataTemplate)FindResource("ctCheckList");
                                  ctCheckListLTR = (DataTemplate)FindResource("ctCheckListLTR");
                                  ctCheckListRTL = (DataTemplate)FindResource("ctCheckListRTL");

                                  ctColor = (DataTemplate)FindResource("ctColor");
                                  ctFile = (DataTemplate)FindResource("ctFile");
                                  ctImage = (DataTemplate)FindResource("ctImage");

                                  ctDateDefault = (DataTemplate)FindResource("ctDateDefault");
                                  ctDateShamsi = (DataTemplate)FindResource("ctDateShamsi");
                                  ctDateHijri = (DataTemplate)FindResource("ctDateHijri");
                                  ctDateGregorian = (DataTemplate)FindResource("ctDateGregorian");

                                  ctTime = (DataTemplate)FindResource("ctTime");

                                  ctDateTimeDefault = (DataTemplate)FindResource("ctDateTimeDefault");
                                  ctDateTimeShamsi = (DataTemplate)FindResource("ctDateTimeShamsi");
                                  ctDateTimeHijri = (DataTemplate)FindResource("ctDateTimeHijri");
                                  ctDateTimeGregorian = (DataTemplate)FindResource("ctDateTimeGregorian");

                                  ctContact = (DataTemplate)FindResource("ctContact");
                                  ctList = (DataTemplate)FindResource("ctList");


                                  GenerateGridColumns();
                                  var model = new MListViewer(this);
                                  model.CanEdit = canEdit;
                                  DataContext = model;
                              };
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message);
            }
        }





        private void GenerateGridColumns()
        {
            var items = (from r in ACacheData.GetProfileItemList()
                         from g in r.ChildList
                         where ((DBCTProfileGroup)g.Tag).Oid == DynamicDBList.ProfileGroup.Oid
                         select g.ChildList.Select(m => (DBCTProfileItem)m.Tag).ToList()).FirstOrDefault();
            if (items == null || !items.Any())
                return;

            var HasSort = false;
            items.ForEach(
                item =>
                {
                    switch (item.ItemType)
                    {
                        case EnumProfileItemType.Bool:
                            GenerateColumnForBool(item);
                            break;
                        case EnumProfileItemType.Double:
                            GenerateColumnForDouble(item);
                            break;
                        case EnumProfileItemType.Country:
                            GenerateColumnForCountry(item);
                            break;
                        case EnumProfileItemType.City:
                            GenerateColumnForCity(item);
                            break;
                        case EnumProfileItemType.Location:
                            GenerateColumnForLocation(item);
                            break;
                        case EnumProfileItemType.String:
                            GenerateColumnForString(item);
                            break;
                        case EnumProfileItemType.Memo:
                            GenerateColumnForMemo(item);
                            break;
                        case EnumProfileItemType.StringCombo:
                            GenerateColumnForCombo(item);
                            break;
                        case EnumProfileItemType.StringCheckList:
                            GenerateColumnForCheckList(item);
                            break;
                        case EnumProfileItemType.Color:
                            GenerateColumnForColor(item);
                            break;
                        case EnumProfileItemType.File:
                            GenerateColumnForFile(item);
                            break;
                        case EnumProfileItemType.Image:
                            GenerateColumnForImage(item);
                            break;
                        case EnumProfileItemType.Date:
                            GenerateColumnForDate(item, ref HasSort);
                            break;
                        case EnumProfileItemType.Time:
                            GenerateColumnForTime(item);
                            break;
                        case EnumProfileItemType.DateTime:
                            GenerateColumnForDateTime(item, ref HasSort);
                            break;
                        case EnumProfileItemType.Contact:
                            GenerateColumnForContact(item);
                            break;
                        case EnumProfileItemType.List:
                            GenerateColumnForList(item);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
        }


        private void GenerateColumnForBool(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
                CellTemplate = ctBool,
            };
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForDouble(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
            };
            if (item.String1 == "n0")
            {
                gc.CellTemplate = ctDouble_n0;
            }
            if (item.String1 == "d")
            {
                gc.CellTemplate = ctDouble_d;
            }
            if (item.String1 == "n2")
            {
                gc.CellTemplate = ctDouble_n2;
            }
            if (item.String1 == "n6")
            {
                gc.CellTemplate = ctDouble_n6;
            }
            if (item.String1 == "P0")
            {
                gc.CellTemplate = ctDouble_P0;
            }
            if (item.String1 == "P2")
            {
                gc.CellTemplate = ctDouble_P2;
            }

            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForCountry(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
                CellTemplate = ctCountry,
            };
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForCity(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
                CellTemplate = ctCity,
            };
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForLocation(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
                CellTemplate = ctLocation,
            };
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForString(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
            };
            if (item.Int3 == 0)
            {
                gc.CellTemplate = ctString;
            }
            if (item.Int3 == 1)
            {
                gc.CellTemplate = ctStringRTL;
            }
            if (item.Int3 == 2)
            {
                gc.CellTemplate = ctStringLTR;
            }
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForMemo(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
            };
            if (item.Int3 == 0)
            {
                gc.CellTemplate = ctMemo;
            }
            if (item.Int3 == 1)
            {
                gc.CellTemplate = ctMemoRTL;
            }
            if (item.Int3 == 2)
            {
                gc.CellTemplate = ctMemoLTR;
            }
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForCombo(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
            };
            if (item.Int3 == 0)
            {
                gc.CellTemplate = ctCombo;
            }
            if (item.Int3 == 1)
            {
                gc.CellTemplate = ctComboRTL;
            }
            if (item.Int3 == 2)
            {
                gc.CellTemplate = ctComboLTR;
            }
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForCheckList(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
            };
            if (item.Int3 == 0)
            {
                gc.CellTemplate = ctCheckList;
            }
            if (item.Int3 == 1)
            {
                gc.CellTemplate = ctCheckListRTL;
            }
            if (item.Int3 == 2)
            {
                gc.CellTemplate = ctCheckListLTR;
            }
            gcList.Columns.Add(gc);
        }


        private void GenerateColumnForColor(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
                CellTemplate = ctColor,
            };
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForFile(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
                CellTemplate = ctFile,
            };
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForImage(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
                CellTemplate = ctImage,
            };
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForDate(DBCTProfileItem item, ref bool haseSort)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
            };
            switch (item.Int1)
            {
                case 1:
                    gc.CellTemplate = ctDateShamsi;
                    break;
                case 2:
                    gc.CellTemplate = ctDateHijri;
                    break;
                case 3:
                    gc.CellTemplate = ctDateGregorian;
                    break;
                default:
                    gc.CellTemplate = ctDateDefault;
                    break;
            }
            gcList.Columns.Add(gc);
            if (!haseSort)
            {
                gc.SortOrder = ColumnSortOrder.Descending;
                gc.SortIndex = 1;
                haseSort = true;
            }
        }
        private void GenerateColumnForTime(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
                CellTemplate = ctTime,
            };
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForDateTime(DBCTProfileItem item, ref bool haseSort)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
            };
            switch (item.Int1)
            {
                case 1:
                    gc.CellTemplate = ctDateTimeShamsi;
                    break;
                case 2:
                    gc.CellTemplate = ctDateTimeHijri;
                    break;
                case 3:
                    gc.CellTemplate = ctDateTimeGregorian;
                    break;
                default:
                    gc.CellTemplate = ctDateTimeDefault;
                    break;
            }
            gcList.Columns.Add(gc);
            if (!haseSort)
            {
                gc.SortOrder = ColumnSortOrder.Descending;
                gc.SortIndex = 1;
                haseSort = true;
            }
        }
        private void GenerateColumnForContact(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
                CellTemplate = ctContact,
            };
            gcList.Columns.Add(gc);
        }
        private void GenerateColumnForList(DBCTProfileItem item)
        {
            var gc = new GridColumn
            {
                Name = string.Format("col{0}", item.UnicCode),
                FieldName = string.Format("F{0}", item.UnicCode),
                Header = item.Title,
                CellTemplate = ctList,
            };
            gcList.Columns.Add(gc);
        }

        public DBCTList DynamicDBList { get; set; }
        public DBCTProfileValue DynamicDBProfileValue { get; set; }
        public Window DynamicOwner
        {
            get { return this; }
        }
        public GridControl DynamicGridControl
        {
            get { return gcList; }
        }
        public TableView DynamicTableView
        {
            get { return tvList; }
        }
        public Assembly DynamicDBListAssembly { get; set; }
        public DBCTContact DynamicDBContact { get; set; }

    }
}
