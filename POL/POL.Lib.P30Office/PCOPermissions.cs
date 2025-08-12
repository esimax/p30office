using System.ComponentModel;
using POL.Lib.Interfaces;

namespace POL.Lib.XOffice
{
    public enum PCOPermissions
    {

        #region مدیریت كاربران

        [Category("مدیریت كاربران"), Browsable(true), Description("ورود به مدیریت كاربران"), InTamas] Membership_Settings = 1,
        [Category("مدیریت كاربران"), Browsable(true), Description("امكان ایجاد كاربر جدید"), InTamas] Membership_User_Add = 2,
        [Category("مدیریت كاربران"), Browsable(true), Description("امكان ویرایش اطلاعات كاربران"), InTamas] Membership_User_Edit = 3,

        [Category("مدیریت كاربران"), Browsable(true), Description("امكان حذف كاربران"), InTamas] Membership_User_Delete
        = 4,
        [Category("مدیریت كاربران"), Browsable(true), Description("امكان تغییر رمز كاربران"), InTamas] Membership_User_ResetPassword = 5,
        [Category("مدیریت كاربران"), Browsable(true), Description("امكان الحاق سطوح دسترسی به كاربران"), InTamas] Membership_User_LinkRole = 6,
        [Category("مدیریت كاربران"), Browsable(true), Description("امكان حذف سطوح دسترسی از كاربران"), InTamas] Membership_User_UnLinkRole = 7,

        [Category("مدیریت كاربران"), Browsable(true), Description("امكان ثبت سطوح دسترسی جدید"), InTamas] Membership_Role_Add = 8,
        [Category("مدیریت كاربران"), Browsable(true), Description("امكان ویرایش سطوح دسترسی"), InTamas] Membership_Role_Edit = 9,
        [Category("مدیریت كاربران"), Browsable(true), Description("امكان حذف سطوح دسترسی"), InTamas] Membership_Role_Delete = 10,

        [Category("مدیریت كاربران"), Browsable(true), Description("امكان تغییر مجوزها"), InTamas] Membership_Permission_Set = 11,
        [Category("مدیریت كاربران"), Browsable(true), Description("امكان استخراج به Xml"), InTamas] Membership_Role_Export = 252,

        [Category("مدیریت كاربران"), Browsable(true), Description("امكان ورود از Xml"), InTamas] Membership_Role_Import
        = 253,

        #endregion

        #region مدیریت اطلاعات فردی

        [Category("مدیریت اطلاعات فردی"), Browsable(true), Description("كاربر می تواند اطلاعات خود را تغییر دهد"),
         InTamas] Membership_ActiveUser_AllowEdit = 12,
        [Category("مدیریت اطلاعات فردی"), Browsable(true), Description("كاربر می تواند رمز خود را تغییر دهد"), InTamas] Membership_ActiveUser_AllowChangePassword = 13,

        #endregion

        #region پرونده > پرونده

        [Browsable(true), Category("پرونده > پرونده"), Description("نمایش پرونده ها"), InTamas] Contact_Contact_View =
            14,

        [Browsable(true), Category("پرونده > پرونده"), Description("ایجاد پرونده جدید"), InTamas] Contact_Contact_New =
            15,

        [Browsable(true), Category("پرونده > پرونده"), Description("ویرایش پرونده"), InTamas] Contact_Contact_Edit = 16,

        [Browsable(true), Category("پرونده > پرونده"), Description("حذف پرونده"), InTamas] Contact_Contact_Delete = 17,

        [Browsable(true), Category("پرونده > پرونده"), Description("امكان نمایش و ویرایش پرونده های دیگران"), InTamas] Contact_Contact_AllowEditUnowned = 30,

        [Browsable(true), Category("پرونده > پرونده"), Description("امكان تغییر ثبت كننده پرونده"), InTamas] Contact_Contact_AllowChangeCreator = 110,


        [Browsable(true), Category("پرونده > سبد انتخاب"), Description("ایجاد سبد انتخاب"), InTamas] Contact_Basket_New
        = 18,

        [Browsable(true), Category("پرونده > سبد انتخاب"), Description("ویرایش سبد انتخاب"), InTamas] Contact_Basket_Edit = 19,

        [Browsable(true), Category("پرونده > سبد انتخاب"), Description("حذف سبد انتخاب"), InTamas] Contact_Basket_Delete
        = 20,

        [Browsable(true), Category("پرونده > سبد انتخاب"), Description("اضافه به سبد انتخاب"), InTamas] Contact_Basket_Add = 21,

        [Browsable(true), Category("پرونده > سبد انتخاب"), Description("حذف از سبد انتخاب"), InTamas] Contact_Basket_Remove = 22,

