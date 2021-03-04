using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuRow
    {
        public SudokuRow(int i, Sudoku sudoku) : this(3 * 3, i, sudoku)
        {
        }

        public SudokuRow(int n, int i, Sudoku sudoku)
        {
            this.n = n;
            this.i = i;
            this.Sudoku = sudoku;
            Sum = Sudoku.GetSum(n);
            Items = new SudokuItem[n];
        }

        public int n { get; }

        public int i { get; }

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
                    SudokuColumn column = Sudoku.Columns[idx];
                    if (column.GetItemByValue(number) != null)
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
                    SudokuColumn column = Sudoku.Columns[idx];
                    if (column.GetItemByValue(number) != null)
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

        public SudokuItem GetItemByValue(int v)
        {
            return Items.FirstOrDefault(x => x.value == v);
        }

        public override string ToString()
        {
            return $"(i={i})";
        }
    }
}