using System.Windows;
using DevExpress.Xpf.Core;
using POC.Module.SMS.Models;
using POL.DB.P30Office;


namespace POC.Module.SMS.Views
{
    public partial class WTextStaticAddEdit : DXWindow
    {
        private DBSMTextStatic SelectedData { get; set; }
        private MTextStaticAddEdit Model { get; set; }

        public WTextStaticAddEdit(DBSMTextStatic selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new Models.MTextStaticAddEdit(this);
                DataContext = Model;
                firstFocused.Focus();
            };
        }

        public DBSMTextStatic DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
