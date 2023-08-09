using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SinusoidAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class ToPointCollection : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null)
            return new PointCollection(value as ObservableCollection<Point>);
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;
            handlers(this, new PropertyChangedEventArgs(propertyName));
        }
        ObservableCollection<Point> _graph2DPoints = new ObservableCollection<Point>();
        public ObservableCollection<Point> Graph2DPoints
        {
            get
            {
                return _graph2DPoints;
            }
            set
            {
                _graph2DPoints = value;
                RaisePropertyChanged("Graph2DPoints");
            }
        }
        Timer timer = new Timer() { Interval = 1, AutoReset = true };
        double HeightSurface;
        public MainWindow()
        {
            InitializeComponent();
            Graph2DPoints.CollectionChanged += (s, e) => { RaisePropertyChanged("Graph2DPoints"); };
            timer.Elapsed += Timer_Elapsed;
            HeightSurface = drawingSurface.Height / 2;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            button.Visibility = Visibility.Collapsed;
            placeholder.Visibility = Visibility.Collapsed;

            List<Point> GraphPoints = new List<Point>(), scalePoints = new List<Point>();
            for (double phi = 0.0; phi <= 4 * Math.PI; phi = phi + 0.1)
            {
                phi = Math.Round(phi, 2);
                var cosValue = -Math.Cos(phi);
                scalePoints.Add(new Point(20 + phi * 15, drawingSurface.Height / 2));
                Graph2DPoints.Add(new Point(20 + phi * 15, drawingSurface.Height / 2 + cosValue * 15));
            }
            scale.Points = new PointCollection(scalePoints);
            scale.Stroke = new SolidColorBrush(Colors.WhiteSmoke);
            scale.StrokeThickness = 1;
            graph2D.Visibility = Visibility.Visible;
            scale.Visibility = Visibility.Visible;
            graph2D.Visibility = Visibility.Collapsed;

            timer.Start();
        }
        double phase = Math.PI / 2 + 0.1;
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            for (int i = 0; i < Graph2DPoints.Count; i++)
            {
                Graph2DPoints[i] = new Point(Graph2DPoints[i].X - 0.5, Graph2DPoints[i].Y);
            }
            if (Graph2DPoints[0].X < 0)
            {
                Graph2DPoints.RemoveAt(0);
                Point p = Graph2DPoints.Last();
                System.Diagnostics.Debug.WriteLine(p.X);
                Graph2DPoints.Add(new Point(p.X + 4.5, HeightSurface - Math.Cos(phase) * 15));
            }
            phase += 0.1;
            if (phase >= 5 * Math.PI / 2)
                phase = Math.PI / 2;

            phase = Math.Round(phase, 2);
            timer.Start();
        }
    }
}