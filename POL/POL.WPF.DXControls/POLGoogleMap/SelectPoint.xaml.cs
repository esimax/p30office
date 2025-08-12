using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls.Map;

namespace POL.WPF.DXControls.POLGoogleMap
{
    public partial class SelectPoint : UserControl
    {
        public SelectPoint()
        {
            InitializeComponent();
            map.DataContext = this;
        }


        public Visibility ZoomBarVisibility
        {
            get { return map.ZoomBarVisibility; }
            set { map.ZoomBarVisibility = value; }
        }

        public Visibility CommandBarVisibility
        {
            get { return map.CommandBarVisibility; }
            set { map.CommandBarVisibility = value; }
        }

        public Visibility MouseLocationIndicatorVisibility
        {
            get { return map.MouseLocationIndicatorVisibility; }
            set { map.MouseLocationIndicatorVisibility = value; }
        }

        public Visibility ZoomBarPresetsVisibility
        {
            get { return map.ZoomBarPresetsVisibility; }
            set { map.ZoomBarPresetsVisibility = value; }
        }

        public Visibility ScaleVisibility
        {
            get { return map.ScaleVisibility; }
            set { map.ScaleVisibility = value; }
        }

        public Visibility NavigationVisibility
        {
            get { return map.NavigationVisibility; }
            set { map.NavigationVisibility = value; }
        }

        private void map_MapMouseClick(object sender, MapMouseRoutedEventArgs eventArgs)
        {
            if (!IsInSelectionMode) return;
            eventArgs.Handled = true;
            SelectedPointLocation = eventArgs.Location;
            pp.Visibility = Visibility.Visible;
        }

        public void SetCenter(Location center)
        {
            map.Center = center;
        }

        public void SetCachPath(string path)
        {
            fsc.CachePath = path;
        }

        #region IsInSelectionModeProperty

        public bool IsInSelectionMode
        {
            get { return (bool) GetValue(IsInSelectionModeProperty); }
            set { SetValue(IsInSelectionModeProperty, value); }
        }

        public static readonly DependencyProperty IsInSelectionModeProperty =
            DependencyProperty.Register("IsInSelectionMode", typeof (bool), typeof (SelectPoint),
                new UIPropertyMetadata(true,
                    (d, e) => { ((SelectPoint) d).map.Cursor = (bool) e.NewValue ? Cursors.Cross : Cursors.Arrow; }));

        #endregion

        #region SelectedPointLocationProperty

        public Location SelectedPointLocation
        {
            get { return (Location) GetValue(SelectedPointLocationProperty); }
            set { SetValue(SelectedPointLocationProperty, value); }
        }

        public static readonly DependencyProperty SelectedPointLocationProperty =
            DependencyProperty.Register("SelectedPointLocation", typeof (Location), typeof (SelectPoint),
                new UIPropertyMetadata(Location.Empty,
                    (d, e) =>
                    {
                        var self = (SelectPoint) d;
                        var loc = (Location) e.NewValue;
                        if (loc == Location.Empty)
                        {
                            self.SelectedPointVisibility = Visibility.Hidden;
                            self.pp.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            self.SelectedPointVisibility = Visibility.Visible;
                            self.pp.SetValue(MapLayer.LocationProperty, loc);
                            self.pp.Visibility = Visibility.Visible;
                        }
                    }
                    ));

        #endregion

        #region SelectedPointVisibilityProperty

        private Visibility SelectedPointVisibility
        {
            get { return (Visibility) GetValue(SelectedPointVisibilityProperty); }
            set { SetValue(SelectedPointVisibilityProperty, value); }
        }

        public static readonly DependencyProperty SelectedPointVisibilityProperty =
            DependencyProperty.Register("SelectedPointVisibility", typeof (Visibility), typeof (SelectPoint),
                new UIPropertyMetadata(Visibility.Hidden));

        #endregion

        #region ZoomLevelProperty

        public int ZoomLevel
        {
            get
            {
                return map.ZoomLevel; 
            }
            set { SetValue(ZoomLevelProperty, value); }
        }

        public static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register("ZoomLevel",
            typeof (int), typeof (SelectPoint), new UIPropertyMetadata(4,
                (d, e) =>
                {
                    var self = (SelectPoint) d;
                    self.map.ZoomLevel = Convert.ToInt32(e.NewValue);
                }
                ));

        #endregion
    }
}
