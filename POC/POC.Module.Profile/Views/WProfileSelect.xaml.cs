using System.Windows.Controls;
using DevExpress.Xpf.Core;
using POC.Module.Profile.Models;
using POL.Lib.Interfaces;


namespace POC.Module.Profile.Views
{
    public partial class WProfileSelect : DXWindow
    {
        public WProfileSelect(EnumProfileItemType? type)
        {
            DynamicProfileItemType = type;
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var model = new MProfileSelect(this);
                DataContext = model;
            };
        }

        public ProfileTreeItem DynamicSelectedData { get; set; }
        public ListBox DynamicListBox { get { return lbeMain; } }
        public EnumProfileItemType? DynamicProfileItemType { get; set; }
    }
}
