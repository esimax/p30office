using System;

namespace POL.Lib.Interfaces
{
    public interface IPopup
    {
        void AddPopup(IPopupItem item);
        void RemovePopup(IPopupItem item);

        event EventHandler<ObjectEventArgs> OnPopupAdded;
        event EventHandler<ObjectEventArgs> OnPopupRemoveed;
    }
}
