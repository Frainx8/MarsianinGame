using System;

namespace MarsianinGame
{
    public struct Point : IEquatable<Point>
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point (Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public bool Equals(Point other)
        {
            if (other.X == X && other.Y == Y)
                return true;
            else
                return false;
        }
    }
}
