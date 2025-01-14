using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Maze_Solving.Models.Algorithms
{
    /// <summary>
    /// Depth First Search algorithm
    /// </summary>
    public class DFS : SearchAlgorithm
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
            TotalCost = 0;

            // Create a visited array
            var visited = new bool[_maze.MazeHeight, _maze.MazeWidth];

            // Create a stack for DFS
            Stack<Node> stack = new Stack<Node>();

            // Start from the start point
            stack.Push(new Node()
            {
                X = _maze.Start.Key,
                Y = _maze.Start.Value,
                Parent = null
            });

            visited[_maze.Start.Key, _maze.Start.Value] = true;

            while (stack.Count > 0)
            {
                // Get the current point
                var current = stack.Pop();

                // Yield the current point
                yield return (current.X, current.Y);

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

                    // Check if the new point is valid
                    if (IsValidMove(x, y) && !visited[x, y])
                    {
                        stack.Push(new Node()
                        {
                            X = x,
                            Y = y,
                            Parent = current
                        });

                        TotalCost += 1;
                        visited[x, y] = true;
                    }
                }
            }

            stopwatch.Stop();
            TotalTime = stopwatch.ElapsedMilliseconds;
            MessageBox.Show("No path found");
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
                PathCost++;
            }
        }
    }
}
