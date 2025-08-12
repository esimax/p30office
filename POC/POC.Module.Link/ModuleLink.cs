using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.Link.Models;
using POC.Module.Link.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;

namespace POC.Module.Link
{
    [Version]
    [Priority(ConstantPOCModules.OrderLink)]
    [Module(ModuleName = ConstantPOCModules.NameLink)]
    public class ModuleLink : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IBaseTable ABaseTable { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        public ModuleLink(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer,
            IPOCRootTools rootTools, IPOCContactModule contactModule, IBaseTable baseTable, IPOCMainWindow pocMainWindow)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            AContactModule = contactModule;
            ABaseTable = baseTable;
            APOCMainWindow = pocMainWindow;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameLink), Category.Debug, Priority.None);

            RegisterContactModule();
            RegisterBaseTable();

            APOCMainWindow.RegisterManageRelMain(
                owner =>
                {
                    var w = new WLinkRelMainManager(true)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow()
                    };
                    return w.ShowDialog() == true ? w.SelectedData : null;
                });

            APOCMainWindow.RegisterManageRelSub(
                (owner,relMain) =>
                {
                    var o = relMain as POL.DB.P30Office.DBCTContactRelMain;
                    if (o == null) return null;
                    var w = new WLinkRelSubManager(true, o)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow()
                    };
                    return w.ShowDialog() == true ? w.SelectedData : null;
                });
        }
        #endregion



        private void RegisterContactModule()
        {
            AContactModule.RegisterContactModule(
                new POCContactModuleItem
                {
                    Key = ConstantPOCModules.NameLink,
                    Image = HelperImage.GetStandardImage32("_32_Hierarchy.png"),
                    Order = ConstantPOCModuleContact.OrderLink,
                    Permission = (int)PCOPermissions.Contact_Link_View,
                    Title = "ارتباطات",
                    Tooltip = "تعریف و مدیریت ارتباط بین پرونده ها",
                    ViewType = typeof(ULinkContactModule),
                    ModelType = typeof(MLinkContactModule),
                    Command = new RelayCommand<object>(
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
                    Key = ConstantPOCModules.NameLink,
                    Image = HelperImage.GetStandardImage32("_32_Hierarchy.png"),
                    Order = ConstantPOCRootTool.OrderLink,
                    Permission = -1,
                    Title = "ارتباطات",
                    Tooltip = "مدیریت ارتباطات بین پرونده ها",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                        }),
                    InTamas = false,
                });
        }
        private void RegisterBaseTable()
        {
            ABaseTable.RegisterBaseTable(new BaseTableItem
            {
                Key = ConstantPOCBaseTable.NameContactRelation,
                Order = ConstantPOCBaseTable.OrderContactRelation,
                Permission = 0,
                Image = HelperImage.GetStandardImage32("_32_Hierarchy.png"),
                Title = "نوع ارتباط بین پرونده",
                Tooltip = "عناوین استفاده شده در تعریف ارتباط بین پرونده ها",
                Command = new RelayCommand(
                    () =>
                    {
                        var w = new WLinkRelMainManager(false)
                        {
                            Owner = APOCMainWindow.GetWindow()
                        };
                        w.ShowDialog();
                    }, () => true),
                IsTamas = false,
            });
        }
    }
}
