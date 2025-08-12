using DevExpress.Xpf.Core;
using POL.DB.P30Office.GL;
using POL.Lib.Utils;


namespace POC.Module.CountryCity.Views
{
    public partial class WCountryCity : DXWindow
    {
        public WCountryCity(DBGLCountry data,bool allowPhoneCode)
        {
            InitializeComponent();
            AllowPhoneCode = allowPhoneCode;
            
            Loaded += (s, e) =>
            {
                var model = new Models.MWCountryCity(this, data, this);
                model.RequestClose +=
                    (s1, e1) =>
                    {
                        DialogResult = e1.DialogResult;
                        Close();
                    };
                DataContext = model;
                
                HelperLocalize.SetLanguageToDefault();
            };
        }

        
        public bool AllowPhoneCode { get; set; }
        public DevExpress.Xpf.Grid.GridControl GetDynamicGrid()
        {
            return gcMain;
        }

        public DBGLCity SelectedData { get; set; }
    }
}
