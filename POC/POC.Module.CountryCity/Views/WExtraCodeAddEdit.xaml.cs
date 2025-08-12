using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POL.DB.P30Office.GL;


namespace POC.Module.CountryCity.Views
{
    public partial class WExtraCodeAddEdit : DXWindow
    {
        public WExtraCodeAddEdit(DBGLExtraCodes extracode)
        {
            InitializeComponent();
            ExtraCode = extracode;

            Loaded += (s, e) =>
            {
                var model = new Models.MWExtraCodeAddEdit(this, extracode, this);
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

        public DBGLExtraCodes ExtraCode { get; set; }
    }
}
