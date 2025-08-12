using DevExpress.Xpf.Printing;

namespace POC.Shell.Localizers.fa
{
    internal class PersianPrintingLocalizer : PrintingLocalizer
    {
        public override string GetLocalizedString(PrintingStringId id)
        {
            switch (id)
            {
                case PrintingStringId.OK:
                    return "تایید";

                case PrintingStringId.Cancel:
                    return "لغو";

                case PrintingStringId.ToolBarCaption:
                    return "پیش نمایش";

                case PrintingStringId.StatusBarCaption:
                    return "نوار وضعیت";

                case PrintingStringId.Print:
                    return "چاپ ...";

                case PrintingStringId.PrintPdf:
                    return "چاپ بوسیله Pdf ...";

                case PrintingStringId.PrintDirect:
                    return "چاپ سریع";

                case PrintingStringId.Zoom:
                    return "بزرگنمایی";

                case PrintingStringId.DecreaseZoom:
                    return "كوچكتر";

                case PrintingStringId.IncreaseZoom:
                    return "بزرگتر";

                case PrintingStringId.ZoomToPageWidth:
                    return "پهنای صفحه";

                case PrintingStringId.ZoomToPageHeight:
                    return "برحسب ارتفاع";

                case PrintingStringId.ZoomToWholePage:
                    return "بر حسب ابعاد صفحه";

                case PrintingStringId.ZoomToTwoPages:
                    return "دو صفحه ای";

                case PrintingStringId.FirstPage:
                    return "صفحه اول";

                case PrintingStringId.PreviousPage:
                    return "صفحه قبل";

                case PrintingStringId.NextPage:
                    return "صفحه بعد";

                case PrintingStringId.LastPage:
                    return "صفحه آخر";

                case PrintingStringId.Watermark:
                    return "تصویر پشت زمینه";

                case PrintingStringId.WatermarkTitle:
                    return "تصویر پشت زمینه";

                case PrintingStringId.TextWatermarkTitle:
                    return "متن پشت زمینه";

                case PrintingStringId.PictureWatermarkTitle:
                    return "عكس پشت زمینه";

                case PrintingStringId.WatermarkText:
                    return "متن";

                case PrintingStringId.WatermarkTextDirection:
                    return "جهت";

                case PrintingStringId.WatermarkTextColor:
                    return "رنگ";

                case PrintingStringId.WatermarkFontName:
                    return "قلم";

                case PrintingStringId.WatermarkFontSize:
                    return "اندازه";

                case PrintingStringId.WatermarkFontBold:
                    return "ضخیم";

                case PrintingStringId.WatermarkFontItalic:
                    return "كج";

                case PrintingStringId.WatermarkPosition:
                    return "موقعیت";

                case PrintingStringId.WatermarkPositionInFront:
                    return "جلو";

                case PrintingStringId.WatermarkPositionBehind:
                    return "عقب";

                case PrintingStringId.WatermarkPageRange:
                    return "بازه صفحات";

                case PrintingStringId.WatermarkPageRangeAllPages:
                    return "همه";

                case PrintingStringId.WatermarkPageRangePages:
                    return "صفحات";

                case PrintingStringId.WatermarkPageRangeHint:
                    return "به عنوان مثال: 1،3،5-12";

                case PrintingStringId.WatermarkLoadImage:
                    return "عكس";

                case PrintingStringId.WatermarkImageSizeMode:
                    return "نوع اندازه";

                case PrintingStringId.WatermarkImageHorizontalAlignment:
                    return "چیدمان افقی";

                case PrintingStringId.WatermarkImageVerticalAlignment:
                    return "چیدمان عمودی";

                case PrintingStringId.WatermarkImageTiling:
                    return "عنوان";

                case PrintingStringId.WatermarkTransparency:
                    return "شفافیت";

                case PrintingStringId.WatermarkClearAll:
                    return "حذف همه";

                case PrintingStringId.WatermarkImageLoadError:
                    return "فایل خراب می باشد";

                case PrintingStringId.ExportPdf:
                    return "فایل PDF";

                case PrintingStringId.ExportHtm:
                    return "فایل HTML";

                case PrintingStringId.ExportMht:
                    return "فایل MHT";

                case PrintingStringId.ExportRtf:
                    return "فایل RTF";

                case PrintingStringId.ExportXls:
                    return "فایل XLS";

                case PrintingStringId.ExportXlsx:
                    return "فایل XLSX";

                case PrintingStringId.ExportCsv:
                    return "فایل CSV";

                case PrintingStringId.ExportTxt:
                    return "فایل متنی";

                case PrintingStringId.ExportImage:
                    return "فایل تصویر";

                case PrintingStringId.ExportXps:
                    return "فایل XPS";

                case PrintingStringId.ExportFile:
                    return "خروجی به ...";

                case PrintingStringId.Scaling:
                    return "";

                case PrintingStringId.Scaling_Adjust_Start_Label:
                    return "تنظیم به";

                case PrintingStringId.Scaling_Adjust_End_Label:
                    return "اندازه معمول";

                case PrintingStringId.Scaling_Fit_Start_Label:
                    return "متناسب با";

                case PrintingStringId.Scaling_Fit_End_Label:
                    return "پهنای صفحات";

                case PrintingStringId.Scaling_ComboBoxEdit_Validation_Error:
                    return "مقدار معتبر نمی باشد";

                case PrintingStringId.Scaling_ComboBoxEdit_Validation_OutOfRange_Error:
                    return "مقدار خارج از بازه مجاز می باشد";

                case PrintingStringId.Search:
                    return "جستجو";

                case PrintingStringId.SendPdf:
                    return "فایل PDF";

                case PrintingStringId.SendMht:
                    return "فایل MHT";

                case PrintingStringId.SendRtf:
                    return "فایل RTF";

                case PrintingStringId.SendXls:
                    return "فایل XLS";

                case PrintingStringId.SendXlsx:
                    return "فایل XLSX";

                case PrintingStringId.SendCsv:
                    return "فایل CSV";

                case PrintingStringId.SendTxt:
                    return "فایل متنی";

                case PrintingStringId.SendImage:
                    return "فایل تصویری";

                case PrintingStringId.SendXps:
                    return "فایل XPS";

                case PrintingStringId.SendFile:
                    return "ارسال با ایمیل ...";

                case PrintingStringId.StopPageBuilding:
                    return "توقف";

                case PrintingStringId.CurrentPageDisplayFormat:
                    return "صفحه {0} از {1}";

                case PrintingStringId.ZoomDisplayFormat:
                    return "بزرگنمایی: %{0:0}";

                case PrintingStringId.MsgCaption:
                    return "چاپ";

                case PrintingStringId.PageSetup:
                    return "تنظیمات صفحه";

                case PrintingStringId.GoToPage:
                    return "ارجاع به صفحه:";

                case PrintingStringId.PrintPreviewWindowCaption:
                    return "پیش نمایش";

                case PrintingStringId.DefaultPrintJobDescription:
                    return "سند";

                case PrintingStringId.PdfPasswordSecurityOptions_Title:
                    return "رمز امنیت PDF";

                case PrintingStringId.PdfPasswordSecurityOptions_RequireOpenPassword:
                    return "جهت نمایش فایل رمز نیاز می باشد";

                case PrintingStringId.PdfPasswordSecurityOptions_OpenPassword:
                    return "رمز باز كردن سند:";

                case PrintingStringId.PdfPasswordSecurityOptions_RestrictPermissions:
                    return "ویرایش و چاپ سند مسدود شود";

                case PrintingStringId.PdfPasswordSecurityOptions_PermissionsPassword:
                    return "تغییر رمز دسترسی:";

                case PrintingStringId.PdfPasswordSecurityOptions_PrintingPermissions:
                    return "مجوز چاپ:";

                case PrintingStringId.PdfPasswordSecurityOptions_ChangingPermissions:
                    return "مجوز ویرایش:";

                case PrintingStringId.PdfPasswordSecurityOptions_EnableCopying:
                    return "فعال سازی كپی متن، تصویر و دیگر محتوی";

                case PrintingStringId.PdfPasswordSecurityOptions_EnableScreenReaders:
                    return "فعال كردن دسترسی به متن برای خواننده روی صفحه نمایش دستگاه جهت برطرف كردن مشكل دیداری";

                case PrintingStringId.RepeatPassword_OpenPassword_Title:
                    return "رمز مجدد";

                case PrintingStringId.RepeatPassword_OpenPassword_Note:
                    return "لطفا رمز باز كردن سند را وارد كنید. حتما آن را جایی یادداشت كنید چون  جهت باز كردن از شما پرسیده می شود.";

                case PrintingStringId.RepeatPassword_OpenPassword:
                    return "رمز باز كردن سند:";

                case PrintingStringId.RepeatPassword_PermissionsPassword_Title:
                    return "تكرار رمز :";

                case PrintingStringId.RepeatPassword_PermissionsPassword_Note:
                    return "تكرار رمز";

                case PrintingStringId.RepeatPassword_PermissionsPassword:
                    return "تكرار رمز:";

                case PrintingStringId.RepeatPassword_ConfirmationPasswordDoesNotMatch:
                    return "رمز و تكرار آن با هم تطبیق ندارند";

                case PrintingStringId.PageSetupWindowTitle:
                    return "تنظیمات صفحه";

                case PrintingStringId.PageSetupMarginsCaptionFormat:
                    return "حاشیه بر حسب {0}";

                case PrintingStringId.PageSetupPrinterCaption:
                    return "چاپگر";

                case PrintingStringId.PageSetupPrinter:
                    return "چاپگر:";

                case PrintingStringId.PageSetupPrinterType:
                    return "نوع چاپگر:";

                case PrintingStringId.PageSetupPrinterPort:
                    return "پورت:";

                case PrintingStringId.PageSetupPrinterComment:
                    return "نكته:";

                case PrintingStringId.PageSetupPaperCaption:
                    return "كاغذ";

                case PrintingStringId.PageSetupPaperSize:
                    return "ابعاد كاغذ:";

                case PrintingStringId.PageSetupOrientationCaption:
                    return "جهت:";

                case PrintingStringId.PageSetupOrientationPortrait:
                    return "عمودی";

                case PrintingStringId.PageSetupOrientationLandscape:
                    return "افقی";

                case PrintingStringId.PageSetupMarginsLeft:
                    return "چپ:";

                case PrintingStringId.PageSetupMarginsTop:
                    return "بالا:";

                case PrintingStringId.PageSetupMarginsRight:
                    return "راست:";

                case PrintingStringId.PageSetupMarginsBottom:
                    return "پایین:";

                case PrintingStringId.PageSetupMillimeters:
                    return "میلیمتر";

                case PrintingStringId.PageSetupInches:
                    return "اینچ";

                case PrintingStringId.ZoomValueItemFormat:
                    return "{0}%";

                case PrintingStringId.Open:
                    return "باز كردن";

                case PrintingStringId.Save:
                    return "ذخیره";

                case PrintingStringId.Error:
                    return "خطا";

                case PrintingStringId.DocumentMap:
                    return "نقشه سند";

                case PrintingStringId.Refresh:
                    return "بازخوانی";

                case PrintingStringId.Information:
                    return "اطلاعات";

                case PrintingStringId.Parameters:
                    return "پارامتر ها";

                case PrintingStringId.ParametersReset:
                    return "ریست";

                case PrintingStringId.ParametersSubmit:
                    return "تایید";

                case PrintingStringId.PdfPasswordSecurityOptions_Permissions:
                    return "مجوز";

                case PrintingStringId.PdfPasswordSecurityOptions_OpenPasswordHeader:
                    return "رمز بازكردن سند";

                case PrintingStringId.Search_EmptyResult:
                    return "هیچ نتیجه ای یافت نشد.";

                case PrintingStringId.PreparingPages:
                    return "آماده سازی صفحات ...";

                case PrintingStringId.PagesArePrepared:
                    return "صفحات آماده هستند. چاپ انجام شود؟";

                case PrintingStringId.ExportPdfToWindow:
                    return "فایل PDF";

                case PrintingStringId.ExportHtmToWindow:
                    return "فایل HTML";

                case PrintingStringId.ExportMhtToWindow:
                    return "فایل MHT";

                case PrintingStringId.ExportRtfToWindow:
                    return "فایل RTF";

                case PrintingStringId.ExportXlsToWindow:
                    return "فایل XLS";

                case PrintingStringId.ExportXlsxToWindow:
                    return "فایل XLSX";

                case PrintingStringId.ExportCsvToWindow:
                    return "فایل CSV";

                case PrintingStringId.ExportTxtToWindow:
                    return "فایل متنی";

                case PrintingStringId.ExportImageToWindow:
                    return "فایل تصویری";

                case PrintingStringId.ExportXpsToWindow:
                    return "فایل XPS";

                case PrintingStringId.ExportFileToWindow:
                    return "استخراج به پنجره ...";

                case PrintingStringId.ClosePreview:
                    return "بستن";

                case PrintingStringId.Exception_NoPrinterFound:
                    return "هیچ چاپگری یافت نشد.";

                case PrintingStringId.Msg_EmptyDocument:
                    return "این سند هیچ صفحه ای جهت چاپ ندارد.";

            }
            return GetLocalizedString(id);
        }
    }
}
