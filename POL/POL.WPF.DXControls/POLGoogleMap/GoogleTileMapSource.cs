using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Telerik.Windows.Controls.Map;

namespace POLGoogleMap
{
    public class GoogleTileMapSource : TiledMapSource
    {
        private const string TileUrlFormat =
            @"http://mt{serverNumber}.google.com/vt/lyrs=m@128&hl=fa&x={x}&y={y}&z={zoom}";

        public GoogleTileMapSource()
            : base(1, 20, 256, 256)
        {
        }

        public override void Initialize()
        {
            RaiseIntializeCompleted();
        }

        protected override Uri GetTile(int tileLevel, int tilePositionX, int tilePositionY)
        {
            var zoomLevel = ConvertTileToZoomLevel(tileLevel);
            var url = TileUrlFormat;

            url = ProtocolHelper.SetScheme(url);
            url = url.Replace("{serverNumber}", "1");
            url = url.Replace("{zoom}", zoomLevel.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{x}", tilePositionX.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{y}", tilePositionY.ToString(CultureInfo.InvariantCulture));

            return new Uri(url);
        }

        protected override bool IsValidCacheUri(int tileLevel, int tilePositionX, int tilePositionY, Uri uri)
        {
            var tileUri = GetTile(tileLevel, tilePositionX, tilePositionY);
            var regEx = new Regex(@"http\:\/\/[^\.]+\.");

            return tileUri != null
                   && regEx.Replace(uri.OriginalString, @"http://") == regEx.Replace(tileUri.OriginalString, @"http://");
        }
    }
}
