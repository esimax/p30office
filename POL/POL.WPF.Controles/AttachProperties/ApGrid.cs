using System.Windows;
using System.Windows.Controls;

namespace POL.WPF.Controles.AttachProperties
{
    public class ApGrid
    {
        private static GridLength ParseLength(string length)
        {
            length = length.Trim();

            if (length.ToLowerInvariant().Equals("auto"))
            {
                return new GridLength(0, GridUnitType.Auto);
            }
            if (length.Contains("*"))
            {
                length = length.Replace("*", "");
                if (string.IsNullOrEmpty(length)) length = "1";
                return new GridLength(double.Parse(length), GridUnitType.Star);
            }

            return new GridLength(double.Parse(length), GridUnitType.Pixel);
        }

        #region RowDefinitions attached property

        public static readonly DependencyProperty RowDefinitionsProperty =
            DependencyProperty.RegisterAttached("RowDefinitions", typeof (string), typeof (ApGrid),
                new PropertyMetadata("", OnRowDefinitionsPropertyChanged));

        public static string GetRowDefinitions(DependencyObject d)
        {
            return (string) d.GetValue(RowDefinitionsProperty);
        }

        public static void SetRowDefinitions(DependencyObject d, string value)
        {
            d.SetValue(RowDefinitionsProperty, value);
        }

        private static void OnRowDefinitionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var targetGrid = d as Grid;
            if (targetGrid == null) return;
            targetGrid.RowDefinitions.Clear();
            var rowDefs = e.NewValue as string;
            if (rowDefs == null) return;
            var rowDefArray = rowDefs.Split(',');
            foreach (var rowDefinition in rowDefArray)
            {
                if (rowDefinition.Trim() == "")
                {
                    targetGrid.RowDefinitions.Add(new RowDefinition());
                }
                else
                {
                    targetGrid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = ParseLength(rowDefinition)
                    });
                }
            }
        }

        #endregion

        #region ColumnDefinitions attached property

        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.RegisterAttached("ColumnDefinitions", typeof (string), typeof (ApGrid),
                new PropertyMetadata("", OnColumnDefinitionsPropertyChanged));

        public static string GetColumnDefinitions(DependencyObject d)
        {
            return (string) d.GetValue(ColumnDefinitionsProperty);
        }

        public static void SetColumnDefinitions(DependencyObject d, string value)
        {
            d.SetValue(ColumnDefinitionsProperty, value);
        }

        private static void OnColumnDefinitionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var targetGrid = d as Grid;
            if (targetGrid == null) return;
            targetGrid.ColumnDefinitions.Clear();
            var columnDefs = e.NewValue as string;
            if (columnDefs == null) return;
            var columnDefArray = columnDefs.Split(',');
            foreach (var columnDefinition in columnDefArray)
            {
                if (columnDefinition.Trim() == "")
                {
                    targetGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }
                else
                {
                    targetGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = ParseLength(columnDefinition)
                    });
                }
            }
        }

        #endregion
    }
}
