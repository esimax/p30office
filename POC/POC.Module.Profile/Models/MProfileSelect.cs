using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Profile.Models
{
    public class MProfileSelect : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private ListBox DynamicListBox { get; set; }
        public EnumProfileItemType? DynamicProfileItemType { get; set; }

        private DispatcherTimer DataUpdateTimer { get; set; }

        #region CTOR
        public MProfileSelect(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitCommands();
            GetDynamicData();
            PopulateDataList();

            NoSearch = true;
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "فهرست فیلد ها"; }
        }
        #endregion

        #region DataList
        private ObservableCollection<ProfileTreeItem> _DataList;
        public ObservableCollection<ProfileTreeItem> DataList
        {
            get { return _DataList; }
            set
            {
                _DataList = value;
                RaisePropertyChanged("DataList");
            }
        }
        #endregion

        #region SelectedTreeItem
        private ProfileTreeItem _SelectedTreeItem;
        public ProfileTreeItem SelectedTreeItem
        {
            get { return _SelectedTreeItem; }
            set
            {
                if (ReferenceEquals(value, _SelectedTreeItem))
                    return;
                _SelectedTreeItem = value;
                RaisePropertyChanged("SelectedTreeItem");
                MainView.DynamicSelectedData = value;
            }
        }
        #endregion


        #region SearchText
        private string _SearchText;
        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                _SearchText = value;
                RaisePropertyChanged("SearchText");
                NoSearch = string.IsNullOrWhiteSpace(_SearchText);
                UpdateSearchWithDelay();
            }
        }
        #endregion

        private bool NoSearch { get; set; }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => SelectedTreeItem != null);
            CommandClearSearchText = new RelayCommand(ClearSearchText, () => !NoSearch);
        }

        private void OK()
        {
            MainView.DynamicSelectedData = SelectedTreeItem;
            DynamicOwner.DialogResult = true;
            DynamicOwner.Close();
        }


        private void UpdateSearchWithDelay()
        {
            if (DataUpdateTimer == null)
            {
                DataUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                DataUpdateTimer.Tick += (s, e) =>
                {
                    DataUpdateTimer.Stop();
                    PopulateDataList();
                };
            }
            DataUpdateTimer.Stop();
            DataUpdateTimer.Start();
        }





        private List<ProfileTreeItem> _Fullcach;

        private void PopulateDataList()
        {
            if (_Fullcach == null)
            {
                _Fullcach = new List<ProfileTreeItem>();
                var ps = (from n in ACacheData.GetProfileItemList() select (DBCTProfileRoot)n.Tag).ToList();
                ps.ForEach(
                root =>
                {
                    var itemR = new ProfileTreeItem
                    {
                        Title = root.Title,
                        ImageMargin = new Thickness(3, 3, 3, 3),
                        Image = HelperImage.GetSpecialImage16("_16_TabPage.png"),
                        Tag = root,
                    };

                    _Fullcach.Add(itemR);

                    var gs =
                        (from n in ACacheData.GetProfileItemList()
                         where ((DBCTProfileRoot)n.Tag).Oid == root.Oid
                         select n.ChildList.Select(m => (DBCTProfileGroup)m.Tag).ToList()).FirstOrDefault();
                    if (gs != null)
                        gs.ForEach(
                            group =>
                            {
                                var itemG = new ProfileTreeItem
                                {
                                    Title = group.Title,
                                    ImageMargin = new Thickness(32, 3, 3, 3),
                                    Image = HelperImage.GetStandardImage16("_16_Group.png"),
                                    Tag = group,
                                };

                                var v = (from r in ACacheData.GetProfileItemList()
                                         from g in r.ChildList
                                         where ((DBCTProfileGroup)g.Tag).Oid == @group.Oid
                                         select g.ChildList.Select(m => (DBCTProfileItem)m.Tag).ToList()).FirstOrDefault();
                                var ms = v == null ? new List<DBCTProfileItem>() : v.Where(n => string.IsNullOrWhiteSpace(SearchText) || n.Title.Contains(SearchText)).ToList();



                                _Fullcach.Add(itemG);

                                ms.ForEach(
                                    item =>
                                    {
                                        var itemI = new ProfileTreeItem
                                        {
                                            Title = item.Title,
                                            ImageMargin = new Thickness(64, 3, 3, 3),
                                            Image = POL.Lib.Common.HelperP30office.GetProfileItemImage(item.ItemType),
                                            Tag = item,
                                        };
                                        if (DynamicProfileItemType != null)
                                        {
                                            if (item.ItemType == DynamicProfileItemType.Value)
                                                _Fullcach.Add(itemI);
                                        }
                                        else
                                            _Fullcach.Add(itemI);
                                    });

                            });

                });
            }
            {
                _DataList = new ObservableCollection<ProfileTreeItem>();
                var isEmpty = string.IsNullOrWhiteSpace(SearchText);
                foreach (var v in _Fullcach)
                {
                    if (isEmpty || v.Tag is DBCTProfileRoot || v.Tag is DBCTProfileGroup || (v.Tag is DBCTProfileItem && v.Title.Contains(SearchText)))
                        _DataList.Add(v);
                }
                if (!isEmpty)
                {
                    var emptyG = new List<ProfileTreeItem>();
                    for (var i = 0; i < _DataList.Count - 1; i++)
                        if (_DataList[i].Tag is DBCTProfileGroup && !(_DataList[i + 1].Tag is DBCTProfileItem) && !_DataList[i].Title.Contains(SearchText))
                            emptyG.Add(_DataList[i]);

                    foreach (var eg in emptyG)
                        _DataList.Remove(eg);


                    var emptyR = new List<ProfileTreeItem>();
                    for (var i = 0; i < _DataList.Count - 1; i++)
                        if (_DataList[i].Tag is DBCTProfileRoot && !(_DataList[i + 1].Tag is DBCTProfileGroup) && !_DataList[i].Title.Contains(SearchText))
                            emptyR.Add(_DataList[i]);

                    foreach (var er in emptyR)
                        _DataList.Remove(er);

                    if (_DataList[_DataList.Count - 1].Tag is DBCTProfileGroup)
                        _DataList.RemoveAt(_DataList.Count - 1);

                    if (_DataList[_DataList.Count - 1].Tag is DBCTProfileRoot)
                        _DataList.RemoveAt(_DataList.Count - 1);
                }








            }
            RaisePropertyChanged("DataList");
        }



        private void GetDynamicData()
        {
            DynamicOwner = MainView as Window;
            DynamicListBox = MainView.DynamicListBox;
            DynamicProfileItemType = MainView.DynamicProfileItemType;
        }

        private void ClearSearchText()
        {
            SearchText = string.Empty;
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandClearSearchText { get; set; }
        #endregion
    }
}
