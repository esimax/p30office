using DevExpress.Xpf.Core;
using POC.Module.Contact.Models;
using POL.DB.P30Office;
using POL.Lib.Utils;


namespace POC.Module.Contact.Views
{
    public partial class WBasketOperation : DXWindow
    {
        public WBasketOperation(DBCTContactSelection currentBasket,EnumBoolOperationType operationType)
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var model = new Models.MBasketOperation(this, currentBasket, operationType);
                model.RequestClose +=
                    (s1, e1) =>
                    {
                        DialogResult = e1.DialogResult;
                        Close();
                    };
                DataContext = model;
                cbeBasketA.Focus();
                HelperLocalize.SetLanguageToDefault();
            };
        }
    }
}
