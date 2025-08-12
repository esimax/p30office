using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.SMS.Models;
using POC.Module.SMS.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;

namespace POC.Module.SMS
{
    [Version]
    [Priority(ConstantPOCModules.OrderSMS)]
    [Module(ModuleName = ConstantPOCModules.NameSMS)]
    public class ModuleSMS : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IModuleSyncronizer ModuleSyncronizer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IPOCSettings APOCSettings { get; set; }
        private IMessagingClient AMessageClient { get; set; }
        private IPopup APopup { get; set; }
        private POCCore APOCCore { get; set; }
        private IMembership AMembership { get; set; }

        public ModuleSMS(IUnityContainer unityContainer, ILoggerFacade logger,
            IModuleSyncronizer moduleSyncronizer, IPOCRootTools rootTools,
            IPOCContactModule contactModule, IPOCMainWindow pocMainWindow,
            IPOCSettings settings, IMessagingClient msg, IPopup popup, POCCore pocCore,
            IMembership membership)
        {
            UnityContainer = unityContainer;
            ModuleSyncronizer = moduleSyncronizer;
            Logger = logger;
            ARootTools = rootTools;
            AContactModule = contactModule;
            APOCMainWindow = pocMainWindow;
            APOCSettings = settings;
            AMessageClient = msg;
            APopup = popup;
            APOCCore = pocCore;
            AMembership = membership;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameSMS), Category.Debug, Priority.None);



            APOCMainWindow.RegisterSendSMS(
                (owner, selectionType, selectedPhone, selectedContact, selectedContactList, selectedCategory, selectedBasket, customeList, text) =>
                {
                    var phone = selectedPhone as DBCTPhoneBook;
                    var contact = selectedContact as DBCTContact;
                    var contactList = selectedContactList as List<DBCTContact>;
                    var category = selectedCategory as DBCTContactCat;
                    var basket = selectedBasket as DBCTContactSelection;

                    var w = new Views.WSMSSend(selectionType, contact, contactList, category, basket, customeList, text)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() != true ? null : w.DialogResult;
                });

            APOCSettings.RegisterUIElement("SMSPopup", new POCSettingItem
            {
                Order = 3,
                Permission = 0,
                Element = new USettingsSMSPopup(),
            });



            AMessageClient.RegisterHookForMessage(
                        m =>
                        {
                            try
                            {
                                if (!AMembership.IsAuthorized) return;
                                if (APOCCore.SMSReceivePopupDurationIndex == 0)
                                    return; 
                                var szSMS = new XmlSerializer(typeof (SMSInfo));
                                var ei = (SMSInfo) szSMS.Deserialize(new StringReader(m.MessageData[0]));
                                    var ui = new UPopupSMS(true, ei, HelperImage.GetSpecialImage64("_64_SMS.png"),
                                        m.MessageKind);
                                    APopup.AddPopup(ui);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log("61A4EF7E-91C5-4AFA-9909-6573805C1863", Category.Exception, Priority.High);
                                Logger.Log(ex.Message, Category.Exception, Priority.High);
                            }
                        }, EnumMessageKind.SMSReceive);

            AMessageClient.RegisterHookForMessage(SendPopup, EnumMessageKind.SMSSendSuccess);
            AMessageClient.RegisterHookForMessage(SendPopup, EnumMessageKind.SMSWaitForDelivery);
            AMessageClient.RegisterHookForMessage(SendPopup, EnumMessageKind.SMSForwardOnCredit);
            AMessageClient.RegisterHookForMessage(SendPopup, EnumMessageKind.SMSForwardOnFailed);
            AMessageClient.RegisterHookForMessage(SendPopup, EnumMessageKind.SMSSendFailed);
            AMessageClient.RegisterHookForMessage(SendPopup, EnumMessageKind.SMSDeliveryResult);
            
            RegisterRootTool();
            RegisterContactModule();
        }

        private void SendPopup(MessagingItem m)
        {
            try
            {
                if (!AMembership.IsAuthorized) return;
                if (APOCCore.SMSSendPopupDurationIndex == 0)
                    return; 

                var szSMS = new XmlSerializer(typeof(SMSInfo));
                var ei = (SMSInfo)szSMS.Deserialize(new StringReader(m.MessageData[0]));
                if (ei.Sender == AMembership.ActiveUser.Title)
                {
                    var ui = new UPopupSMS(false, ei, HelperImage.GetSpecialImage64("_64_SMS.png"),m.MessageKind);
                    APopup.AddPopup(ui);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("4D21FD53-1F1C-4A84-8D7C-D7CF641EA4CF : " + m.MessageKind, Category.Exception, Priority.High);
                Logger.Log(ex.Message, Category.Exception, Priority.High);
            }
        }

        private void RegisterContactModule()
        {
            AContactModule.RegisterContactModule(
                item: new POCContactModuleItem
                {
                    Key = ConstantPOCModules.NameSMS,
                    Image = HelperImage.GetStandardImage32("_32_SMS.png"),
                    Order = ConstantPOCModuleContact.OrderSMS,
                    Permission = 0,
                    Title = "پیامك",
                    Tooltip = "ارسال و دریافت پیامك",
                    ViewType = typeof(USMSContactModule),
                    ModelType = typeof(MSMSContactModule),
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
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
                    Key = ConstantPOCModules.NameSMS,
                    Image = HelperImage.GetStandardImage32("_32_SMS.png"),
                    Order = ConstantPOCRootTool.OrderSMS,
                    Permission = (int)PCOPermissions.SMS_View,
                    Title = "پیامك",
                    Tooltip = "مدیریت پیامك ها",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(Views.USMS), typeof(Models.MSMS));
                        }),
                    InTamas = false,
                });
        }
        #endregion
    }
}
