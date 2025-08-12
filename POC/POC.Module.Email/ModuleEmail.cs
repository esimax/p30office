using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.Email.Models;
using POC.Module.Email.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;

namespace POC.Module.Email
{
    [Version]
    [Priority(ConstantPOCModules.OrderEmail)]
    [Module(ModuleName = ConstantPOCModules.NameEmail)]
    public class ModuleEmail : IModule
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
        private IPOCSettings APOCSettings { get; set; }
        private POCCore APOCCore { get; set; }
        private IPOCFastContactUnit APOCFastContactUnit { get; set; }

        public ModuleEmail(IUnityContainer unityContainer, ILoggerFacade logger, IModuleSyncronizer moduleSyncronizer,
            IPOCRootTools rootTools, IPOCContactModule contactModule,
            IBaseTable baseTable, IPOCMainWindow pocMainWindow, IPopup popup, IMessagingClient msg,
            IMembership membership, IPOCSettings settings, POCCore pocCore, IPOCFastContactUnit contactUnit)
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
            APOCSettings = settings;
            APOCCore = pocCore;
            APOCFastContactUnit = contactUnit;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameEmail), Category.Debug, Priority.None);

            RegisterBaseTable();
            RegisterRootTool();
            RegisterContactModule();
            RegisterFastContactUnit();

            #region ManageEmailTitle
            APOCMainWindow.RegisterManageEmailTitle(
                    owner =>
                    {
                        var w = new WEmailTitleManage(true)
                        {
                            Owner = owner ?? APOCMainWindow.GetWindow(),
                        };
                        return w.ShowDialog() == true ? w.SelectedData : null;
                    });
            #endregion

            #region EmailSync
            APOCMainWindow.RegisterEmailSync(
                   (owner, call) =>
                   {
                       var cc = call as DBEMEmailInbox;
                       if (cc == null) return null;
                       var w = new WEmailSync(cc)
                       {
                           Owner = owner ?? APOCMainWindow.GetWindow(),
                       };
                       return w.ShowDialog() != true ? null : w.DynamicDBEmail;
                   });
            #endregion

            #region EmailSend
            APOCMainWindow.RegisterEmailSend(
                  (owner, emailApp, defInbox, defEmails, defContact, defCat, defBasket) =>
                  {
                      var w = new WEmailSend(emailApp as DBEMEmailApp, defInbox as DBEMEmailInbox,
                          defEmails, defContact as DBCTContact,
                          defCat as DBCTContactCat, defBasket as DBCTContactSelection)
                      {
                          Owner = owner ?? APOCMainWindow.GetWindow(),
                      };
                      return w.ShowDialog();
                  });
            #endregion


            APOCSettings.RegisterUIElement("EmailPopup", new POCSettingItem
            {
                Order = 3,
                Permission = 0,
                Element = new USettingsEmailPopup(),
            });

            APOCMainWindow.RegisterSelectEmail(
               (owner, contactCat) =>
               {
                   var w = new WEmailSelect()
                   {
                       Owner = owner ?? APOCMainWindow.GetWindow(),
                   };
                   if (w.ShowDialog() != true) return null;
                   return w.SelectedEmail;
               });

            AMessageClient.RegisterHookForMessage(
                        m =>
                        {
                            if (APOCCore.EmailReceivePopupDurationIndex == 0) return; 

                            var szEmail = new XmlSerializer(typeof(EmailInfo));
                            var ei = (EmailInfo)szEmail.Deserialize(new StringReader(m.MessageData[0]));


                            bool allow = false;
                            if (ei.ViewPermissionType == 3)
                                allow = true;
                            else if (ei.ViewPermissionType == 2 && AMembership.ActiveUser.RolesOid.Contains(ei.ViewOid))
                                allow = true;
                            else if (ei.ViewPermissionType == 1 && AMembership.ActiveUser.UserID == ei.ViewOid)
                                allow = true;
                            else
                            {
                                if (AMembership.ActiveUser.UserName.ToLower() == "admin")
                                    allow = true;
                            }

                            if (allow)
                            {
                                var ui = new UPopupEmail(true, ei.From, ei.To, ei.Subject,
                                                         HelperImage.GetSpecialImage64("_64_Email.png"));

                                APopup.AddPopup(ui);
                            }
                        }, EnumMessageKind.EmailReceive);

            AMessageClient.RegisterHookForMessage(
                        m =>
                        {
                            if (APOCCore.EmailSendPopupDurationIndex == 0) return; 

                            var szEmail = new XmlSerializer(typeof(EmailInfo));
                            var ei = (EmailInfo)szEmail.Deserialize(new StringReader(m.MessageData[0]));

                            bool allow = false;
                            if (ei.ViewPermissionType == 3)
                                allow = true;
                            else if (ei.ViewPermissionType == 2 && AMembership.ActiveUser.RolesOid.Contains(ei.ViewOid))
                                allow = true;
                            else if (ei.ViewPermissionType == 1 && AMembership.ActiveUser.UserID == ei.ViewOid)
                                allow = true;
                            else
                            {
                                if (AMembership.ActiveUser.UserName.ToLower() == "admin")
                                    allow = true;
                            }

                            if (allow)
                            {
                                var ui = new UPopupEmail(false, ei.From, ei.To, ei.Subject,
                                                         HelperImage.GetSpecialImage64("_64_Email.png"));
                                APopup.AddPopup(ui);
                            }
                        }, EnumMessageKind.EmailSend);
        }
        #endregion

        private void RegisterBaseTable()
        {
            ABaseTable.RegisterBaseTable(new BaseTableItem
            {
                Key = ConstantPOCBaseTable.NameEmailTitle,
                Order = ConstantPOCBaseTable.OrderEmailTitle,
                Permission = 0,
                Image = HelperImage.GetStandardImage32("_32_Email.png"),
                Title = "عنوان ایمیل",
                Tooltip = "عناوین استفاده شده در ثبت و ویرایش ایمیل",
                Command = new RelayCommand(
                    () =>
                    {
                        var w = new WEmailTitleManage(false)
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
                    Key = ConstantPOCModules.NameEmail,
                    Image = HelperImage.GetStandardImage32("_32_Email.png"),
                    Order = ConstantPOCModuleContact.OrderEmail,
                    Permission = (int)PCOPermissions.Contact_Email_View,
                    Title = "ایمیل",
                    Tooltip = "ایمیل های ارسالی و دریافتی",
                    ViewType = typeof(UEmailContactModule),
                    ModelType = typeof(MEmailContactModule),
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
                    Key = ConstantPOCModules.NameEmail,
                    Image = HelperImage.GetStandardImage32("_32_Email.png"),
                    Order = ConstantPOCRootTool.OrderEmail,
                    Permission = (int)PCOPermissions.Email_View,
                    Title = "ایمیل",
                    Tooltip = "مدیریت ایمیلها",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(Views.UEmail), typeof(Models.MEmail));
                        }),
                    InTamas = false,
                });
        }
        private void RegisterFastContactUnit()
        {
            APOCFastContactUnit.Register(new FastContactUnitItem
            {
                Key = "Email",
                Order = 20,
                Permission = 0,
                ContentType = typeof(Views.FastContactUnit.UEmail)
            });

        }
    }
}
