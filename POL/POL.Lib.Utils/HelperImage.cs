using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace POL.Lib.Utils
{
    public class HelperImage
    {
        public static BitmapImage GetImageFromUri(Uri uri)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = uri;
            img.EndInit();
            return img;
        }

        public static BitmapImage GetStandardImage32(string name)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource =
                new Uri(string.Format("pack://application:,,,/POL.Lib.Resources;component/Standard/32/{0}", name));
            img.EndInit();
            return img;
        }

        public static BitmapImage GetStandardImage16(string name)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource =
                new Uri(string.Format("pack://application:,,,/POL.Lib.Resources;component/Standard/16/{0}", name));
            img.EndInit();
            return img;
        }

        public static BitmapImage GetSpecialImage48(string name)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource =
                new Uri(string.Format("pack://application:,,,/POL.Lib.Resources;component/Special/48/{0}", name));
            img.EndInit();
            return img;
        }

        public static BitmapImage GetSpecialImage64(string name)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource =
                new Uri(string.Format("pack://application:,,,/POL.Lib.Resources;component/Special/64/{0}", name));
            img.EndInit();
            return img;
        }

        public static BitmapImage GetSpecialImage32(string name)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource =
                new Uri(string.Format("pack://application:,,,/POL.Lib.Resources;component/Special/32/{0}", name));
            img.EndInit();
            return img;
        }

        public static BitmapImage GetSpecialImage16(string name)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource =
                new Uri(string.Format("pack://application:,,,/POL.Lib.Resources;component/Special/16/{0}", name));
            img.EndInit();
            return img;
        }

        public static BitmapImage GetSpecialImageXX(string name)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource =
                new Uri(string.Format("pack://application:,,,/POL.Lib.Resources;component/Special/XX/{0}", name));
            img.EndInit();
            return img;
        }

        public static byte[] ConvertBitmapImageToPNGBytestream(BitmapImage bi)
        {
            var memStream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bi));
            encoder.Save(memStream);
            var bytestream = memStream.GetBuffer();
            return bytestream;
        }

        public static BitmapImage ConvertImageByteToBitmapImage(byte[] bytes)
        {
            if (bytes == null) return null;
            try
            {
                var stream = new MemoryStream(bytes) {Position = 0};
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = stream;
                bi.EndInit();
                return bi;
            }
            catch
            {
                return null;
            }
        }


        public static RenderTargetBitmap GetImageFromUI(FrameworkElement view)
        {
            var size = new Size(view.ActualWidth, view.ActualHeight);
            if (size.IsEmpty)
                return null;

            var result = new RenderTargetBitmap((int) size.Width, (int) size.Height, 96, 96, PixelFormats.Pbgra32);

            var drawingvisual = new DrawingVisual();
            using (var context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(view), null, new Rect(new Point(), size));
                context.Close();
            }

            result.Render(drawingvisual);
            return result;
        }


        public static byte[] ConvertBitmapSourceToByteArrayAsJPG(BitmapSource bitmap)
        {
            var memStream = new MemoryStream();
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memStream);
            return memStream.GetBuffer();
        }

        public static byte[] ConvertBitmapSourceToByteArrayAsPNG(BitmapSource bitmap)
        {
            var memStream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memStream);
            return memStream.GetBuffer();
        }


        public static BitmapSource RTLTransform(BitmapSource source, double scaleX, double transX)
        {
            var tg = new TransformGroup();
            tg.Children.Add(new ScaleTransform {ScaleX = scaleX});
            tg.Children.Add(new TranslateTransform {X = transX});
            var transformBitmap = new TransformedBitmap(source, tg);
            return transformBitmap.Clone();
        }
    }
}
