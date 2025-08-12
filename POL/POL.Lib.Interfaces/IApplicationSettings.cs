namespace POL.Lib.Interfaces
{
    public interface IApplicationSettings
    {
        TResult GetValue<TResult, TEnum>(TEnum enumName, bool fromCach, bool isEncripted);
        TResult GetValue<TResult, TEnum>(TEnum enumName, bool fromCach);
        TResult GetValue<TResult, TEnum>(TEnum enumName);

        void SetValue<TResult, TEnum>(TEnum enumName, TResult value, bool isEncripted);
        void SetValue<TResult, TEnum>(TEnum enumName, TResult value);
    }
}
