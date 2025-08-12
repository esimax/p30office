using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;

namespace POC.Module.Calendar
{
    [Version]
    [Priority(ConstantPOCModules.OrderCalendar)]
    [Module(ModuleName = ConstantPOCModules.NameCalendar)]
    public class ModuleCalendar : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IPOCSettings APOCSettings { get; set; }
        private IMessagingClient AMessageClient { get; set; }
        private IPopup APopup { get; set; }
        private POCCore APOCCore { get; set; }
        private IMembership AMembership { get; set; }

        public ModuleCalendar(IUnityContainer unityContainer, ILoggerFacade logger,
                          IModuleSyncronizer moduleSyncronizer, IPOCRootTools rootTools,
                          IPOCContactModule contactModule, IPOCMainWindow pocMainWindow,
                          IPOCSettings settings, IMessagingClient msg,
                          IPopup popup, POCCore pocCore, IMembership membership)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            AContactModule = contactModule;
            APOCMainWindow = pocMainWindow;
            APOCSettings = settings;
            AMessageClient = msg;
            APopup = popup;
            APOCCore = pocCore;
            AMembership = membership;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameCalendar), Category.Debug, Priority.None);
            
            RegisterRootTool();

            

        }
        private void RegisterRootTool()
        {
            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameCalendar,
                    Image = HelperImage.GetStandardImage32("_32_Calendar.png"),
                    Order = ConstantPOCRootTool.OrderCalendar,
                    Permission = (int)PCOPermissions.Calendar_View,
                    Title = "سررسید",
                    Tooltip = "تقویم جامع برای مدیریت كارتابل و وظایف",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(Views.UCalendar), typeof(Models.MCalendar));
                        }),
                    InTamas = true,
                });
        }
        #endregion
    }
}
