using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfLife;

namespace GameOfLifeProject
{
    class GameOfLife
    {
        private int _width;
        private int _height;
        private IRenderer _renderer;
        private Random _random;

        private Cell[,] _cells;

        public GameOfLife(int width, int height, IRenderer renderer)
        {
            _width = width;
            _height = height;
            _renderer = renderer;
            _random = new Random();

            GenerateCells();
            _renderer.Setup(_cells);
        }

        private void GenerateCells()
        {
            _cells = new Cell[_width, _height];

            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    _cells[i, j] = new Cell(new Position(i, j));
                }
            }
        }

        public void GenerateRandom()
        {
            foreach (var cell in _cells)
            {
                cell.State = _random.Next(0, 2) == 0 ? CellState.Dead : CellState.Alive;
            }

            Render();
        }

        public void Reset()
        {
            GenerateCells();
            Render();
        }

        public void EditCell(Position position)
        {
            var cell = _cells[position.X, position.Y];
            cell.State = cell.State == CellState.Alive ? CellState.Dead : CellState.Alive;

            Render();
        }

        public void DoStep()
        {
            DoMagic();
            Render();
        }

        private void DoMagic()
        {
            List<Cell> changedCells = new List<Cell>();

            foreach (var cell in _cells)
            {
                var neighbors = CountNeighbors(cell.Position);
                var cellState = cell.State;
                
                if (neighbors < 2)
                {
                    if (cellState == CellState.Alive)
                    {
                        var changedCell = new Cell(cell.Position) { State = CellState.Dead };
                        changedCells.Add(changedCell);
                    }
                }
                else if (neighbors == 2 || neighbors == 3)
                {
                    if (cellState == CellState.Dead && neighbors == 3)
                    {
                        var changedCell = new Cell(cell.Position) { State = CellState.Alive };
                        changedCells.Add(changedCell);
                    }
                }
                else if (neighbors > 3)
                {
                    if (cellState == CellState.Alive)
                    {
                        var changedCell = new Cell(cell.Position) { State = CellState.Dead };
                        changedCells.Add(changedCell);
                    }
                }
            }

            UpdateChangedCells(changedCells);
        }

        private int CountNeighbors(Position position)
        {
            int count = 0;

            if (position.Y - 1 >= 0)
            {
                if (position.X - 1 >= 0)
                {
                    if (_cells[position.X - 1, position.Y - 1].State == CellState.Alive)
                    {
                        count++;
                    }
                }

                if (_cells[position.X, position.Y - 1].State == CellState.Alive)
                {
                    count++;
                }

                if (position.X + 1 < _width)
                {
                    if (_cells[position.X + 1, position.Y - 1].State == CellState.Alive)
                    {
                        count++;
                    }
                }
            }

            if (position.X + 1 < _width)
            {
                if (_cells[position.X + 1, position.Y].State == CellState.Alive)
                {
                    count++;
                }
            }

            if (position.X - 1 >= 0)
            {
                if (_cells[position.X - 1, position.Y].State == CellState.Alive)
                {
                    count++;
                }
            }

            if (position.Y + 1 < _height)
            {
                if (position.X - 1 >= 0)
                {
                    if (_cells[position.X - 1, position.Y + 1].State == CellState.Alive)
                    {
                        count++;
                    }
                }

                if (_cells[position.X, position.Y + 1].State == CellState.Alive)
                {
                    count++;
                }

                if (position.X + 1 < _width)
                {
                    if (_cells[position.X + 1, position.Y + 1].State == CellState.Alive)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void UpdateChangedCells(List<Cell> changedCells)
        {
            foreach (var cell in changedCells)
            {
                _cells[cell.Position.X, cell.Position.Y].State = cell.State;
            }
        }

        private void Render()
        {
            _renderer.RenderCells(_cells);
        }

        public SavedData GenerateSavedData()
        {
            return new SavedData { Cells = (Cell[,]) _cells.Clone() };
        }

        public void LoadSavedData(SavedData data)
        {
            this._cells = data.Cells;
            Render();
        }
    }
}
