using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarsianinGame
{
    public class Algorithm
    {
        private Maps map;
        private const int POINTSRANGE = 1;
        private static readonly char[] DOORS = { 'A', 'B', 'C', 'E', 'D' };
        public Algorithm(Maps map)
        {
            this.map = map;

            //FIRST step - find the way to the Q through doors.
            Point[] firstStep = FindPath(map.ReturnAnElementPosition('S'), map.ReturnAnElementPosition('Q'));

            //SECOND step - find all the doors that lead to the Q.

            //Use it for all needed doors to get to Q. Bool to check if already check the way to the door.
            Dictionary<Point, bool> allDoorsInTheSecondStep = new Dictionary<Point, bool>();

            //Fill the dictionary by the path from the first step.
            foreach (Point doorPosition in ReturnDoorsInThePath(firstStep))
            {
                allDoorsInTheSecondStep.Add(doorPosition, false);
            }

            //Check if there is at least one door in the way to Q.
            if (allDoorsInTheSecondStep.Any() == true)
            {
                //looking for doors that lead to the 'main' doors from the first path
                while (AreAllDoorsVisited(allDoorsInTheSecondStep) != true)
                {
                    Point notVisitedDoor = ReturnNotVisitedDoor(allDoorsInTheSecondStep);
                    allDoorsInTheSecondStep[notVisitedDoor] = true;

                    char doorKey = Char.ToLower(map.ReturnObject(notVisitedDoor));
                    Point[] tempPathToTheNotVisitedDoor = FindPath(notVisitedDoor, map.ReturnAnElementPosition(doorKey));

                    //Check if there is any door in the temp way.
                    if (tempPathToTheNotVisitedDoor.Any() == true)
                        foreach (Point tempDoor in ReturnDoorsInThePath(tempPathToTheNotVisitedDoor))
                        {
                            if (allDoorsInTheSecondStep.ContainsKey(tempDoor) != true)
                            {
                                allDoorsInTheSecondStep.Add(tempDoor, false);
                            }
                        }
                }
            }
            
        }

        private Point ReturnNotVisitedDoor(Dictionary<Point, bool> allDoorsDictionary)
        {
            foreach(KeyValuePair<Point, bool> door in allDoorsDictionary)
            {
                if(door.Value == false)
                {
                    return door.Key;
                }
            }
            return new Point(-1, -1);
        }

        /// <summary>
        /// Check if all the doors in the dictionary are visited.
        /// </summary>
        /// <param name="doors">A dictionary of doors by their position.</param>
        /// <returns>True if all are visited, otherwise false</returns>
        private bool AreAllDoorsVisited(Dictionary<Point, bool> doors)
        {
            foreach (bool check in doors.Values)
            {
                if (check == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Finds all doors in the path.
        /// </summary>
        /// <param name="thePath">Array of Points - A path for searching the doors.</param>
        /// <returns>An array of position of the founded doors.</returns>
        private List<Point> ReturnDoorsInThePath(Point[] path)
        {
            List<Point> result = new List<Point>();
            foreach (Point position in path)
            {
                char tempObject = map.ReturnObject(position);
                if (tempObject == '.')
                    continue;
                else if (DOORS.Contains(tempObject))
                {
                    if(result.Contains(position) != true)
                    {
                        result.Add(position);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Uses A* algorithm to find the shortest way to the goal.
        /// </summary>
        /// <param name="start">The start Point.</param>
        /// <param name="goal">The destiny point.</param>
        /// <returns>The array of Point from which the path is.</returns>
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
