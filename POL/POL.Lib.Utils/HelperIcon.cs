using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace POL.Lib.Utils
{
    public class HelperIcon
    {
        #region FolderType enum

        public enum FolderType
        {
            Open = 0,
            Closed = 1
        }

        #endregion

        #region IconSize enum

        public enum IconSize
        {
            Large = 0,
            Small = 1
        }

        #endregion

        public static Icon GetFileIcon(string name, IconSize size, bool linkOverlay)
        {
            var shfi = new HelperShell32.SHFILEINFO();
            var flags = HelperShell32.SHGFI_ICON | HelperShell32.SHGFI_USEFILEATTRIBUTES;

            if (linkOverlay)
                flags += HelperShell32.SHGFI_LINKOVERLAY;

            
            if (IconSize.Small == size)
            {
                flags += HelperShell32.SHGFI_SMALLICON;
            }
            else
            {
                flags += HelperShell32.SHGFI_LARGEICON;
            }

            HelperShell32.SHGetFileInfo(name,
                HelperShell32.FILE_ATTRIBUTE_NORMAL,
                ref shfi,
                (uint) Marshal.SizeOf(shfi),
                flags);

            var icon = (Icon) Icon.FromHandle(shfi.hIcon).Clone();
            HelperUser32.DestroyIcon(shfi.hIcon); 
            return icon;
        }

        public static Icon GetFolderIcon(IconSize size, FolderType folderType)
        {
            var flags = HelperShell32.SHGFI_ICON | HelperShell32.SHGFI_USEFILEATTRIBUTES;

            if (FolderType.Open == folderType)
            {
                flags += HelperShell32.SHGFI_OPENICON;
            }

            if (IconSize.Small == size)
            {
                flags += HelperShell32.SHGFI_SMALLICON;
            }
            else
            {
                flags += HelperShell32.SHGFI_LARGEICON;
            }

            var shfi = new HelperShell32.SHFILEINFO();
            HelperShell32.SHGetFileInfo(null,
                HelperShell32.FILE_ATTRIBUTE_DIRECTORY,
                ref shfi,
                (uint) Marshal.SizeOf(shfi),
                flags);

            Icon.FromHandle(shfi.hIcon); 

            var icon = (Icon) Icon.FromHandle(shfi.hIcon).Clone();

            HelperUser32.DestroyIcon(shfi.hIcon); 
            return icon;
        }


        public static void RemoveIcon(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var extendedStyle = HelperUser32.GetWindowLong(hwnd, HelperUser32.GWL_EXSTYLE);
            var v = extendedStyle.ToInt32() | HelperUser32.WS_EX_DLGMODALFRAME;
            HelperUser32.SetWindowLong(hwnd, HelperUser32.GWL_EXSTYLE, new IntPtr(v));

            HelperUser32.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
                HelperUser32.SWP_NOMOVE | HelperUser32.SWP_NOSIZE | HelperUser32.SWP_NOZORDER |
                HelperUser32.SWP_FRAMECHANGED);
        }
    }
}
