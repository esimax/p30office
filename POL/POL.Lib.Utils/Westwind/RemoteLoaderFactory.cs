using System;
using System.Reflection;

namespace POL.Lib.Utils.Westwind
{
    public class RemoteLoaderFactory : MarshalByRefObject
    {
        private const BindingFlags bfi = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;

        public IRemoteInterface Create(string assemblyFile, string typeName, object[] constructArgs)
        {
            return (IRemoteInterface)Activator.CreateInstanceFrom(
                assemblyFile, typeName, false, bfi, null, constructArgs,
                null, null, null).Unwrap();
        }
    }
}
