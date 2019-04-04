using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms;
using GameOfLife;
using Newtonsoft.Json;

namespace GameOfLifeProject
{
    public partial class MainWindow : Window, IRenderer
    {
        private GameOfLife _gameOfLife;
        private DispatcherTimer _timer;

        private readonly Color _aliveColor = Colors.White;
        private readonly Color _deadColor = Colors.Black;

        private bool _editing;

        public MainWindow()
        {
            InitializeComponent();

            _editing = false;

            _gameOfLife = new GameOfLife(32, 18, this);

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Render, Tick, Dispatcher.CurrentDispatcher);
            _timer.Stop();
        }

        private void Tick(object sender, EventArgs e)
        {
            _gameOfLife.DoStep();
        }

        public void Setup(in Cell[,] cells)
        {
            Grid.ColumnDefinitions.Clear();
            Grid.RowDefinitions.Clear();

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                Grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < cells.GetLength(1); i++)
            {
                Grid.RowDefinitions.Add(new RowDefinition());
            }

            foreach (var cell in cells)
            {
                Rectangle rectangle = new Rectangle
                {
                    Width = Grid.Width / cells.GetLength(0),
                    Height = Grid.Height / cells.GetLength(1),
                    Fill = new SolidColorBrush(cell.State == CellState.Alive ? _aliveColor : _deadColor),
                    Tag = cell.Position
                };

                rectangle.MouseUp += EditCell;

                Grid.SetColumn(rectangle, cell.Position.X);
                Grid.SetRow(rectangle, cell.Position.Y);

                Grid.Children.Add(rectangle);
            }
        }

        private void EditCell(object sender, MouseButtonEventArgs e)
        {
            if (_editing && sender is Rectangle rectangle && rectangle.Tag is Position position)
            {
                _gameOfLife.EditCell(position);
            }
        }

        public void RenderCells(in Cell[,] cells)
        {
            foreach (var child in Grid.Children)
            {
                if (!(child is Rectangle rectangle)) continue;
                if (!(rectangle.Tag is Position position)) continue;

                Cell cell = cells[position.X, position.Y];
                rectangle.Fill = new SolidColorBrush(cell.State == CellState.Alive ? _aliveColor : _deadColor);
            }
        }

        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                if (_timer.IsEnabled)
                {
                    _timer.Stop();
                    button.Content = "Start";
                }
                else
                {
                    _timer.Start();
                    button.Content = "Pause";

                    _editing = false;
                    EditDoneButton.Content = "Edit";
                }
            }
        }

        private void EditDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                _editing = !_editing;
                button.Content = _editing ? "Done!" : "Edit";

                if (_editing)
                {
                    _timer.Stop();
                    StartPauseButton.Content = "Start";
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                StartPauseButton.Content = "Start";
            }

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Title = "Save your game";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.RestoreDirectory = true;

            dialog.DefaultExt = "json";
            dialog.Filter = "Game of Life (*.json)|*.json";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string json = JsonConvert.SerializeObject(_gameOfLife.GenerateSavedData());
                File.WriteAllText(dialog.FileName, json);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                StartPauseButton.Content = "Start"; 
            }

            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Load your game";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.RestoreDirectory = true;

            dialog.DefaultExt = "json";
            dialog.Filter = "Game of Life (*.json)|*.json";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string json = File.ReadAllText(dialog.FileName);
                _gameOfLife.LoadSavedData(JsonConvert.DeserializeObject<SavedData>(json));
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                StartPauseButton.Content = "Start";
            }

            _gameOfLife.Reset();
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                StartPauseButton.Content = "Start";
            }

            _gameOfLife.GenerateRandom();
        }
    }
}
