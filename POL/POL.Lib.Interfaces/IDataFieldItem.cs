namespace POL.Lib.Interfaces
{
    public interface IDataFieldItem
    {
        bool NeedToSave { get; set; }
        bool SaveChanges { get; set; }
    }
}
