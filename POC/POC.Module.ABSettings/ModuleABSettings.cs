using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using POC.Module.ABSettings.Models;
using POC.Module.ABSettings.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;

namespace POC.Module.ABSettings
{
    [Version]
    [Priority(ConstantPOCModules.OrderABSettings)]
    [Module(ModuleName = ConstantPOCModules.NameABSettings)]
    public class ModuleABSettings : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IApplicationBar ApplicationBar { get; set; }
        private ILoggerFacade Logger { get; set; }

        public ModuleABSettings(IUnityContainer unityContainer, ILoggerFacade logger, IApplicationBar applicationBar)
        {
            UnityContainer = unityContainer;
            ApplicationBar = applicationBar;
            Logger = logger;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameABSettings), Category.Debug, Priority.None);
            ApplicationBar.RegisterContent(new ApplicationBarHolder
            {
                Name = ConstantApplicationBar.NameSettings,
                Order = ConstantApplicationBar.OrderSettings,
                Title = "تنظیمات",
                ViewType = typeof(UApplicationBarSettings),
                ModelType = typeof(MApplicationBarSettings)
            });
        }
        #endregion
    }
}
