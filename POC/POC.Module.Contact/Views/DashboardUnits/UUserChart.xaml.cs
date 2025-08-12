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
using POL.DB.Membership;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.DXControls;
using System.Drawing.Printing;

namespace POC.Module.Contact.Views.DashboardUnits
{
    public partial class UUserChart : UserControl
    {
        private ICacheData ACacheData { get; set; }
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }

        public UUserChart()
        {
            InitializeComponent();

            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            PopulateCategory();
            PopulateMonth();

            cbeCategory.ItemsSource = ContactCatList;
            cbeCategory.SelectedIndex = 0;
            cbeChartType.SelectedIndex = 0;
            #region Loaded
            Loaded +=
                    (s, e) =>
                    {
                        PopulateCategory();
                        PopulateUsers();
                    }; 
            #endregion
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

        private void PopulateUsers()
        {
            UsersList = (from n in DBMSUser2.UserGetAll(ADatabase.Dxs, null) select n).ToList();
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

        #region UsersList
        public List<DBMSUser2> UsersList { get; set; }
        #endregion


        private void PopulateCategory()
        {
            ContactCatList = new List<object> { "(همه دسته ها)" };
            var xpc = (from n in ACacheData.GetContactCatList() let cat = n.Tag as DBCTContactCat orderby cat.Title select cat).ToList();
            xpc.ToList().ForEach(n => ContactCatList.Add(n));
        }
        private void PopulateChart()
        {
            var cat = cbeCategory.SelectedItem as DBCTContactCat;
            var chartType = cbeChartType.SelectedIndex == 0 ? "UserCreated" : "UserModified"; 
            
            AxisYTitle.Content = cbeChartType.SelectedIndex == 0 ? "تعداد ثبت" : "تعداد ویرایش";
            var data = new List<ChartDataInt64>();

            foreach (var dbuser in UsersList)
            {
                var xpc = new XPCollection<DBCTContact>(ADatabase.Dxs);
                try
                {
                    var crit = new GroupOperator();
                    if (cat != null)
                        crit.Operands.Add(new ContainsOperator("Categories", new BinaryOperator("Oid", cat.Oid)));
                    crit.Operands.Add(new BinaryOperator(chartType, dbuser.Username));
                    xpc.Criteria = crit;
                    data.Add(new ChartDataInt64 { Data = dbuser.Title, Value = xpc.Count });
                }
                catch
                {
                }
            }
            MainSeries.DataSource = data;
        }
        private bool ValidateBeforPopulate()
        {
            return true;
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
