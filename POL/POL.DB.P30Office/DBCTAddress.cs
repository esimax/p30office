using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.P30Office.GL;
using POL.DB.Root;
using POL.Lib.Interfaces.PObjects;

namespace POL.DB.P30Office
{
    public class DBCTAddress : XPGUIDLogableObject 
    {
        #region MapImageByte

        [Delayed(true)]
        [DisplayName("نقشه")]
        public byte[] MapImageByte
        {
            get { return GetDelayedPropertyValue<byte[]>("MapImageByte"); }
            set { SetDelayedPropertyValue("MapImageByte", value); }
        }

        #endregion

        [NonPersistent]
        [DisplayName("آدرس كامل")]
        public string FullAddress
        {
            get
            {
                var rv = string.Empty;
                if (City != null)
                {
                    rv = string.Format("{0} - {1}", City.TitleXX, rv);
                    if (!string.IsNullOrEmpty(City.StateTitle))
                    {
                        rv = string.Format("{0} - {1}", City.StateTitle, rv);
                    }
                    if (City.Country != null)
                    {
                        if (!string.IsNullOrEmpty(City.Country.TitleXX))
                            rv = string.Format("{0} - {1}", City.Country.TitleXX, rv);
                    }
                }
                if (!string.IsNullOrWhiteSpace(Area))
                    rv += string.Format("{0} - ", Area);
                if (!string.IsNullOrWhiteSpace(Address))
                    rv += string.Format("{0} - ", Address.Replace(Environment.NewLine, ","));
                if (!string.IsNullOrWhiteSpace(ZipCode))
                    rv += string.Format("كد پستی : {0} ", ZipCode);
                if (!string.IsNullOrWhiteSpace(POBox))
                    rv += string.Format("صندوق پستی : {0} ", POBox);

                if (!string.IsNullOrWhiteSpace(Title))
                {
                    rv = string.Format("{0} : {1}", Title, rv);
                }
                return rv.Trim().Trim('-');
            }
        }


        public static DBCTAddress FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTAddress>(new BinaryOperator("Oid", oid));
        }

        public static XPCollection<DBCTAddress> GetByContact(Session session, Guid contactOid)
        {
            return new XPCollection<DBCTAddress>(session, new BinaryOperator("Contact.Oid", contactOid));
        }


        internal static Address PopulateToFake(DBCTAddress dbaddress)
        {
            var rv = new Address();
            rv.Area = dbaddress.Area;
            rv.City = dbaddress.City != null ? dbaddress.City.TitleXX : string.Empty;
            rv.Country = dbaddress.City != null ? dbaddress.City.Country.TitleXX : string.Empty;
            rv.Latitude = dbaddress.Latitude;
            rv.Longitude = dbaddress.Longitude;
            rv.Note = dbaddress.Note;
            rv.POBox = dbaddress.POBox;
            rv.State = dbaddress.City != null ? dbaddress.City.StateTitle : string.Empty;
            rv.Street = dbaddress.Address;
            rv.Title = dbaddress.Title;
            rv.ZipCode = dbaddress.ZipCode;
            return rv;
        }

        #region Design

        public DBCTAddress(Session session) : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Latitude = -1;
            Longitude = -1;
        }

        #endregion

        #region Title

        private string _Title;

        [Size(32)]
        [DisplayName("عنوان آدرس")]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region City

        private DBGLCity _City;

        [DisplayName("شهر")]
        public DBGLCity City
        {
            get { return _City; }
            set { SetPropertyValue("City", ref _City, value); }
        }

        #endregion

        #region Area

        private string _Area;

        [Size(32)]
        [DisplayName("محله")]
        public string Area
        {
            get { return _Area; }
            set { SetPropertyValue("Area", ref _Area, value); }
        }

        #endregion

        #region Address

        private string _Address;

        [Size(1024)]
        [DisplayName("آدرس")]
        public string Address
        {
            get { return _Address; }
            set { SetPropertyValue("Address", ref _Address, value); }
        }

        #endregion

        #region POBox

        private string _POBox;

        [Size(32)]
        [DisplayName("صندوق پستی")]
        public string POBox
        {
            get { return _POBox; }
            set { SetPropertyValue("POBox", ref _POBox, value); }
        }

        #endregion

        #region ZipCode

        private string _ZipCode;

        [Size(32)]
        [DisplayName("كد پستی")]
        public string ZipCode
        {
            get { return _ZipCode; }
            set { SetPropertyValue("ZipCode", ref _ZipCode, value); }
        }

        #endregion

        #region Note

        private string _Note;

        [Size(1024)]
        [DisplayName("نكته")]
        public string Note
        {
            get { return _Note; }
            set { SetPropertyValue("Note", ref _Note, value); }
        }

        #endregion

        #region Latitude

        private double _Latitude;

        [DisplayName("عرض جغرافیایی")]
        public double Latitude
        {
            get { return _Latitude; }
            set { SetPropertyValue("Latitude", ref _Latitude, value); }
        }

        #endregion

        #region Longitude

        private double _Longitude;

        [DisplayName("طول جغرافیایی")]
        public double Longitude
        {
            get { return _Longitude; }
            set { SetPropertyValue("Longitude", ref _Longitude, value); }
        }

        #endregion

        #region ZoomLevel

        private int _ZoomLevel;

        [DisplayName("بزرگنمایی نقشه")]
        public int ZoomLevel
        {
            get { return _ZoomLevel; }
            set { SetPropertyValue("ZoomLevel", ref _ZoomLevel, value); }
        }

        #endregion

        #region Contact

        private DBCTContact _Contact;

        [DisplayName("پرونده")]
        [Association("CTContact_CTAddresses")]
        public DBCTContact Contact
        {
            get { return _Contact; }
            set { SetPropertyValue("Contact", ref _Contact, value); }
        }

        #endregion
    }
}
