using System;
using DevExpress.Data.Filtering;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POC.Module.Contact.Views;
using POL.DB.P30Office;

namespace POC.Module.Contact
{
    [Version]
    [Priority(ConstantPOCModules.OrderContact)]
    [Module(ModuleName = ConstantPOCModules.NameContact)]
    public class ModuleContact : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IBaseTable ABaseTable { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private IPOCDashboardUnit APOCDashboardUnit { get; set; }

        public ModuleContact(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer,
            IPOCRootTools rootTools, IBaseTable baseTable, IPOCMainWindow pocMainWindow,
            IPOCContactModule pocContactModule, IPOCDashboardUnit dashboard
            )
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            ABaseTable = baseTable;
            APOCMainWindow = pocMainWindow;
            APOCContactModule = pocContactModule;
            APOCDashboardUnit = dashboard;
        }



        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameContact), Category.Debug, Priority.None);
            RegisterRootTool();
            RegisterBaseTable();
            RegisterDashboardUnits();

            APOCMainWindow.RegisterSelectContact(
                (owner, contactCat) =>
                {
                    var cc = contactCat as DBCTContactCat;
                    var w = new WContactSelect(cc)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() != true ? null : w.SelectedContact;
                });
            APOCMainWindow.RegisterSelectContactCat(
                (owner) =>
                {
                    var w = new WContactCatSelect(true)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };

                    return w.ShowDialog() != true ? null : w.SelectedContactCat;
                });
            APOCMainWindow.RegisterContactSelectByResult(
                owner =>
                {
                    var w = new WContactSelectBy
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() != true ? null : w.DynamicResult;
                });
            APOCMainWindow.RegisterManageCategory(
                owner =>
                {
                    var w = new WContactCatManage(false)
                    {
                        Owner = APOCMainWindow.GetWindow()
                    };
                    return w.ShowDialog() != true ? null : w.SelectedData;
                });

            APOCMainWindow.RegisterAddToBasket(
                (owner, surceBasket, criteria, oids) =>
                {
                    var w = new Views.WContactToBasket(surceBasket as DBCTContactSelection, criteria as CriteriaOperator, oids)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };

                    w.ShowDialog();
                    return null;
                });
            APOCMainWindow.RegisterShowContact(
                (owner, contact) =>
                {
                    if ((contact as DBCTContact) == null)
                        return null;
                    var w = new WContactInWindow(contact as DBCTContact)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog();
                });

            APOCMainWindow.RegisterShowFastContact(
                (owner, contact) =>
                {
                    var cc = contact as DBCTContact;
                    var w = new WContactFastCreation(cc)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() != true ? null : w.SelectedContact;
                });
        }
        #endregion

        private void RegisterBaseTable()
        {
            ABaseTable.RegisterBaseTable(new BaseTableItem
            {
                Key = ConstantPOCBaseTable.NameContactCat,
                Order = ConstantPOCBaseTable.OrderContactCat,
                Permission = 0,
                Image = HelperImage.GetStandardImage32("_32_Contact.png"),
                Title = "دسته بندی پرونده ها",
                Tooltip = "عناوین استفاده شده در دسته بندی پرونده ها",
                Command = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            var w = new WContactCatManage(false)
                            {
                                Owner = APOCMainWindow.GetWindow()
                            };
                            w.ShowDialog();
                        }
                        catch (Exception ex)
                        {
                        }
                    }, () => true),
                IsTamas = true,
            });
        }
        private void RegisterRootTool()
        {
            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameContact,
                    Image = HelperImage.GetStandardImage32("_32_Contact.png"),
                    Order = ConstantPOCRootTool.OrderContact,
                    Permission = (int)PCOPermissions.Contact_Contact_View,
                    Title = "پرونده",
                    Tooltip = "مدیریت پرونده ها",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(Views.UContact), typeof(Models.MContact));
                        }),
                    InTamas = true,
                });


            APOCContactModule.HookGotoContactByCode(
                code =>
                {
                    var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                    v.Show();
                    v.LoadContent(typeof(Views.UContact), typeof(Models.MContact));
                }
                , 20);
        }
        private void RegisterDashboardUnits()
        {
            APOCDashboardUnit.Register(new DashboardUnitItem
            {
                Key = "Contact-TimeChart",
                ContentType = typeof(Views.DashboardUnits.UTimeChart),
                Order = 10,
                Permission = 0,
                TabName = ConstantDashboardTabs.NameContact,
            });

            APOCDashboardUnit.Register(new DashboardUnitItem
            {
                Key = "Contact-UserChart",
                ContentType = typeof(Views.DashboardUnits.UUserChart),
                Order = 20,
                Permission = 0,
                TabName = ConstantDashboardTabs.NameContact,
            });
        }



    }
}
