using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;

namespace POC.Module.Call.Views.DashboardUnits
{
    public partial class UCategoryStat : UserControl
    {
        private IDatabase ADatabase { get; set; }
        private ICacheData ACacheData { get; set; }

        public UCategoryStat()
        {
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            InitializeComponent();
            InitValues();

            #region bPopulate.Click
            bPopulate.Click +=
                    (s, e) =>
                    {
                        if (!poldeStart.DateEditValue.HasValue) return;
                        if (!poldeEnd.DateEditValue.HasValue) return;

                        var datasource = new List<CallStat>();
                        var cats = ACacheData.GetContactCatList();
                        foreach (var cat in cats)
                        {
                            var go = new GroupOperator();
                            go.Operands.Add(new BinaryOperator("CallDate", poldeStart.DateEditValue.Value, BinaryOperatorType.GreaterOrEqual));
                            go.Operands.Add(new BinaryOperator("CallDate", poldeEnd.DateEditValue.Value.AddDays(1), BinaryOperatorType.Less));
                            go.Operands.Add(CriteriaOperator.Parse("[Contact.Categories][Oid == {" + ((DBCTContactCat)cat.Tag).Oid + "}]"));

                            var xpc = new XPCollection<DBCLCall>(ADatabase.Dxs, go, null);

                            var item = new CallStat
                            {
                                Category = cat.Title,

                                CountCallIn = xpc.Where(n => n.CallType == EnumCallType.CallIn).Count(),
                                CountCallOut = xpc.Where(n => n.CallType == EnumCallType.CallOut).Count(),
                                CountCallAll = xpc.Count(),

                                DurationCallIn = xpc.Where(n => n.CallType == EnumCallType.CallIn).Sum(n => n.DurationSeconds),
                                DurationCallOut = xpc.Where(n => n.CallType == EnumCallType.CallOut).Sum(n => n.DurationSeconds),
                                DurationCallAll = xpc.Sum(n => n.DurationSeconds),
                            };
                            datasource.Add(item);
                        }
                        {
                            var go = new GroupOperator();
                            go.Operands.Add(new BinaryOperator("CallDate", poldeStart.DateEditValue.Value, BinaryOperatorType.GreaterOrEqual));
                            go.Operands.Add(new BinaryOperator("CallDate", poldeEnd.DateEditValue.Value.AddDays(1), BinaryOperatorType.Less));

                            var xpc = new XPCollection<DBCLCall>(ADatabase.Dxs, go, null);

                            var item = new CallStat
                            {
                                Category = "{همه}",

                                CountCallIn = xpc.Where(n => n.CallType == EnumCallType.CallIn).Count(),
                                CountCallOut = xpc.Where(n => n.CallType == EnumCallType.CallOut).Count(),
                                CountCallAll = xpc.Count(),

                                DurationCallIn = xpc.Where(n => n.CallType == EnumCallType.CallIn).Sum(n => n.DurationSeconds),
                                DurationCallOut = xpc.Where(n => n.CallType == EnumCallType.CallOut).Sum(n => n.DurationSeconds),
                                DurationCallAll = xpc.Sum(n => n.DurationSeconds),
                            };
                            datasource.Add(item);
                        }
                        TheGrid.ItemsSource = datasource;
                        ((TableView)(TheGrid.View)).BestFitColumns();
                    }; 
            #endregion

            bPrint.Click += (s, e) =>
            {
                ((TableView)(TheGrid.View)).ShowPrintPreviewDialog(Window.GetWindow(this));
            };
        }

        private void InitValues()
        {
            var db1 = DBCLCall.GetFirstCall(ADatabase.Dxs);
            var db2 = DBCLCall.GetLastCall(ADatabase.Dxs);
            if (db1 == null)
                poldeStart.DateEditValue = DateTime.Now.Date;
            else
                poldeStart.DateEditValue = db1.CallDate.HasValue ? db1.CallDate.Value.Date : DateTime.Now.Date;
            if (db2 == null)
                poldeEnd.DateEditValue = DateTime.Now.Date;
            else
                poldeEnd.DateEditValue = db2.CallDate.HasValue ? db2.CallDate.Value.Date : DateTime.Now.Date;
        }
    }
}
