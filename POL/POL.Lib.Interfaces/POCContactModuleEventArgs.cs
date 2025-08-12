using System;

namespace POL.Lib.Interfaces
{
    public class POCContactModuleEventArgs : EventArgs
    {
        public POCContactModuleEventArgs(POCContactModuleItem item)
        {
            Item = item;
        }

        public POCContactModuleItem Item { get; set; }
    }
}
