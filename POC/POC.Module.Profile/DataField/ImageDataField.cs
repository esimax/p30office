using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Editors;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.DXControls;
using System.IO;
using POL.Lib.XOffice;

namespace POC.Module.Profile.DataField
{
    class ImageDataField : IDataField
    {
        public string Title
        {
            get { return "تصویر"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.Image; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Standard/16/_16_UIImageEdit.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;
            var dynamicOwner = ((object[])tag)[0] as Window;

            dbpv.Int1NP = dbpv.Int1; 
            dbpv.Int2NP = dbpv.Int2; 
            dbpv.String1NP = dbpv.String1; 
            dbpv.Double1NP = dbpv.Double1; 

            var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            var dbb = DBCTBytes.FindByOid(aDatabase.Dxs, dbpv.Guid1);
            dbpv.Byte1NP = dbb == null ? null : dbb.DataByte;

            var rv = new Grid();
            rv.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rv.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var st = Stretch.None;
            if ((dbpv.Byte1NP == null) && dbpv.Int1 > 320 || dbpv.Int2 > 320)
                st = Stretch.UniformToFill;

            var img = new ImageEdit()
            {
                Stretch = st, 
                SnapsToDevicePixels = true,
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = dbpv.Byte1NP,
                Name = "img",
                Tag = dbpv,
                ToolTip = ((dbpv.Byte1NP == null) ? "بدون اطلاعات" : string.Format("پهنا : {0}{1}ادتفاع : {2}", dbpv.Int1, Environment.NewLine, dbpv.Int2)),
                IsReadOnly = isReadOnly,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
            };

            img.SetValue(Grid.ColumnProperty, 0);
            img.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(img);


            var teNote = new TextEdit
            {
                NullText = "توضیحات ...",
                EditValue = dbpv.String1,
                MaxLength = 128,
                Margin = new Thickness(0, 6, 0, 0),
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };
            teNote.SetValue(Grid.ColumnProperty, 0);
            teNote.SetValue(Grid.RowProperty, 1);
            rv.Children.Add(teNote);



            teNote.EditValueChanged += (s, e) =>
            {
                var te = (TextEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                var newVal = te.EditValue == null ? null : te.EditValue.ToString();
                db.String1NP = newVal;
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.String1NP != db.String1);
            };
            img.EditValueChanging += (s, e) =>
            {
                var te = (ImageEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                if (e.NewValue is BitmapSource || e.NewValue is byte[])
                {

                    var bs = e.NewValue is BitmapSource ? e.NewValue as BitmapSource : HelperImage.ConvertImageByteToBitmapImage(e.NewValue as byte[]);
                    var w = (int)bs.Width;
                    var h = (int)bs.Height;
                    if ((db.ProfileItem.Int1 > 0 && w > db.ProfileItem.Int1) || (db.ProfileItem.Int2 > 0 && h > db.ProfileItem.Int2))
                    {
                        POLMessageBox.ShowError(
                            string.Format("اندازه تصویر بیشتر از حد مجاز می باشد.{0}پهنای مجاز : {1}{0}ارتفاع مجاز : {2}",
                            Environment.NewLine,
                            (db.ProfileItem.Int1 > 0 ? db.ProfileItem.Int1.ToString() : "نامحدود"),
                            (db.ProfileItem.Int2 > 0 ? db.ProfileItem.Int2.ToString() : "نامحدود")),
                            dynamicOwner);
                        e.IsCancel = true;
                        e.Handled = true;
                        return;
                    }
                    db.Int1NP = w;
                    db.Int2NP = h;
                    db.Byte1NP = HelperImage.ConvertBitmapSourceToByteArrayAsPNG(bs);
                    db.Double1NP = db.Byte1NP.Length;
                }
                else
                {
                    db.Byte1NP = null;
                    db.Double1NP = 0;
                    db.Int1NP = 0;
                    db.Int2NP = 0;
                }
                if (updateSaveStatus != null)
                    updateSaveStatus(db, ((Math.Abs(db.Double1NP - db.Double1) > 0.0000009) || (db.Int1NP != db.Int1) || (db.Int2NP != db.Int2)));
            };
            img.MouseDoubleClick +=
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
                        var fn = Path.Combine(path, string.Format("{0}.png", dbfile.Oid.ToString()));
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
            return rv;
        }
















        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIImage(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            return;
        }


        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.Int1 = db.Int1NP;
            db.Int2 = db.Int2NP;
            db.Double1 = db.Double1NP;
            db.String1 = db.String1NP;

            var dbpb = DBCTBytes.FindByOid(db.Session, db.Guid1) ?? new DBCTBytes(db.Session);
            dbpb.DataByte = db.Byte1NP;
            dbpb.Contact = db.Contact;
            dbpb.ByteDataType = EnumByteDataType.ProfileImage;
            dbpb.Save();

            db.Guid1 = dbpb.Oid;
            db.Save();
        }

        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.ImageSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            var dbi = dbItem as DBCTProfileItem;
            if (dbi == null) return string.Empty;
            if (dbv.Int1 == 0) return st.DateSettings.NullValue;

            return string.Format(st.ImageSettings.Format,
                dbv.Int1,
                dbv.Int2,
                dbv.String1,
                dbv.String2,
                dbv.Double1,
                dbi.Int1,
                dbi.Int2);
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return db.Int1NP > 0 && db.Int2NP > 0;
        }
    }
}
