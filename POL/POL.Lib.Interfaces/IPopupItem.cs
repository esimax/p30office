using System;
using System.Windows;
using System.Windows.Media;

namespace POL.Lib.Interfaces
{
    public interface IPopupItem
    {
        UIElement PopupElement { get; }
        Brush PopupBrush { get; }
        TimeSpan PopupTimeOut { get; }
        bool PopupCanPin { get; }
        bool PopupCanClose { get; }
        bool PopupCanTimeOut { get; }
        double PopupWidth { get; }
        double PopupHeight { get; }

        bool IsClosed { get; }
    }
}
