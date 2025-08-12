using POL.Lib.Interfaces;
using System;

namespace POC.Shell.Adapters
{
    public class AdapterPopup : IPopup
    {
        #region IPopup
        public void AddPopup(IPopupItem item)
        {
            if (item == null) return;
            if (OnPopupAdded == null) return;
            var e = OnPopupAdded;
            e.Invoke(null, new ObjectEventArgs(item));
        }
        public void RemovePopup(IPopupItem item)
        {
            if (item == null) return;
            if (OnPopupRemoveed == null) return;
            var e = OnPopupRemoveed;
            e.Invoke(null, new ObjectEventArgs(item));
        }

        public event EventHandler<ObjectEventArgs> OnPopupAdded;
        public event EventHandler<ObjectEventArgs> OnPopupRemoveed; 
        #endregion
    }
}
