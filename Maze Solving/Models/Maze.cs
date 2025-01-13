using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze_Solving.Models
{
    /// <summary>
    /// Maze class
    /// </summary>
    public class Maze
    {
        /// <summary>
        /// List of cells
        /// </summary>
        public List<string> ListCells { get; set; } = new List<string>();

        /// <summary>
        /// Width and height of the maze
        /// </summary>
        public int MazeWidth { get; set; }

        /// <summary>
        /// Height of the maze
        /// </summary>
        public int MazeHeight { get; set; }

        /// <summary>
        /// Start and end points
        /// </summary>
        public KeyValuePair<int, int> Start { get; set; }
        public KeyValuePair<int, int> End { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="listCells"></param>
        public Maze()
        {
            ListCells = new List<string>();
            MazeWidth = 0;
            MazeHeight = 0;
        }

        
        /// <summary>
        /// Input maze
        /// </summary>
        /// <param name="listCells"></param>
        public void InputMaze(List<string> listCells)
        {
            ListCells = listCells;
            MazeWidth = listCells[0].Length;
            MazeHeight = listCells.Count;
        }
    }
}
