using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Membership;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using System.Threading.Tasks;

namespace POC.Module.Contact.Models
{
    public class MContactAddEdit : NotifyObjectBase, IRequestCloseViewModel
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private Window Owner { get; set; }
        private DBCTContact Data { get; set; }
        private ICacheData MCacheData { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IMembership AMembership { get; set; }

        private dynamic MainView { get; set; }
        public ListBoxEdit DynamicListBoxCat { get; set; }

        private DispatcherTimer CheckDuplicateTimer { get; set; }

        #region CTOR
        public MContactAddEdit(object mainView, DBCTContact data)
        {
            MainView = mainView;
            Owner = MainView as Window;
            Data = data;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            MCacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            TitleVisibility = Visibility.Hidden;

            InitDynamics();
            InitCommands();
            PopulateContactCatList();
            PopulateUserList();

            AutoProfileGenerate = Data == null;

            AddingProfilesList = new ObservableCollection<string>();
            if (Data != null)
            {
                Title = Data.Title;
                Task.Factory.StartNew(
                    () =>
                    {
                        Thread.Sleep(500);
                        HelperUtils.DoDispatcher(
                            () =>
                            {
                                foreach (var v in Data.Categories)
                                {
                                    var item = (from n in ContactCatList where ((DBCTContactCat)n.Tag).Oid == v.Oid select n).FirstOrDefault();
                                    if (item != null)
                                        DynamicListBoxCat.SelectedItems.Add(item);
                                }
                            });
                    });
            }
            else
            {
                Task.Factory.StartNew(
                    () =>
                    {
                        Thread.Sleep(500);
                        HelperUtils.DoDispatcher(
                            () =>
                            {
                                var catsString = DBMSSetting2.LoadSettings<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.DefaultCategoryOnCreation);
                                if (string.IsNullOrEmpty(catsString)) return;
                                var cats = catsString.Split(',');

                                foreach (var v in cats)
                                {
                                    var item = (from n in ContactCatList where n.Title == v select n).FirstOrDefault();
                                    if (item != null)
                                        DynamicListBoxCat.SelectedItems.Add(item);
                                }
                            });
                    });
            }

        }

        private void PopulateContactCatList()
        {
            ContactCatList = new ObservableCollection<CacheItemContactCat>();

            MCacheData
                .GetContactCatList()
                .Where(n => (
                    AMembership.ActiveUser.UserName.ToLower() == "admin" || (
                    n.Role != null &&
                    AMembership.ActiveUser.Roles != null &&
                    AMembership.ActiveUser.Roles.Contains(n.Role))))
                .ToList()
                .ForEach(n => ContactCatList.Add(n));
        }
        private void PopulateUserList()
        {
            CanChangeCreator = AMembership.HasPermission(PCOPermissions.Contact_Contact_AllowChangeCreator);
            if (CanChangeCreator)
            {
                var v = DBMSUser2.UserGetAll(ADatabase.Dxs, null, true);
                UserList = (from u in v select u).ToList();

                if (Data == null)
                {
                    SelectedUser =
                        (from n in UserList
                         where n.UsernameLower == AMembership.ActiveUser.UserName.ToLower()
                         select n).FirstOrDefault();
                }
                else
                {
                    if (Data.UserCreated != null)
                        SelectedUser =
                            (from n in UserList
                             where n.UsernameLower == Data.UserCreated.ToLower()
                             select n).FirstOrDefault();
                }
            }
            else UserList = null;
        }
        #endregion

