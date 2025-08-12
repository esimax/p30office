using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using Microsoft.Practices.ServiceLocation;

namespace POC.Module.Profile.DataField
{
    class LocationDataField : IDataField
    {
        public string Title
        {
            get { return "موقعیت جغرافیایی"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.Location; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UILocation.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;
            var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

            var dynamicOwner = ((object[])tag)[0] as Window;

            dbpv.Double1NP = dbpv.Double1;
            dbpv.Double2NP = dbpv.Double2;
            dbpv.Int1NP = dbpv.Int1;
            dbpv.String1NP = dbpv.String1;



            var rv = new Grid();
            rv.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rv.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            var teLat = new TextEdit
            {
                MaskType = MaskType.Numeric,
                Mask = "n4",
                MaskUseAsDisplayFormat = true,
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = dbpv.Double1,
                Margin = new Thickness(0, 0, 0, 0),
                Tag = dbpv,
                Name = "teLat",
                IsReadOnly = isReadOnly,
            };
            teLat.SetValue(Grid.ColumnProperty, 0);
            teLat.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(teLat);


            var teLong = new TextEdit
            {
                MaskType = MaskType.Numeric,
                Mask = "n4",
                MaskUseAsDisplayFormat = true,
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = dbpv.Double2,
                Margin = new Thickness(6, 0, 6, 0),
                Tag = dbpv,
                Name = "teLong",
                IsReadOnly = isReadOnly,
            };
            teLong.SetValue(Grid.ColumnProperty, 1);
            teLong.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(teLong);


            var but = new Button
            {
                Margin = new Thickness(0, 0, 0, 0),
                Content = "+",
                Tag = dbpv,
                IsEnabled = !isReadOnly,
            };
            but.SetValue(Grid.ColumnProperty, 2);
            but.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(but);

            if (dbpv.ProfileItem.Int1 == 0)
            {
                var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                var dbb = DBCTBytes.FindByOid(aDatabase.Dxs, dbpv.Guid1);
                dbpv.Byte1NP = dbb == null ? null : dbb.DataByte;
                var img = new ImageEdit()
                {
                    Stretch = Stretch.None,
                    SnapsToDevicePixels = true,
                    FlowDirection = FlowDirection.LeftToRight,
                    EditValue = dbb == null ? null : dbb.DataByte,
                    Margin = new Thickness(0, 6, 0, 0),
                    ShowMenu = false,
                    Name = "img",
                    ShowLoadDialogOnClickMode = ShowLoadDialogOnClickMode.Never,
                    IsEnabled = !isReadOnly,
                };
                img.SetValue(Grid.ColumnProperty, 0);
                img.SetValue(Grid.ColumnSpanProperty, 3);
                img.SetValue(Grid.RowProperty, 1);
                rv.Children.Add(img);
            }





            teLat.EditValueChanged += (s, e) =>
            {
                var te = (TextEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                HelperUtils.Try(() =>
                {
                    var newVal = Convert.ToDouble(te.EditValue);
                    db.Double1NP = newVal;
                    if (updateSaveStatus != null)
                        updateSaveStatus(db, Math.Abs(db.Double1NP - db.Double1) > 0.0000009);
                });
            };
            teLong.EditValueChanged += (s, e) =>
            {
                var te = (TextEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                HelperUtils.Try(() =>
                {
                    var newVal = Convert.ToDouble(te.EditValue);
                    db.Double2NP = newVal;
                    if (updateSaveStatus != null)
                        updateSaveStatus(db, Math.Abs(db.Double2NP - db.Double2) > 0.0000009);
                });
            };


            but.Click += (s, e)
                =>
            {
                var te = (Button)s;
                var db = (DBCTProfileValue)te.Tag;
                var mli = aPOCMainWindow.ShowSelectPointOnMap(dynamicOwner,
                    new MapLocationItem
                    {
                        Lat = db.Double1NP,
                        Lon = db.Double2NP,
                        ZoomLevel = db.Int1NP,
                    });
                if (mli == null) return;

                var gridUI = (Grid)te.Parent;
                var latUI = (from n in gridUI.Children.Cast<FrameworkElement>() where n.Name == "teLat" select n as TextEdit).First();
                latUI.EditValue = mli.Lat;
                db.Double1NP = mli.Lat;

                var longUI = (from n in gridUI.Children.Cast<FrameworkElement>() where n.Name == "teLong" select n as TextEdit).First();
                longUI.EditValue = mli.Lon;
                db.Double2NP = mli.Lon;

                var bs1 = HelperImage.ConvertImageByteToBitmapImage(mli.ImageArray);

                var bs2 = HelperLocalize.ApplicationFlowDirection == FlowDirection.RightToLeft
                              ? HelperImage.RTLTransform(bs1, -1, 320)
                              : bs1;

                var img = (from n in gridUI.Children.Cast<FrameworkElement>() where n.Name == "img" select n as ImageEdit).FirstOrDefault();
                if (img != null)
                    img.EditValue = bs2.Clone();
                db.Byte1NP = HelperImage.ConvertBitmapSourceToByteArrayAsJPG(bs2);
                db.Int1NP = mli.ZoomLevel;

                if (updateSaveStatus != null)
                    updateSaveStatus(db, (Math.Abs(db.Double1NP - db.Double1) > 0.0000009) || (Math.Abs(db.Double2NP - db.Double2) > 0.0000009) || db.Int1 != db.Int1NP);
            };
            return rv;
        }

























        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UILocation(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            var dbv = dbValue as DBCTProfileValue;
            var dbi = dbItem as DBCTProfileItem;

            if (dbv == null) return;
            if (dbi == null) return;

            HelperUtils.Try(
                    () =>
                    {
                        var dv = dbi.DefaultValue;
                        var ss = dv.Split('|');
                        dbv.Double1 = Convert.ToDouble(ss[0]);
                        dbv.Double2 = Convert.ToDouble(ss[1]);
                        dbv.Int1 = Convert.ToInt32(ss[2]);
                    });
        }


        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.Int1 = db.Int1NP;
            db.Double1 = db.Double1NP;
            db.Double2 = db.Double2NP;
            db.String1 = db.String1NP;

            if (db.ProfileItem.Int1 == 0)
            {
                var dbpb = DBCTBytes.FindByOid(db.Session, db.Guid1) ?? new DBCTBytes(db.Session);
                dbpb.DataByte = db.Byte1NP;
                dbpb.Contact = db.Contact;
                dbpb.ByteDataType = EnumByteDataType.ProfileMap;
                dbpb.Save();
                db.Guid1 = dbpb.Oid;
            }
            db.Save();
        }

        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.LocationSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            var dbi = dbItem as DBCTProfileItem;
            if (dbi == null) return string.Empty;

            return string.Format(st.LocationSettings.Format, dbi.Double1, dbi.Double2, dbi.Int1, dbi.String1);
        }




        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return !Double.IsNaN(db.Double1NP) && !Double.IsNaN(db.Double2NP);
        }
    }
}
