using DevExpress.Xpf.Core;

namespace POC.Shell.Localizers.fa
{
    internal class PersianDXMessageBoxLocalizer : DXMessageBoxLocalizer
    {
        public override string GetLocalizedString(DXMessageBoxStringId id)
        {
            switch (id)
            {
                case DXMessageBoxStringId.Ok:
                    return "تایید";
                case DXMessageBoxStringId.Cancel:
                    return "لغو";
                case DXMessageBoxStringId.Yes:
                    return "بله";
                case DXMessageBoxStringId.No:
                    return "خیر";
            }
            return GetLocalizedString(id);
        }
    }
}
