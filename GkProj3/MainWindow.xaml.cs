using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Microsoft.Win32;


namespace GkProj3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainApp _app = new MainApp();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _app.Initialize(BezierCanvas, MainImage, CyanSep, MagentaSep, YellowSep, BlackSep);
            _app.CurvesManager.Curves = new BezierCurve[]
            {
                new BezierCurve(new Vector2(0,255), new Vector2(60,195), new Vector2(127,127), new Vector2(255,0)),
                new BezierCurve(new Vector2(0,255), new Vector2(123,184), new Vector2(173,122), new Vector2(255,0)),
                new BezierCurve(new Vector2(0,255), new Vector2(45,221), new Vector2(174,165), new Vector2(255,0)),
                new BezierCurve(new Vector2(0,255), new Vector2(60,255), new Vector2(127,255), new Vector2(255,255)),
            };
            _app.RenderEverything();
        }

        private void LoadPhotoButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;|All files (*.*)|*.*";
            ofd.InitialDirectory = Directory.GetCurrentDirectory() + "\\Photos";
            var res = ofd.ShowDialog();

            if (res.HasValue && res.Value)
            {
                _app.CreateBitmaps(new Uri(ofd.FileName));
                _app.RenderEverything();
            }
        }
        private void PresentPhotosButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CyanSep.Source != null)
            {
                PresentImages images = new PresentImages(CyanSep.Source, MagentaSep.Source, YellowSep.Source, BlackSep.Source);
                images.Show();
            }
        }
        private void SaveCurvesButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            bool? res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                var fs = new FileStream(dialog.FileName, FileMode.Create);
                XmlSerializer xs = new XmlSerializer(typeof(BezierCurve[]));
                xs.Serialize(fs, _app.CurvesManager.Curves);
                fs.Close();
            }
        }
        private void LoadCurvesButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            bool? res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                try
                {
                    var fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read);
                    XmlSerializer xs = new XmlSerializer(typeof(BezierCurve[]));
                    _app.CurvesManager.Curves = (BezierCurve[])xs.Deserialize(fs);
                    fs.Close();
                    _app.RenderEverything();
                }
                catch (Exception)
                {
                    MessageBox.Show("Select correct XML file\n", "Wrong file!");
                }
            }
        }
        private void Withdraw0PercentButton_OnClick(object sender, RoutedEventArgs e)
        {
            _app.CurvesManager.Curves = new BezierCurve[]
            {
                new BezierCurve(new Vector2(0,255), new Vector2(60,195), new Vector2(127,127), new Vector2(255,0)),
                new BezierCurve(new Vector2(0,255), new Vector2(123,184), new Vector2(173,122), new Vector2(255,0)),
                new BezierCurve(new Vector2(0,255), new Vector2(45,221), new Vector2(174,165), new Vector2(255,0)),
                new BezierCurve(new Vector2(0,255), new Vector2(60,255), new Vector2(127,255), new Vector2(255,255)),
            };
            _app.RenderEverything();
        }
        private void Withdraw100PercentButton_OnClick(object sender, RoutedEventArgs e)
        {
            _app.CurvesManager.Curves = new BezierCurve[]
            {
                new BezierCurve(new Vector2(0,255), new Vector2(60,255), new Vector2(127,255), new Vector2(255,255)),
                new BezierCurve(new Vector2(0,255), new Vector2(60,255), new Vector2(127,255), new Vector2(255,255)),
                new BezierCurve(new Vector2(0,255), new Vector2(60,255), new Vector2(127,255),new Vector2(255,255)),
                new BezierCurve(new Vector2(0,255), new Vector2(60,195), new Vector2(127,127), new Vector2(255,0)),
            };
            _app.RenderEverything();
        }
        private void WithdrawUCRPercentButton_OnClick(object sender, RoutedEventArgs e)
        {
            _app.CurvesManager.Curves = new BezierCurve[]
            {
                new BezierCurve(new Vector2(0,255), new Vector2(125,125), new Vector2(78,173), new Vector2(215,48)),
                new BezierCurve(new Vector2(0,255), new Vector2(133,190), new Vector2(201,100), new Vector2(215,81)),
                new BezierCurve(new Vector2(0,255), new Vector2(95,207), new Vector2(149,165), new Vector2(215,83)),
                new BezierCurve(new Vector2(0,255), new Vector2(237,255), new Vector2(222,237), new Vector2(255,0)),
            };
            _app.RenderEverything();
        }
        private void WithdrawGCRPercentButton_OnClick(object sender, RoutedEventArgs e)
        {
            _app.CurvesManager.Curves = new BezierCurve[]
            {
                new BezierCurve(new Vector2(0,255), new Vector2(93,146), new Vector2(166,77), new Vector2(252,64)),
                new BezierCurve(new Vector2(0,255), new Vector2(131,153), new Vector2(186,105), new Vector2(250,84)),
                new BezierCurve(new Vector2(0,255), new Vector2(82,199), new Vector2(112,145), new Vector2(251,88)),
                new BezierCurve(new Vector2(0,255), new Vector2(193,255), new Vector2(142,255), new Vector2(255,0)),
            };
            _app.RenderEverything();
        }
        private void CyanCurveSelected_OnClick(object sender, RoutedEventArgs e)
        {
            _app?.SelectCurve(0);
            _app.RenderEverything();
        }
        private void MagentaCurveSelected_OnClick(object sender, RoutedEventArgs e)
        {
            _app?.SelectCurve(1);
            _app.RenderEverything();
        }
        private void YellowCurveSelected_OnClick(object sender, RoutedEventArgs e)
        {
            _app?.SelectCurve(2);
            _app.RenderEverything();
        }
        private void BlackCurveSelected_OnClick(object sender, RoutedEventArgs e)
        {
            _app?.SelectCurve(3);
            _app.RenderEverything();
        }
        private void ShowAllCurvesClicked_OnClick(object sender, RoutedEventArgs e)
        {
            _app.DrawAll = DrawAll.IsChecked.Value;
            _app.RenderEverything();
        }

        private void PopularityAlgorithm_OnClick(object sender, RoutedEventArgs e)
        {
            _app.popularityAlg = PopularityAlgCheck.IsChecked.Value;
            _app.ReduceColors();
            _app.RenderEverything();
        }

        private void k_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _app.k = (int)e.NewValue;
            _app.ReduceColors();
            _app.RenderEverything();
        }

        private void BezierCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _app.MousePressed(Mouse.GetPosition(BezierCanvas));
        }

        private void BezierCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _app.MouseReleased();
        }

        private void BezierCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _app.MouseMoved(Mouse.GetPosition(BezierCanvas));
        }
    }
}
