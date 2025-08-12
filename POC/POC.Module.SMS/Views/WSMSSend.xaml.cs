using System.Collections.Generic;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using POL.Lib.Interfaces;
using POL.DB.P30Office;
using POC.Module.SMS.Models;


namespace POC.Module.SMS.Views
{
    public partial class WSMSSend : DXWindow
    {
        public WSMSSend(EnumSelectionType selectionType,
            DBCTContact selectedContact,
            List<DBCTContact> selectedContactList,
            DBCTContactCat selectedCategory,
            DBCTContactSelection selectedBasket,
            List<string> customeList, string text)
        {
            InitializeComponent();

            DynamicSelectionType = selectionType;
            DynamicSelectedContact = selectedContact;
            DynamicSelectedContactList = selectedContactList;
            DynamicSelectedCategory = selectedCategory;
            DynamicSelectedBasket = selectedBasket;
            DynamicCustomeList = customeList;
            DynamicText = text;
            Loaded += (s, e) =>
            {
                Model = new MSMSSend(this);
                DataContext = Model;
                POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();
            };


        }

        private MSMSSend Model { get; set; }
        public EnumSelectionType DynamicSelectionType { get; set; }
        public DBCTContact DynamicSelectedContact { get; set; }
        public List<DBCTContact> DynamicSelectedContactList { get; set; }
        public DBCTContactCat DynamicSelectedCategory { get; set; }
        public DBCTContactSelection DynamicSelectedBasket { get; set; }
        public List<string> DynamicCustomeList { get; set; }
        public string DynamicText { get; set; }

        private void ListBoxEdit_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                Model.SetTextEntered(false);
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                Model.SetTextEntered(true);
            else
            {
                bTextStaticEdit.Command.Execute(null);
            }

        }

    }
}
