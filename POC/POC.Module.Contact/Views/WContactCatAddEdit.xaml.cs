using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Contact.Models;
using POL.DB.P30Office;


namespace POC.Module.Contact.Views
{
    public partial class WContactCatAddEdit : DXWindow
    {
        private DBCTContactCat SelectedData { get; set; }
        private MContactCatAddEdit Model { get; set; }

        public WContactCatAddEdit(DBCTContactCat selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new MContactCatAddEdit(this);
                DataContext = Model;
                firstFocused.Focus();
                POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();
                Task.Factory.StartNew(
                    () =>
                    {
                        System.Threading.Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => firstFocused.SelectAll()));
                    });
            };
        }

        public DBCTContactCat DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
