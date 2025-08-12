using System;
using DevExpress.XtraScheduler.Localization;

namespace POC.Shell.Localizers.fa
{
    internal class PersianXtraSchedulerLocalizer : SchedulerLocalizer
    {
        public override string GetLocalizedString(SchedulerStringId id)
        {
            switch (id)
            {
                case SchedulerStringId.DefaultToolTipStringFormat_SplitAppointment:
                    return "{0} : step {1}";

                case SchedulerStringId.Msg_IsNotValid:
                    return "'{0}' is not a valid value for '{1}'";

                case SchedulerStringId.Msg_InvalidDayOfWeekForDailyRecurrence:
                    return "Invalid day of week for a daily recurrence. Only WeekDays.EveryDay, WeekDays.WeekendDays and WeekDays.WorkDays are valid in this context.";

                case SchedulerStringId.Msg_InternalError:
                    return "Internal error!";

                case SchedulerStringId.Msg_NoMappingForObject:
                    return "The following required mappings for the object {0} are not assigned";

                case SchedulerStringId.Msg_XtraSchedulerNotAssigned:
                    return "The SchedulerStorage component is not assigned to the SchedulerControl";

                case SchedulerStringId.Msg_InvalidTimeOfDayInterval:
                    return "Invalid duration for the TimeOfDayInterval";

                case SchedulerStringId.Msg_OverflowTimeOfDayInterval:
                    return "Invalid value for the TimeOfDayInterval. Should be less than or equal to a day";

                case SchedulerStringId.Msg_LoadCollectionFromXml:
                    return "The scheduler needs to be in unbound mode to load collection items from xml";

                case SchedulerStringId.AppointmentLabel_None:
                    return "هیچ";

                case SchedulerStringId.AppointmentLabel_Important:
                    return "مهم";

                case SchedulerStringId.AppointmentLabel_Business:
                    return "كاری";

                case SchedulerStringId.AppointmentLabel_Personal:
                    return "شخصی";

                case SchedulerStringId.AppointmentLabel_Vacation:
                    return "مرخصی";

                case SchedulerStringId.AppointmentLabel_MustAttend:
                    return "واجب";

                case SchedulerStringId.AppointmentLabel_TravelRequired:
                    return "سفر";

                case SchedulerStringId.AppointmentLabel_NeedsPreparation:
                    return "مقدمات";

                case SchedulerStringId.AppointmentLabel_Birthday:
                    return "تولد";

                case SchedulerStringId.AppointmentLabel_Anniversary:
                    return "سالگرد";

                case SchedulerStringId.AppointmentLabel_PhoneCall:
                    return "تماس تلفنی";

                case SchedulerStringId.Appointment_StartContinueText:
                    return "از {0}";

                case SchedulerStringId.Appointment_EndContinueText:
                    return "تا {0}";

                case SchedulerStringId.Msg_InvalidEndDate:
                    return "تاریخ وارد شده قبل از تاریخ شروع می باشد.";

                case SchedulerStringId.Caption_Appointment:
                    return "{0} - رویداد";

                case SchedulerStringId.Caption_Event:
                    return "{0} - واقعه";

                case SchedulerStringId.Caption_UntitledAppointment:
                    return "بدون عنوان";

                case SchedulerStringId.Caption_ReadOnly:
                    return " {فقط خواندنی}";

                case SchedulerStringId.Caption_WeekDaysEveryDay:
                    return "روز";

                case SchedulerStringId.Caption_WeekDaysWeekendDays:
                    return "روز آخر هفته";

                case SchedulerStringId.Caption_WeekDaysWorkDays:
                    return "روز هفته";

                case SchedulerStringId.Caption_WeekOfMonthFirst:
                    return "اولین";

                case SchedulerStringId.Caption_WeekOfMonthSecond:
                    return "دومین";

                case SchedulerStringId.Caption_WeekOfMonthThird:
                    return "سومین";

                case SchedulerStringId.Caption_WeekOfMonthFourth:
                    return "چهارمین";

                case SchedulerStringId.Caption_WeekOfMonthLast:
                    return "آخرین";

                case SchedulerStringId.Msg_InvalidDayCount:
                    return "تعداد روز معتبر نمی باشد.";

                case SchedulerStringId.Msg_InvalidDayCountValue:
                    return "تعداد روز معتبر نمی باشد.";

                case SchedulerStringId.Msg_InvalidWeekCount:
                    return "تعداد هفته معتبر نمی باشد.";

                case SchedulerStringId.Msg_InvalidWeekCountValue:
                    return "تعداد هفته معتبر نمی باشد.";

                case SchedulerStringId.Msg_InvalidMonthCount:
                    return "عدد ماه معتبر نمی باشد.";

                case SchedulerStringId.Msg_InvalidMonthCountValue:
                    return "عدد ماه معتبر نمی باشد.";

                case SchedulerStringId.Msg_InvalidYearCount:
                    return "عدد سال معتبر نمی باشد.";

                case SchedulerStringId.Msg_InvalidYearCountValue:
                    return "عدد سال معتبر نمی باشد.";

                case SchedulerStringId.Msg_InvalidOccurrencesCount:
                    return "عدد وارد شده برای تعداد تكرار معتبر نمی باشد.";

                case SchedulerStringId.Msg_InvalidOccurrencesCountValue:
                    return "عدد وارد شده برای تعداد تكرار معتبر نمی باشد.";

                case SchedulerStringId.Msg_InvalidDayNumber:
                    return "عدد روز صحیح نمی باشد. لطفا عددی بین 1 تا {0} وارد كنید.";

                case SchedulerStringId.Msg_InvalidDayNumberValue:
                    return "عدد روز صحیح نمی باشد. لطفا عددی بین 1 تا {0} وارد كنید.";

                case SchedulerStringId.Msg_WarningDayNumber:
                    return "بعضی ماه ها كمتر از {0} روز دارند. رویداد برای آخرین روز ماه ثبت نخواهد شد.";

                case SchedulerStringId.Msg_InvalidDayOfWeek:
                    return "حداقا یك روز در هفته را انتخاب كنید.";

                case SchedulerStringId.Msg_WarningAppointmentDeleted:
                    return "این رویداد توسط كاربر دیگری حذف شده است.";

                case SchedulerStringId.MenuCmd_OpenAppointment:
                    return "باز كردن";

                case SchedulerStringId.DescCmd_OpenAppointment:
                    return "بازكردن رویداد انتخاب شده.";

                case SchedulerStringId.MenuCmd_PrintAppointment:
                    return "چاپ";

                case SchedulerStringId.MenuCmd_DeleteAppointment:
                    return "حذف";

                case SchedulerStringId.DescCmd_DeleteAppointment:
                    return "حذف رویداد(های) انتخاب شده.";

                case SchedulerStringId.MenuCmd_EditSeries:
                    return "ویرایش سری";

                case SchedulerStringId.MenuCmd_RestoreOccurrence:
                    return "بازگشت به وضعیت اولیه";

                case SchedulerStringId.MenuCmd_NewAppointment:
                    return "ثیت رویداد جدید";

                case SchedulerStringId.DescCmd_NewAppointment:
                    return "ثبت رویداد جدید.";

                case SchedulerStringId.MenuCmd_NewAllDayEvent:
                    return "ثبت رویداد روزانه";

                case SchedulerStringId.MenuCmd_NewRecurringAppointment:
                    return "ثبت رویداد تكراری";

                case SchedulerStringId.DescCmd_NewRecurringAppointment:
                    return "ثبت رویداد تكراری";

                case SchedulerStringId.MenuCmd_NewRecurringEvent:
                    return "ثبت واقه تكراری";

                case SchedulerStringId.MenuCmd_EditAppointmentDependency:
                    return "ویرایش";

                case SchedulerStringId.DescCmd_EditAppointmentDependency:
                    return "ویرایش وابستگی های";

                case SchedulerStringId.MenuCmd_DeleteAppointmentDependency:
                    return "حذف";

                case SchedulerStringId.DescCmd_DeleteAppointmentDependency:
                    return "حذف وابستگی ها";

                case SchedulerStringId.MenuCmd_GotoThisDay:
                    return "برو به این تاریخ";

                case SchedulerStringId.MenuCmd_GotoToday:
                    return "برو به امروز";

                case SchedulerStringId.DescCmd_GotoToday:
                    return "تغییر نحوه نمایش به تاریخ امروز.";

                case SchedulerStringId.MenuCmd_GotoDate:
                    return "برو به تاریخ ...";

                case SchedulerStringId.MenuCmd_OtherSettings:
                    return "دیگر تنظیمات ...";

                case SchedulerStringId.MenuCmd_CustomizeCurrentView:
                    return "تنظیمات نمایش ...";

                case SchedulerStringId.MenuCmd_CustomizeTimeRuler:
                    return "تنظیمات نوار ساعت";

                case SchedulerStringId.MenuCmd_5Minutes:
                    return "5 دقیقه";

                case SchedulerStringId.MenuCmd_6Minutes:
                    return "6 دقیقه";

                case SchedulerStringId.MenuCmd_10Minutes:
                    return "10 دقیقه";

                case SchedulerStringId.MenuCmd_15Minutes:
                    return "15 دقیقه";

                case SchedulerStringId.MenuCmd_20Minutes:
                    return "20 دقیقه";

                case SchedulerStringId.MenuCmd_30Minutes:
                    return "30 دقیقه";

                case SchedulerStringId.MenuCmd_60Minutes:
                    return "60 دقیقه";

                case SchedulerStringId.MenuCmd_SwitchViewMenu:
                    return "نحوه نمایش";

                case SchedulerStringId.MenuCmd_SwitchToDayView:
                    return "روزانه";

                case SchedulerStringId.MenuCmd_SwitchToWorkWeekView:
                    return "هفته كاری";

                case SchedulerStringId.MenuCmd_SwitchToWeekView:
                    return "هفتگی";

                case SchedulerStringId.MenuCmd_SwitchToMonthView:
                    return "ماهیانه";

                case SchedulerStringId.MenuCmd_SwitchToTimelineView:
                    return "خطكش زمانی";

                case SchedulerStringId.MenuCmd_SwitchToGroupByNone:
                    return "بدون دسته بندی";

                case SchedulerStringId.MenuCmd_SwitchToGroupByResource:
                    return "طبغه بندی بر اساس منابع";

                case SchedulerStringId.MenuCmd_SwitchToGroupByDate:
                    return "طبقه بندی بر اساس تاریخ";

                case SchedulerStringId.MenuCmd_SwitchToGanttView:
                    return "نمایش گانت";

                case SchedulerStringId.MenuCmd_TimeScalesMenu:
                    return "بزرگنمایی زمان";

                case SchedulerStringId.MenuCmd_TimeScaleCaptionsMenu:
                    return "عنوان نوار زمان";

                case SchedulerStringId.MenuCmd_TimeScaleHour:
                    return "ساعت";

                case SchedulerStringId.MenuCmd_TimeScaleDay:
                    return "روز";

                case SchedulerStringId.MenuCmd_TimeScaleWeek:
                    return "هفته";

                case SchedulerStringId.MenuCmd_TimeScaleMonth:
                    return "ماه";

                case SchedulerStringId.MenuCmd_TimeScaleQuarter:
                    return "فصل";

                case SchedulerStringId.MenuCmd_TimeScaleYear:
                    return "سال";

                case SchedulerStringId.MenuCmd_ShowTimeAs:
                    return "نمایش زمان بصورت";

                case SchedulerStringId.DescCmd_ShowTimeAs:
                    return "تغییر وضعیت رویداد انتخاب شده.";

                case SchedulerStringId.MenuCmd_Free:
                    return "آزاد";

                case SchedulerStringId.MenuCmd_Busy:
                    return "مشغول";

                case SchedulerStringId.MenuCmd_Tentative:
                    return "برسی";

                case SchedulerStringId.MenuCmd_OutOfOffice:
                    return "بیرون از دفتر";

                case SchedulerStringId.MenuCmd_LabelAs:
                    return "برچسب";

                case SchedulerStringId.DescCmd_LabelAs:
                    return "تغییر برچسب رویداد انتخاب شده.";

                case SchedulerStringId.MenuCmd_AppointmentLabelNone:
                    return "هیچ";

                case SchedulerStringId.MenuCmd_AppointmentLabelImportant:
                    return "مهم";

                case SchedulerStringId.MenuCmd_AppointmentLabelBusiness:
                    return "كاری";

                case SchedulerStringId.MenuCmd_AppointmentLabelPersonal:
                    return "شخصی";

                case SchedulerStringId.MenuCmd_AppointmentLabelVacation:
                    return "مرخصی";

                case SchedulerStringId.MenuCmd_AppointmentLabelMustAttend:
                    return "واجب";

                case SchedulerStringId.MenuCmd_AppointmentLabelTravelRequired:
                    return "سفر";

                case SchedulerStringId.MenuCmd_AppointmentLabelNeedsPreparation:
                    return "نیاز به مقدمات";

                case SchedulerStringId.MenuCmd_AppointmentLabelBirthday:
                    return "تولد";

                case SchedulerStringId.MenuCmd_AppointmentLabelAnniversary:
                    return "سالگرد";

                case SchedulerStringId.MenuCmd_AppointmentLabelPhoneCall:
                    return "تماس تلفنی";

                case SchedulerStringId.MenuCmd_AppointmentMove:
                    return "جابجایی";

                case SchedulerStringId.MenuCmd_AppointmentCopy:
                    return "كپی";

                case SchedulerStringId.MenuCmd_AppointmentCancel:
                    return "لغو";

                case SchedulerStringId.Caption_5Minutes:
                    return "5 دقیقه";

                case SchedulerStringId.Caption_6Minutes:
                    return "6 دقیقه";

                case SchedulerStringId.Caption_10Minutes:
                    return "10 دقیقه";

                case SchedulerStringId.Caption_15Minutes:
                    return "15 دقیقه";

                case SchedulerStringId.Caption_20Minutes:
                    return "20 دقیقه";

                case SchedulerStringId.Caption_30Minutes:
                    return "30 دقیقه";

                case SchedulerStringId.Caption_60Minutes:
                    return "60 دقیقه";

                case SchedulerStringId.Caption_Free:
                    return "آزاد";

                case SchedulerStringId.Caption_Busy:
                    return "مشغول";

                case SchedulerStringId.Caption_Tentative:
                    return "برسی";

                case SchedulerStringId.Caption_OutOfOffice:
                    return "خارج از دفتر";

                case SchedulerStringId.ViewDisplayName_Day:
                    return "تقویم روزانه";

                case SchedulerStringId.ViewDisplayName_WorkDays:
                    return "تقویم هفته كاری";

                case SchedulerStringId.ViewDisplayName_Week:
                    return "تقویم هفتگی";

                case SchedulerStringId.ViewDisplayName_Month:
                    return "تقویم ماهیانه";

                case SchedulerStringId.ViewDisplayName_Timeline:
                    return "تقویم سیر زمانی";

                case SchedulerStringId.ViewDisplayName_Gantt:
                    return "نمودار گانت";

                case SchedulerStringId.ViewShortDisplayName_Day:
                    return "روز";

                case SchedulerStringId.ViewShortDisplayName_WorkDays:
                    return "هفته كاری";

                case SchedulerStringId.ViewShortDisplayName_Week:
                    return "هفته";

                case SchedulerStringId.ViewShortDisplayName_Month:
                    return "ماه";

                case SchedulerStringId.ViewShortDisplayName_Timeline:
                    return "خطكش زمان";

                case SchedulerStringId.ViewShortDisplayName_Gantt:
                    return "گانت";

                case SchedulerStringId.TimeScaleDisplayName_Hour:
                    return "ساعت";

                case SchedulerStringId.TimeScaleDisplayName_Day:
                    return "روز";

                case SchedulerStringId.TimeScaleDisplayName_Week:
                    return "هفته";

                case SchedulerStringId.TimeScaleDisplayName_Month:
                    return "ماه";

                case SchedulerStringId.TimeScaleDisplayName_Quarter:
                    return "فصل";

                case SchedulerStringId.TimeScaleDisplayName_Year:
                    return "سال";

                case SchedulerStringId.Abbr_MinutesShort1:
                    return "دقیقه";

                case SchedulerStringId.Abbr_MinutesShort2:
                    return "دقیقه";

                case SchedulerStringId.Abbr_Minute:
                    return "دقیقه";

                case SchedulerStringId.Abbr_Minutes:
                    return "دقایق";

                case SchedulerStringId.Abbr_HoursShort:
                    return "ساعت";

                case SchedulerStringId.Abbr_Hour:
                    return "ساعت";

                case SchedulerStringId.Abbr_Hours:
                    return "ساعات";

                case SchedulerStringId.Abbr_DaysShort:
                    return "روز";

                case SchedulerStringId.Abbr_Day:
                    return "روز";

                case SchedulerStringId.Abbr_Days:
                    return "روزها";

                case SchedulerStringId.Abbr_WeeksShort:
                    return "هفته";

                case SchedulerStringId.Abbr_Week:
                    return "هفته";

                case SchedulerStringId.Abbr_Weeks:
                    return "هفته ها";

                case SchedulerStringId.Abbr_Month:
                    return "ماه";

                case SchedulerStringId.Abbr_Months:
                    return "ماه ها";

                case SchedulerStringId.Abbr_Year:
                    return "سال";

                case SchedulerStringId.Abbr_Years:
                    return "سالیان";

                case SchedulerStringId.Caption_Reminder:
                    return "یادآوری {0}";

                case SchedulerStringId.Caption_Reminders:
                    return "یادآوری {0}";

                case SchedulerStringId.Caption_StartTime:
                    return "ساعت شروع : {0}";

                case SchedulerStringId.Caption_NAppointmentsAreSelected:
                    return "تعداد {0} رویداد انتخاب شده";

                case SchedulerStringId.Format_TimeBeforeStart:
                    return "{0} قبل از شروع";

                case SchedulerStringId.Msg_Conflict:
                    return "رویداد ویرایش شده با دیگر رویداد ها تداخل دارد.";

                case SchedulerStringId.Msg_InvalidAppointmentDuration:
                    return "مقدار وارد شده برای بازه زمانی معتبر نمی باشد. لطفا از مقدار مثبت استفاده كنید.";

                case SchedulerStringId.Msg_InvalidReminderTimeBeforeStart:
                    return "مقدار وارد شده برای زمان یادآوری معتبر نمی باشد.";

                case SchedulerStringId.TextDuration_FromTo:
                    return "از {0} تا {1}";

                case SchedulerStringId.TextDuration_FromForDays:
                    return "از {0} برای {1}";

                case SchedulerStringId.TextDuration_FromForDaysHours:
                    return "از {0} برای {1} {2}";

                case SchedulerStringId.TextDuration_FromForDaysMinutes:
                    return "از {0} برای {1} {3}";

                case SchedulerStringId.TextDuration_FromForDaysHoursMinutes:
                    return "از {0} برای {1} {2} {3}";

                case SchedulerStringId.TextDuration_ForPattern:
                    return "{0} {1}";

                case SchedulerStringId.TextDailyPatternString_EveryDay:
                    return "هر {1} {0}";

                case SchedulerStringId.TextDailyPatternString_EveryDays:
                    return "هر {2} {1} {0}";

                case SchedulerStringId.TextDailyPatternString_EveryWeekDay:
                    return "هر روز {0}";

                case SchedulerStringId.TextDailyPatternString_EveryWeekend:
                    return "هر آخر هفته {0}";

                case SchedulerStringId.TextWeekly_0Day:
                    return "نامشخص بودن روز هفته";

                case SchedulerStringId.TextWeekly_1Day:
                    return "{0}";

                case SchedulerStringId.TextWeekly_2Day:
                    return "{0} و {1}";

                case SchedulerStringId.TextWeekly_3Day:
                    return "{0}، {1} و {2}";

                case SchedulerStringId.TextWeekly_4Day:
                    return "{0}، {1}، {2} و {3}";

                case SchedulerStringId.TextWeekly_5Day:
                    return "{0}، {1}، {2}، {3} و {4}";

                case SchedulerStringId.TextWeekly_6Day:
                    return "{0}، {1}، {2}، {3}، {4} و {5}";

                case SchedulerStringId.TextWeekly_7Day:
                    return "{0}، {1}، {2}، {3}، {4}، {5} و {6}";

                case SchedulerStringId.TextWeeklyPatternString_EveryWeek:
                    return "هر {3} {0}";

                case SchedulerStringId.TextWeeklyPatternString_EveryWeeks:
                    return "هر {1} {2} در {3} {0}";

                case SchedulerStringId.TextMonthlyPatternString_SubPattern:
                    return "از هر {0} {1} {2}";

                case SchedulerStringId.TextMonthlyPatternString1:
                    return "روز {3} {0}";

                case SchedulerStringId.TextMonthlyPatternString2:
                    return "{1} {2} {0}";

                case SchedulerStringId.TextYearlyPattern_YearString1:
                    return "هر {3} {4} {0}";

                case SchedulerStringId.TextYearlyPattern_YearString2:
                    return "{5} {6} از {3} {0}";

                case SchedulerStringId.TextYearlyPattern_YearsString1:
                    return "{3} {4} از هر {1} {2} {0}";

                case SchedulerStringId.TextYearlyPattern_YearsString2:
                    return "the {5} {6} of {3} every {1} {2} {0}";

                case SchedulerStringId.Caption_AllDay:
                    return "تمام روز";

                case SchedulerStringId.Caption_PleaseSeeAbove:
                    return "لطفا به بالا توجه كنید";

                case SchedulerStringId.Caption_RecurrenceSubject:
                    return "موضوع :";

                case SchedulerStringId.Caption_RecurrenceLocation:
                    return "موقعیت :";

                case SchedulerStringId.Caption_RecurrenceStartTime:
                    return "شروع :";

                case SchedulerStringId.Caption_RecurrenceEndTime:
                    return "پایان :";

                case SchedulerStringId.Caption_RecurrenceShowTimeAs:
                    return "نمایش زمان :";

                case SchedulerStringId.Caption_Recurrence:
                    return "تكرار :";

                case SchedulerStringId.Caption_RecurrencePattern:
                    return "نحوه تكرار :";

                case SchedulerStringId.Caption_NoneRecurrence:
                    return "(هیچ)";

                case SchedulerStringId.MemoPrintDateFormat:
                    return "{0} {1} {2}";

                case SchedulerStringId.Caption_EmptyResource:
                    return "(هر چیزی)";

                case SchedulerStringId.Caption_DailyPrintStyle:
                    return "نقش روزانه";

                case SchedulerStringId.Caption_WeeklyPrintStyle:
                    return "نقش هفتگی";

                case SchedulerStringId.Caption_MonthlyPrintStyle:
                    return "نقش ماهیانه";

                case SchedulerStringId.Caption_TrifoldPrintStyle:
                    return "Tri-fold Style";

                case SchedulerStringId.Caption_CalendarDetailsPrintStyle:
                    return "تقویمی با جزئیات";

                case SchedulerStringId.Caption_MemoPrintStyle:
                    return "متن بلند";

                case SchedulerStringId.Caption_ColorConverterFullColor:
                    return "تمام رنگی";

                case SchedulerStringId.Caption_ColorConverterGrayScale:
                    return "سایه روشن";

                case SchedulerStringId.Caption_ColorConverterBlackAndWhite:
                    return "سیاه و سفید";

                case SchedulerStringId.Caption_ResourceNone:
                    return "(هیچ)";

                case SchedulerStringId.Caption_ResourceAll:
                    return "(همه)";

                case SchedulerStringId.PrintPageSetupFormatTabControlCustomizeShading:

                case SchedulerStringId.PrintPageSetupFormatTabControlSizeAndFontName:
                    return "اندازه {0} {1}";

                case SchedulerStringId.PrintRangeControlInvalidDate:
                    return "تاریخ پایان می بایست بیشتر و یا مساوی با تاریخ شروع باشد.";

                case SchedulerStringId.PrintCalendarDetailsControlDayPeriod:
                    return "روز";

                case SchedulerStringId.PrintCalendarDetailsControlWeekPeriod:
                    return "هفته";

                case SchedulerStringId.PrintCalendarDetailsControlMonthPeriod:
                    return "ماه";

                case SchedulerStringId.PrintMonthlyOptControlOnePagePerMonth:
                    return "1 صفحه/ماه";

                case SchedulerStringId.PrintMonthlyOptControlTwoPagesPerMonth:
                    return "2 صفحه/ماه";

                case SchedulerStringId.PrintTimeIntervalControlInvalidDuration:
                    return "بازه زمانی می بایست بیشتر از صفر و كمتر از یك روز باشد.";

                case SchedulerStringId.PrintTimeIntervalControlInvalidStartEndTime:
                    return "زمان پایان باید بیشتر از زمان شروع باشد.";

                case SchedulerStringId.PrintTriFoldOptControlDailyCalendar:
                    return "تقویم روزانه";

                case SchedulerStringId.PrintTriFoldOptControlWeeklyCalendar:
                    return "تقویم هفتگی";

                case SchedulerStringId.PrintTriFoldOptControlMonthlyCalendar:
                    return "تقویم ماهیانه";

                case SchedulerStringId.PrintWeeklyOptControlOneWeekPerPage:
                    return "1 صفحه/هفته";

                case SchedulerStringId.PrintWeeklyOptControlTwoWeekPerPage:
                    return "2 صفحه/هفته";

                case SchedulerStringId.PrintPageSetupFormCaption:
                    return "تنظیمات چاپ : {0}";

                case SchedulerStringId.PrintMoreItemsMsg:
                    return "موارد بیشتر ...";

                case SchedulerStringId.PrintNoPrintersInstalled:
                    return "چاپگری پیدا نشد";

                case SchedulerStringId.Caption_FirstVisibleResources:
                    return "اولین";

                case SchedulerStringId.Caption_PrevVisibleResourcesPage:
                    return "صفحه قبل";

                case SchedulerStringId.Caption_PrevVisibleResources:
                    return "قبلی";

                case SchedulerStringId.Caption_NextVisibleResources:
                    return "بعدی";

                case SchedulerStringId.Caption_NextVisibleResourcesPage:
                    return "صفحه بعد";

                case SchedulerStringId.Caption_LastVisibleResources:
                    return "آخرین";

                case SchedulerStringId.Caption_IncreaseVisibleResourcesCount:
                    return "افزایش منابع نمایش داده شده";

                case SchedulerStringId.Caption_DecreaseVisibleResourcesCount:
                    return "كاهش منابع نمایش داده شده";

                case SchedulerStringId.Caption_ShadingApplyToAllDayArea:
                    return "قسمت تمام روز";

                case SchedulerStringId.Caption_ShadingApplyToAppointments:
                    return "قسمت رویدادها";

                case SchedulerStringId.Caption_ShadingApplyToAppointmentStatuses:
                    return "قسمت وضعت رویداد";

                case SchedulerStringId.Caption_ShadingApplyToHeaders:
                    return "قسمت عناوین";

                case SchedulerStringId.Caption_ShadingApplyToTimeRulers:
                    return "قسمت خطكش زمان";

                case SchedulerStringId.Caption_ShadingApplyToCells:
                    return "قسمت اصلی";

                case SchedulerStringId.Msg_InvalidSize:
                    return "مقدار وارد شده برای اندازه معتبر نمی باشد.";

                case SchedulerStringId.Msg_ApplyToAllStyles:
                    return "تنظیمات چاپگر برای همه اعمال شود.";

                case SchedulerStringId.Msg_MemoPrintNoSelectedItems:
                    return "جهت چاپ، یك مورد را انتخاب كنید.";

                case SchedulerStringId.Caption_AllResources:
                    return "تمام منابع";

                case SchedulerStringId.Caption_VisibleResources:
                    return "منابه قابل مشاهده";

                case SchedulerStringId.Caption_OnScreenResources:
                    return "منابه روی صفحه";

                case SchedulerStringId.Caption_GroupByNone:
                    return "بدون دسته بندی";

                case SchedulerStringId.Caption_GroupByDate:
                    return "بر اساس تاریخ";

                case SchedulerStringId.Caption_GroupByResources:
                    return "براساس منابع";

                case SchedulerStringId.Msg_InvalidInputFile:
                    return "فایل ورودی معتبر نمی باشد";

                case SchedulerStringId.TextRecurrenceTypeDaily:
                    return "روزانه";

                case SchedulerStringId.TextRecurrenceTypeWeekly:
                    return "هفتگی";

                case SchedulerStringId.TextRecurrenceTypeMonthly:
                    return "ماهیانه";

                case SchedulerStringId.TextRecurrenceTypeYearly:
                    return "سالیانه";

                case SchedulerStringId.TextRecurrenceTypeMinutely:
                    return "دقیقه";

                case SchedulerStringId.TextRecurrenceTypeHourly:
                    return "ساعت";

                case SchedulerStringId.Msg_Warning:
                    return "اخطار!";

                case SchedulerStringId.Msg_CantFitIntoPage:
                    return "It's impossible to fit the printing output into a single page using the current printing settings. Please try to increase the page height or decrease the PrintTime interval.";

                case SchedulerStringId.Msg_PrintStyleNameExists:
                    return "The style name '{0}' already exists. Type another name.";

                case SchedulerStringId.Msg_OutlookCalendarNotFound:
                    return "The '{0}' calendar is not found.";

                case SchedulerStringId.Caption_PrevAppointment:
                    return "رویداد قبلی";

                case SchedulerStringId.Caption_NextAppointment:
                    return "رویداد بعدی";

                case SchedulerStringId.DisplayName_Appointment:
                    return "رویداد";

                case SchedulerStringId.Format_CopyOf:
                    return "Copy of {0}";

                case SchedulerStringId.Format_CopyNOf:
                    return "Copy ({0}) of {1}";

                case SchedulerStringId.Msg_MissingRequiredMapping:
                    return "The required mapping for the '{0}' property is missing.";

                case SchedulerStringId.Msg_MissingMappingMember:
                    return "Missing '{1}' member of the '{0}' property mapping.";

                case SchedulerStringId.Msg_DuplicateMappingMember:
                    return "The '{0}' member mapping is not unique: ";

                case SchedulerStringId.Msg_InconsistentRecurrenceInfoMapping:
                    return "To support recurrence you must map both RecurrenceInfo and Type members.";

                case SchedulerStringId.Msg_IncorrectMappingsQuestion:
                    return "Incorrect mappings. Continue anyway? Details:";

                case SchedulerStringId.Msg_DuplicateCustomFieldMappings:
                    return "Duplicate custom field name. Revise the mappings: {0}";

                case SchedulerStringId.Msg_MappingsCheckPassedOk:
                    return "Mappings are correct!";

                case SchedulerStringId.Caption_SetupAppointmentMappings:
                    return "Setup Appointment Mappings";

                case SchedulerStringId.Caption_SetupResourceMappings:
                    return "Setup Resource Mappings";

                case SchedulerStringId.Caption_SetupDependencyMappings:
                    return "Setup Dependency Mappings";

                case SchedulerStringId.Caption_ModifyAppointmentMappingsTransactionDescription:
                    return "Modify Appointment Mappings";

                case SchedulerStringId.Caption_ModifyResourceMappingsTransactionDescription:
                    return "Modify Resource Mappings";

                case SchedulerStringId.Caption_ModifyAppointmentDependencyMappingsTransactionDescription:
                    return "Modify Appointment Dependency Mappings";

                case SchedulerStringId.Caption_MappingsValidation:
                    return "Mappings Validation";

                case SchedulerStringId.Caption_MappingsWizard:
                    return "Mappings Wizard...";

                case SchedulerStringId.Caption_CheckMappings:
                    return "Check Mappings";

                case SchedulerStringId.Caption_SetupAppointmentStorage:
                    return "Setup Appointment Storage";

                case SchedulerStringId.Caption_SetupResourceStorage:
                    return "Setup Resource Storage";

                case SchedulerStringId.Caption_SetupAppointmentDependencyStorage:
                    return "Setup Dependency Storage";

                case SchedulerStringId.Caption_ModifyAppointmentStorageTransactionDescription:
                    return "Modify Appointment Storage";

                case SchedulerStringId.Caption_ModifyResourceStorageTransactionDescription:
                    return "Modify Resource Storage";

                case SchedulerStringId.Caption_ModifyAppointmentDependencyStorageTransactionDescription:
                    return "Modify AppointmentDependency Storage";

                case SchedulerStringId.Caption_DayViewDescription:
                    return "Switch to the Day view. The most detailed view of appointments for a specific day(s).";

                case SchedulerStringId.Caption_WorkWeekViewDescription:
                    return "Switch to the Work Week view. Detailed view for the working days in a certain week.";

                case SchedulerStringId.Caption_WeekViewDescription:
                    return "Switch to the Week view. Arranges appointments for a particular week in a compact form.";

                case SchedulerStringId.Caption_MonthViewDescription:
                    return "Switch to the Month (Multi-Week) view. Calendar view useful for long-term plans.";

                case SchedulerStringId.Caption_TimelineViewDescription:
                    return "Switch to the Timeline view. Plots appointments in relation to time.";

                case SchedulerStringId.Caption_GanttViewDescription:
                    return "Switch to the Gantt View. Project management view that shows appointments and their dependencies in relation to time.";

                case SchedulerStringId.Caption_GroupByNoneDescription:
                    return "Ungroup appointments.";

                case SchedulerStringId.Caption_GroupByDateDescription:
                    return "Group appointments by date.";

                case SchedulerStringId.Caption_GroupByResourceDescription:
                    return "Group appointments by resource.";

                case SchedulerStringId.Msg_iCalendar_NotValidFile:
                    return "Invalid Internet Calendar file";

                case SchedulerStringId.Msg_iCalendar_AppointmentsImportWarning:
                    return "Cannot import some appointment";

                case SchedulerStringId.MenuCmd_NavigateBackward:
                    return "Backward";

                case SchedulerStringId.MenuCmd_NavigateForward:
                    return "Forward";

                case SchedulerStringId.DescCmd_NavigateBackward:
                    return "Step back in time as suggested by the current view.";

                case SchedulerStringId.DescCmd_NavigateForward:
                    return "Advance forward in time as suggested by the current view.";

                case SchedulerStringId.MenuCmd_ViewZoomIn:
                    return "بزرگ نمایی";

                case SchedulerStringId.MenuCmd_ViewZoomOut:
                    return "كوچك نمایی";

                case SchedulerStringId.DescCmd_ViewZoomIn:
                    return "Perform scaling up to display content in more detail.";

                case SchedulerStringId.DescCmd_ViewZoomOut:
                    return "Perform scaling down to display a broader look of the View.";

                case SchedulerStringId.DescCmd_SplitAppointment:
                    return "Split the selected appointment in two by dragging a splitter line.";

                case SchedulerStringId.Caption_SplitAppointment:
                    return "Split";

                case SchedulerStringId.VS_SchedulerReportsToolboxCategoryName:
                    return "DX.{0}: Scheduler Reporting";

                case SchedulerStringId.UD_SchedulerReportsToolboxCategoryName:
                    return "Scheduler Controls";

                case SchedulerStringId.Reporting_NotAssigned_TimeCells:
                    return "Required TimeCells control is not assigned";

                case SchedulerStringId.Reporting_NotAssigned_View:
                    return "Required View component is not assigned";

                case SchedulerStringId.Msg_RecurrenceExceptionsWillBeLost:
                    return "Any exceptions associated with this recurring appointment will be lost. Proceed?";

                case SchedulerStringId.DescCmd_CreateAppointmentDependency:
                    return "Create dependency between appointments";

                case SchedulerStringId.MenuCmd_CreateAppointmentDependency:
                    return "Create Dependency";

                case SchedulerStringId.Caption_AppointmentDependencyTypeFinishToStart:
                    return "Finish-to-start (FS)";

                case SchedulerStringId.Caption_AppointmentDependencyTypeStartToStart:
                    return "Start-to-start (SS)";

                case SchedulerStringId.Caption_AppointmentDependencyTypeFinishToFinish:
                    return "Finish-to-finish (FF)";

                case SchedulerStringId.Caption_AppointmentDependencyTypeStartToFinish:
                    return "Start-to-finish (SF)";

                case SchedulerStringId.TextAppointmentSnapToCells_Always:
                    return "Always";

                case SchedulerStringId.TextAppointmentSnapToCells_Auto:
                    return "Auto";

                case SchedulerStringId.TextAppointmentSnapToCells_Disabled:
                    return "Disabled";

                case SchedulerStringId.TextAppointmentSnapToCells_Never:
                    return "Never";

                case SchedulerStringId.MenuCmd_PrintPreview:
                    return "Print &Preview";

                case SchedulerStringId.DescCmd_PrintPreview:
                    return "Preview and make changes to pages before printing.";

                case SchedulerStringId.MenuCmd_Print:
                    return "Quick Print";

                case SchedulerStringId.DescCmd_Print:
                    return "Send the schedule directly to the default printer without making changes.";

                case SchedulerStringId.MenuCmd_PrintPageSetup:
                    return "Page &Setup";

                case SchedulerStringId.DescCmd_PrintPageSutup:
                    return "Customize the page appearance and configure various printing options.";

                case SchedulerStringId.DescCmd_TimeScalesMenu:
                    return "Change the time scale.";

                case SchedulerStringId.MenuCmd_ShowWorkTimeOnly:
                    return "Working Hours";

                case SchedulerStringId.DescCmd_ShowWorkTimeOnly:
                    return "Show only working hours in the calendar.";

                case SchedulerStringId.MenuCmd_CompressWeekend:
                    return "Compress Weekend";

                case SchedulerStringId.DescCmd_CompressWeekend:
                    return "Show Saturday and Sunday compressed into a single column.";

                case SchedulerStringId.MenuCmd_CellsAutoHeight:
                    return "Cell Auto Height";

                case SchedulerStringId.DescCmd_CellsAutoHeight:
                    return "Enable a time cell to automatically adjust its size to accommodate appointments it contains.";

                case SchedulerStringId.MenuCmd_ToggleRecurrence:
                    return "Recurrence";

                case SchedulerStringId.DescCmd_ToggleRecurrence:
                    return "Make the selected appointment recurring, or edit the recurrence pattern. ";

                case SchedulerStringId.MenuCmd_ChangeAppointmentReminderUI:
                    return "Reminder";

                case SchedulerStringId.DescCmd_ChangeAppointmentReminderUI:
                    return "Choose when to be reminded of the selected appointment.";

                case SchedulerStringId.Caption_NoneReminder:
                    return "None";

                case SchedulerStringId.DescCmd_ChangeTimelineScaleWidth:
                    return "Specify column width in pixels for the base scale.";

                case SchedulerStringId.MenuCmd_ChangeTimelineScaleWidth:
                    return "Scale Width";

                case SchedulerStringId.MenuCmd_OpenSchedule:
                    return "Open";

                case SchedulerStringId.DescCmd_OpenSchedule:
                    return "Import a schedule from a file (.ics).";

                case SchedulerStringId.MenuCmd_SaveSchedule:
                    return "Save";

                case SchedulerStringId.DescCmd_SaveSchedule:
                    return "Save a schedule to a file (.ics).";

                case SchedulerStringId.MenuCmd_ChangeSnapToCellsUI:
                    return "Snap to Cells";

                case SchedulerStringId.DescCmd_ChangeSnapToCellsUI:
                    return "Specify a snapping mode for displaying appointments within time cells.";

                case SchedulerStringId.MenuCmd_OpenOccurrence:
                    return "Open Occurrence";

                case SchedulerStringId.DescCmd_OpenOccurrence:
                    return "Open this meeting occurrence.";

                case SchedulerStringId.MenuCmd_OpenSeries:
                    return "Open Series";

                case SchedulerStringId.DescCmd_OpenSeries:
                    return "Open this meeting series.";

                case SchedulerStringId.MenuCmd_DeleteOccurrence:
                    return "حذف رویداد";

                case SchedulerStringId.DescCmd_DeleteOccurrence:
                    return "حذف رویداد.";

                case SchedulerStringId.MenuCmd_DeleteSeries:
                    return "حذف سری";

                case SchedulerStringId.DescCmd_DeleteSeries:
                    return "حذف سری.";
                default:
                    throw new ArgumentOutOfRangeException("id");
            }
        }
    }
}
