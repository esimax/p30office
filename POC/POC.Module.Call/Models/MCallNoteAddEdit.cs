using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Call.Models
{
    public class MCallNoteAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IMembership AMembership { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCLCall DynamicDBCLCall { get; set; }

        public MCallNoteAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitDynamics();
            InitCommands();

            PopulateTitleList();
        }





        #region WindowTitle
        public string WindowTitle
        {
            get { return "نكته تماس"; }
        }
        #endregion

        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");

                var db = DBBTCallNoteTitle2.FindDuplicateTitleExcept(ADatabase.Dxs, null, value);
                if (db == null) return;
                SetColorIndex(db.LastColorIndex);
            }
        }
        #endregion

        public List<string> TitleList { get; set; }

        #region IsNone
        private bool _IsNone;
        public bool IsNone
        {
            get { return _IsNone; }
            set
            {
                _IsNone = value;
                RaisePropertyChanged("IsNone");
            }
        }
        #endregion
        #region IsGray
        private bool _IsGray;
        public bool IsGray
        {
            get { return _IsGray; }
            set
            {
                _IsGray = value;
                RaisePropertyChanged("IsGray");
            }
        }
        #endregion
        #region IsYellow
        private bool _IsYellow;
        public bool IsYellow
        {
            get { return _IsYellow; }
            set
            {
                _IsYellow = value;
                RaisePropertyChanged("IsYellow");
            }
        }
        #endregion
        #region IsRed
        private bool _IsRed;
        public bool IsRed
        {
            get { return _IsRed; }
            set
            {
                _IsRed = value;
                RaisePropertyChanged("IsRed");
            }
        }
        #endregion
        #region IsGreen
        private bool _IsGreen;
        public bool IsGreen
        {
            get { return _IsGreen; }
            set
            {
                _IsGreen = value;
                RaisePropertyChanged("IsGreen");
            }
        }
        #endregion
        #region IsBlue
        private bool _IsBlue;
        public bool IsBlue
        {
            get { return _IsBlue; }
            set
            {
                _IsBlue = value;
                RaisePropertyChanged("IsBlue");
            }
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion


        private void InitDynamics()
        {
            DynamicOwner = MainView as Window;
            DynamicDBCLCall = MainView.DynamicDBCLCall;
            IsNone = true;
            if (DynamicDBCLCall != null)
            {
                Title = DynamicDBCLCall.NoteText;
                SetColorIndex(DynamicDBCLCall.NoteFlag);
            }
        }
        private void PopulateTitleList()
        {
            var xpc = DBBTCallNoteTitle2.GetAll(ADatabase.Dxs);
            TitleList = xpc.ToList().OrderBy(n => n.DateModified).Select(n => n.Title).ToList();
            RaisePropertyChanged("TitleList");
        }
        private void SetColorIndex(int index)
        {
            switch (index)
            {
                case 1: IsGray = true; break;
                case 2: IsYellow = true; break;
                case 3: IsRed = true; break;
                case 4: IsGreen = true; break;
                case 5: IsBlue = true; break;
                default: IsNone = true; break;
            }
        }
        private int GetColorIndex()
        {
            if (IsGray) return 1;
            if (IsYellow) return 2;
            if (IsRed) return 3;
            if (IsGreen) return 4;
            return IsBlue ? 5 : 0;
        }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (Validate())
                        if (Save())
                        {
                            DynamicOwner.DialogResult = true;
                            DynamicOwner.Close();
                        }

                }, () => true);
        }

        private bool Validate()
        {
            if (DynamicDBCLCall == null)
            {
                POLMessageBox.ShowError("خطا : تماس وابسته خالی می باشد.", DynamicOwner);
                return false;
            }
            return true;
        }
        private bool Save()
        {
            try
            {
                if (string.IsNullOrEmpty(Title))
                    Title = string.Empty;

                var db = DBBTCallNoteTitle2.FindDuplicateTitleExcept(ADatabase.Dxs, null, HelperConvert.CorrectPersianBug(Title.Trim())) ??
                         new DBBTCallNoteTitle2(ADatabase.Dxs) { Title = Title };
                db.LastColorIndex = GetColorIndex();
                db.Save();

                DBBTCallNoteTitle2.DeleteMoreThan(ADatabase.Dxs, 20);

                DynamicDBCLCall.NoteText = Title;
                DynamicDBCLCall.NoteFlag = db.LastColorIndex;
                DynamicDBCLCall.NoteWriter = AMembership.ActiveUser.Title;
                DynamicDBCLCall.Save();

                return true;
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowWarning(ex.Message, DynamicOwner);
                return false;
            }
        }



    }
}
