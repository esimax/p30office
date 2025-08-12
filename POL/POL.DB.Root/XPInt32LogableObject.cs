using System;
using DevExpress.Xpo;

namespace POL.DB.Root
{
    [NonPersistent, MemberDesignTimeVisibility(false)]
    public abstract class XPInt32LogableObject : XPInt32Object
    {
        protected override void OnSaving()
        {
            base.OnSaving();
            if (Services.EnableLoogable)
            {
                var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                if (DateModified == null)
                    DateModified = date;
                else if (DateModified.Value.Ticks != date.Ticks)
                    DateModified = date;

                UserModified = Services.GetProperUserName();
            }
        }

        #region CTOR


        protected XPInt32LogableObject(Session session)
            : base(session)
        {
        }


        public override void AfterConstruction()
        {
            base.AfterConstruction();
            if (Services.EnableLoogable)
            {
                DateCreated = DateTime.Now;
                UserCreated = Services.GetProperUserName();
            }
        }

        #endregion

        #region DateCreated

        private DateTime? _DateCreated;

        [DisplayName("تاریخ ثبت")]
        public DateTime? DateCreated
        {
            get { return _DateCreated; }
            set
            {
                if (value == _DateCreated) return;
                if (value == DateTime.MinValue)
                    SetPropertyValue("DateCreated", ref _DateCreated, null);
                else
                    SetPropertyValue("DateCreated", ref _DateCreated, value);
            }
        }

        #endregion

        #region DateModified

        private DateTime? _DateModified;

        [DisplayName("تاریخ تغییر")]
        public DateTime? DateModified
        {
            get { return _DateModified; }
            set
            {
                if (value == _DateModified) return;
                if (value == DateTime.MinValue)
                    SetPropertyValue("DateModified", ref _DateModified, null);
                else
                    SetPropertyValue("DateModified", ref _DateModified, value);
            }
        }

        #endregion

        #region UserCreated

        private string _UserCreated;

        [Size(128)]
        [DisplayName("كاربر ثبت كننده")]
        public string UserCreated
        {
            get { return _UserCreated; }
            set
            {
                if (value == _UserCreated) return;
                SetPropertyValue("UserCreated", ref _UserCreated, value);
            }
        }

        #endregion

        #region UserModified

        private string _UserModified;

        [DisplayName("كاربر ویرایش كننده")]
        [Size(128)]
        public string UserModified
        {
            get { return _UserModified; }
            set
            {
                if (value == _UserModified) return;
                SetPropertyValue("UserModified", ref _UserModified, value);
            }
        }

        #endregion
    }
}
