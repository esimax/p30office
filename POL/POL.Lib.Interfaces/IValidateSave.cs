namespace POL.Lib.Interfaces
{
    public interface IValidateSaveFastContactModule
    {
        bool Validate();
        bool Save();

        object Contact { get; set; }
    }
}
