using System;

namespace POL.Lib.Interfaces
{
    public interface IDataField
    {
        EnumProfileItemType ItemType { get; }
        string Title { get; }
        string ImageUriString { get; }
        string Note { get; }

        object GetUISettingsWpf(object dbValue);

        object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus);

        void Save(object dbValue);

        void SetValueToDefault(object dbValue, object dbItem);


        string GetEmailData(object dbValue, object dbItem, object settings);

        bool IsRequiredSatisfied(object dbValue);
    }
}
