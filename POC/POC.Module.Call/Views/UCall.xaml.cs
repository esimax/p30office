using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Ribbon;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace POC.Module.Call.Views
{
    public partial class UCall : UserControl, IModuleRibbon
    {
        public UCall()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var pocCore = ServiceLocator.Current.GetInstance<POCCore>();
                if (pocCore.STCI.IsTamas)
                {
                    rpRecord.IsVisible = false;
                    colRecordTag.Visible = false;
                    colRecordRole.Visible = false;
                }

                rpRecord.IsVisible = false;
                colRecordTag.Visible = false;
                colRecordRole.Visible = false;
            };

        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return callRibbon;
        }
        public void LoadChildRibbons()
        {
        }

        public void UnloadChildRibbons()
        {
        }
        #endregion

        public RibbonControl DynamicRibbonControl
        {
            get { return GetRibbon() as RibbonControl; }
        }
        public ChartControl DynamicChartControl
        {
            get { return chartMain; }
        }
        public GridControl DynamicDynamicGrid
        {
            get { return gridCall; }
        }
        public DevExpress.Xpf.Editors.Filtering.FilterControl ActiveFilterControl { get { return filterEditor; } }



        private void filterEditor_BeforeShowValueEditor(object sender, DevExpress.Xpf.Editors.Filtering.ShowValueEditorEventArgs e)
        {
            if (e.CurrentNode.FirstOperand.PropertyName == "CallType")
            {
                if (CallTypeSource == null)
                {
                    CallTypeSource = new List<CallTypeHolder>();
                    CallTypeSource.Add(new CallTypeHolder { Title = "ارسالی", Value = 1 });
                    CallTypeSource.Add(new CallTypeHolder { Title = "دریافتی", Value = 0 });
                }
                e.CustomEditSettings = new ComboBoxEditSettings
                {
                    ItemsSource = CallTypeSource,
                    IsTextEditable = false,
                    DisplayMember = "Title",
                    ValueMember = "Value",
                    AllowSpinOnMouseWheel = false
                };
            }

            if (e.CurrentNode.FirstOperand.PropertyName == "DurationSeconds")
            {
                e.CustomEditSettings = new TextEditSettings
                {
                    DisplayTextConverter = new POL.Lib.Common.ConvCallDuration(),
                };
                e.Editor.FlowDirection = FlowDirection.LeftToRight;
            }

        }

        public List<CallTypeHolder> CallTypeSource { get; set; }
        public class CallTypeHolder
        {
            public string Title { get; set; }
            public int Value { get; set; }
        }

        public void BeginPrepareChart()
        {
            tiChartBar.BeginInit();
            tiChartBar.Content = null;

            var vb = new Viewbox { Child = chartMain };
            tiChartBar.Content = vb;
            tiChartBar.EndInit();
        }

        public void EndPrepareChart()
        {
            tiChartBar.BeginInit();
            var vb = (Viewbox)tiChartBar.Content;
            vb.Child = null;
            tiChartBar.Content = null;
            tiChartBar.Content = chartMain;
            tiChartBar.EndInit();
        }
    }
}
