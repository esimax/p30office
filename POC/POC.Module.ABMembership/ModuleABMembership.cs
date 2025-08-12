using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using POC.Module.ABMembership.Models;
using POC.Module.ABMembership.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using Microsoft.Practices.ServiceLocation;

namespace POC.Module.ABMembership
{
    [Version]
    [Priority(ConstantPOCModules.OrderABMembership)]
    [Module(ModuleName = ConstantPOCModules.NameABMembership)]
    public class ModuleABMembership : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private IApplicationBar ApplicationBar { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCRootTools ARootTools { get; set; }

        public ModuleABMembership(IUnityContainer unityContainer, ILoggerFacade logger, IApplicationBar applicationBar, IPOCRootTools rootTools)
        {
            UnityContainer = unityContainer;
            ApplicationBar = applicationBar;
            Logger = logger;
            ARootTools = rootTools;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NameABMembership), Category.Debug, Priority.None);
            ApplicationBar.RegisterContent(new ApplicationBarHolder
            {
                Name = ConstantApplicationBar.NameMembership,
                Order = ConstantApplicationBar.OrderMembership,
                Title = "عضویت",
                ViewType = typeof(UApplicationBarMembership),
                ModelType = typeof(MApplicationBarMembership),
                IsFirst = true,
            });

            RegisterRootTool();
        }
        #endregion

        private void RegisterRootTool()
        {
            ARootTools.RegisterRootTool(
                new POCRootToolItem
                {
                    Key = ConstantPOCModules.NameABMembership,
                    Image = HelperImage.GetStandardImage32("_32_User.png"),
                    Order = ConstantPOCRootTool.OrderMembership,
                    Permission = (int)PCOPermissions.Membership_Settings,
                    Title = "مدیریت كاربران",
                    Tooltip = "مدیریت كاربران و تعریف سطوح دسترسی",
                    Command = new POL.WPF.Controles.MVVM.RelayCommand<object>(
                        o =>
                        {
                            var v = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            v.Show();
                            v.LoadContent(typeof(POL.Lib.Common.POLMembership.UMembershipUserManagment),  null);
                        }),
                        InTamas = true,
                });
        }
    }
}
