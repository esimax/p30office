using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Core;
using POC.Module.Contact.Models;
using POL.DB.P30Office;


namespace POC.Module.Contact.Views
{
    public partial class WContactCatSelect : DXWindow
    {
        private MContactCatSelect Model { get; set; }
        public WContactCatSelect(bool isSingleSelect )
        {
            IsSingleSelect = isSingleSelect;
            InitializeComponent();
            Loaded += (s, e) =>
            {
                Model = new MContactCatSelect(this);
                DataContext = Model;
                POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();

                if (isSingleSelect)
                    lbMain.ItemTemplate = FindResource("TextBlockType") as DataTemplate;
            };
        }

        public ObservableCollection<CategoryHolder> CategoryList { get { return Model.CategoryList; } }
        public DBCTContactCat SelectedContactCat { get; set; }
        public bool IsSingleSelect { get; set; }
    }
}
