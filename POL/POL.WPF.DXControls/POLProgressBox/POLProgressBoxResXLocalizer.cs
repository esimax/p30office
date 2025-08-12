using System.Resources;
using DevExpress.Xpf.Core;

namespace POL.WPF.DXControls
{
    public class POLProgressBoxResXLocalizer : DXResXLocalizer<POLProgressBoxStringId>
    {
        public POLProgressBoxResXLocalizer()
            : base(new POLProgressBoxLocalizer())
        {
        }

        protected override ResourceManager CreateResourceManagerCore()
        {
            return new ResourceManager("POL.WPF.DXControls.POLProgressBoxRes",
                typeof (POLProgressBoxResXLocalizer).Assembly);
        }
    }
}
