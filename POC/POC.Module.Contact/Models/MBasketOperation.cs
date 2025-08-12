using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Contact.Models
{
    public class MBasketOperation : NotifyObjectBase, IRequestCloseViewModel
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        private Window Owner { get; set; }
        private DBCTContactSelection Data { get; set; }
        private EnumBoolOperationType OperationType { get; set; }


        #region CTOR
        public MBasketOperation(Window owner, DBCTContactSelection currentBasket, EnumBoolOperationType operationType)
        {
            Owner = owner;
            Data = currentBasket;
            OperationType = operationType;


            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

            PopulateSelectionBasket();
            InitCommands();

            switch (operationType)
            {
                case EnumBoolOperationType.And:
                    OperationText = "اشتراك سبد اول و دوم";
                    break;
                case EnumBoolOperationType.Or:
                    OperationText = "اجتماع سبد اول و دوم";
                    break;
                case EnumBoolOperationType.Minus:
                    OperationText = "اختلاف سبد دوم از اول";
                    break;
            }
            if (currentBasket != null)
                SelectedBasketA = currentBasket;
        }
        #endregion

        #region WindowTitle
        public string WindowTitle
        {
            get { return "عملیات بر روی سبد انتخاب"; }
        }
        #endregion



        #region SelectionBasketList
        private XPCollection<DBCTContactSelection> _SelectionBasketList;
        public XPCollection<DBCTContactSelection> SelectionBasketList
        {
            get
            {
                return _SelectionBasketList;
            }
            set
            {
                if (_SelectionBasketList == value)
                    return;
                _SelectionBasketList = value;
                RaisePropertyChanged("SelectionBasketList");
            }
        }
        #endregion
        #region SelectedBasketA
        private DBCTContactSelection _SelectedBasketA;
        public DBCTContactSelection SelectedBasketA
        {
            get
            {
                return _SelectedBasketA;
            }
            set
            {
                if (ReferenceEquals(_SelectedBasketA, value)) return;
                _SelectedBasketA = value;
                RaisePropertyChanged("SelectedBasketA");
            }
        }
        #endregion
        #region SelectedBasketB
        private DBCTContactSelection _SelectedBasketB;
        public DBCTContactSelection SelectedBasketB
        {
            get
            {
                return _SelectedBasketB;
            }
            set
            {
                if (ReferenceEquals(_SelectedBasketB, value)) return;
                _SelectedBasketB = value;
                RaisePropertyChanged("SelectedBasketB");
            }
        }
        #endregion
        #region SelectedBasketC
        private DBCTContactSelection _SelectedBasketC;
        public DBCTContactSelection SelectedBasketC
        {
            get
            {
                return _SelectedBasketC;
            }
            set
            {
                if (ReferenceEquals(_SelectedBasketC, value)) return;
                _SelectedBasketC = value;
                RaisePropertyChanged("SelectedBasketC");
            }
        }
        #endregion
        #region OperationText
        private string _OperationText;
        public string OperationText
        {
            get { return _OperationText; }
            set
            {
                _OperationText = value;
                RaisePropertyChanged("OperationText");
            }
        }
        #endregion


        #region [METHODS]
        private void PopulateSelectionBasket()
        {
            if (!AMembership.IsAuthorized) return;
            SelectionBasketList = DBCTContactSelection.GetByUser(ADatabase.Dxs, AMembership.ActiveUser.UserID, false, null);
            SelectionBasketList.Sorting = new SortingCollection(new SortProperty("Title", SortingDirection.Ascending));
        }
        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (Validate())
                        if (Save())
                            RaiseRequestClose(true);
                }, () => SelectedBasketA != null && SelectedBasketB != null && SelectedBasketC != null && !ReferenceEquals(SelectedBasketA, SelectedBasketB));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp27 != "");
        }
        private bool Validate()
        {
            return true;
        }
        private bool Save()
        {
            var rv = false;
            SelectedBasketA.Reload();
            SelectedBasketA.Contacts.Reload();
            SelectedBasketB.Reload();
            SelectedBasketB.Contacts.Reload();

            POLProgressBox.Show(3,
                w =>
                {
                    try
                    {


                        var qa = from n in SelectedBasketA.Contacts select n.Oid;
                        w.AsyncSetText(1, "تعداد سبد اول : " + qa.Count());
                        var qb = from n in SelectedBasketB.Contacts select n.Oid;
                        w.AsyncSetText(2, "تعداد سبد دوم : " + qb.Count());

                        IEnumerable<Guid> qc = null;
                        switch (OperationType)
                        {
                            case EnumBoolOperationType.And:
                                qc = qa.Intersect(qb);
                                break;
                            case EnumBoolOperationType.Or:
                                qc = qa.Union(qb);
                                break;
                            case EnumBoolOperationType.Minus:
                                qc = qa.Except(qb);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        w.AsyncSetText(3, "تعداد سبد نتیجه : " + qc.Count());

                        var list = qc.ToList();

                        var dxs = ADatabase.GetNewSession();
                        var result = DBCTContactSelection.FindByOid(dxs, SelectedBasketC.Oid);

                        var clearList = (from n in result.Contacts select n).ToList();
                        foreach (var dbc in clearList)
                        {
                            result.Contacts.Remove(dbc);
                        }
                        if (clearList.Count > 0)
                        {
                            result.Save();
                        }

                        foreach (var db in list.Select(guid => DBCTContact.FindByOid(dxs, guid)).Where(db => db != null))
                        {
                            result.Contacts.Add(db);
                        }
                        result.Save();
                        rv = true;
                    }
                    catch (Exception ex)
                    {
                        ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                        POLMessageBox.ShowWarning(ex.Message, Owner);
                    }
                },
                w =>
                {
                    POLMessageBox.ShowInformation("عملیات انجام شد.", Owner);
                },
                APOCMainWindow.GetWindow());
            return rv;
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp27);
        }

        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCancel { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IRequestCloseViewModel
        public event EventHandler<RequestCloseEventArgs> RequestClose;
        private void RaiseRequestClose(bool? dialogResult)
        {
            if (RequestClose != null)
                RequestClose.Invoke(this, new RequestCloseEventArgs { DialogResult = dialogResult });
        }
        #endregion
    }
}
