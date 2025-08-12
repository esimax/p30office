using System;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.SMS.Models
{
    internal class MTextStaticAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBSMTextStatic DynamicSelectedData { get; set; }

        #region CTOR
        public MTextStaticAddEdit(object mainView)
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
            get { return "متن پیامك"; }
        }
        #endregion


        #region TextFlowDirection
        private FlowDirection _TextFlowDirection;
        public FlowDirection TextFlowDirection
        {
            get { return _TextFlowDirection; }
            set
            {
                if (value == _TextFlowDirection)
                    return;

                _TextFlowDirection = value;
                RaisePropertyChanged("TextFlowDirection");
            }
        }
        #endregion
        #region TextEntered
        private string _TextEntered;
        public string TextEntered
        {
            get { return _TextEntered; }
            set
            {
                if (value == _TextEntered)
                    return;

                _TextEntered = value;
                RaisePropertyChanged("TextEntered");
                RaisePropertyChanged("TextEnteredCountChar");
                RaisePropertyChanged("TextEnteredCountSMS");

            }
        }
        #endregion
        #region TextEnteredCountChar
        private int _TextEnteredCountChar;
        public string TextEnteredCountChar
        {
            get
            {
                _TextEnteredCountChar = string.IsNullOrEmpty(_TextEntered)
                                            ? 0
                                            : _TextEntered.Length * (_TextEntered.ContainsUnicodeCharacter() ? 2 : 1);
                return string.Format("كاراكتر : {0}", _TextEnteredCountChar);
            }
        }
        #endregion
        #region TextEnteredCountSMS
        public string TextEnteredCountSMS
        {
            get { return TextEnteredCountChar != null ? string.Format("صفحه : {0}", ((_TextEnteredCountChar / 160) + 1)) : ""; }
        }
        #endregion






        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => !string.IsNullOrWhiteSpace(TextEntered));
            CommandTextRTL = new RelayCommand(() => { TextFlowDirection = FlowDirection.RightToLeft; HelperLocalize.SetLanguageToRTL(); });
            CommandTextLTR = new RelayCommand(() => { TextFlowDirection = FlowDirection.LeftToRight; HelperLocalize.SetLanguageToLTR(); });
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
            if (string.IsNullOrWhiteSpace(TextEntered)) TextEntered = string.Empty;
            if (string.IsNullOrWhiteSpace(TextEntered.Trim()))
            {
                POLMessageBox.ShowError("متن پیامك معتبر نمی باشد. امكان ثبت وجود ندارد.", DynamicOwner);
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
                        DynamicSelectedData = new DBSMTextStatic(uow)
                        {
                            Body = TextEntered,
                            IsRTL = TextFlowDirection == FlowDirection.RightToLeft,
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBSMTextStatic.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Body = TextEntered;
                    DynamicSelectedData.IsRTL = TextFlowDirection == FlowDirection.RightToLeft;
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
            TextFlowDirection = HelperLocalize.ApplicationFlowDirection;
            TextEntered = string.Empty;

            if (DynamicSelectedData == null) return;
            TextEntered = DynamicSelectedData.Body;
            if (DynamicSelectedData.IsRTL)
                CommandTextRTL.Execute(null);
            else
                CommandTextLTR.Execute(null);
        }
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandTextRTL { get; set; }
        public RelayCommand CommandTextLTR { get; set; }
        #endregion
    }
}
