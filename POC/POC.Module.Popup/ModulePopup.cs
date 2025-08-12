using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using POC.Module.Popup.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using System.Windows;

namespace POC.Module.Popup
{
    [Version]
    [Priority(ConstantPOCModules.OrderPopup)]
    [Module(ModuleName = ConstantPOCModules.NamePopup)]
    public class ModulePopup : IModule
    {
        private IUnityContainer UnityContainer { get; set; }
        private ILoggerFacade Logger { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IMembership MMembership { get; set; }
        private IPopup MPopup { get; set; }
        private POCCore APOCCore { get; set; }

        private Thread MainThread { get; set; }
        private readonly List<WPopup> _WList = new List<WPopup>();
        private readonly List<double> _DestList = new List<double>();
        private bool MainThreadDoWork { get; set; }

        public ModulePopup(IUnityContainer unityContainer, ILoggerFacade logger,
                           IPOCMainWindow pocMainWindow, IPopup popup, IMembership membership, POCCore POCCore)
        {
            UnityContainer = unityContainer;
            Logger = logger;
            APOCMainWindow = pocMainWindow;
            MMembership = membership;
            MPopup = popup;
            APOCCore = POCCore;
        }

        #region Implementation of IModule
        public void Initialize()
        {
            Logger.Log(string.Format("Initializing {0}", ConstantPOCModules.NamePopup), Category.Debug, Priority.None);

            MPopup.OnPopupAdded += (s, e) =>
            {
                MainThreadDoWork = false;

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
                {
                    var w = new WPopup(e.Parameter as IPopupItem);
                    if (_WList.Count == 0)
                        w.Top = SystemParameters.PrimaryScreenHeight;
                    else
                    {
                        w.Top = _WList[_WList.Count - 1].Top + _WList[_WList.Count - 1].ActualHeight;
                        if (w.Top < SystemParameters.PrimaryScreenHeight)
                            w.Top = SystemParameters.PrimaryScreenHeight;
                    }
                    w.Left = APOCCore.PopupHorizontalAlignmentOnLeft ? 0 : SystemParameters.PrimaryScreenWidth - w.Width + 1;
                    w.ShowInTaskbar = false;
                    _WList.Add(w);
                    RecalculateDestinations();
                    w.Show();
                    w.LayoutUpdated += (s1, e1) =>
                    {
                        MainThreadDoWork = false;
                        RecalculateDestinations();
                        MainThreadDoWork = true;
                    };
                }));
                MainThreadDoWork = true;
            };

            MPopup.OnPopupRemoveed += (s, e) =>
            {
                MainThreadDoWork = false;
                RemoveClosed();
                RecalculateDestinations();
                MainThreadDoWork = true;
            };

            Application.Current.MainWindow.Closing += (s, e) =>
            {
                MainThreadDoWork = false;
                _WList.ToList().ForEach(w =>
                {
                    try
                    {
                        w.Close();
                    }
                    catch { }
                });
            };

            MainThreadDoWork = false;

            MainThread = new Thread(RepositionPopups);
            MainThread.SetApartmentState(ApartmentState.STA);
            MainThread.IsBackground = true;
            MainThread.Start();
        }
        #endregion

        private void RemoveClosed()
        {
            var l1 = from n in _WList where n.IsClosed == true select n;
            if (l1.Any())
            {
                l1.ToList().ForEach(e => _WList.Remove(e));
            }
        }
        private void RecalculateDestinations()
        {
            _DestList.Clear();
            var c = _WList.Count;
            var top = SystemParameters.WorkArea.Height;
            for (var i = c - 1; i >= 0; i--)
            {
                top = top - _WList[i].ActualHeight - 3;
                _DestList.Insert(0, top);
            }
        }
        private void RepositionPopups()
        {
            if (_WList.Count == 0)
                MainThreadDoWork = false;

            do
            {
                while (!MainThreadDoWork)
                    Thread.Sleep(10);

                var inplace = false;
                while (!inplace)
                {
                    if (!MainThreadDoWork)
                        break;

                    var i = 0;
                    if (Application.Current == null) return;
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Send,
                        new Action(
                            () => _WList.ForEach(
                                e =>
                                {
                                    var dest = _DestList[i];
                                    if (Math.Abs(e.Top - dest) > 0.01)
                                    {
                                        inplace = false;
                                        e.Top = e.Top - (e.Top - dest) * 0.1;
                                        if (Math.Abs(e.Top - dest) < 2) e.Top = dest;
                                    }
                                    i++;
                                })));
                    Thread.Sleep(10);
                }
                MainThreadDoWork = false;
            } while (true);
        }
    }
}
