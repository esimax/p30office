using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Contact.Models
{
    public class MContactCatSelect : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }


        private dynamic MainView { get; set; }
        private Window Owner { get; set; }

        #region CTOR
        public MContactCatSelect(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();

            InitDynamics();
            InitCommands();
            PopulateCategoryList();
        }
        #endregion

        private void PopulateCategoryList()
        {
            var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            var q = from n in aCacheData.GetContactCatList()
                    let cat = (DBCTContactCat)n.Tag
                    select new CategoryHolder { CategoryTitle = cat.Title, Category = cat, CategoryState = null };
            CategoryList = new ObservableCollection<CategoryHolder>();
            foreach (var v in q)
            {
                CategoryList.Add(v);
            }
        }

        #region WindowTitle
        public string WindowTitle
        {
            get { return "انتخاب دسته"; }
        }
        #endregion

        #region CategoryList
        public ObservableCollection<CategoryHolder> CategoryList { get; set; }
        #endregion


        #region SelectedContactCat
        private CategoryHolder _SelectedContactCat;
        public CategoryHolder SelectedContactCat
        {
            get { return _SelectedContactCat; }
            set
            {
                if (value == _SelectedContactCat)
                    return;

                _SelectedContactCat = value;
                RaisePropertyChanged("SelectedContactCat");
            }
        }
        #endregion


        private void InitDynamics()
        {
            Owner = MainView as Window;
        }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    MainView.SelectedContactCat = SelectedContactCat == null ? null : SelectedContactCat.Category;
                    Owner.DialogResult = true;
                    Owner.Close();
                }, () => true);
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCancel { get; set; }
        #endregion
    }
}
