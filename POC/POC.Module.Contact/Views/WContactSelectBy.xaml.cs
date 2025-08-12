using DevExpress.Xpf.Core;
using POC.Module.Contact.Models;
using POL.Lib.Interfaces;


namespace POC.Module.Contact.Views
{
    public partial class WContactSelectBy : DXWindow
    {
        private MContactSelectBy Model { get; set; }

        public WContactSelectBy()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                Model = new MContactSelectBy(this);
                DataContext = Model;
                firstFocused.Focus();
                POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();
            };
        }

        public ContactSelectByResult DynamicResult { get; set; }
    }
}
