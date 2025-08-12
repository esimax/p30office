using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POC.Module.Map.Views;

namespace POC.Module.Map
{
    [Version]
    [Priority(ConstantPOCModules.OrderMap)]
    [Module(ModuleName = ConstantPOCModules.NameMap)]
    public class ModuleMap : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        public ModuleMap(IUnityContainer unityContainer, ILoggerFacade logger,  IPOCMainWindow pocMainWindow)
        {
            UnityContainer = unityContainer;
            Logger = logger;
            APOCMainWindow = pocMainWindow;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameMap), Category.Debug, Priority.None);

            APOCMainWindow.RegisterSelectPointOnMap(
                (owner, o) =>
                {
                    var w = new WSelectPointOnMap(o)
                        {
                            Owner = owner ?? APOCMainWindow.GetWindow()
                        };
                    return w.ShowDialog() == true ? w.ResultData : null;
                });
        }


        #endregion



    }
}