        [Browsable(true), Category("پرونده > سبد انتخاب"), Description("عملیات تركیبی روی سبد انتخاب"), InTamas] Contact_Basket_Operation = 23,

        [Browsable(true), Category("پرونده > ابزار"), Description("جستجوی پیشرفته"), InTamas] Contact_Tools_AdvSearch =
            24,

        [Browsable(true), Category("پرونده > ابزار"), Description("ورود از اكسل"), InTamas] Contact_Tools_Import = 25,

        [Browsable(true), Category("پرونده > ابزار"), Description("خروج به اكسل"), InTamas] Contact_Tools_Export = 26,

        [Browsable(true), Category("پرونده > ابزار"), Description("چاپ"), InTamas] Contact_Tools_Print = 27,

        [Browsable(true), Category("پرونده > ابزار"), Description("گزارشات"), InTamas] Contact_Tools_Report = 28,

        [Browsable(true), Category("پرونده > ابزار"), Description("تنظیمات"), InTamas] Contact_Tools_Settings = 29,

        #endregion

        #region پرونده > پروفایل ها

        [Browsable(true), Category("پرونده > پروفایل ها"), Description("نمایش"), InTamas] Contact_Profile_View = 31,

        [Browsable(true), Category("پرونده > پروفایل ها"), Description("اضافه كردن"), InTamas] Contact_Profile_Add = 32,

        [Browsable(true), Category("پرونده > پروفایل ها"), Description("حذف پروفایل"), InTamas] Contact_Profile_DelRoot
        = 33,

        [Browsable(true), Category("پرونده > پروفایل ها"), Description("حذف گروه"), InTamas] Contact_Profile_DelGroup =
            34,

        [Browsable(true), Category("پرونده > پروفایل ها"), Description("حذف ماژول"), InTamas] Contact_Profile_DelItem =
            35,

        [Browsable(true), Category("پرونده > پروفایل ها"), Description("امكان جابجا كردن"), InTamas] Contact_Profile_Reorder = 36,

        #endregion

        #region پرونده > شماره تماس

        [Browsable(true), Category("پرونده > شماره تماس"), Description("نمایش"), InTamas] Contact_Phones_View = 37,

        [Browsable(true), Category("پرونده > شماره تماس"), Description("ثبت شماره جدید"), InTamas] Contact_Phones_New =
            38,

        [Browsable(true), Category("پرونده > شماره تماس"), Description("ویرایش شماره"), InTamas] Contact_Phones_Edit =
            39,

        [Browsable(true), Category("پرونده > شماره تماس"), Description("حذف شماره"), InTamas] Contact_Phones_Delete = 40,

        [Browsable(true), Category("پرونده > شماره تماس"), Description("برش و جایگذاری"), InTamas] Contact_Phones_CopyCut = 41,

        [Browsable(true), Category("پرونده > شماره تماس"), Description("محاسبه اطلاعات آماری"), InTamas] Contact_Phones_Calculate = 42,

        [Browsable(true), Category("پرونده > شماره تماس"), Description("نمایش كامل"), InTamas] Contact_Phones_FullView =
            43,

        [Browsable(true), Category("پرونده > شماره تماس"), Description("امكان چاپ"), InTamas] Contact_Phones_Print = 44,

        #endregion

        #region پرونده > تماس های ارسالی و در یافتی

        [Browsable(true), Category("پرونده > تماس های ارسالی و در یافتی"), Description("نمایش"), InTamas] Contact_Calls_View = 45,

        [Browsable(true), Category("پرونده > تماس های ارسالی و در یافتی"), Description("ثبت نكته"), InTamas] Contact_Calls_AddTag = 46,

        [Browsable(true), Category("پرونده > تماس های ارسالی و در یافتی"), Description("حذف"), InTamas] Contact_Calls_Delete = 47,

        [Browsable(true), Category("پرونده > تماس های ارسالی و در یافتی"), Description("پخش صدا"), InTamas] Contact_Calls_RecordPlay = 48,

        [Browsable(true), Category("پرونده > تماس های ارسالی و در یافتی"), Description("ذخیره صدا"), InTamas] Contact_Calls_RecordSave = 49,

        [Browsable(true), Category("پرونده > تماس های ارسالی و در یافتی"), Description("حذف صدا"), InTamas] Contact_Calls_RecordDelete = 50,

        [Browsable(true), Category("پرونده > تماس های ارسالی و در یافتی"), Description("امكان تنظیم قوانین"), InTamas] Contact_Calls_RecordChangeRole = 51,

        #endregion

        #region پرونده > ایمیل

        [Browsable(true), Category("پرونده > ایمیل"), Description("نمایش")] Contact_Email_View = 52,

        [Browsable(true), Category("پرونده > ایمیل"), Description("ایجاد ایمیل جدید")] Contact_Email_Add = 53,

