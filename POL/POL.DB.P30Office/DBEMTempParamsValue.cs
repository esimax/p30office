using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    [DeferredDeletion(false), OptimisticLocking(false)]
    public class DBEMTempParamsValue : XPObject
    {
        #region Design

        public DBEMTempParamsValue(Session session) : base(session)
        {
        }
        #endregion

        public static DBEMTempParamsValue FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBEMTempParamsValue>(new BinaryOperator("Oid", oid));
        }

        public static DBEMTempParamsValue FindByOidTemplateAndTag(Session session, Guid tempOid, string tagTitle)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("Tag", HelperConvert.CorrectPersianBug(tagTitle),
                BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("TemplateOid", tempOid, BinaryOperatorType.Equal));
            return session.FindObject<DBEMTempParamsValue>(go);
        }

        public static XPCollection<DBEMTempParamsValue> GetByTemplate(Session session, Guid tempOid)
        {
            return new XPCollection<DBEMTempParamsValue>(session, new BinaryOperator("TemplateOid", tempOid));
        }

        #region Tag

        private string _Tag;

        [Size(64)]
        [PersianString]
        public string Tag
        {
            get { return _Tag; }
            set { SetPropertyValue("Tag", ref _Tag, value); }
        }

        #endregion

        #region StringValue

        private string _StringValue;

        [Size(SizeAttribute.Unlimited)]
        public string StringValue
        {
            get { return _StringValue; }
            set { SetPropertyValue("StringValue", ref _StringValue, value); }
        }

        #endregion

        #region TemplateOid

        private Guid _TemplateOid;

        public Guid TemplateOid
        {
            get { return _TemplateOid; }
            set { SetPropertyValue("TemplateOid", ref _TemplateOid, value); }
        }

        #endregion
    }
}
