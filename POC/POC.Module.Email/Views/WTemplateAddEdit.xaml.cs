using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using POC.Module.Email.Models;
using POL.DB.P30Office;


namespace POC.Module.Email.Views
{
    public partial class WTemplateAddEdit
    {
        private DBEMTemplate SelectedData { get; set; }
        private MTemplateAddEdit Model { get; set; }

        public WTemplateAddEdit(DBEMTemplate selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new MTemplateAddEdit(this);
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


        public DBEMTemplate DynamicSelectedData { get { return SelectedData; } }
    }
}
