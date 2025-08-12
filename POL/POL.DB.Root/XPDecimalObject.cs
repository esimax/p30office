using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace POL.DB.Root
{
    [NonPersistent, MemberDesignTimeVisibility(false)]
    public abstract class XPInt32Object : XPCustomObject
    {
        protected XPInt32Object() : base() { }
        protected XPInt32Object(Session session) : base(session) { }
        protected XPInt32Object(Session session, XPClassInfo classInfo) : base(session, classInfo) { }

        protected Int32 oid;
        [Persistent("OID"), Key(AutoGenerate = true)]
        public Int32 Oid
        {
            get
            {
                return oid;
            }
            set
            {
                Int64 oldValue = Oid;
                if (oldValue == value)
                    return;
                oid = value;
                OnChanged("Oid", oldValue, value);
            }
        }

        protected override void OnSaving()
        {
            try
            {
                foreach (var v1 in this.ClassInfo.OwnMembers)
                {
                    try
                    {
                        if (v1.IsPersistent && (v1.MemberType == typeof(string)))
                        {
                            foreach (var v2 in v1.Attributes)
                            {
                                object o = v2.GetType();
                                if (v2.GetType() == typeof(PersianStringAttribute))
                                {                                    
                                    string s = this.GetMemberValue(v1.Name) as string;
                                    POL.Lib.Utils.HelperConvert.CorrectPersianBug(ref s);
                                    this.SetMemberValue(v1.Name, s);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }

            base.OnSaving();
        }

        #region Fields
        new public static FieldsClass Fields { get { return new FieldsClass(); } }
        new public class FieldsClass : XPCustomObject.FieldsClass
        {
            public FieldsClass() : base() { }
            public FieldsClass(string propertyName) : base(propertyName) { }
            public OperandProperty Oid { get { return new OperandProperty(GetNestedName("Oid")); } }
        }
        #endregion
    }   
}
