using System;
using System.Linq;
using Limilabs.Mail;
using Limilabs.Mail.MIME;


namespace POL.Lib.Utils
{
    public class HelperEmail
    {

        public static string ExtractFromName(string from)
        {
            try
            {
                var i1 = @from.IndexOf("\"", StringComparison.Ordinal) + 1;
                var i2 = @from.LastIndexOf("\"", StringComparison.Ordinal);
                return from.Substring(i1, i2 - i1);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string ExtractFromAddress(string from)
        {
            try
            {
                var i1 = @from.IndexOf("<", StringComparison.Ordinal) + 1;
                var i2 = @from.LastIndexOf(">", StringComparison.Ordinal);
                return from.Substring(i1, i2 - i1);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string RenderBodyCache(string body)
        {
            int j;
            var v = body;
            add_tag_head:
            var i = v.IndexOf("<head", StringComparison.InvariantCultureIgnoreCase);
            if (i <= -1)
            {
                add_tag_html:
                i = v.IndexOf("<html", StringComparison.InvariantCultureIgnoreCase);
                if (i <= -1)
                {
                    v = v.Insert(0, "<html>");
                    v = v.Insert(v.Length - 1, "</html>");
                    goto add_tag_html;
                }
                j = v.IndexOf(">", i, StringComparison.Ordinal);
                v = v.Insert(j + 1, "<head></head>");
                goto add_tag_head;
            }
            j = v.IndexOf(">", i, StringComparison.Ordinal);
            if (i > 0 && j > 0)
            {
                v = v.Insert(j + 1, "<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>");
            }
            return v;
        }



        public static string RenderInlines(IMimeDataReadOnlyCollection atts, string body)
        {
            var inlines = atts.Where(n => !n.HasFileName); 
            var mimeDatas = inlines as MimeData[] ?? inlines.ToArray();
            if (mimeDatas.Any())
            {
                foreach (var att in mimeDatas)
                {
                    var id = string.Format("cid:{0}", att.ContentId);
                    var location = body.IndexOf(id, StringComparison.Ordinal);
                    if (location <= 0) continue;
                    var rep = string.Format("data:{0};base64,{1}", att.ContentType, Convert.ToBase64String(att.Data));
                    body = body.Replace(id, rep);
                }
            }
            return body;
        }
    }
}
