using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MarsianinGame
{
    public class Algorithm
    {
        private Maps map;
        private const int POINTSRANGE = 1;
        private static readonly char[] DOORS = { 'A', 'B', 'C', 'E', 'D' };
        private const char MEDKIT = 'H';
        const int MAX_XP = 100;
        int currentXP = MAX_XP;
        public Algorithm(Maps map)
        {
            this.map = map;

            #region First step
            //FIRST step - find the way to the Q through doors.
            Point[] firstStep = FindPath(map.ReturnAnElementPosition('S'), map.ReturnAnElementPosition('Q'));
            #endregion

            #region Second step
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
                    if (notVisitedDoor.Equals(new Point(-1, -1)))
                    {
                        throw new System.ArgumentException("There is point -1, -1!", "notVisitedDoor");
                    }
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
            #endregion

            #region Third step
            //Find closer cards, deleting the doors and escaping the fire.

            //Position where the 'character' is after made steps.
            Point currentPosition = map.ReturnAnElementPosition('S');

            //Before looping
            List<Point> allKeysInTheThirdStep = new List<Point>();
            foreach (Point door in allDoorsInTheSecondStep.Keys)
            {
                allKeysInTheThirdStep.Add(map.ReturnAnElementPosition(Char.ToLower(map.ReturnObject(door))));
            }

            List<Point> tempAllKeysInTheThirdStep = new List<Point>();
            foreach (Point key in allKeysInTheThirdStep)
            {
                tempAllKeysInTheThirdStep.Add(key);
            }

            List<Point[]> theWholePath = new List<Point[]>();

            while (allKeysInTheThirdStep.Any() == true)
            {
                //Counting steps to all keys
                Dictionary<Point, int> numberOfStepsToKeys = new Dictionary<Point, int>();
                foreach (Point key in tempAllKeysInTheThirdStep)
                {
                    Point[] pathToTheKey = FindPath(currentPosition, key);
                    numberOfStepsToKeys.Add(key, pathToTheKey.Length);
                }


                //Searching for key with minumal number of steps.
                Point goal = numberOfStepsToKeys.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;

                Point[] pathToTheGoal = FindPath(currentPosition, goal);

                //If there is a door - switch to another closest door
                //NOTE I can simplify the search just to use higher amount of steps after break.
                bool flagIfBreak = false;

                foreach (Point position in pathToTheGoal)
                {
                    if (DOORS.Contains(map.ReturnObject(position)))
                    {
                        flagIfBreak = true;
                        break;
                    }
                }

                if (flagIfBreak == true)
                {
                    tempAllKeysInTheThirdStep.Remove(goal);
                    continue;
                }

                
                Point theDoor = map.ReturnAnElementPosition(Char.ToUpper(map.ReturnObject(goal)));
                map.DeleteObject(theDoor);
                map.DeleteObject(goal);
                allKeysInTheThirdStep.Remove(goal);
                tempAllKeysInTheThirdStep.Clear();
                foreach (Point key in allKeysInTheThirdStep)
                {
                    tempAllKeysInTheThirdStep.Add(key);
                }
                currentPosition = pathToTheGoal.Last();


                //NOTE i have excess point in the end og the way.
                theWholePath.Add(pathToTheGoal);
            }

            Point[] pathToTheExit = FindPath(currentPosition, map.ReturnAnElementPosition('Q'));

            theWholePath.Add(pathToTheExit);
            theWholePath.ForEach(x => Array.ForEach(x, i => map.WritePointInConsole(i)));

            currentPosition = pathToTheExit.Last();
            #endregion
        }

        /// <summary>
        /// Check if a dictionary has at least one unvisited door.
        /// </summary>
        /// <param name="allDoorsDictionary">Dictionary to check.</param>
        /// <returns>Return position of the door. Else return Point -1, -1.</returns>
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
        
        /// <summary>
        /// Uses Cell's parents to get the full path.
        /// </summary>
        /// <param name="goal">Last Cell of the path.</param>
        /// <returns>Array of points which the path is made of.</returns>
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

        /// <summary>
        /// Manhattan Distance for A* algorithm.
        /// </summary>
        /// <param name="current">Position of a point on a map.</param>
        /// <param name="goal">Target point where we want to come.</param>
        /// <returns>The sum of absolute values of differences.</returns>
        /// <remarks>Use it when we are allowed to move only in four directions only</remarks>
        private int ReturnH(Point current, Point goal)
        {
            return Math.Abs(current.X - goal.X) + Math.Abs(current.Y - goal.Y);
        }
    }
}
