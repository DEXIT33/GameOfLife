using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    interface IRenderer
    {
        void Setup(in Cell[,] cells);
        void RenderCells(in Cell[,] cells);
    }
}
