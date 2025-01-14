using Maze_Solving.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Maze_Solving.ViewModels.ControlViewModels
{
    /// <summary>
    /// View model for the cell
    /// </summary>
    public class CellViewModel : BaseViewModel
    {
        #region Fields

        private double _cellSize = 50;
        private CellType _cellType = CellType.Empty;

        #endregion 

        #region Properties

        /// <summary>
        /// Height of the cell
        /// </summary>
        public double CellSize
        {
            get => _cellSize;
            set
            {
                _cellSize = value;
                OnPropertyChanged(nameof(CellSize));
            }
        }

        /// <summary>
        /// Type of the cell
        /// </summary>
        public CellType CellType
        {
            get => _cellType;
            set
            {
                _cellType = value;
                OnPropertyChanged(nameof(CellType));
                Debug.WriteLine($"CellType changed to: {_cellType}");  // Debug log
            }
        }

        /// <summary>
        /// Position of the cell
        /// </summary>
        public KeyValuePair<int, int> Position { get; set; }

        #endregion

        #region Commands

        public ICommand SelectStartEndPoint { get; }
        #endregion

        public CellViewModel(ICommand selectStartEndPoint)
        {
            SelectStartEndPoint = selectStartEndPoint;
        }
    }
}
