using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.Lib.Utils;

namespace POL.DB.Root
{
    [NonPersistent, MemberDesignTimeVisibility(false)]
    public abstract class XPGUIDObject : XPCustomObject
    {
        #region CTOR


        protected XPGUIDObject(Session session)
            : base(session)
        {
        }


        #endregion

        protected override void OnSaving()
        {
            base.OnSaving();

            try
            {
                foreach (var v1 in ClassInfo.OwnMembers)
                {
                    try
                    {
                        if (!v1.IsPersistent || (v1.MemberType != typeof (string))) continue;
                        foreach (var v2 in v1.Attributes)
                        {
                            if (!(v2 is PersianStringAttribute)) continue;
                            var s = GetMemberValue(v1.Name) as string;
                            var vtemp = HelperConvert.CorrectPersianBug(s);
                            if (s != vtemp)
                                SetMemberValue(v1.Name, vtemp);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        public static void FixEmptyGroupOperand(GroupOperator criteria)
        {
            if (ReferenceEquals(criteria, null)) return;
            var toremove = (from t in criteria.Operands let v = t.ToString() where v == "()" select t).ToList();
            if (toremove.Any())
                foreach (var criteriaOperator in toremove)
                {
                    criteria.Operands.Remove(criteriaOperator);
                }
            if (!criteria.Operands.Any())
            {
                criteria.Operands.Add(new BinaryOperator("Oid", Guid.Empty, BinaryOperatorType.NotEqual));
            }
        }

        #region Oid

        private Guid _Oid;

        [DisplayName("كد واحد")]
        [Persistent("Oid"), Key(AutoGenerate = true)]
        public Guid Oid
        {
            get { return _Oid; }
            set
            {
                if (_Oid == value) return;
                _Oid = value;
                SetPropertyValue("Oid", ref _Oid, value);
            }
        }

        #endregion

        #region Fields

        public new static FieldsClass Fields
        {
            get { return new FieldsClass(); }
        }

        public new class FieldsClass : PersistentBase.FieldsClass
        {
            public FieldsClass()
            {
            }

            public FieldsClass(string propertyName)
                : base(propertyName)
            {
            }

            public OperandProperty Oid
            {
                get { return new OperandProperty(GetNestedName("Oid")); }
            }
        }

        #endregion
    }
}
