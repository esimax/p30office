using System;

namespace POLGoogleMap
{
    public class MapPointEventArgs : EventArgs
    {
        public MapPointEventArgs(IMapPoint mapPoint)
        {
            MapPoint = mapPoint;
        }

        public IMapPoint MapPoint { get; private set; }
    }
}
