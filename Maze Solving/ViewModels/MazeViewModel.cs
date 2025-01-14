using CommunityToolkit.Mvvm.Input;
using Maze_Solving.Models;
using Maze_Solving.Models.Algorithms;
using Maze_Solving.Models.Enums;
using Maze_Solving.ViewModels.ControlViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Maze_Solving.ViewModels
{
    public class MazeViewModel : BaseViewModel
    {
        #region Fields
        private double _mazeWidth = 580;
        private double _mazeHeight = 580;
        private int _speed = 10;

        private int _xStart = 0;
        private int _yStart = 0;

        private int _totalCost = 0;
        private int _pathCost = 0;
        private long _totalTime = 0;

        private Maze? _maze;

        private SearchAlgorithm? _algorithm;

        private SearchAlgorithm BFS = new BFS();
        private SearchAlgorithm DFS = new DFS();
        private SearchAlgorithm UCS = new UCS();
        private SearchAlgorithm BestFS = new BestFS();
        private SearchAlgorithm AStar = new AStar();
        private SearchAlgorithm AStarPlus = new AStarPlus();

        private bool _isChooseStartPoint = true;
        private bool _isChooseEndPoint = false;


        /// <summary>
        /// Semaphore for the signal to stop the algorithm
        /// </summary>
        private SemaphoreSlim _signal = new SemaphoreSlim(1, 1);

        #endregion

        #region Properties
        /// <summary>
        /// List of cells
        /// </summary>
        public ObservableCollection<CellViewModel> ListCells { get; set; }

        /// <summary>
        /// Width of the maze
        /// </summary>
        public double MazeWidth
        {
            get => _mazeWidth;
            set
            {
                _mazeWidth = value;
                OnPropertyChanged(nameof(MazeWidth));
            }
        }

        /// <summary>
        /// Height of the maze
        /// </summary>
        public double MazeHeight
        {
            get => _mazeHeight;
            set
            {
                _mazeHeight = value;
                OnPropertyChanged(nameof(MazeHeight));
            }
        }

        /// <summary>
        /// Algorithm's speed
        /// </summary>
        public int Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                OnPropertyChanged(nameof(Speed));
            }
        }

        /// <summary>
        /// Total cost of the algorithm
        /// </summary>
        public int TotalCost
        {
            get => _totalCost;
            set
            {
                _totalCost = value; OnPropertyChanged(nameof(TotalCost));
            }
        }

        /// <summary>
        /// Total cost of the path
        /// </summary>
        public int PathCost
        {
            get => _pathCost;
            set
            {
                _pathCost = value; 
                OnPropertyChanged(nameof(PathCost));
            }
        }

        /// <summary>
        /// Total time of the algorithm
        /// </summary>
        public long TotalTime
        {
            get => _totalTime;
            set
            {
                _totalTime = value;
                OnPropertyChanged(nameof(TotalTime));
            }
        }

        /// <summary>
        /// Choose start point
        /// </summary>
        public bool IsStartPointSelected
        {
            get => _isChooseStartPoint;
            set
            {
                _isChooseStartPoint = value;
                OnPropertyChanged(nameof(IsStartPointSelected));
            }
        }

        /// <summary>
        /// Choose end point
        /// </summary>
        public bool IsEndPointSelected
        {
            get => _isChooseEndPoint;
            set
            {
                _isChooseEndPoint = value;
                OnPropertyChanged(nameof(IsEndPointSelected));
            }
        }

        #endregion

        #region Commands
        public ICommand RunBFSCommand { get; }
        public ICommand RunDFSCommand { get; }
        public ICommand RunUCSCommand { get; }
        public ICommand RunBestFSCommand { get; }
        public ICommand RunAStarCommand { get; }
        public ICommand RunAStarPlusCommand { get; }
        public ICommand SelectStartEndPoint { get; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MazeViewModel()
        {
            ListCells = new ObservableCollection<CellViewModel>();

            RunBFSCommand = new RelayCommand(runBFSCommand);
            RunDFSCommand = new RelayCommand(runDFSCommand);
            RunUCSCommand = new RelayCommand(runUCSCommand);
            RunBestFSCommand = new RelayCommand(runBestFSCommand);
            RunAStarCommand = new RelayCommand(runAStarCommand);
            RunAStarPlusCommand = new RelayCommand(runAStarPlusCommand);
            SelectStartEndPoint = new RelayCommand<KeyValuePair<int, int>>(selectStartEndPoint);

        }

        #region Initialized Methods


        /// <summary>
        /// Load maze from file
        /// </summary>
        /// <param name="filePath"></param>
        public void InitializeMaze(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                try
                {
                    List<string> rows = new List<string>();
                    string? line;
                    int maxRowLength = 0;
                    bool isStartFound = false;
                    bool isEndFound = false;
                    MazeHeight = 580;
                    MazeWidth = 580;

                    // Clear list of cell to add new cell
                    ListCells.Clear();

                    if (_maze == null)
                    {
                        _maze = new Maze();
                    }

                    // Read all lines
                    while ((line = sr.ReadLine()) != null)
                    {
                        foreach (var c in line)
                        {
                            // Check if maze have multiple start or end points
                            if (c == 'S')
                            {
                                if (isStartFound)
                                {
                                    MessageBox.Show("Maze have multiple start points", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                isStartFound = true;
                                _maze.Start = new KeyValuePair<int, int>(rows.Count, line.IndexOf(c));
                            }
                            else if (c == 'E')
                            {
                                if (isEndFound)
                                {
                                    MessageBox.Show("Maze have multiple end points", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                isEndFound = true;
                                _maze.End = new KeyValuePair<int, int>(rows.Count, line.IndexOf(c));

                            }
                        }

                        rows.Add(line);
                        if (line.Length > maxRowLength)
                        {
                            maxRowLength = line.Length;
                        }
                    }

                    // Check if start and end points are found
                    if (!isStartFound || !isEndFound)
                    {
                        // Show error message
                        MessageBox.Show("Maze not have start or end point", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Convert maze to Rectangular maze
                    for (int i = 0; i < rows.Count; i++)
                    {
                        for (int j = rows[i].Length - 1; j < maxRowLength - 1; j++)
                        {
                            rows[i] += ".";
                        }
                    }

                    // Input maze to maze model
                    _maze?.InputMaze(rows);

                    // Calculate cell size and maze size
                    var tempCellWidth = MazeWidth / maxRowLength;
                    var tempCellHeight = MazeHeight / rows.Count;
                    var cellSize = 0.0;

                    if (tempCellWidth > tempCellHeight)
                    {
                        MazeWidth = tempCellHeight * maxRowLength;
                        cellSize = tempCellHeight;
                    }
                    else
                    {
                        MazeHeight = tempCellWidth * rows.Count;
                        cellSize = tempCellWidth;
                    }

                    // Initialize maze
                    for (int i = 0; i < rows.Count; i++)
                    {
                        for (int j = 0; j < rows[i].Length; j++)
                        {
                            ListCells.Add(new CellViewModel(SelectStartEndPoint)
                            {
                                CellSize = cellSize,
                                Position = new KeyValuePair<int, int>(i, j),
                                CellType = Models.Convert.CharToCellType(rows[i][j])
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Cannot open this file! Please re-check your characters!");
                }
            }
        }



        /// <summary>
        /// Set algorithm to BFS and run
        /// </summary>
        private void runBFSCommand()
        {
            // Check if algorithm is initialized
            if (_algorithm != BFS)
            {
                _algorithm = BFS;
            }

            runSearchAlgorithms();
        }


        /// <summary>
        /// Set algorithm to DFS and run
        /// </summary>
        private void runDFSCommand()
        {
            // Check if algorithm is initialized
            if (_algorithm != DFS)
            {
                _algorithm = DFS;
            }

            runSearchAlgorithms();
        }

        /// <summary>
        /// Set algorithm to UCS and run
        /// </summary>
        private void runUCSCommand()
        {
            // Check if algorithm is initialized
            if (_algorithm != UCS)
            {
                _algorithm = UCS;
            }

            runSearchAlgorithms();
        }

        /// <summary>
        /// Set algorithm to UCS and run
        /// </summary>
        private void runBestFSCommand()
        {
            // Check if algorithm is initialized
            if (_algorithm != BestFS)
            {
                _algorithm = BestFS;
            }

            runSearchAlgorithms();
        }

        /// <summary>
        /// Set algorithm to UCS and run
        /// </summary>
        private void runAStarCommand()
        {
            // Check if algorithm is initialized
            if (_algorithm != AStar)
            {
                _algorithm = AStar;
            }

            runSearchAlgorithms();
        }

        /// <summary>
        /// Set algorithm to UCS and run
        /// </summary>
        private void runAStarPlusCommand()
        {
            // Check if algorithm is initialized
            if (_algorithm != AStarPlus)
            {
                _algorithm = AStarPlus;
            }

            runSearchAlgorithms();
        }


        /// <summary>
        /// Run search algorithms with _algorithm
        /// </summary>
        private async void runSearchAlgorithms()
        {
            try
            {
                await _signal.WaitAsync();
                ResetMaze();

                if (_algorithm == null)
                {
                    _signal.Release();
                    return;
                }

                // Check if maze is loaded
                if (_maze == null)
                {
                    MessageBox.Show("Maze is not loaded", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _signal.Release();
                    return;
                }

                // Input maze to algorithm
                _algorithm.InputMaze(_maze);

                // Solve the maze
                foreach (var step in _algorithm.Solve())
                {
                    if ((step.X == _maze.End.Key && step.Y == _maze.End.Value) || (step.X == _maze.Start.Key && step.Y == _maze.Start.Value))
                    {
                        continue;
                    }
                    ListCells[step.X * _maze.MazeWidth + step.Y].CellType = CellType.Search;
                    await Task.Delay(Speed);
                }

                // Trace back the path
                foreach (var step in _algorithm.TraceBack())
                {
                    if ((step.X == _maze.End.Key && step.Y == _maze.End.Value) || (step.X == _maze.Start.Key && step.Y == _maze.Start.Value))
                    {
                        continue;
                    }
                    ListCells[step.X * _maze.MazeWidth + step.Y].CellType = CellType.Path;
                    await Task.Delay(Speed);
                }

                TotalCost = _algorithm.TotalCost;
                PathCost = _algorithm.PathCost;
                TotalTime = _algorithm.TotalTime;

                _signal.Release();
            }
            catch (Exception)
            {
                _signal.Release();
                MessageBox.Show("Your maze is not solvable", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void selectStartEndPoint(KeyValuePair<int, int> position)
        {
            if (_maze == null) return;
            if (_maze.ListCells[position.Key][position.Value] == '#') return;

            if (IsStartPointSelected)
            {
                ListCells[_maze.Start.Key * _maze.MazeWidth + _maze.Start.Value].CellType = CellType.Empty;
                ListCells[position.Key * _maze.MazeWidth + position.Value].CellType = CellType.Start;
                _maze.ListCells[position.Key] = _maze.ListCells[position.Key].Remove(position.Value, 1).Insert(position.Value, "S");
                _maze.ListCells[_maze.Start.Key] = _maze.ListCells[_maze.Start.Key].Remove(_maze.Start.Value, 1).Insert(_maze.Start.Value, ".");

                _maze.Start = position;
                
            }
            else
            {
                ListCells[_maze.End.Key * _maze.MazeWidth + _maze.End.Value].CellType = CellType.Empty;
                ListCells[position.Key * _maze.MazeWidth + position.Value].CellType = CellType.End;
                _maze.ListCells[position.Key] = _maze.ListCells[position.Key].Remove(position.Value, 1).Insert(position.Value, "E");
                _maze.ListCells[_maze.End.Key] = _maze.ListCells[_maze.End.Key].Remove(_maze.End.Value, 1).Insert(_maze.End.Value, ".");
                _maze.End = position;
            }

            ResetMaze();
        }

        /// <summary>
        /// Reset maze
        /// </summary>
        private void ResetMaze()
        {
            if (_maze == null) return;

            for (int i = 0; i < _maze.MazeHeight; i++)
            {
                for (int j = 0; j < _maze.MazeWidth; j++)
                {
                    ListCells[i * _maze.MazeWidth + j].CellType = Models.Convert.CharToCellType(_maze.ListCells[i][j]);
                }
            }
        }


        #endregion
    }
}
