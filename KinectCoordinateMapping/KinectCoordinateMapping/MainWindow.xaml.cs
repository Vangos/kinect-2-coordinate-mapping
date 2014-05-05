using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectCoordinateMapping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;

        CameraMode _mode = CameraMode.Color;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.Default;

            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }

            if (_bodies != null)
            {
                if (_bodies.Count() > 0)
                {
                    foreach (var body in _bodies)
                    {
                        body.Dispose();
                    }
                }
            }

            if (_sensor != null)
            {
                _sensor.Close();
            }
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == CameraMode.Color)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }

            // Depth
            using (var frame = reference.DepthFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == CameraMode.Depth)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }

            // Infrared
            using (var frame = reference.InfraredFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == CameraMode.Infrared)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();

                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body.IsTracked)
                        {
                            // COORDINATE MAPPING
                            foreach (Joint joint in body.Joints.Values)
                            {
                                if (joint.TrackingState == TrackingState.Tracked)
                                {
                                    // 3D space point
                                    CameraSpacePoint jointPosition = joint.Position;

                                    // 2D space point
                                    Point point = new Point();

                                    if (_mode == CameraMode.Color)
                                    {
                                        ColorSpacePoint colorPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);

                                        point.X = float.IsInfinity(colorPoint.X) ? 0 : colorPoint.X;
                                        point.Y = float.IsInfinity(colorPoint.Y) ? 0 : colorPoint.Y;
                                    }
                                    else if (_mode == CameraMode.Depth || _mode == CameraMode.Infrared) // Change the Image and Canvas dimensions to 512x424
                                    {
                                        DepthSpacePoint depthPoint = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(jointPosition);

                                        point.X = float.IsInfinity(depthPoint.X) ? 0 : depthPoint.X;
                                        point.Y = float.IsInfinity(depthPoint.Y) ? 0 : depthPoint.Y;
                                    }

                                    // Draw
                                    Ellipse ellipse = new Ellipse
                                    {
                                        Fill = Brushes.Red,
                                        Width = 30,
                                        Height = 30
                                    };

                                    Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                                    Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

                                    canvas.Children.Add(ellipse);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    enum CameraMode
    {
        Color,
        Depth,
        Infrared
    }
}
