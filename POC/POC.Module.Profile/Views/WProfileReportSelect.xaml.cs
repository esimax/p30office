using DevExpress.Xpf.Core;
using POC.Module.Profile.Models;
using POL.DB.P30Office;

namespace POC.Module.Profile.Views
{
    public partial class WProfileReportSelect : DXWindow
    {
        private MProfileReportSelect Model { get; set; }

        public WProfileReportSelect()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Model = new MProfileReportSelect(this);
                DataContext = Model;
                POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();
            };
        }

        public DBCTProfileReport DynamicSelectedData { get; set; }
    }
}
