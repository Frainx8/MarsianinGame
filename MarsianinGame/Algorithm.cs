using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarsianinGame
{
    public class Algorithm
    {
        Maps map;
        const int POINTSRANGE = 1;
        public Algorithm(Maps map)
        {
            this.map = map;
            //Point[] example = FindPath(map.ReturnAnElementPosition('S'), map.ReturnAnElementPosition('Q'));
            //foreach(Point x in example)
            //{
            //    map.ChangeObject(x, '0');
            //}
        }

        private Point[] FindPath(Point start, Point goal)
        {
            Cell source = new Cell() { Parent = null, Point = start };
            List<Cell> openList = new List<Cell>()
            {
                source
            };
            List<Cell> closedList = new List<Cell>();
            while(openList.Any() == true)
            {
                IEnumerable<Cell> query = openList.OrderBy(x => x.F);
                Cell current = query.First();
                openList.Remove(current);
                closedList.Add(current);
                Point[] neighbors =  map.ReturnNeighbours(current.Point).ToArray();
                foreach(Point neighbor in neighbors)
                {
                    Cell newCell = new Cell(neighbor, current.G + POINTSRANGE, ReturnH(neighbor, goal), current);
                    if (newCell.Point.Equals(goal) == true)
                        return ReturnPath(newCell);
                    if (closedList.Contains(newCell) == false)
                    {

                        if(openList.Contains(newCell) == true)
                        {
                            int index = openList.IndexOf(newCell);
                            if(openList[index].F > newCell.F)
                            {
                                openList.RemoveAt(index);
                                openList.Add(newCell);
                            }
                        }
                        else
                        {
                            openList.Add(newCell);
                        }
                    }
                }
            }
            return null;
        }
        private Point[] ReturnPath(Cell goal)
        {
            List<Cell> list = new List<Cell>();
            list.Add(goal);
            while (true)
            {
                Cell parent = list[list.Count - 1].Parent;
                if (parent != null)
                    list.Add(parent);
                
                else
                {
                    list.Reverse();
                    Point[] result = new Point[list.Count];
                    for(int i = 0; i < list.Count; i++)
                    {
                        result[i] = list[i].Point;
                    }
                    return result;
                }
            }
        }

        private int ReturnH(Point current, Point goal)
        {
            return Math.Abs(current.X - goal.X) + Math.Abs(current.Y - goal.Y);
        }
    }
}
