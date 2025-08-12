using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using Telerik.Windows.Controls.Map;
using POL.Lib.XOffice;


namespace POC.Module.Map.Views
{
    public partial class WSelectPointOnMap : DXWindow
    {
        private MapLocationItem InData { get; set; }


        public WSelectPointOnMap(MapLocationItem initData)
        {
            InitializeComponent();
            InData = initData;

            DataContext = this;

            InitCommand();
            Loaded += (s, e) =>
            {
                var path = HelperSettingsClient.MapCachPath;
                if (string.IsNullOrWhiteSpace(path))
                    path = ConstantGeneral.PathMapCache;
                map.SetCachPath(path);
                if (InData != null)
                {
                    map.SelectedPointLocation = new Location(InData.Lat, InData.Lon);
                    map.ZoomLevel = InData.ZoomLevel;
                    map.SetCenter(map.SelectedPointLocation);
                }
            };
        }

        private void InitCommand()
        {
            CommandOK = new RelayCommand(OK, () => map.SelectedPointLocation != Location.Empty);
        }

        private void OK()
        {
            if (map.SelectedPointLocation == Location.Empty)
            {
                ResultData = null;
                DialogResult = false;
                Close();
            }
            else
            {
                ResultData = new MapLocationItem
                                 {
                                     Lat = map.SelectedPointLocation.Latitude,
                                     Lon = map.SelectedPointLocation.Longitude,
                                     ZoomLevel = map.ZoomLevel,
                                 };
                map.ZoomBarVisibility = Visibility.Collapsed;
                map.MouseLocationIndicatorVisibility = Visibility.Collapsed;
                map.ScaleVisibility = Visibility.Collapsed;
                map.ZoomBarPresetsVisibility = Visibility.Collapsed;
                map.CommandBarVisibility = Visibility.Collapsed;
                map.NavigationVisibility = Visibility.Collapsed;

                var rt = HelperImage.GetImageFromUI(map);
                var y = (rt.Height - 240) / 2;
                var x = (rt.Width - 320) / 2;
                var crb = new CroppedBitmap(rt, new Int32Rect((int)x, (int)y, 320, 240));
                using (var f = new MemoryStream())
                {
                    SaveAsJpg(crb, f, 95);
                    f.Flush();
                    f.Seek(0, SeekOrigin.Begin);

                    using (var br = new BinaryReader(f))
                    {
                        ResultData.ImageArray = br.ReadBytes((Int32)f.Length);
                    }
                }

                map.ZoomBarVisibility = Visibility.Visible;
                map.MouseLocationIndicatorVisibility = Visibility.Visible;
                map.ScaleVisibility = Visibility.Visible;
                map.ZoomBarPresetsVisibility = Visibility.Visible;
                map.NavigationVisibility = Visibility.Visible;

                DialogResult = true;
                Close();
            }

        }

        #region ResultData
        public MapLocationItem ResultData { get; set; }
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion


        public static void SaveAsJpg(BitmapSource src, Stream outputStream, int quality)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));
            encoder.QualityLevel = quality;
            encoder.Save(outputStream);
        }



    }
}
