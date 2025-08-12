using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.Attachment.Models;
using POC.Module.Attachment.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;

namespace POC.Module.Attachment
{
    [Version]
    [Priority(ConstantPOCModules.OrderFactor)]
    [Module(ModuleName = ConstantPOCModules.NameFactor)]
    public class ModuleAttachment : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IBaseTable ABaseTable { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        public ModuleAttachment(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer, IPOCRootTools rootTools,
            IBaseTable baseTable, IPOCContactModule contactModule, IPOCMainWindow mainWindow)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            AContactModule = contactModule;
            ABaseTable = baseTable;
            APOCMainWindow = mainWindow;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameFactor), Category.Debug, Priority.None);

            RegisterRootTool();
            RegisterBaseTable();
            RegisterContactModule();


            APOCMainWindow.RegisterManageFactorTitle(
                owner =>
                {
                    var w = new Views.WFactorTitleManage(true)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() == true ? w.SelectedData : null;
                });

            ABaseTable.RegisterBaseTable(new BaseTableItem
            {
                Key = ConstantPOCBaseTable.NameFactorTitle,
                Order = ConstantPOCBaseTable.OrderFactorTitle,
                Permission = 0,
                Image = HelperImage.GetStandardImage32("_32_List.png"),
                Title = "عنوان فاکتور",
                Tooltip = "عناوین استفاده شده در ثبت و ویرایش فاکتور",
                Command = new RelayCommand(
                    () =>
                    {
                        var w = new WFactorTitleManage(false)
                        {
                            Owner = APOCMainWindow.GetWindow()
                        };
                        w.ShowDialog();
                    }, () => true),
            });

            ABaseTable.RegisterBaseTable(new BaseTableItem
            {
                Key = ConstantPOCBaseTable.NameFactorReportTemplate,
                Order = ConstantPOCBaseTable.OrderFactorReportTemplate,
                Permission = 0,
                Image = HelperImage.GetStandardImage32("_32_List.png"),
                Title = "قالب فاکتور",
                Tooltip = "قالب فاکتور با فرمت HTML",
                Command = new RelayCommand(
                    () =>
                    {
                        var w = new WFactorReportTemplateManage(false)
                        {
                            Owner = APOCMainWindow.GetWindow()
                        };
                        w.ShowDialog();
                    }, () => true),
            });
        }

        #endregion

        private void RegisterRootTool()
        {

            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameFactor,
                    Image = HelperImage.GetStandardImage32("_32_Invoice.png"),
                    Order = ConstantPOCRootTool.OrderFactor,
                    Permission = (int)PCOPermissions.Factor_View,
                    Title = "فاكتور و پیش فاكتور",
                    Tooltip = "مشاهده و مدیریت تمام فاكتور ها در یك نگاه",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(POC.Module.Attachment.Views.UFactor),
                                typeof(POC.Module.Attachment.Models.MFactor));
                        }),
                    InTamas = false,
                });
        }

        private void RegisterBaseTable()
        {
            ABaseTable.RegisterBaseTable(new BaseTableItem
            {
                Key = ConstantPOCBaseTable.NameUnit,
                Order = ConstantPOCBaseTable.OrderUnit,
                Permission = 0,
                Image = HelperImage.GetStandardImage32("_32_List.png"),
                Title = "واحد",
                Tooltip = "واحد استفاده شده در تعریف كالا",
                Command = new RelayCommand(
                    () =>
                    {
                        var w = new WUnitManage(false)
                        {
                            Owner = APOCMainWindow.GetWindow()
                        };
                        w.ShowDialog();
                    }, () => true),
                IsTamas = false,
            });

            ABaseTable.RegisterBaseTable(new BaseTableItem
            {
                Key = ConstantPOCBaseTable.NameProduct,
                Order = ConstantPOCBaseTable.OrderProduct,
                Permission = (int)PCOPermissions.Product_View,
                Image = HelperImage.GetStandardImage32("_32_List.png"),
                Title = "كالا / محصول",
                Tooltip = "تعریف انواع كالا و محصولات",
                Command = new RelayCommand(
                    () =>
                    {
                        var w = new WProductManage(false)
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
                    Key = ConstantPOCModules.NameFactor,
                    Image = HelperImage.GetStandardImage32("_32_Invoice.png"),
                    Order = ConstantPOCModuleContact.OrderAddress,
                    Permission = (int)PCOPermissions.Contact_Factor_View,
                    Title = "فاكتور",
                    Tooltip = "مدیریت فاكتور و پیش فاكتور های پرونده",
                    ViewType = typeof(UFactorContactModule),
                    ModelType = typeof(MFactorContactModule),
                    Command = new RelayCommand<object>(
                        o =>
                        {
                        }),
                    InTamas = false,
                });
        }
    }
}
