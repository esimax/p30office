using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Printing;
using DevExpress.Xpo;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.DXControls;
using System.Drawing.Printing;

namespace POC.Module.Contact.Views.DashboardUnits
{
    public partial class UTimeChart : UserControl
    {
        private ICacheData ACacheData { get; set; }
        private IDatabase ADatabase { get; set; }

        public UTimeChart()
        {
            InitializeComponent();

            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();

            PopulateCategory();
            PopulateMonth();

            cbeCategory.ItemsSource = ContactCatList;
            cbeCategory.SelectedIndex = 0;

            cbeChartType.SelectedIndex = 0;

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

        #region ContactCatList
        private List<object> _ContactCatList;
        public List<object> ContactCatList
        {
            get
            {
                return _ContactCatList;
            }
            set
            {
                if (_ContactCatList == value)
                    return;
                _ContactCatList = value;
            }
        }
        #endregion

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


        private void PopulateCategory()
        {
            ContactCatList = new List<object> { "(همه دسته ها)" };
            var xpc = (from n in ACacheData.GetContactCatList() let cat = n.Tag as DBCTContactCat orderby cat.Title select cat).ToList();
            xpc.ToList().ForEach(n => ContactCatList.Add(n));
        }
        private void PopulateChart()
        {
            var calc = HelperLocalize.GetCalendar(HelperLocalize.ApplicationCalendar);
            var cat = cbeCategory.SelectedItem as DBCTContactCat;
            var chartType = cbeChartType.SelectedIndex == 0 ? "DateCreated" : "DateModified"; 
            AxisYTitle.Content = cbeChartType.SelectedIndex == 0 ? "تعداد ثبت" : "تعداد ویرایش";
            var chartGroup = cbeChartGroup.SelectedIndex; 
            var month = cbeMonth.SelectedIndex + 1;
            var year = Convert.ToInt32(teYear.EditValue);

            var data = new List<ChartDataInt64>();

            var xpc = new XPCollection<DBCTContact>(ADatabase.Dxs);



            if (chartGroup == 0)
            {
                for (int i = 1; i <= 31; i++)
                {
                    try
                    {
                        var dt1 = calc.ToDateTime(year, month, i, 0, 0, 0, 0);
                        var dt2 = dt1.AddDays(1);
                        var crit = new GroupOperator();
                        if (cat != null)
                            crit.Operands.Add(new ContainsOperator("Categories", new BinaryOperator("Oid", cat.Oid)));
                        crit.Operands.Add(new BinaryOperator(chartType, dt1, BinaryOperatorType.GreaterOrEqual));
                        crit.Operands.Add(new BinaryOperator(chartType, dt2, BinaryOperatorType.Less));
                        xpc.Criteria = crit;
                        data.Add(new ChartDataInt64 { Data = i.ToString(), Value = xpc.Count });
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
                        var crit = new GroupOperator();
                        if (cat != null)
                            crit.Operands.Add(new ContainsOperator("Categories", new BinaryOperator("Oid", cat.Oid)));
                        crit.Operands.Add(new BinaryOperator(chartType, dt1, BinaryOperatorType.GreaterOrEqual));
                        crit.Operands.Add(new BinaryOperator(chartType, dt2, BinaryOperatorType.Less));
                        xpc.Criteria = crit;
                        data.Add(new ChartDataInt64 { Data = i.ToString(), Value = xpc.Count });
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
                        var crit = new GroupOperator();
                        if (cat != null)
                            crit.Operands.Add(new ContainsOperator("Categories", new BinaryOperator("Oid", cat.Oid)));
                        crit.Operands.Add(new BinaryOperator(chartType, dt1, BinaryOperatorType.GreaterOrEqual));
                        crit.Operands.Add(new BinaryOperator(chartType, dt2, BinaryOperatorType.Less));
                        xpc.Criteria = crit;
                        data.Add(new ChartDataInt64 { Data = i.ToString(), Value = xpc.Count });
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
