using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuBlock
    {
        public SudokuBlock(Sudoku sudoku, int i, int j) : this(sudoku, 3, i, j)
        {
        }

        public SudokuBlock(Sudoku sudoku, int n, int i, int j)
        {
            this.Sudoku = sudoku;
            this.n = n;
            this.i = i;
            this.j = j;
            Items = new SudokuItem[n][];
            for (int k = 0; k < Items.Length; k++)
            {
                Items[k] = new SudokuItem[n];
            }
        }

        public int n { get; }

        public int i { get; }

        public int j { get; }

        public SudokuItem[][] Items { get; }

        public Sudoku Sudoku { get; }

        public void ClearCandidate(int c)
        {
            for (int k = 0; k < n; k++)
            {
                for (int l = 0; l < n; l++)
                {
                    SudokuItem item = Items[k][l];
                    item.ClearCandidate(c);
                }
            }
        }

        public bool ClearCandidates(IEnumerable<int> lst, IEnumerable<SudokuItem> excludeList)
        {
            bool rtVal = false;
            for (int k = 0; k < n; k++)
            {
                for (int l = 0; l < n; l++)
                {
                    SudokuItem item = Items[k][l];
                    if (item.value > 0)
                    {
                        continue;
                    }
                    if (excludeList == null)
                    {
                        rtVal |= item.ClearCandidates(lst);
                    }
                    else if (!excludeList.Contains(item))
                    {
                        rtVal |= item.ClearCandidates(lst);
                    }
                }
            }
            return rtVal;
        }

        public bool IsComplete()
        {
            bool rtVal = false;
            int[] numbers = new int[n * n];
            for (int k = 0; k < n; k++)
            {
                for (int l = 0; l < n; l++)
                {
                    SudokuItem item = Items[k][l];
                    if (item.value > 0)
                    {
                        numbers[item.value - 1] = 1;
                    }
                }
            }
            rtVal = numbers.Sum() == numbers.Length;
            return rtVal;
        }

        public bool TryComplete()
        {
            bool rtVal = false;
            int[] numbers = new int[n * n];
            for (int k = 0; k < n; k++)
            {
                for (int l = 0; l < n; l++)
                {
                    SudokuItem item = Items[k][l];
                    if (item.value > 0)
                    {
                        numbers[item.value - 1] = 1;
                    }
                }
            }
            List<int> candiateNumbers = new List<int>();
            for (int k = 0; k < numbers.Length; k++)
            {
                if (numbers[k] == 0)
                {
                    candiateNumbers.Add(k + 1);
                }
            }
            List<Tuple<int, int>> candiateIndex = new List<Tuple<int, int>>();
            for (int k = 0; k < n; k++)
            {
                for (int l = 0; l < n; l++)
                {
                    SudokuItem item = Items[k][l];
                    if (item.value == 0)
                    {
                        candiateIndex.Add(Tuple.Create(l, k));
                    }
                }
            }
            rtVal = TryComplete(candiateNumbers, candiateIndex);
            if (!rtVal)
            {
                rtVal = TryHiddenSingles();
            }
            if (!rtVal)
            {
                rtVal |= TryNakedTriples();
            }
            return rtVal;
        }

        public bool TryComplete(List<int> candiateNumbers, List<Tuple<int, int>> candidateIndex)
        {
            bool rtVal = false;
            foreach (int number in candiateNumbers)
            {
                List<Tuple<int, int>> excludeList = new List<Tuple<int, int>>();
                foreach (var idx in candidateIndex)
                {
                    SudokuRow row = Sudoku.Rows[j * n + idx.Item2];
                    if (row.GetItemByValue(number) != null)
                    {
                        excludeList.Add(idx);
                    }
                    SudokuColumn column = Sudoku.Columns[i * n + idx.Item1];
                    if (column.GetItemByValue(number) != null && !excludeList.Contains(idx))
                    {
                        excludeList.Add(idx);
                    }
                }
                var result = candidateIndex.Except(excludeList);
                if (result.Count() == 1)
                {
                    Tuple<int, int> tuple = result.First();
                    Items[tuple.Item2][tuple.Item1].SetValue(number);
                    rtVal = true;
                    return rtVal;
                }
            }

            foreach (var idx in candidateIndex)
            {
                List<int> excludeList = new List<int>();
                foreach (int number in candiateNumbers)
                {
                    SudokuRow row = Sudoku.Rows[j * n + idx.Item2];
                    if (row.GetItemByValue(number) != null)
                    {
                        excludeList.Add(number);
                    }
                    SudokuColumn column = Sudoku.Columns[i * n + idx.Item1];
                    if (column.GetItemByValue(number) != null && !excludeList.Contains(number))
                    {
                        excludeList.Add(number);
                    }
                }
                var result = candiateNumbers.Except(excludeList);
                if (result.Count() == 1)
                {
                    Items[idx.Item2][idx.Item1].SetValue(result.First());
                    rtVal = true;
                    return rtVal;
                }
            }
            return rtVal;
        }

        public bool TryRowCompleteWithValue(int rowIndex, int v)
        {
            bool rtVal = false;
            List<int> candiateColumns = new List<int>();
            List<int> excludeColumns = new List<int>();
            for (int k = 0; k < n; k++)
            {
                SudokuItem item = Items[rowIndex][k];
                if (item.value == 0)
                {
                    candiateColumns.Add(k);
                }
            }
            foreach (int clmn in candiateColumns)
            {
                SudokuColumn column = Sudoku.Columns[i * n + clmn];
                if (column.GetItemByValue(v) != null)
                {
                    excludeColumns.Add(clmn);
                }
            }
            var result = candiateColumns.Except(excludeColumns);
            if (result.Count() == 1)
            {
                Items[rowIndex][result.First()].SetValue(v);
                rtVal = true;
            }
            return rtVal;
        }

        public bool TryColumnCompleteWithValue(int colIndex, int v)
        {
            bool rtVal = false;
            List<int> candiateRows = new List<int>();
            List<int> excludeRows = new List<int>();
            for (int k = 0; k < n; k++)
            {
                SudokuItem item = Items[k][colIndex];
                if (item.value == 0)
                {
                    candiateRows.Add(k);
                }
            }
            foreach (int rw in candiateRows)
            {
                SudokuRow row = Sudoku.Rows[j * n + rw];
                if (row.GetItemByValue(v) != null)
                {
                    excludeRows.Add(rw);
                }
            }
            var result = candiateRows.Except(excludeRows);
            if (result.Count() == 1)
            {
                Items[result.First()][colIndex].SetValue(v);
                rtVal = true;
            }
            return rtVal;
        }

        public bool TryHiddenSingles()
        {
            bool rtVal = false;
            Dictionary<int, int> candidates = new Dictionary<int, int>();
            for (int k = 1; k <= n * n; k++)
            {
                candidates[k] = 0;
            }
            for (int k = 0; k < n; k++)
            {
                for (int l = 0; l < n; l++)
                {
                    SudokuItem item = Items[k][l];
                    if (item.value == 0)
                    {
                        if (item.HasSingleCandidate(out int c))
                        {
                            item.SetValue(c);
                            rtVal = true;
                            return rtVal;
                        }

                        for (int m = 1; m < item.Candidates.Length; m++)
                        {
                            if (item.Candidates[m])
                            {
                                candidates[m] = candidates[m] + 1;
                            }
                        }
                    }
                }
            }
            foreach (var pair in candidates)
            {
                if (pair.Value == 1)
                {
                    rtVal = true;
                    for (int k = 0; k < n; k++)
                    {
                        for (int l = 0; l < n; l++)
                        {
                            SudokuItem item = Items[k][l];
                            if (item.value == 0 && item.Candidates[pair.Key])
                            {
                                item.SetValue(pair.Key);
                                break;
                            }
                        }
                    }
                }
            }
            return rtVal;
        }

        public bool TryNakedTriples()
        {
            bool rtVal = false;
            //TODO: Implement TryNakedTriples
            return rtVal;
        }

        public override string ToString()
        {
            return $"({i * n},{j * n})->({(i + 1) * n - 1},{(j + 1) * n - 1})";
        }
    }
}