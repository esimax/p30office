using System;
using DevExpress.Xpf.Scheduler;

namespace POC.Shell.Localizers.fa
{
    internal class PersianSchedulerControlLocalizer : SchedulerControlLocalizer
    {
        public override string GetLocalizedString(SchedulerControlStringId id)
        {
            switch (id)
            {
                case SchedulerControlStringId.Caption_GotoDate:
                    return "برو به تاریخ";

                case SchedulerControlStringId.Caption_DeleteRecurrentApt:
                    return "تایید حذف";

                case SchedulerControlStringId.Caption_OpenRecurrentApt:
                    return "بازكردن موارد دوره ای";

                case SchedulerControlStringId.Caption_TimeRuler:
                    return "خطكش زمانی";

                case SchedulerControlStringId.Form_RecurrentAppointmentAction_DeleteSeries:
                    return "حذف سری ها";

                case SchedulerControlStringId.Form_RecurrentAppointmentAction_DeleteOccurrence:
                    return "حذف این مورد";

                case SchedulerControlStringId.Form_RecurrentAppointmentAction_EditSeries:
                    return "ویرایش سری";

                case SchedulerControlStringId.Form_RecurrentAppointmentAction_EditOccurrence:
                    return "ویرایش این سری";

                case SchedulerControlStringId.Form_DeleteRecurrentAppointmentFormMessage:
                    return "آیا می خواهید تمام موارد دوره \"{0}\" را حذف كنید, یا فقط همین مورد؟";

                case SchedulerControlStringId.Form_EditRecurrentAppointmentFormMessage:
                    return "آیا می خواهید تمام موارد دوره \"{0}\" را ویرایش كنید, یا فقط همین مورد؟";

                case SchedulerControlStringId.ButtonCaption_DismissAll:

                case SchedulerControlStringId.ButtonCaption_Dismiss:
                    return "عزل كردن";

                case SchedulerControlStringId.ButtonCaption_OpenItem:
                    return "باز كردن";

                case SchedulerControlStringId.ButtonCaption_Snooze:
                    return "چرت زدن";

                case SchedulerControlStringId.Form_ClickSnoozeToBeReminderAgainIn:
                    return "روی چرت زدن كلیك كنید تا دوباره یادآور شوید در :";

                case SchedulerControlStringId.Form_Subject:
                    return "موضوع";

                case SchedulerControlStringId.Form_Location:
                    return "موقعیت";

                case SchedulerControlStringId.Form_StartTime:
                    return "زمان شروع";

                case SchedulerControlStringId.Form_StartDate:
                    return "تاریخ شروع";

                case SchedulerControlStringId.Form_EndTime:
                    return "زمان پایان";

                case SchedulerControlStringId.Form_EndDate:
                    return "تاریخ پایان";

                case SchedulerControlStringId.Form_Recurrence:
                    return "تكرار واقعه";

                case SchedulerControlStringId.Form_Resource:
                    return "منابع";

                case SchedulerControlStringId.Form_Label:
                    return "برچسب";

                case SchedulerControlStringId.Form_ShowTimeAs:
                    return "نوع نمایش :";

                case SchedulerControlStringId.Form_Reminder:
                    return "یادآوری :";

                case SchedulerControlStringId.Form_Description:
                    return "توضیحات :";

                case SchedulerControlStringId.Form_AllDayEvent:
                    return "واقعه تمام روز";

                case SchedulerControlStringId.Form_Date:
                    return "تاریخ :";

                case SchedulerControlStringId.Form_ShowIn:
                    return "نمایش در :";

                case SchedulerControlStringId.ButtonCaption_OK:
                    return "قبول";

                case SchedulerControlStringId.ButtonCaption_Cancel:
                    return "لغو";

                case SchedulerControlStringId.ButtonCaption_Delete:
                    return "حذف";

                case SchedulerControlStringId.ButtonCaption_Recurrence:
                    return "تكرار";

                case SchedulerControlStringId.Form_DayOrDays:
                    return "روز(ها)";

                case SchedulerControlStringId.Form_Every:
                    return "هر";

                case SchedulerControlStringId.Form_EveryWeekday:
                    return "هر روز هفته";

                case SchedulerControlStringId.Form_Day:
                    return "روز";

                case SchedulerControlStringId.Form_OfEvery:
                    return "از هر";

                case SchedulerControlStringId.Form_MonthOrMonths:
                    return "ماه(ها)";

                case SchedulerControlStringId.Form_The:
                    return "از";

                case SchedulerControlStringId.Form_RecurrencePattern:
                    return "نحوه تكرار";

                case SchedulerControlStringId.Form_RangeOfRecurrence:
                    return "پایان تكرار";

                case SchedulerControlStringId.Form_Occurrences:
                    return "رخداد";

                case SchedulerControlStringId.Form_Start:
                    return "شروع :";

                case SchedulerControlStringId.Form_NoEndDate:
                    return "بدون محدودیت";

                case SchedulerControlStringId.Form_EndAfter:
                    return "اتمام پس از :";

                case SchedulerControlStringId.Form_EndBy:
                    return "اتمام در :";

                case SchedulerControlStringId.Form_RecurEvery:
                    return "تكرار هر";

                case SchedulerControlStringId.Form_WeekOrWeeksOn:
                    return "هفته(ها) در :";

                case SchedulerControlStringId.Form_On:
                    return "در";

                case SchedulerControlStringId.Form_OnThe:
                    return "در";

                case SchedulerControlStringId.Form_of:
                    return "از";

                case SchedulerControlStringId.Form_DayNumber:
                    return "شماره روز";

                case SchedulerControlStringId.Form_DayOfWeek:
                    return "روز هفته";

                case SchedulerControlStringId.Form_TimeZone:
                    return "محدوده زمانی";

                case SchedulerControlStringId.Form_CurrentTime:
                    return "زمان فعلی";

                case SchedulerControlStringId.Form_AdjustForDaylightSavingTime:
                    return "تنظیم اتوماتیك محدوده زمانی";

                case SchedulerControlStringId.Form_EveryEditableTextDay:
                    return "Every Editable Text day(s):";

                case SchedulerControlStringId.Form_AccessibleText_RecurEveryEditableTextWeek:
                    return "Recur every Editable Text week(s)";

                case SchedulerControlStringId.Caption_PageViewNavigator:
                    return "View Navigator";

                case SchedulerControlStringId.Caption_PageViewSelector:
                    return "View Selector";

                case SchedulerControlStringId.Caption_PageGroupBy:
                    return "طبقه بندی";

                case SchedulerControlStringId.Caption_GroupViewNavigator:
                    return "View Navigator";

                case SchedulerControlStringId.Caption_GroupViewSelector:
                    return "View Selector";

                case SchedulerControlStringId.Caption_GroupGroupBy:
                    return "طبقه بندی";

                case SchedulerControlStringId.Form_AccessibleText_EveryNDayText:
                    return "Day Editable Text of every Editable Text month(s)";

                case SchedulerControlStringId.Form_AccessibleText_EveryWeekOfMonthText:
                    return "The Editable Text Editable Text of every Editable Text month(s)";

                case SchedulerControlStringId.Form_AccessibleText_EveryNMonthText:
                    return "On Editable Text Editable Text";

                case SchedulerControlStringId.Form_AccessibleText_EveryDayOfWeekText:
                    return "On the Editable Text Editable Text of Editable Text";

                case SchedulerControlStringId.Form_AccessibleText_EndAfterOccurencesText:
                    return "End after Editable Text occurences:";

                case SchedulerControlStringId.Caption_PrintingSettings:

                case SchedulerControlStringId.Form_OpenReportTemplate:
                    return "Open report template (*.schrepx)";

                case SchedulerControlStringId.Form_PrintingSettings_Format:
                    return "Format";

                case SchedulerControlStringId.Form_PrintingSettings_Resources:
                    return "Resources";

                case SchedulerControlStringId.Form_ResourcesKind:
                    return "Resources kind:";

                case SchedulerControlStringId.Form_AvailableResources:
                    return "Available resources:";

                case SchedulerControlStringId.Form_ResourcesToPrint:
                    return "Resources to print";

                case SchedulerControlStringId.Form_PrintUsingCustomCollection:
                    return "Print using the custom collection";

                case SchedulerControlStringId.Form_Preview:
                    return "Preview";
                default:
                    throw new ArgumentOutOfRangeException("id");
            }
        }
    }
}
