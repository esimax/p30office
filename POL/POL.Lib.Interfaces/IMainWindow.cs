using System;
using System.Windows;

namespace POL.Lib.Interfaces
{
    public interface IMainWindow
    {
        void ShowWindow();
        void HideWindow();

        void ShowBusyIndicator();
        void HideBusyIndicator();

        void LoadContent(Type view, Type model);

        Window GetWindow();
    }
}
