using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Maze_Solving.Models.Algorithms
{
    /// <summary>
    /// Breadth First Search algorithm
    /// </summary>
    public class BFS : SearchAlgorithm
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BFS()
        {
        }

        /// <summary>
        /// Run BFS algorithm
        /// </summary>
        public override IEnumerable<(int X, int Y)> Solve()
        {
            if (_maze == null)
            {
                MessageBox.Show("Maze is not loaded");
                yield break;
            }

            // Create a visited array
            var visited = new bool[_maze.MazeHeight, _maze.MazeWidth];

            // Create a queue for BFS
            Queue<Node> queue = new Queue<Node>();

            // Start from the start point
            queue.Enqueue(new Node()
            {
                X = _maze.Start.Key,
                Y = _maze.Start.Value,
                Parent = null
            });

            visited[_maze.Start.Key, _maze.Start.Value] = true;

            while (queue.Count > 0)
            {
                // Get the current point
                var current = queue.Dequeue();

                // Yield the current point
                yield return (current.X, current.Y);

                // Check if we reached the end point
                if (current.X == _maze.End.Key && current.Y == _maze.End.Value)
                {
                    MessageBox.Show("Path found");
                    Head = current;
                    yield break;
                }

                // Check all directions
                foreach (var direction in directions)
                {
                    var x = current.X + direction.Value.Key;
                    var y = current.Y + direction.Value.Value;

                    // Check if the move is valid
                    if (IsValidMove(x, y) && !visited[x, y])
                    {
                        queue.Enqueue(new Node()
                        {
                            X = x,
                            Y = y,
                            Parent = current
                        });
                        visited[x, y] = true;
                    }
                }


            }

            MessageBox.Show("Path not found");
        }

        /// <summary>
        /// Trace back the path
        /// </summary>
        /// <returns></returns>
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
