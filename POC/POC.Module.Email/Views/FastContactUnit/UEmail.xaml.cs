using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.DXControls;

namespace POC.Module.Email.Views.FastContactUnit
{
    public partial class UEmail : UserControl, IValidateSaveFastContactModule
    {
        private IDatabase ADatabase { get; set; }
        private POCCore APOCCore { get; set; }
        private ILoggerFacade ALogger { get; set; }

        public UEmail()
        {
            InitializeComponent();

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            var EmailTitleList = (from n in POL.DB.P30Office.BT.DBBTEmailTitle2.GetAll(ADatabase.Dxs)
                                  select n.Title).ToList();

            cbeEmailtitle1.ItemsSource = EmailTitleList;
            cbeEmailtitle2.ItemsSource = EmailTitleList;
        }

        public bool Validate()
        {
            if (!string.IsNullOrEmpty(teEmail1.Text))
            {
                if (!teEmail1.Text.IsValidEmailAddress())
                {
                    POLMessageBox.ShowWarning("آدرس ایمیل اول معتبر نمی باشد.", Window.GetWindow(this));
                    return false;
                }
                var dbe = DBCTEmail.FindByAddressExcept(ADatabase.Dxs, null, teEmail1.Text);
                if (dbe != null)
                {
                    POLMessageBox.ShowWarning("آدرس ایمیل اول قبلا ثبت شده، امكان ثبت مجدد نمی باشد.", Window.GetWindow(this));
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(teEmail2.Text))
            {
                if (!teEmail2.Text.IsValidEmailAddress())
                {
                    POLMessageBox.ShowWarning("آدرس ایمیل دوم معتبر نمی باشد.", Window.GetWindow(this));
                    return false;
                }
                var dbe = DBCTEmail.FindByAddressExcept(ADatabase.Dxs, null, teEmail2.Text);
                if (dbe != null)
                {
                    POLMessageBox.ShowWarning("آدرس ایمیل دوم قبلا ثبت شده، امكان ثبت مجدد نمی باشد.", Window.GetWindow(this));
                    return false;
                }
            }
            return true;
        }

        public bool Save()
        {
            if (!string.IsNullOrEmpty(teEmail1.Text))
            {
                var ct = Contact as DBCTContact;
                if (ct != null)
                {
                    var dbp = new DBCTEmail(ct.Session);
                    dbp.Address = teEmail1.Text;
                    dbp.Title = cbeEmailtitle1.Text;
                    dbp.Contact = ct;
                    dbp.Save();
                }
            }
            if (!string.IsNullOrEmpty(teEmail2.Text))
            {
                var ct = Contact as DBCTContact;
                if (ct != null)
                {
                    var dbp = new DBCTEmail(ct.Session);
                    dbp.Address = teEmail2.Text;
                    dbp.Title = cbeEmailtitle2.Text;
                    dbp.Contact = ct;
                    dbp.Save();
                }
            }
            return true;
        }

        public object Contact { get; set; }
    }
}
