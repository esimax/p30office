using POL.DB.P30Office;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Contact.Models
{
    public class CategoryHolder : NotifyObjectBase
    {
        public CategoryHolder()
        {
            CategoryState = null;
        }

        #region CategoryTitle
        private string _CategoryTitle;
        public string CategoryTitle
        {
            get { return _CategoryTitle; }
            set
            {
                _CategoryTitle = value;
                RaisePropertyChanged("CategoryTitle");
            }
        }
        #endregion

        #region CategoryState
        private bool? _CategoryState;
        public bool? CategoryState
        {
            get { return _CategoryState; }
            set
            {
                _CategoryState = value;
                RaisePropertyChanged("CategoryState");
            }
        }
        #endregion

        public DBCTContactCat Category { get; set; }

        public override string ToString()
        {
            return CategoryTitle;
        }
    }
}
