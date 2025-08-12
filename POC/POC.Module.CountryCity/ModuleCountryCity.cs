using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.CountryCity.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;

namespace POC.Module.CountryCity
{
    [Version]
    [Priority(ConstantPOCModules.OrderCountryCity)]
    [Module(ModuleName = ConstantPOCModules.NameCountryCity)]
    public class ModuleCountryCity : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        public ModuleCountryCity(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer, IPOCRootTools rootTools, IPOCContactModule contactModule, IPOCMainWindow pocMainWindow)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            AContactModule = contactModule;
            APOCMainWindow = pocMainWindow;
        }
        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameCountryCity), Category.Debug, Priority.None);
            RegisterRootTool();

            APOCMainWindow.RegisterManageCity(
                (owner, country, allowPhoneCode) =>
                {
                    var db = country as POL.DB.P30Office.GL.DBGLCountry;
                    if (db == null) return null;
                    var w = new WCountryCity(db, allowPhoneCode)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() == true ? w.SelectedData : null;
                });
        }
        #endregion

        private void RegisterRootTool()
        {
            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameCountryCity,
                    Image = HelperImage.GetStandardImage32("_32_Earth.png"),
                    Order = ConstantPOCRootTool.OrderCountryCity,
                    Permission = (int)PCOPermissions.CountryCity_View,
                    Title = "كشور / شهر",
                    Tooltip = "مدیریت اطلاعات كشور، استان و شهرها",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(Views.UCountryCity), typeof(Models.MCountryCity));
                        }),
                    InTamas = true,
                });
        }
    }
}
