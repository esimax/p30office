using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Attachment.Reports;
using POC.Module.Attachment.Views;
using POL.DB.P30Office;
using POL.DB.P30Office.AC;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;

namespace POC.Module.Attachment.Models
{
    public class MFactorAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBACFactor DynamicSelectedData { get; set; }
        private DBCTContact DynamicSelectedContact { get; set; }

        private TableView DynamicTableView { get; set; }

        #region CTOR
        public MFactorAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitCommands();
            GetDynamicData();
            PopulateData();

            using (var dxs = ADatabase.GetNewSession())
            {
                FactorTitleDataSource = new XPQuery<DBBTFactorTitle2>(dxs).Select(n => n.Title).ToList();
            }
        }


        #endregion

        public bool IsEditMode { get; set; }

        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات فاكتور"; }
        }
        #endregion

        #region Code
        private int _Code;
        public int Code
        {
            get { return _Code; }
            set
            {
                if (_Code == value) return;
                _Code = value;
                RaisePropertyChanged("Code");
            }
        }
        #endregion

        #region CodeShow
        private bool _CodeShow;
        public bool CodeShow
        {
            get { return _CodeShow; }
            set
            {
                if (_CodeShow == value) return;
                _CodeShow = value;
                RaisePropertyChanged("CodeShow");
            }
        }
        #endregion

        #region IsItemMode
        private bool _IsItemMode;
        public bool IsItemMode
        {
            get { return _IsItemMode; }
            set
            {
                _IsItemMode = value;
                RaisePropertyChanged("IsItemMode");
            }
        }
        #endregion

        #region IsFactorMode
        private bool _IsFactorMode;
        public bool IsFactorMode
        {
            get { return _IsFactorMode; }
            set
            {
                _IsFactorMode = value;
                RaisePropertyChanged("IsFactorMode");
            }
        }
        #endregion

        #region Date
        private DateTime _Date;
        public DateTime Date
        {
            get { return _Date; }
            set
            {
                if (_Date == value) return;
                _Date = value;
                RaisePropertyChanged("Date");
            }
        }
        #endregion

        #region IsPreFactor
        private bool _IsPreFactor;
        public bool IsPreFactor
        {
            get { return _IsPreFactor; }
            set
            {
                if (_IsPreFactor == value) return;
                _IsPreFactor = value;
                RaisePropertyChanged("IsPreFactor");
            }
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

        #region TitleOfTitle
        private string _TitleOfTitle;
        public string TitleOfTitle
        {
            get { return _TitleOfTitle; }
            set
            {
                if (ReferenceEquals(_TitleOfTitle, value)) return;
                _TitleOfTitle = value;
                RaisePropertyChanged("TitleOfTitle");
            }
        }
        #endregion

        #region SelectedContact
        private DBCTContact _SelectedContact;
        public DBCTContact SelectedContact
        {
            get { return _SelectedContact; }
            set
            {
                if (ReferenceEquals(_SelectedContact, value)) return;
                _SelectedContact = value;
                RaisePropertyChanged("SelectedContact");
            }
        }
        #endregion

        #region FocusedFactorItem
        private DBACFactorItem _FocusedFactorItem;
        public DBACFactorItem FocusedFactorItem
        {
            get { return _FocusedFactorItem; }
            set
            {
                if (ReferenceEquals(_FocusedFactorItem, value)) return;
                _FocusedFactorItem = value;
                RaisePropertyChanged("FocusedFactorItem");
            }
        }
        #endregion

        #region FactorTitleDataSource
        private List<string> _FactorTitleDataSource;
        public List<string> FactorTitleDataSource
        {
            get { return _FactorTitleDataSource; }
            set
            {
                if (ReferenceEquals(_FactorTitleDataSource, value)) return;
                _FactorTitleDataSource = value;
                RaisePropertyChanged("FactorTitleDataSource");
            }
        }
        #endregion


        #region FactorItemList
        private XPCollection<DBACFactorItem> _FactorItemList;
        public XPCollection<DBACFactorItem> FactorItemList
        {
            get { return _FactorItemList; }
            set
            {
                _FactorItemList = value;
                RaisePropertyChanged("FactorItemList");
            }
        }
        #endregion





        #region PercentIncrease
        private decimal _PercentIncrease;
        public decimal PercentIncrease
        {
            get { return _PercentIncrease; }
            set
            {
                if (_PercentIncrease == value) return;
                _PercentIncrease = value;
                RaisePropertyChanged("PercentIncrease");
            }
        }
        #endregion
        #region PercentDiscount
        private decimal _PercentDiscount;
        public decimal PercentDiscount
        {
            get { return _PercentDiscount; }
            set
            {
                if (_PercentDiscount == value) return;
                _PercentDiscount = value;
                RaisePropertyChanged("PercentDiscount");
            }
        }
        #endregion
        #region AmountDiscount
        private decimal _AmountDiscount;
        public decimal AmountDiscount
        {
            get { return _AmountDiscount; }
            set
            {
                if (_AmountDiscount == value) return;
                _AmountDiscount = value;
                RaisePropertyChanged("AmountDiscount");
            }
        }
        #endregion

        #region Note
        private string _Note;
        public string Note
        {
            get { return _Note; }
            set
            {
                if (ReferenceEquals(_Note, value)) return;
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }
        #endregion




        #region T1
        private string _T1;
        public string T1
        {
            get { return _T1; }
            set
            {
                if (ReferenceEquals(_T1, value)) return;
                _T1 = value;
                RaisePropertyChanged("T1");
            }
        }
        #endregion
        #region V1
        private string _V1;
        public string V1
        {
            get { return _V1; }
            set
            {
                if (ReferenceEquals(_V1, value)) return;
                _V1 = value;
                RaisePropertyChanged("V1");
            }
        }
        #endregion

        #region T2
        private string _T2;
        public string T2
        {
            get { return _T2; }
            set
            {
                if (ReferenceEquals(_T2, value)) return;
                _T2 = value;
                RaisePropertyChanged("T2");
            }
        }
        #endregion
        #region V2
        private string _V2;
        public string V2
        {
            get { return _V2; }
            set
            {
                if (ReferenceEquals(_V2, value)) return;
                _V2 = value;
                RaisePropertyChanged("V2");
            }
        }
        #endregion

        #region T3
        private string _T3;
        public string T3
        {
            get { return _T3; }
            set
            {
                if (ReferenceEquals(_T3, value)) return;
                _T3 = value;
                RaisePropertyChanged("T3");
            }
        }
        #endregion
        #region V3
        private string _V3;
        public string V3
        {
            get { return _V3; }
            set
            {
                if (ReferenceEquals(_V3, value)) return;
                _V3 = value;
                RaisePropertyChanged("V3");
            }
        }
        #endregion

        #region T4
        private string _T4;
        public string T4
        {
            get { return _T4; }
            set
            {
                if (ReferenceEquals(_T4, value)) return;
                _T4 = value;
                RaisePropertyChanged("T4");
            }
        }
        #endregion
        #region V4
        private string _V4;
        public string V4
        {
            get { return _V4; }
            set
            {
                if (ReferenceEquals(_V4, value)) return;
                _V4 = value;
                RaisePropertyChanged("V4");
            }
        }
        #endregion

        #region T5
        private string _T5;
        public string T5
        {
            get { return _T5; }
            set
            {
                if (ReferenceEquals(_T5, value)) return;
                _T5 = value;
                RaisePropertyChanged("T5");
            }
        }
        #endregion
        #region V5
        private string _V5;
        public string V5
        {
            get { return _V5; }
            set
            {
                if (ReferenceEquals(_V5, value)) return;
                _V5 = value;
                RaisePropertyChanged("V5");
            }
        }
        #endregion

        #region T6
        private string _T6;
        public string T6
        {
            get { return _T6; }
            set
            {
                if (ReferenceEquals(_T6, value)) return;
                _T6 = value;
                RaisePropertyChanged("T6");
            }
        }
        #endregion
        #region V6
        private string _V6;
        public string V6
        {
            get { return _V6; }
            set
            {
                if (ReferenceEquals(_V6, value)) return;
                _V6 = value;
                RaisePropertyChanged("V6");
            }
        }
        #endregion

        #region T7
        private string _T7;
        public string T7
        {
            get { return _T7; }
            set
            {
                if (ReferenceEquals(_T7, value)) return;
                _T7 = value;
                RaisePropertyChanged("T7");
            }
        }
        #endregion
        #region V7
        private string _V7;
        public string V7
        {
            get { return _V7; }
            set
            {
                if (ReferenceEquals(_V7, value)) return;
                _V7 = value;
                RaisePropertyChanged("V7");
            }
        }
        #endregion

        #region T8
        private string _T8;
        public string T8
        {
            get { return _T8; }
            set
            {
                if (ReferenceEquals(_T8, value)) return;
                _T8 = value;
                RaisePropertyChanged("T8");
            }
        }
        #endregion
        #region V8
        private string _V8;
        public string V8
        {
            get { return _V8; }
            set
            {
                if (ReferenceEquals(_V8, value)) return;
                _V8 = value;
                RaisePropertyChanged("V8");
            }
        }
        #endregion

        #region T9
        private string _T9;
        public string T9
        {
            get { return _T9; }
            set
            {
                if (ReferenceEquals(_T9, value)) return;
                _T9 = value;
                RaisePropertyChanged("T9");
            }
        }
        #endregion
        #region V9
        private string _V9;
        public string V9
        {
            get { return _V9; }
            set
            {
                if (ReferenceEquals(_V9, value)) return;
                _V9 = value;
                RaisePropertyChanged("V9");
            }
        }
        #endregion

        #region T10
        private string _T10;
        public string T10
        {
            get { return _T10; }
            set
            {
                if (ReferenceEquals(_T10, value)) return;
                _T10 = value;
                RaisePropertyChanged("T10");
            }
        }
        #endregion
        #region V10
        private string _V10;
        public string V10
        {
            get { return _V10; }
            set
            {
                if (ReferenceEquals(_V10, value)) return;
                _V10 = value;
                RaisePropertyChanged("V10");
            }
        }
        #endregion


        private void InitCommands()
        {
            CommandSelectContact = new RelayCommand(SelectContact, () => true);
            CommandSaveFactor = new RelayCommand(SaveFactor, () => IsFactorMode && !string.IsNullOrWhiteSpace(Title));
            CommandItemAdd = new RelayCommand(ItemAdd, () => IsItemMode && DynamicSelectedData != null);
            CommandItemEdit = new RelayCommand(ItemEdit, () => IsItemMode && DynamicSelectedData != null && FocusedFactorItem != null);
            CommandItemDelete = new RelayCommand(ItemDelete, () => IsItemMode && DynamicSelectedData != null && FocusedFactorItem != null);

            CommandPrint = new RelayCommand(Print, () => DynamicSelectedData != null);
            CommandPrintFromTemplate = new RelayCommand(PrintFromTemplate, () => DynamicSelectedData != null);
            CommandSave = new RelayCommand(Store, () => DynamicSelectedData != null);

            CommandSaveTemplate = new RelayCommand(() =>
            {
                var w = new WFactorTemplateSave()
                {
                    Items = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>(T1 ?? "",V1 ?? ""),
                        new Tuple<string, string>(T2 ?? "",V2 ?? ""),
                        new Tuple<string, string>(T3 ?? "",V3 ?? ""),
                        new Tuple<string, string>(T4 ?? "",V4 ?? ""),
                        new Tuple<string, string>(T5 ?? "",V5 ?? ""),
                        new Tuple<string, string>(T6 ?? "",V6 ?? ""),
                        new Tuple<string, string>(T7 ?? "",V7 ?? ""),
                        new Tuple<string, string>(T8 ?? "",V8 ?? ""),
                        new Tuple<string, string>(T9 ?? "",V9 ?? ""),
                        new Tuple<string, string>(T10 ?? "",V10 ?? ""),
                    },
                    Owner = DynamicOwner,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
                w.ShowDialog();
                if (w.SelectedItem == null)
                    return;
                var val = w.SelectedItem.Value;
                PopulateTemplate(val, false);

            }, () => true);
        }

        private void PrintFromTemplate()
        {
            SaveFactor();
            var w = new WSelectTemplate
            {
                Owner = DynamicOwner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.Factor = DynamicSelectedData;
            var dr = w.ShowDialog();
            if(dr!=true)
                return;
            DynamicOwner.Close();
        }

        private void PopulateTemplate(string val, bool isEditMode)
        {
            try
            {
                var items = val.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                val = items[0];
                var ss = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                T1 = ss[0];
                V1 = GetValueForV(ss[1], isEditMode);

                val = items[1];
                ss = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                T2 = ss[0];
                V2 = GetValueForV(ss[1], isEditMode);

                val = items[2];
                ss = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                T3 = ss[0];
                V3 = GetValueForV(ss[1], isEditMode);

                val = items[3];
                ss = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                T4 = ss[0];
                V4 = GetValueForV(ss[1], isEditMode);

                val = items[4];
                ss = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                T5 = ss[0];
                V5 = GetValueForV(ss[1], isEditMode);

                val = items[5];
                ss = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                T6 = ss[0];
                V6 = GetValueForV(ss[1], isEditMode);

                val = items[6];
                ss = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                T7 = ss[0];
                V7 = GetValueForV(ss[1], isEditMode);

                val = items[7];
                ss = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                T8 = ss[0];
                V8 = GetValueForV(ss[1], isEditMode);

                val = items[8];
                ss = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                T9 = ss[0];
                V9 = GetValueForV(ss[1], isEditMode);

                val = items[9];
                ss = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                T10 = ss[0];
                V10 = GetValueForV(ss[1], isEditMode);
            }
            catch
            {
            }
        }

        private string GetValueForV(string s, bool isEditMode)
        {
            if (isEditMode)
            {
                return s;
            }
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            if (s.StartsWith(":"))
                return s;
            return "";
        }

        private void Store()
        {
            if (Validate())
                if (Save())
                {
                    IsItemMode = true;
                    IsFactorMode = false;
                    DynamicOwner.DialogResult = true;
                    DynamicOwner.Close();
                }
        }

        private void Print()
        {
            SaveFactor();
            var xrr = new xrReportFactor(DynamicSelectedData);
            var printTool = new DevExpress.XtraReports.UI.ReportPrintTool(xrr);
            printTool.ShowPreviewDialog();
        }

        private void ItemAdd()
        {
            var w = new WFactorItemAddEdit(DynamicSelectedData, null)
            {
                Owner = Window.GetWindow(MainView)
            };
            w.ShowDialog();
            PopulateFactorItems();
        }

        private void ItemEdit()
        {
            if (FocusedFactorItem == null) return;
            var w = new WFactorItemAddEdit(DynamicSelectedData, FocusedFactorItem)
            {
                Owner = Window.GetWindow(MainView)
            };
            w.ShowDialog();
            PopulateFactorItems();
        }

        private void ItemDelete()
        {
            var srh = DynamicTableView.Grid.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("موارد انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicTableView.Grid.GetRow(n) as DBACFactorItem).ToList();

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
                            var factor = db.Factor.Oid;
                            if (w.NeedToCancel) return;
                            var db2 = DBACFactorItem.FindByOid(dxs, db.Oid);
                            db2.Delete();
                            db2.Save();
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(
                                () =>
                                {
                                    var db3 = DBACFactor.FindByOid(ADatabase.Dxs, factor);
                                    db3.FactoreItems.Reload();
                                    db3.UpdateSums();
                                    db3.Save();
                                }));

                            success++;
                        }
                        catch
                        {
                            failed++;
                        }
                    }
                }, null, DynamicOwner);
            POLMessageBox.ShowInformation(string.Format("گزارش حذف : {0}{0}موفقیت آمیز : {1}{0}بروز خطا : {2}", Environment.NewLine, success, failed), DynamicOwner);
            PopulateFactorItems();
        }

        private void SaveFactor()
        {
            if (Validate())
                if (Save())
                {
                    IsItemMode = true;
                    IsFactorMode = false;
                }
        }

        private void SelectContact()
        {
            var poc = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var ct = poc.ShowSelectContact(DynamicOwner, null);
            if (ct is DBCTContact)
            {
                SelectedContact = ((DBCTContact)ct);
                if (!string.IsNullOrEmpty(Title))
                    Title = SelectedContact.Title;
            }
        }




        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                POLMessageBox.ShowError("عنوان چاپی معتبر نمی باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }

            if (SelectedContact == null)
            {
                POLMessageBox.ShowError("پرونده معتبر نمی باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }

            if (!IsEditMode)
            {
                var db = DBACFactor.FindByCode(ADatabase.Dxs, Code);
                if (DynamicSelectedData == null)
                {
                    if (db != null)
                    {
                        POLMessageBox.ShowError("كد تكراری می باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                        return false;
                    }
                }
            }
            else
            {
                var db = DBACFactor.FindByCode(ADatabase.Dxs, Code);
                if (DynamicSelectedData != null)
                {
                    if (db.Oid != DynamicSelectedData.Oid)
                    {
                        POLMessageBox.ShowError("كد تكراری می باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                        return false;
                    }
                }
            }
            return true;
        }
        private bool Save()
        {
            try
            {
                if (DynamicSelectedData == null)
                {
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        DynamicSelectedData = new DBACFactor(uow)
                        {
                            Title = Title.Trim(),
                            Code = Code,
                            CodeShow = CodeShow,
                            Date = Date.Date,
                            IsPreFactor = IsPreFactor,
                            Contact = SelectedContact == null ? null : DBCTContact.FindByOid(uow, SelectedContact.Oid),
                            AmountDiscount = AmountDiscount,
                            PercentDiscount = PercentDiscount,
                            PercentIncrease = PercentIncrease,
                            TitleOfTitle = TitleOfTitle,
                            Note = Note,
                            TemplateValues = GetTemplateValue(),
                        };
                        DynamicSelectedData.UpdateSums();
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBACFactor.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = Title.Trim();
                    DynamicSelectedData.Code = Code;
                    DynamicSelectedData.CodeShow = CodeShow;
                    DynamicSelectedData.Date = Date.Date;
                    DynamicSelectedData.IsPreFactor = IsPreFactor;
                    DynamicSelectedData.Contact = SelectedContact;
                    DynamicSelectedData.AmountDiscount = AmountDiscount;
                    DynamicSelectedData.PercentDiscount = PercentDiscount;
                    DynamicSelectedData.PercentIncrease = PercentIncrease;
                    DynamicSelectedData.TitleOfTitle = TitleOfTitle;
                    DynamicSelectedData.Note = Note;
                    DynamicSelectedData.UpdateSums();
                    DynamicSelectedData.TemplateValues = GetTemplateValue();
                    DynamicSelectedData.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowWarning(ex.Message, DynamicOwner);
                return false;
            }
        }

        private string GetTemplateValue()
        {
            var vv = new List<Tuple<string, string>>
            {
                new Tuple<string, string>(T1 ?? "", V1 ?? ""),
                new Tuple<string, string>(T2 ?? "", V2 ?? ""),
                new Tuple<string, string>(T3 ?? "", V3 ?? ""),
                new Tuple<string, string>(T4 ?? "", V4 ?? ""),
                new Tuple<string, string>(T5 ?? "", V5 ?? ""),
                new Tuple<string, string>(T6 ?? "", V6 ?? ""),
                new Tuple<string, string>(T7 ?? "", V7 ?? ""),
                new Tuple<string, string>(T8 ?? "", V8 ?? ""),
                new Tuple<string, string>(T9 ?? "", V9 ?? ""),
                new Tuple<string, string>(T10 ?? "", V10 ?? ""),
            };
            var rv = "";
            foreach (var t in vv)
            {
                rv += $"{t.Item1};{t.Item2}|";
            }
            return rv;
        }


        private void PopulateFactorItems()
        {
            if (DynamicSelectedData != null)
            {
                DynamicSelectedData.FactoreItems.Reload();
                FactorItemList = DynamicSelectedData.FactoreItems;

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(500);
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        new Action(() =>
                        {
                            DynamicTableView.BestFitColumns();
                        }));
                });
            }
        }
        private void PopulateData()
        {
            IsEditMode = (DynamicSelectedData != null);
            if (!IsEditMode)
            {
                IsItemMode = false;
                IsFactorMode = true;
                if (DynamicSelectedContact != null)
                {
                    SelectedContact = DynamicSelectedContact;
                }
            }
            else
            {
                IsItemMode = true;
                IsFactorMode = true;
            }

            Code = DBACFactor.GetNextCode(ADatabase.Dxs);
            Date = DateTime.Now.Date;
            IsPreFactor = true;
            Title = string.Empty;
            TitleOfTitle = string.Empty;
            
            CodeShow = true;



            if (DynamicSelectedData == null)
            {
                using (var _dxs = ADatabase.GetNewSession())
                {
                    var deftemplate = new XPQuery<DBACFactorTemplate>(_dxs).FirstOrDefault(n => n.Title.StartsWith("* "));
                    if (deftemplate != null)
                        PopulateTemplate(deftemplate.Value, false);
                }
                return;
            }

            Code = DynamicSelectedData.Code;
            CodeShow = DynamicSelectedData.CodeShow;
            Date = DynamicSelectedData.Date;
            IsPreFactor = DynamicSelectedData.IsPreFactor;
            Title = DynamicSelectedData.Title;
            SelectedContact = DynamicSelectedData.Contact;
            TitleOfTitle = DynamicSelectedData.TitleOfTitle;
            PercentDiscount = DynamicSelectedData.PercentDiscount;
            AmountDiscount = DynamicSelectedData.AmountDiscount;
            PercentIncrease = DynamicSelectedData.PercentIncrease;
            Note = DynamicSelectedData.Note;
            PopulateFactorItems();
            PopulateTemplate(DynamicSelectedData.TemplateValues, true);

        }

        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
            DynamicSelectedContact = MainView.DynamicSelectedContact;
            DynamicTableView = MainView.DynamicTableView;

            DynamicTableView.Grid.MouseDoubleClick +=
                (s, e) =>
                {
                    var i = DynamicTableView.GetRowHandleByMouseEventArgs(e);
                    if (i < 0) return;
                    if (CommandItemEdit.CanExecute(null))
                        CommandItemEdit.Execute(null);
                    e.Handled = true;
                };
        }

        #region [COMMANDS]

        public RelayCommand CommandSelectContact { get; set; }
        public RelayCommand CommandSaveFactor { get; set; }
        public RelayCommand CommandItemAdd { get; set; }
        public RelayCommand CommandItemEdit { get; set; }
        public RelayCommand CommandItemDelete { get; set; }

        public RelayCommand CommandPrint { get; set; }
        public RelayCommand CommandSave { get; set; }
        public RelayCommand CommandSaveTemplate { get; set; }

        public RelayCommand CommandPrintFromTemplate { get; set; }
        
        #endregion


    }
}
