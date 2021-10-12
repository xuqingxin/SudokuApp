using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuColumn : SudokuCollection
    {
        public SudokuColumn(Sudoku sudoku, int index) : this(sudoku, 3, index)
        {
        }

        public SudokuColumn(Sudoku sudoku, int n, int index) : base(sudoku, n, index)
        {
        }

        public bool TryComplete()
        {
            bool rtVal = TryHiddenSingles();
            TryPairs();
            TryTriples();
            return rtVal;
        }

        public override string ToString()
        {
            return $"(j={Index})";
        }
    }
}