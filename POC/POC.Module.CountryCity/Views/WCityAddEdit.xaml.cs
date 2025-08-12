using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POL.DB.P30Office.GL;


namespace POC.Module.CountryCity.Views
{
    public partial class WCityAddEdit : DXWindow
    {
        public WCityAddEdit(DBGLCountry country, DBGLCity data, bool allowPhoneCode)
        {
            InitializeComponent();
            AllowPhoneCode = allowPhoneCode;
            Country = country;

            Loaded += (s, e) =>
            {
                var model = new Models.MCityAddEdit(this, data,this);
                model.RequestClose +=
                    (s1, e1) =>
                    {
                        DialogResult = e1.DialogResult;
                        Close();
                    };
                DataContext = model;
                firstFocused.Focus();
                POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();
                Task.Factory.StartNew(
                    () =>
                    {
                        System.Threading.Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => firstFocused.SelectAll()));
                    });
            };
        }

        public DBGLCountry Country { get; set; }
        public bool AllowPhoneCode { get; set; }
    }
}
