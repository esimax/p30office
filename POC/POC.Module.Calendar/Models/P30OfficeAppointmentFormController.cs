using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.UI;
using DevExpress.XtraScheduler;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.DB.P30Office;
using POL.WPF.Controles.MVVM;
using System.Windows;

namespace POC.Module.Calendar.Models
{
    public class P30OfficeAppointmentFormController : AppointmentFormController
    {
        public P30OfficeAppointmentFormController(SchedulerControl control, Appointment apt)
            : base(control, apt)
        {
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();

            MySchedulerControl = control;


            ViewList = new List<PermissionHolder>();
            EditList = new List<PermissionHolder>();
            DeleteList = new List<PermissionHolder>();

            if (!IsNewAppointment)
            {
                if (ViewPermissionType == 1) ViewList = GetUserList();
                if (ViewPermissionType == 2) ViewList = GetRoleList();

                if (EditPermissionType == 1) EditList = GetUserList();
                if (EditPermissionType == 2) EditList = GetRoleList();

                if (DeletePermissionType == 1) DeleteList = GetUserList();
                if (DeletePermissionType == 2) DeleteList = GetRoleList();

                if (CategoryOid != Guid.Empty)
                {
                    RelatedCategory = DBCTContactCat.FindByOid(ADatabase.Dxs, CategoryOid);
                }
                if (ContactOid != Guid.Empty)
                {
                    RelatedContact = DBCTContact.FindByOid(ADatabase.Dxs, ContactOid);
                }
            }
            else
            {
                ViewPermissionType = 0;
                ViewOid = Guid.Empty;

                EditPermissionType = 0;
                EditOid = Guid.Empty;

                DeletePermissionType = 0;
                DeleteOid = Guid.Empty;

                UserCreated = AMembership.ActiveUser.UserName;
            }
            HasReminder = true;
            InitCommands();
        }



        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IDatabase ADatabase { get; set; }

        private SchedulerControl MySchedulerControl { get; set; }

        private List<PermissionHolder> UserList { get; set; }
        private List<PermissionHolder> RoleList { get; set; }

        private List<PermissionHolder> GetUserList()
        {
            if (UserList != null) return UserList;
            var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            var xpc = POL.DB.Membership.DBMSUser2.UserGetAll(aDatabase.Dxs, null);
            UserList = xpc.Select(db => new PermissionHolder { Oid = db.Oid, Title = db.Title }).ToList();
            return UserList;
        }
        private List<PermissionHolder> GetRoleList()
        {
            if (RoleList != null) return RoleList;
            var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            var xpc = POL.DB.Membership.DBMSRole2.RoleGetAll(aDatabase.Dxs, null);
            RoleList = xpc.Select(db => new PermissionHolder { Oid = db.Oid, Title = db.Title }).ToList();
            return RoleList;
        }


        #region ViewPermissionType
        public int ViewPermissionType
        {
            get { return GetViewPermissionTypeValue(EditedAppointmentCopy); }
            set
            {
                EditedAppointmentCopy.CustomFields["ViewPermissionType"] = value;
                NotifyPropertyChanged("CanSelectView");
                if (value == 1)
                    ViewList = GetUserList();
                if (value == 2)
                    ViewList = GetRoleList();
            }
        }
        private int SourceViewPermissionType
        {
            get { return GetViewPermissionTypeValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["ViewPermissionType"] = value; }
        }
        protected int GetViewPermissionTypeValue(Appointment apt)
        {
            return Convert.ToInt32(apt.CustomFields["ViewPermissionType"]);
        }
        #endregion
        #region ViewOid
        public Guid ViewOid
        {
            get { return GetViewOidValue(EditedAppointmentCopy); }
            set { EditedAppointmentCopy.CustomFields["ViewOid"] = value; }
        }
        private Guid SourceViewOid
        {
            get { return GetViewOidValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["ViewOid"] = value; }
        }
        protected Guid GetViewOidValue(Appointment apt)
        {
            try
            {
                return Guid.Parse(Convert.ToString(apt.CustomFields["ViewOid"]));
            }
            catch
            {
                return Guid.Empty;
            }
        }
        #endregion
        #region CanSelectView
        public bool CanSelectView
        {
            get { return (ViewPermissionType == 1 || ViewPermissionType == 2); }
        }
        #endregion
        #region ViewList
        private List<PermissionHolder> _ViewList;
        public List<PermissionHolder> ViewList
        {
            get { return _ViewList; }
            set
            {
                if (value == _ViewList)
                    return;

                _ViewList = value;
                NotifyPropertyChanged("ViewList");
            }
        }
        #endregion
        #region SelectedView
        public PermissionHolder SelectedView
        {
            get { return (from n in ViewList where n.Oid == ViewOid select n).FirstOrDefault(); }
            set
            {
                if (value == null)
                    return;
                ViewOid = value.Oid;
                NotifyPropertyChanged("SelectedView");
            }
        }
        #endregion


