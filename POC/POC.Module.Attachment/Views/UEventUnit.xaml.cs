using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Grid;
using POL.Lib.Interfaces;

namespace POC.Module.Attachment.Views
{
    public partial class UEventUnit : UserControl, IModuleRibbon
    {
        public UEventUnit()
        {
            InitializeComponent();
        }

        #region Implementation of IModuleRibbon
        public object GetRibbon()
        {
            return EventUnitRibbon;
        }
        public void UnloadChildRibbons()
        {
        }
        public void LoadChildRibbons()
        {
        }
        #endregion

        public GridControl DynamicGridControl
        {
            get { return gcMain; }
        }
    }
}
