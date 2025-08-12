using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace POL.WPF.DXControls.POLControls
{
    public partial class PieChar : UserControl
    {
        public PieChar()
        {
            InitializeComponent();
            SizeChanged += (s, e) => Render();
            try
            {
                ForegroundProperty.OverrideMetadata(typeof (PieChar),
                                                    new FrameworkPropertyMetadata(OnForegroundPropertyChanged));
            }
            catch
            {
            }
            Render();
        }

        private static void OnForegroundPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            var pie = obj as PieChar;
            if (pie != null) pie.Render();
        }

        private void Render()
        {
            if (ActualWidth == 0 || ActualHeight == 0)
                return;
            RenderPie(
                new Point(ActualWidth/2, ActualHeight/2),
                Math.Min(ActualWidth, ActualHeight)/2,
                InitialAngle,
                Foreground,
                Percent);
        }

        private void RenderPie(Point center, double radius, double initialAngle, Brush brush, double percentage)
        {
            c1.BeginInit();
            c1.Children.Clear();

            var el = new Ellipse {Width = radius*2, Height = radius*2, Fill = PieBackground};

            var tbGap = (c1.ActualHeight - el.Height)/2;
            Canvas.SetTop(el, tbGap);
            Canvas.SetBottom(el, tbGap + el.Height);

            var lrGap = (c1.ActualWidth - el.Width)/2;
            Canvas.SetLeft(el, lrGap);
            Canvas.SetRight(el, lrGap + el.Width);

            c1.Children.Add(el);


            var angle = CalcAngleFromPercentage(percentage);
            var endAngle = initialAngle + angle;
            var thetaInit = ConvertToRadians(initialAngle);
            var thetaEnd = ConvertToRadians(endAngle);

            var piePiece = new PathFigure {StartPoint = center};

            piePiece.Segments.Add(new LineSegment(CreatePointFromAngle(thetaInit, center, radius), true));

            var size = new Size(radius, radius);

            piePiece.Segments.Add(
                new ArcSegment(
                    CreatePointFromAngle(thetaEnd, center, radius),
                    size,
                    0,
                    (angle > 180),
                    SweepDirection.Clockwise,
                    true
                    )
                );

            piePiece.Segments.Add(new LineSegment(new Point(center.X, center.Y), true));

            var pieGeometry = new PathGeometry();
            pieGeometry.Figures.Add(piePiece);

            var path = new Path {Data = pieGeometry, Fill = brush};

            c1.Children.Add(path);


            c1.EndInit();
        }

        private Point CreatePointFromAngle(double angleInRadians, Point center, double radius)
        {
            var point = new Point
                            {
                                X = radius*Math.Cos(angleInRadians) + center.X,
                                Y = radius*Math.Sin(angleInRadians) + center.Y
                            };
            return point;
        }

        private double CalcAngleFromPercentage(double percentage)
        {
            return 360*percentage;
        }

        private double ConvertToRadians(double theta)
        {
            return (Math.PI/180)*theta;
        }

        #region InitialAngle

        public static readonly DependencyProperty InitialAngleProperty =
            DependencyProperty.Register("InitialAngle", typeof (double), typeof (PieChar),
                                        new UIPropertyMetadata(0.0, InitialAngleChanged));

        public double InitialAngle
        {
            get { return (double) GetValue(InitialAngleProperty); }
            set { SetValue(InitialAngleProperty, value); }
        }

        private static void InitialAngleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            var pie = obj as PieChar;
            if (pie != null) pie.Render();
        }

        #endregion

        #region Percent

        public static readonly DependencyProperty PercentProperty =
            DependencyProperty.Register("Percent", typeof (double), typeof (PieChar),
                                        new UIPropertyMetadata(0.33, PercentChanged));

        public double Percent
        {
            get { return (double) GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        private static void PercentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            var pie = obj as PieChar;
            if (pie != null) pie.Render();
        }

        #endregion

        #region PieBackground

        public static readonly DependencyProperty PieBackgroundProperty =
            DependencyProperty.Register("PieBackground", typeof (Brush), typeof (PieChar),
                                        new UIPropertyMetadata(Brushes.Gray, PercentChanged));

        public Brush PieBackground
        {
            get { return (Brush) GetValue(PieBackgroundProperty); }
            set { SetValue(PieBackgroundProperty, value); }
        }

        #endregion
    }
}
