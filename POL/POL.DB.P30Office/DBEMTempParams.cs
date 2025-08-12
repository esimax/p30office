using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBEMTempParams : XPGUIDLogableObject
    {
        #region Design

        public DBEMTempParams(Session session) : base(session)
        {
        }

        #endregion

        [NonPersistent]
        public string TagTypeString
        {
            get
            {
                switch (TagType)
                {
                    case EnumEmailTemplateTagType.String:
                        return "متن تك خط";
                    case EnumEmailTemplateTagType.Memo:
                        return "متن چند خط";
                    case EnumEmailTemplateTagType.ProfileItem:
                        return "ماژول پروفایل";
                }
                return string.Empty;
            }
        }

        [NonPersistent]
        public string ProfileItemString
        {
            get
            {
                if (ProfileItem == null) return string.Empty;
                return ProfileItem.FullPathString;
            }
        }

        public static DBEMTempParams FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBEMTempParams>(new BinaryOperator("Oid", oid));
        }

        public static DBEMTempParams FindDuplicateTagExcept(Session session, DBEMTemplate parentTemp,
            DBEMTempParams exceptContact, string tagTitle)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Tag", HelperConvert.CorrectPersianBug(tagTitle),
                BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("Template.Oid", parentTemp.Oid, BinaryOperatorType.Equal));
            return session.FindObject<DBEMTempParams>(go);
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

        #region TagType

        private EnumEmailTemplateTagType _TagType;

        public EnumEmailTemplateTagType TagType
        {
            get { return _TagType; }
            set { SetPropertyValue("TagType", ref _TagType, value); }
        }

        #endregion

        #region ProfileItem

        private DBCTProfileItem _ProfileItem;

        public DBCTProfileItem ProfileItem
        {
            get { return _ProfileItem; }
            set { SetPropertyValue("ProfileItem", ref _ProfileItem, value); }
        }

        #endregion

        #region Template

        private DBEMTemplate _Template;

        [Association("EMTemplate_EMTempParams")]
        public DBEMTemplate Template
        {
            get { return _Template; }
            set { SetPropertyValue("Template", ref _Template, value); }
        }

        #endregion
    }
}
