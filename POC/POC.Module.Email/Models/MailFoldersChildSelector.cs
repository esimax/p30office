using System.Collections;
using DevExpress.Xpf.Grid;

namespace POC.Module.Email.Models
{
    public class MailFoldersChildSelector : IChildNodesSelector
    {
        IEnumerable IChildNodesSelector.SelectChildren(object item)
        {
            if (item is EmailTreeItem)
                return (item as EmailTreeItem).SubFolders;

            return null;
        }
    }
}
