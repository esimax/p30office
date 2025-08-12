using System.Windows.Controls;

namespace POC.Module.ABCalendar.Views
{
    public partial class USettingPrayTime : UserControl
    {
        public USettingPrayTime()
        {
            InitializeComponent();
            Loaded += (s, e) =>
                          {
                              DataContext = new Models.MSettingPrayTime(this);
                          };
        }
    }
}
