using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using POC.Module.Email.Models;
using POL.DB.P30Office;


namespace POC.Module.Email.Views
{
    public partial class WTemplateParameterAddEdit
    {
        private DBEMTempParams SelectedData { get; set; }
        private MTemplateParameterAddEdit Model { get; set; }

        public WTemplateParameterAddEdit(DBEMTempParams selectedData, DBEMTemplate parentTemp) 
        {
            InitializeComponent();
            SelectedData = selectedData;
            DynamicParentTemp = parentTemp;

            Loaded += (s, e) =>
            {
                Model = new MTemplateParameterAddEdit(this);
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

        public DBEMTempParams DynamicSelectedData { get { return SelectedData; } }
        public DBEMTemplate DynamicParentTemp { get; set; }
    }
}
