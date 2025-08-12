using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.Devices;
using POL.Lib.Interfaces.Info;


namespace POL.Lib.Utils
{
    public class HelperComputerInfo
    {
        public static ulong GetTotalMemoryInBytes()
        {
            var cit = new ComputerInfo();
            return cit.TotalPhysicalMemory;
        }

        public static string GetOSVersions()
        {
            var cit = new ComputerInfo();
            return cit.OSVersion;
        }

        public static string GetOSFullName()
        {
            var cit = new ComputerInfo();
            return cit.OSFullName;
        }

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        private static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName,
            out uint lpSectorsPerCluster, out uint lpBytesPerSector,
            out uint lpNumberOfFreeClusters,
            out uint lpTotalNumberOfClusters);

        public static InfoDisk GetDiskInformation(string diskpath)
        {
            var rv = new InfoDisk();

            var dp = Path.GetPathRoot(diskpath);
            uint dummy;
            uint sectorsPerCluster;
            uint bytesPerSector;
            var result = GetDiskFreeSpaceW(dp, out sectorsPerCluster, out bytesPerSector, out dummy, out dummy);
            if (result == 0)
                return null;
            var clusterSize = sectorsPerCluster*bytesPerSector;

            rv.SectorsPerCluster = sectorsPerCluster;
            rv.BytesPerSector = bytesPerSector;
            rv.ClusterSize = clusterSize;
            rv.DrivePath = dp;

            return rv;
        }

        public static InfoOS GetOperatingSystemInformation()
        {
            var rv = new InfoOS();
            try
            {
                var operatingSystem = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                var col = operatingSystem.Get();
                foreach (var group in col)
                {
                    rv.Caption = group.Properties["Caption"].Value.ToString();
                    rv.Architecture = group.Properties["OSArchitecture"].Value.ToString();
                    rv.ServicePack = group.Properties["CSDVersion"].Value.ToString();
                    rv.Version = group.Properties["Version"].Value.ToString();
                    rv.ComputerName = group.Properties["CSName"].Value.ToString();
                    rv.Path = group.Properties["WindowsDirectory"].Value.ToString();
                    rv.Serial = group.Properties["SerialNumber"].Value.ToString();
                    rv.UserName = group.Properties["RegisteredUser"].Value.ToString();
                    break;
                }
            }
            catch
            {
            }
            return rv;
        }

        public static InfoCPU GetProcessorInformation()
        {
            var rv = new InfoCPU();
            try
            {
                var operatingSystem = new ManagementObjectSearcher("select * from Win32_Processor");
                var col = operatingSystem.Get();
                foreach (var group in col)
                {
                    rv.Caption = group.Properties["Name"].Value.ToString();
                    rv.Manufacturer = group.Properties["Manufacturer"].Value.ToString();
                    rv.MaxClockSpeed = group.Properties["MaxClockSpeed"].Value.ToString();
                    rv.ProcessorID = group.Properties["ProcessorId"].Value.ToString();
                    rv.Socket = group.Properties["SocketDesignation"].Value.ToString();
                    rv.Description = group.Properties["Description"].Value.ToString();
                    rv.NumberOfCores = group.Properties["NumberOfCores"].Value.ToString();
                    break;
                }
            }
            catch
            {
            }
            return rv;
        }

        public static InfoLogicalDisk[] GetLogicalDiskInformation()
        {
            var rv = new List<InfoLogicalDisk>();
            try
            {
                var operatingSystem = new ManagementObjectSearcher("select * from Win32_LogicalDisk");
                var col = operatingSystem.Get();
                foreach (var group in col)
                {
                    try
                    {
                        var li = new InfoLogicalDisk
                        {
                            Caption = @group.Properties["Caption"].Value.ToString(),
                            Description = @group.Properties["Description"].Value.ToString()
                        };

                        HelperUtils.Try(() => { li.FileSystem = group.Properties["FileSystem"].Value.ToString(); });
                        HelperUtils.Try(() => { li.Size = group.Properties["Size"].Value.ToString(); });
                        HelperUtils.Try(() => { li.FreeSpace = group.Properties["FreeSpace"].Value.ToString(); });
                        HelperUtils.Try(
                            () => { li.VolumeSerialNumber = group.Properties["VolumeSerialNumber"].Value.ToString(); });
                        HelperUtils.Try(() => { li.VolumeName = group.Properties["VolumeName"].Value.ToString(); });
                        rv.Add(li);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
            return rv.ToArray();
        }

        public static InfoSystem GetSystemInformation()
        {
            var rv = new InfoSystem();
            try
            {
                var operatingSystem = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
                var col = operatingSystem.Get();
                foreach (var group in col)
                {
                    rv.Domain = group.Properties["Domain"].Value.ToString();
                    rv.Manufacturer = group.Properties["Manufacturer"].Value.ToString();
                    rv.Model = group.Properties["Model"].Value.ToString();
                    rv.Roles = (string[]) group.Properties["Roles"].Value;
                    break;
                }
            }
            catch
            {
            }
            return rv;
        }

        public static InfoNetwork[] GetNetworkInformation()
        {
            var rv = new List<InfoNetwork>();
            try
            {
                var operatingSystem = new ManagementObjectSearcher("select * from Win32_NetworkAdapter");
                var col = operatingSystem.Get();
                foreach (var group in col)
                {
                    try
                    {
                        var li = new InfoNetwork();

                        HelperUtils.Try(() => { li.AdapterType = group.Properties["AdapterType"].Value.ToString(); });
                        HelperUtils.Try(() => { li.Name = group.Properties["Name"].Value.ToString(); });
                        HelperUtils.Try(() => { li.MACAddress = group.Properties["MACAddress"].Value.ToString(); });
                        HelperUtils.Try(() => { li.Manufacturer = group.Properties["Manufacturer"].Value.ToString(); });
                        HelperUtils.Try(
                            () =>
                            {
                                li.PhysicalAdapter =
                                    Convert.ToBoolean(group.Properties["PhysicalAdapter"].Value.ToString());
                            });
                        HelperUtils.Try(() => { li.Speed = group.Properties["Speed"].Value.ToString(); });
                        rv.Add(li);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
            return rv.ToArray();
        }
    }
}
