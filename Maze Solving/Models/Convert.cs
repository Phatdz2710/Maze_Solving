using Maze_Solving.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Maze_Solving.Models
{
    /// <summary>
    /// Convert class
    /// </summary>
    public partial class Convert
    {
        /// <summary>
        /// Convert cell type to char
        /// </summary>
        /// <param name="cellType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static char CellTypeToChar(CellType cellType)
        {
            return cellType switch
            {
                CellType.Empty => '.',
                CellType.Wall => '#',
                CellType.Start => 'S',
                CellType.End => 'E',
                CellType.Path => '@',
                _ => throw new ArgumentOutOfRangeException(nameof(cellType), cellType, null)
            };
        }


        /// <summary>
        /// Convert char to cell type
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static CellType CharToCellType(char c)
        {
            return c switch
            {
                '.' => CellType.Empty,
                '#' => CellType.Wall,
                'S' => CellType.Start,
                'E' => CellType.End,
                '@' => CellType.Path,
                _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
            };
        }
    }
}
