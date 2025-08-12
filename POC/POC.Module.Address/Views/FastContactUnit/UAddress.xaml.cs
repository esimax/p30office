using System.Linq;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;

namespace POC.Module.Address.Views.FastContactUnit
{
    public partial class UAddress : UserControl, IValidateSaveFastContactModule
    {
        private IDatabase ADatabase { get; set; }
        private POCCore APOCCore { get; set; }
        private ILoggerFacade ALogger { get; set; }
        public UAddress()
        {
            InitializeComponent();

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            var AddressTitleList = (from n in DBBTAddressTitle2.GetAll(ADatabase.Dxs)
                                  select n.Title).ToList();

            cbeAddresstitle1.ItemsSource = AddressTitleList;
            cbeAddresstitle2.ItemsSource = AddressTitleList;
        }

        public bool Validate()
        {
            return true;
        }

        public bool Save()
        {
            if (!string.IsNullOrEmpty(teAddress1.Text))
            {
                var ct = Contact as DBCTContact;
                if (ct != null)
                {
                    var dbp = new DBCTAddress(ct.Session);
                    dbp.Address = teAddress1.Text;
                    dbp.Title = cbeAddresstitle1.Text;
                    dbp.Contact = ct;
                    dbp.Save();
                }
            }
            if (!string.IsNullOrEmpty(teAddress2.Text))
            {
                var ct = Contact as DBCTContact;
                if (ct != null)
                {
                    var dbp = new DBCTAddress(ct.Session);
                    dbp.Address = teAddress2.Text;
                    dbp.Title = cbeAddresstitle2.Text;
                    dbp.Contact = ct;
                    dbp.Save();
                }
            }
            return true;
        }

        public object Contact { get; set; }
    }
}
