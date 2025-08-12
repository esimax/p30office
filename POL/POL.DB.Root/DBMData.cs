using System;
using DevExpress.Xpo;

namespace POL.DB.Root
{
    public class DBMData : XPObject
    {
        #region CTOR


        public DBMData(Session session) : base(session)
        {
        }


        #endregion

        public static string GetGroupText(int p)
        {
            if (p == 42) return "آب و فاضلاب";
            if (p == 50) return "آرایشی و بهداشتی";
            if (p == 01) return "آلومینیوم و فرآورده های آلومینیومی";
            if (p == 06) return "ابزار آلات";
            if (p == 08) return "اتوماسیون - ابزار دقیق";
            if (p == 38) return "ادارات دولتی (وزارتخانه ها، سازمانها و ...)";
            if (p == 20) return "پیمانكاران";
            if (p == 02) return "تبلیغات و بازاریابی - بانكهای اطلاعاتی، صنعت فیلم";
            if (p == 31) return "تجهیزات آتش نشانی، سیستم های هشدار دهنده و ترافیكی";
            if (p == 44) return "تجهیزات آزمایشگاهی";
            if (p == 54) return "تجهیزات صنعت نفت، گاز و پتروشیمی، فرآورده های نفتی، گازی و پتروشیمی - سوخت";
            if (p == 23) return "توزیع كنندگان (شركتهای پخش)";
            if (p == 47) return "تكنولوژی جابجایی (نقاله ها - بالابرها - آسانسور و پله های برقی و ...)";
            if (p == 28) return "تكنولوژی زیست محیطی و بازیافت";
            if (p == 04) return "حمل و نقل (دریایی، زمینی، ریلی، هوایی)";
            if (p == 10) return "خدمات";
            if (p == 45) return "خدمات مسافرتی";
            if (p == 09) return "خودرو و وسایل نقلیه دوچرخ";
            if (p == 56) return "دكوراسیون داخلی و تجهیزات اداری";
            if (p == 35) return "ریخته گری";
            if (p == 34) return "سفارت ها و مؤسسات خارجی در ایران";
            if (p == 13) return "سندیكاها - تعاونیها و انجمن ها";
            if (p == 39) return "شركت های بازرگانی";
            if (p == 25) return "صنایع برق, الكترونیك و مخابرات";
            if (p == 46) return "صنایع چاپ، بسته بندی، كاغذ";
            if (p == 52) return "صنایع دارویی";
            if (p == 05) return "صنایع سرمایشی و گرمایشی";
            if (p == 14) return "صنایع شیمیائی";
            if (p == 53) return "صنایع معدنی";
            if (p == 36) return "صنعت بیمه";
            if (p == 17) return "صنعت پوشاك، كیف و كفش، چرم";
            if (p == 37) return "صنعت شیشه";
            if (p == 12) return "صنعت فرش و صنایع دستی";
            if (p == 18) return "صنعت فن آوری ارتباطات و اطلاعات IT,ICT (كامپیوتر)";
            if (p == 40) return "صنعت لاستیك و پلاستیك";
            if (p == 15) return "صنعت ورزش ";
            if (p == 48) return "طراحی و ساخت ماشین آلات";
            if (p == 21) return "عایق ها";
            if (p == 57) return "عدسی ها - مكانیسمهای دقیق، ترازوهای دقیق و باسكول ها";
            if (p == 55) return "فلزات غیر آهنی و فرآورده های فلزات غیر آهنی";
            if (p == 33) return "فولاد و آهن، فرآورده های فولادی و آهنی";
            if (p == 51) return "كالاها و قطعات صنعتی";
            if (p == 03) return "كشاورزی، دامپروری";
            if (p == 43) return "لوازم آشپزخانه";
            if (p == 16) return "لوازم التحریر و طراحی";
            if (p == 22) return "لوازم خانگی";
            if (p == 26) return "لوله و اتصالات";
            if (p == 30) return "مؤسسات مالی";
            if (p == 27) return "محصولات و فرآورده های چوبی";
            if (p == 29) return "مراكز تحقیقاتی";
            if (p == 24) return "مراكز آموزشی و كمك آموزشی";
            if (p == 41) return "مشاوره های صنعتی و فنی، مدیریت، گواهینامه های صنعتی و...، برگزاری نمایشگاه، سمینار";
            if (p == 11) return "مصالح ساختمانی، انواع چینی و سرامیك های صنعتی";
            if (p == 19) return "مهندسین مشاور";
            if (p == 32) return "مواد غذایی و آشامیدنی";
            if (p == 07) return "نساجی";
            if (p == 49) return "وسایل و تجهیزات پزشكی و دندانپزشكی";
            return string.Empty;
        }

        #region CoName

        private string _CoName;

        [Size(256)]
        public string CoName
        {
            get { return _CoName; }
            set { SetPropertyValue("CoName", ref _CoName, value); }
        }

        #endregion

        #region Tel

        private string _Tel;

        [Size(SizeAttribute.Unlimited)]
        public string Tel
        {
            get { return _Tel; }
            set { SetPropertyValue("Tel", ref _Tel, value); }
        }

        #endregion

        #region Fax

        private string _Fax;

        [Size(256)]
        public string Fax
        {
            get { return _Fax; }
            set { SetPropertyValue("Fax", ref _Fax, value); }
        }

        #endregion

        #region Email

        private string _Email;

        [Size(256)]
        public string Email
        {
            get { return _Email; }
            set { SetPropertyValue("Email", ref _Email, value); }
        }

        #endregion

        #region WebSite

        private string _WebSite;

        [Size(256)]
        public string WebSite
        {
            get { return _WebSite; }
            set { SetPropertyValue("WebSite", ref _WebSite, value); }
        }

        #endregion

        #region ManagerName

        private string _ManagerName;

        [Size(256)]
        public string ManagerName
        {
            get { return _ManagerName; }
            set { SetPropertyValue("ManagerName", ref _ManagerName, value); }
        }

        #endregion

        #region ActivityOrProduct

        private string _ActivityOrProduct;

        [Size(SizeAttribute.Unlimited)]
        public string ActivityOrProduct
        {
            get { return _ActivityOrProduct; }
            set { SetPropertyValue("ActivityOrProduct", ref _ActivityOrProduct, value); }
        }

        #endregion

        #region Establish

        private string _Establish;

        [Size(256)]
        public string Establish
        {
            get { return _Establish; }
            set { SetPropertyValue("Establish", ref _Establish, value); }
        }

        #endregion

        #region Other

        private string _Other;

        [Size(SizeAttribute.Unlimited)]
        public string Other
        {
            get { return _Other; }
            set { SetPropertyValue("Other", ref _Other, value); }
        }

        #endregion

        #region Group1

        private int _Group1;

        public int Group1
        {
            get { return _Group1; }
            set { SetPropertyValue("Group1", ref _Group1, value); }
        }

        #endregion

        #region ImportID

        private Guid _ImportID;

        public Guid ImportID
        {
            get { return _ImportID; }
            set { SetPropertyValue("ImportID", ref _ImportID, value); }
        }

        #endregion
    }
}
