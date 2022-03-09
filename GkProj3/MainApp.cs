using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;

namespace GkProj3
{
    public class MainApp
    {
        public static int PanelsCount => 4;
        private Canvas _curvesCanvas;
        private Image _mainImage;
        private readonly Image[] _panelsImage = new Image[PanelsCount];

        private DirectBitmap _mainImageBitmap;
        private Uri imageUri;
        private readonly DirectBitmap[] _panelsBitmaps = new DirectBitmap[PanelsCount];

        public int k { set; get; } = 8;
        public bool popularityAlg = false;
        public CurvesManager CurvesManager { get; private set; }

        public bool DrawAll
        {
            set => CurvesManager.DrawAll = value;
            get => CurvesManager.DrawAll;
        }
    
        public void Initialize(Canvas canvas, Image mainImage, Image cyan, Image magenta, Image yellow, Image black)
        {
            _curvesCanvas = canvas;
            _mainImage = mainImage;
            _panelsImage[0] = cyan;
            _panelsImage[1] = magenta;
            _panelsImage[2] = yellow;
            _panelsImage[3] = black;
            CurvesManager = new CurvesManager(PanelsCount, (int)_curvesCanvas.ActualWidth, (int)_curvesCanvas.ActualHeight);
            DrawAll = true;

            CurvesManager.RenderCurves(canvas);
        }
        public void CreateBitmaps(Uri imageUri)
        {
            _mainImageBitmap = new DirectBitmap(new BitmapImage(imageUri));
            this.imageUri = imageUri;
            _mainImage.Source = _mainImageBitmap.GetDrawable();
            for (int i = 0; i < PanelsCount; ++i)
            {
                _panelsBitmaps[i] = new DirectBitmap(new BitmapImage(imageUri));
                _panelsImage[i].Source = _panelsBitmaps[i].GetDrawable();
            }

            if (popularityAlg)
                ReduceColors();
        }

        public void ReduceColors()
        {
            if (popularityAlg && _mainImageBitmap != null)
            {
                _mainImageBitmap = new DirectBitmap(new BitmapImage(imageUri));
                _mainImage.Source = _mainImageBitmap.GetDrawable();

                _mainImageBitmap.Lock();

                Dictionary<(int r, int g, int b), int> colorPopularity = new Dictionary<(int r, int g, int b), int>();

                for (int y = 0; y < _mainImageBitmap.Height; ++y)
                    for (int x = 0; x < _mainImageBitmap.Width; ++x) {
                        Color c = _mainImageBitmap.GetPixel(x,y);
                        if (colorPopularity.ContainsKey((c.R, c.G, c.B)))
                            colorPopularity[(c.R, c.G, c.B)]++;
                        else
                            colorPopularity.Add((c.R, c.G, c.B), 1);
                    }

                int count = Math.Min(k, colorPopularity.Count);

                var sortedArray = (from entry in colorPopularity orderby entry.Value descending select entry).Take(count).ToArray();
                List<(int r, int g, int b)> colors = new List<(int r, int g, int b)>();

                for (int i = 0; i < count; ++i)
                    colors.Add(sortedArray[i].Key);

                for (int y = 0; y < _mainImageBitmap.Height; ++y)
                    for (int x = 0; x < _mainImageBitmap.Width; ++x)
                    {
                        int minDistance = int.MaxValue;
                        int id = -1; 

                        Color c = _mainImageBitmap.GetPixel(x, y);

                        for (int i = 0; i < count; ++i)
                        {
                            int distance = (int)Math.Pow(c.R - colors[i].r, 2) + (int)Math.Pow(c.G - colors[i].g, 2) + (int)Math.Pow(c.B - colors[i].b, 2);
                            if(distance < minDistance)
                            {
                                minDistance = distance;
                                id = i;
                            }
                        }
                        _mainImageBitmap.PutPixel(x, y, Color.FromRgb((byte)colors[id].r, (byte)colors[id].g, (byte)colors[id].b));
                    }
                _mainImageBitmap.UpdateBitmap();
                _mainImageBitmap.Unlock();
            }
            else
            {
                if (imageUri != null)
                {
                    _mainImageBitmap = new DirectBitmap(new BitmapImage(imageUri));
                    _mainImage.Source = _mainImageBitmap.GetDrawable();
                }
            }
        }

        public void SelectCurve(int curveId)
        {
            CurvesManager.SelectCurve(curveId);
        }
        public void RenderEverything()
        {
            if (_curvesCanvas != null)
            {
                _curvesCanvas.Children.Clear();
                CurvesManager.RenderCurves(_curvesCanvas);
                if (_mainImageBitmap != null)
                {
                    _mainImageBitmap.Lock();
                    foreach (var img in _panelsBitmaps)
                        img.Lock();

                    ImageColorer.ColorImagesForSeparatedColors(_mainImageBitmap, _panelsBitmaps, CurvesManager.Curves);

                    _mainImageBitmap.Unlock();
                    foreach (var img in _panelsBitmaps)
                    {
                        img.UpdateBitmap();
                        img.Unlock();
                    }
                }
            }
        }

        public void MousePressed(Point pos)
        {
            CurvesManager.SelectVertex(pos);
        }

        public void MouseMoved(Point pos)
        {
            CurvesManager.MoveVertex(pos);
            RenderEverything();
        }

        public void MouseReleased()
        {
            CurvesManager.UnselectVertex();
        }

    }
}