        #region EditPermissionType
        public int EditPermissionType
        {
            get { return GetEditPermissionTypeValue(EditedAppointmentCopy); }
            set
            {
                EditedAppointmentCopy.CustomFields["EditPermissionType"] = value;
                NotifyPropertyChanged("CanSelectEdit");
                if (value == 1)
                    EditList = GetUserList();
                if (value == 2)
                    EditList = GetRoleList();
            }
        }
        private int SourceEditPermissionType
        {
            get { return GetEditPermissionTypeValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["EditPermissionType"] = value; }
        }
        protected int GetEditPermissionTypeValue(Appointment apt)
        {
            return Convert.ToInt32(apt.CustomFields["EditPermissionType"]);
        }
        #endregion
        #region EditOid
        public Guid EditOid
        {
            get { return GetEditOidValue(EditedAppointmentCopy); }
            set { EditedAppointmentCopy.CustomFields["EditOid"] = value; }
        }
        private Guid SourceEditOid
        {
            get { return GetEditOidValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["EditOid"] = value; }
        }
        protected Guid GetEditOidValue(Appointment apt)
        {
            try
            {
                return Guid.Parse(Convert.ToString(apt.CustomFields["EditOid"]));
            }
            catch
            {
                return Guid.Empty;
            }
        }
        #endregion
        #region CanSelectEdit
        public bool CanSelectEdit
        {
            get { return (EditPermissionType == 1 || EditPermissionType == 2); }
        }
        #endregion
        #region EditList
        private List<PermissionHolder> _EditList;
        public List<PermissionHolder> EditList
        {
            get { return _EditList; }
            set
            {
                if (value == _EditList)
                    return;

                _EditList = value;
                NotifyPropertyChanged("EditList");
            }
        }
        #endregion
        #region SelectedEdit
        public PermissionHolder SelectedEdit
        {
            get { return (from n in EditList where n.Oid == EditOid select n).FirstOrDefault(); }
            set
            {
                if (value == null)
                    return;
                EditOid = value.Oid;
                NotifyPropertyChanged("SelectedEdit");
            }
        }
        #endregion


