using System;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.AC;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;

namespace POC.Module.Attachment.Models
{
    public class MFactorItemAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBACFactorItem DynamicSelectedData { get; set; }
        private DBACFactor DynamicFactor { get; set; }

        #region CTOR
        public MFactorItemAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitCommands();
            GetDynamicData();
            PopulateProductList();
            PopulateData();


        }

        private void PopulateProductList()
        {
            ProductList = DBACProduct.GetAll(ADatabase.Dxs);
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اقلام فاكتور"; }
        }
        #endregion


        #region Product
        private DBACProduct _Product;
        public DBACProduct Product
        {
            get { return _Product; }
            set
            {
                if (ReferenceEquals(_Product, value)) return;
                _Product = value;
                RaisePropertyChanged("Product");
                if (_Product != null)
                {
                    Price = _Product.Price;
                    if (_Product.Unit != null)
                        UnitTitle = _Product.Unit.Title;
                    else
                    {
                        UnitTitle = string.Empty;
                    }
                }
                else
                {
                    Price = 0;
                    UnitTitle = string.Empty;
                }
                UpdateSum();
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
                UpdateSum();
            }
        }
        #endregion

        #region UnitTitle
        private string _UnitTitle;
        public string UnitTitle
        {
            get { return _UnitTitle; }
            set
            {
                if (_UnitTitle == value) return;
                _UnitTitle = value;
                RaisePropertyChanged("UnitTitle");
            }
        }
        #endregion

        #region ProductCount
        private double _ProductCount;
        public double ProductCount
        {
            get { return _ProductCount; }
            set
            {
                if (_ProductCount == value) return;
                _ProductCount = value;
                RaisePropertyChanged("ProductCount");
                UpdateSum();
            }
        }
        #endregion


       
        #region Sum
        private decimal _Sum;
        public decimal Sum
        {
            get { return _Sum; }
            set
            {
                if (_Sum == value) return;
                _Sum = value;
                RaisePropertyChanged("Sum");
            }
        }
        #endregion

        private void UpdateSum()
        {
            var cost = (double)Price*ProductCount;
            Sum = (decimal)cost; 
        }



        #region [METHODS]
        public XPCollection<DBACProduct> ProductList { get; set; }


        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp12 != "");
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
            if (Product == null)
            {
                POLMessageBox.ShowError("كالا معتبر نمی باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
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
                        DynamicSelectedData = new DBACFactorItem(uow)
                        {
                            Product = DBACProduct.FindByOid(uow, Product.Oid),
                            Count = ProductCount,
                            Price = Price,
                            Sum = Sum,
                            Factor = DBACFactor.FindByOid(uow, DynamicFactor.Oid),
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBACFactorItem.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                    DynamicSelectedData.Factor.Reload();
                    DynamicSelectedData.Factor.UpdateSums();
                    DynamicSelectedData.Factor.Save();
                }
                else
                {
                    DynamicSelectedData.Product = Product;
                    DynamicSelectedData.Count = ProductCount;
                    DynamicSelectedData.Price = Price;
                    DynamicSelectedData.Sum = Sum;
                    DynamicSelectedData.Save();

                    DynamicSelectedData.Factor.Reload();
                    DynamicSelectedData.Factor.UpdateSums();
                    DynamicSelectedData.Factor.Save();
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
            ProductCount = 1;
            Price = 0;
            Sum = 0;
            if (DynamicSelectedData == null)
                return;

            Product = DynamicSelectedData.Product;
            ProductCount = DynamicSelectedData.Count;
            Price = DynamicSelectedData.Price;
            Sum = DynamicSelectedData.Sum;
        }
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
            DynamicFactor = MainView.DynamicFactor;
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp12);
        } 
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion


    }
}
