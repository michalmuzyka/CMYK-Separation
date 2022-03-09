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
using System.Windows.Shapes;

namespace GkProj3
{
    /// <summary>
    /// Logika interakcji dla klasy PresentImages.xaml
    /// </summary>
    public partial class PresentImages : Window
    {
        public PresentImages(ImageSource cyan, ImageSource magenta, ImageSource yellow, ImageSource black)
        {
            InitializeComponent();

            CyanImg.Source = cyan;
            MagentaImg.Source = magenta;
            YellowImg.Source = yellow;
            BlackImg.Source = black;
        }
    }
}
