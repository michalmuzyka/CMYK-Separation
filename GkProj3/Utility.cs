using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GkProj3
{
    public static class Utility
    {
        public static int Distance(Point v1, Point v2)
        {
            return (int)Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
        }
        public static bool InsideHitbox(Point v1, Point where, int hitboxR)
        {
            return Utility.Distance(v1, where) <= hitboxR;
        }
    }
}
