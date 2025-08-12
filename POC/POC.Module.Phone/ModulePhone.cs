using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.Phone.Models;
using POC.Module.Phone.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;

namespace POC.Module.Phone
{
    [Version]
    [Priority(ConstantPOCModules.OrderPhone)]
    [Module(ModuleName = ConstantPOCModules.NamePhone)]
    public class ModulePhone : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IBaseTable ABaseTable { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IPOCFastContactUnit APOCFastContactUnit { get; set; }

        public ModulePhone(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer,
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
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NamePhone), Category.Debug, Priority.None);

            RegisterRootTool();
            RegisterContactModule();
            RegisterBaseTable();
            RegisterFastContactUnit();

            APOCMainWindow.RegisterManagePhoneTitle(
                owner =>
                {
                    var w = new Views.WPhoneTitleManage(true)
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
                Key = ConstantPOCBaseTable.NamePhoneTitle,
                Order = ConstantPOCBaseTable.OrderPhoneTitle,
                Permission = 0,
                Image = HelperImage.GetStandardImage32("_32_List.png"),
                Title = "عنوان شماره تماس",
                Tooltip = "عناوین استفاده شده در ثبت و ویرایش شماره تماس",
                Command = new RelayCommand(
                    () =>
                    {
                        var w = new WPhoneTitleManage(false)
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
                    Key = ConstantPOCModules.NamePhone,
                    Image = HelperImage.GetStandardImage32("_32_Phone.png"),
                    Order = ConstantPOCModuleContact.OrderPhone,
                    Permission = (int)PCOPermissions.Contact_Phones_View,
                    Title = "شماره",
                    Tooltip = "تعریف، ویرایش و حذف شماره های تماس",
                    ViewType = typeof(UPhoneContactModule),
                    ModelType = typeof(MPhoneContactModule),
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
                    Key = ConstantPOCModules.NamePhone,
                    Image = HelperImage.GetStandardImage32("_32_Phone.png"),
                    Order = ConstantPOCRootTool.OrderPhone,
                    Permission = (int)PCOPermissions.Phones_View,
                    Title = "شماره",
                    Tooltip = "مدیریت شماره های تلفن، فكس و موبایل",
                    Command = new RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(UPhone), typeof(MPhone));
                        }),
                    InTamas = true,
                });
        }

        private void RegisterFastContactUnit()
        {
            APOCFastContactUnit.Register(new FastContactUnitItem
                                         {
                                             Key = "Phone",
                                             Order = 10,
                                             Permission = 0,
                                             ContentType = typeof(Views.FastContactUnit.UPhone)
                                         });
                
        }
    }
}
