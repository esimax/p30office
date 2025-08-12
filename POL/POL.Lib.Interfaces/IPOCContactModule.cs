using System;
using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public interface IPOCContactModule
    {
        object SelectedContact { get; set; }
        int LastContactCode { get; }


        DateTime SelectedDate { get; set; }
        void RegisterContactModule(POCContactModuleItem item);
        void InvokeContactModule(string key);
        List<POCContactModuleItem> GetList();

        event EventHandler<POCContactModuleEventArgs> OnModuleInvoked;
        event EventHandler OnSelectedContactChanged;
        void RaiseOnSelectedContactChanged();


        void GotoContactByCode(int code);
        void HookGotoContactByCode(Action<int> action, int order);
        event EventHandler OnSelectedDateChanged;
        void RaiseOnSelectedDateChanged();
    }
}
