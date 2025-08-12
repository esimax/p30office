using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Printing;
using DevExpress.Xpo;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.DXControls;
using DevExpress.Xpf.Bars;

namespace POC.Module.Call.Views.DashboardUnits
{
    public partial class UMultiChart : UserControl
    {
        private IDatabase ADatabase { get; set; }

        public UMultiChart()
        {
            InitializeComponent();

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            cbeChartCallType.SelectedIndex = 0;
            cbeChartStatType.SelectedIndex = 0;
            cbeChartGroup.SelectedIndex = 0;
            #region bPopulate.Click
            bPopulate.Click
                    += (s, e) =>
                    {
                        if (ValidateBeforPopulate())
                        {
                            PopulateChart();
                        }
                        else
                        {
                            POLMessageBox.ShowWarning("خطا در پارامترهای وارد شده.", Window.GetWindow(this));
                        }
                    }; 
            #endregion
            #region bPrint.Click
            bPrint.Click += (s, e) =>
                {
                    TheBorder.BeginInit();
                    TheBorder.Child = null;

                    var vb = new Viewbox { Child = ccChart, FlowDirection = FlowDirection.LeftToRight };
                    TheBorder.Child = vb;
                    TheBorder.EndInit();


                    var oldwidth = ccChart.Width;
                    var oldhight = ccChart.Height;

                    var ratio = 2.8;
                    var mL = 0.0;
                    var mR = 0.0;
                    var mT = 0.0;
                    var mB = 0.0;

                    var landscape = true;

                    var p = HelperPrinter.GetPaperKindDimensions(PaperKind.A4);
                    var w = (p.X / ratio) - mL - mR;
                    var h = (p.Y / ratio) - mT - mB;
                    ccChart.Height = landscape ? w : h;
                    ccChart.Width = landscape ? h : w;

                    var sl = new SimpleLink
                    {
                        DetailCount = 1,
                        DetailTemplate = (DataTemplate)this.Resources["ChartTemplate"]
                    };
                    sl.CreateDetail +=
                        (s1, e1) =>
                        {
                            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
                            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

                            var dpiX = (int)dpiXProperty.GetValue(null, null);
                            var dpiY = (int)dpiYProperty.GetValue(null, null);

                            var brush = new VisualBrush(ccChart);
                            var visual = new DrawingVisual();
                            var context = visual.RenderOpen();

                            context.DrawRectangle(brush, null,
                                new Rect(0, 0, ccChart.ActualWidth, ccChart.ActualHeight));
                            context.Close();

                            var bmp = new RenderTargetBitmap(
                                (int)ccChart.ActualWidth + 1,
                                (int)ccChart.ActualHeight + 1,
                                dpiX, dpiY, PixelFormats.Pbgra32);
                            bmp.Render(visual);

                            e1.Data = bmp;
                        };
                    sl.CreateDocument(true);
                    sl.PaperKind = PaperKind.A4;
                    sl.Landscape = landscape;
                    sl.Margins = new Margins((int)mL, (int)mR, (int)mT, (int)mB);

                    var preview = new DocumentPreview { Model = new LinkPreviewModel(sl) };
                    var v = (DataTemplate)this.FindResource("toolbarCustomization");
                    var barManagerCustomizer = new TemplatedBarManagerController { Template = v };
                    preview.BarManager.Controllers.Add(barManagerCustomizer);
                    preview.FlowDirection = FlowDirection.LeftToRight;
                    preview.FontFamily = new FontFamily("Tahoma");
                    preview.FontSize = 12.0;

                    var previewWindow = new DocumentPreviewWindow
                    {
                        Owner = Window.GetWindow(this),
                        Content = preview,
                        FlowDirection = FlowDirection.LeftToRight,
                        FontFamily = new FontFamily(HelperLocalize.ApplicationFontName),
                        FontSize = HelperLocalize.ApplicationFontSize,
                        Title = "پیش نمایش",
                    };

                    sl.CreateDocument(true);
                    previewWindow.ShowDialog();

                    ccChart.Width = oldwidth;
                    ccChart.Height = oldhight;

                    TheBorder.BeginInit();
                    var vb2 = (Viewbox)TheBorder.Child;
                    vb2.Child = null;
                    TheBorder.Child = null;
                    TheBorder.Child = ccChart;
                    TheBorder.EndInit();
                }; 
            #endregion
        }

