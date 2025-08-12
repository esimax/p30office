using System;
using System.Collections.ObjectModel;
using System.Linq;
using POL.Lib.Interfaces;

namespace POC.Shell.Adapters
{
    internal class AdapterDataFieldManager : IDataFieldManager
    {
        public AdapterDataFieldManager()
        {
            RegisteredDataFields = new ObservableCollection<IDataField>();
        }


        #region IDataFieldManager
        public IDataField FindByType(EnumProfileItemType type)
        {
            var q = from n in RegisteredDataFields where n.ItemType == type select n;
            return q.FirstOrDefault();
        }

        public ObservableCollection<IDataField> GetRegisterFields()
        {
            return RegisteredDataFields;
        }

        public void Register(IDataField datafield)
        {
            if (datafield == null)
                return;
            var q = from n in RegisteredDataFields where n.ItemType == datafield.ItemType select n;
            if (q.Any())
                throw new Exception("Can not register same DataField.ItemType");
            RegisteredDataFields.Add(datafield);
            RegisteredDataFields = new ObservableCollection<IDataField>(RegisteredDataFields.OrderBy(n => n.ItemType));
        }
        #endregion



        private ObservableCollection<IDataField> RegisteredDataFields { get; set; }

    }
}
