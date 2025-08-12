using System;
using System.Windows;
using POL.Lib.Interfaces;
using System.Collections.Generic;

namespace POC.Shell.Adapters
{
    internal class AdapterPOCMainWindow : IPOCMainWindow
    {
        #region IPOCMainWindow

        #region Main Window
        private IMainWindow Window { get; set; }

        public void RegisterMainWindow(IMainWindow window)
        {
            if (Window == null && window != null)
                Window = window;
        }
        public void Hide()
        {
            if (Window == null) return;
            Window.HideWindow();
        }
        public void Show()
        {
            if (Window == null) return;
            Window.ShowWindow();
        }
        public void LoadContent(System.Type view, System.Type model)
        {
            Window.ShowBusyIndicator();
            Window.LoadContent(view, model);
        }
        public Window GetWindow()
        {
            return Window.GetWindow();
        }

        public void HideBusyIndicator()
        {
            Window.HideBusyIndicator();
        }

        public void ShowBusyIndicator()
        {
            Window.ShowBusyIndicator();
        }
        #endregion


        #region Address
        private Func<Window, object> ManageAddressTitleFunc { get; set; }
        public void RegisterManageAddressTitle(Func<Window, object> func)
        {
            ManageAddressTitleFunc = func;
        }
        public object ShowManageAddressTitle(Window owner)
        {
            return ManageAddressTitleFunc == null ? null : ManageAddressTitleFunc(owner);
        }
        #endregion

        #region City
        private Func<Window, object, bool, object> ManageCityFunc { get; set; }
        public void RegisterManageCity(Func<Window, object, bool, object> func)
        {
            ManageCityFunc = func;
        }
        public object ShowManageCity(Window owner, object country, bool allowPhoneCode)
        {
            return ManageCityFunc == null ? null : ManageCityFunc(owner, country, allowPhoneCode);
        }
        #endregion

        #region Phone
        private Func<Window, object> ManagePhoneTitleFunc { get; set; }
        public void RegisterManagePhoneTitle(Func<Window, object> func)
        {
            ManagePhoneTitleFunc = func;
        }
        public object ShowManagePhoneTitle(Window owner)
        {
            return ManagePhoneTitleFunc == null ? null : ManagePhoneTitleFunc(owner);
        }
        #endregion

        #region Map
        private Func<Window, MapLocationItem, MapLocationItem> SelectPointOnMapFunc { get; set; }
        public void RegisterSelectPointOnMap(Func<Window, MapLocationItem, MapLocationItem> func)
        {
            SelectPointOnMapFunc = func;
        }
        public MapLocationItem ShowSelectPointOnMap(Window owner, MapLocationItem inData)
        {
            return SelectPointOnMapFunc == null ? null : SelectPointOnMapFunc(owner, inData);
        }
        #endregion

        #region RelMain
        private Func<Window, object> ManageRelMainFunc { get; set; }
        public void RegisterManageRelMain(Func<Window, object> func)
        {
            ManageRelMainFunc = func;
        }
        public object ShowManageRelMain(Window owner)
        {
            return ManageRelMainFunc == null ? null : ManageRelMainFunc(owner);
        }
        #endregion

        #region RelSub
        private Func<Window, object, object> ManageRelSubFunc { get; set; }
        public void RegisterManageRelSub(Func<Window, object, object> func)
        {
            ManageRelSubFunc = func;
        }
        public object ShowManageRelSub(Window owner, object relMain)
        {
            return ManageRelSubFunc == null ? null : ManageRelSubFunc(owner, relMain);
        }
        #endregion

        #region ProfileTable
        private Func<Window, object> ManageProfileTableFunc { get; set; }
        public void RegisterManageProfileTable(Func<Window, object> func)
        {
            ManageProfileTableFunc = func;
        }
        public object ShowManageProfileTable(Window owner)
        {
            return ManageProfileTableFunc == null ? null : ManageProfileTableFunc(owner);
        }
        #endregion

        #region ProfileTValue
        private Func<Window, object, object> ManageProfileTValueFunc { get; set; }
        public void RegisterManageProfileTValue(Func<Window, object, object> func)
        {
            ManageProfileTValueFunc = func;
        }
        public object ShowManageProfileTValue(Window owner, object relMain)
        {
            return ManageProfileTValueFunc == null ? null : ManageProfileTValueFunc(owner, relMain);
        }
        #endregion

        #region SelectContact
        private Func<Window, object, object> SelectContactFunc { get; set; }
        public void RegisterSelectContact(Func<Window, object, object> func)
        {
            SelectContactFunc = func;
        }

        public object ShowSelectContact(Window owner, object contactCat)
        {
            return SelectContactFunc == null ? null : SelectContactFunc(owner, contactCat);
        }
        #endregion


