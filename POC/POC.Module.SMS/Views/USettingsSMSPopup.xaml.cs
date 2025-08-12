using System.Windows.Controls;

namespace POC.Module.SMS.Views
{
    public partial class USettingsSMSPopup : UserControl
    {
        public USettingsSMSPopup()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                DataContext = new Models.MSettingsSMSPopup(this);
            };
        }
    }
}
