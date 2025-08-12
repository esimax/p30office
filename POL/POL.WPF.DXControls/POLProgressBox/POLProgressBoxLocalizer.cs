using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Core;

namespace POL.WPF.DXControls
{
    public class POLProgressBoxLocalizer : DXLocalizer<POLProgressBoxStringId>
    {
        static POLProgressBoxLocalizer()
        {
            SetActiveLocalizerProvider(
                new DefaultActiveLocalizerProvider<POLProgressBoxStringId>(CreateDefaultLocalizer()));
        }

        public static XtraLocalizer<POLProgressBoxStringId> CreateDefaultLocalizer()
        {
            return new POLProgressBoxResXLocalizer();
        }

        public override XtraLocalizer<POLProgressBoxStringId> CreateResXLocalizer()
        {
            return new POLProgressBoxResXLocalizer();
        }

        public static string GetString(POLProgressBoxStringId id)
        {
            return Active.GetLocalizedString(id);
        }

        protected override void PopulateStringTable()
        {
            AddString(POLProgressBoxStringId.Cancel, "Cancel");
            AddString(POLProgressBoxStringId.ProgressTitle, "Processing ...");
        }
    }
}
