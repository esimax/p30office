using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTProfileTable : XPGUIDLogableObject 
    {
        #region Design

        public DBCTProfileTable(Session session) : base(session)
        {
        }
        #endregion

        #region Link [1-n] - TableValues

        public XPCollection<DBCTProfileTValue> TableValues
        {
            get { return GetCollection<DBCTProfileTValue>("TableValues"); }
        }

        #endregion

        public int GetTableDepth()
        {
            var dList = new List<int>();
            dList.Add(1);
            var values = DBCTProfileTValue.GetAll(Session, Oid);
            foreach (var v in values)
            {
                if (v.Children.Count > 0)
                {
                    dList.Add(2);
                    foreach (var sub1 in v.Children)
                    {
                        if (sub1.Children.Count > 0)
                        {
                            dList.Add(3);
                        }
                    }
                }
            }
            return dList.Max();
        }

        public void UpdateDepthValue()
        {
            var nd = GetTableDepth();
            if (nd != ValueDepth)
            {
                ValueDepth = nd;
                Save();
            }
        }

        public static DBCTProfileTable FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTProfileTable>(new BinaryOperator("Oid", oid));
        }

        public static DBCTProfileTable FindDuplicateTitleExcept(Session session, DBCTProfileTable exceptContactCat,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContactCat != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContactCat.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCTProfileTable>(go);
        }

        public static XPCollection<DBCTProfileTable> GetAll(Session session)
        {
            return new XPCollection<DBCTProfileTable>(session, null,
                new SortProperty("Title", SortingDirection.Ascending));
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

        #region ValueDepth

        private int _ValueDepth;

        public int ValueDepth
        {
            get { return _ValueDepth; }
            set { SetPropertyValue("ValueDepth", ref _ValueDepth, value); }
        }

        #endregion
    }
}
