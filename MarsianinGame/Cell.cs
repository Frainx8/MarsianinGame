using System;
using System.Collections.Generic;
using System.Text;

namespace MarsianinGame
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

        public bool Equals(Cell other)
        {
            if (other.Point.Equals(Point))
                return true;
            else
                return false;
        }
    }
}
