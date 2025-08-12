using System;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.SMS.Models
{
    public class SMSPhoneHolder : NotifyObjectBase
    {

        #region Selected
        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (value == _Selected)
                    return;

                _Selected = value;
                RaisePropertyChanged("Selected");
            }
        }
        #endregion

        public int Code { get; set; }
        public string Title { get; set; }
        #region PhoneNumber
        private string _PhoneNumber;
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set
            {
                _PhoneNumber = value;
                CompanyType = EnumPhoneCompanyType.Unknown;

                if (_PhoneNumber.StartsWith("091"))
                    CompanyType = EnumPhoneCompanyType.HamrahAval;
                if (_PhoneNumber.StartsWith("092"))
                    CompanyType = EnumPhoneCompanyType.RighTel;
                if (_PhoneNumber.StartsWith("093"))
                    CompanyType = EnumPhoneCompanyType.IranCell;
            }
        }
        #endregion

        public DBCTPhoneBook PhoneBook { get; set; }

        public Guid ContactOid { get; set; }
        public EnumPhoneCompanyType CompanyType { get; set; }

        public string Body { get; set; }
    }
}
