using System;

namespace AlgorithmLibrary
{
    public struct Point : IEquatable<Point>, IComparable
    {

        public static readonly Point nullPoint = new Point(-1, -1);
        public int X { get; }
        public int Y { get; }
        public enum Key : byte
        {
            X,
            Y
        }
        /// <summary>
        /// Creates point with X and Y.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
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


        public bool Equals(Point other)
        {
            if (other.X == X && other.Y == Y)
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }

        public static bool operator ==(Point one, Point another)
        {
            return one.Equals(another);
        }

        public static bool operator !=(Point one, Point another)
        {
            return !(one == another);
        }


        public int CompareTo(object obj)
        {
            Point otherPoint = (Point)obj;
            if (Y < otherPoint.Y || Y == otherPoint.Y && X < otherPoint.X)
            {
                return -1;
            }
            else if (X == otherPoint.X && Y == otherPoint.Y)
            {
                return 0;
            }
            else if (Y > otherPoint.Y || Y == otherPoint.Y && X > otherPoint.X)
            {
                return 1;
            }
            else
            {
                return 1;
            }
        }
    }
}