        [Browsable(true), Category("پرونده > ایمیل"), Description("ویرایش ایمیل")] Contact_Email_Edit = 54,

        [Browsable(true), Category("پرونده > ایمیل"), Description("حذف حساب ایمیل")] Contact_Email_Delete = 55,

        [Browsable(true), Category("پرونده > ایمیل"), Description("ارسال ایمیل")] Contact_Email_Send = 56,

        [Browsable(true), Category("پرونده > ایمیل"), Description("امكان پاسخ به ایمیل")] Contact_Email_Reply = 57,

        [Browsable(true), Category("پرونده > ایمیل"), Description("امكان ارجاع ایمیل")] Contact_Email_Forward = 58,

        [Browsable(true), Category("پرونده > ایمیل"), Description("امكان حذف ایمیل")] Contact_Email_Trash = 59,

        [Browsable(true), Category("پرونده > ایمیل"), Description("امكان چاپ محتوای ایمیل")] Contact_Email_Print = 60,

        #endregion

        #region پرونده > آدرس

        [Browsable(true), Category("پرونده > آدرس"), Description("نمایش"), InTamas] Contact_Address_View = 61,

        [Browsable(true), Category("پرونده > آدرس"), Description("ثبت آدرس جدید"), InTamas] Contact_Address_Add = 62,

        [Browsable(true), Category("پرونده > آدرس"), Description("ویرایش آدرس"), InTamas] Contact_Address_Edit = 63,

        [Browsable(true), Category("پرونده > آدرس"), Description("حذف آدرس"), InTamas] Contact_Address_Delete = 64,

        [Browsable(true), Category("پرونده > آدرس"), Description("كپی كردن آدرس"), InTamas] Contact_Address_Copy = 65,

        [Browsable(true), Category("پرونده > آدرس"), Description("برش زدن آدرس"), InTamas] Contact_Address_Cut = 66,

        [Browsable(true), Category("پرونده > آدرس"), Description("چاپ آدرس"), InTamas] Contact_Address_Print = 67,

        [Browsable(true), Category("پرونده > آدرس"), Description("الحاق به نقشه"), InTamas] Contact_Address_MapAdd = 68,

        [Browsable(true), Category("پرونده > آدرس"), Description("حذف از روی نقشه"), InTamas] Contact_Address_MapDelete
        = 69,

        #endregion

        #region پرونده > فاكتور

        [Browsable(true), Category("پرونده > فاكتور"), Description("نمایش")] Contact_Factor_View = 243,

        [Browsable(true), Category("پرونده > فاكتور"), Description("ثبت فاكتور جدید")] Contact_Factor_Add = 244,

        [Browsable(true), Category("پرونده > فاكتور"), Description("ویرایش فاكتور")] Contact_Factor_Edit = 245,

        [Browsable(true), Category("پرونده > فاكتور"), Description("حذف فاكتور")] Contact_Factor_Delete = 246,

        #endregion

        #region پرونده > ارتباطات

        [Browsable(true), Category("پرونده > ارتباطات"), Description("نمایش")] Contact_Link_View = 70,

        [Browsable(true), Category("پرونده > ارتباطات"), Description("ثبت ارتباطات جدید")] Contact_Link_Add = 71,

        [Browsable(true), Category("پرونده > ارتباطات"), Description("ویرایش ارتباطات")] Contact_Link_Edit = 72,

        [Browsable(true), Category("پرونده > ارتباطات"), Description("حذف ارتباطات")] Contact_Link_Delete = 73,

        #endregion

        #region پرونده > كارتابل

        [Browsable(true), Category("پرونده > كارتابل"), Description("نمایش")] Contact_Automations_View = 238,

        #endregion

        #region مدیریت تماس ها

        [Browsable(true), Category("مدیریت تماس ها"), Description("امكان مشاهده تماس ها"), InTamas] Call_View = 193,

        [Browsable(true), Category("مدیریت تماس ها"), Description("امكان ثبت نكته"), InTamas] Call_TagAdd = 74,

        [Browsable(true), Category("مدیریت تماس ها"), Description("امكان فیلترینگ اطلاعات"), InTamas] Call_AllowFilter =
            75,

        [Browsable(true), Category("مدیریت تماس ها"), Description("مشاهده تماس های دریافتی"), InTamas] Call_AllowCallIn
        = 76,

        [Browsable(true), Category("مدیریت تماس ها"), Description("مشاهده تماس های ارسالی"), InTamas] Call_AllowCallOut
        = 77,

        [Browsable(true), Category("مدیریت تماس ها"), Description("امكان حذف تماس"), InTamas] Call_AllowDelete = 78,

        [Browsable(true), Category("مدیریت تماس ها"), Description("امكان اصلاح تماس"), InTamas] Call_AllowCorrection =
            79,

