using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Microsoft.CSharp;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTList : XPGUIDLogableObject 
    {
        private static readonly Dictionary<string, Assembly> asmDic = new Dictionary<string, Assembly>();

        #region Design

        public DBCTList(Session session) : base(session)
        {
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }

        protected override void OnSaving()
        {
            if (string.IsNullOrEmpty(TableName))
            {
                try
                {
                    TableName = "DBL" + DateTime.Now.Ticks.ToString("X").PadLeft(16, '0');
                }
                catch
                {
                }
            }
            base.OnSaving();
        }

        public static DBCTList FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTList>(new BinaryOperator("Oid", oid));
        }

        public static DBCTList FindDuplicateTitleExcept(Session session, DBCTList exceptListOid, string title)
        {
            var go = new GroupOperator();
            if (exceptListOid != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptListOid.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCTList>(go);
        }

        public static XPCollection<DBCTList> GetAll(Session session)
        {
            return new XPCollection<DBCTList>(session, null, new SortProperty("Title", SortingDirection.Ascending));
        }

        public Assembly GetAssemblyOfListObject()
        {
            if (asmDic.ContainsKey(TableName))
                return asmDic[TableName];

            var c = new CSharpCodeProvider();
            var cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("mscorlib.dll");

            cp.ReferencedAssemblies.Add(@"DevExpress.Xpo.v13.2.dll");
            cp.ReferencedAssemblies.Add(@"DevExpress.Data.v13.2.dll");

            cp.ReferencedAssemblies.Add(@"POL.Lib.Interfaces.dll");
            cp.ReferencedAssemblies.Add(@"POL.Lib.Utils.dll");
            cp.ReferencedAssemblies.Add(@"POL.DB.Root.dll");
            cp.ReferencedAssemblies.Add(@"POL.DB.General.dll");
            cp.ReferencedAssemblies.Add(@"POL.DB.P30Office.dll");


            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;
            var cr = c.CompileAssemblyFromSource(cp, GetDBClassString());
            if (cr.Errors.Count > 0)
            {
                throw new InvalidExpressionException(
                    string.Format("Error ({0}) line: {1}",
                        cr.Errors[0].ErrorText, cr.Errors[0].Line));
            }

            var asm = cr.CompiledAssembly;
            asmDic.Add(TableName, asm);
            return asm;
        }


        private string GetDBClassString()
        {
            var properties = GetDBProperties();
            var methods = GetDBMethods();
            var rv =
                DBClassStructure.Replace("|PROPERTIES|", properties)
                    .Replace("|METHODS|", methods)
                    .Replace("|DATABASENAME|", TableName);
            return rv;
        }

        private string GetDBProperties()
        {
            var sb = new StringBuilder();
            sb.Append(GetContactProperty());

            if (ProfileGroup == null) return sb.ToString();
            var items = DBCTProfileItem.GetAll(Session, ProfileGroup.Oid);
            if (items.Count == 0) return string.Empty;

            foreach (var item in items)
            {
                sb.Append(GetDBProperty(item));
            }
            return sb.ToString();
        }

        private string GetContactProperty()
        {
            return GetDBPropNormal("DBCTContact", "Contact");
        }

        private string GetDBPropNormal(string propType, string propName)
        {
            return DBNormalProperty.Replace("|TYPE|", propType).Replace("|PROPNAME|", propName);
        }

        private string GetDBPropForcePersist(string propType, string propName)
        {
            return DBForcePersistProperty.Replace("|TYPE|", propType).Replace("|PROPNAME|", propName);
        }

        private string GetDBPropPersian(string propType, string propName)
        {
            return DBPersianProperty.Replace("|TYPE|", propType).Replace("|PROPNAME|", propName);
        }

        private string GetDBProperty(DBCTProfileItem item)
        {
            if (item == null) return string.Empty;
            switch (item.ItemType)
            {
                case EnumProfileItemType.Bool:
                    return GetDBPropertyForBool(item);
                case EnumProfileItemType.Double:
                    return GetDBPropertyForDouble(item);
                case EnumProfileItemType.Country:
                    return GetDBPropertyForCountry(item);
                case EnumProfileItemType.City:
                    return GetDBPropertyForCity(item);
                case EnumProfileItemType.Location:
                    return GetDBPropertyForLocation(item);
                case EnumProfileItemType.String:
                    return GetDBPropertyForString(item);
                case EnumProfileItemType.Memo:
                    return GetDBPropertyForMemo(item);
                case EnumProfileItemType.StringCombo:
                    return GetDBPropertyForCombo(item);
                case EnumProfileItemType.StringCheckList:
                    return GetDBPropertyForCheckList(item);
                case EnumProfileItemType.Color:
                    return GetDBPropertyForColor(item);
                case EnumProfileItemType.File:
                    return GetDBPropertyForFile(item);
                case EnumProfileItemType.Image:
                    return GetDBPropertyForImage(item);
                case EnumProfileItemType.Date:
                    return GetDBPropertyForDate(item);
                case EnumProfileItemType.Time:
                    return GetDBPropertyForTime(item);
                case EnumProfileItemType.DateTime:
                    return GetDBPropertyForDateTime(item);
                case EnumProfileItemType.Contact:
                    return GetDBPropertyForContact(item);
                case EnumProfileItemType.List:
                    return GetDBPropertyForList(item);
            }
            return string.Empty;
        }


        private string GetDBPropertyForBool(DBCTProfileItem item)
        {
            return GetDBPropNormal("int", "F" + item.UnicCode);
        }

        private string GetDBPropertyForDouble(DBCTProfileItem item)
        {
            return GetDBPropNormal("double", "F" + item.UnicCode);
        }

        private string GetDBPropertyForCountry(DBCTProfileItem item)
        {
            return GetDBPropForcePersist("CountryStruct", "F" + item.UnicCode);
        }

        private string GetDBPropertyForCity(DBCTProfileItem item)
        {
            return GetDBPropForcePersist("CityStruct", "F" + item.UnicCode);
        }

        private string GetDBPropertyForLocation(DBCTProfileItem item)
        {
            return GetDBPropForcePersist("LocationStruct", "F" + item.UnicCode);
        }

        private string GetDBPropertyForString(DBCTProfileItem item)
        {
            return GetDBPropPersian("string", "F" + item.UnicCode);
        }

        private string GetDBPropertyForMemo(DBCTProfileItem item)
        {
            return GetDBPropPersian("string", "F" + item.UnicCode);
        }

        private string GetDBPropertyForCombo(DBCTProfileItem item)
        {
            return GetDBPropPersian("string", "F" + item.UnicCode);
        }

        private string GetDBPropertyForCheckList(DBCTProfileItem item)
        {
            return GetDBPropPersian("string", "F" + item.UnicCode);
        }

        private string GetDBPropertyForColor(DBCTProfileItem item)
        {
            return GetDBPropNormal("double", "F" + item.UnicCode);
        }

        private string GetDBPropertyForFile(DBCTProfileItem item)
        {
            return GetDBPropForcePersist("FileStruct", "F" + item.UnicCode);
        }

        private string GetDBPropertyForImage(DBCTProfileItem item)
        {
            return GetDBPropForcePersist("ImageStruct", "F" + item.UnicCode);
        }

        private string GetDBPropertyForDate(DBCTProfileItem item)
        {
            return GetDBPropNormal("DateTime", "F" + item.UnicCode);
        }

        private string GetDBPropertyForTime(DBCTProfileItem item)
        {
            return GetDBPropNormal("int", "F" + item.UnicCode);
        }

        private string GetDBPropertyForDateTime(DBCTProfileItem item)
        {
            return GetDBPropNormal("DateTime", "F" + item.UnicCode);
        }

        private string GetDBPropertyForContact(DBCTProfileItem item)
        {
            return GetDBPropForcePersist("ContactStruct", "F" + item.UnicCode);
        }

        private string GetDBPropertyForList(DBCTProfileItem item)
        {
            return GetDBPropForcePersist("ListStruct", "F" + item.UnicCode);
        }

        private string GetDBMethods()
        {
            var sb = new StringBuilder();
            sb.Append(DBMethod);
            return sb.ToString();
        }

        #region Title

        private string _Title;

        [Size(32)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region TableName

        private string _TableName;

        [Size(19)]
        public string TableName
        {
            get { return _TableName; }
            set { SetPropertyValue("TableName", ref _TableName, value); }
        }

        #endregion

        #region ProfileGroup

        private DBCTProfileGroup _ProfileGroup;

        public DBCTProfileGroup ProfileGroup
        {
            get { return _ProfileGroup; }
            set { SetPropertyValue("ProfileGroup", ref _ProfileGroup, value); }
        }

        #endregion

        #region PropName

        private int _PropName;

        public int PropName
        {
            get { return _PropName; }
            set { SetPropertyValue("PropName", ref _PropName, value); }
        }

        #endregion

        #region Constants

        private const string DBClassStructure =
            @"
            using System;
            using DevExpress.Xpo;
            using DevExpress.Xpo.Metadata;
            using DevExpress.Data.Filtering;

            using POL.DB.Root;
            using POL.DB.P30Office;
            using POL.Lib.Utils;
            using POL.Lib.Interfaces;

            [DeferredDeletion(false)]
            [Persistent(""|DATABASENAME|"")]
            public class |DATABASENAME| : XPGUIDObject
            {
            
            public |DATABASENAME|(Session session): base(session){}
            
            

            |PROPERTIES|

            |METHODS|
            }
        ";

        private const string DBNormalProperty =
            @"
        #region |PROPNAME|
        private |TYPE| _|PROPNAME|;
        public |TYPE| |PROPNAME|
        {
            get { return _|PROPNAME|; }
            set { SetPropertyValue(""|PROPNAME|"", ref _|PROPNAME|, value); }
        }
        #endregion
        ";

        private const string DBForcePersistProperty =
            @"
        #region |PROPNAME|
        private |TYPE| _|PROPNAME|;
        [Persistent]
        public |TYPE| |PROPNAME|
        {
            get { return _|PROPNAME|; }
            set { SetPropertyValue(""|PROPNAME|"", ref _|PROPNAME|, value); }
        }
        #endregion
        ";

        private const string DBPersianProperty =
            @"
        #region |PROPNAME|
        private |TYPE| _|PROPNAME|;
        [PersianString]
        [Size(SizeAttribute.Unlimited)]
        public |TYPE| |PROPNAME|
        {
            get { return _|PROPNAME|; }
            set { SetPropertyValue(""|PROPNAME|"", ref _|PROPNAME|, value); }
        }
        #endregion
        ";


        private const string DBMethod =
            @"
        public |DATABASENAME| FindByOid(Guid oid)
        {
            return Session.FindObject<|DATABASENAME|>(new BinaryOperator(""Oid"", oid));
        }
        public |DATABASENAME| FindByOid(Session session,Guid oid)
        {
            return session.FindObject<|DATABASENAME|>(new BinaryOperator(""Oid"", oid));
        }
        ";

        #endregion
    }
}
