using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Maze_Solving.Models.Algorithms
{
    public class AStarPlus : SearchAlgorithm
    {
        Node? Head1;
        public override IEnumerable<(int X, int Y)> Solve()
        {
            if (_maze == null)
            {
                MessageBox.Show("Maze is not loaded");
                yield break;
            }

            // Calculate cost for algorithms
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            TotalCost = 0;

            // Create a visited array
            var visitedS = new bool[_maze.MazeHeight, _maze.MazeWidth];
            var visitedE = new bool[_maze.MazeHeight, _maze.MazeWidth];

            // Create a priority queue for AStar from the start point
            var sortDictStart = new SortedDictionary<Node, double>();

            // Create a priority queue for AStar from the end point
            var sortDictEnd = new SortedDictionary<Node, double>();

            // Start from the start point
            sortDictStart[new Node()
            {
                X = _maze.Start.Key,
                Y = _maze.Start.Value,
                Parent = null,
                DistanceToStart = 0
            }] = 0;

            // Start from the end point
            sortDictEnd[new Node()
            {
                X = _maze.End.Key,
                Y = _maze.End.Value,
                Parent = null,
                DistanceToStart = 0
            }] = 0;


            //visited[_maze.Start.Key, _maze.Start.Value] = true;

            while (sortDictStart.Count > 0 || sortDictEnd.Count > 0)
            {
                if (sortDictStart.Count > 0)
                {
                    // Get the current point
                    var currentS = sortDictStart.OrderBy(x => x.Value).First().Key;

                    // Yield the current point
                    yield return (currentS.X, currentS.Y);

                    sortDictStart.Remove(currentS);

                    // If this touch the start point
                    if (visitedE[currentS.X, currentS.Y])
                    {
                        MessageBox.Show("Path found");
                        Head = currentS;

                        foreach(var node in sortDictEnd)
                        {
                            if (node.Key.X == currentS.X && node.Key.Y == currentS.Y)
                            {
                                Head1 = node.Key;
                                break;
                            }
                        }

                        stopwatch.Stop();
                        TotalTime = stopwatch.ElapsedMilliseconds;
                        yield break;
                    }

                    // Check if we reached the end point
                    if (currentS.X == _maze.End.Key && currentS.Y == _maze.End.Value)
                    {
                        MessageBox.Show("Path found");
                        Head = currentS;
                        stopwatch.Stop();
                        TotalTime = stopwatch.ElapsedMilliseconds;

                        yield break;
                    }

                    // Check the neighbors
                    foreach (var direction in directions)
                    {
                        var x = currentS.X + direction.Value.Key;
                        var y = currentS.Y + direction.Value.Value;

                        var newNode = new Node()
                        {
                            X = x,
                            Y = y,
                            Parent = currentS,
                            DistanceToStart = currentS.DistanceToStart + 1
                        };

                        // Update heuristic of node if this node is already in the sortDict
                        if (sortDictStart.ContainsKey(newNode))
                        {
                            if (sortDictStart[newNode] > HeuristicForStart(newNode, sortDictEnd))
                            {
                                sortDictStart.Remove(newNode);
                                sortDictStart[newNode] = HeuristicForStart(newNode, sortDictEnd);
                            }

                            continue;
                        }

                        // Check if the point is valid
                        if (x < 0 || x >= _maze.MazeHeight || y < 0 || y >= _maze.MazeWidth || visitedS[x, y] || _maze.ListCells[x][y] == '#')
                        {
                            continue;
                        }
                        TotalCost++;
                        sortDictStart[newNode] = HeuristicForStart(newNode, sortDictEnd);

                        visitedS[x, y] = true;
                    }
                }

                if (sortDictEnd.Count > 0)
                {
                    // Get the current point
                    var currentE = sortDictEnd.OrderBy(x => x.Value).First().Key;

                    // Yield the current point
                    yield return (currentE.X, currentE.Y);

                    sortDictEnd.Remove(currentE);

                    // If this touch the start point
                    if (visitedS[currentE.X, currentE.Y])
                    {
                        MessageBox.Show("Path found");
                        Head = currentE;

                        foreach (var node in sortDictStart)
                        {
                            if (node.Key.X == currentE.X && node.Key.Y == currentE.Y)
                            {
                                Head1 = node.Key;
                                break;
                            }
                        }
                        stopwatch.Stop();
                        TotalTime = stopwatch.ElapsedMilliseconds;

                        yield break;
                    }

                    // Check if we reached the end point
                    if (currentE.X == _maze.Start.Key && currentE.Y == _maze.Start.Value)
                    {
                        MessageBox.Show("Path found");
                        Head = currentE;
                        stopwatch.Stop();
                        TotalTime = stopwatch.ElapsedMilliseconds;

                        yield break;
                    }

                    // Check the neighbors
                    foreach (var direction in directions)
                    {
                        var x = currentE.X + direction.Value.Key;
                        var y = currentE.Y + direction.Value.Value;

                        var newNode = new Node()
                        {
                            X = x,
                            Y = y,
                            Parent = currentE,
                            DistanceToStart = currentE.DistanceToStart + 1
                        };

                        // Update heuristic of node if this node is already in the sortDict
                        if (sortDictEnd.ContainsKey(newNode))
                        {
                            if (sortDictEnd[newNode] > HeuristicForEnd(newNode, sortDictStart))
                            {
                                sortDictEnd.Remove(newNode);
                                sortDictEnd[newNode] = HeuristicForEnd(newNode, sortDictStart);
                            }

                            continue;
                        }

                        // Check if the point is valid
                        if (x < 0 || x >= _maze.MazeHeight || y < 0 || y >= _maze.MazeWidth || visitedE[x, y] || _maze.ListCells[x][y] == '#')
                        {
                            continue;
                        }
                        TotalCost++;

                        sortDictEnd[newNode] = HeuristicForEnd(newNode, sortDictStart);

                        visitedE[x, y] = true;
                    }
                }
            } 

            stopwatch.Stop();
            TotalTime = stopwatch.ElapsedMilliseconds;


            MessageBox.Show("Path not found");
        }

        /// <summary>
        /// Calculate the weight using the Euclidean distance
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private double calculateWeightForStart(KeyValuePair<int, int> point)
        {
            return Math.Sqrt(Math.Pow(point.Key - _maze.End.Key, 2) + Math.Pow(point.Value - _maze.End.Value, 2));
        }

        private double calculateWeightForEnd(KeyValuePair<int, int> point)
        {
            return Math.Sqrt(Math.Pow(point.Key - _maze.Start.Key, 2) + Math.Pow(point.Value - _maze.Start.Value, 2));
        }


        ///// <summary>
        ///// Calculate the distance to the start point
        ///// </summary>
        ///// <param name="point"></param>
        ///// <returns></returns>
        //private double calculateDistanceToStart(KeyValuePair<int, int> point)
        //{
        //    return Math.Sqrt(Math.Pow(point.Key - _maze.Start.Key, 2) + Math.Pow(point.Value - _maze.Start.Value, 2));
        //}


        /// <summary>
        /// Calculate the heuristic of a node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private double HeuristicForStart(Node node, SortedDictionary<Node, double> sortDictEnd)
        {
            // return calculateWeightForStart(new KeyValuePair<int, int>(node.X, node.Y));
            var min = double.MaxValue;
            foreach (var endNode in sortDictEnd)
            {
                var distance = Math.Sqrt(Math.Pow(node.X - endNode.Key.X, 2) + Math.Pow(node.Y - endNode.Key.Y, 2));
                if (distance < min)
                {
                    min = distance;
                }
            }

            return min;
        }

        private double HeuristicForEnd(Node node, SortedDictionary<Node, double> sortDictStart)
        {
            // return calculateWeightForEnd(new KeyValuePair<int, int>(node.X, node.Y));
            var min = double.MaxValue;
            foreach (var endNode in sortDictStart)
            {
                var distance = Math.Sqrt(Math.Pow(node.X - endNode.Key.X, 2) + Math.Pow(node.Y - endNode.Key.Y, 2));
                if (distance < min)
                {
                    min = distance;
                }
            }

            return min;
        }


        public override IEnumerable<(int X, int Y)> TraceBack()
        {
            if (Head == null)
            {
                yield break;
            }
            PathCost = 0;
            var current = Head;
            while (current != null)
            {
                yield return (current.X, current.Y);
                current = current.Parent;
                PathCost += 1;
            }

            current = Head1;
            while (current != null)
            {
                yield return (current.X, current.Y);
                current = current.Parent;
                PathCost += 1;
            }
        }
    }
}
