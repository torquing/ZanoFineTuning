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
using ZanoFineTuning.Core;
using ZanoFineTuning.Tools.Vibration.Views.Controls;

namespace ZanoFineTuning.Tools.Vibration.Views
{
    /// <summary>
    /// Interaction logic for Complete.xaml
    /// </summary>
    public partial class Complete : Page
    {

        private SolidColorBrush ResultBrushGood, ResultBrushOkay, ResultBrushBad;

        public Complete()
        {
            InitializeComponent();
            
            ResultBrushGood = new SolidColorBrush(Color.FromRgb(0x64, 0xfc, 0x80));
            ResultBrushOkay = new SolidColorBrush(Color.FromRgb(0xff, 0x82, 0x35));
            ResultBrushBad = new SolidColorBrush(Color.FromRgb(0xdc, 0x31, 0x31));

        }

        static MotorReading GetMotorReading(int motor)
        {
            var mr = G.GetCurrentReading(motor);
            if (mr == null)
            {
                mr = G.GetMostRecentReading(motor);
                if (mr == null)
                    return null;
            }
            return mr;
        }

        static double GetRating(int motor)
        {
            var mr = GetMotorReading(motor);
            if (mr == null)
                return double.NaN;
            return mr.Rating;
        }

        static String FormatRating(int motor)
        {
            double rating = GetRating(motor);
            if (double.IsNaN(rating))
            {
                return "-";
            }
            else
            {
                return String.Format("{0:0.0}", rating);
            }
        }
            
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Motor0.Content = FormatRating(0);
            Motor1.Content = FormatRating(1);
            Motor2.Content = FormatRating(2);
            Motor3.Content = FormatRating(3);

            MotorResults.Children.Clear();

            for (int i = 0; i < 4; i++)
            {
                var mr = GetMotorReading(i);
                if (mr == null)
                    continue;

                var vmr = new VibMotorResult();
                vmr.Motor.Content = i.ToString();
                vmr.MotorName.Content = U.MotorIdToName(i);
                vmr.Rating.Content = String.Format("{0:0.0}/5.0", mr.Rating);

                if (mr.Rating < 2)
                {
                    vmr.Rating.Foreground = ResultBrushBad;
                    vmr.Recommendation.Text = "Please change the propeller";
                }
                else if (mr.Rating < 3.75)
                {
                    vmr.Rating.Foreground = ResultBrushOkay;
                    vmr.Recommendation.Text = "Good";
                }
                else
                {
                    vmr.Rating.Foreground = ResultBrushGood;
                    vmr.Recommendation.Text = "Excellent";
                }
                
                MotorResults.Children.Add(vmr);
            }

            double lowestRating = 5, highestRating = 0;
            double average = 0;
            int averageCount = 0;
            for (int i = 0; i < 4; i++)
            {
                var mr = GetMotorReading(i);
                if (mr == null)
                    continue;
                if (mr.Rating < lowestRating)
                    lowestRating = mr.Rating;
                else if (mr.Rating > highestRating)
                    highestRating = mr.Rating;
                average += mr.Rating;
                averageCount++;
            }

            double invertRange = 5 - (highestRating - lowestRating);
            double normalisedAverage = (average/averageCount)/5.0;

            double overallScore = Math.Min(normalisedAverage*invertRange, 5.0);

            OverallScore.Content = String.Format("{0:0.0}/5.0", overallScore);

            if (overallScore < 2)
                OverallScore.Foreground = ResultBrushBad;
            else if (overallScore < 3.75)
                OverallScore.Foreground = ResultBrushOkay;
            else
                OverallScore.Foreground = ResultBrushGood;

            double arrowSize = G.MovementLength;

            if (G.MovementLength >= 6.0)
                arrowSize = 6.0f;
            else if (G.MovementLength <= 1.0f)
                arrowSize = 1.0f;

            arrowSize *= 25.0f;

            var points = CreateLineWithArrowPointCollection(new Point(256, 256), new Point(256 - G.MovementDirectionX * arrowSize, 256 + G.MovementDirectionY * arrowSize), 5);

            var polygon = new Polygon();
            polygon.Points = points;
            polygon.Fill = Brushes.White;

            C.Children.Add(polygon);

            // Mag.Content = G.MovementLength.ToString();
        }

        private const double _maxArrowLengthPercent = 0.3; // factor that determines how the arrow is shortened for very short lines
        private const double _lineArrowLengthFactor = 3.73205081; // 15 degrees arrow:  = 1 / Math.Tan(15 * Math.PI / 180); 

        public static PointCollection CreateLineWithArrowPointCollection(Point startPoint, Point endPoint, double lineWidth)
        {
            Vector direction = endPoint - startPoint;

            Vector normalizedDirection = direction;
            normalizedDirection.Normalize();

            Vector normalizedlineWidenVector = new Vector(-normalizedDirection.Y, normalizedDirection.X); // Rotate by 90 degrees
            Vector lineWidenVector = normalizedlineWidenVector * lineWidth * 0.5;

            double lineLength = direction.Length;

            double defaultArrowLength = lineWidth * _lineArrowLengthFactor;

            // Prepare usedArrowLength
            // if the length is bigger than 1/3 (_maxArrowLengthPercent) of the line length adjust the arrow length to 1/3 of line length

            double usedArrowLength;
            if (lineLength * _maxArrowLengthPercent < defaultArrowLength)
                usedArrowLength = lineLength * _maxArrowLengthPercent;
            else
                usedArrowLength = defaultArrowLength;

            // Adjust arrow thickness for very thick lines
            double arrowWidthFactor;
            if (lineWidth <= 1.5)
                arrowWidthFactor = 3;
            else if (lineWidth <= 2.66)
                arrowWidthFactor = 4;
            else
                arrowWidthFactor = 1.5 * lineWidth;

            Vector arrowWidthVector = normalizedlineWidenVector * arrowWidthFactor;


            // Now we have all the vectors so we can create the arrow shape positions
            var pointCollection = new PointCollection(7);

            Point endArrowCenterPosition = endPoint - (normalizedDirection * usedArrowLength);

            pointCollection.Add(endPoint); // Start with tip of the arrow
            pointCollection.Add(endArrowCenterPosition + arrowWidthVector);
            pointCollection.Add(endArrowCenterPosition + lineWidenVector);
            pointCollection.Add(startPoint + lineWidenVector);
            pointCollection.Add(startPoint - lineWidenVector);
            pointCollection.Add(endArrowCenterPosition - lineWidenVector);
            pointCollection.Add(endArrowCenterPosition - arrowWidthVector);

            return pointCollection;
        }

        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            T.Trigger("retry");
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            T.Trigger("menu");
        }
    }
}
