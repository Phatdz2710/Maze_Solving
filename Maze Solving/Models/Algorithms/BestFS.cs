using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Maze_Solving.Models.Algorithms
{
    /// <summary>
    /// Best First Search algorithm
    /// </summary>
    class BestFS : SearchAlgorithm
    {
        public override IEnumerable<(int X, int Y)> Solve()
        {
            if (_maze == null)
            {
                MessageBox.Show("Maze is not loaded");
                yield break;
            }

            // Create a visited array
            var visited = new bool[_maze.MazeHeight, _maze.MazeWidth];

            // Create a sort Dict for BestFS
            var priorityQueue = new PriorityQueue<Node, double>();
            // Dictionary<Node, int> bestWeight = new Dictionary<Node, int>();

            // Start from the start point
            priorityQueue.Enqueue(new Node()
            {
                X = _maze.Start.Key,
                Y = _maze.Start.Value,
                Parent = null,
                DistanceToStart = 0
            }, 0);


            //visited[_maze.Start.Key, _maze.Start.Value] = true;

            while (priorityQueue.Count > 0)
            {
                // Get the current point
                var current = priorityQueue.Dequeue();

                // Yield the current point
                yield return (current.X, current.Y);

                // Check if we reached the end point
                if (current.X == _maze.End.Key && current.Y == _maze.End.Value)
                {
                    MessageBox.Show("Path found");
                    Head = current;
                    yield break;
                }

                // Check the neighbors
                foreach (var direction in directions)
                {
                    var x = current.X + direction.Value.Key;
                    var y = current.Y + direction.Value.Value;

                    // Check if the point is valid
                    if (x < 0 || x >= _maze.MazeHeight || y < 0 || y >= _maze.MazeWidth || visited[x, y] || _maze.ListCells[x][y] == '#')
                    {
                        continue;
                    }

                    // Calculate the weight
                    var weight = calculateWeight(new KeyValuePair<int, int>(x, y));
                    priorityQueue.Enqueue(new Node()
                    {
                        X = x,
                        Y = y,
                        Parent = current,
                    }, calculateWeight(new KeyValuePair<int, int>(x, y))); 

                    visited[x, y] = true;
                }
            }

            MessageBox.Show("Path not found");
        }

        
        /// <summary>
        /// Calculate the weight using the Euclidean distance
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private double calculateWeight(KeyValuePair<int, int> point)
        {
            return Math.Sqrt(Math.Pow(point.Key - _maze.End.Key, 2) + Math.Pow(point.Value - _maze.End.Value, 2));
        }



        public override IEnumerable<(int X, int Y)> TraceBack()
        {
            if (Head == null)
            {
                yield break;
            }

            var current = Head;
            while (current != null)
            {
                yield return (current.X, current.Y);
                current = current.Parent;
            }
        }
    }
}
