using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;

namespace GkProj3
{
    [Serializable]
    public class BezierCurve
    {
        public static int Degree = 4;
        public static int ChannelSize => 256;
        
        public Vector2[] V { get; set; } = new Vector2[Degree]; 
        public int[] Values { get; set; } = new int[ChannelSize];

        public BezierCurve(){}
        public BezierCurve(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            V[0] = v0;
            V[1] = v1;
            V[2] = v2;
            V[3] = v3;
            CalculateValues();
        }

        public Point this[int key]
        {
            get => new (V[key].X, V[key].Y);
            set
            {
                V[key] = new Vector2((float)Math.Round(MathF.Max(MathF.Min((float)value.X, 255f), 0)), (float)Math.Round(MathF.Max(MathF.Min((float)value.Y, 255f), 0)));
                CalculateValues();
            }
        }

        private float[] GetAFactors()
        {
            float[] a = new float[Degree];

            a[0] = V[0].Y;
            a[1] = 3 * (V[1].Y - V[0].Y);
            a[2] = 3 * (V[2].Y - 2 * V[1].Y + V[0].Y);
            a[3] = V[3].Y - 3 * V[2].Y + 3 * V[1].Y - V[0].Y;

            return a;
        }

        public void CalculateValues()
        {
            var A = GetAFactors();
            for (int i = 0; i < Values.Length; ++i)
            {
                if (i <= V[3].X)
                {
                    var res = GetRealRootsForChannelValues(i);
                    float t = 0;
                    foreach (var val in res)
                        if (val >= 0 && val <= 1)
                            t = val;

                    Values[i] = (int)(((A[3]*t + A[2])*t + A[1])*t + A[0]);
                    Values[i] = Math.Min(Math.Max(Values[i], 0), 255);
                }
                else
                    Values[i] = Values[(int)V[3].X];
            }
        }


        //obliczanie pierwiastków równania opisanego przez krzywą kubiczną, kod użyty ze stackoverflow:
        //https://stackoverflow.com/questions/51879836/cubic-bezier-curves-get-y-for-given-x-special-case-where-x-of-control-points
        private float[] GetRealRootsForChannelValues(float x)
        {
            float cbrt(float x)
            {
                return x < 0 ? -MathF.Pow(-x, 1f / 3f) : MathF.Pow(x, 1f / 3f);
            }

            float
                pa3 = 3 * V[0].X,
                pb3 = 3 * V[1].X,
                pc3 = 3 * V[2].X,
                a = -V[0].X + pb3 - pc3 + V[3].X,
                b = pa3 - 2 * pb3 + pc3,
                c = -pa3 + pb3,
                d = V[0].X - x;
            float q;

            if (Math.Abs(a) < 0.000001)
            {
                if (Math.Abs(b) < 0.000001)
                {
                    if (Math.Abs(c) < 0.000001)
                        return new float[] { };
                    return new float[] { -d / c };
                }

                q = MathF.Sqrt(c * c - 4 * b * d);
                float b2 = 2 * b;
                return new float[]{
                  (q - c) / b2,
                  (-c - q) / b2
                };
            }

            b /= a;
            c /= a;
            d /= a;

            q = (2 * b * b * b - 9 * b * c + 27 * d) / 27;
            float
              b3 = b / 3,
              p = (3 * c - b * b) / 3,
              p3 = p / 3,
              q2 = q / 2,
              discriminant = q2 * q2 + p3 * p3 * p3,
              u1, v1;

            if (discriminant < 0)
            {
                float mp3 = -p / 3;
                float r = MathF.Sqrt(mp3 * mp3 * mp3);
                float t = -q / (2 * r);
                float cosphi = t < -1 ? -1 : t > 1 ? 1 : t;
                float phi = MathF.Acos(cosphi);
                float crtr = cbrt(r);
                float t1 = 2 * crtr;
                return new float[]{
                      t1 * MathF.Cos(phi / 3) - b3,
                      t1 * MathF.Cos((phi + 2*MathF.PI) / 3) - b3,
                      t1 * MathF.Cos((phi + 2 * 2*MathF.PI) / 3) - b3
                    };
            }

            if (discriminant == 0)
            {
                u1 = q2 < 0 ? cbrt(-q2) : -cbrt(q2);
                return new float[]{
                    2 * u1 - b3,
                    -u1 - b3
                };
            }


            float sd = MathF.Sqrt(discriminant);
            u1 = cbrt(-q2 + sd);
            v1 = cbrt(q2 + sd);
            return new float[] { u1 - v1 - b3 };
        }

    }
}