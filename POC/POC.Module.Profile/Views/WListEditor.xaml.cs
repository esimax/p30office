using System;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using POC.Module.Profile.Models;
using POL.DB.P30Office;
using System.Reflection;


namespace POC.Module.Profile.Views
{
    public partial class WListEditor : DXWindow
    {
        private MListEditor Model { get; set; }

        public WListEditor(DBCTContact contact, object selectedData, Type dataType, Assembly assembly, DBCTList list)
        {
            InitializeComponent();
            DynamicDBListAssembly = assembly;
            DynamicDBListType = dataType;
            DynamicList = list;
            DynamicSelectedData = selectedData;
            DynamicDBContact = contact;

            lblContact.Content = contact.Title;

            Loaded += (s, e) =>
                          {
                              Model = new MListEditor(this);
                              DataContext = Model;
                              POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();
                          };
        }

        public Assembly DynamicDBListAssembly { get; set; }
        public Type DynamicDBListType { get; set; }
        public DBCTList DynamicList { get; set; }
        public object DynamicSelectedData { get; set; }
        public DBCTContact DynamicDBContact { get; set; }

        public Grid DynamicGrid { get { return grid; } }
        public Window DynamicOwner { get { return this.Owner; } }
    }
}
