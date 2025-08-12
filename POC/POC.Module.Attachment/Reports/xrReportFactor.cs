using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UI;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using System.Drawing;
using System.Linq;

namespace POC.Module.Attachment.Reports
{
    public partial class xrReportFactor : DevExpress.XtraReports.UI.XtraReport
    {
        public xrReportFactor(DBACFactor factor)
        {
            InitializeComponent();

            var pocCore = ServiceLocator.Current.GetInstance<POCCore>();

            Factor = factor;
            if (factor.IsPreFactor)
                xrlTitle.Text = "پیش فاكتور";

            xrlName.Text = HelperConvert.ConvertToPersianDigit(factor.Title);
            xrlDate.Text = HelperConvert.ConvertToPersianDigit(HelperLocalize.DateTimeToString(factor.Date, EnumCalendarType.Shamsi,
                "dd / MM / yy"));
            xrlNumber.Text = HelperConvert.ConvertToPersianDigit(factor.Code.ToString());
            xrlCode.Text = HelperConvert.ConvertToPersianDigit(pocCore.STCI.EcoCode);
            if (string.IsNullOrWhiteSpace(xrlCode.Text))
                xrlCode.Text = pocCore.STCI.IntCode;

            if (!string.IsNullOrWhiteSpace(factor.TitleOfTitle))
                xrlTitleOfTitle.Text = factor.TitleOfTitle;

            xrlNote.Text = factor.Note;

            this.DataSource = factor.FactoreItems;

            xrtcProductTitle.DataBindings.Add(new XRBinding("Text", null, "Product.Title", "{0:#,#,#,#,#,#}"));
            xrtcProductUnit.DataBindings.Add(new XRBinding("Text", null, "Product.Unit.Title", "{0:#,#,#,#,#,#}"));
            xrtcProductPrice.DataBindings.Add(new XRBinding("Text", null, "PricePersian", "{0:#,#,#,#,#,#}"));
            xrtcProductCount.DataBindings.Add(new XRBinding("Text", null, "CountPersian", "{0:#,#,#,#,#,#}"));
            xrtcProductSum.DataBindings.Add(new XRBinding("Text", null, "SumPersian", "{0:#,#,#,#,#,#}"));

            var sum = (decimal)factor.FactoreItems.Sum(n => (double)n.Price * n.Count);
            xrtcSum.Text = HelperConvert.ConvertToPersianDigit(sum.ToString("#,#,#,#,#,#"));
            xrtcDiscount.Text = HelperConvert.ConvertToPersianDigit(factor.SumDiscount.ToString("#,#,#,#,#,#"));
            xrtcDiscountSum.Text = HelperConvert.ConvertToPersianDigit((sum - factor.SumDiscount).ToString("#,#,#,#,#,#"));
            xrtcIncrease.Text = HelperConvert.ConvertToPersianDigit(factor.SumPercentIncrease.ToString("#,#,#,#,#,#"));
            xrtcTotalSum.Text = HelperConvert.ConvertToPersianDigit(factor.SumSum.ToString("#,#,#,#,#,#"));


            if (!factor.CodeShow)
            {
                xrlNumber.Text = "";
            }

            if (string.IsNullOrEmpty(xrtcDiscount.Text) && string.IsNullOrEmpty(xrtcIncrease.Text))
            {
                xrtrSum.Visible = false;
            }

            if (string.IsNullOrEmpty(xrtcDiscount.Text))
            {
                xrtcDiscount.Text = HelperConvert.ConvertToPersianDigit("0");
                xrtrDiscount.Visible = false;

                xrtcDiscountSum.Text = HelperConvert.ConvertToPersianDigit("0");
                xrtrDiscountSum.Visible = false;
            }

            if (string.IsNullOrEmpty(xrtcIncrease.Text))
            {
                xrtcIncrease.Text = HelperConvert.ConvertToPersianDigit("0");
                xrtrIncrease.Visible = false;
            }

            try
            {
                this.Watermark.Image = Bitmap.FromFile(pocCore.STCI.SarbargPath);
                this.Watermark.ImageAlign = ContentAlignment.MiddleCenter;
                this.Watermark.ImageTiling = false;
                this.Watermark.ImageViewMode = ImageViewMode.Stretch;
                this.Watermark.ImageTransparency = 0;
                this.Watermark.ShowBehind = true;
            }
            catch
            {
            }
        }

        public DBACFactor Factor { get; set; }
    }
}
