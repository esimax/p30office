using System.Collections.ObjectModel;

namespace POL.Lib.Interfaces
{
    public interface IDataFieldManager
    {
        void Register(IDataField datafield);
        ObservableCollection<IDataField> GetRegisterFields();
        IDataField FindByType(EnumProfileItemType type);
    }
}
