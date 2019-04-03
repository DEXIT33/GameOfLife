using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class Cell
    {
        public Position Position { get; private set; }
        public CellState State { get; set; }

        public Cell(Position position)
        {
            Position = position;
            State = CellState.Dead;
        }
    }

    public enum CellState
    {
        Alive,
        Dead
    }
}
