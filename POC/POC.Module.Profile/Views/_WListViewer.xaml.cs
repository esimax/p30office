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
using System.Windows.Shapes;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon;
using Microsoft.Practices.Prism.Logging;
using POC.Module.Profile.Models;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using Microsoft.Practices.ServiceLocation;
using DevExpress.Xpf.Grid;
using System.Reflection;
using POL.WPF.DXControls;
using POL.WPF.Controles.AttachProperties;


namespace POC.Module.Profile.Views
{
    public partial class WListViewer : DXRibbonWindow
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }













        public WListViewer(DBCTProfileValue profileValue, bool canEdit)
        {
            DynamicDBProfileValue = profileValue;




            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
                ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

                if (DynamicDBProfileValue == null) return;
                if (DynamicDBProfileValue.ProfileItem == null) return;
                if (DynamicDBProfileValue.ProfileItem.ItemType != EnumProfileItemType.List) return;

                DynamicDBContact = DynamicDBProfileValue.Contact;
                DynamicDBList = DBCTList.FindByOid(ADatabase.Dxs, DynamicDBProfileValue.ProfileItem.Guid1);
                if (DynamicDBList == null) return;

            }

            InitializeComponent();

            try
            {

                Loaded += (s, e)
                          =>
                              {












                                  var model = new MListViewer(this);
                                  model.CanEdit = canEdit;
                                  DataContext = model;
                              };
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message);
            }
        }












        public DBCTList DynamicDBList { get; set; }
        public DBCTProfileValue DynamicDBProfileValue { get; set; }
        public Window DynamicOwner
        {
            get { return this; }
        }
        public GridControl DynamicGridControl
        {
            get { return gcList; }
        }
        public TableView DynamicTableView
        {
            get { return tvList; }
        }
        public Assembly DynamicDBListAssembly { get; set; }
        public DBCTContact DynamicDBContact { get; set; }

    }
}
