using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POL.DB.P30Office.GL;
using POL.Lib.Utils;


namespace POC.Module.CountryCity.Views
{
    public partial class WCountryEdit : DXWindow
    {
        public WCountryEdit(DBGLCountry data)
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var model = new Models.MCountryAddEdit(this, data);
                model.RequestClose +=
                    (s1, e1) =>
                    {
                        DialogResult = e1.DialogResult;
                        Close();
                    };
                DataContext = model;
                firstFocused.Focus();
                HelperLocalize.SetLanguageToDefault();
                Task.Factory.StartNew(
                    () =>
                    {
                        System.Threading.Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => firstFocused.SelectAll()));
                    });
            };
        }
    }
}
