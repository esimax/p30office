using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.Profile.DataField;
using POC.Module.Profile.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;

namespace POC.Module.Profile
{
    [Version]
    [Priority(ConstantPOCModules.OrderProfile)]
    [Module(ModuleName = ConstantPOCModules.NameProfile)]
    public class ModuleProfile : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IBaseTable ABaseTable { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IDataFieldManager ADataFieldManager { get; set; }
        private IPOCFastContactUnit APOCFastContactUnit { get; set; }

        public ModuleProfile(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer, IPOCRootTools rootTools, IPOCContactModule contactModule,
            IBaseTable baseTable, IPOCMainWindow pocMainWindow, IDataFieldManager datafield, IPOCFastContactUnit contactUnit)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            AContactModule = contactModule;

            ABaseTable = baseTable;
            APOCMainWindow = pocMainWindow;

            ADataFieldManager = datafield;
            APOCFastContactUnit = contactUnit;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameProfile), Category.Debug, Priority.None);

            RegisterRootTool();
            RegisterContactModule();
            RegisterBaseTable();
            RegisterPOCMainWindow();
            RegisterDataFields();
            RegisterFastContactUnit();
        }
        #endregion

        private void RegisterContactModule()
        {
            AContactModule.RegisterContactModule(
                new POCContactModuleItem
                {
                    Key = ConstantPOCModules.NameProfile,
                    Image = HelperImage.GetStandardImage32("_32_Profile.png"),
                    Order = ConstantPOCModuleContact.OrderProfile,
                    Permission = (int)PCOPermissions.Contact_Profile_View,
                    Title = "فرم ها",
                    Tooltip = "مدیریت اطلاعات فرم",
                    ModelType = typeof(Models.MProfileContactModule),
                    ViewType = typeof(Views.UProfileContactModule),
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
                    Key = ConstantPOCModules.NameProfile,
                    Image = HelperImage.GetStandardImage32("_32_Profile.png"),
                    Order = ConstantPOCRootTool.OrderProfile,
                    Permission = (int)PCOPermissions.Profile_View,
                    Title = "مدیریت فرم ها",
                    Tooltip = "مدیریت اطلاعات فرم ها",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(Views.UProfile), typeof(Models.MProfile));
                        }),
                    InTamas = true,
                });


            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameProfileReport,
                    Image = HelperImage.GetStandardImage32("_32_Profile.png"),
                    Order = ConstantPOCRootTool.OrderProfileReport,
                    Permission = (int)PCOPermissions.ProfileReport_View,
                    Title = "گزارش فرم ها",
                    Tooltip = "گزارش ساز برای اطلاعات فرم ها",
                    Command = new RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(Views.UProfileReport), typeof(Models.MProfileReport));
                        }),
                    InTamas = true,
                });

            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameProfileList,
                    Image = HelperImage.GetStandardImage32("_32_List2.png"),
                    Order = ConstantPOCRootTool.OrderProfileList,
                    Permission = (int)PCOPermissions.ProfileList_View,
                    Title = "گزارش لیست ها",
                    Tooltip = "گزارش برای اطلاعات لیست ها",
                    Command = new RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(Views.UProfileList), typeof(Models.MProfileList));
                        }),
                    InTamas = false,
                });
        }
        private void RegisterBaseTable()
        {
            ABaseTable.RegisterBaseTable(new BaseTableItem
            {
                Key = ConstantPOCBaseTable.NameProfileTable,
                Order = ConstantPOCBaseTable.OrderProfileTable,
                Permission = 0,
                Image = HelperImage.GetStandardImage32("_32_List.png"),
                Title = "جداول ساده (فرم)",
                Tooltip = "تعریف و مدیریت جداول ساده استفاده شده در فرم ها",
                Command = new RelayCommand(
                    () =>
                    {
                        var w = new WProfileTableManager(false)
                        {
                            Owner = APOCMainWindow.GetWindow()
                        };
                        w.ShowDialog();
                    }, () => true),
                IsTamas = true,
            });

            ABaseTable.RegisterBaseTable(new BaseTableItem
            {
                Key = ConstantPOCBaseTable.NameList,
                Order = ConstantPOCBaseTable.OrderList,
                Permission = 0,
                Image = HelperImage.GetStandardImage32("_32_List.png"),
                Title = "لیست ها",
                Tooltip = "تعریف و مدیریت لیست ها",
                Command = new RelayCommand(
                    () =>
                    {
                        var w = new WListManager(false)
                        {
                            Owner = APOCMainWindow.GetWindow(),
                        };
                        w.ShowDialog();
                    }, () => true),
                IsTamas = false,
            });
        }
        private void RegisterPOCMainWindow()
        {
            APOCMainWindow.RegisterManageProfileTable(
               owner =>
               {
                   var w = new WProfileTableManager(true)
                   {
                       Owner = owner ?? APOCMainWindow.GetWindow()
                   };
                   return w.ShowDialog() == true ? w.SelectedData : null;
               });

            APOCMainWindow.RegisterManageProfileTValue(
                (owner, table) =>
                {
                    var o = table as POL.DB.P30Office.DBCTProfileTable;
                    if (o == null) return null;
                    var w = new WProfileTValueManager(false, o)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow()
                    };
                    return w.ShowDialog() == true ? w.SelectedData : null;
                });
            APOCMainWindow.RegisterManageList(
                owner =>
                {
                    var w = new WListManager(true)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow()
                    };
                    return w.ShowDialog() == true ? w.SelectedData : null;
                });
            APOCMainWindow.RegisterSelectProfileItem(
                (owner,type) =>
                {
                    var w = new WProfileSelect(type)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow()
                    };
                    return w.ShowDialog() == true ? w.DynamicSelectedData.Tag : null;
                });
        }
        private void RegisterDataFields()
        {
            ADataFieldManager.Register(new BoolDataField());
            ADataFieldManager.Register(new DoubleDataField());
            ADataFieldManager.Register(new StringDataField());
            ADataFieldManager.Register(new CountryDataField());
            ADataFieldManager.Register(new CityDataField());
            ADataFieldManager.Register(new LocationDataField());

            ADataFieldManager.Register(new MemoDataField());
            ADataFieldManager.Register(new StringComboDataField());
            ADataFieldManager.Register(new StringCheckListDataField());
            ADataFieldManager.Register(new ColorDataField());
            ADataFieldManager.Register(new FileDataField());
            ADataFieldManager.Register(new ImageDataField());
            ADataFieldManager.Register(new DateDataField());
            ADataFieldManager.Register(new TimeDataField());
            ADataFieldManager.Register(new DateTimeDataField());
            ADataFieldManager.Register(new ContactDataField());
            ADataFieldManager.Register(new ListDataField());
        }
        private void RegisterFastContactUnit()
        {
            APOCFastContactUnit.Register(new FastContactUnitItem
            {
                Key = "Profile",
                Order = 40,
                Permission = 0,
                ContentType = typeof(Views.FastContactUnit.UProfile)
            });

        }
    }
}
