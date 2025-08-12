using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;

namespace POC.Module.Finalizer
{
    [Version]
    [Priority(ConstantPOCModules.OrderFinalizer)]
    [Module(ModuleName = ConstantPOCModules.NameFinalizer)]
    public class ModuleFinalizer : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }

        public ModuleFinalizer(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameFinalizer), Category.Debug, Priority.None);
            ModuleSyncronizer.RaiseOnModuleFinilize();
        }
        #endregion
    }
}
