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

        public void WriteResultToFile()
        {
            string fileName = @"moves.txt";

            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                sw.WriteLine(Directions.Length);
                sw.WriteLine(Directions);
            }
        }

        private Point[] DoAlgorithm()
        {

            List<Point[]> differentWaysToQ = new List<Point[]>();
            List<Point[]> visitedWaysToQ = new List<Point[]>();

            bool foundTheWay = false;

            #region First step
            //FIRST step - find the way to the Q through doors.
            Point[] firstStep = FindPath(map.ReturnAnElementPositionOnMap('S'), map.ReturnAnElementPositionOnMap('Q'));

            if (firstStep == null)
            {
                throw new ArgumentException("There are no way to the Q!");
            }
            #endregion

            differentWaysToQ.Add(firstStep);
            visitedWaysToQ.Add(firstStep);

            while (foundTheWay != true)
            {
                Point[] theWay = differentWaysToQ.Last().ToArray();
                visitedWaysToQ.Add(differentWaysToQ.First());
                
                

                #region Second step
                //SECOND step - find all the doors that lead to the Q.

                Dictionary<Point, bool> allDoorsInTheSecondStep = null;

                Point[] result = CheckDoorsInTheWay(theWay);
                if (result != null)
                {
                    return result;
                }

                #endregion

                #region Third step
                //Find closer cards, deleting the doors and escaping the fire.

                List<Point> allKeysInTheThirdStep = new List<Point>();

                //Position where the 'character' is after made steps.
                Point currentPosition = map.ReturnAnElementPositionOnMap('S');

                //Positions (cards, medkits), where the charcter were.
                List<Point> allVisitedPositions = new List<Point>()
                {
                    currentPosition
                };
                //Objects that are found in the ways and used but recover if 'the character' died or
                //when need to make steps back.
                List<Dictionary<Point, char>> tempObjects = new List<Dictionary<Point, char>>();

                bool IsDead = false;

                result = FindCardsAndGothroughFire();

                if(result == null)
                {
                    continue;
                }

                return result;


                #endregion

                /// <summary>
                /// Check if the 'character' can die in the way with currentXP;
                /// </summary>
                /// <param name="path"></param>
                /// <returns>If died returns point of death and point before it, else null.</returns>
                Point[] IsDeadInTheWay(Point[] path)
                {
                    int tempCurrentHP = currentXP;
                    Dictionary<Point, char> tempObjectsInThisWay = new Dictionary<Point, char>();

                    for (int i = 0; i < path.Count(); i++)
                    {
                        Point positionInTheWay = path[i];
                        char tempObject = map.ReturnObject(positionInTheWay);

                        if (tempObject == '.' || tempObject == 'S' || tempObject == 'Q')
                        {
                            continue;
                        }

                        else if (Maps.KEYS.Contains(tempObject))
                        {
                            Point doorP = DeleteDoorAndKey(positionInTheWay, tempObject);
                            tempObjectsInThisWay.Add(positionInTheWay, tempObject);
                            tempObjectsInThisWay.Add(doorP, map.ReturnObject(doorP));
                        }
                        else if (Maps.MEDKIT == tempObject)
                        {
                            UseMedkit(positionInTheWay);
                            tempObjectsInThisWay.Add(positionInTheWay, tempObject);
                        }
                        else if (Maps.FIRE_POWER.Contains(tempObject))
                        {

                            IsDead = GetDamage((int)Char.GetNumericValue(tempObject));
                            if (IsDead == true)
                            {
                                IsDead = false;
                                //Recover founded medkites, doors and cards.
                                foreach (KeyValuePair<Point, char> _object in tempObjectsInThisWay)
                                {
                                    map.ChangeObject(_object.Key, _object.Value);
                                }
                                Point[] result = new Point[2];
                                currentXP = tempCurrentHP;
                                Point pointBeforeDiePoint = path[i - 1];
                                Point diePoint = positionInTheWay;
                                result[0] = diePoint;
                                result[1] = pointBeforeDiePoint;
                                return result;
                            }
                        }
                        else
                        {
                            throw new ArgumentException("There is something unknown in the way!", $"{tempObject}");
                        }
                    }
                    tempObjects.Add(tempObjectsInThisWay);
                    return null;
                }

                /// <summary>
                /// Check if there are doors in the way.
                /// </summary>
                /// <param name="way"></param>
                /// <returns>Returns path, if there is no doors and fire. Otherwise null, but
                /// keep founded doors in allDoorsInTheSecondStep.</returns>
                Point[] CheckDoorsInTheWay(Point[] way)
                {
                    //Use it for all needed doors to get to Q. Bool to check if already check the way to the door.
                    allDoorsInTheSecondStep = new Dictionary<Point, bool>();

                    //Fill the dictionary by the path from the first step.
                    foreach (Point doorPosition in ReturnDoorsInThePath(way))
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
                            if (notVisitedDoor.Equals(Point.nullPoint))
                            {
                                throw new System.ArgumentException("It is not a visited door!", "notVisitedDoor");
                            }


                            char doorKey = Char.ToLower(map.ReturnObject(notVisitedDoor));

                            //Find a way to the key.
                            Point[] tempPathToTheKey = FindPath(notVisitedDoor, map.ReturnAnElementPositionOnMap(doorKey));

                            if (tempPathToTheKey == null)
                            {
                                throw new ArgumentException("There are no way to the Q!");
                            }

                            //Check if there is any door in the key way.

                            foreach (Point tempDoor in ReturnDoorsInThePath(tempPathToTheKey))
                            {
                                //Add another door in the dictionary if it in the way to the card and if it is not already in the dictionary.
                                if (allDoorsInTheSecondStep.ContainsKey(tempDoor) != true)
                                {
                                    allDoorsInTheSecondStep.Add(tempDoor, false);
                                }
                            }

                            //This door have became visited.
                            allDoorsInTheSecondStep[notVisitedDoor] = true;
                        }
                    }
                    else
                    {
                        if (IsThereFire(way) == false)
                        {
                            //If there is no any door in the way to Q - return it.
                            return way;
                        }
                    }

                    return null;
                }

                Point[] FindCardsAndGothroughFire()
                {
                    if (allDoorsInTheSecondStep.Any())
                    {
                        //Before looping
                        foreach (Point door in allDoorsInTheSecondStep.Keys)
                        {
                            allKeysInTheThirdStep.Add(map.ReturnAnElementPositionOnMap(Char.ToLower(map.ReturnObject(door))));
                        }

                        //The result of the algorithm.
                        List<Point> theWholePath = new List<Point>();

                        while (allKeysInTheThirdStep.Any() == true)
                        {
                            //Counting steps to all keys
                            Dictionary<Point, int> numberOfStepsToKeys = ReturnClosestObjects(currentPosition, allKeysInTheThirdStep.ToArray());

                            //The closest available card.
                            Point goal;
                            Point[] pathToTheGoal;

                            bool flagIfDoor = false;

                            do
                            {

                                //If all keys were deleted from the list cause
                                //there are doors in the ways - it's impossible to reach Q.
                                if (numberOfStepsToKeys.Any() == false)
                                {
                                    throw new ArgumentException("There are no way to the Q!");
                                }
                                //Searching for a key with the minimal number of steps.
                                goal = numberOfStepsToKeys.First().Key;

                                pathToTheGoal = FindPath(currentPosition, goal);

                                //If there is a door - switch to another closest door.
                                flagIfDoor = IsThereDoor(pathToTheGoal);

                                if (flagIfDoor == true)
                                {
                                    //Remove the key from the list, cause it can't be reached now.
                                    numberOfStepsToKeys.Remove(goal);
                                }

                            } while (flagIfDoor == true);


                            //We got the closet availiable card.
                            //Creating two temp path to the goal for checking fire in the way.

                            //Keeps whole way from one card to another
                            List<Point> tempPathToTheGoal = new List<Point>(pathToTheGoal);
                            //If the 'character' died in the way - the way was changed.
                            bool IsChanged = false;

                            //Used for keeping goal as target of the way.
                            List<Point> tempPathToTheGoal2 = new List<Point>(pathToTheGoal);





                            //Trying to pass to the card despite the fire and find medkits.
                            do
                            {
                                IsDead = false;
                                Point[] resultOfIsDeadInTheWay = IsDeadInTheWay(tempPathToTheGoal2.ToArray());
                                if (resultOfIsDeadInTheWay != null)
                                {
                                    Point pointBeforeDiePoint = resultOfIsDeadInTheWay[1];
                                }
                                else
                                {
                                    IsDead = true;
                                }

                                if (IsDead == false)
                                {
                                    if (IsChanged == true)
                                    {
                                        tempPathToTheGoal.AddRange(tempPathToTheGoal2);
                                    }
                                }
                                else
                                {

                                    if (IsChanged == false)
                                    {
                                        tempPathToTheGoal = new List<Point>(FindWayToNearestMedkit(currentPosition, pointBeforeDiePoint));
                                        IsChanged = true;
                                    }
                                    else
                                    {
                                        tempPathToTheGoal.AddRange(FindWayToNearestMedkit(currentPosition, pointBeforeDiePoint));
                                    }
                                    currentPosition = tempPathToTheGoal.Last();
                                    tempPathToTheGoal.Remove(tempPathToTheGoal.Last());
                                    tempPathToTheGoal2 = new List<Point>(FindPath(currentPosition, goal));
                                }
                            } while (IsDead == true);

                            if (tempPathToTheGoal.SequenceEqual(pathToTheGoal) == false)
                            {
                                pathToTheGoal = tempPathToTheGoal.ToArray();
                            }

                            //Remove the key from the goal list.
                            allKeysInTheThirdStep.Remove(goal);
                            currentPosition = pathToTheGoal.Last();
                            theWholePath.AddRange(pathToTheGoal);
                            theWholePath.Remove(theWholePath.Last());
                        }

                        Point[] pathToTheExit = FindPath(currentPosition, map.ReturnAnElementPositionOnMap('Q'));

                        theWholePath.AddRange(pathToTheExit);

                        currentPosition = pathToTheExit.Last();
                        return theWholePath.ToArray();

                    }
                    else
                    {
                        //If we didn't find a door but found fire - check if die in the way.
                        Point[] resultOfIsDeadInTheWay = IsDeadInTheWay(theWay);

                        if (resultOfIsDeadInTheWay != null)
                        {
                            IsDead = true;
                        }
                        else
                        {
                            return theWay;
                        }
                        if (IsDead == true)
                        {
                            //NOTE I don't know when to use it.
                            differentWaysToQ.Remove(differentWaysToQ.First());
                            
                            //If die - try to find to another way to Q and a closest medkit.
                            Point[] resultOfFindDifferentPathToQ = FindPath(currentPosition, map.Q, visitedWaysToQ.ToArray());
                            if (resultOfFindDifferentPathToQ != null)
                            {
                                if(visitedWaysToQ.Contains(resultOfFindDifferentPathToQ) != true)
                                {
                                    differentWaysToQ.Add(resultOfFindDifferentPathToQ);
                                    return null;
                                }
                            }
                            //TODO Remove it from here!!
                            List<Point> wayToMedkit = new List<Point>(ReturnWayToFirstClosestMedkit(currentPosition, resultOfIsDeadInTheWay[1]));
                            currentPosition = wayToMedkit.Last();
                            allVisitedPositions.Add(currentPosition);
                            wayToMedkit.Remove(wayToMedkit.Last());

                            wayToMedkit.AddRange(FindPath(currentPosition, map.ReturnAnElementPositionOnMap('Q')));
                        }
                        else
                        {
                            //if didn't die - return path.
                            return firstStep;
                        }

                    }
                }
            }
            
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

        private void UseMedkit(Point position)
        {
            currentXP = MAX_XP;
            map.DeleteObject(position);
        }

        /// <summary>
        /// Uses Dijkstra's algorithm to find the closest medkit.
        /// </summary>
        /// <param name="start">Start position where the search starts</param>
        /// <param name="deathPoint">Position, that doesn't include in the search. </param>
        /// <returns>Returns a path to the closest medkit.</returns>
        private Point ReturnClosestObject(Point start, Point deathPoint, char _object)
        {
            Cell source = new Cell(start, 0, null);
            Cell deathCell = new Cell(deathPoint, 0, null);
            List<Cell> openList = new List<Cell>()
            {
                source,
                
            };
            List<Cell> closedList = new List<Cell>()
            {
                deathCell
            };

            while(openList.Any() == true)
            {
                IEnumerable<Cell> query = openList.OrderBy(x => x.G);
                Cell current = query.First();
                openList.Remove(current);
                closedList.Add(current);
                Point[] neighbors = map.ReturnNeighbours(current.Point);
                foreach (Point neighbor in neighbors)
                {
                    Cell newCell = new Cell(neighbor, current.G + POINTSRANGE, current);
                    if (map.ReturnObject(newCell.Point) == _object)
                        return newCell.Point;
                    if (closedList.Contains(newCell) == false)
                    {

                        if (openList.Contains(newCell) == true)
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

            return Point.nullPoint;
        }
        /// <summary>
        /// Uses Dijkstra's algorithm to find the closest object.
        /// </summary>
        /// <param name="start">Start position where the search starts</param>
        /// <returns>Returns a path to the closest object.</returns>
        private Point ReturnClosestObject(Point start, char _object)
        {
            return ReturnClosestObject(start, Point.nullPoint, _object);
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
            return Point.nullPoint;
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
        private Point[] ReturnDoorsInThePath(Point[] path)
        {
            List<Point> result = new List<Point>();
            foreach (Point position in path)
            {
                char tempObject = map.ReturnObject(position);
                if (tempObject == '.')
                    continue;
                else if (Maps.DOORS.Contains(tempObject))
                {
                    if(result.Contains(position) != true)
                    {
                        result.Add(position);
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Check, if there is at least one object in the path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="_objects"></param>
        /// <returns>Returns true if found, else false.</returns>
        private bool IsThereObjectInWay(Point[] path, char[] _objects)
        {
            foreach (Point position in path)
            {
                char tempObject = map.ReturnObject(position);
                if (tempObject == '.')
                    continue;
                else if (_objects.Contains(tempObject))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsThereFire(Point[] path)
        {
            return IsThereObjectInWay(path, Maps.FIRE_POWER);
        }

        private bool IsThereDoor(Point[] path)
        {
            return IsThereObjectInWay(path, Maps.DOORS);
        }


        /// <summary>
        /// Uses A* algorithm to find the shortest way to the goal.
        /// </summary>
        /// <param name="start">The start Point.</param>
        /// <param name="goal">The destiny point.</param>
        /// <returns>The array of Point from which the path is.</returns>
        private Point[] FindPath(Point start, Point goal)
        {
            return FindPath(start, goal, Point.nullPoint);
        }
        /// <summary>
        /// Uses A* algorithm to find the shortest way to the goal.
        /// </summary>
        /// <param name="start">The start Point.</param>
        /// <param name="goal">The destiny point.</param>
        /// <returns>The array of Point from which the path is.</returns>
        private Point[] FindPath(Point start, Point goal, Point[] diePoints)
        {
            Cell source = new Cell() { Parent = null, Point = start };
            Cell[] dieCells = new Cell[diePoints.Length];

            int i = 0;
            foreach (Point diePoint in diePoints)
            {
                dieCells[i] = new Cell() { Parent = null, Point = diePoint };
                i++;
            }
            
            List<Cell> openList = new List<Cell>()
            {
                source
            };
            List<Cell> closedList = new List<Cell>(dieCells);
            while (openList.Any() == true)
            {
                IEnumerable<Cell> query = openList.OrderBy(x => x.F);
                Cell current = query.First();
                openList.Remove(current);
                closedList.Add(current);
                Point[] neighbors = map.ReturnNeighbours(current.Point).ToArray();
                foreach (Point neighbor in neighbors)
                {
                    Cell newCell = new Cell(neighbor, current.G + POINTSRANGE, ReturnH(neighbor, goal), current);
                    if (newCell.Point.Equals(goal) == true)
                        return ReturnPath(newCell);
                    if (closedList.Contains(newCell) == false)
                    {

                        if (openList.Contains(newCell) == true)
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
        /// Uses A* algorithm to find the shortest way to the goal.
        /// </summary>
        /// <param name="start">The start Point.</param>
        /// <param name="goal">The destiny point.</param>
        /// <returns>The array of Point from which the path is.</returns>
        private Point[] FindPath(Point start, Point goal, Point diePoint)
        {
            return FindPath(start, goal, new Point[] { diePoint });
        }
        /// <summary>
        /// Uses Dijkstra's algorithm to find shortest way to the goal, trying not include visited ways.
        /// </summary>
        /// <param name="start">Start position where the search starts</param>
        /// <param name="arrayOfPaths">Array of visited paths.</param>
        /// <returns>Returns a closest path to the goal.</returns>
        private Point[] FindPath(Point start, Point goal, Point[][] arrayOfPaths)
        {
            Cell source = new Cell(start, 0, null);
            List<Cell> openList = new List<Cell>()
            {
                source,

            };

            int numberOfCells = 0;
            for (int i = 0; i < arrayOfPaths.Length; i++)
            {
                for (int j = 0; j < arrayOfPaths[i].Length; j++)
                {
                    numberOfCells++;
                }
            }

            Point[] visitedCells = new Point[numberOfCells];
            int _index = 0;
            for (int i = 0; i < arrayOfPaths.Length; i++)
            {
                for (int j = 0; j < arrayOfPaths[i].Length; j++)
                {
                    visitedCells[_index] = new Point(arrayOfPaths[i][j]);
                    _index++;
                }
            }

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
                    Cell newCell = null;
                    if (visitedCells.Contains(neighbor))
                    {
                        newCell = new Cell(neighbor, current.G + POINTSRANGE + map.AreaOfMap, current);
                    }
                    else
                    {
                        newCell = new Cell(neighbor, current.G + POINTSRANGE, current);
                    }
                    if (newCell.Point.Equals(goal))
                        return ReturnPath(newCell);
                    if (closedList.Contains(newCell) == false)
                    {

                        if (openList.Contains(newCell) == true)
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

        /// <summary>
        /// Deletes a key and the door.
        /// </summary>
        /// <param name="keyPosition"></param>
        /// <param name="key"></param>
        /// <returns>Door position.</returns>
        private Point DeleteDoorAndKey(Point keyPosition, char key)
        {
            map.DeleteObject(keyPosition);
            char doorC = Char.ToUpper(key);
            Point doorP = map.ReturnAnElementPositionOnMap(doorC);
            map.DeleteObject(doorP);
            return doorP;
        }

        private Point[] ReturnWayToFirstClosestMedkit(Point currentPosition, Point pointBeforeDiePoint)
        {
            Point[] medkitsPositions = map.ReturnElementsPositionsOnMap(Maps.MEDKIT);
            if(medkitsPositions == null)
            {
                throw new ArgumentException("There is no a medkit!");
            }
            Point[] closestMedkits = FindClosestObjects(pointBeforeDiePoint, medkitsPositions);

            for (int i = 0; i < closestMedkits.Length; i++)
            {
                Point[] wayToMedkit = FindPath(currentPosition, closestMedkits[i]);
                if(wayToMedkit != null)
                {
                    return wayToMedkit;
                }
            }

            throw new ArgumentException("There are no ways to medkits!");
        }
        /// <summary>
        /// Order objects from start point by numberOfSteps.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="listOfObjects"></param>
        /// <returns>Returns a sorted dictionary of closest ways to the objects.</returns>
        private Dictionary<Point[], int> ReturnClosestWaysToObjectsDictionary(Point startPoint, Point[] listOfObjects)
        {
            Dictionary<Point[], int> numberOfStepsToObjects = new Dictionary<Point[], int>();
            foreach (Point positionOfObject in listOfObjects)
            {
                Point[] pathToTheObject = FindPath(startPoint, positionOfObject);
                if(pathToTheObject != null)
                {
                    numberOfStepsToObjects.Add(pathToTheObject, pathToTheObject.Length);
                }
            }

            //Sort the dictonary from smaller to higher.
            var result = numberOfStepsToObjects.OrderBy(key => key.Value);

            return result.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Order objects from start point by numberOfSteps.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="listOfObjects"></param>
        /// <returns>Returns a sorted array of closest ways to the objects.</returns>
        private Point[][] ReturnClosestWaysToObjects(Point startPoint, Point[] listOfObjects)
        {
            Dictionary<Point[], int> numberOfStepsToObjects = new Dictionary<Point[], int>();
            foreach (Point positionOfObject in listOfObjects)
            {
                Point[] pathToTheObject = FindPath(startPoint, positionOfObject);
                if (pathToTheObject != null)
                {
                    numberOfStepsToObjects.Add(pathToTheObject, pathToTheObject.Length);
                }
            }

            //Sort the dictonary from smaller to higher.
            var result = numberOfStepsToObjects.OrderBy(key => key.Value);

            Dictionary<Point[], int> resultDictionary = result.ToDictionary(pair => pair.Key, pair => pair.Value);

            return resultDictionary.Keys.ToArray();
        }

        private Point[] FindClosestObjects(Point startPoint, Point[] listOfObjects)
        {
            Dictionary<Point, int> numberOfStepsToObjects = new Dictionary<Point, int>();
            foreach (Point positionOfObject in listOfObjects)
            {
                Point[] pathToTheObject = FindPath(startPoint, positionOfObject);
                if (pathToTheObject != null)
                {
                    numberOfStepsToObjects.Add(positionOfObject, pathToTheObject.Length);
                }
            }

            //Sort the dictonary from smaller to higher.
            var result = numberOfStepsToObjects.OrderBy(key => key.Value);

            Dictionary<Point, int> resultDictionary = result.ToDictionary(pair => pair.Key, pair => pair.Value);

            return resultDictionary.Keys.ToArray();
        }
    }
}
