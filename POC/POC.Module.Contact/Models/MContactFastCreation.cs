using System;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors.Helpers;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Contact.Models
{
    public class MContactFastCreation : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCFastContactUnit AFastContactUnit { get; set; }

        private dynamic MainView { get; set; }
        public DBCTContact DynamicContact { get; set; }
        public StackPanel DynamicTheStackPanel { get; set; }
        public MContactFastCreation(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            AFastContactUnit = ServiceLocator.Current.GetInstance<IPOCFastContactUnit>();

            InitDynamics();


            SelectedContactCategories = string.Empty;
            DynamicContact.Categories.ForEach(cat =>
            {
                SelectedContactCategories += cat + Environment.NewLine;
            });

            PopulateFastContactItems();

            InitCommands();
        }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => true);
        }

        private void OK()
        {
            foreach (var uie in DynamicTheStackPanel.Children)
            {
                if (!(uie is IValidateSaveFastContactModule)) continue;
                var vsfc = uie as IValidateSaveFastContactModule;
                if (!vsfc.Validate())
                    return;
            }

            foreach (var uie in DynamicTheStackPanel.Children)
            {
                if (!(uie is IValidateSaveFastContactModule)) continue;
                var vsfc = uie as IValidateSaveFastContactModule;
                if (!vsfc.Save())
                    return;
            }

            var contact = DBCTContact.PopulateToFake(DynamicContact);

            MainView.DialogResult = true;
            MainView.Close();

        }

        private void PopulateFastContactItems()
        {
            AFastContactUnit.GetList().ForEach(u =>
            {
                var uc = Activator.CreateInstance(u.ContentType);
                if (uc is IValidateSaveFastContactModule)
                {
                    var vsfc = uc as IValidateSaveFastContactModule;
                    vsfc.Contact = DynamicContact;
                }
                var uie = uc as UIElement;
                if (uie != null)
                    DynamicTheStackPanel.Children.Add(uie);
            });
        }

        public string SelectedContactCategories { get; set; }

        private void InitDynamics()
        {
            DynamicContact = MainView.DynamicContact;
            DynamicTheStackPanel = MainView.DynamicTheStackPanel;
        }

        public RelayCommand CommandOK { get; set; }
    }
}
