namespace POL.Lib.Interfaces.Info
{
    public class InfoDisk
    {
        public InfoDisk()
        {
            SectorsPerCluster = 0;
            BytesPerSector = 0;
            ClusterSize = 0;
            DrivePath = string.Empty;
        }

        public uint SectorsPerCluster { get; set; }
        public uint BytesPerSector { get; set; }
        public uint ClusterSize { get; set; }
        public string DrivePath { get; set; }
    }
}
