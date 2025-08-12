using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POL.DB.P30Office;
using POL.Lib.Utils;
using DevExpress.Xpf.Editors;


namespace POC.Module.Contact.Views
{
    public partial class WContactAddEdit : DXWindow
    {
        public WContactAddEdit(DBCTContact data)
        {
            InitializeComponent();
            Loaded += (s, e) =>
                          {
                              var model = new Models.MContactAddEdit(this, data);
                              model.RequestClose +=
                                  (s1, e1) =>
                                  {
                                      DialogResult = e1.DialogResult;
                                      Close();
                                  };
                              DataContext = model;
                              firstFocused.Focus();
                              HelperLocalize.SetLanguageToDefault();
                              Task.Factory.StartNew(
                                  () =>
                                  {
                                      System.Threading.Thread.Sleep(200);
                                      Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                                          new Action(() => firstFocused.SelectAll()));
                                  });
                          };
        }
        public ListBoxEdit DynamicListBoxCat
        {
            get
            {
                return lbeCat;
            }
        }

        public DBCTContact AddedContact { get; set; }
    }
}
