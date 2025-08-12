using System;
using System.IO;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POL.DB.P30Office.AC;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Attachment.Models
{
    public class MFactorReportTemplateAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBACFactorReportTemplate DynamicSelectedData { get; set; }

        private bool FileIsSelected { get; set; }

        #region CTOR
        public MFactorReportTemplateAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitCommands();
            GetDynamicData();
            PopulateData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات عنوان فاکتور"; }
        }
        #endregion



        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (ReferenceEquals(_Title, value)) return;
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        #endregion

        #region FileName
        private string _FileName;
        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (ReferenceEquals(_FileName, value)) return;
                _FileName = value;
                RaisePropertyChanged("FileName");
            }
        }
        #endregion

        public bool CanDownloadFile
        {
            get
            {
                var rv = false;
                if (DynamicSelectedData == null)
                    rv = !FileIsSelected && !string.IsNullOrEmpty(FileName);
                else
                    rv = !string.IsNullOrEmpty(FileName);
                return rv;
            }
        }



        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => !string.IsNullOrWhiteSpace(Title));
            CommandSelectFile = new RelayCommand(SelectFile, () => true);
            CommandDownloadFile = new RelayCommand(DownloadFile, () => CanDownloadFile);
        }

        private void DownloadFile()
        {
            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "html",
                Filter = "HTML (*.html)|*.html",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = Path.GetFileName(FileName)
            };
            if (sf.ShowDialog() != true) return;

            try
            {
                File.WriteAllText(sf.FileName,DynamicSelectedData.FileData);
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", DynamicOwner);
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }

        private void SelectFile()
        {
            var of = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "html",
                Filter = "Html (*.html)|*.html",
                FilterIndex = 0,
                RestoreDirectory = true,
            };

            if (of.ShowDialog() != true)
                return;
            FileIsSelected = true;
            FileName = of.FileName;
            RaisePropertyChanged("CanDownloadFile");
            RaisePropertyChanged("CommandDownloadFile");
        }

        private void OK()
        {
            if (Validate())
                if (Save())
                {
                    DynamicOwner.DialogResult = true;
                    DynamicOwner.Close();
                }
        }



        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title)) Title = string.Empty;
            if (string.IsNullOrWhiteSpace(Title.Trim()))
            {
                POLMessageBox.ShowError("عنوان معتبر نمی باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }

            if (string.IsNullOrWhiteSpace(FileName))
            {
                POLMessageBox.ShowError("فایلی انتخاب نشده است. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }

            var db = DBACFactorReportTemplate.FindDuplicateTitleExcept(ADatabase.Dxs, DynamicSelectedData, HelperConvert.CorrectPersianBug(Title.Trim()));
            if (db != null)
            {
                POLMessageBox.ShowError("عنوان تكراری می باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }
            return true;
        }
        private bool Save()
        {
            try
            {
                if (DynamicSelectedData == null)
                {
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        DynamicSelectedData = new DBACFactorReportTemplate(uow)
                        {
                            Title = Title.Trim(),
                            FileName = FileName,
                        };
                        if (FileIsSelected && !string.IsNullOrWhiteSpace(FileName))
                        {
                            DynamicSelectedData.FileData = File.ReadAllText(FileName);
                        }
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBACFactorReportTemplate.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = Title.Trim();
                    if (FileIsSelected && !string.IsNullOrWhiteSpace(FileName))
                    {
                        DynamicSelectedData.FileName = FileName;
                        DynamicSelectedData.FileData = File.ReadAllText(FileName);
                    }
                    DynamicSelectedData.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowWarning(ex.Message, DynamicOwner);
                return false;
            }
        }

        private void PopulateData()
        {
            if (DynamicSelectedData == null)
                return;
            Title = DynamicSelectedData.Title;
            FileName = DynamicSelectedData.FileName;
            RaisePropertyChanged("CanDownloadFile");
            RaisePropertyChanged("CommandDownloadFile");
        }
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandSelectFile { get; set; }
        public RelayCommand CommandDownloadFile { get; set; }
        #endregion
    }
}
