using DevExpress.XtraPrinting.Localization;
using System;

namespace POC.Shell.Localizers.fa
{
    internal class PersianPreviewLocalizer : PreviewLocalizer
    {
        public override string GetLocalizedString(PreviewStringId id)
        {
            switch (id)
            {


                case PreviewStringId.Button_Cancel:
                    return "لغو";

                case PreviewStringId.Button_Ok:
                    return "تایید";

                case PreviewStringId.Button_Help:
                    return "راهنما";

                case PreviewStringId.Button_Apply:
                    return "اعمال";


                case PreviewStringId.PreviewForm_Caption:
                    return "پیش نمایش";


                case PreviewStringId.TB_TTip_Customize:
                    return "سفارشی";


                case PreviewStringId.TB_TTip_Print:
                    return "چاپ";


                case PreviewStringId.TB_TTip_PrintDirect:
                    return "چاپ سریع";


                case PreviewStringId.TB_TTip_PageSetup:
                    return "تنظیمات صفحه";


                case PreviewStringId.TB_TTip_Magnifier:
                    return "ذره بین";


                case PreviewStringId.TB_TTip_ZoomIn:
                    return "بزرگتر";


                case PreviewStringId.TB_TTip_ZoomOut:
                    return "كوچكتر";


                case PreviewStringId.TB_TTip_Zoom:
                    return "بزرگنمایی";


                case PreviewStringId.TB_TTip_Search:
                    return "جستجو";


                case PreviewStringId.TB_TTip_FirstPage:
                    return "صفحه اول";


                case PreviewStringId.TB_TTip_PreviousPage:
                    return "صفحه قبل";


                case PreviewStringId.TB_TTip_NextPage:
                    return "صفحه بعد";


                case PreviewStringId.TB_TTip_LastPage:
                    return "صفحه آخر";


                case PreviewStringId.TB_TTip_MultiplePages:
                    return "چندین صفحه";


                case PreviewStringId.TB_TTip_Backgr:
                    return "پشت زمینه";


                case PreviewStringId.TB_TTip_Close:
                    return "بستن پنجره";


                case PreviewStringId.TB_TTip_EditPageHF:
                    return "سربرگ و پاورقی ";


                case PreviewStringId.TB_TTip_HandTool:
                    return "ابزار دست";


                case PreviewStringId.TB_TTip_Export:
                    return "استخراج سند به ...";


                case PreviewStringId.TB_TTip_Send:
                    return "ارسال توسط ایمیل ...";


                case PreviewStringId.TB_TTip_Map:
                    return "نقشه سند";

                case PreviewStringId.TB_TTip_Parameters:
                    return "پارامتر ها";


                case PreviewStringId.TB_TTip_Watermark:
                    return "تصویر پشت زمینه";

                case PreviewStringId.TB_TTip_Scale:
                    return "مقیاس";

                case PreviewStringId.TB_TTip_Open:
                    return "باز كردن";

                case PreviewStringId.TB_TTip_Save:
                    return "ذخیره";


                case PreviewStringId.MenuItem_PdfDocument:
                    return "فایل PDF";

                case PreviewStringId.MenuItem_PageLayout:
                    return "چیدمان صفحه";


                case PreviewStringId.MenuItem_TxtDocument:
                    return "فایل متنی";

                case PreviewStringId.MenuItem_GraphicDocument:
                    return "فایل تصویری";


                case PreviewStringId.MenuItem_CsvDocument:
                    return "فایل CSV";


                case PreviewStringId.MenuItem_MhtDocument:
                    return "فایل MHT";


                case PreviewStringId.MenuItem_XlsDocument:
                    return "فایل XLS";

                case PreviewStringId.MenuItem_XlsxDocument:
                    return "فایل XLSX";


                case PreviewStringId.MenuItem_RtfDocument:
                    return "فایل RTF";


                case PreviewStringId.MenuItem_HtmDocument:
                    return "فایل HTML";


                case PreviewStringId.SaveDlg_FilterBmp:
                    return "BMP Bitmap Format";


                case PreviewStringId.SaveDlg_FilterGif:
                    return "GIF Graphics Interchange Format";


                case PreviewStringId.SaveDlg_FilterJpeg:
                    return "JPEG File Interchange Format";


                case PreviewStringId.SaveDlg_FilterPng:
                    return "PNG Portable Network Graphics Format";


                case PreviewStringId.SaveDlg_FilterTiff:
                    return "TIFF Tag Image File Format";


                case PreviewStringId.SaveDlg_FilterEmf:
                    return "EMF Enhanced Windows Metafile";


                case PreviewStringId.SaveDlg_FilterWmf:
                    return "WMF Windows Metafile";



                case PreviewStringId.SB_PageOfPages:
                    return "صفحه {0} از {1}";

                case PreviewStringId.SB_ZoomFactor:
                    return "درصد بزرگنمایی:";

                case PreviewStringId.SB_PageNone:
                    return "هیچ";


                case PreviewStringId.SB_PageInfo:
                    return "{0} از {1}";

                case PreviewStringId.SB_TTip_Stop:
                    return "توقف";















                case PreviewStringId.MPForm_Lbl_Pages:
                    return "صفحات";

                case PreviewStringId.Msg_EmptyDocument:
                    return "این سند هیچ صفحه ای ندارد.";

                case PreviewStringId.Msg_CreatingDocument:
                    return "در حال ایجاد سند ...";

                case PreviewStringId.Msg_UnavailableNetPrinter:
                    return "چاپگر تحت شبكه موجود نمی باشد.";

                case PreviewStringId.Msg_NeedPrinter:
                    return "هیچ چاپگری نصب نشده.";

                case PreviewStringId.Msg_WrongPrinter:
                    return "نام چاپگر معتبر نمی باشد. لطفا به تنظیمات مراجعه كنید.";

                case PreviewStringId.Msg_WrongPrinting:
                    return "بروز خطا هنگام چاپ";

                case PreviewStringId.Msg_WrongPageSettings:
                    return "كاغذ انتخاب شده برای این چاپگر مناسب نمی باشد.";

                case PreviewStringId.Msg_CustomDrawWarning:
                    return "اخطار!";

                case PreviewStringId.Msg_PageMarginsWarning:
                    return "حاشیه های تنظیم شده خارج از حد كاغذ می باشد. می خواهید ادامه دهید؟";

                case PreviewStringId.Msg_IncorrectPageRange:
                    return "بازه صفحات معتبر نمی باشد.";

                case PreviewStringId.Msg_FontInvalidNumber:
                    return "اندازه قلم نمی تواند كمتر از صفر ویا عددی منفی باشد.";

                case PreviewStringId.Msg_NotSupportedFont:
                    return "این قلم پشتیبانی نمی شود.";

                case PreviewStringId.Msg_IncorrectZoomFactor:
                    return "عدد میبایست بین {0} و {1} باشد.";

                case PreviewStringId.Msg_InvalidMeasurement:
                    return "واحد اندازه گیری معتبر نمی باشد.";

                case PreviewStringId.Msg_CannotAccessFile:
                    return "امكان باز كردن فایل {0} وجود ندارد. این فایل در حال استفاده می باشد.";

                case PreviewStringId.Msg_FileReadOnly:
                    return "فایل {0} بصورت فقط خواندنی می باشد. لطفا از نام دیگری استفاده كنید.";

                case PreviewStringId.Msg_OpenFileQuestion:
                    return "آیا میخواهید این فایل را باز كنید؟";

                case PreviewStringId.Msg_OpenFileQuestionCaption:
                    return "استخراج";

                case PreviewStringId.Msg_CantFitBarcodeToControlBounds:
                    return "اندازه كنترلر برای باركد كوچك می باشد.";

                case PreviewStringId.Msg_InvalidBarcodeText:
                    return "از كاراكتر های غیر مجاز در این متن استفاده شده است.";

                case PreviewStringId.Msg_InvalidBarcodeTextFormat:
                    return "متن معتبر نمی باشد.";

                case PreviewStringId.Msg_InvalidBarcodeData:
                    return "امكان استفاده از داده های باینری بیشتر از 1033 بایت وجود ندارد.";

                case PreviewStringId.Msg_InvPropName:
                    return "نام صفت معتبر نمی باشد.";

                case PreviewStringId.Msg_SearchDialogFinishedSearching:
                    return "به آخر جستجو رسیدیم !";

                case PreviewStringId.Msg_SearchDialogTotalFound:
                    return "تعداد یافت شده :";

                case PreviewStringId.Msg_SearchDialogReady:
                    return "آماده";

                case PreviewStringId.Msg_NoDifferentFilesInStream:
                    return "A document can't be exported to a stream in the DifferentFiles mode. Use the SingleFile or SingleFilePageByPage mode instead.";

                case PreviewStringId.Msg_BigFileToCreate:
                    return "فایل خروجی بسیار بزرگ می باشد. لطفا از صفحات كمتر و یا فایل های بیشتر استفاده كنید.";

                case PreviewStringId.Msg_BigFileToCreateJPEG:
                    return "به علت ابعاد بزرگ، امكان ذخیره به فرمت JPEG وجود ندارد. لطفا از فرمت دیگری استفاده كنید.";

                case PreviewStringId.Msg_BigBitmapToCreate:
                    return "";

                case PreviewStringId.Msg_XlsMoreThanMaxRows:
                    return string.Format("حجم فایل بیش از ظرفیت فایل XLS می باشد. (محدود به 65536 ردیف){0}لطفا از فرمت XLSX استفاده كنید.", Environment.NewLine);

                case PreviewStringId.Msg_XlsMoreThanMaxColumns:
                    return string.Format("حجم فایل بیش از ظرفیت فایل XLS می باشد. (محدود به 256 ستون){0}لطفا از فرمت XLSX استفاده كنید.", Environment.NewLine);

                case PreviewStringId.Msg_XlsxMoreThanMaxRows:
                    return "حجم فایل بیش از ظرفیت فایل XLSX می باشد. (محدود به 1,048,576 ردیف)";

                case PreviewStringId.Msg_XlsxMoreThanMaxColumns:
                    return "حجم فایل بیش از ظرفیت فایل XLSX می باشد. (محدود به 16,384 ستون)";


                case PreviewStringId.Msg_FileDoesNotHavePrnxExtention:
                    return "پسوند فایل PRNX نمی باشد. ادامه دهیم؟";


                case PreviewStringId.Msg_FileDoesNotContainValidXml:
                    return "این فایل معتبر نمی باشد. عملیات متوقف شد.";

                case PreviewStringId.Msg_GoToNonExistentPage:
                    return "صفحه ای با شماره {0} در این سند وجود ندارد.";

                case PreviewStringId.Msg_Caption:
                    return "در حال چاپ";

                case PreviewStringId.Msg_PathTooLong:
                    return "مسیر فایل بسیار بزرگ می باشد. لطفا از نام و مسیر كوتاهتر استفاده كنید.";

                case PreviewStringId.Msg_CannotLoadDocument:
                    return "به علت حجم ویا نامعتبر بودن فایل امكان بازكردن فایل وجود ندارد.";

                case PreviewStringId.Msg_NoParameters:
                    return "این پارامتر وجود ندارد : {0}";

                case PreviewStringId.Margin_Inch:
                    return "اینچ";

                case PreviewStringId.Margin_Millimeter:
                    return "میلیمتر";

                case PreviewStringId.Margin_TopMargin:
                    return "حاشیه بالا";

                case PreviewStringId.Margin_BottomMargin:
                    return "حاشیه پایین";

                case PreviewStringId.Margin_LeftMargin:
                    return "حاشیه چپ";

                case PreviewStringId.Margin_RightMargin:
                    return "حاشیه راست";

                case PreviewStringId.Shapes_Rectangle:
                    return "مستطیل";

                case PreviewStringId.Shapes_Ellipse:
                    return "بیضی";

                case PreviewStringId.Shapes_Arrow:
                    return "نشانگر";

                case PreviewStringId.Shapes_TopArrow:
                    return "نشانگر بالا";

                case PreviewStringId.Shapes_BottomArrow:
                    return "نشانگر پایین";

                case PreviewStringId.Shapes_LeftArrow:
                    return "نشانگر چپ";

                case PreviewStringId.Shapes_RightArrow:
                    return "نشانگر راست";

                case PreviewStringId.Shapes_Polygon:
                    return "چند ضلعی";

                case PreviewStringId.Shapes_Triangle:
                    return "مثلث";

                case PreviewStringId.Shapes_Square:
                    return "مربع";

                case PreviewStringId.Shapes_Pentagon:
                    return "پنج ضلعی";

                case PreviewStringId.Shapes_Hexagon:
                    return "شش ضلعی";

                case PreviewStringId.Shapes_Octagon:
                    return "هشت ضلعی";

                case PreviewStringId.Shapes_Star:
                    return "ستاره";

                case PreviewStringId.Shapes_ThreePointStar:
                    return "ستاره 3 گوش";

                case PreviewStringId.Shapes_FourPointStar:
                    return "ستاره 4 گوش";

                case PreviewStringId.Shapes_FivePointStar:
                    return "ستاره 5 گوش";

                case PreviewStringId.Shapes_SixPointStar:
                    return "ستاره 6 گوش";

                case PreviewStringId.Shapes_EightPointStar:
                    return "ستاره 8 گوش";

                case PreviewStringId.Shapes_Line:
                    return "خط";

                case PreviewStringId.Shapes_SlantLine:
                    return "خط اریب";

                case PreviewStringId.Shapes_BackslantLine:
                    return "خط اریب معكوس";

                case PreviewStringId.Shapes_HorizontalLine:
                    return "خط افقی";

                case PreviewStringId.Shapes_VerticalLine:
                    return "خط عمودی";

                case PreviewStringId.Shapes_Cross:
                    return "ضربدر";

                case PreviewStringId.Shapes_Brace:
                    return "خط بریس";

                case PreviewStringId.Shapes_Bracket:
                    return "كروشه";


                case PreviewStringId.ScrollingInfo_Page:
                    return "صفحه";

                case PreviewStringId.WMForm_PictureDlg_Title:
                    return "انتخاب عكس";

                case PreviewStringId.WMForm_ImageStretch:
                    return "كشیده";

                case PreviewStringId.WMForm_ImageClip:
                    return "برش";

                case PreviewStringId.WMForm_ImageZoom:
                    return "بزرگنمایی";

                case PreviewStringId.WMForm_Watermark_Asap:
                    return "به زودی";

                case PreviewStringId.WMForm_Watermark_Confidential:
                    return "محرمانه";

                case PreviewStringId.WMForm_Watermark_Copy:
                    return "نسخه كپی";

                case PreviewStringId.WMForm_Watermark_DoNotCopy:
                    return "كپی نكنید";

                case PreviewStringId.WMForm_Watermark_Draft:
                    return "چرك نویس";

                case PreviewStringId.WMForm_Watermark_Evaluation:
                    return "آزمایشی";

                case PreviewStringId.WMForm_Watermark_Original:
                    return "اصلی";

                case PreviewStringId.WMForm_Watermark_Personal:
                    return "شخصی";

                case PreviewStringId.WMForm_Watermark_Sample:
                    return "نمونه";

                case PreviewStringId.WMForm_Watermark_TopSecret:
                    return "بسیار محرمانه";

                case PreviewStringId.WMForm_Watermark_Urgent:
                    return "فوری";

                case PreviewStringId.WMForm_Direction_Horizontal:
                    return "افقی";

                case PreviewStringId.WMForm_Direction_Vertical:
                    return "عمودی";

                case PreviewStringId.WMForm_Direction_BackwardDiagonal:
                    return "مورب به عقب";

                case PreviewStringId.WMForm_Direction_ForwardDiagonal:
                    return "مورب به جلو";

                case PreviewStringId.WMForm_VertAlign_Bottom:
                    return "پایین";

                case PreviewStringId.WMForm_VertAlign_Middle:
                    return "وسط";

                case PreviewStringId.WMForm_VertAlign_Top:
                    return "بالا";

                case PreviewStringId.WMForm_HorzAlign_Left:
                    return "چپ";

                case PreviewStringId.WMForm_HorzAlign_Center:
                    return "وسط";

                case PreviewStringId.WMForm_HorzAlign_Right:
                    return "راست";

                case PreviewStringId.WMForm_ZOrderRgrItem_InFront:
                    return "به جلو";

                case PreviewStringId.WMForm_ZOrderRgrItem_Behind:
                    return "عقب";

                case PreviewStringId.WMForm_PageRangeRgrItem_All:
                    return "همه";

                case PreviewStringId.WMForm_PageRangeRgrItem_Pages:
                    return "صفحات:";


                case PreviewStringId.SaveDlg_Title:
                    return "ذخیره به";

                case PreviewStringId.SaveDlg_FilterPdf:
                    return "فایل PDF";

                case PreviewStringId.SaveDlg_FilterHtm:
                    return "فایل HTML";

                case PreviewStringId.SaveDlg_FilterMht:
                    return "فایل MHT";

                case PreviewStringId.SaveDlg_FilterRtf:
                    return "فایل RTF";

                case PreviewStringId.SaveDlg_FilterXls:
                    return "فایل XLS";

                case PreviewStringId.SaveDlg_FilterXlsx:
                    return "فایل XLSX";

                case PreviewStringId.SaveDlg_FilterCsv:
                    return "فایل CSV";

                case PreviewStringId.SaveDlg_FilterTxt:
                    return "فایل متنی";

                case PreviewStringId.SaveDlg_FilterNativeFormat:
                    return "فرمت اصلی";

                case PreviewStringId.SaveDlg_FilterXps:
                    return "فایل XPS";

                case PreviewStringId.MenuItem_File:
                    return "فایل";

                case PreviewStringId.MenuItem_View:
                    return "نمایش";

                case PreviewStringId.MenuItem_Background:
                    return "پشت زمینه";

                case PreviewStringId.MenuItem_PageSetup:
                    return "تنظیمات صفحه ...";

                case PreviewStringId.MenuItem_Print:
                    return "چاپ ...";

                case PreviewStringId.MenuItem_PrintDirect:
                    return "چاپ";

                case PreviewStringId.MenuItem_Export:
                    return "استخراج به";

                case PreviewStringId.MenuItem_Send:
                    return "ارسال به";

                case PreviewStringId.MenuItem_Exit:
                    return "خروج";

                case PreviewStringId.MenuItem_ViewToolbar:
                    return "نوار ابزار";

                case PreviewStringId.MenuItem_ViewStatusbar:
                    return "نوار وضعیت";

                case PreviewStringId.MenuItem_ViewContinuous:
                    return "پیوسته";

                case PreviewStringId.MenuItem_ViewFacing:
                    return "صفحه به صفحه";

                case PreviewStringId.MenuItem_BackgrColor:
                    return "رنگ";

                case PreviewStringId.MenuItem_Watermark:
                    return "واتر مارك";

                case PreviewStringId.MenuItem_ZoomPageWidth:
                    return "پهنای صفحه";

                case PreviewStringId.MenuItem_ZoomTextWidth:
                    return "پهنای متن";

                case PreviewStringId.MenuItem_ZoomWholePage:
                    return "تمام صفحه";

                case PreviewStringId.MenuItem_ZoomTwoPages:
                    return "دو صفحه";

                case PreviewStringId.PageInfo_PageNumber:
                    return "[Page #]";

                case PreviewStringId.PageInfo_PageNumberOfTotal:
                    return "[Page # of Pages #]";

                case PreviewStringId.PageInfo_PageTotal:
                    return "[Pages #]";

                case PreviewStringId.PageInfo_PageDate:
                    return "[Date Printed]";

                case PreviewStringId.PageInfo_PageTime:
                    return "[Time Printed]";

                case PreviewStringId.PageInfo_PageUserName:
                    return "[User Name]";


                case PreviewStringId.EMail_From:
                    return "از";

                case PreviewStringId.BarText_Toolbar:
                    return "نوار ابزار";

                case PreviewStringId.BarText_MainMenu:
                    return "منوی اصلی";

                case PreviewStringId.BarText_StatusBar:
                    return "نوار وضعیت";

                case PreviewStringId.ScalePopup_GroupText:
                    return "تغییر مقیاس";

                case PreviewStringId.ScalePopup_AdjustTo:
                    return "تنظیم به";

                case PreviewStringId.ScalePopup_NormalSize:
                    return "% اندازه واقعی";

                case PreviewStringId.ScalePopup_FitTo:
                    return "متناسب با";

                case PreviewStringId.ScalePopup_PagesWide:
                    return "پهنای صفحه";

                case PreviewStringId.ExportOption_PdfPageRange:
                    return "بازه صفحات:";

                case PreviewStringId.ExportOption_PdfConvertImagesToJpeg:
                    return "تبدیل تصویر به Jpeg";

                case PreviewStringId.ExportOption_PdfCompressed:
                    return "فشرده";

                case PreviewStringId.ExportOption_PdfShowPrintDialogOnOpen:
                    return "نمایش پنجره چاپ هنگام باز كردن";

                case PreviewStringId.ExportOption_PdfNeverEmbeddedFonts:
                    return "این قلم ها را ادقام نكن:";

                case PreviewStringId.ExportOption_PdfPasswordSecurityOptions:
                    return "رمز امنیتی";

                case PreviewStringId.ExportOption_PdfImageQuality:
                    return "كیفیت تصاویر:";

                case PreviewStringId.ExportOption_PdfImageQuality_Lowest:
                    return "كمترین";

                case PreviewStringId.ExportOption_PdfImageQuality_Low:
                    return "كم";

                case PreviewStringId.ExportOption_PdfImageQuality_Medium:
                    return "متوسط";

                case PreviewStringId.ExportOption_PdfImageQuality_High:
                    return "بالا";

                case PreviewStringId.ExportOption_PdfImageQuality_Highest:
                    return "بیشترین";

                case PreviewStringId.ExportOption_PdfDocumentAuthor:
                    return "نویسنده:";

                case PreviewStringId.ExportOption_PdfDocumentApplication:
                    return "در خواست:";

                case PreviewStringId.ExportOption_PdfDocumentTitle:
                    return "عنوان:";

                case PreviewStringId.ExportOption_PdfDocumentSubject:
                    return "موضوع:";

                case PreviewStringId.ExportOption_PdfDocumentKeywords:
                    return "كلمات كلیدی:";

                case PreviewStringId.ExportOption_PdfPrintingPermissions_None:
                    return "هیچ";

                case PreviewStringId.ExportOption_PdfPrintingPermissions_LowResolution:
                    return "دقت پایین 150 dpi";

                case PreviewStringId.ExportOption_PdfPrintingPermissions_HighResolution:
                    return "دقت بالا";

                case PreviewStringId.ExportOption_PdfChangingPermissions_None:
                    return "هیچ";

                case PreviewStringId.ExportOption_PdfChangingPermissions_InsertingDeletingRotating:
                    return "اضافه، حذف و چرخش صفحات";

                case PreviewStringId.ExportOption_PdfChangingPermissions_FillingSigning:
                    return "پر كردن اطلاعات و امكان ثبت امضاء";

                case PreviewStringId.ExportOption_PdfChangingPermissions_CommentingFillingSigning:
                    return "ثبت نكته، پر كردن اطلاعات و امكان ثبت امضاء";

                case PreviewStringId.ExportOption_PdfChangingPermissions_AnyExceptExtractingPages:
                    return "همه چیز غیر از استخراج صفحه";

                case PreviewStringId.ExportOption_ConfirmOpenPasswordForm_Caption:
                    return "رمز مجدد";
















                case PreviewStringId.ExportOption_ConfirmOpenPasswordForm_Note:
                    return "";

                case PreviewStringId.ExportOption_ConfirmOpenPasswordForm_Name:
                    return "";

                case PreviewStringId.ExportOption_ConfirmPermissionsPasswordForm_Caption:
                    return "";

                case PreviewStringId.ExportOption_ConfirmPermissionsPasswordForm_Note:
                    return "";

                case PreviewStringId.ExportOption_ConfirmPermissionsPasswordForm_Name:
                    return "";

                case PreviewStringId.ExportOption_ConfirmationDoesNotMatchForm_Msg:
                    return "";

                case PreviewStringId.ExportOption_HtmlExportMode:
                    return "";

                case PreviewStringId.ExportOption_HtmlExportMode_SingleFile:
                    return "";

                case PreviewStringId.ExportOption_HtmlExportMode_SingleFilePageByPage:
                    return "";

                case PreviewStringId.ExportOption_HtmlExportMode_DifferentFiles:
                    return "";

                case PreviewStringId.ExportOption_HtmlCharacterSet:
                    return "";

                case PreviewStringId.ExportOption_HtmlTitle:
                    return "";

                case PreviewStringId.ExportOption_HtmlRemoveSecondarySymbols:
                    return "";

                case PreviewStringId.ExportOption_HtmlEmbedImagesInHTML:
                    return "";

                case PreviewStringId.ExportOption_HtmlPageRange:
                    return "";

                case PreviewStringId.ExportOption_HtmlPageBorderWidth:
                    return "";

                case PreviewStringId.ExportOption_HtmlPageBorderColor:
                    return "";

                case PreviewStringId.ExportOption_RtfExportMode:
                    return "";

                case PreviewStringId.ExportOption_RtfExportMode_SingleFile:
                    return "";

                case PreviewStringId.ExportOption_RtfExportMode_SingleFilePageByPage:
                    return "";

                case PreviewStringId.ExportOption_RtfPageRange:
                    return "";

                case PreviewStringId.ExportOption_RtfExportWatermarks:
                    return "";

                case PreviewStringId.ExportOption_TextSeparator:
                    return "";

                case PreviewStringId.ExportOption_TextSeparator_TabAlias:
                    return "";

                case PreviewStringId.ExportOption_TextEncoding:
                    return "";

                case PreviewStringId.ExportOption_TextQuoteStringsWithSeparators:
                    return "";

                case PreviewStringId.ExportOption_TextExportMode:
                    return "";

                case PreviewStringId.ExportOption_TextExportMode_Value:
                    return "";

                case PreviewStringId.ExportOption_TextExportMode_Text:
                    return "";

                case PreviewStringId.ExportOption_XlsShowGridLines:
                    return "";

                case PreviewStringId.ExportOption_XlsUseNativeFormat:
                    return "";

                case PreviewStringId.ExportOption_XlsExportHyperlinks:
                    return "";

                case PreviewStringId.ExportOption_XlsSheetName:
                    return "";

                case PreviewStringId.ExportOption_XlsExportMode:
                    return "";

                case PreviewStringId.ExportOption_XlsExportMode_SingleFile:
                    return "";

                case PreviewStringId.ExportOption_XlsExportMode_DifferentFiles:
                    return "";

                case PreviewStringId.ExportOption_XlsPageRange:
                    return "";

                case PreviewStringId.ExportOption_XlsxExportMode:
                    return "";

                case PreviewStringId.ExportOption_XlsxExportMode_SingleFile:
                    return "";

                case PreviewStringId.ExportOption_XlsxExportMode_SingleFilePageByPage:
                    return "";

                case PreviewStringId.ExportOption_XlsxExportMode_DifferentFiles:
                    return "";

                case PreviewStringId.ExportOption_XlsxPageRange:
                    return "";

                case PreviewStringId.ExportOption_ImageExportMode:
                    return "";

                case PreviewStringId.ExportOption_ImageExportMode_SingleFile:
                    return "";

                case PreviewStringId.ExportOption_ImageExportMode_SingleFilePageByPage:
                    return "";

                case PreviewStringId.ExportOption_ImageExportMode_DifferentFiles:
                    return "";

                case PreviewStringId.ExportOption_ImagePageRange:
                    return "";

                case PreviewStringId.ExportOption_ImagePageBorderWidth:
                    return "";

                case PreviewStringId.ExportOption_ImagePageBorderColor:
                    return "";

                case PreviewStringId.ExportOption_ImageFormat:
                    return "";

                case PreviewStringId.ExportOption_ImageResolution:
                    return "";

                case PreviewStringId.ExportOption_NativeFormatCompressed:
                    return "";

                case PreviewStringId.ExportOption_XpsPageRange:
                    return "";

                case PreviewStringId.ExportOption_XpsCompression:
                    return "";

                case PreviewStringId.ExportOption_XpsCompression_NotCompressed:
                    return "";

                case PreviewStringId.ExportOption_XpsCompression_Normal:
                    return "";

                case PreviewStringId.ExportOption_XpsCompression_Maximum:
                    return "";

                case PreviewStringId.ExportOption_XpsCompression_Fast:
                    return "";

                case PreviewStringId.ExportOption_XpsCompression_SuperFast:
                    return "";

                case PreviewStringId.ExportOption_XpsDocumentCreator:
                    return "";

                case PreviewStringId.ExportOption_XpsDocumentCategory:
                    return "";

                case PreviewStringId.ExportOption_XpsDocumentTitle:
                    return "";

                case PreviewStringId.ExportOption_XpsDocumentSubject:
                    return "";

                case PreviewStringId.ExportOption_XpsDocumentKeywords:
                    return "";

                case PreviewStringId.ExportOption_XpsDocumentVersion:
                    return "";

                case PreviewStringId.ExportOption_XpsDocumentDescription:
                    return "";

                case PreviewStringId.FolderBrowseDlg_ExportDirectory:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionPdf:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionXls:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionXlsx:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionTxt:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionCsv:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionImage:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionHtml:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionMht:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionRtf:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionNativeOptions:
                    return "";

                case PreviewStringId.ExportOptionsForm_CaptionXps:
                    return "";

                case PreviewStringId.RibbonPreview_PageText:
                    return "";

                case PreviewStringId.RibbonPreview_PageGroup_Print:
                    return "";

                case PreviewStringId.RibbonPreview_PageGroup_PageSetup:
                    return "";

                case PreviewStringId.RibbonPreview_PageGroup_Navigation:
                    return "";

                case PreviewStringId.RibbonPreview_PageGroup_Zoom:
                    return "";

                case PreviewStringId.RibbonPreview_PageGroup_Background:
                    return "";

                case PreviewStringId.RibbonPreview_PageGroup_Export:
                    return "";

                case PreviewStringId.RibbonPreview_PageGroup_Document:
                    return "";

                case PreviewStringId.RibbonPreview_DocumentMap_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Parameters_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Find_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Pointer_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_HandTool_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Customize_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Print_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_PrintDirect_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_PageSetup_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_EditPageHF_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Magnifier_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ZoomOut_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ZoomExact_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ZoomIn_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ShowFirstPage_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ShowPrevPage_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ShowNextPage_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ShowLastPage_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_MultiplePages_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_FillBackground_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Watermark_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportFile_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_SendFile_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ClosePreview_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Scale_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_PageOrientation_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_PaperSize_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_PageMargins_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Zoom_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Save_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_Open_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_DocumentMap_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Parameters_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Find_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Pointer_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_HandTool_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Customize_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Print_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_PrintDirect_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_PageSetup_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_EditPageHF_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Magnifier_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ZoomOut_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ZoomIn_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ShowFirstPage_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ShowPrevPage_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ShowNextPage_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ShowLastPage_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_MultiplePages_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_FillBackground_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Watermark_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ExportFile_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_SendFile_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ClosePreview_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Scale_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_PageOrientation_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_PaperSize_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_PageMargins_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Zoom_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_PageGroup_PageSetup_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Save_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_Open_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_DocumentMap_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Parameters_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Find_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Pointer_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_HandTool_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Customize_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Print_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_PrintDirect_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_PageSetup_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_EditPageHF_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Magnifier_STipContent:

                    return "";

                case PreviewStringId.RibbonPreview_ZoomOut_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ZoomIn_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ShowFirstPage_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ShowPrevPage_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ShowNextPage_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ShowLastPage_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_MultiplePages_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_FillBackground_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Watermark_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ExportFile_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_SendFile_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ClosePreview_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Scale_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_PageOrientation_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_PaperSize_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_PageMargins_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Zoom_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_PageGroup_PageSetup_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Save_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_Open_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ExportPdf_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportHtm_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportMht_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportRtf_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportXls_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportXlsx_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportCsv_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportTxt_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportGraphic_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportXps_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_SendPdf_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_SendMht_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_SendRtf_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_SendXls_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_SendXlsx_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_SendCsv_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_SendTxt_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_SendGraphic_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_SendXps_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_ExportPdf_Description:
                    return "";

                case PreviewStringId.RibbonPreview_ExportHtm_Description:
                    return "";

                case PreviewStringId.RibbonPreview_ExportTxt_Description:
                    return "";

                case PreviewStringId.RibbonPreview_ExportCsv_Description:
                    return "";

                case PreviewStringId.RibbonPreview_ExportMht_Description:
                    return "";

                case PreviewStringId.RibbonPreview_ExportXls_Description:
                    return "";

                case PreviewStringId.RibbonPreview_ExportXlsx_Description:
                    return "";

                case PreviewStringId.RibbonPreview_ExportRtf_Description:
                    return "";

                case PreviewStringId.RibbonPreview_ExportGraphic_Description:
                    return "";

                case PreviewStringId.RibbonPreview_ExportXps_Description:
                    return "";

                case PreviewStringId.RibbonPreview_SendPdf_Description:
                    return "";

                case PreviewStringId.RibbonPreview_SendTxt_Description:
                    return "";

                case PreviewStringId.RibbonPreview_SendCsv_Description:
                    return "";

                case PreviewStringId.RibbonPreview_SendMht_Description:
                    return "";

                case PreviewStringId.RibbonPreview_SendXls_Description:
                    return "";

                case PreviewStringId.RibbonPreview_SendXlsx_Description:
                    return "";

                case PreviewStringId.RibbonPreview_SendRtf_Description:
                    return "";

                case PreviewStringId.RibbonPreview_SendGraphic_Description:
                    return "";

                case PreviewStringId.RibbonPreview_SendXps_Description:
                    return "";

                case PreviewStringId.RibbonPreview_ExportPdf_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ExportHtm_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ExportTxt_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ExportCsv_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ExportMht_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ExportXls_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ExportXlsx_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ExportRtf_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ExportGraphic_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_SendPdf_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_SendTxt_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_SendCsv_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_SendMht_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_SendXls_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_SendXlsx_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_SendRtf_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_SendGraphic_STipTitle:
                    return "";

                case PreviewStringId.RibbonPreview_ExportPdf_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ExportHtm_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ExportTxt_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ExportCsv_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ExportMht_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ExportXls_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ExportXlsx_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ExportRtf_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_ExportGraphic_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_SendPdf_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_SendTxt_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_SendCsv_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_SendMht_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_SendXls_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_SendXlsx_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_SendRtf_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_SendGraphic_STipContent:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageOrientationPortrait_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageOrientationLandscape_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageMarginsNormal_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageMarginsNarrow_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageMarginsModerate_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageMarginsWide_Caption:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageOrientationPortrait_Description:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageOrientationLandscape_Description:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageMarginsNormal_Description:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageMarginsNarrow_Description:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageMarginsModerate_Description:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageMarginsWide_Description:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PageMargins_Description:
                    return "";

                case PreviewStringId.RibbonPreview_GalleryItem_PaperSize_Description:
                    return "";

                case PreviewStringId.OpenFileDialog_Filter:
                    return "";

                case PreviewStringId.OpenFileDialog_Title:
                    return "";

                case PreviewStringId.ExportOption_PdfPasswordSecurityOptions_DocumentOpenPassword:
                    return "";

                case PreviewStringId.ExportOption_PdfPasswordSecurityOptions_Permissions:
                    return "";

                case PreviewStringId.ExportOption_PdfPasswordSecurityOptions_None:
                    return "";

                case PreviewStringId.ParametersRequest_Submit:
                    return "";

                case PreviewStringId.ParametersRequest_Reset:
                    return "";

                case PreviewStringId.ParametersRequest_Caption:
                    return "";

                case PreviewStringId.NoneString:
                    return "";

                case PreviewStringId.WatermarkTypePicture:
                    return "";

                case PreviewStringId.WatermarkTypeText:
                    return "";

                case PreviewStringId.ParameterLookUpSettingsNoLookUp:
                    return "";


            }
            return "";
        }
    }
}
