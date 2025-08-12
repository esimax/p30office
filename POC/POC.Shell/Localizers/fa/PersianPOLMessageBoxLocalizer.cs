using POL.WPF.DXControls;

namespace POC.Shell.Localizers.fa
{
    internal class PersianPOLMessageBoxLocalizer : POLMessageBoxLocalizer
    {
        public override string GetLocalizedString(POLMessageBoxStringId id)
        {
            switch (id)
            {
                case POLMessageBoxStringId.Ok:
                    return "تایید";
                case POLMessageBoxStringId.Cancel:
                    return "لغو";
                case POLMessageBoxStringId.Yes:
                    return "بله";
                case POLMessageBoxStringId.No:
                    return "خیر";
            }
            return this.GetLocalizedString(id);
        }
    }
}