        #region SelectContactCat
        private Func<Window, object> SelectContactCatFunc { get; set; }
        public void RegisterSelectContactCat(Func<Window, object> func)
        {
            SelectContactCatFunc = func;
        }

        public object ShowSelectContactCat(Window owner)
        {
            return SelectContactCatFunc == null ? null : SelectContactCatFunc(owner);
        }
        #endregion

        #region List
        private Func<Window, object> ManageListFunc { get; set; }
        public void RegisterManageList(Func<Window, object> func)
        {
            ManageListFunc = func;
        }
        public object ShowManageList(Window owner)
        {
            return ManageListFunc == null ? null : ManageListFunc(owner);
        }
        #endregion

        #region CallSync
        private Func<Window, object, object> CallSyncFunc { get; set; }
        public void RegisterCallSync(Func<Window, object, object> func)
        {
            CallSyncFunc = func;
        }

        public object ShowCallSync(Window owner, object call)
        {
            return CallSyncFunc == null ? null : CallSyncFunc(owner, call);
        }
        #endregion

        #region SetCallNote
        private Func<Window, object, bool?> SetCallNoteFunc { get; set; }
        public void RegisterSetCallNote(Func<Window, object, bool?> func)
        {
            SetCallNoteFunc = func;
        }

        public bool? ShowSetCallNote(Window owner, object dbcall)
        {
            return SetCallNoteFunc == null ? false : SetCallNoteFunc(owner, dbcall);
        }
        #endregion

        #region ManageEmailTitle
        private Func<Window, object> ManageEmailTitleFunc { get; set; }
        public void RegisterManageEmailTitle(Func<Window, object> func)
        {
            ManageEmailTitleFunc = func;
        }
        public object ShowManageEmailTitle(Window owner)
        {
            return ManageEmailTitleFunc == null ? null : ManageEmailTitleFunc(owner);
        }
        #endregion

        #region EmailSync
        private Func<Window, object, object> EmailSyncFunc { get; set; }
        public void RegisterEmailSync(Func<Window, object, object> func)
        {
            EmailSyncFunc = func;
        }

        public object ShowEmailSync(Window owner, object call)
        {
            return EmailSyncFunc == null ? null : EmailSyncFunc(owner, call);
        }
        #endregion

        #region EmailSend
        private Func<Window, object, object, string, object, object, object, object> EmailSendFunc { get; set; }
        public void RegisterEmailSend(Func<Window, object, object, string, object, object, object, object> func)
        {
            EmailSendFunc = func;
        }

        public object ShowEmailSend(Window owner, object emailApp, object defInbox, string defEmails, object defContact, object defCat, object defBasket)
        {
            return EmailSendFunc == null ? null : EmailSendFunc(owner, emailApp, defInbox, defEmails, defContact, defCat, defBasket);
        }
        #endregion

        #region SelectEmail
        private Func<Window, object, object> SelectEmailFunc { get; set; }
        public void RegisterSelectEmail(Func<Window, object, object> func)
        {
            SelectEmailFunc = func;
        }

        public object ShowSelectEmail(Window owner, object contactCat)
        {
            return SelectEmailFunc == null ? null : SelectEmailFunc(owner, contactCat);
        }
        #endregion

        #region ManageCategory
        private Func<Window, object> ManageCategoryFunc { get; set; }
        public void RegisterManageCategory(Func<Window, object> func)
        {
            ManageCategoryFunc = func;
        }

        public object ShowManageCategory(Window owner)
        {
            return ManageCategoryFunc == null ? null : ManageCategoryFunc(owner);
        }
        #endregion

        #region ShowSendSMS
        private Func<Window, EnumSelectionType, object, object, object, object, object, List<string>, string, object> ShowSendSMSFunc { get; set; }
        public void RegisterSendSMS(Func<Window, EnumSelectionType, object, object, object, object, object, List<string>, string, object> func)
        {
            ShowSendSMSFunc = func;
        }
        public object ShowSendSMS(Window owner, EnumSelectionType selectionType, object selectedPhone, object selectedContact, object selectedContactList, object selectedCategory, object selectedBasket, List<string> customeList, string text)
        {
            return ShowSendSMSFunc == null ? null : ShowSendSMSFunc(owner, selectionType, selectedPhone, selectedContact, selectedContactList, selectedCategory, selectedBasket, customeList, text);
        }
        #endregion

        #region ShowSelectProfile
        private Func<Window, EnumProfileItemType?, object> SelectProfileItemFunc { get; set; }
        public void RegisterSelectProfileItem(Func<Window, EnumProfileItemType?, object> func)
        {
            SelectProfileItemFunc = func;
        }
        public object ShowSelectProfileItem(Window owner, EnumProfileItemType? selectionType)
        {
            return SelectProfileItemFunc == null ? null : SelectProfileItemFunc(owner, selectionType);
        }
        #endregion

