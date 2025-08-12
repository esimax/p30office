using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using DevExpress.Xpo;

namespace POC.Module.Contact.Models
{
    public class MContactSelectBy : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }

        private ContactSelectByResult Result { get; set; }

        #region CTOR
        public MContactSelectBy(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            Result = new ContactSelectByResult { SelectionType = EnumContactSelectType.All, SelectionOid = Guid.Empty };

            InitCommands();
            GetDynamicData();
            PopulateCategoryList();
            PopulateBasketList();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "انتخاب پرونده"; }
        }
        #endregion



        #region IsAll
        public bool IsAll
        {
            get { return Result.SelectionType == EnumContactSelectType.All; }
            set
            {
                if (value)
                    Result.SelectionType = EnumContactSelectType.All;
                RaisePropertyChanged("IsAll");
            }
        }
        #endregion
        #region IsCategory
        public bool IsCategory
        {
            get { return Result.SelectionType == EnumContactSelectType.Category; }
            set
            {
                if (value)
                    Result.SelectionType = EnumContactSelectType.Category;
                RaisePropertyChanged("IsCategory");
            }
        }
        #endregion
        #region IsBasket
        public bool IsBasket
        {
            get { return Result.SelectionType == EnumContactSelectType.SelectionBasket; }
            set
            {
                if (value)
                    Result.SelectionType = EnumContactSelectType.SelectionBasket;
                RaisePropertyChanged("IsBasket");
            }
        }
        #endregion





        #region SelectedCategory
        private DBCTContactCat _SelectedCategory;
        public DBCTContactCat SelectedCategory
        {
            get { return _SelectedCategory; }
            set
            {
                if (ReferenceEquals(_SelectedCategory, value)) return;
                _SelectedCategory = value;
                RaisePropertyChanged("SelectedCategory");
            }
        }
        #endregion
        #region ContactCategoryList
        private List<DBCTContactCat> _ContactCategoryList;
        public List<DBCTContactCat> ContactCategoryList
        {
            get { return _ContactCategoryList; }
            set
            {
                if (ReferenceEquals(_ContactCategoryList, value)) return;
                _ContactCategoryList = value;
                RaisePropertyChanged("ContactCategoryList");
            }
        }
        #endregion

        #region SelectedBasket
        private DBCTContactSelection _SelectedBasket;
        public DBCTContactSelection SelectedBasket
        {
            get { return _SelectedBasket; }
            set
            {
                if (ReferenceEquals(_SelectedBasket, value)) return;
                _SelectedBasket = value;
                RaisePropertyChanged("SelectedBasket");
            }
        }
        #endregion
        #region ContactBasketList
        private XPCollection<DBCTContactSelection> _ContactBasketList;
        public XPCollection<DBCTContactSelection> ContactBasketList
        {
            get { return _ContactBasketList; }
            set
            {
                if (ReferenceEquals(_ContactBasketList, value)) return;
                _ContactBasketList = value;
                RaisePropertyChanged("ContactBasketList");
            }
        }
        #endregion




        #region [METHODS]
        private void PopulateCategoryList()
        {
            ContactCategoryList =
                (from n in ACacheData.GetContactCatList() orderby n.Title select (DBCTContactCat)n.Tag).ToList();
        }
        private void PopulateBasketList()
        {
            ContactBasketList = DBCTContactSelection.GetByUser(ADatabase.Dxs, AMembership.ActiveUser.UserID, true, null);
        }
        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => IsAll || IsBasket || IsCategory);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp33 != "");
        }

        private void OK()
        {
            if (Validate())
                if (Save())
                {
                    MainView.DialogResult = true;
                    MainView.Close();
                }
        }
        private bool Validate()
        {
            if (IsCategory && SelectedCategory == null)
            {
                POLMessageBox.ShowError("خطا : دسته پرونده ها به درستی انتخاب نشده است.", MainView);
                return false;
            }
            if (IsBasket && SelectedBasket == null)
            {
                POLMessageBox.ShowError("خطا : سبد انتخاب به درستی انتخاب نشده است.", MainView);
                return false;
            }

            return true;
        }
        private bool Save()
        {
            if (IsAll)
            {
                Result.SelectionType = EnumContactSelectType.All;
                Result.SelectionOid = Guid.Empty;
            }
            if (IsBasket)
            {
                Result.SelectionType = EnumContactSelectType.SelectionBasket;
                Result.SelectionOid = SelectedBasket.Oid;
            }
            if (IsCategory)
            {
                Result.SelectionType = EnumContactSelectType.Category;
                Result.SelectionOid = SelectedCategory.Oid;
            }
            MainView.DynamicResult = Result;
            return true;
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp33);
        } 
        private void GetDynamicData()
        {
        }
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
