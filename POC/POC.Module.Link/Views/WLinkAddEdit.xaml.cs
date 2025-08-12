using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using POL.DB.P30Office;
using POL.Lib.Utils;


namespace POC.Module.Link.Views
{
    public partial class WLinkAddEdit : DXWindow
    {
        public WLinkAddEdit(DBCTContact contact, DBCTContactRelation relation)
        {
            InitializeComponent();
            SelectedContact = contact;
            SelectedRelation = relation;

            Loaded += (s, e) =>
            {
                var model = new Models.MLinkAddEdit(this);
                DataContext = model;
                firstFocused.Focus();
                HelperLocalize.SetLanguageToDefault();
                Task.Factory.StartNew(
                    () =>
                    {
                        System.Threading.Thread.Sleep(500);
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => firstFocused.SelectAll()));
                    });
            };
        }

        public DBCTContact SelectedContact { get; set; }
        public DBCTContactRelation SelectedRelation { get; set; }
        public GridControl DynamicDynamicGrid { get { return gridContact; } }

    }
}
