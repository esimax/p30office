using System.ComponentModel;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public enum EnumSMSSendResult
    {
        [DataMember] InvalidCredential,
        [DataMember] Success,
        [DataMember] NoCredit,
        [DataMember] LimitedDailyCount,
        [DataMember] LimitedDailySize,
        [DataMember] InvalidNumber,
        [DataMember] UpdatingService,
        [DataMember] FilteredWords,
        [DataMember] UnreachedMinLimits,
        [DataMember] CanNotSendFromPublicLine,
        [DataMember] UserDenide,
        [DataMember] CanNotSend,

        [DataMember] [Description("شماره دریافت كننده خالی است")] IranTc_InvalidToNumber,
        [DataMember] [Description("شماره ارسال كننده نامعتبر است")] IranTc_InvalidFromNumber,
        [DataMember] [Description("پارامتر انكودینگ نامعتبر است")] IranTc_InvalidEncoderparameter,
        [DataMember] [Description("پارامتر MessageClass نامعتبر است")] IranTc_InvalidMesClassParameter,
        [DataMember] [Description("پارامتر UDH نامعتبر است")] IranTc_InvalidUDHParameter,
        [DataMember] [Description("پیامك خالی است")] IranTc_BodyIsEmpty,
        [DataMember] [Description("مشكل نامشخص رخ داده است")] IranTc_UnkownError,
        [DataMember] [Description("سرور در هنگام ارسال پیام مشغول برطرف نمودن ایراد داخلی بوده است.")] IranTc_ServerIsBusy,
        [DataMember] [Description("حساب غیر فعال می باشد")] IranTc_AccountIsDisable,
        [DataMember] [Description("حساب منقضی شده است")] IranTc_AccountIsExpired,
        [DataMember] [Description("درخواست دارای اعتبار نمی باشد")] IranTc_InvalidCredit,
        [DataMember] [Description("استفاده از این سرویس برای این حساب مقدور نمی باشد")] IranTc_SeviceAccessDenide,
        [DataMember] [Description("به دلیل ترافیك بالا سرور آمادگی دریافت پیام جدید را ندارد")] IranTc_HighTrafic,
        [DataMember] [Description("شناسه پیامك معتبر نمی باشد")] IranTc_InvalidCredential,
        [DataMember] [Description("نوع سرویس درخواستی تامعتبر است")] IranTc_InvalidService,
        [DataMember] [Description("شماره فرستنده در لیست غیر فعال شركت همراه اول قرار دارد")] IranTc_BlockedNumber,
        [DataMember] [Description("آرایه گیرندگان خالی می باشد")] IranTc_EmptyReceivers,
        [DataMember] [Description("طول آرایه پارامتر گیرندگان بیشتر از طول مجاز می باشد")] IranTc_ReciversExceedLimits,
        [DataMember] [Description("شماره فرستنده خالی می باشد")] IranTc_EmptySender,
        [DataMember] [Description("نام كاربری یا كلمه عبور اشتباه است")] IranTc_InvalidUsernamePassword,
        [DataMember] [Description("شماره گیرنده مجاز نمی باشد و یا متن اس ام اس خالی می باشد")] IranTc_EmptyBody,
        [DataMember] [Description("ارسال كننده اعتبار كافی ندارد")] IranTc_InsufficientCredit,
        [DataMember] [Description("خطا در برقراری ارتباط با وب سرویس")] IranTc_InvalidConnection,


        [DataMember] [Description("شناسه پیامك نامعتبر است")] Sunway_InvalidCredential,
        [DataMember] [Description("هنوز وضعیتی دریافت نشده است")] Sunway_InvalidYetRecheckAgain,
        [DataMember] [Description("با مووفقیت به موبایل رسید")] Sunway_OK,
        [DataMember] [Description("پیامك به موبایل گیرنده نرسیده و نخواهد رسید")] Sunway_InvalidDoNotCheck,
        [DataMember] [Description("پیامك در صف مخابرات قرار دارد")] Sunway_InQueue,
        [DataMember] [Description("پیامك به موبایل گیرنده نرسیده و نخواهد رسید")] Sunway_Invalid2DoNotCheck,
        [DataMember] [Description("پیامك به موبایل گیرنده نرسیده و نخواهد رسید")] Sunway_BlockedDoNotCheck,
        [DataMember] [Description("پیامك در صف مخابرات قرار دارد")] Sunway_InQueue2,
        [DataMember] [Description("در حال ارسال می باشد.")] Sunway_Sending,
        [DataMember] [Description("اعتبار كافی نمی باشدف پیامك ارسال نشد")] Sunway_OutOfCreditDoNotCheck,
        [DataMember] [Description("پیام هنوز ارسال نشده است")] Sunway_Warning,
        [DataMember] [Description("نامشخص")] Sunway_Unknown
    }
}
