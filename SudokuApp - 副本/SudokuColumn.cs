using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuColumn
    {
        public SudokuColumn(int j, Sudoku sudoku) : this(3 * 3, j, sudoku)
        {
        }

        public SudokuColumn(int n, int j, Sudoku sudoku)
        {
            this.n = n;
            this.j = j;
            this.Sudoku = sudoku;
            this.Sum = Sudoku.GetSum(n);
            Items = new SudokuItem[n];
        }

        public int n { get; }

        public int j { get; }

        public int Sum { get; }

        public SudokuItem[] Items { get; }

        public Sudoku Sudoku { get; }

        public bool IsComplete()
        {
            bool rtVal = false;
            int[] numbers = new int[n];
            foreach (SudokuItem item in Items)
            {
                if (item.value > 0)
                {
                    numbers[item.value - 1] = 1;
                }
            }
            rtVal = numbers.Sum() == numbers.Length;
            return rtVal;
        }

        public bool TryComplete()
        {
            bool rtVal = false;
            int[] numbers = new int[n];
            foreach (SudokuItem item in Items)
            {
                if (item.value > 0)
                {
                    numbers[item.value - 1] = 1;
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
            List<int> candiateIndex = new List<int>();
            for (int k = 0; k < n; k++)
            {
                SudokuItem item = Items[k];
                if (item.value == 0)
                {
                    candiateIndex.Add(k);
                }
            }
            rtVal = TryComplete(candiateNumbers, candiateIndex);
            if (!rtVal)
            {
                //rtVal = TryCompleteByCandidates();
            }
            return rtVal;
        }

        public bool TryComplete(List<int> candiateNumbers, List<int> candidateIndex)
        {
            bool rtVal = false;
            foreach (int number in candiateNumbers)
            {
                List<int> excludeList = new List<int>();
                foreach (int idx in candidateIndex)
                {
                    SudokuRow row = Sudoku.Rows[idx];
                    if (row.GetItemByValue(number) != null)
                    {
                        excludeList.Add(idx);
                    }
                }
                var result = candidateIndex.Except(excludeList);
                if (result.Count() == 1)
                {
                    Items[result.First()].SetValue(number);
                    rtVal = true;
                    return rtVal;
                }
            }

            foreach (int idx in candidateIndex)
            {
                List<int> excludeList = new List<int>();
                foreach (int number in candiateNumbers)
                {
                    SudokuRow row = Sudoku.Rows[idx];
                    if (row.GetItemByValue(number) != null)
                    {
                        excludeList.Add(number);
                    }
                }
                var result = candiateNumbers.Except(excludeList);
                if (result.Count() == 1)
                {
                    Items[idx].SetValue(result.First());
                    rtVal = true;
                    return rtVal;
                }
            }
            return rtVal;
        }

        public bool TryCompleteByCandidates()
        {
            bool rtVal = false;
            Dictionary<int, int> candidates = new Dictionary<int, int>();
            for (int k = 1; k <= n; k++)
            {
                candidates[k] = 0;
            }
            foreach (SudokuItem item in Items)
            {
                if (item.value == 0)
                {
                    for (int k = 1; k < item.Candidates.Length; k++)
                    {
                        if (item.Candidates[k])
                        {
                            candidates[k] = candidates[k] + 1;
                        }
                    }
                }
            }
            foreach (var pair in candidates)
            {
                if (pair.Value == 1)
                {
                    rtVal = true;
                    foreach (SudokuItem item in Items)
                    {
                        if (item.value == 0 && item.Candidates[pair.Key])
                        {
                            item.SetValue(pair.Key);
                            break;
                        }
                    }
                }
            }
            return rtVal;
        }

        public SudokuItem GetItemByValue(int v)
        {
            return Items.FirstOrDefault(x => x.value == v);
        }

        public override string ToString()
        {
            return $"(j={j})";
        }
    }
}