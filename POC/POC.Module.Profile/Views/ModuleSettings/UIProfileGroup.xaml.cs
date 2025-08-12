using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using Microsoft.Practices.Prism.Logging;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIProfileGroup
    {
        private ICacheData MCacheData { get; set; }
        private ILoggerFacade MLogger { get; set; }
        public UIProfileGroup()
        {
            InitializeComponent();
            MCacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            MLogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();

            JustConstructed = true;
            Loaded += (s, e) =>
            {

                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;


                cbRequired.IsChecked = Item.IsRequired;
                cbRequired.EditValueChanged +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                        Item.IsRequired = cbRequired.IsChecked == true;
                    };
            };
            

            Unloaded += (s, e) => SaveChanges();

        }

        public DBCTProfileGroup Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }

        private void SaveChanges()
        {
            if (Item == null) return;
            if (!HasChanges) return;
            
            Item.Save();
            HasChanges = false;
        }
    }
}