        [Browsable(true), Category("مدیریت تماس ها"), Description("امكان تطبیق تماس ها"), InTamas] Call_AllowSync = 80,

        [Browsable(true), Category("مدیریت تماس ها"), Description("امكان تعیین كاربر در ایجاد پرونده در تطبیق تماس ها"),
         InTamas] Call_AllowSelectUserInSync = 198,

        [Browsable(true), Category("مدیریت تماس ها"), Description("امكان چاپ"), InTamas] Call_AllowPrint = 81,

        [Browsable(true), Category("مدیریت تماس ها"), Description("امكان استفاده از گزارش نموداری"), InTamas] Call_AllowChart = 82,

        [Browsable(true), Category("مدیریت تماس ها"), Description("پخش مكالمات"), InTamas] Call_Record_Play = 83,

        [Browsable(true), Category("مدیریت تماس ها"), Description("ذخیره مكالمات"), InTamas] Call_Record_Save = 84,

        [Browsable(true), Category("مدیریت تماس ها"), Description("حذف مكالمات"), InTamas] Call_Record_Delete = 85,

        [Browsable(true), Category("مدیریت تماس ها"), Description("تغییر قوانین حذف مكالمات"), InTamas] Call_Record_SetRole = 86,

        [Browsable(true), Category("مدیریت تماس ها"), Description("فقط امكان مشاهده داخلی مربوطه"), InTamas] Call_Internal_Only = 249,

        #endregion

        #region مدیریت ایمیل

