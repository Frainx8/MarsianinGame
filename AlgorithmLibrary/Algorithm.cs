using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AlgorithmLibrary
{
    public class Algorithm
    {
        public Point[] Result { get; private set; }
        public string Directions { get; private set; }

        private Maps map;

        private const int POINTSRANGE = 1;

        private const int MAX_XP = 100;
        private int currentXP = MAX_XP;
        
        public Algorithm(Maps map)
        {
            this.map = map;

            Result = DoAlgorithm();

            Directions = WriteDirections(Result);
        }

        private string WriteDirections(Point[] path)
        {
            StringBuilder directions = new StringBuilder();
            for(int i = 0; i < path.Count() - 1; i++)
            {
                Point thisPoint = path[i];
                Point nextPoint = path[i + 1];

                ComparePoint(thisPoint, nextPoint);
            }

            return directions.ToString();

            void ComparePoint(Point thisPoint, Point nextPoint)
            {
                if(thisPoint.X > nextPoint.X)
                {
                    directions.Append('L');
                }
                else if (thisPoint.X < nextPoint.X)
                {
                    directions.Append('R');
                }
                else if(thisPoint.Y > nextPoint.Y)
                {
                    directions.Append('U');
                }
                else if(thisPoint.Y < nextPoint.Y)
                {
                    directions.Append('D');
                }
            }
        }

        public void WriteResultToFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                sw.WriteLine(Directions.Length);
                sw.WriteLine(Directions);
            }
        }

        private Point[] DoAlgorithm()
        {
            #region First step

            Point[] firstStep = FindPath(map.S, map.Q);

            if (firstStep == null)
            {
                throw new ArgumentException("There are no way to the Q!");
            }
            else if(!IsThereDoor(firstStep) && !IsThereFire(firstStep))
            {
                return firstStep;
            }
            #endregion

            #region Second step            

            //Dictionary that keeps all objects from the map.
            var allObjects = FindAllObjects(map.S);

            //All combinations from allObjects of the map.
            List<string[]> allCombinations = new List<string[]>();

            //Generating all combinations from founded objects.
            for (int i = allObjects.Count(); i > 0; i--)
            {
                var resultT = Combinations.FindCombinations(allObjects.Keys, i);
                foreach (var comb in resultT.ToArray())
                {
                    allCombinations.Add(comb.ToArray());
                }
            }

            Point[] resultOfAlgorithm = ReturnShortestPathFromCombinations(allCombinations, allObjects);

            if(resultOfAlgorithm != null)
            {
                return resultOfAlgorithm;
            }
            else
            {
                throw new ArgumentException("The character has died!");
            }

            #endregion
        }

        /// <summary>
        /// Take damage using currentXP.
        /// </summary>
        /// <param name="firePower">Fire power from 1 to 5.</param>
        /// <returns>Return true if currentXP equals or below zero else false.</returns>
        private bool GetDamage(int firePower)
        {
            int damage = 20;
            currentXP -= damage * firePower;
            if (currentXP <= 0)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Returns true if dead.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool TakeDamageFromPath(Point[] path)
        {
            foreach (Point position in path)
            {
                char tempObject = map.ReturnObject(position);
                if (tempObject == '.')
                    continue;
                else if (Maps.FIRE_POWER.Contains(tempObject))
                {
                    int firePower = int.Parse(tempObject.ToString());
                    if(GetDamage(firePower))
                    {
                        return true;
                    }
                    
                }
            }
            return false;
        }

        private void UseMedkit(Point position)
        {
            currentXP = MAX_XP;
            map.DeleteObject(position);
        }

        /// <summary>
        /// Check, if there is at least one object in the path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="_objectsToCompare"></param>
        /// <returns>Returns true if found, else false.</returns>
        private bool IsThereObjectInWay(Point[] path, char[] _objectsToCompare)
        {
            foreach (Point position in path)
            {
                char tempObject = map.ReturnObject(position);
                if (tempObject == '.')
                    continue;
                else if (_objectsToCompare.Contains(tempObject))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check, if there is fire in the way.
        /// </summary>
        /// <param name="path">Way to check.</param>
        /// <returns>True if found fire, else false.</returns>
        private bool IsThereFire(Point[] path)
        {
            return IsThereObjectInWay(path, Maps.FIRE_POWER);
        }

        /// <summary>
        /// Check, if there is at least one door in the way.
        /// </summary>
        /// <param name="path">Way to check.</param>
        /// <returns>True if found a door, else false.</returns>
        private bool IsThereDoor(Point[] path)
        {
            return IsThereObjectInWay(path, Maps.DOORS);
        }

        private void RestoreObjects(Dictionary<Point, char> deletedObjects)
        {
            foreach (var item in deletedObjects)
            {
                map.ChangeObject(item.Key, item.Value);
            }
            deletedObjects.Clear();
        }


        #region Pathfinding algorithms
        private Point[] ReturnShortestPathFromCombinations(IList<string[]> combinations, Dictionary<string, Point> _objectsOfMap)
        {
            int minNumberOfSteps = Int32.MaxValue;
            Point[] shortestWay = null;
            Dictionary<Point, char> deletedObjects = new Dictionary<Point, char>();

            foreach (string[] combination in combinations)
            {
                List<string[]> permutationsOfCombination = new List<string[]>();
                
                //Load permutationsOfObjectsName
                var resultT = Permutations.GetPermutations(combination, combination.Length);
                foreach (var comb in resultT.ToArray())
                {
                    permutationsOfCombination.Add(comb.ToArray());
                }

                foreach (string[] aPermutation in permutationsOfCombination)
                {
                    Point currentPosition = map.S;
                    currentXP = MAX_XP;
                    RestoreObjects(deletedObjects);
                    List<Point> wholePath = new List<Point>();

                    bool breakFlag = false;
                    foreach (string letter in aPermutation)
                    {
                        Point goal = _objectsOfMap[letter];
                        Point[] tempPath = FindPath(currentPosition, goal);
                        if (tempPath == null)
                        {
                            breakFlag = true;
                            break;
                        }
                        else if(IsThereDoor(tempPath))
                        {
                            breakFlag = true;
                            break;
                        }
                        else
                        {
                            
                            if(!TakeDamageFromPath(tempPath))
                            {
                                currentPosition = goal;
                                char tempObject = map.ReturnObject(goal);
                                wholePath.AddRange(tempPath);
                                wholePath.Remove(wholePath.Last());
                                if(Maps.MEDKIT == tempObject)
                                {
                                    UseMedkit(goal);
                                }
                                else if(Maps.KEYS.Contains(tempObject))
                                {
                                    map.DeleteObject(goal);
                                    char door = Char.ToUpper(tempObject);
                                    Point doorPosition = map.ReturnAnElementPositionOnMap(door);
                                    map.DeleteObject(doorPosition);
                                    deletedObjects.Add(doorPosition, door);
                                }
                                deletedObjects.Add(goal, tempObject);
                            }
                            else
                            {
                                breakFlag = true;
                                break;
                            }

                        }
                    }
                    if(breakFlag)
                    {
                        continue;
                    }
                    else if(wholePath != null)
                    {
                        Point[] pathToQ = FindPath(currentPosition, map.Q);
                        map.WritePointInConsole(currentPosition);
                        if (pathToQ == null || IsThereDoor(pathToQ))
                        {
                            continue;
                        }
                        else
                        {

                            if (!TakeDamageFromPath(pathToQ))
                            {
                                wholePath.AddRange(pathToQ);

                                if(wholePath.Count < minNumberOfSteps)
                                {
                                    
                                    minNumberOfSteps = wholePath.Count;
                                    shortestWay = wholePath.ToArray();
                                }
                            }
                            else
                            {
                                continue;
                            }

                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return shortestWay;
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
            while (openList.Any())
            {
                IEnumerable<Cell> query = openList.OrderBy(x => x.F);
                Cell current = query.First();
                openList.Remove(current);
                closedList.Add(current);
                Point[] neighbors = map.ReturnNeighbours(current.Point).ToArray();
                foreach (Point neighbor in neighbors)
                {
                    Cell newCell = new Cell(neighbor, current.G + POINTSRANGE, ReturnH(neighbor, goal), current);
                    if (newCell.Point.Equals(goal))
                        return ReturnPath(newCell);
                    if (!closedList.Contains(newCell))
                    {
                        if (openList.Contains(newCell))
                        {
                            int index = openList.IndexOf(newCell);
                            if (openList[index].F > newCell.F)
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
        /// Uses Dijkstra's algorithm to find the shortest way to the goal.
        /// </summary>
        /// <param name="start">Start position where the search starts</param>
        /// <param name="goal">Goal position.</param>
        /// <returns>Returns a closest path to the goal.</returns>
        private Point[] FindPathDijklstra(Point start, Point goal)
            {
                Cell source = new Cell(start, 0, null);
                List<Cell> openList = new List<Cell>()
                {
                    source,
                };

                List<Cell> closedList = new List<Cell>();

                while (openList.Any() == true)
                {
                    IEnumerable<Cell> query = openList.OrderBy(x => x.G);
                    Cell current = query.First();
                    openList.Remove(current);
                    closedList.Add(current);
                    Point[] neighbors = map.ReturnNeighbours(current.Point);
                    foreach (Point neighbor in neighbors)
                    {
                        Cell newCell = new Cell(neighbor, current.G + POINTSRANGE, current);

                        if (newCell.Point.Equals(goal))
                            return ReturnPath(newCell);
                        else if (!closedList.Contains(newCell))
                        {

                            if (openList.Contains(newCell))
                            {
                                continue;
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
        /// Uses BreadthFirstSearch algorithm to find all objects on the map.
        /// </summary>
        /// <param name="start">Start point to start the search.</param>
        /// <returns>Returns dictionary of founded objects by char and point.</returns>
        private Dictionary<string, Point> FindAllObjects(Point start)
        {
            Cell source = new Cell(start, null);
            List<Cell> openList = new List<Cell>()
                {
                    source,
                };

            List<Cell> closedList = new List<Cell>();

            Dictionary<string, Point> result = new Dictionary<string, Point>();

            int numberOfMedkit = 1;

            while (openList.Any() == true)
            {
                Cell current = openList.First();

                char tempObject = map.ReturnObject(current.Point);
                
                if(tempObject != '.')
                {
                    if(Maps.KEYS.Contains(tempObject))
                    {
                        result.Add(tempObject.ToString(), current.Point);
                    }
                    if (Maps.MEDKIT == tempObject)
                    {
                        string medkitName = $"H{numberOfMedkit}";
                        numberOfMedkit++;
                        result.Add(medkitName, current.Point);
                    }
                }

                openList.Remove(current);
                closedList.Add(current);
                Point[] neighbors = map.ReturnNeighbours(current.Point);
                foreach (Point neighbor in neighbors)
                {
                    Cell newCell = new Cell(neighbor, current);

                    if (!closedList.Contains(newCell))
                    {

                        if (openList.Contains(newCell))
                        {
                            continue;
                        }
                        else
                        {
                            openList.Add(newCell);
                        }
                    }
                }
            }

            return result;
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
                    for (int i = 0; i < list.Count; i++)
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

        #endregion
    }
}
