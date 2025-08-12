using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.Address.Models;
using POC.Module.Address.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;

namespace POC.Module.Address
{
    [Version]
    [Priority(ConstantPOCModules.OrderAddress)]
    [Module(ModuleName = ConstantPOCModules.NameAddress)]
    public class ModuleAddress : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IBaseTable ABaseTable { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IPOCFastContactUnit APOCFastContactUnit { get; set; }

        public ModuleAddress(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer,
            IPOCRootTools rootTools, IPOCContactModule contactModule, IBaseTable baseTable, IPOCMainWindow pocMainWindow,
            IPOCFastContactUnit contactUnit)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            AContactModule = contactModule;
            ABaseTable = baseTable;
            APOCMainWindow = pocMainWindow;
            APOCFastContactUnit = contactUnit;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameAddress), Category.Debug, Priority.None);

            RegisterRootTool();
            RegisterContactModule();
            RegisterBaseTable();
            RegisterFastContactUnit();

            APOCMainWindow.RegisterManageAddressTitle(
                owner =>
                {
                    var w = new Views.WAddressTitleManage(true)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() == true ? w.SelectedData : null;
                });
        }
        #endregion

        private void RegisterBaseTable()
        {
            ABaseTable.RegisterBaseTable(new BaseTableItem
            {
                Key = ConstantPOCBaseTable.NameAddressTitle,
                Order = ConstantPOCBaseTable.OrderAddressTitle,
                Permission = 0,
                Image = HelperImage.GetStandardImage32("_32_List.png"),
                Title = "عنوان آدرس",
                Tooltip = "عناوین استفاده شده در ثبت و ویرایش آدرس",
                Command = new RelayCommand(
                    () =>
                    {
                        var w = new WAddressTitleManage(false)
                        {
                            Owner = APOCMainWindow.GetWindow()
                        };
                        w.ShowDialog();
                    }, () => true),
                    IsTamas = false,
            });
        }
        private void RegisterContactModule()
        {
            AContactModule.RegisterContactModule(
                new POCContactModuleItem
                {
                    Key = ConstantPOCModules.NameAddress,
                    Image = HelperImage.GetStandardImage32("_32_Address.png"),
                    Order = ConstantPOCModuleContact.OrderAddress,
                    Permission = (int)PCOPermissions.Contact_Address_View,
                    Title = "آدرس",
                    Tooltip = "تعریف آدرس و مشخص كردن آن روی نقشه",
                    ViewType = typeof(UAddressContactModule),
                    ModelType = typeof(MAddressContactModule),
                    Command = new RelayCommand<object>(
                        o =>
                        {
                        }),
                    InTamas = true,
                });
        }
        private void RegisterRootTool()
        {
            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameAddress,
                    Image = HelperImage.GetStandardImage32("_32_Address.png"),
                    Order = ConstantPOCRootTool.OrderAddress,
                    Permission = (int)PCOPermissions.Addresses_View,
                    Title = "آدرس",
                    Tooltip = "مدیریت آدرس ها",
                    Command = new RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(UAddress), typeof(MAddress));
                        }),
                    InTamas = true,
                });
        }
        private void RegisterFastContactUnit()
        {
            APOCFastContactUnit.Register(new FastContactUnitItem
            {
                Key = "Address",
                Order = 30,
                Permission = 0,
                ContentType = typeof(Views.FastContactUnit.UAddress)
            });

        }

    }

}
