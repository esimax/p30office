using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Membership;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POC.Module.ABTools.Views;

namespace POC.Module.ABTools.Models
{
    public class MApplicationBarChat : NotifyObjectBase
    {
        private IPOCRootTools APOCRootTools { get; set; }
        private IMembership AMembership { get; set; }
        private IMessagingClient AMessagingClient { get; set; }
        private IDatabase ADatabase { get; set; }
        private POCCore APOCCore { get; set; }
        private IPopup APopup { get; set; }
        dynamic MainView { get; set; }
        public MApplicationBarChat(object mainView)
        {
            MainView = mainView;

            APOCRootTools = ServiceLocator.Current.GetInstance<IPOCRootTools>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            AMessagingClient = ServiceLocator.Current.GetInstance<IMessagingClient>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            APopup = ServiceLocator.Current.GetInstance<IPopup>();

            AMembership.OnMembershipStatusChanged += 
                (s, e) =>
                {
                    if (e.Status == EnumMembershipStatus.AfterLogin)
                    {
                        PopulateUsers();
                    }
                    if (e.Status == EnumMembershipStatus.AfterLogout)
                    {
                        ListOfUsers = null;
                    }



                };
            InitCommands();

            AMessagingClient.RegisterHookForMessage((mi) =>
                                                    {
                                                        var mi2 = mi.MessageData as string[];
                                                        var text = mi2[0];
                                                        var user = mi2[1];
                                                        var sender = mi2[3];
                                                        var users = user.Split('|');
                                                        if (!users.Contains(AMembership.ActiveUser.UserName.ToLower()))
                                                            return;
                                                        MainView.AddChat(text,sender,false);

                                                        var ui = new UPopupChat(sender,text);

                                                        APopup.AddPopup(ui);

                                                    }, EnumMessageKind.Chat);
        }
        


        #region ListOfUsers
        private List<DBMSUser2> _ListOfUsers;
        public List<DBMSUser2> ListOfUsers
        {
            get { return _ListOfUsers; }

            set
            {
                _ListOfUsers = value;
                RaisePropertyChanged("ListOfUsers");
            }
        }
        #endregion
        #region MessageText
        private string _MessageText;
        public string MessageText
        {
            get { return _MessageText; }

            set
            {
                _MessageText = value;
                RaisePropertyChanged("MessageText");
            }
        }
        #endregion





        #region [METHODS]
        private void PopulateUsers()
        {
            ListOfUsers = DBMSUser2.UserGetAll(ADatabase.Dxs, null)
                .Where(n=>n.UsernameLower != AMembership.ActiveUser.UserName.ToLower())
                .OrderBy(n=>n.Title) 
                .ToList();
        }
        private void InitCommands()
        {
            CommandSend = new RelayCommand(Send, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp04 != "");
        }

        private void Send()
        {
            var users = MainView.GetSelectedUsers() as List<DBMSUser2>;

            var s = String.Join("|", users.Select(n => n.UsernameLower).ToList());

            if (string.IsNullOrWhiteSpace(MessageText))
                return;

            AMessagingClient.SendMessage(new MessagingItem()
                           {
                               From = APOCCore.InstanceGuid,
                               MessageData = new string[] { MessageText, s, AMembership.ActiveUser.UserName.ToLower(), AMembership.ActiveUser.Title },
                               MessageDate = DateTime.Now,
                               MessageKind = EnumMessageKind.Chat,
                               To = Guid.Empty,
                           });
            MainView.AddChat(MessageText, AMembership.ActiveUser.Title, true);

        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp04);
        }
        #endregion


        #region IDisposable
        public void Dispose()
        {

        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandSend { get; set; }
        public RelayCommand CommandHelp { get; set; } 
        #endregion
    }
}
