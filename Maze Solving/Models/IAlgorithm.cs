using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze_Solving.Models
{
    /// <summary>
    /// Interface for the algorithm
    /// </summary>
    public abstract class SearchAlgorithm
    {

        /// <summary>
        /// Maze
        /// </summary>
        protected Maze? _maze;

        public int TotalCost { get; set; } = 0;
        public int PathCost { get; set; } = 0;
        public long TotalTime { get; set; } = 0;
        protected class Node : IComparable<Node>
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Node? Parent { get; set; }
            public double DistanceToStart { get; set; } = 0;

            public int CompareTo(Node? other)
            {
                if (other == null)
                {
                    return 1;  // Nếu other là null, đối tượng hiện tại được coi là lớn hơn
                }

                // So sánh theo X
                int xComparison = this.X.CompareTo(other.X);
                if (xComparison != 0)
                {
                    return xComparison;  // Nếu X khác nhau, trả về kết quả so sánh X
                }

                // Nếu X bằng nhau, so sánh theo Y
                return this.Y.CompareTo(other.Y);
            }

            // Override Equals method to compare based on X and Y
            public override bool Equals(object? obj)
            {
                if (obj is Node otherNode)
                {
                    return this.X == otherNode.X && this.Y == otherNode.Y;
                }
                return false;
            }

            // Override GetHashCode method to return a hash based on X and Y
            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y);
            }

        }

        protected Node? Head = null;


        /// <summary>
        /// Directions
        /// </summary>
        protected readonly Dictionary<string, KeyValuePair<int, int>> directions = new Dictionary<string, KeyValuePair<int, int>>()
        {
            { "up", new KeyValuePair<int, int>(-1, 0) },
            { "down", new KeyValuePair<int, int>(1, 0) },
            { "left", new KeyValuePair<int, int>(0, -1) },
            { "right", new KeyValuePair<int, int>(0, 1) }
        };

        /// <summary>
        /// Solve the maze
        /// </summary>
        public abstract IEnumerable<(int X, int Y)> Solve();

        /// <summary>
        /// Trace back the path
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<(int X, int Y)> TraceBack();

        /// <summary>
        /// Input the maze
        /// </summary>
        /// <param name="maze"></param>
        public void InputMaze(Maze maze)
        {
            _maze = maze;
        }


        /// <summary>
        /// Check if the move is valid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsValidMove(int x, int y)
        {
            // Check if the move is valid
            if (_maze == null)
            {
                return false;
            }

            // Check if the move is within the maze
            if (x < 0 || x >= _maze.MazeHeight || y < 0 || y >= _maze.MazeWidth)
            {
                return false;
            }

            // Check if the move is not a wall
            if (_maze.ListCells[x][y] == '#')
            {
                return false;
            }

            return true;
        }
    }
}
