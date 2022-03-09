using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GkProj3
{
    public class CurvesManager
    {
        public BezierCurve[] Curves { get; set; }
        private readonly int _ellipseR = 8;

        private int _selectedCurveId;
        private int? _selectedVertexId;

        private readonly int _canvasWidth;
        private readonly int _canvasHeight;
        private readonly int _xOff;
        private readonly int _yOff;
        public bool DrawAll { set; get; }

        public CurvesManager(int count, int canvasWidth, int canvasHeight)
        {
            Curves = new BezierCurve[count];
            _canvasHeight = canvasHeight;
            _canvasWidth = canvasWidth;

            _xOff = (_canvasWidth - 255) / 2;
            _yOff = (_canvasHeight - 255) / 2;

            for (int i = 0; i < count; ++i)
                Curves[i] = new BezierCurve(new Vector2(0,255), new Vector2( 125, 125), new Vector2(125, 125), new Vector2(255, 0));
        }

        private Point MapPointToCanvas(Point p)
        {
            return new Point(p.X * _canvasWidth, p.Y * _canvasHeight);
        }
        private Point MapPointToCurve(Point p)
        {
            return new Point(p.X / _canvasWidth, p.Y / _canvasHeight);
        }

        public void RenderCurves(Canvas curvesCanvas)
        {
            if (!DrawAll)
                DrawCurve(curvesCanvas, _selectedCurveId, true);
            else
                for(int i=0; i<Curves.Length; ++i)
                    DrawCurve(curvesCanvas, i, i == _selectedCurveId);
        }

        private SolidColorBrush GetCurveColor(int id)
            => id switch
            {
                0 => Brushes.Cyan,
                1 => Brushes.Magenta,
                2 => Brushes.Yellow,
                3 => Brushes.Black
            };

        public void DrawCurve(Canvas canvas, int id, bool isSelected)
        {
            BezierSegment bezier = new BezierSegment()
            {
                Point1 = new (Curves[id][1].X + _xOff, Curves[id][1].Y + _yOff),
                Point2 = new(Curves[id][2].X + _xOff, Curves[id][2].Y + _yOff),
                Point3 = new(Curves[id][3].X + _xOff, Curves[id][3].Y + _yOff),
                IsStroked = true
            };
            PathFigure figure = new PathFigure
            {
                StartPoint = new(Curves[id][0].X + _xOff, Curves[id][0].Y + _yOff)
            };
            figure.Segments.Add(bezier);

            Path path = new Path();
            path.Stroke = GetCurveColor(id);
            path.Data = new PathGeometry(new PathFigure[] { figure });
            canvas.Children.Add(path);

            if (isSelected)
            {
                for (int j = 0; j < BezierCurve.Degree; ++j)
                {
                    var e = new Ellipse()
                    {
                        Width = _ellipseR*2,
                        Height = _ellipseR*2,
                        Stroke = GetCurveColor(id),
                        StrokeThickness = 1
                    };
                    var p = Curves[id][j];
                    Canvas.SetLeft(e, p.X - _ellipseR + _xOff);
                    Canvas.SetTop(e, p.Y - _ellipseR + _yOff);
                    canvas.Children.Add(e);
                }
            }
        }

        public void SelectVertex(Point whereClicked)
        {
            for (int i = 0; i < BezierCurve.Degree; ++i)
            {
                Point vert = new Point(Curves[_selectedCurveId][i].X + _xOff, Curves[_selectedCurveId][i].Y + _yOff);
                if (Utility.InsideHitbox(vert, whereClicked, _ellipseR))
                    _selectedVertexId = i;
            }
        }

        public void MoveVertex(Point newPos)
        {
            if (_selectedVertexId != null)
                Curves[_selectedCurveId][_selectedVertexId.Value] = new Point(newPos.X - _xOff, newPos.Y - _yOff);
        }

        public void UnselectVertex()
        {
            _selectedVertexId = null;
        }

        public void SelectCurve(int curveId)
        {
            _selectedCurveId = curveId;
        }
    }
}
