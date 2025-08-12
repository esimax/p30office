using System;
using System.Collections.Generic;
using System.IO;

namespace POL.Lib.Interfaces.PObjects
{
    [Serializable]
    public class Contact
    {
        public Contact()
        {
            Categories = new List<Category>();
            Phones = new List<Phone>();
            Emails = new List<Email>();
            Profiles = new List<ProfileRoot>();
            Address = new List<Address>();
        }

        public int Code { get; set; }
        public string Title { get; set; }
        public string CCText1 { get; set; }
        public string CCText2 { get; set; }
        public string CCText3 { get; set; }
        public string CCText4 { get; set; }
        public string CCText5 { get; set; }
        public string CCText6 { get; set; }
        public string CCText7 { get; set; }
        public string CCText8 { get; set; }
        public string CCText9 { get; set; }
        public string CCText0 { get; set; }

        public List<Category> Categories { get; set; }
        public List<Phone> Phones { get; set; }
        public List<Address> Address { get; set; }
        public List<Email> Emails { get; set; }
        public List<ProfileRoot> Profiles { get; set; }


        private void PopulateFromTemplate(string src, string dest)
        {
            var stext = File.ReadAllText(src);
            stext = stext.Replace("[Code]", Code.ToString())
                .Replace("[Title]", Title)
                .Replace("[CCText1]", CCText1)
                .Replace("[CCText2]", CCText2)
                .Replace("[CCText3]", CCText3)
                .Replace("[CCText4]", CCText4)
                .Replace("[CCText5]", CCText5)
                .Replace("[CCText6]", CCText6)
                .Replace("[CCText7]", CCText7)
                .Replace("[CCText8]", CCText8)
                .Replace("[CCText9]", CCText9)
                .Replace("[CCText0]", CCText0);
            File.WriteAllText(dest, stext);
        }
    }
}
