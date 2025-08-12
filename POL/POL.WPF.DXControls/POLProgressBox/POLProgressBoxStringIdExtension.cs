using System;
using System.Windows.Markup;

namespace POL.WPF.DXControls
{
    public class POLProgressBoxStringIdExtension : MarkupExtension
    {
        public POLProgressBoxStringId StringId { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return POLProgressBoxLocalizer.GetString(StringId);
        }
    }
}
