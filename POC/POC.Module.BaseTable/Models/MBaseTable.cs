using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.BaseTable.Models
{
    public class MBaseTable : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IBaseTable ABaseTable { get; set; }
        private POCCore APOCCore { get; set; }


        private dynamic MainView { get; set; }
        private Window Owner { get; set; }


        #region CTOR
        public MBaseTable(object mainView)
        {
            MainView = mainView;

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            ABaseTable = ServiceLocator.Current.GetInstance<IBaseTable>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            InitDynamics();
            PopulateBaseTableList();
        }


        #endregion

        public List<BaseTableItem> BaseTableList { get; set; }

        private void PopulateBaseTableList()
        {
            BaseTableList = ABaseTable.GetList().Where(n => n.IsTamas || !APOCCore.STCI.IsTamas).ToList();
        }
        
        private void InitDynamics()
        {
            try
            {
                Owner = MainView.DynamicOwner;
            }
            catch
            {
            }
        }
    }
}

