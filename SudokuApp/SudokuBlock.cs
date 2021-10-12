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
            bool rtVal = TryHiddenSingles();
            TryPointing();
            TryBoxLineReduction();
            return rtVal;
        }

        public bool TryPointing()
        {
            bool rtVal = false;

            // Row
            for (int k = 0; k < n; k++)
            {
                List<SudokuItem> blockRow = new List<SudokuItem>();
                HashSet<int> candidatesRow = new HashSet<int>();
                for (int l = 0; l < n; l++)
                {
                    SudokuItem item = Items[k * n + l];
                    blockRow.Add(item);
                    int[] numbers = item.GetCandidateNumbers();
                    foreach (int n in numbers)
                    {
                        candidatesRow.Add(n);
                    }
                }

                List<SudokuItem> blockRestItems = Items.Except(blockRow).ToList();
                HashSet<int> candidatesRestItems = new HashSet<int>();
                foreach (SudokuItem item in blockRestItems)
                {
                    int[] numbers = item.GetCandidateNumbers();
                    foreach (int n in numbers)
                    {
                        candidatesRestItems.Add(n);
                    }
                }

                IEnumerable<int> result = candidatesRow.Except(candidatesRestItems);
                if (result.Any())
                {
                    SudokuRow row = Sudoku.Rows[blockRow[0].j];
                    row.ClearCandidates(result, blockRow);
                }
            }

            // Column
            for (int k = 0; k < n; k++)
            {
                List<SudokuItem> blockCol = new List<SudokuItem>();
                HashSet<int> candidatesCol = new HashSet<int>();
                for (int l = 0; l < n; l++)
                {
                    SudokuItem item = Items[k + l * n];
                    blockCol.Add(item);
                    int[] numbers = item.GetCandidateNumbers();
                    foreach (int n in numbers)
                    {
                        candidatesCol.Add(n);
                    }
                }

                List<SudokuItem> blockRestItems = Items.Except(blockCol).ToList();
                HashSet<int> candidatesRestItems = new HashSet<int>();
                foreach (SudokuItem item in blockRestItems)
                {
                    int[] numbers = item.GetCandidateNumbers();
                    foreach (int n in numbers)
                    {
                        candidatesRestItems.Add(n);
                    }
                }

                IEnumerable<int> result = candidatesCol.Except(candidatesRestItems);
                if (result.Any())
                {
                    SudokuColumn col = Sudoku.Columns[blockCol[0].i];
                    col.ClearCandidates(result, blockCol);
                }
            }

            return rtVal;
        }

        public bool TryBoxLineReduction()
        {
            bool rtVal = false;

            // Row
            for (int k = 0; k < n; k++)
            {
                List<SudokuItem> blockRow = new List<SudokuItem>();
                HashSet<int> candidatesRow = new HashSet<int>();
                for (int l = 0; l < n; l++)
                {
                    SudokuItem item = Items[k * n + l];
                    blockRow.Add(item);
                    int[] numbers = item.GetCandidateNumbers();
                    foreach (int n in numbers)
                    {
                        candidatesRow.Add(n);
                    }
                }

                List<SudokuItem> rowRestItems = Sudoku.Rows[blockRow[0].j].Items.Except(blockRow).ToList();
                HashSet<int> candidatesRestItems = new HashSet<int>();
                foreach (SudokuItem item in rowRestItems)
                {
                    int[] numbers = item.GetCandidateNumbers();
                    foreach (int n in numbers)
                    {
                        candidatesRestItems.Add(n);
                    }
                }

                IEnumerable<int> result = candidatesRow.Except(candidatesRestItems);
                if (result.Any())
                {
                    this.ClearCandidates(result, blockRow);
                }
            }

            // Column
            for (int k = 0; k < n; k++)
            {
                List<SudokuItem> blockCol = new List<SudokuItem>();
                HashSet<int> candidatesCol = new HashSet<int>();
                for (int l = 0; l < n; l++)
                {
                    SudokuItem item = Items[k + l * n];
                    blockCol.Add(item);
                    int[] numbers = item.GetCandidateNumbers();
                    foreach (int n in numbers)
                    {
                        candidatesCol.Add(n);
                    }
                }

                List<SudokuItem> colRestItems = Sudoku.Columns[blockCol[0].i].Items.Except(blockCol).ToList();
                HashSet<int> candidatesRestItems = new HashSet<int>();
                foreach (SudokuItem item in colRestItems)
                {
                    int[] numbers = item.GetCandidateNumbers();
                    foreach (int n in numbers)
                    {
                        candidatesRestItems.Add(n);
                    }
                }

                IEnumerable<int> result = candidatesCol.Except(candidatesRestItems);
                if (result.Any())
                {
                    this.ClearCandidates(result, blockCol);
                }
            }

            return rtVal;
        }

        public override string ToString()
        {
            return $"({i * n},{j * n})->({(i + 1) * n - 1},{(j + 1) * n - 1})";
        }
    }
}