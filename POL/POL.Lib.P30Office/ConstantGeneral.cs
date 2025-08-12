namespace POL.Lib.XOffice
{
    public class ConstantGeneral
    {
        public const string XOfficeTilyLockFileName = @"C:\XOfficeTinyLock.txt";
        public const string ModuleFolderForPos = "ModulePOS";
        public const string ModuleFolderForPox = "ModulePOX";
        public const string ModuleFolderForPoc = "ModulePOC";


#if P30Office
        public const string UpdateTag = "";
        public const string AppName = "P30Office";
        public const string RegKeyRoot = "P30Office3";
        public const byte EncKey1 = 23;
        public const byte EncKey2 = 00;
        public const string ApplicationTitle = "پی سی آفیس";
        public const string SupportEmail = "info@p30office.com";
        public const string WebUrl = "www.p30office.com";
        public const string WebUrlForReqUpdate = "www.p30office.com";
        public const string AdminEmail = "esimax.uk@gmail.com";
        public const string FullPhoneNumber = "";
            
        public const string PathMapCache = "C:\\P30Office\\MapCach";
        public const string PathPopQueue = "C:\\P30Office\\PopQueue";
        public const string PathFileTemp = "C:\\P30Office\\FileTemp";

        public const string LoremIpsumParagraph =
           "این یك متن آزمایشی می باشد : <br/>لورم ایپسوم متنی است كه ساختگی برای طراحی و چاپ آن مورد است. صنعت چاپ زمانی لازم بود شرایطی شما باید فكر ثبت نام و طراحی، لازمه خروج می باشد. در ضمن قاعده همفكری ها جوابگوی سئوالات زیاد شاید باشد، آنچنان كه لازم بود طراحی گرافیكی خوب بود. كتابهای زیادی شرایط سخت ، دشوار و كمی در سالهای دور لازم است. هدف از این نسخه فرهنگ پس از آن و دستاوردهای خوب شاید باشد. حروفچینی لازم در شرایط فعلی لازمه تكنولوژی بود كه گذشته، حال و آینده را شامل گردد. سی و پنج درصد از طراحان در قرن پانزدهم میبایست پرینتر در ستون و سطر حروف لازم است، بلكه شناخت این ابزار گاه اساسا بدون هدف بود و سئوالهای زیادی در گذشته بوجود می آید، تنها لازمه آن بود.";


        public const string EmailHeaderDefault = @"<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html>

<head>
<meta content='text/html;UTF-8' http-equiv='Content-Type' />
<meta http-equiv='X-UA-Compatible' content='IE=9' />
</head>

<body style='margin: 0px; font-family: Tahoma, Arial, sans-serif; font-size: 12px; direction: rtl; color: #444444'>
<div style='margin-left: auto; margin-right: auto; width: 500px; border: 1px solid #d9d9d9;'>
	<div style='border-style: none none solid none; border-width: 1px; height: 29px; background-color: #00263b; border-top-color: #001a29; border-right-color: 001a29; border-bottom-color: 001a29; border-left-color: 001a29;'>
	</div>
	<div style='border-style: solid none none none; border-width: 1px; border-color: #2e6687; height: 68px; background-color: #004169'>
		<div style='height: 16px;'>
		</div>
		<div style='height: 32px;'>
			<center>
			<img alt='پی سی آفیس' height='32px' src='[IMG01]' width='80px' />
			</center></div>
	</div>
	<div style='background-color: #eeeeee'>
		<table border='0' cellpadding='0' cellspacing='0' dir='rtl' style='color: #444; line-height: 1.6; font-size: 12px; font-family: Tahoma, sans-serif;' width='100%'>
			<tr>
				<td>
				<div style='padding: 15px 0; float: right; padding-left: 30px; padding-right: 30px;font-family:Tahoma;'>
";

        public const string EmailFooterDefault = @"			</div>
				</td>
			</tr>
		</table>
	</div>
	<div style='border-style: solid none none none; border-width: 1px; border-color: #d9d9d9; background-color: #eeeeee; padding: 3px;font-family:Tahoma;'>		
        ارسال شده توسط نرم افزار 
		<a href='http://www.p30office.com' style='text-decoration:none'> پی سی آفیس </a>
	</div>
</div>

</body>

</html>
";

        public const string EmailImage01Default = "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";
#endif


        public const int MaximumPhoneLineSupported = 64;

        public const int ProtocolCorePortOffset = 0;
        public const string ProtocolCoreServiceName = "Core";

        public const int ProtocolPhoneMonitoringPortOffset = 1;
        public const string ProtocolPhoneMonitoringServiceName = "PhoneMonitoring";

        public const int ProtocolMembershipPortOffset = 0;
        public const string ProtocolMembershipServiceName = "Membership";

        public const int ProtocolMessagingPortOffset = 1;
        public const string ProtocolMessagingServiceName = "Messaging";

        public const int ProtocolVoiceRecordPortOffset = 1;
        public const string ProtocolVoiceRecordServiceName = "VoiceRecord";


        public const string EmailInboxTitle = "دریافتی";
        public const string EmailSentTitle = "ارسالی";
        public const string EmailTrashTitle = "حذف شده";
        public const string EmailDraftsTitle = "پیش نویس";
        public const string EmailWaitForSendTitle = "جهت ارسال";

        public const string MissCallTitle = "بی پاسخ";
    }
}
