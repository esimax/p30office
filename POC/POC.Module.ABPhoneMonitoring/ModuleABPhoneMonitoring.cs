using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using POC.Module.ABPhoneMonitoring.Models;
using POC.Module.ABPhoneMonitoring.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;

namespace POC.Module.ABPhoneMonitoring
{
    [Version]
    [Priority(ConstantPOCModules.OrderABPhoneMonitoring)]
    [Module(ModuleName = ConstantPOCModules.NameABPhoneMonitoring)]
    public class ModuleABPhoneMonitoring : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IApplicationBar ApplicationBar { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCSettings APOCSettings { get; set; }

        public ModuleABPhoneMonitoring(IUnityContainer unityContainer, ILoggerFacade logger, IApplicationBar applicationBar, IPOCSettings settings)
        {
            UnityContainer = unityContainer;
            ApplicationBar = applicationBar;
            Logger = logger;
            APOCSettings = settings;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameABPhoneMonitoring), Category.Debug, Priority.None);
            ApplicationBar.RegisterContent(new ApplicationBarHolder
            {
                Name = ConstantApplicationBar.NamePhoneMonitoring,
                Order = ConstantApplicationBar.OrderPhoneMonitoring,
                Title = "مانیتورینگ",
                ViewType = typeof(UApplicationBarPhoneMonitoring),
                ModelType = typeof(MApplicationBarPhoneMonitoring),
                IsFirst = false,
            });

        }
        #endregion
    }
}
