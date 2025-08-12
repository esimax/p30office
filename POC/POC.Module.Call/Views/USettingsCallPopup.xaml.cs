using System.Windows.Controls;

namespace POC.Module.Call.Views
{
    public partial class USettingsCallPopup : UserControl
    {
        public USettingsCallPopup()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                DataContext = new Models.MSettingsCallPopup(this);
            };
        }
    }
}
