using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POL.DB.P30Office
{
    public class DBEMEmailApp : XPGUIDLogableObject
    {
        #region EmbedImage1

        [Delayed(true)]
        public byte[] EmbedImage1
        {
            get { return GetDelayedPropertyValue<byte[]>("EmbedImage1"); }
            set { SetDelayedPropertyValue("EmbedImage1", value); }
        }

        #endregion

        #region EmbedImage2

        [Delayed(true)]
        public byte[] EmbedImage2
        {
            get { return GetDelayedPropertyValue<byte[]>("EmbedImage2"); }
            set { SetDelayedPropertyValue("EmbedImage2", value); }
        }

        #endregion

        #region EmbedImagebase641

        [Delayed(true)]
        [Size(SizeAttribute.Unlimited)]
        public string EmbedImagebase641
        {
            get { return GetDelayedPropertyValue<string>("EmbedImagebase641"); }
            set { SetDelayedPropertyValue("EmbedImagebase641", value); }
        }

        #endregion

        #region EmbedImagebase642

        [Delayed(true)]
        [Size(SizeAttribute.Unlimited)]
        public string EmbedImagebase642
        {
            get { return GetDelayedPropertyValue<string>("EmbedImagebase642"); }
            set { SetDelayedPropertyValue("EmbedImagebase642", value); }
        }

        #endregion

        #region App - Folders

        [Association("EMEmailApp_EMEmailFolder"), Aggregated]
        public XPCollection<DBEMEmailFolder> Folders
        {
            get { return GetCollection<DBEMEmailFolder>("Folders"); }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} ({1})", Title, Email.ToLower());
        }

        public static DBEMEmailApp FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBEMEmailApp>(new BinaryOperator("Oid", oid));
        }

        public static XPCollection<DBEMEmailApp> GetAll(Session session)
        {
            return new XPCollection<DBEMEmailApp>(session);
        }

        #region Design

        public DBEMEmailApp(Session session) : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            AutoDeleteCount = 2000;
            AutoDeleteDay = 1;
            AutoDeletePolicy = EnumEmailAutoDeletePolicy.DoNotDelete;
            RemoveFromServerDelay = 0;
        }

        #endregion

        #region IsEnable

        private bool _IsEnable;

        public bool IsEnable
        {
            get { return _IsEnable; }
            set { SetPropertyValue("IsEnable", ref _IsEnable, value); }
        }

        #endregion

        #region Title

        private string _Title;

        [Size(32)]
        [PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Email

        private string _Email;

        [Size(254)]
        public string Email
        {
            get { return _Email; }
            set { SetPropertyValue("Email", ref _Email, value); }
        }

        #endregion

        #region UserName

        private string _UserName;

        [Size(32)]
        public string UserName
        {
            get { return _UserName; }
            set { SetPropertyValue("UserName", ref _UserName, value); }
        }

        #endregion

        #region Password

        private string _Password;

        [Size(128)]
        public string Password
        {
            get { return _Password; }
            set { SetPropertyValue("Password", ref _Password, value); }
        }

        #endregion

        #region IncomeServerName

        private string _IncomeServerName;

        [Size(64)]
        public string IncomeServerName
        {
            get { return _IncomeServerName; }
            set { SetPropertyValue("IncomeServerName", ref _IncomeServerName, value); }
        }

        #endregion

        #region OutgoingServerName

        private string _OutgoingServerName;

        [Size(64)]
        public string OutgoingServerName
        {
            get { return _OutgoingServerName; }
            set { SetPropertyValue("OutgoingServerName", ref _OutgoingServerName, value); }
        }

        #endregion

        #region IncomeServerPort

        private int _IncomeServerPort;

        public int IncomeServerPort
        {
            get { return _IncomeServerPort; }
            set { SetPropertyValue("IncomeServerPort", ref _IncomeServerPort, value); }
        }

        #endregion

        #region OutgoingServerPort

        private int _OutgoingServerPort;

        public int OutgoingServerPort
        {
            get { return _OutgoingServerPort; }
            set { SetPropertyValue("OutgoingServerPort", ref _OutgoingServerPort, value); }
        }

        #endregion

        #region UseSPA

        private bool _UseSPA;

        public bool UseSPA
        {
            get { return _UseSPA; }
            set { SetPropertyValue("UseSPA", ref _UseSPA, value); }
        }

        #endregion

        #region UseSPASend

        private bool _UseSPASend;

        public bool UseSPASend
        {
            get { return _UseSPASend; }
            set { SetPropertyValue("UseSPASend", ref _UseSPASend, value); }
        }

        #endregion

        #region RemoveFromServer

        private bool _RemoveFromServer;

        public bool RemoveFromServer
        {
            get { return _RemoveFromServer; }
            set { SetPropertyValue("RemoveFromServer", ref _RemoveFromServer, value); }
        }

        #endregion

        #region RemoveFromServerDelay

        private int _RemoveFromServerDelay;

        public int RemoveFromServerDelay
        {
            get { return _RemoveFromServerDelay; }
            set { SetPropertyValue("RemoveFromServerDelay", ref _RemoveFromServerDelay, value); }
        }

        #endregion

        #region IgnoreLargerThan

        private bool _IgnoreLargerThan;

        public bool IgnoreLargerThan
        {
            get { return _IgnoreLargerThan; }
            set { SetPropertyValue("IgnoreLargerThan", ref _IgnoreLargerThan, value); }
        }

        #endregion

        #region IgnoreLargerThanSizeKB

        private int _IgnoreLargerThanSizeKB;

        public int IgnoreLargerThanSizeKB
        {
            get { return _IgnoreLargerThanSizeKB; }
            set { SetPropertyValue("IgnoreLargerThanSizeKB", ref _IgnoreLargerThanSizeKB, value); }
        }

        #endregion

        #region AutoDeletePolicy

        private EnumEmailAutoDeletePolicy _AutoDeletePolicy;

        public EnumEmailAutoDeletePolicy AutoDeletePolicy
        {
            get { return _AutoDeletePolicy; }
            set { SetPropertyValue("AutoDeletePolicy", ref _AutoDeletePolicy, value); }
        }

        #endregion

        #region AutoDeleteCount

        private int _AutoDeleteCount;

        public int AutoDeleteCount
        {
            get { return _AutoDeleteCount; }
            set { SetPropertyValue("AutoDeleteCount", ref _AutoDeleteCount, value); }
        }

        #endregion

        #region AutoDeleteDay

        private int _AutoDeleteDay;

        public int AutoDeleteDay
        {
            get { return _AutoDeleteDay; }
            set { SetPropertyValue("AutoDeleteDay", ref _AutoDeleteDay, value); }
        }

        #endregion

        #region ContentSendHeader

        private string _ContentSendHeader;

        [Size(SizeAttribute.Unlimited)]
        public string ContentSendHeader
        {
            get { return _ContentSendHeader; }
            set { SetPropertyValue("ContentSendHeader", ref _ContentSendHeader, value); }
        }

        #endregion

        #region ContentSendFooter

        private string _ContentSendFooter;

        [Size(SizeAttribute.Unlimited)]
        public string ContentSendFooter
        {
            get { return _ContentSendFooter; }
            set { SetPropertyValue("ContentSendFooter", ref _ContentSendFooter, value); }
        }

        #endregion

        #region LimitEmailPerDay

        private int _LimitEmailPerDay;

        public int LimitEmailPerDay
        {
            get { return _LimitEmailPerDay; }
            set { SetPropertyValue("LimitEmailPerDay", ref _LimitEmailPerDay, value); }
        }

        #endregion

        #region LimitSizePerDay

        private int _LimitSizePerDay;

        public int LimitSizePerDay
        {
            get { return _LimitSizePerDay; }
            set { SetPropertyValue("LimitSizePerDay", ref _LimitSizePerDay, value); }
        }

        #endregion

        #region LimitSizePerEmail

        private int _LimitSizePerEmail;

        public int LimitSizePerEmail
        {
            get { return _LimitSizePerEmail; }
            set { SetPropertyValue("LimitSizePerEmail", ref _LimitSizePerEmail, value); }
        }

        #endregion

        #region UIDL

        private string _UIDL;

        [Size(64)]
        public string UIDL
        {
            get { return _UIDL; }
            set { SetPropertyValue("UIDL", ref _UIDL, value); }
        }

        #endregion

        #region CountSend

        private int _CountSend;

        public int CountSend
        {
            get { return _CountSend; }
            set { SetPropertyValue("CountSend", ref _CountSend, value); }
        }

        #endregion

        #region CountReceive

        private int _CountReceive;

        public int CountReceive
        {
            get { return _CountReceive; }
            set { SetPropertyValue("CountReceive", ref _CountReceive, value); }
        }

        #endregion

        #region SizeSend

        private long _SizeSend;

        public long SizeSend
        {
            get { return _SizeSend; }
            set { SetPropertyValue("SizeSend", ref _SizeSend, value); }
        }

        #endregion

        #region SizeReceive

        private long _SizeReceive;

        public long SizeReceive
        {
            get { return _SizeReceive; }
            set { SetPropertyValue("SizeReceive", ref _SizeReceive, value); }
        }

        #endregion

        #region ViewPermissionType
        private int _ViewPermissionType;

        public int ViewPermissionType
        {
            get { return _ViewPermissionType; }
            set { SetPropertyValue("ViewPermissionType", ref _ViewPermissionType, value); }
        }

        #endregion

        #region ViewOid
        private Guid _ViewOid;

        public Guid ViewOid
        {
            get { return _ViewOid; }
            set { SetPropertyValue("ViewOid", ref _ViewOid, value); }
        }

        #endregion

        #region EditPermissionType
        private int _EditPermissionType;

        public int EditPermissionType
        {
            get { return _EditPermissionType; }
            set { SetPropertyValue("EditPermissionType", ref _EditPermissionType, value); }
        }

        #endregion

        #region EditOid
        private Guid _EditOid;

        public Guid EditOid
        {
            get { return _EditOid; }
            set { SetPropertyValue("EditOid", ref _EditOid, value); }
        }

        #endregion

        #region DeletePermissionType
        private int _DeletePermissionType;

        public int DeletePermissionType
        {
            get { return _DeletePermissionType; }
            set { SetPropertyValue("DeletePermissionType", ref _DeletePermissionType, value); }
        }

        #endregion

        #region DeleteOid
        private Guid _DeleteOid;

        public Guid DeleteOid
        {
            get { return _DeleteOid; }
            set { SetPropertyValue("DeleteOid", ref _DeleteOid, value); }
        }

        #endregion
    }
}
