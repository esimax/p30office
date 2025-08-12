using DevExpress.Xpf.Core;
using POC.Module.Contact.Models;
using POL.DB.P30Office;


namespace POC.Module.Contact.Views
{
    public partial class WContactSelect : DXWindow
    {
        
        public WContactSelect(DBCTContactCat  limitByCat)
        {
            SelectedCat = limitByCat;
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var model = new MContactSelect(this);
                DataContext = model;
            };
            if(SelectedCat!=null)
            {
                cbeContactCat.IsEnabled = false;
            }
        }


        public DBCTContact SelectedContact { get; set; }
        public DBCTContactCat SelectedCat { get; set; }
    }
}
