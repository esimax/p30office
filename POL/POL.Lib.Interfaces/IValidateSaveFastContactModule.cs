namespace POL.Lib.Interfaces
{
    public interface IValidateSaveFastContactModule
    {
        object Contact { get; set; }
        bool Validate();
        bool Save();
    }
}
