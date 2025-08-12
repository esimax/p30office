using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.DB.P30Office.GL;
using POL.Lib.Common;
using POL.Lib.Interfaces;
using POL.WPF.DXControls;

namespace POC.Module.Phone.Views.FastContactUnit
{
    public partial class UPhone : UserControl, IValidateSaveFastContactModule
    {
        private IDatabase ADatabase { get; set; }
        private POCCore APOCCore { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private PhoneDecoder3Code Decoder { get; set; }

        public UPhone()
        {
            InitializeComponent();

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            var PhoneTitleList = (from n in POL.DB.P30Office.BT.DBBTPhoneTitle2.GetAll(ADatabase.Dxs)
                                  select n.Title).ToList();

            cbePhonetitle1.ItemsSource = PhoneTitleList;
            cbePhonetitle2.ItemsSource = PhoneTitleList;
            cbePhonetitle3.ItemsSource = PhoneTitleList;

            var dbcity = DBGLCity.FindByOid(ADatabase.Dxs, APOCCore.STCI.CurrentCityGuid);
            Decoder = new PhoneDecoder3Code(ALogger, ADatabase.Dxs, dbcity.Country.Oid, dbcity.Country.TeleCode, APOCCore.STCI.CurrentCityGuid, dbcity.PhoneCode, APOCCore.STCI.MobileLength, Convert.ToInt32(APOCCore.STCI.MobileStartingCode));

        }

        public bool Validate()
        {
            if (!string.IsNullOrEmpty(tePhone1.Text))
            {
                var dec = Decoder.DecodeData(tePhone1.Text, EnumCallType.CallIn);
                if (dec.HasError)
                {
                    POLMessageBox.ShowWarning("شماره تماس اول معتبر نمی باشد.", Window.GetWindow(this));
                    return false;
                }

                var dbp = string.IsNullOrEmpty(dec.CityCode) ?
                    DBCTPhoneBook.FindByPhoneAndCountry(ADatabase.Dxs, dec.Phone, dec.CountryOid) :
                    DBCTPhoneBook.FindByPhoneAndCityOid(ADatabase.Dxs, dec.CityOid, dec.Phone);

                if (dbp != null)
                {
                    POLMessageBox.ShowWarning("شماره تماس اول تكراری می باشد.", Window.GetWindow(this));
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(tePhone2.Text))
            {
                var dec = Decoder.DecodeData(tePhone2.Text, EnumCallType.CallIn);
                if (dec.HasError)
                {
                    POLMessageBox.ShowWarning("شماره تماس دوم معتبر نمی باشد.", Window.GetWindow(this));
                    return false;
                }

                var dbp = string.IsNullOrEmpty(dec.CityCode) ?
                    DBCTPhoneBook.FindByPhoneAndCountry(ADatabase.Dxs, dec.Phone, dec.CountryOid) :
                    DBCTPhoneBook.FindByPhoneAndCityOid(ADatabase.Dxs, dec.CityOid, dec.Phone);

                if (dbp != null)
                {
                    POLMessageBox.ShowWarning("شماره تماس اول تكراری می باشد.", Window.GetWindow(this));
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(tePhone3.Text))
            {
                var dec = Decoder.DecodeData(tePhone3.Text, EnumCallType.CallIn);
                if (dec.HasError)
                {
                    POLMessageBox.ShowWarning("شماره تماس سوم معتبر نمی باشد.", Window.GetWindow(this));
                    return false;
                }

                var dbp = string.IsNullOrEmpty(dec.CityCode) ?
                    DBCTPhoneBook.FindByPhoneAndCountry(ADatabase.Dxs, dec.Phone, dec.CountryOid) :
                    DBCTPhoneBook.FindByPhoneAndCityOid(ADatabase.Dxs, dec.CityOid, dec.Phone);

                if (dbp != null)
                {
                    POLMessageBox.ShowWarning("شماره تماس اول تكراری می باشد.", Window.GetWindow(this));
                    return false;
                }
            }
            return true;
        }

        public bool Save()
        {
            if (!string.IsNullOrEmpty(tePhone1.Text))
            {
                var dec = Decoder.DecodeData(tePhone1.Text, EnumCallType.CallIn);
                var ct = Contact as DBCTContact;
                if (ct != null)
                {
                    var dbp = new DBCTPhoneBook(ct.Session);
                    dbp.PhoneType = dec.Phone.StartsWith("0" + APOCCore.STCI.MobileStartingCode)
                        ? EnumPhoneType.Mobile
                        : EnumPhoneType.PhoneFax;
                    dbp.PhoneNumber = dec.Phone;
                    dbp.Country = DBGLCountry.FindByOid(ct.Session, dec.CountryOid);
                    dbp.City = DBGLCity.FindByOid(ct.Session, dec.CityOid);
                    dbp.Title = cbePhonetitle1.Text;
                    dbp.Contact = ct;
                    dbp.Save();
                }
            }
            if (!string.IsNullOrEmpty(tePhone2.Text))
            {
                var dec = Decoder.DecodeData(tePhone2.Text, EnumCallType.CallIn);
                var ct = Contact as DBCTContact;
                if (ct != null)
                {
                    var dbp = new DBCTPhoneBook(ct.Session);
                    dbp.PhoneType = dec.Phone.StartsWith("0" + APOCCore.STCI.MobileStartingCode)
                        ? EnumPhoneType.Mobile
                        : EnumPhoneType.PhoneFax;
                    dbp.PhoneNumber = dec.Phone;
                    dbp.Country = DBGLCountry.FindByOid(ct.Session, dec.CountryOid);
                    dbp.City = DBGLCity.FindByOid(ct.Session, dec.CityOid);
                    dbp.Title = cbePhonetitle2.Text;
                    dbp.Contact = ct;
                    dbp.Save();
                }
            }
            if (!string.IsNullOrEmpty(tePhone3.Text))
            {
                var dec = Decoder.DecodeData(tePhone3.Text, EnumCallType.CallIn);
                var ct = Contact as DBCTContact;
                if (ct != null)
                {
                    var dbp = new DBCTPhoneBook(ADatabase.Dxs);
                    dbp.PhoneType = dec.Phone.StartsWith("0" + APOCCore.STCI.MobileStartingCode)
                        ? EnumPhoneType.Mobile
                        : EnumPhoneType.PhoneFax;
                    dbp.PhoneNumber = dec.Phone;
                    dbp.Country = DBGLCountry.FindByOid(ct.Session, dec.CountryOid);
                    dbp.City = DBGLCity.FindByOid(ct.Session, dec.CityOid);
                    dbp.Title = cbePhonetitle3.Text;
                    dbp.Contact = ct;
                    dbp.Save();
                }
            }
            return true;
        }

        public object Contact { get; set; }
    }
}
