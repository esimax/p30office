using System.Windows;
using System.Windows.Controls;
using POL.DB.P30Office;

namespace POC.Module.Contact.Views
{
    public partial class WContactFastCreation : Window
    {

        public WContactFastCreation(DBCTContact contact)
        {
            InitializeComponent();
            DynamicContact = contact;
            DynamicTheStackPanel = TheStackPanel;

            Loaded += (s, e) =>
            {
                var model = new Models.MContactFastCreation(this);
                DataContext = model;
            };
        }


        public DBCTContact DynamicContact { get; set; }
        public StackPanel DynamicTheStackPanel { get; set; }

        public DBCTContact SelectedContact { get; set; }
    }
}
