using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public class DBCASchedule : XPInt64LogableObject
    {
        #region Design

        public DBCASchedule(Session session)
            : base(session)
        {
        }


        #endregion

        public static XPCollection<DBCASchedule> GetByRolesAndUsers(Session session, Guid currentUserOid,
            string userName, Guid[] roles)
        {
            var goOr = new GroupOperator(GroupOperatorType.Or);

            {
                var goAnd = new GroupOperator(GroupOperatorType.And);
                goAnd.Operands.Add(new BinaryOperator("ViewPermissionType", 1));
                goAnd.Operands.Add(new BinaryOperator("ViewOid", currentUserOid));
                goOr.Operands.Add(goAnd);
            }
            {
                var goAnd = new GroupOperator(GroupOperatorType.And);
                goAnd.Operands.Add(new BinaryOperator("EditPermissionType", 1));
                goAnd.Operands.Add(new BinaryOperator("EditOid", currentUserOid));
                goOr.Operands.Add(goAnd);
            }

            {
                goOr.Operands.Add(new BinaryOperator("EditPermissionType", 3));
                goOr.Operands.Add(new BinaryOperator("ViewPermissionType", 3));
            }

            {
                if (roles != null)
                {
                    foreach (var role in roles)
                    {
                        {
                            var goAnd = new GroupOperator(GroupOperatorType.And);
                            goAnd.Operands.Add(new BinaryOperator("EditPermissionType", 2));
                            goAnd.Operands.Add(new BinaryOperator("EditOid", role));
                            goOr.Operands.Add(goAnd);
                        }
                        {
                            var goAnd = new GroupOperator(GroupOperatorType.And);
                            goAnd.Operands.Add(new BinaryOperator("ViewPermissionType", 2));
                            goAnd.Operands.Add(new BinaryOperator("ViewOid", role));
                            goOr.Operands.Add(goAnd);
                        }
                    }
                }
            }

            {
                goOr.Operands.Add(new BinaryOperator("UserCreated", userName));
            }


            return new XPCollection<DBCASchedule>(session, goOr);
        }

        public static DBCASchedule FindByOid(Session session, long oid)
        {
            return session.FindObject<DBCASchedule>(new BinaryOperator("Oid", oid));
        }

        #region Subject

        private string _Subject;

        [Size(128)]
        public string Subject
        {
            get { return _Subject; }
            set { SetPropertyValue("Subject", ref _Subject, value); }
        }

        #endregion

        #region StartTime

        private DateTime _StartTime;

        public DateTime StartTime
        {
            get { return _StartTime; }
            set { SetPropertyValue("StartTime", ref _StartTime, value); }
        }

        #endregion

        #region EndTime

        private DateTime _EndTime;

        public DateTime EndTime
        {
            get { return _EndTime; }
            set { SetPropertyValue("EndTime", ref _EndTime, value); }
        }

        #endregion

        #region AllDay

        private bool _AllDay;

        public bool AllDay
        {
            get { return _AllDay; }
            set { SetPropertyValue("AllDay", ref _AllDay, value); }
        }

        #endregion

        #region Label

        private int _Label;

        public int Label
        {
            get { return _Label; }
            set { SetPropertyValue("Label", ref _Label, value); }
        }

        #endregion

        #region Status

        private int _Status;

        public int Status
        {
            get { return _Status; }
            set { SetPropertyValue("Status", ref _Status, value); }
        }

        #endregion

        #region Description

        private string _Description;

        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }

        #endregion

        #region EventType

        private int _EventType;

        public int EventType
        {
            get { return _EventType; }
            set { SetPropertyValue("EventType", ref _EventType, value); }
        }

        #endregion

        #region RecurrenceInfo

        private string _RecurrenceInfo;

        [Size(SizeAttribute.Unlimited)]
        public string RecurrenceInfo
        {
            get { return _RecurrenceInfo; }
            set { SetPropertyValue("RecurrenceInfo", ref _RecurrenceInfo, value); }
        }

        #endregion

        #region ReminderInfo

        private string _ReminderInfo;

        [Size(SizeAttribute.Unlimited)]
        public string ReminderInfo
        {
            get { return _ReminderInfo; }
            set { SetPropertyValue("ReminderInfo", ref _ReminderInfo, value); }
        }

        #endregion

        #region CategoryOid

        private Guid _CategoryOid;

        public Guid CategoryOid
        {
            get { return _CategoryOid; }
            set { SetPropertyValue("CategoryOid", ref _CategoryOid, value); }
        }

        #endregion

        #region ContactOid

        private Guid _ContactOid;

        public Guid ContactOid
        {
            get { return _ContactOid; }
            set { SetPropertyValue("ContactOid", ref _ContactOid, value); }
        }

        #endregion

        #region ProfileItemOid

        private Guid _ProfileItemOid;

        public Guid ProfileItemOid
        {
            get { return _ProfileItemOid; }
            set { SetPropertyValue("ProfileItemOid", ref _ProfileItemOid, value); }
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
