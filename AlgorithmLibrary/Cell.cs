using System;

namespace AlgorithmLibrary
{
    public class Cell : IEquatable<Cell>
    {
        public int G { get; set; }
        public int H { get; set; }
        public int F 
        { 
            get
            {
                return G + H;
            }
        }

        public Point Point { get; set; }

        public Cell Parent { get; set; }

        public Cell() { }

        public Cell(Point point, int g, int h, Cell parent)
        {
            Point = point;
            G = g;
            H = h;
            Parent = parent;
        }

        public Cell(Point point, int g, Cell parent)
        {
            Point = point;
            G = g;
            Parent = parent;
        }

        public Cell(Point point, Cell parent)
        {
            Point = point;
            Parent = parent;
        }

        public bool Equals(Cell other)
        {
            if (other.Point.Equals(Point))
                return true;
            else
                return false;
        }
    }
}
