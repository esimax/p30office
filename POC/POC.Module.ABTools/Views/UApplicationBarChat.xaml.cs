using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using POL.DB.Membership;
using POL.Lib.Interfaces;

namespace POC.Module.ABTools.Views
{
    public partial class UApplicationBarChat : UserControl
    {

        private IMessagingClient AMessagingClient { get; set; }

        public UApplicationBarChat()
        {
            InitializeComponent();

        }

        public List<DBMSUser2> GetSelectedUsers()
        {
            var newVal = (from n in lbeUsers.SelectedItems.Cast<DBMSUser2>() select n).ToList();
            return newVal;
        }

        public void AddChat(string text, string sender, bool ismytext)
        {
            var tb = new TextBlock()
                     {
                         TextWrapping = TextWrapping.Wrap,
                         Text = !ismytext ? sender + " :" + Environment.NewLine + text : text,
                         Margin = new Thickness(3),
                         Foreground = ismytext ? Brushes.Gray : Brushes.Green,
                         Width = 200,
                     };
            lbChatData.Items.Insert(0, tb);
        }


    }
}
