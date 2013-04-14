using System;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;

namespace WpfOpenCV
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sourceImage = new Bitmap("C:\\Steve_Wozniak.jpg");

            string haarcascade = "haarcascade_frontalface_default.xml";
            
            using (HaarCascade face = new HaarCascade(haarcascade))
            {
                var image = new Image<Rgb, Byte>(sourceImage);

                using (var gray = image.Convert<Gray, Byte>())
                {
                    var detectedFaces = face.Detect(
                                            gray,
                                            1.1,
                                            10,
                                            Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                                            new System.Drawing.Size(20, 20));

                    var firstFace = detectedFaces[0];
                    System.Drawing.Bitmap bmpImage = image.Bitmap;
                    System.Drawing.Bitmap bmpCrop = bmpImage.Clone(firstFace.rect,
                                                                    bmpImage.PixelFormat);

                    var cropedImage = new Image<Rgb, Byte>(bmpCrop);

                    MainImage.Source = ToBitmapSource(sourceImage);
                    DetectedFaceImage.Source = ToBitmapSource(cropedImage.Bitmap);
                }
            }
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);
        public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            IntPtr ptr = bitmap.GetHbitmap();
            var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            ptr,
            IntPtr.Zero,
            Int32Rect.Empty,
            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ptr);
            return source;
        }
    }
}
