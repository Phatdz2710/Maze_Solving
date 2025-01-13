using CommunityToolkit.Mvvm.Input;
using Maze_Solving.ViewModels.ControlViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Maze_Solving.ViewModels
{
    /// <summary>
    /// Main view model for the application
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region Fields

        #endregion

        #region Properties
        /// <summary>
        /// Maze view model
        /// </summary>
        public MazeViewModel MazeViewModel { get; set; }


        #endregion

        #region Commands

        /// <summary>
        /// Select file command
        /// </summary>
        public ICommand SelectFileCommand { get; }

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel()
        {
            MazeViewModel = new MazeViewModel();

            SelectFileCommand = new RelayCommand(selectFileCommand);
        }

        /// <summary>
        /// Open file dialog to select a file to load maze
        /// </summary>
        private void selectFileCommand()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                MazeViewModel.InitializeMaze(fileName);
            }
        }
    }
}
