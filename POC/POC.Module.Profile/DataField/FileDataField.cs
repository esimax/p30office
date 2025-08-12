using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Win32;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.DXControls;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.XOffice;

namespace POC.Module.Profile.DataField
{
    class FileDataField : IDataField
    {
        public string Title
        {
            get { return "فایل"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.File; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UIFile.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;
            var dynamicOwner = ((object[])tag)[0] as Window;

            dbpv.Double1NP = dbpv.Double1; 
            dbpv.String1NP = dbpv.String1; 
            dbpv.String2NP = dbpv.String2; 
            dbpv.Int1NP = 0;

            var rv = new StackPanel();

            var beFileName = new ButtonEdit
            {
                IsTextEditable = false,
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = dbpv.String1,
                AllowDefaultButton = false,
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };
            rv.Children.Add(beFileName);

            beFileName.MouseDoubleClick +=
                (s, e) =>
                {
                    try
                    {
                        var dbfile = DBCTBytes.FindByOid(dbpv.Session, dbpv.Guid1);
                        if (dbfile == null) return;
                        if (dbfile.DataByte == null) return;
                        if (dbfile.DataByte.Length == 0) return;

                        var dbc = (DBCTContact)ServiceLocator.Current.GetInstance<IPOCContactModule>().SelectedContact;
                        var path = string.Format("{0}\\{1}", ConstantGeneral.PathFileTemp, dbc.Code);
                        var fn = Path.Combine(path, beFileName.Text);
                        Directory.CreateDirectory(path);
                        if (File.Exists(fn))
                            File.Delete(fn);

                        using (var f = File.Create(fn))
                        {
                            f.Write(dbfile.DataByte, 0, dbfile.DataByte.Length);
                        }

                        System.Diagnostics.Process.Start(fn);
                    }
                    catch (Exception ex)
                    {
                        var aLogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
                        aLogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                        POLMessageBox.ShowError(ex.Message, dynamicOwner);
                    }
                };

            var biOpen = new ButtonInfo
            {
                GlyphKind = GlyphKind.Regular,
                ToolTip = "بارگذاری فایل",
                IsEnabled = !isReadOnly,
            };
            beFileName.Buttons.Add(biOpen);
            biOpen.Click += (s, e)
                =>
                {
                    var sf = new OpenFileDialog
                    {
                        CheckFileExists = true,
                        CheckPathExists = true,
                        Filter = "All Files (*.*)|*.*",
                        FilterIndex = 0,
                        RestoreDirectory = true,
                    };
                    if (sf.ShowDialog() != true) return;
                    var fi = new FileInfo(sf.FileName);
                    if (dbpv.ProfileItem.Int1 != 0)
                    {
                        var limit = (long)dbpv.ProfileItem.Int2 * (dbpv.ProfileItem.Int1 == 1 ? 1024 : 1024 * 1024);
                        if (fi.Length > limit)
                        {
                            POLMessageBox.ShowError(
                                string.Format("حجم فایل بیش از حد مجاز می باشد.{0}حد مجاز: {1}{0}اندازه فایل : {2}",
                                Environment.NewLine,
                                HelperConvert.ConvertToFileSizeFormat(limit),
                                HelperConvert.ConvertToFileSizeFormat(fi.Length)), dynamicOwner);
                            return;
                        }
                    }

                    try
                    {
                        using (var f = fi.OpenRead())
                        {
                            dbpv.Byte1NP = new byte[f.Length];
                            f.Read(dbpv.Byte1NP, 0, (int)f.Length);
                        }
                        dbpv.String1NP = Path.GetFileName(sf.FileName);
                        dbpv.Double1NP = fi.Length;

                        dbpv.Int1NP = 1;
                        beFileName.EditValue = dbpv.String1NP;
                        if (updateSaveStatus != null)
                            updateSaveStatus(dbpv, ((dbpv.String1NP != dbpv.String1) || (Math.Abs(dbpv.Double1NP - dbpv.Double1) >= 1)));
                    }
                    catch (Exception ex)
                    {
                        var aLogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
                        aLogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                        POLMessageBox.ShowError(ex.Message, dynamicOwner);
                    }
                };

            var biSave = new ButtonInfo
            {
                GlyphKind = GlyphKind.Down,
                ToolTip = "ذخیره فایل" + (dbpv.Double1 > 0 ? string.Format("، حجم : {0}", HelperConvert.ConvertToFileSizeFormat(Convert.ToDecimal(dbpv.Double1))) : string.Empty),
                IsEnabled = !isReadOnly,
            };
            beFileName.Buttons.Add(biSave);

            biSave.Click +=
                (s, e) =>
                {
                    var dbfile = DBCTBytes.FindByOid(dbpv.Session, dbpv.Guid1);
                    if (dbfile == null) return;
                    if (dbfile.DataByte == null) return;
                    if (dbfile.DataByte.Length == 0) return;

                    var sf = new SaveFileDialog
                    {
                        CheckFileExists = false,
                        CheckPathExists = true,
                        Filter = "All Files (*.*)|*.*",
                        FilterIndex = 0,
                        RestoreDirectory = true,
                        FileName = dbpv.String1,
                    };
                    if (sf.ShowDialog() != true) return;
                    try
                    {
                        using (var f = File.Create(sf.FileName))
                        {
                            f.Write(dbfile.DataByte, 0, dbfile.DataByte.Length);
                        }
                        POLMessageBox.ShowInformation("فایل ذخیره شد.", dynamicOwner);
                    }
                    catch (Exception ex)
                    {
                        var aLogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
                        aLogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                        POLMessageBox.ShowError(ex.Message, dynamicOwner);
                    }
                };


            var teNote = new TextEdit
            {
                NullText = "توضیحات ...",
                Margin = new Thickness(0, 6, 0, 0),
                EditValue = dbpv.String2,
                Tag = dbpv,
                MaxLength = 128,
                IsReadOnly = isReadOnly,
            };
            rv.Children.Add(teNote);

            teNote.EditValueChanged += (s, e) =>
            {
                var te = (TextEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                var newVal = te.EditValue == null ? null : te.EditValue.ToString();
                db.String2NP = newVal;
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.String2NP != db.String2);
            };


            var biClear = new ButtonInfo
            {
                GlyphKind = GlyphKind.Cancel,
                ToolTip = "حذف اطلاعات فایل",
                IsEnabled = !isReadOnly,
            };
            beFileName.Buttons.Add(biClear);
            biClear.Click +=
                (s, e) =>
                {
                    beFileName.EditValue = null;
                    teNote.EditValue = null;

                    dbpv.String1NP = null;
                    dbpv.String2NP = null;
                    dbpv.Double1NP = 0;
                    dbpv.Byte1NP = null;
                    dbpv.Int1NP = 1;

                    if (updateSaveStatus != null)
                        updateSaveStatus(dbpv, ((dbpv.String1NP != dbpv.String1) || (Math.Abs(dbpv.Double1NP - dbpv.Double1) > 0.1)));
                };
            return rv;
        }




















        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIFile(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            return;
        }

        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.Double1 = db.Double1NP; 
            db.String1 = db.String1NP; 
            db.String2 = db.String2NP; 

            if (db.Int1NP == 1)
            {
                var dbpb = DBCTBytes.FindByOid(db.Session, db.Guid1) ?? new DBCTBytes(db.Session);
                dbpb.DataByte = db.Byte1NP;
                dbpb.Contact = db.Contact;
                dbpb.ByteDataType = EnumByteDataType.ProfileFile;
                dbpb.Save();

                db.Guid1 = dbpb.Oid;
            }
            db.Save();
        }

        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.FileSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            var dbi = dbItem as DBCTProfileItem;
            if (dbi == null) return string.Empty;
            if (dbv.Int1 == 0) return st.DateSettings.NullValue;


            return string.Format(st.FileSettings.Format, dbv.String1, dbv.String2, dbv.Double1);
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return db.Double1NP >= 0 && !string.IsNullOrWhiteSpace(db.String1NP);
        }
    }
}
