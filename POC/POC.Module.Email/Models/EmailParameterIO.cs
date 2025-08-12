using System;
using POL.Lib.Interfaces;

namespace POC.Module.Email.Models
{
    [Serializable]
    public class EmailParameterIO
    {
        public string Tag { get; set; }
        public EnumEmailTemplateTagType TagType { get; set; }

        public string ProfileItemTitle { get; set; }
        public string ProfileGroupTitle { get; set; }
        public string ProfileRootTitle { get; set; }
    }
}
