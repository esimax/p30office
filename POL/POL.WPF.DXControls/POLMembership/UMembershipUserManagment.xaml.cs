using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using POL.Lib.Interfaces;

namespace POL.WPF.DXControls.POLMembership
{
    public partial class UMembershipUserManagment : UserControl
    {
        public UMembershipUserManagment()
        {
            InitializeComponent();
        }





        private static List<PermissionPack> PPList { get; set; }
        public static Func<Type> GetPermissionsEnum { get; set; }
        public static int GetPermissionMax()
        {
            if (PPList == null)
                PopulatePermissionList();
            return PPList == null ? 0 : (from n in PPList select n.Position).Max();
        }
        private static void PopulatePermissionList()
        {
            if (GetPermissionsEnum == null) return;
            if (PPList == null)
                PPList = new List<PermissionPack>();
            else
                return;

            var eType = GetPermissionsEnum.Invoke();
            var eList = Enum.GetNames(eType);

            eList.ToList().ForEach(
                e =>
                {
                    var memInfo = eType.GetMember(e);
                    var attBrowsable = memInfo[0].GetCustomAttributes(typeof(BrowsableAttribute), false);
                    var isenabled = ((BrowsableAttribute)attBrowsable[0]).Browsable;
                    if (!isenabled) return;

                    var attCategory = memInfo[0].GetCustomAttributes(typeof(CategoryAttribute), false);
                    var attDescription = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    var pp = new PermissionPack
                    {
                        Name = e,
                        Position = (int)Enum.Parse(eType, e),
                        Category = ((CategoryAttribute)attCategory[0]).Category,
                        Description = ((DescriptionAttribute)attDescription[0]).Description
                    };

                    PPList.Add(pp);
                });
        }
    }
}