        #region SetSMSNote
        private Func<Window, object, bool?> SetSMSNoteFunc { get; set; }
        public void RegisterSetSMSNote(Func<Window, object, bool?> func)
        {
            SetSMSNoteFunc = func;
        }

        public bool? ShowSetSMSNote(Window owner, object dbcall)
        {
            return SetSMSNoteFunc == null ? false : SetSMSNoteFunc(owner, dbcall);
        }
        #endregion

        #region ContactSelectByResult
        private Func<Window, ContactSelectByResult> ContactSelectByResultFunc { get; set; }
        public void RegisterContactSelectByResult(Func<Window, ContactSelectByResult> func)
        {
            ContactSelectByResultFunc = func;
        }

        public ContactSelectByResult ShowContactSelectByResult(Window owner)
        {
            return ContactSelectByResultFunc == null ? null : ContactSelectByResultFunc(owner);
        }
        #endregion

        #region SelectUserRole
        private Func<Window, int, Guid, object> SelectUserRoleFunc { get; set; }
        public void RegisterSelectUserRole(Func<Window, int, Guid, object> func)
        {
            SelectUserRoleFunc = func;
        }

        public object ShowSelectUserRole(Window owner, int selectionType, Guid user_role_id)
        {
            return SelectUserRoleFunc == null ? null : SelectUserRoleFunc(owner, selectionType, user_role_id);
        }
        #endregion

        #region AddEditAutomation
        private Func<Window, Guid?, object> AddEditAutomationFunc { get; set; }
        public void RegisterAddEditAutomation(Func<Window, Guid?, object> func)
        {
            AddEditAutomationFunc = func;
        }
        public object ShowAddEditAutomation(Window owner, Guid? oid)
        {
            return AddEditAutomationFunc == null ? null : AddEditAutomationFunc(owner, oid);
        }
        #endregion

        #region EditCardTable
        private Func<Window, Guid, object> EditCardTableFunc { get; set; }
        public void RegisterEditCardTable(Func<Window, Guid, object> func)
        {
            EditCardTableFunc = func;
        }
        public object ShowEditCardTable(Window owner, Guid oid)
        {
            return EditCardTableFunc == null ? null : EditCardTableFunc(owner, oid);
        }
        #endregion

        #region AddCardTable
        private Func<Window, string, string, object, object, object, object, object, object> AddCardTableFunc { get; set; }
        public void RegisterAddCardTable(Func<Window, string, string, object, object, object, object, object, object> func)
        {
            AddCardTableFunc = func;
        }
        public object ShowAddCardTable(Window owner, string title, string note, object relCategory, object relContact, object relSMS, object relEmail, object relCall)
        {
            return AddCardTableFunc == null ? null : AddCardTableFunc(owner, title, note, relCategory, relContact, relSMS, relEmail, relCall);
        }
        #endregion

        #region AddToBasket
        private Func<Window, object, object, List<Guid>, object> AddToBasketFunc { get; set; }
        public void RegisterAddToBasket(Func<Window, object, object, List<Guid>, object> func)
        {
            AddToBasketFunc = func;
        }

        public object ShowAddToBasket(Window owner, object basket, object criteria, List<Guid> guids)
        {
            return AddToBasketFunc == null ? null : AddToBasketFunc(owner, basket, criteria, guids);
        }
        #endregion

        #region ShowContact
        private Func<Window, object, object> ShowContactFunc { get; set; }
        public void RegisterShowContact(Func<Window, object, object> func)
        {
            ShowContactFunc = func;
        }

        public object ShowShowContact(Window owner, object contact)
        {
            return ShowContactFunc == null ? null : ShowContactFunc(owner, contact);
        }
        #endregion

        #region ShowFastContact
        private Func<Window, object, object> ShowFastContactFunc { get; set; }
        public void RegisterShowFastContact(Func<Window, object, object> func)
        {
            ShowFastContactFunc = func;
        }

        public object ShowShowFastContact(Window owner, object contact)
        {
            return ShowFastContactFunc == null ? null : ShowFastContactFunc(owner, contact);
        }



        #endregion

        #region Factor
        private Func<Window, object> ManageFactorTitleFunc { get; set; }
        public void RegisterManageFactorTitle(Func<Window, object> func)
        {
            ManageFactorTitleFunc = func;
        }
        public object ShowManageFactorTitle(Window owner)
        {
            return ManageFactorTitleFunc == null ? null : ManageFactorTitleFunc(owner);
        }
        #endregion

        #endregion
    }
}
