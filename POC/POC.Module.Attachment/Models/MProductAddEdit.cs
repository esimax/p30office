using System;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.AC;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;

namespace POC.Module.Attachment.Models
{
    public class MProductAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBACProduct DynamicSelectedData { get; set; }

        #region CTOR
        public MProductAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitCommands();
            GetDynamicData();
            PopulateUnit();
            PopulateData();

            IsEditMode = (DynamicSelectedData != null);

        }

        private void PopulateUnit()
        {
            UnitList = DBBTUnit.GetAll(ADatabase.Dxs);
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات كالا/محصولات"; }
        }
        #endregion

        public bool IsEditMode { get; set; }

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

        #region Price
        private decimal _Price;
        public decimal Price
        {
            get { return _Price; }
            set
            {
                if (_Price == value) return;
                _Price = value;
                RaisePropertyChanged("Price");
            }
        }
        #endregion

        #region Unit
        private DBBTUnit _Unit;
        public DBBTUnit Unit
        {
            get { return _Unit; }
            set
            {
                if (_Unit == value) return;
                _Unit = value;
                RaisePropertyChanged("Unit");
            }
        }
        #endregion

        public XPCollection<DBBTUnit> UnitList { get; set; }


        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => !string.IsNullOrWhiteSpace(Title));
        }

        private void OK()
        {
            if (Validate())
                if (Save())
                {
                    DynamicOwner.DialogResult = true;
                    DynamicOwner.Close();
                }
        }



        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                POLMessageBox.ShowError("عنوان معتبر نمی باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }

            var db = DBACProduct.FindByCode(ADatabase.Dxs, Code);
            if (DynamicSelectedData == null)
            {
                if (db != null)
                {
                    POLMessageBox.ShowError("كد تكراری می باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                    return false;
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
                        DynamicSelectedData = new DBACProduct(uow)
                        {
                            Title = Title.Trim(),
                            Code = Code,
                            Price = Price,
                            Unit = Unit == null ? null : DBBTUnit.FindByOid(uow, Unit.Oid),
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBACProduct.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = Title.Trim();
                    DynamicSelectedData.Code = Code;
                    DynamicSelectedData.Price = Price;
                    DynamicSelectedData.Unit = Unit;
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

        private void PopulateData()
        {
            Code = DBACProduct.GetNextCode(ADatabase.Dxs);
            if (DynamicSelectedData == null) return;
            Title = DynamicSelectedData.Title;
            Code = DynamicSelectedData.Code;
            Price = DynamicSelectedData.Price;
            Unit = DynamicSelectedData.Unit;
        }
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion


    }
}