        #region WindowTitle
        public string WindowTitle
        {
            get { return Data == null ? "ایجاد پرونده" : "ویرایش پرونده"; }
        }
        #endregion
        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");
                CheckDuplicateTitleWithDelay();
            }
        }
        #endregion
        #region CanAutoProfileGenerate
        public bool CanAutoProfileGenerate { get { return Data == null; } }
        #endregion
        #region AutoProfileGenerate
        private bool _AutoProfileGenerate;
        public bool AutoProfileGenerate
        {
            get { return _AutoProfileGenerate; }
            set
            {
                _AutoProfileGenerate = value;
                RaisePropertyChanged("AutoProfileGenerate");
            }
        }
        #endregion
        #region ContactCatList
        private ObservableCollection<CacheItemContactCat> _ContactCatList;
        public ObservableCollection<CacheItemContactCat> ContactCatList
        {
            get { return _ContactCatList; }
            set
            {
                _ContactCatList = value;
                RaisePropertyChanged("ContactCatList");
            }
        }
        #endregion

        #region AddingProfilesList
        private ObservableCollection<string> _AddingProfilesList;
        public ObservableCollection<string> AddingProfilesList
        {
            get { return _AddingProfilesList; }
            set
            {
                _AddingProfilesList = value;
                RaisePropertyChanged("AddingProfilesList");
            }
        }
        #endregion

        #region UserList
        private List<DBMSUser2> _UserList;
        public List<DBMSUser2> UserList
        {
            get { return _UserList; }
            set
            {
                _UserList = value;
                RaisePropertyChanged("UserList");
            }
        }
        #endregion
        #region SelectedUser
        private DBMSUser2 _SelectedUser;
        public DBMSUser2 SelectedUser
        {
            get { return _SelectedUser; }
            set
            {
                if (value == _SelectedUser)
                    return;

                _SelectedUser = value;
                RaisePropertyChanged("SelectedUser");
            }
        }
        #endregion
        #region CanChangeCreator
        private bool _CanChangeCreator;
        public bool CanChangeCreator
        {
            get { return _CanChangeCreator; }
            set
            {
                if (value == _CanChangeCreator)
                    return;

                _CanChangeCreator = value;
                RaisePropertyChanged("CanChangeCreator");
            }
        }
        #endregion

        #region TitleVisibility
        private Visibility _TitleVisibility;
        public Visibility TitleVisibility
        {
            get { return _TitleVisibility; }
            set
            {
                _TitleVisibility = value;
                RaisePropertyChanged("TitleVisibility");
            }
        }
        #endregion





        private void CheckDuplicateTitleWithDelay()
        {
            if (CheckDuplicateTimer == null)
            {
                CheckDuplicateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
                CheckDuplicateTimer.Tick += (s, e) =>
                {
                    CheckDuplicateTimer.Stop();
                    CheckDuplicateTitle(_Title);
                };
            }
            CheckDuplicateTimer.Stop();
            CheckDuplicateTimer.Start();
        }


        #region [METHODS]
        private void InitDynamics()
        {
            DynamicListBoxCat = MainView.DynamicListBoxCat;
            DynamicListBoxCat.SelectedIndexChanged +=
                (s, e) =>
                {
                    AddingProfilesList.Clear();
                    var catList = (from n in DynamicListBoxCat.SelectedItems.Cast<CacheItemContactCat>()
                                   select (DBCTContactCat)n.Tag).ToList();

                    catList.ForEach(cat => cat.ProfileRoots.ToList().ForEach(p => { if (!AddingProfilesList.Contains(p.Title)) AddingProfilesList.Add(p.Title); }));
                    RaisePropertyChanged("AddingProfilesList");
                };

        }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (Validate())
                        if (Save())
                            RaiseRequestClose(true);
                }, () => !string.IsNullOrWhiteSpace(Title));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp28 != "");
        }
        private bool Validate()
        {
            if (DynamicListBoxCat.SelectedItems.Count == 0)
            {
                POLMessageBox.ShowError("حداقل یك دسته می بایست انتخاب شود.", Owner);
                return false;
            }
            if (CanChangeCreator && SelectedUser == null)
            {
                POLMessageBox.ShowError("لطفا ثبت كننده پرونده را تعیین نمایید.", Owner);
                return false;
            }
            return true;
        }
        private bool Save()
        {
            APOCMainWindow.ShowBusyIndicator();
            try
            {
                if (Data == null)
                {
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        var code = DBCTContact.GetNextCode(uow);
                        Data = new DBCTContact(uow) { Code = code, Title = Title };

                        var list =
                            (from n in DynamicListBoxCat.SelectedItems.Cast<CacheItemContactCat>()
                             select DBCTContactCat.FindByOid(uow, ((DBCTContactCat)n.Tag).Oid)).ToList();
                        list.ForEach(n => Data.Categories.Add(n));

                        if (CanChangeCreator)
                            Data.UserCreated = SelectedUser.UsernameLower;
                        uow.CommitChanges();
                    }
                    Data = DBCTContact.FindByOid(ADatabase.Dxs, Data.Oid);

                    if (AutoProfileGenerate)
                    {
                        var list = (from n in DynamicListBoxCat.SelectedItems.Cast<CacheItemContactCat>() select (DBCTContactCat)n.Tag).ToList();
                        var profiles = new List<DBCTProfileRoot>();
                        list.ForEach(cat => cat.ProfileRoots.ToList().ForEach(p => { if (!profiles.Contains(p)) profiles.Add(p); }));
                        if (profiles.Any())
                            profiles.ForEach(p => Data.AddProfileObjectToContact(p));
                    }

                    var contact = DBCTContact.PopulateToFake(Data);
                }
                else
                {
                    var oldCat = Data.Categories.ToList();
                    oldCat.ForEach(n => Data.Categories.Remove(n));
                    var newCat = (from n in DynamicListBoxCat.SelectedItems.Cast<CacheItemContactCat>() select n.Tag as DBCTContactCat).ToList();
                    newCat.ForEach(n => Data.Categories.Add(n));
                    Data.Title = Title;
                    if (CanChangeCreator)
                        Data.UserCreated = SelectedUser.UsernameLower;
                    Data.Save();
                }
                APOCMainWindow.HideBusyIndicator();
                MainView.AddedContact = Data;
                var ContactFastRegistration = false;
                HelperUtils.Try(() => { ContactFastRegistration = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactFastRegistration); });
                if (!ContactFastRegistration)
                    POLMessageBox.ShowInformation(string.Format("پرونده با كد : {0} ثبت شد.", Data.Code), Owner);
                return true;
            }
            catch (Exception ex)
            {
                APOCMainWindow.HideBusyIndicator();
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowWarning(ex.Message, Owner);
                return false;
            }
        }
        private void CheckDuplicateTitle(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                TitleVisibility = Visibility.Hidden;
                return;
            }
            var dbc = DBCTContact.FindDuplicateTitleExcept(ADatabase.Dxs, null, value);
            TitleVisibility = dbc == null ? Visibility.Hidden : Visibility.Visible;
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp28);
        }
        #endregion
        
        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCancel { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IRequestCloseViewModel
        public event EventHandler<RequestCloseEventArgs> RequestClose;
        private void RaiseRequestClose(bool? dialogResult)
        {
            if (RequestClose != null)
                RequestClose.Invoke(this, new RequestCloseEventArgs { DialogResult = dialogResult });
        }
        #endregion
    }
}
