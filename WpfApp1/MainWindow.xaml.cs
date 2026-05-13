using CsvHelper;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static System.Console;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Star> starsList = new List<Star>();
        List<ViewPlanet> planetsList = new List<ViewPlanet>();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => PositionCompass();

            Create_Planets();
            Load_Map();
        }

        // ------------------------------------------------------------------------------------------------------------------

        private void PositionCompass()
        {
            double centerX = CompassOverlay.Width / 2;
            double centerY = CompassOverlay.Height / 2;

            double radius = (DrawingCanvas.Width / 2) - 4;
            double diag = radius * Math.Sqrt(2) / 2;

            PositionElement(NorthText, centerX, centerY - radius);
            PositionElement(SouthText, centerX, centerY + radius);
            PositionElement(EastText, centerX + radius, centerY);
            PositionElement(WestText, centerX - radius, centerY);

            PositionElement(NEText, centerX + diag, centerY - diag);
            PositionElement(SEText, centerX + diag, centerY + diag);
            PositionElement(SWText, centerX - diag, centerY + diag);
            PositionElement(NWText, centerX - diag, centerY - diag);
        }

        private void PositionElement(FrameworkElement element, double x, double y)
        {
            // Force layout update so ActualWidth / ActualHeight are correct
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            element.Arrange(new Rect(element.DesiredSize));

            double halfWidth = element.ActualWidth / 2;
            double halfHeight = element.ActualHeight / 2;

            Canvas.SetLeft(element, x - halfWidth);
            Canvas.SetTop(element, y - halfHeight);
        }

        // ------------------------------------------------------------------------------------------------------------------

        public void Create_Planets()
        {
            ViewPlanet Fégarð = new ViewPlanet(288, 24, 0, "stars.csv");
            ViewPlanet Earth = new ViewPlanet(365.2422, 24, 6.697375, "hyg_v42.csv");

            planetsList.Add(Fégarð);
            planetsList.Add(Earth);
        }

        // ------------------------------------------------------------------------------------------------------------------

        public void Load_Map()
        {
            // create stars.csv
            //using (StreamWriter w = File.AppendText("stars.csv"));

            DrawingCanvas.Children.Clear();
            Draw_Center();

            Read_In_Stars();

            Display_All_Stars();
        }

        // ------------------------------------------------------------------------------------------------------------------

        public void Draw_Center()
        {
            // draw center point
            var center_dot = new Ellipse
            {
                Width = 4,
                Height = 4,
                Fill = Brushes.Gray,
            };

            // Position on canvas
            Canvas.SetLeft(center_dot, 178);
            Canvas.SetTop(center_dot, 178);

            DrawingCanvas.Children.Add(center_dot);
        }

        // ------------------------------------------------------------------------------------------------------------------

        private void Read_In_Stars()
        {
            starsList.Clear();

            DatabaseHelper d = new DatabaseHelper("StarTrakDatabase.db");
            starsList = d.GetStars();

            //string[] lines = File.ReadAllLines("stars.csv");
            //foreach (string line in lines)
            //{
            //    string[] values = line.Split("  ");

            //    MessageBox.Show(line);

            //    starsList.Add(new Star(values[0], Convert.ToDouble(values[1]), Convert.ToDouble(values[2]), Convert.ToDouble(values[3])));
            //}

            //string filePath = "";

            //if (PlanetSelect.Text == "Fégarð")
            //{
            //    filePath = planetsList[0].filePath;
            //}
            //else if (PlanetSelect.Text == "Earth")
            //{
            //    filePath = planetsList[1].filePath;
            //}

            //using var reader = new StreamReader(filePath);
            //using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            //// read CSV file
            //var records = csv.GetRecords<dynamic>();


            //// output
            //foreach (var r in records)
            //{
            //    //MessageBox.Show(r.RA);
            //    //WriteLine($"{r.FirstName,-15}{r.LastName,-10}{r.JoinedDate,15}{r.Salary,15}{r.Active,5}");

            //    //where r.Planet == PlanetSelect.Text -->

            //    starsList.Add(new Star(r.name, Convert.ToDouble(r.ra), Convert.ToDouble(r.dec), Convert.ToDouble(r.mag)));
            //}
        }

        // ------------------------------------------------------------------------------------------------------------------

        public double deg_to_rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public double rad_to_deg(double rad)
        {
            return rad * (180 / Math.PI);
        }

        private void Update_DateTime(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear();
            Draw_Center();

            Display_All_Stars();
        }

        private void Display_All_Stars()
        {

            foreach(Star newStar in starsList)
            {
                Draw_Star(newStar, true);
            }
        }

        private void Draw_Star(Star newStar, bool disp_all)
        {
            if (newStar.mag <= 6)
            {

                if (!double.TryParse(DBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double d) ||
                    !double.TryParse(TBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double local_h) ||
                    !double.TryParse(LaBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) ||
                    !double.TryParse(LoBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
                    return;

                if (LaUnits.Text == "Degrees")
                {
                    lat = deg_to_rad(lat);
                }

                if (LoUnits.Text == "Degrees")
                {
                    lon = deg_to_rad(lon);
                }

                double m = Math.Max(0.8, MaxVisSlider.Value * Math.Pow(((MaxVisSlider.Value - newStar.mag) / (MaxVisSlider.Value - -1.5)), DiffSlider.Value));  //6 * Math.Pow(((6 - newStar.mag) / (6 - -1.5)), 0.7);
            

                //double m = Math.Round((9 - newStar.mag) / 2);

                // find better way to do this
                double GST;
                double utc_h;

                if (PlanetSelect.Text == "Fégarð")
                {
                    utc_h = planetsList[0].calc_utc_h(local_h, lon);
                    GST = planetsList[0].calc_GST(d, utc_h);
                }
                else if (PlanetSelect.Text == "Earth")
                {
                    utc_h = planetsList[1].calc_utc_h(local_h, lon);
                    GST = planetsList[1].calc_GST(d, utc_h);
                }
                else
                {
                    utc_h = local_h;
                    GST = 0;
                }
                // ^^^^^^^

                newStar.calc_pos_1(GST, lon, lat);

                if (newStar.isVisible is true)
                {
                    newStar.calc_pos_2(lat);
                    Tuple<double, double> coordinates = newStar.coordinates;

                    var dot = new Ellipse
                    {
                        Width = m,
                        Height = m,
                        Fill = Brushes.White,
                        Stroke = Brushes.Transparent
                    };

                    // Position on canvas
                    Canvas.SetLeft(dot, (coordinates.Item1 + 180 - (m / 2)));
                    Canvas.SetTop(dot, (coordinates.Item2 + 180 - (m / 2)));

                    dot.Tag = newStar;

                    dot.MouseLeftButtonDown += Dot_MouseLeftButtonDown;

                    DrawingCanvas.Children.Add(dot);

                }
                else if (disp_all is false)
                {
                    MessageBox.Show("Star is not visible at the date and time entered, but has been added successfully");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        Ellipse selectedStar;

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Select_Star(sender as Ellipse);
            e.Handled = true;
        }

        private void Select_Star(Ellipse dot)
        {
            // Unselect previous
            if (selectedStar != null)
            {
                selectedStar.Stroke = Brushes.Transparent;

                
            }

            selectedStar = dot;

            if (selectedStar != null)
            {
                selectedStar.Stroke = Brushes.Yellow;

                Show_Popup(selectedStar);
            }
        }

        private void Show_Popup(Ellipse dot)
        {
            double left = Canvas.GetLeft(dot); // + dot.Width + 5;
            double top = Canvas.GetTop(dot);

            var thisStar = (Star)dot.Tag;

            Star_name.Content = "Name: " + thisStar.name;
            Star_alt.Content = "Altitude: " + thisStar.alt;
            Star_az.Content = "Azimuth: " + thisStar.az;

            DeletePopup.PlacementTarget = DrawingCanvas;
            DeletePopup.HorizontalOffset = left;
            DeletePopup.VerticalOffset = top;
            DeletePopup.IsOpen = true;
        }

        private void DeleteStar_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStar != null)
            {
                DrawingCanvas.Children.Remove(selectedStar);
                selectedStar = null;
                DeletePopup.IsOpen = false;
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedStar != null)
            {
                selectedStar.Stroke = Brushes.Transparent;
                selectedStar = null;
                DeletePopup.IsOpen = false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AddStar_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(RABox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var ra) ||
                !double.TryParse(DecBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec) ||
                !double.TryParse(MBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var m))
                return;

            //if (!double.TryParse(DBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ||
            //    !double.TryParse(TBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var h) ||
            //    !double.TryParse(LaBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) ||
            //    !double.TryParse(LoBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var lon))
            //    return;

            //List<Star> stars = new List<Star>();

            Star newStar = new Star("", ra, dec, m);

            Draw_Star(newStar, false);
        }

        // ------------------------------------------------------------------------------------------------------------------

        public void Switch_Planet(object sender, EventArgs e)
        {
            if (!IsLoaded)
                return;

            Load_Map();
        }
    }
}