using System;

namespace POL.Lib.Interfaces
{
    public class AutoUpdateArgs : EventArgs
    {
        public string LastMessage { get; set; }
        public bool Checking { get; set; }
        public string NewVersion { get; set; }
        public bool Downloading { get; set; }
        public int Percent { get; set; }
        public bool ReadyToInstall { get; set; }
        public bool DownloadForced { get; set; }
    }
}
