using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using POL.DB.P30Office;
using POL.Lib.Utils;


namespace POC.Module.Contact.Views
{
    public partial class WContactToBasket : DXWindow
    {
        public WContactToBasket(DBCTContactSelection sourceBasket, CriteriaOperator co, List<Guid> guids)
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var model = new Models.MContactToBasket(this, sourceBasket, co, guids);
                model.RequestClose +=
                    (s1, e1) =>
                    {
                        DialogResult = e1.DialogResult;
                        Close();
                    };
                DataContext = model;
                firstFocused.Focus();
                HelperLocalize.SetLanguageToDefault();
            };
        }
    }
}
