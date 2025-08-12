using DevExpress.Xpf.Grid;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace POC.Module.Profile.Views
{
    public partial class UProfile : UserControl, IModuleRibbon
    {
        public UProfile()
        {
            InitializeComponent();
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return contactRibbon;
        }
        public void LoadChildRibbons()
        {
        }
        public void UnloadChildRibbons()
        {
        }
        #endregion

        public TreeListControl DynamicTreeListControl
        {
            get { return tlcMain; }
        }

        public Window DynamicOwner
        {
            get { return ServiceLocator.Current.GetInstance<IPOCMainWindow>().GetWindow(); }
        }

        void dragDropManager_Drop(object sender, DevExpress.Xpf.Grid.DragDrop.TreeListDropEventArgs e)
        {
            dynamic model = this.DataContext;
            model.Drop(sender, e);
        }

        private void dragDropManager_DragOver(object sender, DevExpress.Xpf.Grid.DragDrop.TreeListDragOverEventArgs e)
        {
            var list = (from n in e.Manager.DraggingRows.Cast<TreeListNode>() select n.Content as CacheItemProfileItem).ToList();
            var itemcount = list.Count(n => n.Tag is DBCTProfileItem);
            var profilecount = list.Count(n => n.Tag is DBCTProfileRoot);
            var groupcount = list.Count(n => n.Tag is DBCTProfileGroup);
            if (Math.Max(profilecount, Math.Max(itemcount, groupcount)) < (itemcount + profilecount + groupcount))
            {
                e.AllowDrop = false;
                e.ShowDropMarker = false;
                e.ShowDragInfo = false;
                return;
            }



               

        }


    }
}
