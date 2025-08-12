using System;
using DevExpress.Xpo;

namespace POL.Lib.Interfaces
{
    public interface IDatabase
    {
        Session Dxs { get; }

        Session LocalSession { get; }

        Session GetNewSession();
        UnitOfWork GetNewUnitOfWork();

        void RaiseOnDatabaseConnected();
        void RaiseOnDatabaseDisconnecting();
        void SubscribeOnDatabaseConnected(Action action);
        void SubscribeOnDatabaseDisconnecting(Action action);

        void RaiseOnDatabaseBackupStart();
        void RaiseOnDatabaseBackupStop();
        void SubscribeOnDatabaseBackupStart(Action action);
        void SubscribeOnDatabaseBackupStop(Action action);
    }
}
