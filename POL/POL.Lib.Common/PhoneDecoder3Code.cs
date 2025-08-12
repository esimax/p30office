using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using POL.DB.P30Office.GL;
using POL.Lib.Interfaces;

namespace POL.Lib.Common
{
    public class PhoneDecoder3Code
    {
        public PhoneDecoder3Code(ILoggerFacade logger, Session session,
            Guid currentCountryOid, int currentCountryCode,
            Guid currentCityOid, int currentCityCode,
            int mobileLength, int mobileStarting)
        {
            ALoggerFacade = logger;
            Dxs = session;

            CurrentCountryCode = currentCountryCode;
            CurrentCountryOid = currentCountryOid;

            CurrentCityCode = currentCityCode;
            CurrentCityOid = currentCityOid;

            MobileLength = mobileLength;
            MobileStartingCode = mobileStarting;

            MStopwatch = new Stopwatch();

            UpdateCountryCode();
            UpdateCurrentCountryCityCodes();
        }

        private ILoggerFacade ALoggerFacade { get; }

        private Session Dxs { get; }

        private int CurrentCountryCode { get; }
        private Guid CurrentCountryOid { get; }
        private int CurrentCityCode { get; }
        private Guid CurrentCityOid { get; }
        private int CurrentCityPhoneLength { get; set; }

        private int MobileLength { get; }
        private int MobileStartingCode { get; }

        private List<CountryHolder> CountryCodes { get; set; }
        private List<CityHolder> CurrentCountryCityCodes { get; set; } 

        private Stopwatch MStopwatch { get; }
        public TimeSpan DecodeDuration { get; private set; }

        private void UpdateCountryCode()
        {
            try
            {
                CountryCodes = new List<CountryHolder>();
                var xpcCountry = DBGLCountry.GetAll(Dxs);
                foreach (var dbc in xpcCountry)
                {
                    if (dbc.TeleCode1 <= 0) continue;
                    var codestring = dbc.TeleCode.ToString(CultureInfo.InvariantCulture);
                    var findDuplicate = from n in CountryCodes where n.CountryCode == codestring select n;
                    if (findDuplicate.Any())
                    {
                        if (ALoggerFacade != null)
                            ALoggerFacade.Log(
                                string.Format("PhoneDecoder3Code Found Duplicate Country Code :{0}-{1} ", codestring,
                                    dbc.TitleEn), Category.Warn, Priority.High);
                    }
                    else
                        CountryCodes.Add(new CountryHolder {CountryOid = dbc.Oid, CountryCode = codestring});
                }
                if (ALoggerFacade == null) return;
                ALoggerFacade.Log(string.Format("PhoneDecoder3Code Country Count: {0}", CountryCodes.Count),
                    Category.Info, Priority.Medium);
                ALoggerFacade.Log(string.Format("PhoneDecoder3Code Current Country Code: {0}", CurrentCountryCode),
                    Category.Info, Priority.Medium);
                ALoggerFacade.Log(string.Format("PhoneDecoder3Code Current City Code: {0}", CurrentCityCode),
                    Category.Info, Priority.Medium);
            }
            catch (Exception ex)
            {
                if (ALoggerFacade == null) return;
                ALoggerFacade.Log("PhoneDecoder3Code Failed at UpdateCountryCode:", Category.Exception, Priority.High);
                ALoggerFacade.Log(ex.ToString(), Category.Exception, Priority.High);
            }
        }

        private void UpdateCurrentCountryCityCodes()
        {
            try
            {
                CurrentCountryCityCodes = new List<CityHolder>();
                var dbcountry = DBGLCountry.FindByOid(Dxs, CurrentCountryOid);
                var xpcCity = DBGLCity.GetByCountryWithTeleCode(Dxs, dbcountry, string.Empty);
                foreach (var dbc in xpcCity)
                {
                    CurrentCountryCityCodes.Add(new CityHolder
                    {
                        CityOid = dbc.Oid,
                        CityCode = dbc.PhoneCode.ToString(),
                        CityPhoneLen = dbc.PhoneLen
                    });
                    if (dbc.Oid == CurrentCityOid)
                        CurrentCityPhoneLength = dbc.PhoneLen;
                }
            }
            catch (Exception ex)
            {
                if (ALoggerFacade == null) return;
                ALoggerFacade.Log("PhoneDecoder3Code Failed at UpdateCurrentCountryCityCodes:", Category.Exception,
                    Priority.High);
                ALoggerFacade.Log(ex.ToString(), Category.Exception, Priority.High);
            }
        }


