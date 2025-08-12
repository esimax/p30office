using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
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
    public class MContactToBasket : NotifyObjectBase, IRequestCloseViewModel
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IMembership AMembership { get; set; }

        private Window Owner { get; set; }
        private DBCTContactSelection Data { get; set; }
        private CriteriaOperator Criteria { get; set; }
        private List<Guid> GuidList { get; set; }

        #region CTOR
        public MContactToBasket(Window owner, DBCTContactSelection data, CriteriaOperator co, List<Guid> guids)
        {
            Owner = owner;
            Data = data;
            Criteria = co;
            GuidList = guids;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            PopulateBasketList();
            InitCommands();
        }

        #endregion

        #region WindowTitle
        public string WindowTitle
        {
            get { return "اضافه كردن به سبد انتخاب"; }
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
        #region SelectedBasket
        private DBCTContactSelection _SelectedBasket;
        public DBCTContactSelection SelectedBasket
        {
            get
            {
                return _SelectedBasket;
            }
            set
            {
                if (ReferenceEquals(_SelectedBasket, value)) return;
                _SelectedBasket = value;
                RaisePropertyChanged("SelectedBasket");
            }
        }
        #endregion
        #region ClearOldData
        private bool _ClearOldData;
        public bool ClearOldData
        {
            get
            {
                return _ClearOldData;
            }
            set
            {
                if (_ClearOldData == value)
                    return;
                _ClearOldData = value;
                RaisePropertyChanged("ClearOldData");
            }
        }
        #endregion


        #region [METHODS]
        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (BeginTransfer())
                        RaiseRequestClose(true);
                }, () => SelectedBasket != null);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp34 != "");
        }
        private bool BeginTransfer()
        {
            var ClearCount = 0;
            var AddedCount = 0;
            var rv = false;
            POLProgressBox.Show(3,
                w =>
                {
                    try
                    {
                        var dxs = ADatabase.GetNewSession();
                        var bs = DBCTContactSelection.FindByOid(dxs, SelectedBasket.Oid);

                        w.AsyncSetText(1, "بررسی اطلاعات ...");

                        if (ClearOldData)
                        {
                            var xpcdel = new XPCollection<DBCTContact>(dxs)
                                             {
                                                 Criteria =
                                                     new ContainsOperator("Selections",
                                                                          new BinaryOperator("Oid", SelectedBasket.Oid))
                                             };
                            xpcdel.Load();

                            w.AsyncSetMax(xpcdel.Count);
                            w.AsyncSetText(1, "تخلیه اطلاعات قبلی ...");
                            w.AsyncSetText(2, String.Format("تعداد : {0}", xpcdel.Count));
                            w.AsyncEnableCancel();

                            for (var i = 0; i < xpcdel.Count; i++)
                            {
                                if (w.NeedToCancel)
                                    return;
                                var c = xpcdel[i];
                                bs.Contacts.Remove(c);
                                w.AsyncSetText(3, c.Title);
                                w.AsyncSetValue(i);
                                ClearCount++;
                            }
                            bs.Save();
                        }


                        w.AsyncDisableCancel();
                        w.AsyncSetMax(0);
                        w.AsyncSetText(1, "بررسی اطلاعات ...");
                        w.AsyncSetText(2, "");
                        w.AsyncSetText(3, "");


                        var xpc = new XPCollection<DBCTContact>(dxs);
                        if (!ReferenceEquals(Criteria, null))
                        {
                            xpc.Criteria = Criteria;
                        }
                        else if (GuidList != null)
                        {
                            xpc.Criteria = new InOperator("Oid", GuidList);
                        }
                        xpc.Load();
                        var count = xpc.Count;

                        w.AsyncSetMax(xpc.Count);
                        w.AsyncSetText(1, "در حال انتفال ...");
                        w.AsyncSetText(2, String.Format("تعداد : {0}", count));
                        w.AsyncEnableCancel();
                        for (var i = 0; i < count; i++)
                        {
                            if (w.NeedToCancel)
                                return;
                            var c = xpc[i];
                            bs.Contacts.Add(c);
                            w.AsyncSetText(3, c.Title);
                            w.AsyncSetValue(i);
                            AddedCount++;
                        }
                        bs.Save();
                    }
                    catch (Exception ex)
                    {
                        ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                        POLMessageBox.ShowWarning("بروز خطا در اجرای عملیات.", w);
                        w.NeedToCancel = true;
                    }
                },
                w =>
                {
                    if (w.NeedToCancel)
                        POLMessageBox.ShowWarning(
                        string.Format("{3}{0}{0}تعداد تخلیه شده : {1}{0}تعداد اضافه شده : {2}",
                        Environment.NewLine,
                        ClearCount.ToString(CultureInfo.InvariantCulture),
                        AddedCount.ToString(CultureInfo.InvariantCulture),
                        w.NeedToCancel ? "عملیات توسط كاربر لغو شد." : "عملیات به اتمام رسید."), w);

                    else
                        POLMessageBox.ShowInformation(
                        string.Format("{3}{0}{0}تعداد تخلیه شده : {1}{0}تعداد اضافه شده : {2}",
                        Environment.NewLine,
                        ClearCount.ToString(CultureInfo.InvariantCulture),
                        AddedCount.ToString(CultureInfo.InvariantCulture),
                        w.NeedToCancel ? "عملیات توسط كاربر لغو شد." : "عملیات به اتمام رسید."), w);

                    rv = !w.NeedToCancel;
                }, Owner);
            return rv;
        }

        private void PopulateBasketList()
        {
            SelectionBasketList = DBCTContactSelection.GetByUser(ADatabase.Dxs, AMembership.ActiveUser.UserID, true, Data);
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp34);
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

