using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.BT;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Interfaces.PObjects;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTContact : XPGUIDLogableObject 
    {
        #region Design

        public DBCTContact(Session session) : base(session)
        {
        }

        #endregion

        #region Link [n-n] - Contact Category

        [Association("CTContactCats_CTContacts")]
        [DisplayName("دسته ها")]
        public XPCollection<DBCTContactCat> Categories
        {
            get { return GetCollection<DBCTContactCat>("Categories"); }
        }

        #endregion

        #region Link - Selections

        [Association("CTContactSelections_CTContacts")]
        [DisplayName("سبد ها")]
        public XPCollection<DBCTContactSelection> Selections
        {
            get { return GetCollection<DBCTContactSelection>("Selections"); }
        }

        #endregion

        #region Link - Phones

        [Association("CTContact_CTPhoneBooks"), Aggregated]
        [DisplayName("شماده های تماس")]
        public XPCollection<DBCTPhoneBook> Phones
        {
            get { return GetCollection<DBCTPhoneBook>("Phones"); }
        }

        #endregion

        #region Link - Addresses

        [Association("CTContact_CTAddresses"), Aggregated]
        [DisplayName("آدرس ها")]
        public XPCollection<DBCTAddress> Addresses
        {
            get { return GetCollection<DBCTAddress>("Addresses"); }
        }

        #endregion

        #region Link - Emails

        [Association("CTContact_CTEmails"), Aggregated]
        [DisplayName("ایمیل ها")]
        public XPCollection<DBCTEmail> Emails
        {
            get { return GetCollection<DBCTEmail>("Emails"); }
        }

        #endregion

        #region Link - ProfileValues

        [Association("CTContact_CTProfileValues"), Aggregated]
        [DisplayName("فیلد ها")]
        public XPCollection<DBCTProfileValue> ProfileValues
        {
            get { return GetCollection<DBCTProfileValue>("ProfileValues"); }
        }

        #endregion

        #region Link - Factors

        [Association("CTContact_ACFactors"), Aggregated]
        public XPCollection<DBACFactor> Factores
        {
            get { return GetCollection<DBACFactor>("Factores"); }
        }

        #endregion


        [NonPersistent]
        public string Cats
        {
            get
            {
                if (Categories.Count == 0)
                    return string.Empty;
                return Categories.Select(n => n.Title).Aggregate((a, b) => a + ", " + b);
            }
        }


        public static int GetNextCode(Session dxs)
        {
            try
            {
                var maxCode = (int) dxs.Evaluate<DBCTContact>(CriteriaOperator.Parse("Max(Code)"), null);
                return maxCode + 1;
            }
            catch
            {
                return 0;
            }
        }


        public static DBCTContact FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTContact>(new BinaryOperator("Oid", oid));
        }

        public static DBCTContact FindByCodeAndOrTitle(Session session, int? code, string title)
        {
            var go = new GroupOperator();
            if (code != null)
                go.Operands.Add(new BinaryOperator("Code", Convert.ToInt32(code), BinaryOperatorType.Equal));
            if (title != null)
                if (!string.IsNullOrWhiteSpace(title))
                    go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title),
                        BinaryOperatorType.Equal));

            return session.FindObject<DBCTContact>(go);
        }

        public static DBCTContact FindDuplicateCodeExcept(Session session, DBCTContact exceptContact, int code)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Code", code, BinaryOperatorType.Equal));
            return session.FindObject<DBCTContact>(go);
        }

        public static DBCTContact FindDuplicateTitleExcept(Session session, DBCTContact exceptContact, string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCTContact>(go);
        }

        public static XPCollection<DBCTContact> GetAll(Session session)
        {
            return new XPCollection<DBCTContact>(session);
        }

        public int AddProfileObjectToContact(object o)
        {
            var list = new List<DBCTProfileItem>();
            if (o is DBCTProfileItem)
            {
                list.Add((DBCTProfileItem) o);
            }
            else if (o is DBCTProfileGroup)
            {
                var group = (DBCTProfileGroup) o;
                var items = DBCTProfileItem.GetAll(Session, @group.Oid);
                items.ToList().ForEach(list.Add);
            }
            else if (o is DBCTProfileRoot)
            {
                var root = o as DBCTProfileRoot;
                var groups = DBCTProfileGroup.GetAll(Session, root.Oid);
                groups.ToList().ForEach(
                    g =>
                    {
                        var items = DBCTProfileItem.GetAll(Session, g.Oid);
                        items.ToList().ForEach(list.Add);
                    });
            }

            var existList = DBCTProfileValue.GetAll(Session, Oid);
            var guidExist = from n in existList select n.ProfileItem.Oid;
            var guidNew = from n in list select n.Oid;
            var guitTobeAdded = guidNew.Except(guidExist);
            var addedCount = 0;

            var aDataFieldManager = ServiceLocator.Current.GetInstance<IDataFieldManager>();
            using (var uow = new UnitOfWork(Session.DataLayer))
            {
                var contact = FindByOid(uow, Oid);
                guitTobeAdded.ToList().ForEach(
                    n =>
                    {
                        var dbpi = DBCTProfileItem.FindByOid(uow, n);
                        var dbpv = new DBCTProfileValue(uow)
                        {
                            Contact = contact,
                            Order = dbpi.Order,
                            ProfileItem = dbpi
                        };
                        var df = aDataFieldManager.FindByType(dbpi.ItemType);
                        df.SetValueToDefault(dbpv, dbpi);
                        dbpv.Save();
                        addedCount++;
                    });
                uow.CommitChanges();
            }
            return addedCount;
        }

        protected override XPCollection<T> CreateCollection<T>(XPMemberInfo property)
        {
            var collection = base.CreateCollection<T>(property);
            if (property.Name == "Selections")
            {
                collection.Sorting = new SortingCollection(new SortProperty("Title", SortingDirection.Ascending));
            }
            if (property.Name == "Infos")
            {
                collection.Sorting = new SortingCollection(new SortProperty("Order", SortingDirection.Ascending));
            }

            return collection;
        }

        public override string ToString()
        {
            return Title;
        }


        public static Contact PopulateToFake(DBCTContact data)
        {
            var rv = new Contact();
            rv.Code = data.Code;
            rv.Title = data.Title;
            rv.CCText0 = data.CCText0;
            rv.CCText1 = data.CCText1;
            rv.CCText2 = data.CCText2;
            rv.CCText3 = data.CCText3;
            rv.CCText4 = data.CCText4;
            rv.CCText5 = data.CCText5;
            rv.CCText6 = data.CCText6;
            rv.CCText7 = data.CCText7;
            rv.CCText8 = data.CCText8;
            rv.CCText9 = data.CCText9;

            foreach (var dbaddress in data.Addresses)
                rv.Address.Add(DBCTAddress.PopulateToFake(dbaddress));

            foreach (var dbctContactCat in data.Categories)
                rv.Categories.Add(DBCTContactCat.PopulateToFake(dbctContactCat));

            foreach (var dbemail in data.Emails)
                rv.Emails.Add(DBCTEmail.PopulateToFake(dbemail));

            foreach (var dbphone in data.Phones)
                rv.Phones.Add(DBCTPhoneBook.PopulateToFake(dbphone));


            return rv;
        }

        #region Code

        private int _Code;

        [DisplayName("كد پرونده")]
        public int Code
        {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        #endregion

        #region Title

        private string _Title;

        [PersianString]
        [Size(128)]
        [DisplayName("عنوان پرونده")]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region CCText1

        private string _CCText1;
        [Size(256)]
        [DisplayName("ستون اختیاری اول")]
        public string CCText1
        {
            get { return _CCText1; }
            set { SetPropertyValue("CCText1", ref _CCText1, value); }
        }

        #endregion

        #region CCText2

        private string _CCText2;
        [Size(256)]
        [DisplayName("ستون اختیاری دوم")]
        public string CCText2
        {
            get { return _CCText2; }
            set { SetPropertyValue("CCText2", ref _CCText2, value); }
        }

        #endregion

        #region CCText3

        private string _CCText3;
        [DisplayName("ستون اختیاری سوم")]
        [Size(256)]
        public string CCText3
        {
            get { return _CCText3; }
            set { SetPropertyValue("CCText3", ref _CCText3, value); }
        }

        #endregion

        #region CCText4

        private string _CCText4;
        [Size(256)]
        [DisplayName("ستون اختیاری چهارم")]
        public string CCText4
        {
            get { return _CCText4; }
            set { SetPropertyValue("CCText4", ref _CCText4, value); }
        }

        #endregion

        #region CCText5

        private string _CCText5;
        [Size(256)]
        [DisplayName("ستون اختیاری پنجم")]
        public string CCText5
        {
            get { return _CCText5; }
            set { SetPropertyValue("CCText5", ref _CCText5, value); }
        }

        #endregion

        #region CCText6

        private string _CCText6;
        [Size(256)]
        [DisplayName("ستون اختیاری ششم")]
        public string CCText6
        {
            get { return _CCText6; }
            set { SetPropertyValue("CCText6", ref _CCText6, value); }
        }

        #endregion

        #region CCText7

        private string _CCText7;
        [Size(256)]
        [DisplayName("ستون اختیاری هفتم")]
        public string CCText7
        {
            get { return _CCText7; }
            set { SetPropertyValue("CCText7", ref _CCText7, value); }
        }

        #endregion

        #region CCText8

        private string _CCText8;
        [Size(256)]
        [DisplayName("ستون اختیاری هشتم")]
        public string CCText8
        {
            get { return _CCText8; }
            set { SetPropertyValue("CCText8", ref _CCText8, value); }
        }

        #endregion

        #region CCText9

        private string _CCText9;
        [Size(256)]
        [DisplayName("ستون اختیاری نهم")]
        public string CCText9
        {
            get { return _CCText9; }
            set { SetPropertyValue("CCText9", ref _CCText9, value); }
        }

        #endregion

        #region CCText0

        private string _CCText0;
        [Size(256)]
        [DisplayName("ستون اختیاری دهم")]
        public string CCText0
        {
            get { return _CCText0; }
            set { SetPropertyValue("CCText0", ref _CCText0, value); }
        }

        #endregion
    }
}
