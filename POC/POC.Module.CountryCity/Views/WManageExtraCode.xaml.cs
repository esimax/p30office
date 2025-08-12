using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using POL.Lib.Utils;


namespace POC.Module.CountryCity.Views
{
    public partial class WManageExtraCode : DXWindow
    {
        public WManageExtraCode()
        {
            InitializeComponent();
            
            
            Loaded += (s, e) =>
            {
                var model = new Models.MWManageExtraCode(this, this);
                DataContext = model;
                HelperLocalize.SetLanguageToDefault();
            };
        }

        public GridControl GetDynamicGrid()
        {
            return gcMain;
        }
    }
}
