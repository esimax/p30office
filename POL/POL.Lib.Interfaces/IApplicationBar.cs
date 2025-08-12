using System;
using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public interface IApplicationBar
    {
        void SlideIn(object sender, object parameter);
        void SlideOut(object sender, object parameter);

        event EventHandler<ObjectEventArgs> OnSlideIn;
        event EventHandler<ObjectEventArgs> OnSlideOut;

        void RegisterContent(string name, int order, string title, Type viewType, Type modelType, bool isFirst);
        void RegisterContent(ApplicationBarHolder holder);

        List<ApplicationBarHolder> GetContentList();
    }
}
