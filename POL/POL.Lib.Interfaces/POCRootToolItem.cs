using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace POL.Lib.Interfaces
{
    public class POCRootToolItem
    {
        public string Key { get; set; }
        public BitmapImage Image { get; set; }
        public string Title { get; set; }
        public string Tooltip { get; set; }
        public int Order { get; set; }
        public int Permission { get; set; }
        public ICommand Command { get; set; }

        public bool InTamas { get; set; }
    }
}
