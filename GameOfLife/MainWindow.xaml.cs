using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using GameOfLife;

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

            _gameOfLife = new GameOfLife(32 * 2, 18 * 2, this);
            _gameOfLife.GenerateRandom();

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 10), DispatcherPriority.Render, Tick, Dispatcher.CurrentDispatcher);
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
            if (sender is Button button)
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
            if (sender is Button button)
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
    }
}
