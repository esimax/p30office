using System;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Email.Models
{
    public class AttachedFileHolder : NotifyObjectBase
    {
        public AttachedFileHolder()
        {
            Oid = Guid.NewGuid();
        }
        public Guid Oid { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get { return POL.Lib.Utils.HelperConvert.ConvertToFileSizeFormat(FileLen); } }
        public long FileLen { get; set; }

    }
}
