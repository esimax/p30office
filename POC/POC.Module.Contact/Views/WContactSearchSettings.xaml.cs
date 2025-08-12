using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using POL.Lib.Utils;


namespace POC.Module.Contact.Views
{
    public partial class WContactSearchSettings : DXWindow
    {
        public WContactSearchSettings()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var model = new Models.MContactSearchSettings(this);
                model.RequestClose +=
                    (s1, e1) =>
                    {
                        DialogResult = e1.DialogResult;
                        Close();
                    };
                DataContext = model;
                HelperLocalize.SetLanguageToDefault();
                
            };
        }

        public ListBoxEdit DynamicListBoxCat
        {
            get
            {
                return lbeCat;
            }
        }
    }
}
