using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using POC.Module.ABTools.Models;
using POC.Module.ABTools.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;

namespace POC.Module.ABTools
{
    [Version]
    [Priority(ConstantPOCModules.OrderABTools)]
    [Module(ModuleName = ConstantPOCModules.NameABTools)]
    public class ModuleABTools : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IApplicationBar ApplicationBar { get; set; }
        private ILoggerFacade Logger { get; set; }

        public ModuleABTools(IUnityContainer unityContainer, ILoggerFacade logger, IApplicationBar applicationBar)
        {
            UnityContainer = unityContainer;
            ApplicationBar = applicationBar;
            Logger = logger;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log("Initializing "+ConstantPOCModules.NameABTools,Category.Debug, Priority.None);
            ApplicationBar.RegisterContent(new ApplicationBarHolder
            {
                Name = ConstantApplicationBar.NameTools,
                Order = ConstantApplicationBar.OrderTools,
                Title = "خانه",
                ViewType = typeof(UApplicationBarTools),
                ModelType = typeof(MApplicationBarTools)
            });

            ApplicationBar.RegisterContent(new ApplicationBarHolder
            {
                Name = ConstantApplicationBar.NameChat,
                Order = ConstantApplicationBar.OrderChat,
                Title = "چت",
                ViewType = typeof(UApplicationBarChat),
                ModelType = typeof(MApplicationBarChat)
            });
        }
        #endregion
    }
}
