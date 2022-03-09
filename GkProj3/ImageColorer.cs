using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace GkProj3
{
    public static class ImageColorer
    {
        private static int CMYKchannels = 4;
        public static Color[] GetCMYK(Color color, BezierCurve[] curves)
        {
            int c = 255 - color.R;
            int m = 255 - color.G;
            int y = 255 - color.B;

            int kp = Math.Min(c, Math.Min(m, y));
            c = c - kp + 255 - curves[0].Values[kp];
            m = m - kp + 255 - curves[1].Values[kp];
            y = y - kp + 255 - curves[2].Values[kp];
            int k = curves[3].Values[kp];

            Color C = Color.FromRgb((byte)(255 - c), 255, 255);
            Color M = Color.FromRgb(255, (byte)(255 - m), 255);
            Color Y = Color.FromRgb(255, 255, (byte)(255 - y));
            Color K = Color.FromRgb((byte)k, (byte)k, (byte)k);
            
            return new Color[]{C, M, Y, K};
        }

        public static void ColorImagesForSeparatedColors(DirectBitmap main, DirectBitmap[] panels, BezierCurve[] curves)
        {
            Parallel.For(0, panels[0].Height, y =>
            {
                Parallel.For(0, panels[0].Width, x =>
                {
                    var color = GetCMYK(main.GetPixel(x, y), curves);
                    for (int i = 0; i < CMYKchannels; ++i)
                        panels[i].PutPixel(x, y, color[i]);
                });
            });
        }


    }
}
