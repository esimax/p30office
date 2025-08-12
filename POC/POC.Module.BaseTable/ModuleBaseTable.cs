using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;

namespace POC.Module.BaseTable
{
    [Version]
    [Priority(ConstantPOCModules.OrderBaseTable)]
    [Module(ModuleName = ConstantPOCModules.NameBaseTable)]
    public class ModuleBaseTable : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }

        public ModuleBaseTable(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer, IPOCRootTools rootTools, IPOCContactModule contactModule)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            AContactModule = contactModule;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameBaseTable), Category.Debug, Priority.None);

            RegisterRootTool();
        }

        private void RegisterRootTool()
        {
            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameBaseTable,
                    Image = HelperImage.GetStandardImage32("_32_List.png"),
                    Order = ConstantPOCRootTool.OrderBaseTable,
                    Permission = (int)PCOPermissions.BaseTable_View,
                    Title = "جداول پایه",
                    Tooltip = "مدیریت جداول پایه",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(Views.UBaseTable), typeof(Models.MBaseTable));
                        }),
                    InTamas = true,
                });
        }
        #endregion
    }
}
