namespace POL.Lib.Interfaces
{
    public interface IRefrashable
    {
        bool RequiresRefresh { get; set; }
        void DoRefresh();
    }
}
