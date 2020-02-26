using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video;

namespace CCTVStreams
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Public properties

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; this.OnPropertyChanged("ConnectionString"); }
        }

        private string _connectionString;
        private IVideoSource _videoSource;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            ConnectionString = "";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_videoSource == null)
            {
                _videoSource = new MJPEGStream(ConnectionString);
                _videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                _videoSource.Start();
            }

        }


        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)//To update the image.
        {
            try
            {
                BitmapImage bi;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bi = bitmap.ToBitmapImage();
                }
                bi.Freeze(); 
                Dispatcher.BeginInvoke(new ThreadStart(delegate { videoPlayer.Source = bi; }));
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
