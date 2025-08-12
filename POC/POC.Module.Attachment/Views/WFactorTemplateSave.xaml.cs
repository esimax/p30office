using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.AC;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POC.Module.Attachment.Views
{
    public partial class WFactorTemplateSave : Window
    {
        private IDatabase ADatabase { get; set; }
        public List<Tuple<string, string>> Items { get; set; }

        public DBACFactorTemplate SelectedItem { get; set; }
        public WFactorTemplateSave()
        {
            InitializeComponent();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            Loaded += (s, e) =>
            {
                LoadItems();


                #region btOk
                btOk.Click += (s1, e1) =>
                       {
                           if (string.IsNullOrWhiteSpace(textTitle.Text))
                               return;
                           var vv = textTitle.Text;
                           if (vv.Contains("*") || vv.Contains("|") || vv.Contains(";"))
                           {
                               MessageBox.Show("لطفا از علائم زیر استفاده نکنید:\n ; | *");
                               return;
                           }
                           using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                           {
                               var db = DBACFactorTemplate.FindDuplicateTitleExcept(uow, null, textTitle.Text);
                               if (db != null)
                               {
                                   MessageBox.Show("امکان ثبت عنوان تکراری وجود ندارد.");
                                   return;
                               }
                               db = new DBACFactorTemplate(uow)
                               {
                                   Title = HelperConvert.CorrectPersianBug(textTitle.Text)
                               };

                               foreach (var t in Items)
                               {
                                   db.Value += $"{t.Item1};{t.Item2}|";
                               }
                               try
                               {
                                   db.Save();
                                   uow.CommitChanges();
                               }
                               catch
                               {
                               }

                               Close();
                           }
                       };
                #endregion
                #region btDelete
                btDelete.Click += (s1, e1) =>
                        {
                            if (listviewAll.SelectedItem == null)
                                return;

                            using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                            {
                                var t = HelperConvert.CorrectPersianBug(listviewAll.SelectedItem.ToString()) ?? "";
                                var db = new XPQuery<DBACFactorTemplate>(uow).FirstOrDefault(n => n.Title == t);//  DBACFactorTemplate.FindDuplicateTitleExcept(dxs, null, textTitle.Text);
                                if (db != null)
                                {
                                    try
                                    {
                                        db.Delete();
                                        db.Save();
                                        uow.CommitChanges();
                                    }
                                    catch
                                    {
                                    }
                                    LoadItems();
                                }
                            }
                        };
                #endregion
                #region btSelect
                btSelect.Click += (s1, e1) =>
                        {
                            if (listviewAll.SelectedItem == null)
                                return;
                            using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                            {
                                var t = HelperConvert.CorrectPersianBug(listviewAll.SelectedItem.ToString()) ?? "";
                                var db = new XPQuery<DBACFactorTemplate>(uow).FirstOrDefault(n => n.Title == t);
                                SelectedItem = db;
                                if (db == null)
                                    return;
                                Close();
                            }
                        };
                #endregion
                #region btDefault
                btDefault.Click += (s1, e1) =>
                {
                    if (listviewAll.SelectedItem == null)
                        return;

                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        var t = HelperConvert.CorrectPersianBug(listviewAll.SelectedItem.ToString()) ?? "";
                        var db = new XPQuery<DBACFactorTemplate>(uow).FirstOrDefault(n => n.Title == t);//  DBACFactorTemplate.FindDuplicateTitleExcept(dxs, null, textTitle.Text);
                        if (db != null)
                        {
                            try
                            {
                                if (!db.Title.StartsWith("* "))
                                {
                                    var q = new XPQuery<DBACFactorTemplate>(uow).ToList();
                                    foreach (var dbacFactorTemplate in q)
                                    {
                                        if (dbacFactorTemplate.Title.StartsWith("* "))
                                        {
                                            dbacFactorTemplate.Title = dbacFactorTemplate.Title.Substring(2);
                                            dbacFactorTemplate.Save();
                                        }
                                    }
                                }
                                db.Title = "* " + db.Title;
                                db.Save();
                                uow.CommitChanges();
                            }
                            catch
                            {
                            }
                            LoadItems();
                        }
                    }
                };
                #endregion
            };
        }

        private void LoadItems()
        {
            using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
            {
                listviewAll.Items.Clear();
                var q = new XPQuery<DBACFactorTemplate>(uow).OrderBy(n => n.Title).Select(n => n.Title).ToList();
                foreach (var item in q)
                {
                    listviewAll.Items.Add(item);
                }
            }
        }
    }
}
