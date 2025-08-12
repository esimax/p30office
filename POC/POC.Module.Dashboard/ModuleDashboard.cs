using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.Dashboard.Models;
using POC.Module.Dashboard.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;

namespace POC.Module.Dashboard
{
    [Version]
    [Priority(ConstantPOCModules.OrderDashboard)]
    [Module(ModuleName = ConstantPOCModules.NameDashboard)]
    public class ModuleDashboard : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IBaseTable ABaseTable { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }


        public ModuleDashboard(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer,
            IPOCRootTools rootTools, IPOCContactModule contactModule, IBaseTable baseTable, IPOCMainWindow pocMainWindow)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            AContactModule = contactModule;
            ABaseTable = baseTable;
            APOCMainWindow = pocMainWindow;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameDashboard), Category.Debug, Priority.None);

            RegisterRootTool();

        }
        #endregion

        private void RegisterRootTool()
        {
            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameDashboard,
                    Image = HelperImage.GetSpecialImage32("_32_Dashboard.png"),
                    Order = ConstantPOCRootTool.OrderDashboard,
                    Permission = (int)PCOPermissions.Dashboard_View,
                    Title = "داشبورد",
                    Tooltip = "مشاهده و مدیریت اطلاعات در یك نگاه",
                    Command = new RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(UDashboard), typeof(MDashboard));
                        }),
                    InTamas = true,
                });
        }
    }
}
