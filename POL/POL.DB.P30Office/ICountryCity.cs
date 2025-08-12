using System;
using DevExpress.Xpo;
using POL.DB.P30Office.GL;

namespace POL.DB.P30Office
{
    public interface ICountryCity
    {
        DBGLCountry DefaultCountry { get; set; }
        DBGLCity DefaultCity { get; set; }
        XPCollection<DBGLCountry> AllCountries { get; set; }

        void RaiseOnManageCountry();
        void SubscribeOnManageCountry(Action action);

        DBGLCountry RaiseOnSelectCountry();
        void SubscribeOnSelectCountry(Func<DBGLCountry> func);


        void RaiseOnManageCity(DBGLCountry country);
        void SubscribeOnManageCity(Action<DBGLCountry> action);

        DBGLCity RaiseOnSelectCity(DBGLCountry country);
        void SubscribeOnSelectCity(Func<DBGLCountry, DBGLCity> func);
    }
}
