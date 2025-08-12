using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.GL
{
    [OptimisticLocking(false)]
    public class DBGLExtraCodes : XPGUIDObject 
    {
        #region CTOR


        public DBGLExtraCodes(Session session)
            : base(session)
        {
        }



        #endregion

        public override string ToString()
        {
            return Title;
        }


        public static DBGLExtraCodes FindByOid(Session session, Guid guid)
        {
            return session.FindObject<DBGLExtraCodes>(new BinaryOperator("Oid", guid));
        }

        public static DBGLExtraCodes FindDuplicateTitleXXExcept(Session session, DBGLExtraCodes exceptContact,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBGLExtraCodes>(go);
        }

        public static XPCollection<DBGLExtraCodes> GetAll(Session session, CriteriaOperator filter = null)
        {
            return new XPCollection<DBGLExtraCodes>(session, filter);
        }

        public static DBGLExtraCodes GetByExtraCode(Session session, int countrycode, string phonenumber)
        {
            var go = new GroupOperator(GroupOperatorType.And);
            go.Operands.Add(new BinaryOperator("CountryCode", countrycode, BinaryOperatorType.Equal));

            var go2 = new GroupOperator(GroupOperatorType.Or);
            for (var i = 1; i < phonenumber.Length; i++)
            {
                var code = phonenumber.Substring(0, phonenumber.Length - i);
                go2.Operands.Add(new BinaryOperator("AreaCode", code));
            }
            go.Operands.Add(go2);
            var xpc = new XPCollection<DBGLExtraCodes>(session, go,
                new SortProperty("AreaCode", SortingDirection.Descending));
            xpc.TopReturnedObjects = 1;
            return xpc.Count == 1 ? xpc[0] : null;
        }


        #region Title

        private string _Title;

        [Size(256)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region CountryCode

        private int _CountryCode;

        public int CountryCode
        {
            get { return _CountryCode; }
            set { SetPropertyValue("CountryCode", ref _CountryCode, value); }
        }

        #endregion

        #region AreaCode

        private int _AreaCode;

        public int AreaCode
        {
            get { return _AreaCode; }
            set { SetPropertyValue("AreaCode", ref _AreaCode, value); }
        }

        #endregion



    }
}
