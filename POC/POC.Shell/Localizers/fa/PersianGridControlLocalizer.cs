using DevExpress.Xpf.Grid;

namespace POC.Shell.Localizers.fa
{
    internal class PersianGridControlLocalizer : GridControlLocalizer
    {
        public override string GetLocalizedString(GridControlStringId id)
        {
            switch (id)
            {
                   
                case GridControlStringId.CellPeerName:
                    return "Item: {0}, Column Display Index: {1}";
                case GridControlStringId.GridGroupPanelText:
                    return "جهت گروه بندی، ستون مورد نظر را بر روی این قسمت بكشید.";
                case GridControlStringId.GridGroupRowDisplayTextFormat:
                    return "{0}: {1}";
                case GridControlStringId.ErrorWindowTitle:
                    return "خطا";
                case GridControlStringId.InvalidRowExceptionMessage:
                    return "آیا میخواهید مقدار را تصحیح كنید؟";
                case GridControlStringId.GridOutlookIntervals:
                    break;
                case GridControlStringId.DefaultGroupSummaryFormatString_Count:
                    return "تعداد={0}";
                case GridControlStringId.DefaultGroupSummaryFormatString_Min:
                    return "حداقل {1} برابر با {0}";
                case GridControlStringId.DefaultGroupSummaryFormatString_Max:
                    return "حداكثر {1} برابر با {0}";
                case GridControlStringId.DefaultGroupSummaryFormatString_Avg:
                    return "میانگین {1} برابر با {0:0.##}";
                case GridControlStringId.DefaultGroupSummaryFormatString_Sum:
                    return "جمع {1} برابر با {0:0.##}";
                case GridControlStringId.DefaultTotalSummaryFormatStringInSameColumn_Count:
                    return "تعداد={0}";
                case GridControlStringId.DefaultTotalSummaryFormatStringInSameColumn_Min:
                    return "حداقل={0}";
                case GridControlStringId.DefaultTotalSummaryFormatStringInSameColumn_Max:
                    return "حداكثر={0}";
                case GridControlStringId.DefaultTotalSummaryFormatStringInSameColumn_Avg:
                    return "میانگین={0:0.##}";
                case GridControlStringId.DefaultTotalSummaryFormatStringInSameColumn_Sum:
                    return "جمع={0:0.##}";
                case GridControlStringId.DefaultTotalSummaryFormatString_Count:
                    return "تعداد={0}";
                case GridControlStringId.DefaultTotalSummaryFormatString_Min:
                    return "حداقل {1} برابر با {0}";
                case GridControlStringId.DefaultTotalSummaryFormatString_Max:
                    return "حداقل {1} برابر با {0}";
                case GridControlStringId.DefaultTotalSummaryFormatString_Avg:
                    return "میانگین {1} برابر با {0:0.##}";
                case GridControlStringId.DefaultTotalSummaryFormatString_Sum:
                    return "جمع {1} برابر با {0:0.##}";
                case GridControlStringId.DefaultGroupColumnSummaryFormatStringInSameColumn_Count:
                    return "تعداد={0}";
                case GridControlStringId.DefaultGroupColumnSummaryFormatStringInSameColumn_Min:
                    return "حداقل={0}";
                case GridControlStringId.DefaultGroupColumnSummaryFormatStringInSameColumn_Max:
                    return "حداكثر={0}";
                case GridControlStringId.DefaultGroupColumnSummaryFormatStringInSameColumn_Avg:
                    return "میانگین={0:0.##}";
                case GridControlStringId.DefaultGroupColumnSummaryFormatStringInSameColumn_Sum:
                    return "جمع={0:0.##}";
                case GridControlStringId.DefaultGroupColumnSummaryFormatString_Count:
                    return "تعداد={0}";
                case GridControlStringId.DefaultGroupColumnSummaryFormatString_Min:
                    return "حداقل {1} برابر با {0}";
                case GridControlStringId.DefaultGroupColumnSummaryFormatString_Max:
                    return "حداقل {1} برابر با {0}";
                case GridControlStringId.DefaultGroupColumnSummaryFormatString_Avg:
                    return "میانگین {1} برابر با {0:0.##}";
                case GridControlStringId.DefaultGroupColumnSummaryFormatString_Sum:
                    return "جمع {1} برابر با {0:0.##}";
                case GridControlStringId.PopupFilterAll:
                    return "(همه)";
                case GridControlStringId.PopupFilterBlanks:
                    return "(خالی)";
                case GridControlStringId.PopupFilterNonBlanks:
                    return "(غیر خالی)";
                case GridControlStringId.ColumnChooserCaption:
                    return "انتخابگر ستون";
                case GridControlStringId.ColumnChooserCaptionForMasterDetail:
                    return "{0}: انتخابگر ستون";
                case GridControlStringId.ColumnChooserDragText:
                    return "جهت تنظیمات ستون را بر روی این قسمت بكشید";
                case GridControlStringId.GridNewRowText:
                    return "جهت ایجاد بر روی این قسمت كلیك كنید";
                case GridControlStringId.MenuGroupPanelFullExpand:
                    return "گسترش كامل";
                case GridControlStringId.MenuGroupPanelFullCollapse:
                    return "فرو بستن كامل";
                case GridControlStringId.MenuGroupPanelClearGrouping:
                    return "لغو گروه بندی";
                case GridControlStringId.MenuColumnSortAscending:
                    return "ترتیب صعودی";
                case GridControlStringId.MenuColumnSortDescending:
                    return "ترتیب نزولی";
                case GridControlStringId.MenuColumnSortBySummaryAscending:
                    return "صعودی";
                case GridControlStringId.MenuColumnSortBySummaryDescending:
                    return "نزولی";
                case GridControlStringId.MenuColumnSortBySummaryMax:
                    return "حداكثر";
                case GridControlStringId.MenuColumnSortBySummaryMin:
                    return "حداقل";
                case GridControlStringId.MenuColumnSortBySummaryCount:
                    return "تعداد";
                case GridControlStringId.MenuColumnSortBySummaryAverage:
                    return "میانگین";
                case GridControlStringId.MenuColumnSortBySummarySum:
                    return "جمع";
                case GridControlStringId.MenuColumnClearSorting:
                    return "بدون ترتیب";
                case GridControlStringId.MenuColumnUnGroup:
                    return "بدون گروه بندی";
                case GridControlStringId.MenuColumnGroup:
                    return "گروه بر اساس این ستون";
                case GridControlStringId.MenuColumnShowGroupPanel:
                    return "نمایش نوار گروه بندی";
                case GridControlStringId.MenuColumnHideGroupPanel:
                    return "حذف نوار گروه بندی";
                case GridControlStringId.MenuColumnGroupInterval:
                    return "بازه گروه بندی";
                case GridControlStringId.MenuColumnGroupIntervalNone:
                    return "هیچ";
                case GridControlStringId.MenuColumnGroupIntervalDay:
                    return "روز";
                case GridControlStringId.MenuColumnGroupIntervalMonth:
                    return "ماه";
                case GridControlStringId.MenuColumnGroupIntervalYear:
                    return "سال";
                case GridControlStringId.MenuColumnGroupIntervalSmart:
                    return "هوشمند";
                case GridControlStringId.MenuColumnShowColumnChooser:
                    return "نمایش انتخابگر ستون";
                case GridControlStringId.MenuColumnHideColumnChooser:
                    return "پنهان انتخابگر ستون";
                case GridControlStringId.MenuColumnResetGroupSummarySort:
                    return "حذف ترتیب جمع بندی";
                case GridControlStringId.MenuColumnSortGroupBySummaryMenu:
                    return "ترتیب بر اساس جمع بندی";
                case GridControlStringId.MenuColumnGroupSummarySortFormat:
                    return "{1} بر اساس '{0}' - {2}";
                case GridControlStringId.MenuColumnGroupSummaryEditor:
                    return "ویرایشگر گروه جمع بندی ...";
                case GridControlStringId.MenuColumnBestFit:
                    return "بهترین اندازه";
                case GridControlStringId.MenuColumnBestFitColumns:
                    return "بهترین اندازه (همه ستونها)";
                case GridControlStringId.MenuColumnUnboundExpressionEditor:
                    return "ویرایشگر بیانساز ";
                case GridControlStringId.MenuColumnClearFilter:
                    return "حذف فیلتر";
                case GridControlStringId.MenuColumnFilterEditor:
                    return "ویرایشگر فیلتر";
                case GridControlStringId.MenuColumnFixedStyle:
                    return "سبك ستون ثابت";
                case GridControlStringId.MenuColumnFixedNone:
                    return "غیر فعال";
                case GridControlStringId.MenuColumnFixedLeft:
                    return "چپ";
                case GridControlStringId.MenuColumnFixedRight:
                    return "راست";
                case GridControlStringId.MenuColumnShowSearchPanel:
                    return "نمایش جستجو گر";
                case GridControlStringId.MenuColumnHideSearchPanel:
                    return "پنهان جستجوگر";
                case GridControlStringId.MenuFooterSum:
                    return "جمع";
                case GridControlStringId.MenuFooterMin:
                    return "حداقل";
                case GridControlStringId.MenuFooterMax:
                    return "حداكثر";
                case GridControlStringId.MenuFooterCount:
                    return "تعداد";
                case GridControlStringId.MenuFooterAverage:
                    return "میانگین";
                case GridControlStringId.MenuFooterCustomize:
                    return "سفارشی";
                case GridControlStringId.MenuFooterRowCount:
                    return "نمایش جمع ستون";
                case GridControlStringId.GroupSummaryEditorFormCaption:
                    return "خلاصه گروه";
                case GridControlStringId.TotalSummaryEditorFormCaption:
                    return "جمع برای '{0}'";
                case GridControlStringId.TotalSummaryPanelEditorFormCaption:
                    return "نمایش جمع بندی";
                case GridControlStringId.SummaryEditorFormItemsTabCaption:
                    return "موارد";
                case GridControlStringId.SummaryEditorFormOrderTabCaption:
                    return "ترتیب";
                case GridControlStringId.SummaryEditorFormOrderLeftSide:
                    return "سمت چپ";
                case GridControlStringId.SummaryEditorFormOrderRightSide:
                    return "سمت راست";
                case GridControlStringId.SummaryEditorFormOrderAndAlignmentTabCaption:
                    return "ترتیب و چیدمان";
                case GridControlStringId.SummaryEditorFormMoveItemUpCaption:
                    return "بالا";
                case GridControlStringId.SummaryEditorFormMoveItemDownCaption:
                    return "پایین";
                case GridControlStringId.FilterEditorTitle:
                    return "ویرایشگر فیلتر";
                case GridControlStringId.FilterPanelCaptionFormatStringForMasterDetail:
                    return "{0} فیلتر:";
                case GridControlStringId.GroupPanelDisplayFormatStringForMasterDetail:
                    return "{0}:";
                case GridControlStringId.ErrorPanelTextFormatString:
                    return "بروز خطا طی پردازش درخواست سرور ({0})";
                case GridControlStringId.ProgressWindowTitle:
                    return "در حال دریافت اطلاعات";
                case GridControlStringId.ProgressWindowCancel:
                    return "لغو";
                
            }
            return GetLocalizedString(id);
        }
    }
}