        public DecodedPhone DecodeData(string data, EnumCallType calltype)
        {
            MStopwatch.Restart();
            again1:
            var hasCountryCodeAtStart = false;
            var hasZiroAtStart = false;
            var rv = new DecodedPhone(data);
            try
            {
                var stemp = data;

                if (string.IsNullOrWhiteSpace(data))
                    return rv;
                stemp = stemp.TrimStart('0');
                hasZiroAtStart = stemp != data;

                if ((calltype == EnumCallType.CallIn) && (data.Length > CurrentCityPhoneLength) && !hasZiroAtStart)
                {
                    data = "0" + data;
                    goto again1;
                }
                if (stemp.StartsWith(MobileStartingCode.ToString(CultureInfo.InvariantCulture)) && data.StartsWith("00") &&
                    stemp.Length == MobileLength)
                {
                    rv.Phone = string.Format("0{0}", stemp);
                    rv.ExtraDialed = string.Empty;
                    rv.CountryCode = CurrentCountryCode.ToString(CultureInfo.InvariantCulture);
                    rv.CountryOid = CurrentCountryOid;
                    return rv;
                }

                #region Detecting Country

                var detectedCounty = new CountryHolder(); 

                if (data.StartsWith("00"))
                {
                    hasCountryCodeAtStart = true;
                    CountryCodes.Where(n => stemp.StartsWith(n.CountryCode)).ToList().ForEach(
                        e =>
                        {
                            if (e.CountryCode.Length > detectedCounty.CountryCode.Length)
                                if (stemp.Length >= 10 )
                                    detectedCounty = new CountryHolder
                                    {
                                        CountryOid = e.CountryOid,
                                        CountryCode = e.CountryCode
                                    };
                        });
                }

                if (detectedCounty.CountryOid == Guid.Empty)
                {
                    rv.CountryCode = CurrentCountryCode.ToString(CultureInfo.InvariantCulture);
                    rv.CountryOid = CurrentCountryOid;
                }
                else
                {
                    rv.CountryCode = detectedCounty.CountryCode;
                    rv.CountryOid = detectedCounty.CountryOid;
                }

                if (stemp.StartsWith(rv.CountryCode))
                {
                    hasCountryCodeAtStart = true;
                    stemp = stemp.Substring(rv.CountryCode.Length);
                }

                #endregion



                if (stemp.StartsWith(MobileStartingCode.ToString(CultureInfo.InvariantCulture)) && stemp[1] != '0' &&
                    stemp.Length >= MobileLength)
                {
                    var extra = string.Empty;
                    if (stemp.Length > MobileLength)
                        extra = stemp.Substring(MobileLength);
                    rv.Phone = string.Format("0{0}", stemp.Substring(0, MobileLength));
                    rv.ExtraDialed = extra;
                }
                else if (stemp.StartsWith(MobileStartingCode.ToString(CultureInfo.InvariantCulture)) &&
                         (hasCountryCodeAtStart || hasZiroAtStart))
                {
                    rv.Phone = string.Format("0{0}", stemp);
                }
                else
                {
                    if (stemp.Length <= CurrentCityPhoneLength)
                    {
                        rv.CityCode = CurrentCityCode.ToString(CultureInfo.InvariantCulture);
                        rv.CityOid = CurrentCityOid;
                        rv.Phone = stemp;
                    }
                    else
                    {

                        if (rv.CountryOid == CurrentCountryOid)
                        {
                            var q = from n in CurrentCountryCityCodes
                                where stemp.StartsWith(n.CityCode) &&
                                      (stemp.Length >= n.CityCode.Length + n.CityPhoneLen)
                                select n;
                            if (q.Any() && (hasCountryCodeAtStart || hasZiroAtStart))
                            {
                                var city = q.First();
                                rv.CityCode = city.CityCode;
                                rv.CityOid = city.CityOid;
                                stemp = stemp.Substring(city.CityCode.Length); 
                                if (stemp.Length > city.CityPhoneLen)
                                    rv.ExtraDialed = stemp.Substring(city.CityPhoneLen);
                                rv.Phone = stemp.Substring(0, city.CityPhoneLen);
                            }
                            else
                            {
                                rv.CityOid = CurrentCityOid;
                                rv.CityCode = CurrentCityCode.ToString();
                                if (stemp.StartsWith(rv.CityCode))
                                    stemp = stemp.Substring(rv.CityCode.Length);
                                if (stemp.Length > CurrentCityPhoneLength)
                                    rv.ExtraDialed = stemp.Substring(CurrentCityPhoneLength);
                                rv.Phone = stemp.Substring(0, CurrentCityPhoneLength);
                            }
                        }
                        else
                        {

                            var otherCountryCityCodes = new List<CityHolder>();
                            var dbcountry = DBGLCountry.FindByOid(Dxs, rv.CountryOid);
                            var xpcCity = DBGLCity.GetByCountryWithTeleCode(Dxs, dbcountry, string.Empty);
                            foreach (var dbc in xpcCity)
                            {
                                otherCountryCityCodes.Add(new CityHolder
                                {
                                    CityOid = dbc.Oid,
                                    CityCode = dbc.PhoneCode.ToString(),
                                    CityPhoneLen = dbc.PhoneLen
                                });
                                if (dbc.Oid == CurrentCityOid)
                                    CurrentCityPhoneLength = dbc.PhoneLen;
                            }

                            var q = from n in otherCountryCityCodes
                                where stemp.StartsWith(n.CityCode) &&
                                      (stemp.Length >= n.CityCode.Length + n.CityPhoneLen)
                                select n;
                            if (q.Any() && (hasCountryCodeAtStart || hasZiroAtStart))
                            {
                                var city = q.First();
                                rv.CityCode = city.CityCode;
                                rv.CityOid = city.CityOid;
                                stemp = stemp.Substring(city.CityCode.Length); 
                                if (stemp.Length > city.CityPhoneLen)
                                    rv.ExtraDialed = stemp.Substring(city.CityPhoneLen);
                                rv.Phone = stemp.Substring(0, city.CityPhoneLen);
                            }
                            else
                            {
                                rv.Phone = stemp;
                            }
                        }





                    }
                }
                MStopwatch.Stop();
                DecodeDuration = MStopwatch.Elapsed;
            }
            catch (Exception)
            {
                MStopwatch.Stop();
                DecodeDuration = MStopwatch.Elapsed;
                rv.HasError = true;
            }

            return rv;
        }
    }
}
