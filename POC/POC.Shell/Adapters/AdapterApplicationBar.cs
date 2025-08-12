using System;
using System.Collections.Generic;
using POL.Lib.Interfaces;
using System.Linq;

namespace POC.Shell.Adapters
{
    internal partial class AdapterApplicationBar : IApplicationBar
    {
        #region CTOR
        public AdapterApplicationBar()
        {
            Holder = new List<ApplicationBarHolder>();
        } 
        #endregion

        private List<ApplicationBarHolder> Holder { get; set; }

        #region IApplicationBar
        public event EventHandler<ObjectEventArgs> OnSlideIn;
        public event EventHandler<ObjectEventArgs> OnSlideOut;

        public void SlideIn(object sender, object parameter)
        {
            if (OnSlideIn != null)
                OnSlideIn.Invoke(sender, new ObjectEventArgs(parameter));
        }
        public void SlideOut(object sender, object parameter)
        {
            if (OnSlideOut != null)
                OnSlideOut.Invoke(sender, new ObjectEventArgs(parameter));
        }


        public void RegisterContent(string name, int order, string title, Type viewType, Type modelType, bool isFirst)
        {
            var abh = new ApplicationBarHolder
                {
                    Name = name,
                    Order = order,
                    Title = title,
                    ViewType = viewType,
                    ModelType = modelType,
                    IsFirst = isFirst,
                };
            RegisterContent(abh);
        }
        public void RegisterContent(ApplicationBarHolder holder)
        {
            if (Holder.Exists(h => h.Name == holder.Name))
                throw new ArgumentException("Name", "Can not register duplicate names.");
            Holder.Add(holder);
        }
        public List<ApplicationBarHolder> GetContentList()
        {
            return Holder.OrderBy(h=>h.Order).ToList();
        }
        #endregion
    }
}
