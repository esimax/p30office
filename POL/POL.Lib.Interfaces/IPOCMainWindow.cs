using System;
using System.Collections.Generic;
using System.Windows;

namespace POL.Lib.Interfaces
{
    public interface IPOCMainWindow
    {
        void RegisterSelectPointOnMap(Func<Window, MapLocationItem, MapLocationItem> func);
        MapLocationItem ShowSelectPointOnMap(Window owner, MapLocationItem inData);

        void RegisterManagePhoneTitle(Func<Window, object> func);

        object ShowManagePhoneTitle(Window owner);

        void RegisterManageAddressTitle(Func<Window, object> func);

        object ShowManageAddressTitle(Window owner);

        void RegisterManageCity(Func<Window, object, bool, object> func);

        object ShowManageCity(Window owner, object country, bool allowPhoneCode);

        void RegisterManageRelMain(Func<Window, object> func);

        object ShowManageRelMain(Window owner);

        void RegisterManageRelSub(Func<Window, object, object> func);

        object ShowManageRelSub(Window owner, object relMain);

        void RegisterManageProfileTable(Func<Window, object> func);

        object ShowManageProfileTable(Window owner);

        void RegisterManageProfileTValue(Func<Window, object, object> func);

        object ShowManageProfileTValue(Window owner, object profileTable);


        void RegisterSelectContact(Func<Window, object, object> func);

        object ShowSelectContact(Window owner, object contactCat);

        void RegisterSelectContactCat(Func<Window, object> func);

        object ShowSelectContactCat(Window owner);


        void RegisterManageList(Func<Window, object> func);

        object ShowManageList(Window owner);


        void RegisterCallSync(Func<Window, object, object> func);

        object ShowCallSync(Window owner, object call);


        void RegisterSetCallNote(Func<Window, object, bool?> func);

        bool? ShowSetCallNote(Window owner, object dbcall);


        void RegisterContactSelectByResult(Func<Window, ContactSelectByResult> func);

        ContactSelectByResult ShowContactSelectByResult(Window owner);


        void RegisterManageEmailTitle(Func<Window, object> func);

        object ShowManageEmailTitle(Window owner);

        void RegisterEmailSync(Func<Window, object, object> func);

        object ShowEmailSync(Window owner, object call);


        void RegisterEmailSend(Func<Window, object, object, string, object, object, object, object> func);

        object ShowEmailSend(Window owner, object emailApp, object defInbox, string defEmails, object defContact,
            object defCat, object defBasket);


        void RegisterSelectEmail(Func<Window, object, object> func);

        object ShowSelectEmail(Window owner, object contactCat);


        void RegisterManageCategory(Func<Window, object> func);

        object ShowManageCategory(Window owner);

        void RegisterSendSMS(
            Func<Window, EnumSelectionType, object, object, object, object, object, List<string>, string, object> func);

        object ShowSendSMS(Window owner, EnumSelectionType selectionType,
            object selectedPhone,
            object selectedContact,
            object selectedContactList,
            object selectedCategory,
            object selectedBasket,
            List<string> customeList,
            string text);


        void RegisterSelectProfileItem(Func<Window, EnumProfileItemType?, object> func);

        object ShowSelectProfileItem(Window owner, EnumProfileItemType? selectionType);


        void RegisterSelectUserRole(Func<Window, int, Guid, object> func);

        object ShowSelectUserRole(Window owner, int selectionType, Guid user_role_id);


        void RegisterEditCardTable(Func<Window, Guid, object> func);

        object ShowEditCardTable(Window owner, Guid oid);

        void RegisterAddCardTable(Func<Window, string, string, object, object, object, object, object, object> func);

        object ShowAddCardTable(Window owner, string title, string note, object category, object contact, object sms,
            object email, object call);

        void RegisterAddEditAutomation(Func<Window, Guid?, object> func);

        object ShowAddEditAutomation(Window owner, Guid? oid);


        void RegisterAddToBasket(Func<Window, object, object, List<Guid>, object> func);

        object ShowAddToBasket(Window owner, object basket, object criteria, List<Guid> guids);



        void RegisterShowContact(Func<Window, object, object> func);

        object ShowShowContact(Window owner, object contact);

        void RegisterShowFastContact(Func<Window, object, object> func);

        object ShowShowFastContact(Window owner, object contact);


        void RegisterManageFactorTitle(Func<Window, object> func);

        object ShowManageFactorTitle(Window owner);

        #region Main Window

        void RegisterMainWindow(IMainWindow window);
        void Show();
        void Hide();

        void LoadContent(Type view, Type model);

        Window GetWindow();


        void ShowBusyIndicator();
        void HideBusyIndicator();


        #endregion
    }
}
