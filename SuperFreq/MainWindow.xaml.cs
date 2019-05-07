using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuperFreq
{
    public class ModelConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public interface IModel
        {
            Point3DCollection Positions { get; }
            Int32Collection Indices { get; }
        }

        public class TestModel : IModel
        {
            private readonly float[] _positions = new float[]
            {
                0, 0, 0,
                1, 0, 0,
                0, 1, 0,
                1, 1, 0,
                0, 0, 1,
                1, 0, 1,
                0, 1, 1,
                1, 1, 1
            };

            private readonly short[] _indices = new short[]
            {
                2, 3, 1,
                2, 1, 0,
                7, 1, 3,
                7, 5, 1,
                6, 5, 7,
                6, 4, 5,
                6, 2, 0,
                2, 0, 4,
                2, 7, 3,
                2, 6, 7,
                0, 1, 5,
                0, 5, 4
            };

            private readonly float[][] _positions3D;

            public TestModel()
            {
                _positions3D = new float[_positions.Length / 3][];
                int i = 0;

                while (i < _positions3D.Length)
                {
                    var vec = _positions
                        .Skip(i * 3)
                        .Take(3)
                        .ToArray();

                    _positions3D[i] = vec;
                    i++;
                }
            }

            //public float[] Positions => _positions;
            public Point3DCollection Positions => new Point3DCollection(_positions3D.Select(x => new Point3D(x[0], x[1], x[2])));
            public Int32Collection Indices => new Int32Collection(_indices.Select(x => (int)x));
        }

        public MainWindow()
        {
            InitializeComponent();

            var scene = new { Models = new IModel[] { new TestModel() } };
            
            HelixViewport3D.DataContext = scene;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            // Hides the stupid overflow arrow
            // Source: http://stackoverflow.com/questions/4662428/how-to-hide-arrow-on-right-side-of-a-toolbar

            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }
    }
}
