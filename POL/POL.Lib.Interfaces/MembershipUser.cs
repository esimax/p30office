using System;
using System.Linq;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class MembershipUser
    {
        [DataMember]
        public Guid UserID { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string InternalPhone { get; set; }

        [DataMember]
        public string MobilePhone { get; set; }

        [DataMember]
        public DateTime LoginDate { get; set; }

        [DataMember]
        public DateTime RegisterDate { get; set; }

        [DataMember]
        public int CountLogin { get; set; }

        [DataMember]
        public int CountAdd { get; set; }

        [DataMember]
        public int CountEdit { get; set; }

        [DataMember]
        public int CountDelete { get; set; }

        [DataMember]
        public int CountView { get; set; }


        [DataMember]
        public int[] Permissions { get; set; }

        [DataMember]
        public string[] Roles { get; set; }

        [DataMember]
        public Guid[] RolesOid { get; set; }

        public bool HasPermission(int code)
        {
            if (string.IsNullOrEmpty(UserName)) return false;
            if (UserName.ToLower() == "admin") return true;
            return code == 0 || Permissions.Contains(code);
        }
    }
}
