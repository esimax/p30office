using Telerik.Windows.Controls.Map;


namespace POLGoogleMap
{
    public class GoogleMapProvider : TiledProvider
    {
        public GoogleMapProvider()
        {
            var mp = new GoogleTileMapSource();
            MapSources.Add(mp.UniqueId, mp);
        }

        public override ISpatialReference SpatialReference
        {
            get { return new MercatorProjection(); }
        }
    }
}
