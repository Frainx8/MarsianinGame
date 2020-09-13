using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AlgorithmLibrary
{
    public class Algorithm
    //TODO change findPath using delegates, find out why the program is stops on permutations.
    {
        private Maps map;
        private const int POINTSRANGE = 1;
        private int currentHP = CommonStuff.MAX_HP;
        private int BASIC_FIRE_DAMAGE = 20;
        private const int MAX_STEPS_PER_PLACE = 5;
        private delegate Point[] PathFindDelegete(Point start, Point goal, Dictionary<Point, int> visitedPlaces);
        private PathFindDelegete pathFindDelegete;
        public bool IsDead { get; private set; }
        public Point[] Result { get; private set; }
        public string Directions { get; private set; }

        public Algorithm(Maps map)
        {
            this.map = map;

            Result = DoAlgorithm();

            if(Result != null)
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

            #endregion

            #region Second step            

            //Dictionary that keeps all objects from the map.
            Point[] allFoundedObjectsOnMap = FindAllObjects(map.S);

#if false
            Console.WriteLine("I've started genereting combinations!");
#endif
            Point[] shortestWayOfAlgorithm = null;
            if (allFoundedObjectsOnMap.Any())
            {
                shortestWayOfAlgorithm = ReturnShortestPathFromCombinations(allFoundedObjectsOnMap);
            }
#if false
            Console.WriteLine("I finished combinations!");
#endif

            

            if (firstStep != null && shortestWayOfAlgorithm != null)
            {
                if (firstStep.Count() < shortestWayOfAlgorithm.Count())
                {
                    return firstStep;
                }
                else
                {
                    return shortestWayOfAlgorithm;
                }
            }
            else if (shortestWayOfAlgorithm != null)
            {
                return shortestWayOfAlgorithm;
            }

            else if (firstStep != null)
            {
                return firstStep;
            }

            else
            {
                //If there is fire - the character burnt.
                IsDead = map.IsThereFireOnMap();

                return null;
            }

            #endregion
        }

        /// <summary>
        /// Take damage using currentHP.
        /// </summary>
        /// <param name="firePower">Fire power from 1 to 5.</param>
        /// <returns>Return true if currentHP equals or below zero else false.</returns>
        private bool GetDamage(int firePower)
        {
            currentHP -= BASIC_FIRE_DAMAGE * firePower;
            if (currentHP <= 0)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Check for fire in the path and get damage if found one.
        /// </summary>
        /// <param name="path">A path to pass.</param>
        /// <returns>Returns true if died in the way.</returns>
        private bool TakeDamageFromPath(Point[] path)
        {
            foreach (Point position in path)
            {
#if false
                Console.WriteLine(position.ToString());
#endif
                char tempObject = map.ReturnObject(position);
                if (tempObject == '.')
                    continue;
                else if (Maps.FIRE_POWER.Contains(tempObject))
                {
#if false
                    Console.WriteLine("Hurt!");
#endif
                    int firePower = int.Parse(tempObject.ToString());
                    if (GetDamage(firePower))
                    {
                        return true;
                    }
                    
                }
            }
            return false;
        }

        private void UseMedkit(Point position)
        {
            currentHP = CommonStuff.MAX_HP;
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

        /// <summary>
        /// Restores deleted objects on map.
        /// </summary>
        /// <param name="deletedObjects"></param>

        private void RestoreDeletedObjects(Dictionary<Point, char> deletedObjectsmy)
        {
            if(deletedObjectsmy.Any())
            {
                foreach (var item in deletedObjectsmy)
                {
                    map.ChangeObject(item.Key, item.Value);
                }
                deletedObjectsmy.Clear();
            }
        }

        private Point[] ChangeShortestWay(Point[] newWay, Point[] oldWay)
        {
            if (oldWay == null || newWay != null && newWay.Length < oldWay.Length)
            {
                return newWay;
            }
            else
            {
                return oldWay;
            }
        }

        private Point[] TryCombinePathsToObjects(Point[] combination, List<Point[]> differentWays)
        {
#if false
            foreach (var _string in combination)
            {
                Console.Write(" " + _string);
            }
            Console.WriteLine();
#endif
            Point[] shortestWay = null;
            for (int i = 0; i < combination.Length; i++)
            {
                Point currentPosition = map.S;
                currentHP = CommonStuff.MAX_HP;
                Dictionary<Point, char> deletedObjectsThisStep = new Dictionary<Point, char>();
                Dictionary<Point, char> deletedObjectsWholeWay = new Dictionary<Point, char>();
                List<Point> wholePath = new List<Point>();

                Point[] tempPath = ReturnPathFromTwoObject(currentPosition, combination[i], deletedObjectsThisStep, deletedObjectsWholeWay);
#if false
                Console.WriteLine($"From S to {combination[i]} - " + tempPath.Count());
#endif
                if (tempPath != null)
                {
                    wholePath.AddRange(tempPath);
                    wholePath.Remove(wholePath.Last());

                    Point newCombination = combination[i];


                    List<Point> otherObjects = new List<Point>(combination);
                    otherObjects.Remove(combination[i]);

#if false
                    if (newCombination == "a")
                    {
                        Console.Write("New combo " + newCombination);
                        foreach (var _string in otherObjects)
                        {
                            Console.Write(" " + _string);
                        }
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                    }
#endif
                    Point[] copyOfWholePath = wholePath.ToArray();

                    TryCombinePathsToObjects(newCombination, otherObjects.ToArray(), wholePath, differentWays, deletedObjectsWholeWay);

                    wholePath = new List<Point>(copyOfWholePath);

                    RestoreDeletedObjects(deletedObjectsThisStep);

                }
                else
                {
                    RestoreDeletedObjects(deletedObjectsThisStep);
                }
            }
            foreach(Point[] path in differentWays)
            {
                shortestWay = ChangeShortestWay(path, shortestWay);
            }
            return shortestWay;
        }

        private void TryCombinePathsToObjects(Point combination, Point[] oldOtherObjects, List<Point> wholePath,
            List<Point[]> differentWays, Dictionary<Point, char> deletedObjectsWholeWay)
        {
#if false
            Console.Write("current comb " + combination + " and old comb");
            foreach (var _string in oldOtherObjects)
            {
                Console.Write(" " + _string);
            }
            Console.WriteLine();
#endif
            if (oldOtherObjects.Any())
            {
                Point[] copyOfWholePath = wholePath.ToArray();
                for (int i = 0; i < oldOtherObjects.Length; i++)
                {
                    Point currentPosition = combination;
                    Dictionary<Point, char> deletedObjectsThisStep = new Dictionary<Point, char>();
                    Dictionary<Point, char> copyOfdeletedObjectsWholeWay = deletedObjectsWholeWay.ToDictionary(entry => entry.Key,
                       entry => entry.Value);
                    Point[] tempPath = ReturnPathFromTwoObject(currentPosition,
                        oldOtherObjects[i], deletedObjectsThisStep, deletedObjectsWholeWay);
#if false
                    if (tempPath != null)
                    {
                        Console.WriteLine("tempPAth - " + tempPath.Length);
                    }
#endif
#if false
                    if (oldOtherObjects != null && tempPath != null)
                        Console.WriteLine($"From {combination} to" +
                            $" {oldOtherObjects[i]} - " + tempPath.Count());
#endif
                    if (tempPath != null)
                    {
#if false
                        Console.WriteLine("Before" + wholePath.Count());
#endif
                        wholePath.AddRange(tempPath);
                        wholePath.Remove(wholePath.Last());

#if false
                        Console.WriteLine("after" + wholePath.Count());
#endif

                        Point newCombination = oldOtherObjects[i];

                        List<Point> newOtherObjects = new List<Point>(oldOtherObjects);
                        newOtherObjects.Remove(oldOtherObjects[i]);


                        

#if false
                        Console.Write("new comb " + newCombination + " and");
                        foreach (var _string in newOtherObjects)
                        {
                            Console.Write(" " + _string);
                        }
                        Console.WriteLine();
#endif
                        TryCombinePathsToObjects(newCombination, newOtherObjects.ToArray(), wholePath, differentWays, deletedObjectsWholeWay);

#if false
                        Console.WriteLine("Original"+ wholePath.Count());
                        Console.WriteLine("Copy" + copyOfWholePath.Count());
#endif
                        wholePath = new List<Point>(copyOfWholePath);

                        Dictionary<Point, char> secondCopyOfdeletedObjectsWholeWay = deletedObjectsWholeWay.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);

#if false
                        Console.WriteLine("Copy");
                        foreach (var _string in copyOfdeletedObjectsWholeWay)
                        {

                            Console.Write(_string.Value + " " + _string.Key + "; ");
                        }
                        Console.WriteLine();
#endif
#if false
                        Console.WriteLine("Original");
                        foreach (var _string in deletedObjectsWholeWay)
                        {
                            Console.Write(_string.Value + " " + _string.Key + "; ");
                        }
                        Console.WriteLine();
#endif

                        foreach (Point item in secondCopyOfdeletedObjectsWholeWay.Keys)
                        {
                            var itemsToRemove = copyOfdeletedObjectsWholeWay.Where(f => f.Key.Equals(item)).ToArray();
                            foreach (var item2 in itemsToRemove)
                                secondCopyOfdeletedObjectsWholeWay.Remove(item2.Key);

                        }

                        foreach (Point item in deletedObjectsWholeWay.Keys)
                        {
                            var itemsToRemove = secondCopyOfdeletedObjectsWholeWay.Where(f => f.Key.Equals(item)).ToArray();
                            foreach (var item2 in itemsToRemove)
                                deletedObjectsWholeWay.Remove(item2.Key);

                        }

#if false
                        foreach (var _string in secondCopyOfdeletedObjectsWholeWay)
                        {
                            Console.Write(_string.Value + " " + _string.Key + "; ");
                        }
                        Console.WriteLine();
#endif

                        RestoreDeletedObjects(secondCopyOfdeletedObjectsWholeWay);


                    }
                    else
                    {
                        RestoreDeletedObjects(deletedObjectsThisStep);
                    }
                }
            }
            else
            {
                Point[] wayToQ = TryFindWayToQ(combination);
                if (wayToQ != null)
                {

                    wholePath.AddRange(wayToQ);
#if false
                    Console.WriteLine("Way To q - " + wayToQ.Length);
                    Console.WriteLine(wholePath.Count());
#endif
                    differentWays.Add(wholePath.ToArray());
                }
            }
        }

        private Point[] TryFindWayToQ(Point currentPosition)
        {
            Point[] pathToQ = FindPath(currentPosition, map.Q);
            if (pathToQ == null)
            {
                return null;
            }
            else
            {
                return pathToQ;
            }
        }
        private Point[] ReturnPathFromTwoObject(Point firstObject, Point secondObject,
            Dictionary<Point, char> deletedObjectsThisStep, Dictionary<Point, char> deletedObjectsWholeWay)
        {
            Point[] tempPath = FindPath(firstObject, secondObject);

            if (tempPath == null)
            {
                return null;
            }
            else
            {
                char tempObject = map.ReturnObject(secondObject);

                if (Maps.MEDKIT == tempObject)
                {
                    UseMedkit(secondObject);
                }
                //If founded a key - delete also the door.
                else if (Maps.KEYS.Contains(tempObject))
                {
                    map.DeleteObject(secondObject);
                    char door = Char.ToUpper(tempObject);
                    Point doorPosition = map.ReturnAnElementPositionOnMap(door);
                    if(doorPosition != Point.nullPoint)
                    {
                        map.DeleteObject(doorPosition);
                        deletedObjectsThisStep.Add(doorPosition, door);
                        deletedObjectsWholeWay.Add(doorPosition, door);
                    }
                    
                }
                deletedObjectsThisStep.Add(secondObject, tempObject);

                deletedObjectsWholeWay.Add(secondObject, tempObject);


                return tempPath;
            }
        }

        #region Pathfinding algorithms
        private Point[] ReturnShortestPathFromCombinations(Point[] allFoundedObjectsOnMap)
        {
            Point[] shortestWay = null;
            List<Point[]> differentWays = new List<Point[]>();
            //Generating all combinations from founded objects.
            for (int i = 1; i <= allFoundedObjectsOnMap.Count(); i++)
            {
                double procent = i * 100 / allFoundedObjectsOnMap.Count();
                Console.WriteLine($"Loading... {procent}%");

                var severalCombinations = Combinations.MyCombinations(allFoundedObjectsOnMap, i);
                //Example of a combination in string array - {"a", "b", "c"}.
                foreach (Point[] combination in severalCombinations)
                {
#if false
                foreach (var item in combination)
                {
                    Console.Write(map.ReturnObject(item));
                }
                Console.WriteLine();
#endif
                    Point[] tempShortestWay = TryCombinePathsToObjects(combination, differentWays);

                    shortestWay = ChangeShortestWay(tempShortestWay, shortestWay);

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
        private Point[] FindPathA(Point start, Point goal, Dictionary<Point, int> visitedPlaces)
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
                visitedPlaces[current.Point]++;
                closedList.Add(current);
                Point[] neighbors = map.ReturnNeighbours(current.Point).ToArray();
                foreach (Point neighbor in neighbors)
                {
                    char tempObject = map.ReturnObject(neighbor);
                    if (tempObject != '.')
                    {
                        if (Maps.DOORS.Contains(tempObject))
                        {
                            continue;
                        }
                    }
                    Cell newCell = new Cell(neighbor, current.G + POINTSRANGE, ReturnH(neighbor, goal), current);
                    if (newCell.Point.Equals(goal))
                    {
                        visitedPlaces[newCell.Point]++;
                        return ReturnPath(newCell);
                    }
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
        /// <returns>Returns dictionary of founded objects by string and point.</returns>
        private Point[] FindAllObjects(Point start)
        {
            Cell source = new Cell(start, null);
            List<Cell> openList = new List<Cell>()
                {
                    source,
                };

            List<Cell> closedList = new List<Cell>();

            List<Point> result = new List<Point>();

            while (openList.Any())
            {
                Cell current = openList.First();

                char tempObject = map.ReturnObject(current.Point);
                
                if(tempObject != '.' )
                {
                    if(Maps.KEYS.Contains(tempObject) || Maps.MEDKIT == tempObject)
                        result.Add(current.Point);

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

            return result.ToArray();
        }

        /// <summary>
        /// Uses A* algorithm, checking for doors and trying to pass through fire.
        /// </summary>
        /// <param name="start">Start point to start searching.</param>
        /// <param name="goal">Goal of the algorithm</param>
        /// <returns>Returns shortest way to the goal, else null if there is no way.</returns>
        private Point[] FindPath(Point start, Point goal)
        {
            //Used for keeping of number of times that character step on a place.
            Dictionary<Point, int> visitedPlaces = new Dictionary<Point, int>();
            ScanWalkAbleTerritory(start, visitedPlaces);
            pathFindDelegete = FindPathA;
            return FindPath(start, goal, visitedPlaces);
        }

        private Point[] FindPathBread(Point start, Point goal, Dictionary<Point, int> visitedPlaces)
        {
            Cell source = new Cell() { Parent = null, Point = start };

            List<Cell> openList = new List<Cell>()
            {
                source
            };

            List<Cell> closedList = new List<Cell>();

            Point[] foundedPath = null;

            bool isGoalFounded = false;

            //Do until empty.
            //Do after even the goal is founded to fill 
            //the visitedPlaces for compare with numberOfWalkablePlaces.
            while (openList.Any())
            {
                IEnumerable<Cell> query = openList.OrderBy(x => x.G);
                Cell current = query.First();
                visitedPlaces[current.Point]++;
                openList.Remove(current);
                closedList.Add(current);
                //Closest places to the point.
                Point[] neighbors = map.ReturnNeighbours(current.Point);
                foreach (Point neighbor in neighbors)
                {
                    char tempObject = map.ReturnObject(neighbor);
                    if (tempObject != '.')
                    {
                        if (Maps.DOORS.Contains(tempObject))
                        {
                            continue;
                        }
                    }

                    Cell newCell = new Cell(neighbor, visitedPlaces[neighbor], current);

                    if (!isGoalFounded && newCell.Point.Equals(goal))
                    {
                        foundedPath = ReturnPath(newCell);
                        visitedPlaces[newCell.Point]++;
                        isGoalFounded = true;
                    }
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

            return foundedPath;
        }

        private Point[] FindPath(Point start, Point goal, Dictionary<Point, int> visitedPlaces)
        {
            Point[] foundedPath = pathFindDelegete(start, goal, visitedPlaces);

            if (foundedPath == null)
            {
                return null;
            }
            else
            {
#if false
               
                Console.WriteLine($"HP - {currentHP}");
                Console.WriteLine($"Points - {foundedPath.Count()}");
                ShowPathToConsole(foundedPath);
                Console.WriteLine();
#endif
                bool isTherePlacesLessMax = false;

#if false
                foreach (var item in visitedPlaces.Values)
                {
                    Console.WriteLine(item);
                }
#endif
                foreach (int value in visitedPlaces.Values)
                {
                    if (value <= MAX_STEPS_PER_PLACE)
                    {
                        isTherePlacesLessMax = true;
                        break;
                    }
                }
                if (!isTherePlacesLessMax)
                {
                    return null;
                }

                int tempHP = currentHP;

                if (!TakeDamageFromPath(foundedPath))
                {
                    return foundedPath;

                }

                else
                {
                    //If died in the founded way - restore HP and try again considering the visited places.
                    currentHP = tempHP;
                    pathFindDelegete = FindPathBread;
                    return FindPath(start, goal, visitedPlaces);
                }
            }
            
            
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

        /// <summary>
        /// Scan a map for places(points) where the character can step on using BreadthFirstSearch algorithm.
        /// </summary>
        /// <param name="start">Start point where the search starts.</param>
        /// <returns>Returns number of walkable places.</returns>
        private void ScanWalkAbleTerritory(Point start, Dictionary<Point, int> visitedPlaces)
        {
            Cell source = new Cell(start, null);

            //Places is going to be visited.
            List<Cell> openList = new List<Cell>()
                {
                    source
                };

            //Visited plases.
            List<Cell> closedList = new List<Cell>();

            visitedPlaces.Add(start, 0);

            //Do until empty.
            while (openList.Any())
            {
                Cell current = openList.First();

                openList.Remove(current);
                closedList.Add(current);

                //Closest places to the point.
                Point[] neighbors = map.ReturnNeighbours(current.Point);

                foreach (Point neighbor in neighbors)
                {
                    char tempObject = map.ReturnObject(neighbor);

                    if (tempObject != '.')
                    {
                        //If point constains a door - don't include.
                        if (Maps.DOORS.Contains(tempObject))
                        {
                            continue;
                        }
                    }

                    if (!visitedPlaces.ContainsKey(neighbor))
                    {
                        visitedPlaces.Add(neighbor, 0);
                    }

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
        }

        #endregion

        /// <summary>
        /// Show a path to the console.
        /// </summary>
        /// <param name="path">A path to show.</param>
        private void ShowPathToConsole(Point[] path)
        {
            int index = 0;
            for (int y = 0; y < map.Map.GetLength(0); y++)
            {
                for (int x = 0; x < map.Map.GetLength(1); x++)
                {
                    if (path.Contains(new Point(x, y)))
                    {
                        Console.Write('P');
                        index++;
                    }
                    else
                        Console.Write(map.Map[y, x]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            
        }
    }
}
