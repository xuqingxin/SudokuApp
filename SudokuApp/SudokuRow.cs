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

        public SudokuRow(int n2, int i, Sudoku sudoku)
        {
            this.n2 = n2;
            this.i = i;
            this.Sudoku = sudoku;
            Sum = Sudoku.GetSum(n2);
            Items = new SudokuItem[n2];
        }

        public int n2 { get; }

        public int i { get; }

        public int Sum { get; }

        public SudokuItem[] Items { get; }

        public Sudoku Sudoku { get; }

        public bool IsComplete()
        {
            bool rtVal = false;
            int[] numbers = new int[n2];
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
            int[] numbers = new int[n2];
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
            for (int k = 0; k < n2; k++)
            {
                SudokuItem item = Items[k];
                if (item.value == 0)
                {
                    candiateIndex.Add(k);
                }
            }
            rtVal = TryComplete(candiateNumbers, candiateIndex) || TryHiddenSingles() || TryNakedPairs() || TryNakedTriples();
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

        public bool TryHiddenSingles()
        {
            bool rtVal = false;
            Dictionary<int, int> candidates = new Dictionary<int, int>();
            for (int k = 1; k <= n2; k++)
            {
                candidates[k] = 0;
            }
            foreach (SudokuItem item in Items)
            {
                if (item.value == 0)
                {
                    if (item.HasSingleCandidate(out int c))
                    {
                        item.SetValue(c);
                        rtVal = true;
                        return rtVal;
                    }

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

        public bool TryNakedPairs()
        {
            bool rtVal = false;
            for (int k = 0; k < n2 - 1; k++)
            {
                SudokuItem item = Items[k];
                if (item.value > 0)
                {
                    continue;
                }
                List<int> lst = new List<int>();
                for (int idx = 1; idx < item.Candidates.Length; idx++)
                {
                    if (item.Candidates[idx])
                    {
                        lst.Add(idx);
                    }
                }

                if (lst.Count == 2)
                {
                    for (int l = k + 1; l < n2; l++)
                    {
                        SudokuItem item2 = Items[l];
                        if (item.value == 0 && item.HasSameCandidates(item2))
                        {
                            // 清楚行中的候选项
                            for (int m = 0; m < n2; m++)
                            {
                                if (m == k || m == l)
                                {
                                    continue;
                                }
                                SudokuItem item3 = Items[m];
                                if (item3.value == 0)
                                {
                                    rtVal |= item3.ClearCandidates(lst);
                                }
                            }

                            // 清楚区块中的候选项
                            SudokuBlock block = Sudoku.GetBlock(item.i, item.j);
                            SudokuBlock block2 = Sudoku.GetBlock(item2.i, item2.j);
                            if (block == block2)
                            {
                                rtVal |= block.ClearCandidates(lst, new SudokuItem[] { item, item2 });
                            }

                            if (rtVal)
                            {
                                return rtVal;
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
            List<int> lstColIndex = new List<int>();
            for (int k = 0; k < n2; k++)
            {
                SudokuItem item = Items[k];
                if (item.value > 0)
                {
                    continue;
                }
                int count = item.GetCandidateNumbers().Length;
                if (count == 2 || count == 3)
                {
                    lstColIndex.Add(k);
                }
            }

            for (int l = 0; l < lstColIndex.Count - 2; l++)
            {
                SudokuItem item1 = Items[lstColIndex[l]];
                int[] c1 = item1.GetCandidateNumbers();
                HashSet<int> s1 = new HashSet<int>(c1);

                for (int m = l + 1; m < lstColIndex.Count - 1; m++)
                {
                    SudokuItem item2 = Items[lstColIndex[m]];
                    int[] c2 = item2.GetCandidateNumbers();
                    HashSet<int> s2 = new HashSet<int>(s1);
                    foreach (int c in c2)
                    {
                        s2.Add(c);
                    }
                    if (s2.Count > 3)
                    {
                        continue;
                    }

                    for (int n = m + 1; n < lstColIndex.Count; n++)
                    {
                        SudokuItem item3 = Items[lstColIndex[n]];
                        int[] c3 = item3.GetCandidateNumbers();
                        HashSet<int> s3 = new HashSet<int>(s2);
                        foreach (int c in c3)
                        {
                            s3.Add(c);
                        }
                        if (s3.Count > 3)
                        {
                            continue;
                        }

                        // 清楚行中的候选项
                        for (int k = 0; k < n2; k++)
                        {
                            if (k == lstColIndex[l] || k == lstColIndex[m] || k == lstColIndex[n])
                            {
                                continue;
                            }
                            SudokuItem item = Items[k];
                            if (item.value == 0)
                            {
                                rtVal |= item.ClearCandidates(s3);
                            }
                        }

                        // 清楚区块中的候选项
                        SudokuBlock block = Sudoku.GetBlock(item1.i, item1.j);
                        SudokuBlock block2 = Sudoku.GetBlock(item2.i, item2.j);
                        SudokuBlock block3 = Sudoku.GetBlock(item3.i, item3.j);
                        if (block == block2 && block2 == block3)
                        {
                            rtVal |= block.ClearCandidates(s3, new SudokuItem[] { item1, item2, item3 });
                        }

                        if (rtVal)
                        {
                            return rtVal;
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
            return $"(i={i})";
        }
    }
}