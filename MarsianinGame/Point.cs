using System;

namespace MarsianinGame
{
    public struct Point : IEquatable<Point>
    {
        public enum Key : byte
        {
            X,
            Y
        }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point (Point point, int value, Key key)
        {
            if(key == Key.X)
            {
                X = point.X + value;
                Y = point.Y;
            }
            else
            {
                X = point.X;
                Y = point.Y + value;
            }
        }

        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public int X { get;}

        public int Y { get;}

        public bool Equals(Point other)
        {
            if (other.X == X && other.Y == Y)
                return true;
            else
                return false;
        }
    }
}
