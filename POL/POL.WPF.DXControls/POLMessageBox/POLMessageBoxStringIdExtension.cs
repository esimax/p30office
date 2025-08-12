using System;
using System.Windows.Markup;

namespace POL.WPF.DXControls
{
    public class POLMessageBoxStringIdExtension : MarkupExtension
    {
        public POLMessageBoxStringId StringId { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return POLMessageBoxLocalizer.GetString(StringId);
        }
    }
}
