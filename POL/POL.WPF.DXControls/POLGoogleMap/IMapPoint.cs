using Telerik.Windows.Controls.Map;

namespace POLGoogleMap
{
    public interface IMapPoint
    {
        Location Location { get; set; }

        bool IsSelected { get; set; }
    }
}
