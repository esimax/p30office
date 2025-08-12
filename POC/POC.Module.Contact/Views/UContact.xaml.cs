using System;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Ribbon;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using DevExpress.Xpf.Editors;

namespace POC.Module.Contact.Views
{
    public partial class UContact : UserControl, IModuleRibbon
    {
        public UContact()
        {
            InitializeComponent();
            Loaded +=
                (s, e) =>
                {
                    gridModuleContainer.Width = GetModuleContainerWidth();
                    LoadContactColumnWidth();
                };
            Unloaded +=
                (s, e) =>
                {
                    try
                    {
                        HelperSettingsClient.ModuleContentWidth = (int)(gridModuleContainer.Width * 100);
                        SaveContactColumnWidth();
                    }
                    catch { }
                };
        }

        #region [METHODS]
        private double GetModuleContainerWidth()
        {
            try
            {
                var v = (double)HelperSettingsClient.ModuleContentWidth / 100;
                if (v < 200) throw new Exception();
                if (v > (ActualWidth - 200))
                    v = ActualWidth - 200;
                return v;
            }
            catch
            {
                return 200;
            }
        }
        private void LoadContactColumnWidth()
        {
            var i = 0;
            try
            {
                var ss = HelperSettingsClient.ContactColumnWidth.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var w in ss.Select(Convert.ToDouble))
                {
                    gridContact.Columns[i].Width = w;
                    i++;
                }
            }
            catch { }
        }
        private void SaveContactColumnWidth()
        {
            try
            {
                var ss = gridContact.Columns.Select(t => t.ActualWidth).Aggregate(string.Empty, (current, w) => current + string.Format("{0}|", w));
                HelperSettingsClient.ContactColumnWidth = ss;
            }
            catch { }
        }
        #endregion

        #region IModuleRibbon
        public object GetRibbon()
        {
            return contactRibbon;
        }
        public void LoadChildRibbons()
        {
            dynamic model = DataContext;
            model.DynamicLoadChildRibbon();
        }
        public void UnloadChildRibbons()
        {
            dynamic model = DataContext;
            model.DynamicUnloadChildRibbon();
        }
        #endregion


        public RibbonControl DynamicRibbonControl
        {
            get { return GetRibbon() as RibbonControl; }
        }
        public GridControl DynamicDynamicGrid
        {
            get { return gridContact; }
        }

        public ButtonEdit DynamicSerachTextEdit
        {
            get { return beSearchText; }
        }


    }
}
