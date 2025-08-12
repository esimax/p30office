using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.Automation.Models;
using POC.Module.Automation.Views;

using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;

namespace POC.Module.Automation
{
    [Version]
    [Priority(ConstantPOCModules.OrderAutomation)]
    [Module(ModuleName = ConstantPOCModules.NameAutomation)]
    public class ModuleAutomation : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IBaseTable ABaseTable { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IMessagingClient AMessageClient { get; set; }
        private IPopup APopup { get; set; }
        private IMembership AMembership { get; set; }

        public ModuleAutomation(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer,
            IPOCRootTools rootTools, IPOCContactModule contactModule, IBaseTable baseTable, IPOCMainWindow pocMainWindow,
            IMessagingClient msg, IPopup popup, IMembership membership)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            AContactModule = contactModule;
            ABaseTable = baseTable;
            APOCMainWindow = pocMainWindow;
            AMessageClient = msg;
            APopup = popup;
            AMembership = membership;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameAutomation), Category.Debug, Priority.None);

            RegisterRootTool();
            RegisterRootTool2();
            RegisterContactModule();

            #region EnumMessagKind.CallerID
            AMessageClient.RegisterHookForMessage(
                    m =>
                    {
                        try
                        {
                            var p1 = new XmlSerializer(typeof(AutomationPopupInfo));
                            var api = (AutomationPopupInfo)p1.Deserialize(new StringReader(m.MessageData[0]));

                            var hasrole = false;
                            var hasuser = false;
                            if (api.PopupType == 1)
                            {
                                hasrole = AMembership.ActiveUser.RolesOid.ToList().Contains(Guid.Parse(api.PopupData));
                            }
                            if (api.PopupType == 2)
                            {
                                hasuser = AMembership.ActiveUser.UserName == api.PopupData.ToString();
                            }
                            if (hasrole || hasuser || api.PopupType == 0 || AMembership.ActiveUser.UserName.ToLower() == "admin")
                            {
                                var ui = new UPopupAutomation(api);
                                APopup.AddPopup(ui);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(string.Format("Export as Pop file : {0}", ex.ToString()), Category.Exception, Priority.Low);
                        }
                    }, EnumMessageKind.TaskAlert);
            #endregion

            APOCMainWindow.RegisterSelectUserRole(
                (owner, type, user_role_id) =>
                {
                    var w = new Views.WSelectUserRole(type, user_role_id)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() == true ? w.SelectedData : null;
                });


            APOCMainWindow.RegisterEditCardTable(
                (owner, oid) =>
                {
                    var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                    var db = POL.DB.P30Office.DBTMCardTable2.FindByOid(aDatabase.Dxs, oid);
                    var w = new Views.WCardTableAddEdit(db)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() == true ? w.SelectedData : null;
                });
            APOCMainWindow.RegisterAddCardTable(
                (owner, title, note, category, contact, sms, email, call) =>
                {
                    var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                    var w = new Views.WCardTableAddEdit(title, note, category, contact, sms, email, call)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() == true ? w.SelectedData : null;
                });

            APOCMainWindow.RegisterAddEditAutomation(
               (owner, oid) =>
               {
                   POL.DB.P30Office.DBTMAutomation db = null;
                   if (oid.HasValue)
                   {
                       var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                       db = POL.DB.P30Office.DBTMAutomation.FindByOid(aDatabase.Dxs, oid.Value);
                   }
                   var w = new Views.WAutomationAddEdit(db)
               {
                   Owner = owner ?? APOCMainWindow.GetWindow(),
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
                    Key = ConstantPOCModules.NameAutomation,
                    Image = HelperImage.GetStandardImage32("_32_Automation.png"),
                    Order = ConstantPOCModuleContact.OrderAutomation,
                    Permission = (int)PCOPermissions.Contact_Automations_View,
                    Title = "كارتابل",
                    Tooltip = "مشاهده كارتابل های مربوط به پرونده",
                    ViewType = typeof(UCardTableContactModule),
                    ModelType = typeof(MCardTableContactModule),
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
                    Key = ConstantPOCModules.NameAutomation,
                    Image = HelperImage.GetStandardImage32("_32_Automation.png"),
                    Order = ConstantPOCRootTool.OrderAutomation,
                    Permission = (int)PCOPermissions.Automations_View,
                    Title = "اتوماسیون",
                    Tooltip = "مدیریت عملیات اتوماتیك",
                    Command = new RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(UAutomation), typeof(MAutomation));
                        }),
                    InTamas = false,
                });
        }
        private void RegisterRootTool2()
        {
            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameCardTable,
                    Image = HelperImage.GetStandardImage32("_32_CardTable.png"),
                    Order = ConstantPOCRootTool.OrderCardTable,
                    Permission = (int)PCOPermissions.CardTables_View,
                    Title = "كارتابل",
                    Tooltip = "كارتابل كاربر",
                    Command = new RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(UCardTable), typeof(MCardTable));
                        }),
                    InTamas = false,
                });
        }
    }
}
