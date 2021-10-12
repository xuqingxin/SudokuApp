using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuBlock : SudokuCollection
    {
        public SudokuBlock(Sudoku sudoku, int i, int j) : this(sudoku, 3, i, j)
        {
        }

        public SudokuBlock(Sudoku sudoku, int n, int i, int j) : base(sudoku, n, j * n + i)
        {
            this.i = i;
            this.j = j;
        }

        public int i { get; }

        public int j { get; }

        public bool TryComplete()
        {
            return TryHiddenSingles();
        }

        public override string ToString()
        {
            return $"({i * n},{j * n})->({(i + 1) * n - 1},{(j + 1) * n - 1})";
        }
    }
}