using System;
using System.Windows;
using System.Windows.Threading;
using POC.Module.Email.Models;
using POL.DB.P30Office.BT;
using System.Threading.Tasks;


namespace POC.Module.Email.Views
{
    public partial class WEmailTitleAddEdit
    {
        private DBBTEmailTitle2 SelectedData { get; set; }
        private MEmailTitleAddEdit Model { get; set; }

        public WEmailTitleAddEdit(DBBTEmailTitle2 selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new MEmailTitleAddEdit(this);
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

        public DBBTEmailTitle2 DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
