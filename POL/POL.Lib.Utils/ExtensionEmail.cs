using System.IO;
using System.Net.Mail;
using System.Reflection;

namespace POL.Lib.Utils
{
    public static class ExtensionEmail
    {
        public static void SaveToFile(this MailMessage message, string fileName)
        {
            var assembly = typeof (SmtpClient).Assembly;
            var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                var mailWriterContructor =
                    mailWriterType.GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new[] {typeof (Stream)},
                        null);

                var mailWriter = mailWriterContructor.Invoke(new object[] {fileStream});

                var sendMethod =
                    typeof (MailMessage).GetMethod(
                        "Send",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                sendMethod.Invoke(
                    message,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new[] {mailWriter, true, true},
                    null);

                var closeMethod =
                    mailWriter.GetType().GetMethod(
                        "Close",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                closeMethod.Invoke(
                    mailWriter,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] {},
                    null);
            }
        }

        public static string SaveToString(this MailMessage message)
        {
            var assembly = typeof (SmtpClient).Assembly;
            var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

            using (var fileStream = new MemoryStream())
            {
                var mailWriterContructor =
                    mailWriterType.GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new[] {typeof (Stream)},
                        null);

                var mailWriter = mailWriterContructor.Invoke(new object[] {fileStream});

                var sendMethod =
                    typeof (MailMessage).GetMethod(
                        "Send",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                try
                {
                    sendMethod.Invoke(
                        message,
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new[] {mailWriter, true, true},
                        null);
                }
                catch
                {
                    sendMethod.Invoke(
                        message,
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new[] {mailWriter, true},
                        null);
                }


                fileStream.Position = 0;
                var sr = new StreamReader(fileStream);
                return sr.ReadToEnd();
            }
        }
    }
}
