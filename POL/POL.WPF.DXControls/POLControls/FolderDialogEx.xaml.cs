using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using POL.Lib.Utils;

namespace POL.WPF.DXControls.POLControls
{
    public partial class FolderDialogEx
    {
        public FolderDialogEx()
        {
            InitializeComponent();

            SourceInitialized += (s, e) => HelperIcon.RemoveIcon(this);
            SizeChanged +=
                (s, e) =>
                {
                    if (WindowState == WindowState.Maximized)
                    {
                        WindowState = WindowState.Normal;
                    }
                    if (WindowState == WindowState.Minimized)
                    {
                        WindowState = WindowState.Normal;
                    }
                };
            ResizeMode = ResizeMode.CanResizeWithGrip;


            Loaded +=
                (s, e) =>
                {
                    comboBox.FolderView = folderView;
                    folderView.ShowFiles = false;
                    folderView.ShowSpecialFolders = false;
                    if (RestoreLastPath && GetLastPathAction != null)
                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(200);
                            HelperUtils.DoDispatcher(() => HelperUtils.Try(
                                () =>
                                {
                                    var path = GetLastPathAction.Invoke();
                                    if (!Directory.Exists(path)) return;
                                    comboBox.SetPath(path);
                                }));
                        });
                };
            folderView.AfterSelect +=
                (s, e) =>
                {
                    Title = folderView.SelectedNode.Text;
                    SelectedFolder = folderView.SelectedNode.Path;
                    btSelect.IsEnabled = !string.IsNullOrWhiteSpace(SelectedFolder) && !SelectedFolder.StartsWith("::");
                    btNewFolder.IsEnabled = btSelect.IsEnabled;
                };

            btCancel.Click +=
                (s, e) =>
                {
                    SelectedFolder = null;
                    DialogResult = false;
                    Close();
                };
            btSelect.Click +=
                (s, e) =>
                {
                    DialogResult = true;
                    Close();
                };
            btNewFolder.Click +=
                (s, e) => { folderView.CreateNewFolder(true); };
        }

        public string SelectedFolder { get; set; }
        public bool RestoreLastPath { get; set; }

        public Func<string> GetLastPathAction { get; set; }

        public static string ShowDialog(string title, bool restoreLastPath = true, Func<string> getLastPath = null,
            Action<string> saveLastPath = null, Window owner = null)
        {
            var fd = new FolderDialogEx
            {
                Title = title,
                RestoreLastPath = restoreLastPath,
                GetLastPathAction = getLastPath
            };
            if (owner == null)
                fd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else
            {
                fd.Owner = owner;
                fd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            var dr = fd.ShowDialog();
            if (dr == true)
                if (saveLastPath != null)
                    saveLastPath.Invoke(fd.SelectedFolder);
            return fd.SelectedFolder;
        }
    }
}
