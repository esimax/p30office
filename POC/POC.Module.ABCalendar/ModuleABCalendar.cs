using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using POC.Module.ABCalendar.Models;
using POC.Module.ABCalendar.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;

namespace POC.Module.ABCalendar
{

    [Version]
    [Priority(ConstantPOCModules.OrderABCalendar)]
    [Module(ModuleName = ConstantPOCModules.NameABCalendar)]
    public class ModuleABCalendar : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IApplicationBar ApplicationBar { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCSettings APOCSettings { get; set; }

        public ModuleABCalendar(IUnityContainer unityContainer, ILoggerFacade logger, IApplicationBar applicationBar, IPOCSettings settings)
        {
            UnityContainer = unityContainer;
            ApplicationBar = applicationBar;
            Logger = logger;
            APOCSettings = settings;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameABCalendar), Category.Debug, Priority.None);
            ApplicationBar.RegisterContent(new ApplicationBarHolder
            {
                Name = ConstantApplicationBar.NameCalendar,
                Order = ConstantApplicationBar.OrderCalendar,
                Title = "تقویم",
                ViewType = typeof(UApplicationBarCalendar),
                ModelType = typeof(MApplicationBarCalendar),
                IsFirst = false,
            });

            APOCSettings.RegisterUIElement("PrayTime", new POCSettingItem
                {
                    Order = 1,
                    Permission = 0,
                    Element = new Views.USettingPrayTime(),
                });
        }
        #endregion
    }
}
