using System;

namespace POC.Module.Email.Models
{
    [Serializable]
    public class EmailTemplateIO
    {
        public string Titles { get; set; }
        public string HTMLBody { get; set; }
        public EmailParameterIO[] Parameters { get; set; }
    }
}
