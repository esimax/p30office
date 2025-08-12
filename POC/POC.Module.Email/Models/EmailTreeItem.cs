using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using POL.WPF.DXControls.MVVM;
using POL.DB.P30Office;

namespace POC.Module.Email.Models
{
    public class EmailTreeItem : NotifyObjectBase
    {
        public string Name { get; set; }
        public BitmapSource Icon { get; set; }

        #region UnreadCount
        private int _UnreadCount;
        public int UnreadCount
        {
            get { return _UnreadCount; }
            set
            {
                _UnreadCount = value;
                RaisePropertyChanged("UnreadCount");
            }
        }
        #endregion

        public DBEMEmailFolder DBFolder { get; set; }
        public ObservableCollection<EmailTreeItem> SubFolders { get; set; }
    }
}
