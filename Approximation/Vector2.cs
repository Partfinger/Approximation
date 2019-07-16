using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Approximation
{
    class Vector2
    {
        double x, y;
        Point point;
        public double X { get { return x; } }
        public double Y { get { return y; } }
        public Point Point { get { return point; } }

        public static double kx;
        public static double ky;
        public static int min;
        public static int max;

        static int _id = 0;
        int id;
        public int ID { get { return id; } }


        /// <summary>
        /// по реальных
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static Vector2 GetVector2(double i = 0, double j = 0)
        {
            return new Vector2(i, j, new Point((int)(min + min * i * kx), (int)(max - ( min * j * ky ))));
        }

        public Vector2(double i, double j, Point p)
        {
            x = i;
            y = j;
            point = p;
            id = _id++;
        }

        /// <summary>
        /// По графику
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(int x = 0, int y = 0)
        {
            point = new Point(x, y);
            this.x = (double)(x - min) / (min * kx);
            this.y = -(y - max) / (ky * min);
            id = _id++;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y, new Point(a.point.X + b.point.X, a.point.Y + b.point.Y));
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y, new Point(a.point.X - b.point.X, a.point.Y - b.point.Y));
        }

        public void RoundX()
        {
            x = Math.Round(x, 0);
            point.X = (int)(min + min * x * kx);
        }
    }
}
