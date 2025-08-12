using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using Microsoft.Practices.Prism.Logging;
using POC.Module.Link.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;

namespace POC.Module.Link.Models
{

    public class MLinkContactModule : NotifyObjectBase, IDisposable, IRefrashable
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule AContactModule { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }

        private DBCTContact CurrentContact { get; set; }

        #region CTOR
        public MLinkContactModule(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            AContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();

            AContactModule.OnSelectedContactChanged += AContactModule_OnSelectedContactChanged;


            InitCommands();
            GetDynamicData();
            DataRefresh();
        }
        #endregion

        void AContactModule_OnSelectedContactChanged(object sender, EventArgs e)
        {
            if (POCContactModuleItem.LasteSelectedVmType == null || POCContactModuleItem.LasteSelectedVmType != GetType())
            {
                RequiresRefresh = true;
                return;
            }
            if (ReferenceEquals(CurrentContact, (AContactModule.SelectedContact as DBCTContact)))
                return;
            DoRefresh();

        }

        #region RootEnable
        public bool RootEnable { get { return AContactModule.SelectedContact != null; } }
        #endregion


        public class LinkTemplateClass
        {
            public Int64 Code { get; set; }
            public string Title { get; set; }
            public string LinkText { get; set; }

            public DBCTContactRelation DBLink { get; set; }
        }


        #region LinkList
        private List<LinkTemplateClass> _LinkList;
        public List<LinkTemplateClass> LinkList
        {
            get { return _LinkList; }
            set
            {
                _LinkList = value;
                RaisePropertyChanged("LinkList");
            }
        }
        #endregion
        #region FocusedLink
        private LinkTemplateClass _FocusedLink;
        public LinkTemplateClass FocusedLink
        {
            get
            {
                return _FocusedLink;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedLink)) return;
                _FocusedLink = value;
                RaisePropertyChanged("FocusedLink");
            }
        }
        #endregion FocusedLink


        #region [METHODS]
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicGridControl = MainView.DynamicGridControl;
            DynamicTableView = MainView.DynamicTableView;

            DynamicGridControl.MouseDoubleClick +=
                (s, e) =>
                {
                    var i = DynamicTableView.GetRowHandleByMouseEventArgs(e);
                    if (i < 0) return;
                    if (CommandEdit.CanExecute(null))
                        CommandEdit.Execute(null);
                    e.Handled = true;
                };
        }
        private void PopulateLinkList()
        {
            CurrentContact = AContactModule.SelectedContact as DBCTContact;
            if (CurrentContact == null)
            {
                LinkList = null;
                return;
            }

            var list = DBCTContactRelation.GetByContact(ADatabase.Dxs, CurrentContact.Oid);
            _LinkList = new List<LinkTemplateClass>();
            foreach (var db in list)
            {
                if (db.Contact1 == null || db.Contact2 == null) continue;

                var dbcOther = db.Contact1.Oid == CurrentContact.Oid ? db.Contact2 : db.Contact1;
                _LinkList.Add(new LinkTemplateClass
                                 {
                                     Code = dbcOther.Code,
                                     Title = dbcOther.Title,
                                     LinkText = db.Contact1.Oid == CurrentContact.Oid ? string.Format("{0} , {1}", db.TitleMain2, db.TitleSub2) : string.Format("{0} , {1}", db.TitleMain1, db.TitleSub1),
                                     DBLink = db,
                                 });
            }
            RaisePropertyChanged("LinkList");
        }
        private void InitCommands()
        {
            CommandNew = new RelayCommand(LinkNew, () => AContactModule.SelectedContact != null && AMembership.HasPermission(PCOPermissions.Contact_Link_Add));
            CommandEdit = new RelayCommand(LinkEdit, () => FocusedLink != null && DynamicGridControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.Contact_Link_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => DynamicGridControl.SelectedItems.Count >= 1 && AMembership.HasPermission(PCOPermissions.Contact_Link_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);
            CommandGotoContact = new RelayCommand(GotoContact, () => FocusedLink != null);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp45 != "");
        }

        private void LinkNew()
        {
            var w = new WLinkAddEdit(AContactModule.SelectedContact as DBCTContact, null)
                        {
                            Owner = DynamicOwner
                        };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void LinkEdit()
        {

            var w = new WLinkAddEdit(AContactModule.SelectedContact as DBCTContact, FocusedLink.DBLink) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("ارتباط (های) انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => ((LinkTemplateClass)DynamicGridControl.GetRow(n)).DBLink).ToList();

            var failed = 0;
            var success = 0;
            POLProgressBox.Show("حذف اطلاعات", true, 0, srh.Count(), 1,
                w =>
                {
                    var dxs = ADatabase.GetNewSession();
                    foreach (var db in list)
                    {
                        try
                        {
                            if (w.NeedToCancel) return;
                            var db2 = DBCTContactRelation.FindByOid(dxs, db.Oid);
                            w.AsyncSetText(1, string.Format("{0} <-> {1}", db2.Contact1, db2.Contact2));
                            db2.Delete();
                            db2.Save();
                            success++;
                        }
                        catch
                        {
                            failed++;
                        }
                    }
                }, null, DynamicOwner);
            POLMessageBox.ShowInformation(string.Format("گزارش حذف : {0}{0}موفقیت آمیز : {1}{0}بروز خطا : {2}", Environment.NewLine, success, failed), DynamicOwner);
            DataRefresh();
        }
        private void DataRefresh()
        {

            PopulateLinkList();
        }
        private void GotoContact()
        {
            AContactModule.GotoContactByCode((int)FocusedLink.Code);
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp45);
        }
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandNewMulti { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandGotoContact { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            AContactModule.OnSelectedContactChanged -= AContactModule_OnSelectedContactChanged;
            LinkList = null;
        }
        #endregion

        #region IRefrashable
        public void DoRefresh()
        {
            DataRefresh();
            RaisePropertyChanged("RootEnable");
        }

        public bool RequiresRefresh { get; set; }
        #endregion

    }
}