        #region DeletePermissionType
        public int DeletePermissionType
        {
            get { return GetDeletePermissionTypeValue(EditedAppointmentCopy); }
            set
            {
                EditedAppointmentCopy.CustomFields["DeletePermissionType"] = value;
                NotifyPropertyChanged("CanSelectDelete");
                if (value == 1)
                    DeleteList = GetUserList();
                if (value == 2)
                    DeleteList = GetRoleList();
            }
        }
        private int SourceDeletePermissionType
        {
            get { return GetDeletePermissionTypeValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["DeletePermissionType"] = value; }
        }
        protected int GetDeletePermissionTypeValue(Appointment apt)
        {
            return Convert.ToInt32(apt.CustomFields["DeletePermissionType"]);
        }
        #endregion
        #region DeleteOid
        public Guid DeleteOid
        {
            get { return GetDeleteOidValue(EditedAppointmentCopy); }
            set { EditedAppointmentCopy.CustomFields["DeleteOid"] = value; }
        }
        private Guid SourceDeleteOid
        {
            get { return GetDeleteOidValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["DeleteOid"] = value; }
        }
        protected Guid GetDeleteOidValue(Appointment apt)
        {
            try
            {
                return Guid.Parse(Convert.ToString(apt.CustomFields["DeleteOid"]));
            }
            catch
            {
                return Guid.Empty;
            }
        }
        #endregion
        #region CanSelectDelete
        public bool CanSelectDelete
        {
            get { return (DeletePermissionType == 1 || DeletePermissionType == 2); }
        }
        #endregion
        #region DeleteList
        private List<PermissionHolder> _DeleteList;
        public List<PermissionHolder> DeleteList
        {
            get { return _DeleteList; }
            set
            {
                if (value == _DeleteList)
                    return;

                _DeleteList = value;
                NotifyPropertyChanged("DeleteList");
            }
        }
        #endregion
        #region SelectedDelete
        public PermissionHolder SelectedDelete
        {
            get { return (from n in DeleteList where n.Oid == DeleteOid select n).FirstOrDefault(); }
            set
            {
                if (value == null)
                    return;
                DeleteOid = value.Oid;
                NotifyPropertyChanged("SelectedDelete");
            }
        }
        #endregion

        #region UserCreated
        public string UserCreated
        {
            get { return GetUserCreatedValue(EditedAppointmentCopy); }
            set
            {
                EditedAppointmentCopy.CustomFields["UserCreated"] = value;
            }
        }
        private string SourceUserCreated
        {
            get { return GetUserCreatedValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["UserCreated"] = value; }
        }
        protected string GetUserCreatedValue(Appointment apt)
        {
            return Convert.ToString(apt.CustomFields["UserCreated"]);
        }
        #endregion



        #region CategoryOid
        public Guid CategoryOid
        {
            get { return GetCategoryOidValue(EditedAppointmentCopy); }
            set
            {
                EditedAppointmentCopy.CustomFields["CategoryOid"] = value;
            }
        }
        private Guid SourceCategoryOid
        {
            get { return GetCategoryOidValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["CategoryOid"] = value; }
        }
        protected Guid GetCategoryOidValue(Appointment apt)
        {
            var g = Guid.Empty;
            Guid.TryParse(Convert.ToString(apt.CustomFields["CategoryOid"]), out g);
            return g;
        }
        #endregion
        #region RelatedCategory
        private DBCTContactCat _RelatedCategory;
        public DBCTContactCat RelatedCategory
        {
            get { return _RelatedCategory; }
            set
            {
                if (value == _RelatedCategory)
                    return;

                _RelatedCategory = value;
                NotifyPropertyChanged("RelatedCategory");
            }
        }
        #endregion

        #region ContactOid
        public Guid ContactOid
        {
            get { return GetContactOidValue(EditedAppointmentCopy); }
            set
            {
                EditedAppointmentCopy.CustomFields["ContactOid"] = value;
            }
        }
        private Guid SourceContactOid
        {
            get { return GetContactOidValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["ContactOid"] = value; }
        }
        protected Guid GetContactOidValue(Appointment apt)
        {
            var g = Guid.Empty;
            Guid.TryParse(Convert.ToString(apt.CustomFields["ContactOid"]), out g);
            return g;
        }
        #endregion
        #region RelatedContact
        private DBCTContact _RelatedContact;
        public DBCTContact RelatedContact
        {
            get { return _RelatedContact; }
            set
            {
                if (value == _RelatedContact)
                    return;

                _RelatedContact = value;
                NotifyPropertyChanged("RelatedContact");
            }
        }
        #endregion

