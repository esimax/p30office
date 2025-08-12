using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Core;

namespace POL.WPF.DXControls
{
    public class POLMessageBoxLocalizer : DXLocalizer<POLMessageBoxStringId>
    {
        static POLMessageBoxLocalizer()
        {
            SetActiveLocalizerProvider(
                new DefaultActiveLocalizerProvider<POLMessageBoxStringId>(CreateDefaultLocalizer()));
        }

        public static XtraLocalizer<POLMessageBoxStringId> CreateDefaultLocalizer()
        {
            return new POLMessageBoxResXLocalizer();
        }

        public override XtraLocalizer<POLMessageBoxStringId> CreateResXLocalizer()
        {
            return new POLMessageBoxResXLocalizer();
        }

        public static string GetString(POLMessageBoxStringId id)
        {
            return Active.GetLocalizedString(id);
        }

        protected override void PopulateStringTable()
        {
            AddString(POLMessageBoxStringId.Cancel, "Cancel");
            AddString(POLMessageBoxStringId.Ok, "OK");
            AddString(POLMessageBoxStringId.Yes, "Yes");
            AddString(POLMessageBoxStringId.No, "No");
        }
    }
}
