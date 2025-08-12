using System.Windows.Controls;

namespace POC.Module.Email.Models
{
    public class ParameterHolder
    {
        public string TagTitle { get; set; }
        public bool IsMultiLine { get; set; }
        public ScrollBarVisibility HasScrollBar { get; set; }
        public double Height { get; set; }

        public string Data { get; set; }
    }
}
