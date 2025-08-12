namespace POL.Lib.Interfaces
{
    public interface IModuleRibbon
    {
        object GetRibbon();
        void UnloadChildRibbons();
        void LoadChildRibbons();
    }
}
