using System.Windows;
using System.Windows.Forms;

namespace POL.Lib.Utils
{
    public class HelperMouseKeyboard
    {
        public static Point GetMousePositionWindowsForms()
        {
            var point = Control.MousePosition;
            return new Point(point.X, point.Y);
        }

        public static Point GetMousePosition()
        {
            var w32Mouse = new APIPoint();
            HelperUser32.GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
    }
}
