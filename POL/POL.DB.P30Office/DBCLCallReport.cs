using System;
using System.Drawing.Printing;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCLCallReport : XPGUIDLogableObject 
    {
        #region Design

        public DBCLCallReport(Session session)
            : base(session)
        {
        }
        #endregion

        public static DBCLCallReport FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCLCallReport>(new BinaryOperator("Oid", oid));
        }

        public static DBCLCallReport FindDuplicateTitleExcept(Session session, DBCLCallReport exceptFilter, string title,
            string userName)
        {
            var go = new GroupOperator();
            if (exceptFilter != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptFilter.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("UserCreated", userName));
            return session.FindObject<DBCLCallReport>(go);
        }

        public static XPCollection<DBCLCallReport> GetAllByMembership(Session session, MembershipUser user)
        {
            if (user == null) return null;
            var go = new GroupOperator(GroupOperatorType.Or);
            go.Operands.Add(new BinaryOperator("UserCreated", user.UserName));
            go.Operands.Add(new BinaryOperator("PermissionBaseType", EnumPermissionBaseType.EveryOne));
            user.Roles.ToList().ForEach(
                r => go.Operands.Add(new GroupOperator(
                    new BinaryOperator("PermissionBaseType", EnumPermissionBaseType.RoleBase),
                    new BinaryOperator("PermissionValue", r)
                    )));
            go.Operands.Add(new GroupOperator(
                new BinaryOperator("PermissionBaseType", EnumPermissionBaseType.UserBase),
                new BinaryOperator("PermissionValue", user.UserName)
                ));
            return new XPCollection<DBCLCallReport>(session);
        }

        public override string ToString()
        {
            return Title;
        }

        #region Title

        private string _Title;

        [PersianString]
        [Size(64)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Caption

        private string _Caption;

        [PersianString]
        [Size(1024)]
        public string Caption
        {
            get { return _Caption; }
            set { SetPropertyValue("Caption", ref _Caption, value); }
        }

        #endregion

        #region ChartAxisXType

        private EnumChartAxisXType _ChartAxisXType;

        public EnumChartAxisXType ChartAxisXType
        {
            get { return _ChartAxisXType; }
            set { SetPropertyValue("ChartAxisXType", ref _ChartAxisXType, value); }
        }

        #endregion

        #region ChartAxisYType

        private EnumChartAxisYType _ChartAxisYType;

        public EnumChartAxisYType ChartAxisYType
        {
            get { return _ChartAxisYType; }
            set { SetPropertyValue("ChartAxisYType", ref _ChartAxisYType, value); }
        }

        #endregion

        #region ChartSeparation

        private EnumChartSeparation _ChartSeparation;

        public EnumChartSeparation ChartSeparation
        {
            get { return _ChartSeparation; }
            set { SetPropertyValue("ChartSeparation", ref _ChartSeparation, value); }
        }

        #endregion

        #region RemoveEmpty

        private bool _RemoveEmpty;

        public bool RemoveEmpty
        {
            get { return _RemoveEmpty; }
            set { SetPropertyValue("RemoveEmpty", ref _RemoveEmpty, value); }
        }

        #endregion

        #region SortAxisX

        private bool _SortAxisX;

        public bool SortAxisX
        {
            get { return _SortAxisX; }
            set { SetPropertyValue("SortAxisX", ref _SortAxisX, value); }
        }

        #endregion

        #region Paper

        private PaperKind _Paper;

        public PaperKind Paper
        {
            get { return _Paper; }
            set { SetPropertyValue("Paper", ref _Paper, value); }
        }

        #endregion

        #region IsLandscape

        private bool _IsLandscape;

        public bool IsLandscape
        {
            get { return _IsLandscape; }
            set { SetPropertyValue("IsLandscape", ref _IsLandscape, value); }
        }

        #endregion

        #region MarginTop

        private double _MarginTop;

        public double MarginTop
        {
            get { return _MarginTop; }
            set { SetPropertyValue("MarginTop", ref _MarginTop, value); }
        }

        #endregion

        #region MarginBottom

        private double _MarginBottom;

        public double MarginBottom
        {
            get { return _MarginBottom; }
            set { SetPropertyValue("MarginBottom", ref _MarginBottom, value); }
        }

        #endregion

        #region MarginRight

        private double _MarginRight;

        public double MarginRight
        {
            get { return _MarginRight; }
            set { SetPropertyValue("MarginRight", ref _MarginRight, value); }
        }

        #endregion

        #region MarginLeft

        private double _MarginLeft;

        public double MarginLeft
        {
            get { return _MarginLeft; }
            set { SetPropertyValue("MarginLeft", ref _MarginLeft, value); }
        }

        #endregion

        #region ChartRotate

        private bool _ChartRotate;

        public bool ChartRotate
        {
            get { return _ChartRotate; }
            set { SetPropertyValue("ChartRotate", ref _ChartRotate, value); }
        }

        #endregion

        #region ChartLable

        private bool _ChartLable;

        public bool ChartLable
        {
            get { return _ChartLable; }
            set { SetPropertyValue("ChartLable", ref _ChartLable, value); }
        }

        #endregion

        #region ChartLegend

        private bool _ChartLegend;

        public bool ChartLegend
        {
            get { return _ChartLegend; }
            set { SetPropertyValue("ChartLegend", ref _ChartLegend, value); }
        }

        #endregion

        #region PermissionBaseType

        private EnumPermissionBaseType _PermissionBaseType;

        public EnumPermissionBaseType PermissionBaseType
        {
            get { return _PermissionBaseType; }
            set { SetPropertyValue("PermissionBaseType", ref _PermissionBaseType, value); }
        }

        #endregion

        #region PermissionValue

        private string _PermissionValue;

        [Size(64)]
        public string PermissionValue
        {
            get { return _PermissionValue; }
            set { SetPropertyValue("PermissionValue", ref _PermissionValue, value); }
        }

        #endregion
    }
}
