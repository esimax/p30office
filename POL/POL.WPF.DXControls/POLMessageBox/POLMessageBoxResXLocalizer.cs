using System.Resources;
using DevExpress.Xpf.Core;

namespace POL.WPF.DXControls
{
    public class POLMessageBoxResXLocalizer : DXResXLocalizer<POLMessageBoxStringId>
    {
        public POLMessageBoxResXLocalizer()
            : base(new POLMessageBoxLocalizer())
        {
        }

        protected override ResourceManager CreateResourceManagerCore()
        {
            return new ResourceManager("POL.WPF.DXControls.POLMessageBoxRes",
                typeof (POLMessageBoxResXLocalizer).Assembly);
        }
    }
}
