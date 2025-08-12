using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using POC.Module.Call.Models;
using POC.Module.Call.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using System.Xml.Serialization;

namespace POC.Module.Call
{
    [Version]
    [Priority(ConstantPOCModules.OrderCall)]
    [Module(ModuleName = ConstantPOCModules.NameCall)]
    public class ModuleCall : IModule
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
        private IPOCDashboardUnit APOCDashboardUnit { get; set; }

        public ModuleCall(IUnityContainer unityContainer, ILoggerFacade logger,
                          IModuleSyncronizer moduleSyncronizer, IPOCRootTools rootTools,
                          IPOCContactModule contactModule, IPOCMainWindow pocMainWindow,
                          IPOCSettings settings, IMessagingClient msg,
                          IPopup popup, POCCore pocCore, IMembership membership,
                            IPOCDashboardUnit dashboard)
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
            APOCDashboardUnit = dashboard;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameCall), Category.Debug, Priority.None);

            RegisterRootTool();
            RegisterContactModule();
            RegisterDashboardUnits();

            APOCMainWindow.RegisterCallSync(
                (owner, call) =>
                {
                    var cc = call as DBCLCall;
                    if (cc == null) return null;
                    var w = new WCallSync(cc)
                    {
                        Owner = owner ?? APOCMainWindow.GetWindow(),
                    };
                    return w.ShowDialog() != true ? null : w.DynamicDBCall;
                });

            APOCMainWindow.RegisterSetCallNote(
                (owner, dbcall) =>
                {
                    if (dbcall == null) return false;
                    var call = dbcall as DBCLCall;
                    if (call == null) return false;
                    var w = new WCallNoteAddEdit(call)
                                {
                                    Owner = owner,
                                };
                    return w.ShowDialog();
                });

            APOCSettings.RegisterUIElement("CallPopup", new POCSettingItem
            {
                Order = 2,
                Permission = 0,
                Element = new USettingsCallPopup(),
            });

            #region EnumMessagKind.CallerID
            AMessageClient.RegisterHookForMessage(
                    m =>
                    {

                        if (!AMembership.HasPermission(PCOPermissions.Popup_Enabled)) return; 
                        if (APOCCore.CallPopupDurationIndex == 0) return; 
                        try
                        {

                            var szCall = new XmlSerializer(typeof(CallInfo));
                            var call = (CallInfo)szCall.Deserialize(new StringReader(m.MessageData[0]));

                            if (APOCCore.CallOnlyInternal)
                            { if (!APOCCore.CallInternalCode.Contains(call.LastExt.ToString())) return; }


                            if (call.Line == 1 && (!AMembership.HasPermission((int)PCOPermissions.Popup_Line1) || !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_1))) return;
                            if (call.Line == 2 && (!AMembership.HasPermission((int)PCOPermissions.Popup_Line2) || !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_2))) return;
                            if (call.Line == 3 && (!AMembership.HasPermission((int)PCOPermissions.Popup_Line3) || !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_3))) return;
                            if (call.Line == 4 && (!AMembership.HasPermission((int)PCOPermissions.Popup_Line4) || !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_4))) return;
                            if (call.Line == 5 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line5)) return;
                            if (call.Line == 6 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line6)) return;
                            if (call.Line == 7 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line7)) return;
                            if (call.Line == 8 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line8)) return;
                            if (call.Line == 9 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line9)) return;
                            if (call.Line == 10 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line10)) return;
                            if (call.Line == 11 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line11)) return;
                            if (call.Line == 12 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line12)) return;
                            if (call.Line == 13 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line13)) return;
                            if (call.Line == 14 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line14)) return;
                            if (call.Line == 15 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line15)) return;
                            if (call.Line == 16 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line16)) return;
                            if (call.Line == 17 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line17)) return;
                            if (call.Line == 18 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line18)) return;
                            if (call.Line == 19 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line19)) return;
                            if (call.Line == 20 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line20)) return;
                            if (call.Line == 21 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line21)) return;
                            if (call.Line == 22 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line22)) return;
                            if (call.Line == 23 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line23)) return;
                            if (call.Line == 24 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line24)) return;
                            if (call.Line == 25 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line25)) return;
                            if (call.Line == 26 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line26)) return;
                            if (call.Line == 27 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line27)) return;
                            if (call.Line == 28 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line28)) return;
                            if (call.Line == 29 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line29)) return;
                            if (call.Line == 30 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line30)) return;
                            if (call.Line == 31 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line31)) return;
                            if (call.Line == 32 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line32)) return;
                            if (call.Line > 32 && call.Line <= 64 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line33_64)) return;
                            if (call.Line > 64 && call.Line <= 128 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line65_128)) return;
                            if (call.Line > 128 && call.Line <= 256 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line128_256)) return;


                            var szContact = new XmlSerializer(typeof(ContactInfo));
                            var contact = (ContactInfo)szContact.Deserialize(new StringReader(m.MessageData[1]));


                            var ui = new UPopupCallerID(call, contact);


                            APopup.AddPopup(ui);


                            if (APOCCore.CallPopupExportAsPopFile)
                            {
                                try
                                {
                                    if (!Directory.Exists(ConstantGeneral.PathPopQueue))
                                        Directory.CreateDirectory(ConstantGeneral.PathPopQueue);
                                    using (var f = File.CreateText(string.Format("c:\\CoreOfficeQueue\\{0}.pop", DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture))))
                                    {
                                        f.WriteLine(call.CallDate);
                                        f.WriteLine(call.CityCode);
                                        f.WriteLine(call.CityTitle);
                                        f.WriteLine(call.CountryCode);
                                        f.WriteLine(call.CountryTitle);
                                        f.WriteLine(call.Device);
                                        f.WriteLine(call.Duration);
                                        f.WriteLine(call.Extra);
                                        f.WriteLine(call.IsCallIn);
                                        f.WriteLine(call.IsTrans);
                                        f.WriteLine(call.LastExt);
                                        f.WriteLine(call.LastExtDuration);
                                        f.WriteLine(call.Line);
                                        f.WriteLine(call.Oid);
                                        f.WriteLine(call.Phone);
                                        f.WriteLine(call.PhoneInternal);
                                        f.WriteLine(call.PhoneTitle);
                                        f.WriteLine(call.PrevExt);
                                        f.WriteLine(contact.Code);
                                        f.WriteLine(contact.Oid);
                                        f.WriteLine(contact.Title);
                                        f.WriteLine(contact.Cats);
                                    }

                                }
                                catch
                                {
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(string.Format("Export as Pop file : {0}", ex.ToString()), Category.Exception, Priority.Low);
                        }
                    }, EnumMessageKind.CallerID);
            #endregion

            #region EnumMessagKind.CallEnd
            AMessageClient.RegisterHookForMessage(
                    m =>
                    {
                        if (!AMembership.HasPermission(PCOPermissions.Popup_Enabled)) return; 
                        if (APOCCore.CallPopupDurationIndex == 0) return; 

                        if (!APOCCore.CallPopupExportAsPopFile) return;

                        try
                        {
                            var szCall = new XmlSerializer(typeof(CallInfo));
                            var call = (CallInfo)szCall.Deserialize(new StringReader(m.MessageData[0]));

                            if (call.Line == 1 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line1)) return;
                            if (call.Line == 2 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line2)) return;
                            if (call.Line == 3 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line3)) return;
                            if (call.Line == 4 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line4)) return;
                            if (call.Line == 5 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line5)) return;
                            if (call.Line == 6 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line6)) return;
                            if (call.Line == 7 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line7)) return;
                            if (call.Line == 8 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line8)) return;
                            if (call.Line == 9 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line9)) return;
                            if (call.Line == 10 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line10)) return;
                            if (call.Line == 11 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line11)) return;
                            if (call.Line == 12 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line12)) return;
                            if (call.Line == 13 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line13)) return;
                            if (call.Line == 14 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line14)) return;
                            if (call.Line == 15 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line15)) return;
                            if (call.Line == 16 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line16)) return;
                            if (call.Line == 17 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line17)) return;
                            if (call.Line == 18 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line18)) return;
                            if (call.Line == 19 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line19)) return;
                            if (call.Line == 20 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line20)) return;
                            if (call.Line == 21 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line21)) return;
                            if (call.Line == 22 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line22)) return;
                            if (call.Line == 23 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line23)) return;
                            if (call.Line == 24 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line24)) return;
                            if (call.Line == 25 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line25)) return;
                            if (call.Line == 26 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line26)) return;
                            if (call.Line == 27 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line27)) return;
                            if (call.Line == 28 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line28)) return;
                            if (call.Line == 29 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line29)) return;
                            if (call.Line == 30 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line30)) return;
                            if (call.Line == 31 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line31)) return;
                            if (call.Line == 32 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line32)) return;
                            if (call.Line > 32 && call.Line <= 64 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line33_64)) return;
                            if (call.Line > 64 && call.Line <= 128 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line65_128)) return;
                            if (call.Line > 128 && call.Line <= 256 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line128_256)) return;

                            var szContact = new XmlSerializer(typeof(ContactInfo));
                            var contact = (ContactInfo)szContact.Deserialize(new StringReader(m.MessageData[1]));

                            if (APOCCore.CallPopupExportAsPopFile)
                            {
                                try
                                {
                                    if (!Directory.Exists(ConstantGeneral.PathPopQueue))
                                        Directory.CreateDirectory(ConstantGeneral.PathPopQueue);
                                    using (var f = File.CreateText(string.Format("c:\\P30OfficeQueue\\{0}.popend", DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture))))
                                    {
                                        f.WriteLine(call.CallDate);
                                        f.WriteLine(call.CityCode);
                                        f.WriteLine(call.CityTitle);
                                        f.WriteLine(call.CountryCode);
                                        f.WriteLine(call.CountryTitle);
                                        f.WriteLine(call.Device);
                                        f.WriteLine(call.Duration);
                                        f.WriteLine(call.Extra);
                                        f.WriteLine(call.IsCallIn);
                                        f.WriteLine(call.IsTrans);
                                        f.WriteLine(call.LastExt);
                                        f.WriteLine(call.LastExtDuration);
                                        f.WriteLine(call.Line);
                                        f.WriteLine(call.Oid);
                                        f.WriteLine(call.Phone);
                                        f.WriteLine(call.PhoneInternal);
                                        f.WriteLine(call.PhoneTitle);
                                        f.WriteLine(call.PrevExt);
                                        f.WriteLine(contact.Code);
                                        f.WriteLine(contact.Oid);
                                        f.WriteLine(contact.Title);
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(string.Format("Export as Popend file : {0}", ex.ToString()), Category.Exception, Priority.Low);
                        }
                    }, EnumMessageKind.CallEnd);
            #endregion

            #region EnumMessagKind.CallTrans
            AMessageClient.RegisterHookForMessage(
                    m =>
                    {
                        if (APOCCore.CallOnlyInternal)
                        {
                            if (!AMembership.HasPermission(PCOPermissions.Popup_Enabled)) return; 
                            if (APOCCore.CallPopupDurationIndex == 0) return; 
                            try
                            {

                                var szCode = new XmlSerializer(typeof(string));
                                var code = (string)szCode.Deserialize(new StringReader(m.MessageData[1]));
                                var codes = APOCCore.CallInternalCode.Split(';', ',', '/').ToList();
                                if (codes.Contains(code))
                                {
                                    var szCall = new XmlSerializer(typeof(CallInfo));
                                    var call = (CallInfo)szCall.Deserialize(new StringReader(m.MessageData[0]));

                                    if (call.Line == 1 && (!AMembership.HasPermission((int)PCOPermissions.Popup_Line1) || !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_1))) return;
                                    if (call.Line == 2 && (!AMembership.HasPermission((int)PCOPermissions.Popup_Line2) || !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_2))) return;
                                    if (call.Line == 3 && (!AMembership.HasPermission((int)PCOPermissions.Popup_Line3) || !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_3))) return;
                                    if (call.Line == 4 && (!AMembership.HasPermission((int)PCOPermissions.Popup_Line4) || !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_4))) return;
                                    if (call.Line == 5 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line5)) return;
                                    if (call.Line == 6 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line6)) return;
                                    if (call.Line == 7 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line7)) return;
                                    if (call.Line == 8 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line8)) return;
                                    if (call.Line == 9 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line9)) return;
                                    if (call.Line == 10 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line10)) return;
                                    if (call.Line == 11 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line11)) return;
                                    if (call.Line == 12 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line12)) return;
                                    if (call.Line == 13 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line13)) return;
                                    if (call.Line == 14 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line14)) return;
                                    if (call.Line == 15 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line15)) return;
                                    if (call.Line == 16 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line16)) return;
                                    if (call.Line == 17 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line17)) return;
                                    if (call.Line == 18 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line18)) return;
                                    if (call.Line == 19 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line19)) return;
                                    if (call.Line == 20 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line20)) return;
                                    if (call.Line == 21 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line21)) return;
                                    if (call.Line == 22 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line22)) return;
                                    if (call.Line == 23 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line23)) return;
                                    if (call.Line == 24 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line24)) return;
                                    if (call.Line == 25 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line25)) return;
                                    if (call.Line == 26 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line26)) return;
                                    if (call.Line == 27 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line27)) return;
                                    if (call.Line == 28 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line28)) return;
                                    if (call.Line == 29 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line29)) return;
                                    if (call.Line == 30 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line30)) return;
                                    if (call.Line == 31 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line31)) return;
                                    if (call.Line == 32 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line32)) return;
                                    if (call.Line > 32 && call.Line <= 64 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line33_64)) return;
                                    if (call.Line > 64 && call.Line <= 128 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line65_128)) return;
                                    if (call.Line > 128 && call.Line <= 256 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line128_256)) return;


                                    var szContact = new XmlSerializer(typeof(ContactInfo));
                                    var contact = (ContactInfo)szContact.Deserialize(new StringReader(m.MessageData[2]));


                                    var ui = new UPopupCallerID(call, contact);


                                    APopup.AddPopup(ui);
                                }


                            }
                            catch
                            {
                            }
                        }
                        System.Diagnostics.Debugger.Log(0, "", "Trans");
                    }, EnumMessageKind.CallTrans);
            #endregion

            #region EnumMessagKind.CallTran
            AMessageClient.RegisterHookForMessage(
                    m =>
                    {
                        if (!AMembership.HasPermission(PCOPermissions.Popup_Enabled)) return; 
                        if (APOCCore.CallPopupDurationIndex == 0) return; 

                        if (!APOCCore.CallPopupExportAsPopFile) return;

                        try
                        {
                            var szCall = new XmlSerializer(typeof(CallInfo));
                            var call = (CallInfo)szCall.Deserialize(new StringReader(m.MessageData[0]));

                            if (call.Line == 1 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line1)) return;
                            if (call.Line == 2 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line2)) return;
                            if (call.Line == 3 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line3)) return;
                            if (call.Line == 4 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line4)) return;
                            if (call.Line == 5 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line5)) return;
                            if (call.Line == 6 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line6)) return;
                            if (call.Line == 7 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line7)) return;
                            if (call.Line == 8 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line8)) return;
                            if (call.Line == 9 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line9)) return;
                            if (call.Line == 10 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line10)) return;
                            if (call.Line == 11 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line11)) return;
                            if (call.Line == 12 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line12)) return;
                            if (call.Line == 13 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line13)) return;
                            if (call.Line == 14 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line14)) return;
                            if (call.Line == 15 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line15)) return;
                            if (call.Line == 16 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line16)) return;
                            if (call.Line == 17 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line17)) return;
                            if (call.Line == 18 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line18)) return;
                            if (call.Line == 19 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line19)) return;
                            if (call.Line == 20 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line20)) return;
                            if (call.Line == 21 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line21)) return;
                            if (call.Line == 22 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line22)) return;
                            if (call.Line == 23 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line23)) return;
                            if (call.Line == 24 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line24)) return;
                            if (call.Line == 25 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line25)) return;
                            if (call.Line == 26 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line26)) return;
                            if (call.Line == 27 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line27)) return;
                            if (call.Line == 28 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line28)) return;
                            if (call.Line == 29 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line29)) return;
                            if (call.Line == 30 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line30)) return;
                            if (call.Line == 31 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line31)) return;
                            if (call.Line == 32 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line32)) return;
                            if (call.Line > 32 && call.Line <= 64 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line33_64)) return;
                            if (call.Line > 64 && call.Line <= 128 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line65_128)) return;
                            if (call.Line > 128 && call.Line <= 256 && !AMembership.HasPermission((int)PCOPermissions.Popup_Line128_256)) return;


                            if (APOCCore.CallPopupExportAsPopFile)
                            {
                                try
                                {
                                    if (!Directory.Exists(ConstantGeneral.PathPopQueue))
                                        Directory.CreateDirectory(ConstantGeneral.PathPopQueue);
                                    using (var f = File.CreateText(string.Format("c:\\P30OfficeQueue\\{0}.poptans", DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture))))
                                    {
                                        f.WriteLine(call.CallDate);
                                        f.WriteLine(call.CityCode);
                                        f.WriteLine(call.CityTitle);
                                        f.WriteLine(call.CountryCode);
                                        f.WriteLine(call.CountryTitle);
                                        f.WriteLine(call.Device);
                                        f.WriteLine(call.Duration);
                                        f.WriteLine(call.Extra);
                                        f.WriteLine(call.IsCallIn);
                                        f.WriteLine(call.IsTrans);
                                        f.WriteLine(call.LastExt);
                                        f.WriteLine(call.LastExtDuration);
                                        f.WriteLine(call.Line);
                                        f.WriteLine(call.Oid);
                                        f.WriteLine(call.Phone);
                                        f.WriteLine(call.PhoneInternal);
                                        f.WriteLine(call.PhoneTitle);
                                        f.WriteLine(call.PrevExt);
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(string.Format("Export as Poptrans file : {0}", ex.ToString()), Category.Exception, Priority.Low);
                        }
                    }, EnumMessageKind.CallTrans);
            #endregion
        }
        #endregion

        private void RegisterContactModule()
        {
            AContactModule.RegisterContactModule(
                new POCContactModuleItem
                {
                    Key = ConstantPOCModules.NameCall,
                    Image = HelperImage.GetStandardImage32("_32_Call.png"),
                    Order = ConstantPOCModuleContact.OrderCall,
                    Permission = (int)PCOPermissions.Contact_Calls_View,
                    Title = "تماسها",
                    Tooltip = "تماسهای ارسالی و دریافتی",
                    ViewType = typeof(UCallContactModule),
                    ModelType = typeof(MCallContactModule),
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
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
                    Key = ConstantPOCModules.NameCall,
                    Image = HelperImage.GetStandardImage32("_32_Call.png"),
                    Order = ConstantPOCRootTool.OrderCall,
                    Permission = (int)PCOPermissions.Call_View,
                    Title = "تماسها",
                    Tooltip = "مدیریت تماسهای ارسالی و دریافتی",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(Views.UCall), typeof(Models.MCall));
                        }),
                    InTamas = true,
                });
        }
        private void RegisterDashboardUnits()
        {
            APOCDashboardUnit.Register(new DashboardUnitItem
            {
                Key = "Call-CategoryStat",
                ContentType = typeof(Views.DashboardUnits.UCategoryStat),
                Order = 10,
                Permission = 0,
                TabName = ConstantDashboardTabs.NameCall,
            });
            APOCDashboardUnit.Register(new DashboardUnitItem
            {
                Key = "Call-TimeChart",
                ContentType = typeof(Views.DashboardUnits.UTimeChart),
                Order = 20,
                Permission = 0,
                TabName = ConstantDashboardTabs.NameCall,
            });
            APOCDashboardUnit.Register(new DashboardUnitItem
            {
                Key = "Call-MultiChart",
                ContentType = typeof(Views.DashboardUnits.UMultiChart),
                Order = 30,
                Permission = 0,
                TabName = ConstantDashboardTabs.NameCall,
            });
        }
    }
}
