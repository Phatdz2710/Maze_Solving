using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Xps.Serialization;

namespace Maze_Solving.Models.Algorithms
{
    /// <summary>
    /// Uniform Cost Search algorithm
    /// </summary>
    class UCS : SearchAlgorithm
    {
        public override IEnumerable<(int X, int Y)> Solve()
        {
            if (_maze == null)
            {
                MessageBox.Show("Maze is not loaded");
                yield break;
            }

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // Create a visited array
            var visited = new bool[_maze.MazeHeight, _maze.MazeWidth];

            // Create a priority queue for UCS
            var sortDict = new SortedDictionary<Node, double>();
            // Dictionary<Node, int> bestWeight = new Dictionary<Node, int>();

            // Start from the start point
            sortDict[new Node()
            {
                X = _maze.Start.Key,
                Y = _maze.Start.Value,
                Parent = null,
                DistanceToStart = 0
            }] = 0;
            

            //visited[_maze.Start.Key, _maze.Start.Value] = true;

            while (sortDict.Count > 0)
            {
                // Get the current point
                var current = sortDict.OrderBy(x => x.Value).First().Key;

                // Yield the current point
                yield return (current.X, current.Y);

                sortDict.Remove(current);

                // Check if we reached the end point
                if (current.X == _maze.End.Key && current.Y == _maze.End.Value)
                {
                    MessageBox.Show("Path found");
                    Head = current;
                    stopwatch.Stop();
                    TotalTime = stopwatch.ElapsedMilliseconds;
                    yield break;
                }

                // Check the neighbors
                foreach (var direction in directions)
                {
                    var x = current.X + direction.Value.Key;
                    var y = current.Y + direction.Value.Value;

                    var newNode = new Node()
                    {
                        X = x,
                        Y = y,
                        Parent = current,
                        DistanceToStart = current.DistanceToStart + 1
                    };

                    if (sortDict.ContainsKey(newNode))
                    {
                        if (sortDict[newNode] > newNode.DistanceToStart)
                        {
                            sortDict.Remove(newNode);
                            sortDict[newNode] = newNode.DistanceToStart;
                        }

                        continue;
                    }

                    // Check if the point is valid
                    if (x < 0 || x >= _maze.MazeHeight || y < 0 || y >= _maze.MazeWidth || visited[x, y] || _maze.ListCells[x][y] == '#')
                    {
                        continue;
                    }

                    TotalCost += 1;
                    sortDict[newNode] = newNode.DistanceToStart;

                    visited[x, y] = true;
                }
            }

            stopwatch.Stop();
            TotalTime = stopwatch.ElapsedMilliseconds;

            MessageBox.Show("Path not found");
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
        }

    }
}
