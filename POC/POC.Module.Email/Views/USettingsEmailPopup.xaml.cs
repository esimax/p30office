using System.Windows.Controls;

namespace POC.Module.Email.Views
{
    public partial class USettingsEmailPopup : UserControl
    {
        public USettingsEmailPopup()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                DataContext = new Models.MSettingsEmailPopup(this);
            };
        }
    }
}