        #region ProfileItemOid
        public Guid ProfileItemOid
        {
            get { return GetProfileItemOidValue(EditedAppointmentCopy); }
            set
            {
                EditedAppointmentCopy.CustomFields["ProfileItemOid"] = value;
            }
        }
        private Guid SourceProfileItemOid
        {
            get { return GetProfileItemOidValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["ProfileItemOid"] = value; }
        }
        protected Guid GetProfileItemOidValue(Appointment apt)
        {
            var g = Guid.Empty;
            Guid.TryParse(Convert.ToString(apt.CustomFields["ProfileItemOid"]), out g);
            return g;
        }
        #endregion




        public bool CanEditPermissions
        {
            get { return (AMembership.ActiveUser.UserName == UserCreated && !SourceAppointment.IsRecurring) || IsNewAppointment; }
        }
        public override bool IsAppointmentChanged()
        {
            if (base.IsAppointmentChanged())
                return true;
            return (ViewPermissionType != SourceViewPermissionType) ||
                    (ViewOid != SourceViewOid) ||
                    (EditPermissionType != SourceEditPermissionType) ||
                    (EditOid != SourceEditOid) ||
                    (DeletePermissionType != SourceDeletePermissionType) ||
                    (DeleteOid != SourceDeleteOid)
                    ;
        }
        public bool CanEdit
        {
            get
            {
                return IsNewAppointment || CanEditPermissions ||
                       SourceEditPermissionType == 3 ||
                       (SourceEditPermissionType == 1 && SourceEditOid == AMembership.ActiveUser.UserID) ||
                       (SourceEditPermissionType == 2 && AMembership.ActiveUser.RolesOid != null && AMembership.ActiveUser.RolesOid.Contains(SourceEditOid));
            }
        }
        public bool IsReadOnly
        {
            get { return !CanEdit; }
        }
        public bool IsOwner
        {
            get { return UserCreated == AMembership.ActiveUser.UserName; }
        }
        public string CreatorUserString
        {
            get
            {
                return string.Format("ایجاد كننده : {0}", !string.IsNullOrEmpty(UserCreated) ? UserCreated : AMembership.ActiveUser.Title);
            }
        }



        #region [COMMANDS]
        public RelayCommand CommandSelectCategory { get; set; }
        public RelayCommand CommandClearCategory { get; set; }
        public RelayCommand CommandSelectContact { get; set; }
        public RelayCommand CommandClearContact { get; set; }
        #endregion



        private void InitCommands()
        {
            CommandSelectCategory = new RelayCommand(SelectCategory, () => CanEdit);
            CommandClearCategory = new RelayCommand(ClearCategory, () => CanEdit);

            CommandSelectContact = new RelayCommand(SelectContact, () => CanEdit);
            CommandClearContact = new RelayCommand(ClearContact, () => CanEdit);
        }

        private void ClearCategory()
        {
            RelatedCategory = null;
            CategoryOid = Guid.Empty;

            ClearContact();
        }
        private void SelectCategory()
        {
            var selectedCategory = APOCMainWindow.ShowSelectContactCat(Window.GetWindow(MySchedulerControl));
            if (selectedCategory != null)
            {
                RelatedCategory = selectedCategory as DBCTContactCat;
                if (RelatedCategory != null)
                    CategoryOid = RelatedCategory.Oid;
            }
        }

        private void ClearContact()
        {
            RelatedContact = null;
            ContactOid = Guid.Empty;
        }
        private void SelectContact()
        {
            if (RelatedCategory == null) return;
            var selectedContact = APOCMainWindow.ShowSelectContact(Window.GetWindow(MySchedulerControl), RelatedCategory);
            if (selectedContact != null)
            {
                RelatedContact = selectedContact as DBCTContact;
                if (RelatedContact != null)
                    ContactOid = RelatedContact.Oid;
            }
        }
    }
}
