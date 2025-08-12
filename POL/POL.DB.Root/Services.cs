using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POL.DB.Root
{
    public class Services
    {
        static Services()
        {
            EnableLoogable = true;
        }

        public static bool EnableLoogable { get; set; }

        public static string GetProperUserName()
        {
            if (AMembership == null) return string.Empty;
            return !AMembership.IsAuthorized ? string.Empty : AMembership.ActiveUser.UserName;
        }

        #region AMembership

        private static IMembership _member;

        public static IMembership AMembership
        {
            get
            {
                try
                {
                    if (_member != null) return _member;
                    if (ServiceLocator.Current == null) return null;
                    _member = ServiceLocator.Current.GetInstance<IMembership>();
                    return _member ?? null;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion

        #region ADatabase

        private static IDatabase _database;

        public static IDatabase ADatabase
        {
            get
            {
                try
                {
                    if (_database != null) return _database;
                    if (ServiceLocator.Current == null) return null;
                    _database = ServiceLocator.Current.GetInstance<IDatabase>();
                    return _database ?? null;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion


    }
}
