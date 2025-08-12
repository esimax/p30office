using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Printing;
using DevExpress.Xpo;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.DXControls;

namespace POC.Module.Call.Views.DashboardUnits
{
    public partial class UTimeChart : UserControl
    {
        private IDatabase ADatabase { get; set; }

        public UTimeChart()
        {
            InitializeComponent();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            PopulateMonth();
            cbeChartCallType.SelectedIndex = 0;
            cbeChartStatType.SelectedIndex = 0;
            cbeChartGroup.SelectedIndex = 0;
            cbeMonth.ItemsSource = MonthNameList;
            var calc = HelperLocalize.GetCalendar(HelperLocalize.ApplicationCalendar);
            cbeMonth.SelectedIndex = calc.GetMonth(DateTime.Now);
            teYear.EditValue = calc.GetYear(DateTime.Now);

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
            #region cbeChartGroup.SelectedIndexChanged
            cbeChartGroup.SelectedIndexChanged
                    += (s, e) =>
                    {
                        if (cbeChartGroup.SelectedIndex == 0)
                        {
                            cbeMonth.IsEnabled = true;
                            teYear.IsEnabled = true;
                        }
                        if (cbeChartGroup.SelectedIndex == 1)
                        {
                            cbeMonth.IsEnabled = false;
                            teYear.IsEnabled = true;
                        }
                        if (cbeChartGroup.SelectedIndex == 2)
                        {
                            cbeMonth.IsEnabled = false;
                            teYear.IsEnabled = false;
                            teYear.EditValue = calc.GetYear(DateTime.Now);
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
            var calc = HelperLocalize.GetCalendar(HelperLocalize.ApplicationCalendar);
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
            var month = cbeMonth.SelectedIndex + 1;
            var year = Convert.ToInt32(teYear.EditValue);

            var data = new List<ChartDataInt64>();

            var xpq = new XPQuery<DBCLCall>(ADatabase.Dxs);
            var xpq2 = xpq.Where(n => (int)n.CallType != 2);
            if (chartCallType == 1)
                xpq2 = xpq.Where(n => (int)n.CallType == 0);
            if (chartCallType == 2)
                xpq2 = xpq.Where(n => (int)n.CallType == 1);

            if (chartGroup == 0)
            {
                for (int i = 1; i <= 31; i++)
                {
                    try
                    {
                        var dt1 = calc.ToDateTime(year, month, i, 0, 0, 0, 0);
                        var dt2 = dt1.AddDays(1);
                        var q3 = xpq2.Where(n => n.CallDate >= dt1 && n.CallDate < dt2);
            
                        var q4 = q3.Count();
                        if (chartStatType == 1)
                            q4 = q3.Sum(n => n.DurationSeconds);
                        data.Add(new ChartDataInt64 { Data = i.ToString(), Value = q4 });
                    }
                    catch
                    {
                    }
                }
                MainSeries.DataSource = data;
            }
            if (chartGroup == 1)
            {
                for (int i = 1; i <= 12; i++)
                {
                    try
                    {
                        var dt1 = calc.ToDateTime(year, i, 1, 0, 0, 0, 0);
                        var dt2 = dt1.AddMonths(1);
                        var q3 = xpq2.Where(n => n.CallDate >= dt1 && n.CallDate < dt2);

                        var q4 = q3.Count();
                        if (chartStatType == 1)
                            q4 = q3.Sum(n => n.DurationSeconds);
                        data.Add(new ChartDataInt64 { Data = i.ToString(), Value =q4 });
                    }
                    catch
                    {
                    }
                }
                MainSeries.DataSource = data;
            }
            if (chartGroup == 2)
            {
                for (int i = year - 10; i <= year; i++)
                {
                    try
                    {
                        var dt1 = calc.ToDateTime(i, 1, 1, 0, 0, 0, 0);
                        var dt2 = dt1.AddYears(1);
                        var q3 = xpq2.Where(n => n.CallDate >= dt1 && n.CallDate < dt2);

                        var q4 = q3.Count();
                        if (chartStatType == 1)
                            q4 = q3.Sum(n => n.DurationSeconds);
                        data.Add(new ChartDataInt64 { Data = i.ToString(), Value = q4 });
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
            if (!ValidateYear())
                return false;
            return true;
        }
        private bool ValidateYear()
        {
            try
            {
                var year = Convert.ToInt32(teYear.EditValue);
                return year > 1300;
            }
            catch
            {
                return false;
            }
        }
        private void PopulateMonth()
        {
            MonthNameList = new List<string>();
            for (int i = 1; i <= 12; i++)
            {
                var mn = HelperLocalize.GetMonthName(HelperLocalize.ApplicationCalendar, i);
                MonthNameList.Add(mn);
            }
        }
    }
}
