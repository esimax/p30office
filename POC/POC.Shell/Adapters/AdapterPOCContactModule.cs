using System.Collections.Generic;
using System.Linq;
using POL.Lib.Interfaces;
using System;
using Microsoft.Practices.ServiceLocation;

namespace POC.Shell.Adapters
{
    internal class AdapterPOCContactModule : IPOCContactModule
    {
        #region CTOR
        public AdapterPOCContactModule()
        {
            Holder = new Dictionary<string, POCContactModuleItem>();
        }
        #endregion

        private Dictionary<string, POCContactModuleItem> Holder { get; set; }
        private IMembership AMembership { get; set; }

        #region IPOCContactModule
        public List<POCContactModuleItem> GetList()
        {
            if (AMembership == null)
                AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            var q = from n in Holder where AMembership.HasPermission(n.Value.Permission) orderby n.Value.Order select n.Value;
            return q.Any() ? q.ToList() : null;
        }
        public void InvokeContactModule(string key)
        {
            if (Holder.ContainsKey(key))
            {
                var item = Holder[key];
                item.IsChecked = true;
            }
            else
            {
                var empty = new POCContactModuleItem() { Key = "" };
                ClearOtherChecked(empty);
                RaiseOnModuleInvoked(empty);
            }
        }
        public void RegisterContactModule(POCContactModuleItem item)
        {
            if (Holder.ContainsKey(item.Key)) return;
            item.OnIsChecked +=
                (s, e) =>
                {
                    var h = s as POCContactModuleItem;
                    ClearOtherChecked(h);
                    RaiseOnModuleInvoked(h);
                };
            Holder.Add(item.Key, item);
        }
        public event EventHandler<POCContactModuleEventArgs> OnModuleInvoked;


        public event EventHandler OnSelectedContactChanged;
        public void RaiseOnSelectedContactChanged()
        {
            if (OnSelectedContactChanged == null) return;
            var etemp = OnSelectedContactChanged;
            etemp.Invoke(this, EventArgs.Empty);
        }
        public object SelectedContact { get; set; }
        #endregion

        protected void ClearOtherChecked(POCContactModuleItem cmi)
        {
            var q = from n in Holder.Values where n.Key != cmi.Key select n;
            foreach (var v in q)
                v.IsChecked = false;
        }
        protected void RaiseOnModuleInvoked(POCContactModuleItem h)
        {
            if (OnModuleInvoked == null) return;
            var temp = OnModuleInvoked;
            temp.Invoke(this, new POCContactModuleEventArgs(h));
        }





        public void GotoContactByCode(int code)
        {

            _lastContactCode = code;
            var aMembership = ServiceLocator.Current.GetInstance<IMembership>();
            if (!aMembership.HasPermission(POL.Lib.XOffice.PCOPermissions.Contact_Contact_View)) return;
            _gotoContactByCodeHooksList.OrderBy(n => n.Item2).ToList().ForEach(n => n.Item1.Invoke(code));
        }



        public event EventHandler OnSelectedDateChanged;

        public void RaiseOnSelectedDateChanged()
        {
            if (OnSelectedDateChanged == null) return;
            var temp = OnSelectedDateChanged;
            temp.Invoke(this, EventArgs.Empty);
        }

        private int _lastContactCode = -1;
        public int LastContactCode
        {
            get { return _lastContactCode; }
        }
        public DateTime SelectedDate { get; set; }


        private readonly List<Tuple<Action<int>, int>> _gotoContactByCodeHooksList = new List<Tuple<Action<int>, int>>();
        public void HookGotoContactByCode(Action<int> action, int order)
        {
            if (_gotoContactByCodeHooksList.All(n => n.Item2 != order) && action != null)
                _gotoContactByCodeHooksList.Add(new Tuple<Action<int>, int>(action, order));
        }
    }
}
