namespace POL.Lib.Utils.Westwind
{
    public interface IRemoteInterface
    {
        object Invoke(string lcMethod, object[] Parameters);
    }
}
