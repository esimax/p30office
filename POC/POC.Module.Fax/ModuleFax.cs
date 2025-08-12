using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;

namespace POC.Module.Fax
{
    [Version]
    [Priority(ConstantPOCModules.OrderFax)]
    [Module(ModuleName = ConstantPOCModules.NameFax)]
    public class ModuleFax : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }

        public ModuleFax(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer, IPOCRootTools rootTools, IPOCContactModule contactModule)
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
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameFax), Category.Debug, Priority.None);

        }

        private void RegisterContactModule()
        {
            AContactModule.RegisterContactModule(
                new POCContactModuleItem
                {
                    Key = ConstantPOCModules.NameFax,
                    Image = HelperImage.GetStandardImage32("_32_Fax.png"),
                    Order = ConstantPOCModuleContact.OrderFax,
                    Permission = 0,
                    Title = "فكس",
                    Tooltip = " فكس های ارسالی و دریافتی",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                        }),
                    InTamas = false,
                });
        }

        private void RegisterRootTool()
        {
            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameFax,
                    Image = HelperImage.GetStandardImage32("_32_Fax.png"),
                    Order = ConstantPOCRootTool.OrderFax,
                    Permission = -1,
                    Title = "فكس",
                    Tooltip = "مدیریت فكسهای ارسالی و دریافتی",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                        }),
                    InTamas = true,
                });
        }
        #endregion
    }
}
