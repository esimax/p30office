using POL.WPF.DXControls;

namespace POC.Shell.Localizers.fa
{
    internal class PersianPOLProgressBoxLocalizer : POLProgressBoxLocalizer
    {
        public override string GetLocalizedString(POLProgressBoxStringId id)
        {
            switch (id)
            {
                case POLProgressBoxStringId.Cancel:
                    return "لغو";
                case POLProgressBoxStringId.ProgressTitle:
                    return "در حال پردازش ...";
            }
            return this.GetLocalizedString(id);
        }
    }
}
