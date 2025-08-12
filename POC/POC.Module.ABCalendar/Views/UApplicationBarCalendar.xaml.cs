using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using POC.Module.ABCalendar.Models;

namespace POC.Module.ABCalendar.Views
{
    public partial class UApplicationBarCalendar : UserControl
    {
        public UApplicationBarCalendar()
        {
            InitializeComponent();
            Unloaded += (s, e)
                => SaveLayoutPositions();
            Loaded += (s, e)
                =>
                {
                    mydate.DateTime = DateTime.Now.Date;
                    mydate.OnMyDateTimeChanged += (s1, e1) =>
                                                      {
                                                          var m = DataContext as MApplicationBarCalendar;
                                                          if (m != null)
                                                              m.SelectedDate = mydate.DateTime;
                                                      };
                    LoadLayoutPositions();
                    var w = Window.GetWindow(this);
                    if (w != null)
                    {
                        w.Closing +=
                            (s1, e1) => SaveLayoutPositions();
                    }
                };

        }

        private void LoadLayoutPositions()
        {
            try
            {
                var data = POL.Lib.XOffice.HelperSettingsClient.ABCalendarLayout;
                if (string.IsNullOrWhiteSpace(data)) return;
                var ss = data.Split('|');
                flcMain.BeginInit();
                var items = (from fe in flcMain.Children.Cast<FrameworkElement>() select fe).ToList();
                flcMain.Children.Clear();

                ss.ToList().ForEach(
                    n =>
                    {
                        var q = from fe in items
                                where fe.Name == n
                                select fe;
                        if (!q.Any()) return;
                        flcMain.Children.Add(q.First());
                    });
                flcMain.EndInit();
            }
            catch { }
        }
        private void SaveLayoutPositions()
        {
            try
            {
                var q =
                    (from n in flcMain.Children.Cast<FrameworkElement>()
                     where !string.IsNullOrWhiteSpace(n.Name)
                     orderby n.GetValue(Panel.ZIndexProperty)
                     select n).ToList();
                var data = string.Empty;
                q.ForEach(
                    fe =>
                    {
                        data += string.Format("{0}|", fe.Name);
                    });
                POL.Lib.XOffice.HelperSettingsClient.ABCalendarLayout = data;
            }
            catch
            {
            }
        }
    }
}
