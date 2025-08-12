using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POC.Shell.Adapters
{
    internal class AdapterDatabase : IDatabase
    {
        #region Fields
        private Session _Dxs = null;
        private List<Action> ListOnDatabaseConnected { get; set; }
        private List<Action> ListOnDatabaseDisconnecting { get; set; }
        private List<Action> ListOnDatabaseBackupStart { get; set; }
        private List<Action> ListOnDatabaseBackupStop { get; set; }

        private readonly static object lockObject = new object();

        #region DataLayer
        static IDataLayer fDataLayer;
        static IDataLayer DataLayer
        {
            get
            {
                if (fDataLayer == null)
                {
                    lock (lockObject)
                    {
                        fDataLayer = GetDataLayer();
                    }
                }
                return fDataLayer;
            }
        }
        #endregion
        #endregion 

        #region CTOR
        public AdapterDatabase()
        {
            ListOnDatabaseConnected = new List<Action>();
            ListOnDatabaseDisconnecting = new List<Action>();
        }
        #endregion

        #region IDatabase
        public Session Dxs
        {
            get
            {
                if (_Dxs != null && !string.IsNullOrEmpty( _Dxs.ConnectionString)) 
                    return _Dxs;
                try
                {
                    _Dxs = GetNewSession();
                    return _Dxs;
                }
                catch
                {
                    return null;
                }
            }
        }

        public Session GetNewSession()
        {
            var cs = GetConnectionString();
            return cs == string.Empty ? null : new Session
            {
                ConnectionString = cs,
                LockingOption = LockingOption.None,
                OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.Mixed,
            };
        }
        public UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
        }

        public void RaiseOnDatabaseConnected()
        {
            ListOnDatabaseConnected.ForEach(e => e.Invoke());
        }
        public void RaiseOnDatabaseDisconnecting()
        {
            ListOnDatabaseDisconnecting.ForEach(e => e.Invoke());
        }
        public void SubscribeOnDatabaseConnected(Action action)
        {
            ListOnDatabaseConnected.Add(action);
        }
        public void SubscribeOnDatabaseDisconnecting(Action action)
        {
            ListOnDatabaseDisconnecting.Add(action);
        }
        

        public void RaiseOnDatabaseBackupStart()
        {
            throw new NotImplementedException();
        }
        public void RaiseOnDatabaseBackupStop()
        {
            throw new NotImplementedException();
        }
        public void SubscribeOnDatabaseBackupStart(Action action)
        {
            throw new NotImplementedException();
        }
        public void SubscribeOnDatabaseBackupStop(Action action)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region [METHODS]
        private static string GetConnectionString()
        {
            if (ServiceLocator.Current == null) return string.Empty;
            var pocCore = ServiceLocator.Current.GetInstance<POCCore>();
            if (pocCore == null) return string.Empty;
            if (pocCore.STCI == null) return string.Empty;

            switch (pocCore.STCI.DatabaseProvider)
            {
                case EnumDatabaseProvider.MSSQL:
                    if (POL.Lib.XOffice.HelperSettingsClient.UseMSSQLServer2)
                    {
                        if (pocCore.STCI.MSSQLWindowsAuthorization)
                            return MSSqlConnectionProvider.GetConnectionString(pocCore.STCI.MSSQLServer2, pocCore.STCI.MSSQLDatabase);
                        return MSSqlConnectionProvider.GetConnectionString(
                            pocCore.STCI.MSSQLServer2, pocCore.STCI.MSSQLUserName, pocCore.STCI.MSSQLPassword, pocCore.STCI.MSSQLDatabase);
                    }
                    else
                    {
                        if (pocCore.STCI.MSSQLWindowsAuthorization)
                            return MSSqlConnectionProvider.GetConnectionString(pocCore.STCI.MSSQLServer, pocCore.STCI.MSSQLDatabase);
                        return MSSqlConnectionProvider.GetConnectionString(
                            pocCore.STCI.MSSQLServer, pocCore.STCI.MSSQLUserName, pocCore.STCI.MSSQLPassword, pocCore.STCI.MSSQLDatabase);
                    }
                case EnumDatabaseProvider.MSSQLCE:
                    return MSSqlCEConnectionProvider.GetConnectionString(pocCore.STCI.MSSQLCEFileName);
                default:
                    return string.Empty;
            }
        }
        private static IDataLayer GetDataLayer()
        {
            XpoDefault.Session = null;
            var conn = GetConnectionString();
            var store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.DatabaseAndSchema);




            IDataLayer dl = new SimpleDataLayer(store);
            return dl;
        }
        #endregion

        #region LocalSession
        private Session _LocalSession;
        public Session LocalSession
        {
            get
            {
                if (_LocalSession != null) return _LocalSession;
                try
                {
                    _LocalSession = new Session
                                       {
                                           ConnectionString =
                                               String.Format("XpoProvider=XmlDataSet;Data Source={0}", "LocalCach.xml")
                                       };
                    return _LocalSession;
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