        #region MonthNameList
        private List<string> _MonthNameList;
        public List<string> MonthNameList
        {
            get
            {
                return _MonthNameList;
            }
            set
            {
                if (_MonthNameList == value)
                    return;
                _MonthNameList = value;
            }
        }
        #endregion
        private void PopulateChart()
        {
            
            var chartCallType = cbeChartCallType.SelectedIndex; 
            var chartStatType = cbeChartStatType.SelectedIndex; 

            var VTitle = string.Empty;

            if (chartStatType == 0) VTitle = "تعداد تماس ";
            if (chartStatType == 1) VTitle = "مدت مكالمه ";

            if (chartCallType == 0) VTitle += "(ارسالی و دریافتی)";
            if (chartCallType == 1) VTitle += "(دریافتی)";
            if (chartCallType == 2) VTitle += "(ارسالی)";


            AxisYTitle.Content = VTitle;

            var chartGroup = cbeChartGroup.SelectedIndex; 
            var data = new List<ChartDataInt64>();
            
            var xpq = new XPQuery<DBCLCall>(ADatabase.Dxs);
            var xpq2 = xpq.Where(n => (int)n.CallType != 2);
            if (chartCallType == 1)
                xpq2 = xpq.Where(n => (int)n.CallType == 0);
            if (chartCallType == 2)
                xpq2 = xpq.Where(n => (int)n.CallType == 1);

            if (chartGroup == 0)
            {
                var g1 = new XPQuery<DBCLCall>(ADatabase.Dxs).Select(n => n.LineNumber).Distinct().OrderBy(n => n).ToList();
                foreach (var line in g1)
                {
                    if (chartStatType == 0)
                    {
                        var value = xpq2.Count(n => n.LineNumber == line);
                        data.Add(new ChartDataInt64 { Data = "خط " + line, Value = value });
                    }
                    if (chartStatType == 1)
                    {
                        var value = xpq2.Where(n => n.LineNumber == line).Sum(n => n.DurationSeconds);
                        data.Add(new ChartDataInt64 { Data = "خط " + line, Value = value });
                    }

                }
                MainSeries.DataSource = data;
            }
            if (chartGroup == 1)
            {
                try
                {
                    var g1 =
                        new XPQuery<DBCLCall>(ADatabase.Dxs)
                        .Where(n => n.LastExt != null)
                        .Select(n => n.LastExt)
                        .Distinct()
                        .OrderBy(n => n)
                        .ToList();
                    foreach (var line in g1)
                    {
                        if (chartStatType == 0)
                        {
                            var value = xpq2.Count(n => n.LastExt == line);
                            data.Add(new ChartDataInt64 { Data = "داخلی " + line, Value = value });
                        }
                        if (chartStatType == 1)
                        {
                            var value = xpq2.Where(n => n.LastExt == line).Sum(n => n.DurationSeconds);
                            data.Add(new ChartDataInt64 { Data = "داخلی " + line, Value = value });
                        }
                    }
                }
                catch
                {
                    POLMessageBox.ShowWarning("هیچ داخلی پیدا نشد.", Window.GetWindow(this));
                }
                MainSeries.DataSource = data;
            }
            if (chartGroup == 2)
            {
                var g1 = new XPQuery<DBCLCall>(ADatabase.Dxs)
                    .Where(n => n.TeleCode2 != null)
                    .Select(n => n.TeleCode2)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList();
                foreach (var line in g1)
                {
                    try
                    {
                        var len = line.ToString().Length;
                        var code2 = line.ToString().Substring(len - 7);
                        var code1 = line.ToString().Substring(0, len - 7);

                        if (chartStatType == 0)
                        {
                            var value = xpq2.Count(n => n.TeleCode2 == line);
                            data.Add(new ChartDataInt64 { Data = string.Format("(+{0}) {1}", code1, code2.Replace("0", "")), Value = value });
                        }
                        if (chartStatType == 1)
                        {
                            var value =
                                xpq2.Where(n => n.TeleCode2 == line).Sum(n => n.DurationSeconds);
                            data.Add(new ChartDataInt64 { Data = string.Format("(+{0}) {1}", code1, code2.Replace("0", "")), Value = value });
                        }
                    }
                    catch
                    {
                    }
                }
                MainSeries.DataSource = data;
            }


        }
        private bool ValidateBeforPopulate()
        {
            return true;
        }

    }
}
