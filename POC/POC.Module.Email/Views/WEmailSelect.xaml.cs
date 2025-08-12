using POC.Module.Email.Models;
using POL.DB.P30Office;

namespace POC.Module.Email.Views
{
    public partial class WEmailSelect
    {
        
        public WEmailSelect()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var model = new MEmailSelect(this);
                DataContext = model;
            };
        }



        public DBCTEmail SelectedEmail { get; set; }
    }
}
