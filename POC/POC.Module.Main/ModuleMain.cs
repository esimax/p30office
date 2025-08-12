using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using POC.Module.Main.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;

namespace POC.Module.Main
{
    [Version]
    [Priority(ConstantPOCModules.OrderMain)]
    [Module(ModuleName = ConstantPOCModules.NameMain)]
    public class ModuleMain : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCMainWindow AMainWindow { get; set; }

        public ModuleMain(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer,
            IPOCMainWindow pocMainWindow)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            AMainWindow = pocMainWindow;
         }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameMain), Category.Debug, Priority.None);
            AMainWindow.RegisterMainWindow(new DXRibbonWindowMain());
        }
        #endregion
    }
}