        [Browsable(true), Category("مدیریت ایمیل"), Description("مشاهده ایمیل ها")] Email_View = 194,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان ارسال ایمیل جدید")] Email_Send = 87,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان پاسخ دادن به ایمیل")] Email_Reply = 88,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان ارجاع ایمیل")] Email_Forward = 89,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان حذف ایمیل")] Email_Trash = 90,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان تطبیق با پرونده ها")] Email_Sync = 91,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان ایجاد پوشه")] Email_Folder_Add = 92,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان ویرایش پوشه")] Email_Folder_Edit = 93,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان حذف پوشه")] Email_Folder_Delete = 94,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان دسترسی به ایمیل های دریافتی")] Email_Access_Inbox
        = 95,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان دسترسی به ایمیل های ارسالی")] Email_Access_Sent =
            96,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان دسترسی به ایمیل های پیش نویس")] Email_Access_Draft = 97,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان دسترسی به ایمیل های حذف شده")] Email_Access_Trash
        = 98,

        [Browsable(true), Category("مدیریت ایمیل"), Description("امكان دسترسی به ایمیل های داخل پوشه")] Email_Access_Folder = 99,

        [Browsable(true), Category("مدیریت ایمیل"), Description("تغییر اولویت ایمیل های در نوبت به ارسال فوری")] Email_SendNow = 210,

        [Browsable(true), Category("مدیریت ایمیل"), Description("ورود اطلاعات از اكسل")] Email_Import = 247,

        [Browsable(true), Category("مدیریت ایمیل"), Description("استخراج به اكسل")] Email_Export = 248,

        #endregion

        #region مدیریت فرم ها

        [Browsable(true), Category("مدیریت فرم ها"), Description("مشاهده فرم ها"), InTamas] Profile_View = 195,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان ایجاد فرم جدید")] Profile_RootAdd = 100,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكلن ویرایش فرم")] Profile_RootEdit = 101,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان حذف فرم")] Profile_RootDelete = 102,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان ایجاد گروه جدید")] Profile_GroupAdd = 103,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان ویرایش گروه")] Profile_GroupEdit = 104,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان حذف گروه")] Profile_GroupDelete = 105,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان ایجاد ماژول جدید")] Profile_ItemAdd = 106,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان ویرایش ماژول")] Profile_ItemEdit = 107,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان حذف ماژول")] Profile_ItemDelete = 108,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان جابجایی")] Profile_Move = 109,


        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان الحاق به پرونده"), InTamas] Profile_LinkContact
        = 111,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان انفصال از پرونده"), InTamas] Profile_UnlinkContact = 112,

        [Browsable(true), Category("مدیریت فرم ها"), Description("امكان مرتب سازی پرونده ها")] Profile_ReorderContact =
            113,

        [Browsable(true), Category("مدیریت فرم ها"), Description("استخراج به XML")] Profile_ExportXML = 114,

        [Browsable(true), Category("مدیریت فرم ها"), Description("ورود از XML")] Profile_ImportXML = 115,

        #endregion

        #region مدیریت كشور / شهر

        [Browsable(true), Category("مدیریت كشور / شهر"), Description("مشاهده كشور / شهر ها"), InTamas] CountryCity_View
        = 196,

        [Browsable(true), Category("مدیریت كشور / شهر"), Description("ویرایش اطلاعات كشور ها"), InTamas] CountryCity_CountryEdit = 116,

        [Browsable(true), Category("مدیریت كشور / شهر"), Description("حذف اطلاعات كشور ها"), InTamas] CountryCity_CountryDelete = 255,

        [Browsable(true), Category("مدیریت كشور / شهر"), Description("مدیریت شهرها"), InTamas] CountryCity_ManageCity =
            117,

        [Browsable(true), Category("مدیریت كشور / شهر"), Description("مدیریت كد مخابراتی"), InTamas] CountryCity_ManageTeleCode = 118,

        [Browsable(true), Category("مدیریت كشور / شهر"), Description("استخراج به XML"), InTamas] CountryCity_ExportXML =
            119,

        #endregion

        #region جداول پایه > دسته ها

        [Browsable(true), Category("جداول پایه > دسته ها"), Description("جدید"), InTamas] BaseTable_Category_Add = 120,

        [Browsable(true), Category("جداول پایه > دسته ها"), Description("ویرایش"), InTamas] BaseTable_Category_Edit =
            121,

        [Browsable(true), Category("جداول پایه > دسته ها"), Description("حذف"), InTamas] BaseTable_Category_Delete = 122,

        [Browsable(true), Category("جداول پایه > دسته ها"), Description("استخراج به XML"), InTamas] BaseTable_Category_Export = 123,

        [Browsable(true), Category("جداول پایه > دسته ها"), Description("ورود از XML"), InTamas] BaseTable_Category_Import = 124,

        #endregion

        #region جداول پایه > عنوان شماره تماس

        [Browsable(true), Category("جداول پایه > عنوان شماره تماس"), Description("جدید")] BaseTable_CallTitle_Add = 125,

        [Browsable(true), Category("جداول پایه > عنوان شماره تماس"), Description("ویرایش")] BaseTable_CallTitle_Edit =
            126,

        [Browsable(true), Category("جداول پایه > عنوان شماره تماس"), Description("حذف")] BaseTable_CallTitle_Delete =
            127,

        [Browsable(true), Category("جداول پایه > عنوان شماره تماس"), Description("استخراج به XML")] BaseTable_CallTitle_Export = 128,

        [Browsable(true), Category("جداول پایه > عنوان شماره تماس"), Description("ورود از XML")] BaseTable_CallTitle_Import = 129,

        #endregion

        #region جداول پایه > عنوان آدرس ها

        [Browsable(true), Category("جداول پایه > عنوان آدرس ها"), Description("جدید")] BaseTable_AddressTitle_Add = 130,

        [Browsable(true), Category("جداول پایه > عنوان آدرس ها"), Description("ویرایش")] BaseTable_AddressTitle_Edit =
            131,

        [Browsable(true), Category("جداول پایه > عنوان آدرس ها"), Description("حذف")] BaseTable_AddressTitle_Delete =
            132,

        [Browsable(true), Category("جداول پایه > عنوان آدرس ها"), Description("استخراج به XML")] BaseTable_AddressTitle_Export = 133,

        [Browsable(true), Category("جداول پایه > عنوان آدرس ها"), Description("ورود از XML")] BaseTable_AddressTitle_Import = 134,

        #endregion

        #region جداول پایه > عنوان ایمیل ها

        [Browsable(true), Category("جداول پایه > عنوان ایمیل ها"), Description("جدید")] BaseTable_EmailTitle_Add = 135,

        [Browsable(true), Category("جداول پایه > عنوان ایمیل ها"), Description("ویرایش")] BaseTable_EmailTitle_Edit =
            136,

        [Browsable(true), Category("جداول پایه > عنوان ایمیل ها"), Description("حذف")] BaseTable_EmailTitle_Delete = 137,

        [Browsable(true), Category("جداول پایه > عنوان ایمیل ها"), Description("استخراج به XML")] BaseTable_EmailTitle_Export = 138,

        [Browsable(true), Category("جداول پایه > عنوان ایمیل ها"), Description("ورود از XML")] BaseTable_EmailTitle_Import = 139,

        #endregion

        #region جداول پایه > نوع ارتباطات

        [Browsable(true), Category("جداول پایه > نوع ارتباطات"), Description("جدید")] BaseTable_Link_Add = 140,

        [Browsable(true), Category("جداول پایه > نوع ارتباطات"), Description("ویرایش")] BaseTable_Link_Edit = 141,

        [Browsable(true), Category("جداول پایه > نوع ارتباطات"), Description("حذف")] BaseTable_Link_Delete = 142,

        [Browsable(true), Category("جداول پایه > نوع ارتباطات"), Description("استخراج به XML")] BaseTable_Link_Import =
            143,

        [Browsable(true), Category("جداول پایه > نوع ارتباطات"), Description("ورود از XML")] BaseTable_Link_Export = 144,

        [Browsable(true), Category("جداول پایه > نوع ارتباطات"), Description("مدیریت اطلاعات فرعی")] BaseTable_Link_ManageSub = 145,

        #endregion

        #region جداول پایه > جدول پروفایل ها

        [Browsable(true), Category("جداول پایه > جدول پروفایل ها"), Description("جدید"), InTamas] BaseTable_ProfileTable_Add = 147,

        [Browsable(true), Category("جداول پایه > جدول پروفایل ها"), Description("ویرایش"), InTamas] BaseTable_ProfileTable_Edit = 148,

        [Browsable(true), Category("جداول پایه > جدول پروفایل ها"), Description("حذف"), InTamas] BaseTable_ProfileTable_Delete = 149,

        [Browsable(true), Category("جداول پایه > جدول پروفایل ها"), Description("استخراج به XML"), InTamas] BaseTable_ProfileTable_Import = 150,

        [Browsable(true), Category("جداول پایه > جدول پروفایل ها"), Description("ورود از XML"), InTamas] BaseTable_ProfileTable_Export = 151,

        [Browsable(true), Category("جداول پایه > جدول پروفایل ها"), Description("مدیریت مقادیر"), InTamas] BaseTable_ProfileTable_ManageValues = 152,

        #endregion

        #region جداول پایه > لیست ها

        [Browsable(true), Category("جداول پایه > لیست ها"), Description("جدید")] BaseTable_List_Add = 153,

        [Browsable(true), Category("جداول پایه > لیست ها"), Description("ویرایش")] BaseTable_List_Edit = 154,

        [Browsable(true), Category("جداول پایه > لیست ها"), Description("حذف")] BaseTable_List_Delete = 155,



        #endregion

        #region پاپ آپ

        [Browsable(true), Category("پاپ آپ"), Description("نمایش پاپ آپ"), InTamas] Popup_Enabled = 157,
        [Browsable(true), Category("پاپ آپ"), Description("خط 1"), InTamas] Popup_Line1 = 158,
        [Browsable(true), Category("پاپ آپ"), Description("خط 2"), InTamas] Popup_Line2 = 159,
        [Browsable(true), Category("پاپ آپ"), Description("خط 3"), InTamas] Popup_Line3 = 160,
        [Browsable(true), Category("پاپ آپ"), Description("خط 4"), InTamas] Popup_Line4 = 161,
        [Browsable(true), Category("پاپ آپ"), Description("خط 5"), InTamas] Popup_Line5 = 162,
        [Browsable(true), Category("پاپ آپ"), Description("خط 6"), InTamas] Popup_Line6 = 163,
        [Browsable(true), Category("پاپ آپ"), Description("خط 7"), InTamas] Popup_Line7 = 164,
        [Browsable(true), Category("پاپ آپ"), Description("خط 8"), InTamas] Popup_Line8 = 165,
        [Browsable(true), Category("پاپ آپ"), Description("خط 9"), InTamas] Popup_Line9 = 166,
        [Browsable(true), Category("پاپ آپ"), Description("خط 10"), InTamas] Popup_Line10 = 167,
        [Browsable(true), Category("پاپ آپ"), Description("خط 11"), InTamas] Popup_Line11 = 168,
        [Browsable(true), Category("پاپ آپ"), Description("خط 12"), InTamas] Popup_Line12 = 169,
        [Browsable(true), Category("پاپ آپ"), Description("خط 13"), InTamas] Popup_Line13 = 170,
        [Browsable(true), Category("پاپ آپ"), Description("خط 14"), InTamas] Popup_Line14 = 171,
        [Browsable(true), Category("پاپ آپ"), Description("خط 15"), InTamas] Popup_Line15 = 172,
        [Browsable(true), Category("پاپ آپ"), Description("خط 16"), InTamas] Popup_Line16 = 173,
        [Browsable(true), Category("پاپ آپ"), Description("خط 17"), InTamas] Popup_Line17 = 174,
        [Browsable(true), Category("پاپ آپ"), Description("خط 18"), InTamas] Popup_Line18 = 175,
        [Browsable(true), Category("پاپ آپ"), Description("خط 19"), InTamas] Popup_Line19 = 176,
        [Browsable(true), Category("پاپ آپ"), Description("خط 20"), InTamas] Popup_Line20 = 177,
        [Browsable(true), Category("پاپ آپ"), Description("خط 21"), InTamas] Popup_Line21 = 178,
        [Browsable(true), Category("پاپ آپ"), Description("خط 22"), InTamas] Popup_Line22 = 179,
        [Browsable(true), Category("پاپ آپ"), Description("خط 23"), InTamas] Popup_Line23 = 180,
        [Browsable(true), Category("پاپ آپ"), Description("خط 24"), InTamas] Popup_Line24 = 181,
        [Browsable(true), Category("پاپ آپ"), Description("خط 25"), InTamas] Popup_Line25 = 182,
        [Browsable(true), Category("پاپ آپ"), Description("خط 26"), InTamas] Popup_Line26 = 183,
        [Browsable(true), Category("پاپ آپ"), Description("خط 27"), InTamas] Popup_Line27 = 184,
        [Browsable(true), Category("پاپ آپ"), Description("خط 28"), InTamas] Popup_Line28 = 185,
        [Browsable(true), Category("پاپ آپ"), Description("خط 29"), InTamas] Popup_Line29 = 186,
        [Browsable(true), Category("پاپ آپ"), Description("خط 30"), InTamas] Popup_Line30 = 187,
        [Browsable(true), Category("پاپ آپ"), Description("خط 31"), InTamas] Popup_Line31 = 188,
        [Browsable(true), Category("پاپ آپ"), Description("خط 32"), InTamas] Popup_Line32 = 189,

        [Browsable(true), Category("پاپ آپ"), Description("خط 33 تا 64"), InTamas] Popup_Line33_64 = 190,
        [Browsable(true), Category("پاپ آپ"), Description("خط 65 تا 128"), InTamas] Popup_Line65_128 = 191,
        [Browsable(true), Category("پاپ آپ"), Description("خط 129 تا 256"), InTamas] Popup_Line128_256 = 192,

        #endregion

        #region جداول پایه

        [Browsable(true), Category("جداول پایه"), Description("مشاهده جداول پایه"), InTamas] BaseTable_View = 197,

        #endregion

        #region سررسید

        [Browsable(true), Category("سررسید"), Description("نمایش سررسید"), InTamas] Calendar_View = 199,

        #endregion

        #region شماره ها

        [Browsable(true), Category("شماره ها"), Description("نمایش شماره ها"), InTamas] Phones_View = 200,
        [Browsable(true), Category("شماره ها"), Description("ویرایش شماره"), InTamas] Phones_Edit = 201,
        [Browsable(true), Category("شماره ها"), Description("حذف شماره"), InTamas] Phones_Delete = 202,
        [Browsable(true), Category("شماره ها"), Description("استخراج شماره ها به اكسل"), InTamas] Phones_Export = 203,
        [Browsable(true), Category("شماره ها"), Description("ورود شماره ها از اكسل"), InTamas] Phones_Import = 204,

        [Browsable(true), Category("شماره ها"), Description("امكان استفاده از گزارش ساز"), InTamas] Phones_PrintDesigner
        = 212,

        [Browsable(true), Category("شماره ها"), Description("امكان محاسبه اطلاعات آماری"), InTamas] Phones_CalculateStat
        = 251,

        #endregion

        #region آدرس ها

        [Browsable(true), Category("آدرس ها"), Description("نمایش آدرس ها"), InTamas] Addresses_View = 213,
        [Browsable(true), Category("آدرس ها"), Description("ویرایش آدرس"), InTamas] Addresses_Edit = 214,
        [Browsable(true), Category("آدرس ها"), Description("حذف آدرس"), InTamas] Addresses_Delete = 215,
        [Browsable(true), Category("آدرس ها"), Description("استخراج آدرس ها به اكسل"), InTamas] Addresses_Export = 216,
        [Browsable(true), Category("آدرس ها"), Description("ورود آدرس ها از اكسل"), InTamas] Addresses_Import = 217,
        [Browsable(true), Category("آدرس ها"), Description("امكان استفاده از گزارش ساز"), InTamas] Addresses_PrintDesigner = 218,

        #endregion

        #region مدیریت پیامك

        [Browsable(true), Category("مدیریت پیامك"), Description("امكان مشاهده پیامك ها")] SMS_View = 205,

        [Browsable(true), Category("مدیریت پیامك"), Description("امكان حذف پیامك ها")] SMS_Delete = 206,

        [Browsable(true), Category("مدیریت پیامك"), Description("امكان ارسال")] SMS_Send = 207,

        [Browsable(true), Category("مدیریت پیامك"), Description("امكان ثبت نكته - منسوخ شده")] SMS_TagAdd = 232,

        [Browsable(true), Category("مدیریت پیامك"), Description("امكان استخراج به اكسل")] SMS_Export = 254,

        #endregion

        #region فیلترینگ خط

        [Browsable(true), Category("فیلترینگ خط"), Description("تماسهای خط 1 قابل مشاهده می باشد"), InTamas] Line_Filter_1 = 146,

        [Browsable(true), Category("فیلترینگ خط"), Description("تماسهای خط 2 قابل مشاهده می باشد"), InTamas] Line_Filter_2 = 156,

        [Browsable(true), Category("فیلترینگ خط"), Description("تماسهای خط 3 قابل مشاهده می باشد"), InTamas] Line_Filter_3 = 208,

        [Browsable(true), Category("فیلترینگ خط"), Description("تماسهای خط 4 قابل مشاهده می باشد"), InTamas] Line_Filter_4 = 209,

        #endregion

        #region مدیریت گزارش فرم ها

        [Browsable(true), Category("مدیریت گزارش فرم ها"), Description("مشاهده گزارش فرم ها"), InTamas] ProfileReport_View = 211,

        #endregion

        #region اتوماسیون

        [Browsable(true), Category("اتوماسیون"), Description("مشاهده اتوماسیون")] Automations_View = 219,

        [Browsable(true), Category("اتوماسیون"), Description("ایجاد اتوماسیون جدید")] Automations_Add = 220,

        [Browsable(true), Category("اتوماسیون"), Description("ویرایش اتوماسیون")] Automations_Edit = 221,

        [Browsable(true), Category("اتوماسیون"), Description("حذف اتوماسیون")] Automations_Delete = 222,

        #endregion

        #region كارتابل

        [Browsable(true), Category("كارتابل"), Description("مشاهده كارتابل")] CardTables_View = 223,

        [Browsable(true), Category("كارتابل"), Description("ایجاد كارتابل جدید")] CardTables_Add = 224,

        [Browsable(true), Category("كارتابل"), Description("ویرایش كارتابل")] CardTables_Edit = 225,

        [Browsable(true), Category("كارتابل"), Description("حذف كارتابل")] CardTables_Delete = 226,

        #endregion

        #region مدیریت گزارش لیست ها

        [Browsable(true), Category("مدیریت گزارش لیست ها"), Description("مشاهده گزارش لیست ها")] ProfileList_View = 227,
        [Browsable(true), Category("مدیریت گزارش لیست ها"), Description("امكان ایجاد ركورد جدید")] ProfileList_Add = 228,

        [Browsable(true), Category("مدیریت گزارش لیست ها"), Description("امكان ویرایش ركورد موجود")] ProfileList_Edit =
            229,
        [Browsable(true), Category("مدیریت گزارش لیست ها"), Description("امكان حذف ركورد")] ProfileList_Delete = 230,

        [Browsable(true), Category("مدیریت گزارش لیست ها"), Description("امكان استخراج به اكسل")] ProfileList_Export =
            231,

        #endregion

        #region داشبورد

        [Browsable(true), Category("داشبورد"), Description("مشاهده داشبورد"), InTamas] Dashboard_View = 233,

        #endregion

        #region برنامه ریزی

        [Browsable(false), Category("برنامه ریزی"), Description("مشاهده برنامه ریزی")] EventUnit_View = 234,
        [Browsable(false), Category("برنامه ریزی"), Description("ثبت رخداد جدید")] EventUnit_Add = 235,
        [Browsable(false), Category("برنامه ریزی"), Description("ویرایش رخداد")] EventUnit_Edit = 236,
        [Browsable(false), Category("برنامه ریزی"), Description("حذف رخداد")] EventUnit_Delete = 237,

        #endregion

        #region فاكتور

        [Browsable(true), Category("فاكتور"), Description("مشاهده فاكتور ها")] Factor_View = 239,
        [Browsable(true), Category("فاكتور"), Description("ثبت فاكتور جدید")] Factor_Add = 240,
        [Browsable(true), Category("فاكتور"), Description("ویرایش فاكتور")] Factor_Edit = 241,
        [Browsable(true), Category("فاكتور"), Description("حذف فاكتور")] Factor_Delete = 242,

        #endregion

        #region محصولات

        [Browsable(true), Category("محصولات"), Description("مشاهده محصولات")] Product_View = 256,
        [Browsable(true), Category("محصولات"), Description("ثبت محصول جدید")] Product_Add = 257,
        [Browsable(true), Category("محصولات"), Description("ویرایش محصول")] Product_Edit = 258,
        [Browsable(true), Category("محصولات"), Description("حذف محصول")] Product_Delete = 259,
        [Browsable(true), Category("محصولات"), Description("استخراج به اکسل")] Product_Export = 260,
        [Browsable(true), Category("محصولات"), Description("ورود اطلاعات از اکسل")] Product_Import = 261,

        #endregion

        #region نوار ابزار

        [Browsable(true), Category("نوار ابزار"), Description("امكان مشاهده مانیتورینگ")] Tools_ViewMonitoring = 250

        #endregion


    }
}
