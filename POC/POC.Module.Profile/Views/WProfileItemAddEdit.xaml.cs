using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Profile.Models;
using POL.DB.P30Office;
using POL.Lib.Utils;

namespace POC.Module.Profile.Views
{
    public partial class WProfileItemAddEdit : DXWindow
    {
        private MProfileItemAddEdit Model { get; set; }

        public WProfileItemAddEdit(DBCTProfileGroup profileGroup, DBCTProfileItem selectedData)
        {
            InitializeComponent();
            DynamicSelectedData = selectedData;
            DynamicProfileGroup = profileGroup;
            Loaded += (s, e) =>
            {
                Model = new MProfileItemAddEdit(this);
                DataContext = Model;
                firstFocused.Focus();
                POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();
                Task.Factory.StartNew(
                    () =>
                    {
                        System.Threading.Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => firstFocused.SelectAll()));
                        HelperUtils.DoDispatcher(
                            () =>
                            {
                                HelperUtils.Try(
                                    () =>
                                    {
                                        lbeType.ScrollIntoView(lbeType.SelectedItem);
                                    });
                            });
                    });
            };
        }

        public DBCTProfileGroup DynamicProfileGroup { get; set; }

        public DBCTProfileItem DynamicSelectedData { get; set; }
    }
}
