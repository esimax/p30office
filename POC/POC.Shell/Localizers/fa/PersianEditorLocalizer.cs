using System;
using DevExpress.Xpf.Editors;

namespace POC.Shell.Localizers.fa
{
    internal class PersianEditorLocalizer : EditorLocalizer
    {
        public override string Language
        {
            get
            {
                return "fa-IR";
            }
        }
        protected override void PopulateStringTable()
        {
            base.PopulateStringTable();
            AddString(EditorStringId.WaitIndicatorText, "لطفا صبر كنید ...");
        }

        public override string GetLocalizedString(EditorStringId id)
        {
            switch (id)
            {

                case EditorStringId.OK:
                    return "تایید";
                case EditorStringId.Cancel:
                    return "لغو";
                case EditorStringId.Clear:
                    return "پاك سازی";
                case EditorStringId.Apply:
                    return "اعمال";
                case EditorStringId.Yes:
                    return "بله";
                case EditorStringId.No:
                    return "خیر";
                case EditorStringId.Copy:
                    return "كپی";
                case EditorStringId.Paste:
                    return "جای گذاری";
                case EditorStringId.Cut:
                    return "برش";
                case EditorStringId.Open:
                    return "باز كردن";
                case EditorStringId.Save:
                    return "ذخیره";
                case EditorStringId.CaptionError:
                    return "خطا";
                case EditorStringId.SetNullValue:
                    return "تخلیه";
                case EditorStringId.OutOfRange:
                    return "مقدار خارج از بازه مجاز می باشد";
                case EditorStringId.MaskIncomplete:
                    return "ماسك معتبر نمی باشد";
                case EditorStringId.SelectAll:
                    return "(انتخاب همه)";
                case EditorStringId.EmptyItem:
                    return "(خالی)";
                case EditorStringId.CalculatorButtonMC:
                    return "MC";
                case EditorStringId.CalculatorButtonMR:
                    return "MR";
                case EditorStringId.CalculatorButtonMS:
                    return "MS";
                case EditorStringId.CalculatorButtonMAdd:
                    return "M+";
                case EditorStringId.CalculatorButtonMSub:
                    return "M-";
                case EditorStringId.CalculatorButtonBack:
                    return "←";
                case EditorStringId.CalculatorButtonCancel:
                    return "CE";
                case EditorStringId.CalculatorButtonClear:
                    return "C";
                case EditorStringId.CalculatorButtonZero:
                    return "0";
                case EditorStringId.CalculatorButtonOne:
                    return "1";
                case EditorStringId.CalculatorButtonTwo:
                    return "2";
                case EditorStringId.CalculatorButtonThree:
                    return "3";
                case EditorStringId.CalculatorButtonFour:
                    return "4";
                case EditorStringId.CalculatorButtonFive:
                    return "5";
                case EditorStringId.CalculatorButtonSix:
                    return "6";
                case EditorStringId.CalculatorButtonSeven:
                    return "7";
                case EditorStringId.CalculatorButtonEight:
                    return "8";
                case EditorStringId.CalculatorButtonNine:
                    return "9";
                case EditorStringId.CalculatorButtonSign:
                    return "±";
                case EditorStringId.CalculatorButtonAdd:
                    return "+";
                case EditorStringId.CalculatorButtonSub:
                    return "-";
                case EditorStringId.CalculatorButtonMul:
                    return "*";
                case EditorStringId.CalculatorButtonDiv:
                    return "/";
                case EditorStringId.CalculatorButtonFract:
                    return "1/x";
                case EditorStringId.CalculatorButtonPercent:
                    return "%";
                case EditorStringId.CalculatorButtonSqrt:
                    return "√";
                case EditorStringId.CalculatorButtonEqual:
                    return "=";
                case EditorStringId.CalculatorDivisionByZeroError:
                    return "خطا : تقسیم بر صفر";
                case EditorStringId.CalculatorError:
                    return "خطا";
                case EditorStringId.CalculatorInvalidInputError:
                    return "مقدار نامعتبر";
                case EditorStringId.ColorEdit_AutomaticButtonCaption:
                    return "اتوماتیك";
                case EditorStringId.ColorEdit_MoreColorsButtonCaption:
                    return "رنگ بیشتر ...";
                case EditorStringId.ColorEdit_NoColorButtonCaption:
                    return "بدون رنگ";
                case EditorStringId.ColorEdit_RecentColorsCaption:
                    return "آخرین رنگ ها";
                case EditorStringId.ColorEdit_ColorChooserWindowTitle:
                    return "رنگ ها";
                case EditorStringId.ColorEdit_ThemeColorsCaption:
                    return "رنگ تم";
                case EditorStringId.ColorEdit_StandardColorsCaption:
                    return "رنگ های استاندارد";
                case EditorStringId.ColorEdit_DefaultColors_Black:
                    return "Black";
                case EditorStringId.ColorEdit_DefaultColors_White:
                    return "White";
                case EditorStringId.ColorEdit_DefaultColors_DarkRed:
                    return "DarkRed";
                case EditorStringId.ColorEdit_DefaultColors_Red:
                    return "Red";
                case EditorStringId.ColorEdit_DefaultColors_Orange:
                    return "Orange";
                case EditorStringId.ColorEdit_DefaultColors_Yellow:
                    return "Yellow";
                case EditorStringId.ColorEdit_DefaultColors_LightGreen:
                    return "LightGreen";
                case EditorStringId.ColorEdit_DefaultColors_Green:
                    return "Green";
                case EditorStringId.ColorEdit_DefaultColors_LightBlue:
                    return "LightBlue";
                case EditorStringId.ColorEdit_DefaultColors_Blue:
                    return "Blue";
                case EditorStringId.ColorEdit_DefaultColors_DarkBlue:
                    return "DarkBlue";
                case EditorStringId.ColorEdit_DefaultColors_Purple:
                    return "Purple";
                case EditorStringId.ImageEdit_OpenFileFilter:
                    return "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
                case EditorStringId.ImageEdit_OpenFileFilter_SL:
                    return "Image Files(*.JPG;*.PNG)|*.JPG;*.PNG|All files (*.*)|*.*";
                case EditorStringId.ImageEdit_SaveFileFilter:
                    return "PNG(*.png)|*.png|BMP(*.bmp)|*.BMP|JPG(*.jpg)|*.jpg|GIF(*.gif)|*.gif";
                case EditorStringId.ImageEdit_InvalidFormatMessage:
                    return "فرمت اشتباه تصویر";
                case EditorStringId.FilterCriteriaToStringFunctionIsNullOrEmpty:
                    return "خالی باشد";
                case EditorStringId.FilterCriteriaToStringFunctionStartsWith:
                    return "شروع شود";
                case EditorStringId.FilterCriteriaToStringFunctionEndsWith:
                    return "ختم شود";
                case EditorStringId.FilterCriteriaToStringFunctionContains:
                    return "شامل شود";
                case EditorStringId.FilterCriteriaToStringBetween:
                    return "بین";
                case EditorStringId.FilterCriteriaToStringIn:
                    return "در";
                case EditorStringId.FilterCriteriaToStringIsNotNull:
                    return "خالی نباشد";
                case EditorStringId.FilterCriteriaToStringNotLike:
                    return "مشابه نباشد";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorBitwiseAnd:
                    return "&";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorBitwiseOr:
                    return "|";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorBitwiseXor:
                    return "^";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorDivide:
                    return "/";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorEqual:
                    return "=";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorGreater:
                    return ">";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorGreaterOrEqual:
                    return ">=";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorLess:
                    return "<";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorLessOrEqual:
                    return "<=";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorLike:
                    return "مشابه";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorMinus:
                    return "-";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorModulo:
                    return "%";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorMultiply:
                    return "*";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorNotEqual:
                    return "<>";
                case EditorStringId.FilterCriteriaToStringBinaryOperatorPlus:
                    return "+";
                case EditorStringId.FilterCriteriaToStringUnaryOperatorBitwiseNot:
                    return "~";
                case EditorStringId.FilterCriteriaToStringUnaryOperatorIsNull:
                    return "تهی باشد";
                case EditorStringId.FilterCriteriaToStringUnaryOperatorMinus:
                    return "-";
                case EditorStringId.FilterCriteriaToStringUnaryOperatorNot:
                    return "نقض";
                case EditorStringId.FilterCriteriaToStringUnaryOperatorPlus:
                    return "+";
                case EditorStringId.FilterCriteriaToStringGroupOperatorAnd:
                    return "و";
                case EditorStringId.FilterCriteriaToStringGroupOperatorOr:
                    return "یا";
                case EditorStringId.FilterCriteriaInvalidExpression:
                    return "خطا : عبارت وارد شده معتبر نمی باشد (خط {0}، كاراكتر {1}).";
                case EditorStringId.FilterCriteriaInvalidExpressionEx:
                    return "خطا : عبارت وارد شده معتبر نمی باشد.";
                case EditorStringId.ExpressionEditor_Functions_Text:
                    return "Functions";
                case EditorStringId.ExpressionEditor_Operators_Text:
                    return "Operators";
                case EditorStringId.ExpressionEditor_Fields_Text:
                    return "Fields";
                case EditorStringId.ExpressionEditor_Constants_Text:
                    return "Constants";
                case EditorStringId.ExpressionEditor_FunctionType_AllItems:
                    return "(All)";
                case EditorStringId.ExpressionEditor_FunctionType_DateTimeItems:
                    return "Date-time";
                case EditorStringId.ExpressionEditor_FunctionType_LogicalItems:
                    return "Logical";
                case EditorStringId.ExpressionEditor_FunctionType_MathItems:
                    return "Math";
                case EditorStringId.ExpressionEditor_FunctionType_StringItems:
                    return "String";
                case EditorStringId.ExpressionEditor_Abs_Description:
                    return "Abs(Value)" + Environment.NewLine + "مقدار مثبت یك عبارت ریاضی را بر می گرداند.";
                case EditorStringId.ExpressionEditor_Acos_Description:
                    return "Acos(Value)" + Environment.NewLine + "مقدار آرك كوسینوس یك عبارت ریاضی را بر می گرداند.";
                case EditorStringId.ExpressionEditor_AddDays_Description:
                    return "AddDays(DateTime, DaysCount)" + Environment.NewLine + "مقدار تاریخ به انضمام تعدار روز های افزوده شده را بر میگرداند.";
                case EditorStringId.ExpressionEditor_AddHours_Description:
                    break;
                case EditorStringId.ExpressionEditor_AddMilliSeconds_Description:
                    break;
                case EditorStringId.ExpressionEditor_AddMinutes_Description:
                    break;
                case EditorStringId.ExpressionEditor_AddMonths_Description:
                    break;
                case EditorStringId.ExpressionEditor_AddSeconds_Description:
                    break;
                case EditorStringId.ExpressionEditor_AddTicks_Description:
                    break;
                case EditorStringId.ExpressionEditor_AddTimeSpan_Description:
                    break;
                case EditorStringId.ExpressionEditor_AddYears_Description:
                    break;
                case EditorStringId.ExpressionEditor_Ascii_Description:
                    break;
                case EditorStringId.ExpressionEditor_Asin_Description:
                    break;
                case EditorStringId.ExpressionEditor_Atn_Description:
                    break;
                case EditorStringId.ExpressionEditor_Atn2_Description:
                    break;
                case EditorStringId.ExpressionEditor_BigMul_Description:
                    break;
                case EditorStringId.ExpressionEditor_Ceiling_Description:
                    break;
                case EditorStringId.ExpressionEditor_Char_Description:
                    break;
                case EditorStringId.ExpressionEditor_CharIndex_Description:
                    break;
                case EditorStringId.ExpressionEditor_CharIndex3Param_Description:
                    break;
                case EditorStringId.ExpressionEditor_Concat_Description:
                    break;
                case EditorStringId.ExpressionEditor_Cos_Description:
                    break;
                case EditorStringId.ExpressionEditor_Cosh_Description:
                    break;
                case EditorStringId.ExpressionEditor_DateDiffDay_Description:
                    break;
                case EditorStringId.ExpressionEditor_DateDiffHour_Description:
                    break;
                case EditorStringId.ExpressionEditor_DateDiffMilliSecond_Description:
                    break;
                case EditorStringId.ExpressionEditor_DateDiffMinute_Description:
                    break;
                case EditorStringId.ExpressionEditor_DateDiffMonth_Description:
                    break;
                case EditorStringId.ExpressionEditor_DateDiffSecond_Description:
                    break;
                case EditorStringId.ExpressionEditor_DateDiffTick_Description:
                    break;
                case EditorStringId.ExpressionEditor_DateDiffYear_Description:
                    break;
                case EditorStringId.ExpressionEditor_Exp_Description:
                    break;
                case EditorStringId.ExpressionEditor_Floor_Description:
                    break;
                case EditorStringId.ExpressionEditor_GetDate_Description:
                    break;
                case EditorStringId.ExpressionEditor_GetDay_Description:
                    break;
                case EditorStringId.ExpressionEditor_GetDayOfWeek_Description:
                    break; 
                case EditorStringId.ExpressionEditor_GetDayOfYear_Description:
                    break; 
                case EditorStringId.ExpressionEditor_GetHour_Description:
                    break; 
                case EditorStringId.ExpressionEditor_GetMilliSecond_Description:
                    break; 
                case EditorStringId.ExpressionEditor_GetMinute_Description:
                    break; 
                case EditorStringId.ExpressionEditor_GetMonth_Description:
                    break; 
                case EditorStringId.ExpressionEditor_GetSecond_Description:
                    break; 
                case EditorStringId.ExpressionEditor_GetTimeOfDay_Description:
                    break; 
                case EditorStringId.ExpressionEditor_GetYear_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Iif_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Insert_Description:
                    break; 
                case EditorStringId.ExpressionEditor_IsNull_Description:
                    break; 
                case EditorStringId.ExpressionEditor_IsNullOrEmpty_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Len_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Log_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Log2Param_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Log10_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Lower_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Now_Description:
                    break; 
                case EditorStringId.ExpressionEditor_PadLeft_Description:
                    break; 
                case EditorStringId.ExpressionEditor_PadLeft3Param_Description:
                    break; 
                case EditorStringId.ExpressionEditor_PadRight_Description:
                    break; 
                case EditorStringId.ExpressionEditor_PadRight3Param_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Power_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Remove_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Replace_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Reverse_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Rnd_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Round_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Round2Param_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Sign_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Sin_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Sinh_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Sqr_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Substring3param_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Substring2param_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Tan_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Tanh_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Today_Description:
                    break; 
                case EditorStringId.ExpressionEditor_ToInt_Description:
                    break; 
                case EditorStringId.ExpressionEditor_ToLong_Description:
                    break; 
                case EditorStringId.ExpressionEditor_ToFloat_Description:
                    break; 
                case EditorStringId.ExpressionEditor_ToDouble_Description:
                    break; 
                case EditorStringId.ExpressionEditor_ToDecimal_Description:
                    break; 
                case EditorStringId.ExpressionEditor_ToStr_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Trim_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Upper_Description:
                    break; 
                case EditorStringId.ExpressionEditor_UtcNow_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeDayAfterTomorrow_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeLastWeek_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeNextMonth_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeNextWeek_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeNextYear_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeNow_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeThisMonth_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeThisWeek_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeThisYear_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeToday_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeTomorrow_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeTwoWeeksAway_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LocalDateTimeYesterday_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Plus_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Minus_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Multiply_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Divide_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Modulo_Description:
                    break; 
                case EditorStringId.ExpressionEditor_BitwiseOr_Description:
                    break; 
                case EditorStringId.ExpressionEditor_BitwiseAnd_Description:
                    break; 
                case EditorStringId.ExpressionEditor_BitwiseXor_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Equal_Description:
                    break; 
                case EditorStringId.ExpressionEditor_NotEqual_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Less_Description:
                    break; 
                case EditorStringId.ExpressionEditor_LessOrEqual_Description:
                    break; 
                case EditorStringId.ExpressionEditor_GreaterOrEqual_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Greater_Description:
                    break; 
                case EditorStringId.ExpressionEditor_In_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Like_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Between_Description:
                    break; 
                case EditorStringId.ExpressionEditor_And_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Or_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Not_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Max_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Min_Description:
                    break; 
                case EditorStringId.ExpressionEditor_StartsWith_Description:
                    break; 
                case EditorStringId.ExpressionEditor_EndsWith_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Contains_Description:
                    break; 
                case EditorStringId.ExpressionEditor_IsThisWeek_Description:
                    break; 
                case EditorStringId.ExpressionEditor_IsThisMonth_Description:
                    break; 
                case EditorStringId.ExpressionEditor_IsThisYear_Description:
                    break; 
                case EditorStringId.ExpressionEditor_GridFields_Description_Prefix:
                    break; 
                case EditorStringId.ExpressionEditor_True_Description:
                    break; 
                case EditorStringId.ExpressionEditor_False_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Null_Description:
                    break; 
                case EditorStringId.ExpressionEditor_Title:
                    break; 
                case EditorStringId.FilterGroupAnd:
                    return "و";
                case EditorStringId.FilterGroupOr:
                    return "یا";
                case EditorStringId.FilterGroupNotAnd:
                    return "و نه";
                case EditorStringId.FilterGroupNotOr:
                    return "یا نه";
                case EditorStringId.FilterGroupAddCondition:
                    return "شرط جدید";
                case EditorStringId.FilterGroupAddGroup:
                    return "گروه جدید";
                case EditorStringId.FilterGroupRemoveGroup:
                    return "حذف گروه";
                case EditorStringId.FilterGroupClearAll:
                    return "حذف همه";
                case EditorStringId.FilterEmptyValueText:
                    return "{مقدار}";
                case EditorStringId.FilterClauseAnyOf:
                    return "هر یك از مقادیر";
                case EditorStringId.FilterClauseBeginsWith:
                    return "شروع شود";
                case EditorStringId.FilterClauseBetween:
                    return "مابین";
                case EditorStringId.FilterClauseBetweenAnd:
                    return "و";
                case EditorStringId.FilterClauseContains:
                    return "شامل شود";
                case EditorStringId.FilterClauseEndsWith:
                    return "ختم شود";
                case EditorStringId.FilterClauseEquals:
                    return "مساوی";
                case EditorStringId.FilterClauseGreater:
                    return "بزرگتر از";
                case EditorStringId.FilterClauseGreaterOrEqual:
                    return "بزرگتر و مساوی";
                case EditorStringId.FilterClauseIsNotNull:
                    return "خالی نباشد";
                case EditorStringId.FilterClauseIsNull:
                    return "خالی باشد";
                case EditorStringId.FilterClauseLess:
                    return "كمتر از";
                case EditorStringId.FilterClauseLessOrEqual:
                    return "كمتر و مساوی";
                case EditorStringId.FilterClauseLike:
                    return "مشابه";
                case EditorStringId.FilterClauseNoneOf:
                    return "هیچ یك از";
                case EditorStringId.FilterClauseNotBetween:
                    return "مابین نباشد";
                case EditorStringId.FilterClauseDoesNotContain:
                    return "شامل نشود";
                case EditorStringId.FilterClauseDoesNotEqual:
                    return "مساوی نباشد";
                case EditorStringId.FilterClauseNotLike:
                    return "مشابه نباشد";
                case EditorStringId.FilterClauseIsNullOrEmpty:
                    return "خالی باشد";
                case EditorStringId.FilterClauseIsNotNullOrEmpty:
                    return "خالی نباشد";
                case EditorStringId.FilterClauseIsBeyondThisYear:
                    return "برای سال های بعد";
                case EditorStringId.FilterClauseIsLaterThisYear:
                    return "سال های پیش";
                case EditorStringId.FilterClauseIsLaterThisMonth:
                    return "ماه های پیش";
                case EditorStringId.FilterClauseIsNextWeek:
                    return "هفته بعد";
                case EditorStringId.FilterClauseIsLaterThisWeek:
                    return "هفته پیش";
                case EditorStringId.FilterClauseIsTomorrow:
                    return "فردا";
                case EditorStringId.FilterClauseIsToday:
                    return "امروز";
                case EditorStringId.FilterClauseIsYesterday:
                    return "دیروز";
                case EditorStringId.FilterClauseIsEarlierThisWeek:
                    return "همین هفته";
                case EditorStringId.FilterClauseIsLastWeek:
                    return "هفته پیش";
                case EditorStringId.FilterClauseIsEarlierThisMonth:
                    return "همین ماه";
                case EditorStringId.FilterClauseIsEarlierThisYear:
                    return "امسال";
                case EditorStringId.FilterClauseIsPriorThisYear:
                    return "پارسال";
                case EditorStringId.FilterDateTimeOperatorMenuCaption:
                    return "عملیات تاریخی";
                case EditorStringId.FilterEditorChecked:
                    return "تایید شده";
                case EditorStringId.FilterEditorUnchecked:
                    return "تایید نشده";
                case EditorStringId.FilterToolTipNodeAction:
                    return "عملیات";
                case EditorStringId.FilterToolTipNodeAdd:
                    return "ایجاد شرط جدید برای این گروه";
                case EditorStringId.FilterToolTipNodeRemove:
                    return "حذف شرط";
                case EditorStringId.FilterToolTipValueType:
                    return "مقایسه با مقدار و یا مقادیر دیگر";
                case EditorStringId.FilterToolTipElementAdd:
                    return "اضافه كردن آیتم جدید به لیست";
                case EditorStringId.FilterToolTipKeysAdd:
                    return "از دكمه Insert و یا + استفاده كنید";
                case EditorStringId.FilterToolTipKeysRemove:
                    return "از دكمه Delete و یا - استفاده كنید";
                case EditorStringId.FilterPanelEditFilter:
                    return "ویرایش فیلتر";
                case EditorStringId.FilterPanelClearFilter:
                    return "حذف فیلتر";
                case EditorStringId.FilterPanelEnableFilter:
                    return "فعال سازی فیلتر";
                case EditorStringId.FilterPanelDisableFilter:
                    return "غیر فعال سازی فیلتر";
                case EditorStringId.PasswordBoxEditToolTipHeader:
                    return "Caps Lock روشن می باشد";
                case EditorStringId.PasswordBoxEditToolTipContent:
                    return "روشن بودن Caps Lock ممكن است منجر به ورود رمز اشتباه شود." + Environment.NewLine + "لطفا قبل از وارد كردن رمز آن را خاموش كنید.";
                case EditorStringId.WaitIndicatorText:
                    return "لطفا صبر كنید ...";
                case EditorStringId.CantModifySelectedDates:
                    break; 
                case EditorStringId.Page:
                    return "صفحه";
                case EditorStringId.Of:
                    return "از {0}";
                case EditorStringId.DisplayFormatTextControlExample:
                    return "به عنوان مثال : ";
                case EditorStringId.DisplayFormatTextControlPrefix:
                    return "پیشوند :";
                case EditorStringId.DisplayFormatTextControlSuffix:
                    return "پسوند :";
                case EditorStringId.DisplayFormatTextControlDisplayFormatText:
                    return "فرمت نمایش متن :";
                case EditorStringId.DisplayFormatHelperWrongDisplayFormatValue:
                    return "فرمت نمایش متن نا معتبر می باشد";
                case EditorStringId.DataTypeStringExample:
                    return "متن";
                case EditorStringId.DataTypeCharExample:
                    return "كاراكتر";
                case EditorStringId.DisplayFormatNullValue:
                    return "هیچ";
                case EditorStringId.DisplayFormatGroupTypeDefault:
                    return "پیش فرض";
                case EditorStringId.DisplayFormatGroupTypeNumber:
                    return "عدد";
                case EditorStringId.DisplayFormatGroupTypePercent:
                    return "درصد";
                case EditorStringId.DisplayFormatGroupTypeCurrency:
                    return "ریال";
                case EditorStringId.DisplayFormatGroupTypeSpecial:
                    return "بخصوص";
                case EditorStringId.DisplayFormatGroupTypeDatetime:
                    return "تاریخ";
                case EditorStringId.DisplayFormatGroupTypeCustom:
                    return "سفارشی";
                case EditorStringId.LookUpFind:
                    return "جستجو";
                case EditorStringId.LookUpSearch:
                    return "جستجو";
                case EditorStringId.LookUpClose:
                    return "بستن";
                case EditorStringId.LookUpAddNew:
                    return "جدید";
                case EditorStringId.ConfirmationDialogMessage:
                    return "فونت {0} در سیستم شما وجود ندارد. آیا می خواهید از آن استفاده كنید؟";
                case EditorStringId.ConfirmationDialogCaption:
                    return "سوال";
                case EditorStringId.CheckChecked:
                    return "تایید شده";
                case EditorStringId.CheckUnchecked:
                    return "تایید نشده";
                case EditorStringId.CheckIndeterminate:
                    return "نا مشخص";
                case EditorStringId.Today:
                    return "امروز";
            }
            return GetLocalizedString(id);
        }
    }
}
