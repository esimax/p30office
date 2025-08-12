using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.ABTools.Models
{
    public class MApplicationBarTools : NotifyObjectBase
    {
        public MApplicationBarTools(object mainView)
        {
            APOCRootTools = ServiceLocator.Current.GetInstance<IPOCRootTools>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            pocCore = ServiceLocator.Current.GetInstance<POCCore>();
            AMembership.OnMembershipStatusChanged +=
                (s, e) =>
                {
                    if (e.Status == EnumMembershipStatus.AfterLogin)
                    {
                        RootToolList = new List<POCRootToolItem>();
                        RootToolList = (from n in APOCRootTools.GetList() where !pocCore.STCI.IsTamas || n.InTamas select n).ToList();
                        RaisePropertyChanged("RootToolList");
                    }

                };
        }

        private IPOCRootTools APOCRootTools { get; set; }
        private IMembership AMembership { get; set; }
        private POCCore pocCore { get; set; }


        public List<POCRootToolItem> RootToolList { get; set; }


        #region [METHODS]

        #endregion


        #region IDisposable
        public void Dispose()
        {

        }
        #endregion
    }
}
