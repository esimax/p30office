using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POL.DB.P30Office;
using POC.Module.Email.Models;


namespace POC.Module.Email.Views
{
    public partial class WEmailSend : DXWindow
    {
        private MEmailSend Model { get; set; }

        public WEmailSend(DBEMEmailApp emailApp, DBEMEmailInbox defInbox = null, string defEmails = null, DBCTContact defContact = null, DBCTContactCat defCat = null, DBCTContactSelection defBasket = null)
        {
            InitializeComponent();

            DynamicFrom = emailApp;

            DynamicInbox = defInbox;
            DynamicEmailAddresses = defEmails;
            DynamicContact = defContact;
            DynamicCategory = defCat;
            DynamicBasket = defBasket;


            Loaded += (s, e)
                =>
                {
                    tcMain.SelectedIndex = 0;
                    Model = new MEmailSend(this);
                    DataContext = Model;
                    firstFocused.Focus();
                    POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();
                    Task.Factory.StartNew(
                        () =>
                        {
                            System.Threading.Thread.Sleep(200);
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                                new Action(() => firstFocused.SelectAll()));
                        });
                };
        }

        public DBEMEmailApp DynamicFrom { get; set; }
        public DBEMEmailInbox DynamicInbox { get; set; }
        public string DynamicEmailAddresses { get; set; }
        public DBCTContact DynamicContact { get; set; }
        public DBCTContactCat DynamicCategory { get; set; }
        public DBCTContactSelection DynamicBasket { get; set; }

        public RichTextBox DynamicRichTextBox { get { return rtbSimpleBody; } }

    }
}
