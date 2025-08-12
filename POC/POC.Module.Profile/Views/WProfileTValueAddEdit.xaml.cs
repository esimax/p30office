using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using POC.Module.Profile.Models;
using POL.DB.P30Office;

namespace POC.Module.Profile.Views
{
    public partial class WProfileTValueAddEdit
    {
        private MProfileTValueAddEdit Model { get; set; }

        public WProfileTValueAddEdit(DBCTProfileTable profileTable, DBCTProfileTValue selectedData, DBCTProfileTValue parentValue)
        {
            InitializeComponent();
            DynamicSelectedData = selectedData;
            DynamicTable = profileTable;
            DynamicParentValue = parentValue;
            Loaded += (s, e) =>
            {
                Model = new MProfileTValueAddEdit(this);
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

        public DBCTProfileTable DynamicTable { get; set; }
        public DBCTProfileTValue DynamicSelectedData { get; set; }
        public Window DynamicOwner { get { return this; } }
        public DBCTProfileTValue DynamicParentValue { get; set; }
    }
}
