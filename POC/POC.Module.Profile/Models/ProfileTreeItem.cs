using POL.WPF.DXControls.MVVM;
using System.Windows;
using System.Windows.Media.Imaging;

namespace POC.Module.Profile.Models
{
    public class ProfileTreeItem : NotifyObjectBase
    {
        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");
            }
        } 
        #endregion

        public BitmapSource Image { get; set; }
        public Thickness ImageMargin { get; set; }
        public object Tag { get; set; }
        public int Order { get; set; }

        #region TextWeight
        private FontWeight _TextWeight;
        public FontWeight TextWeight
        {
            get { return _TextWeight; }
            set
            {
                _TextWeight = value;
                RaisePropertyChanged("TextWeight");
            }
        } 
        #endregion
    }
}
