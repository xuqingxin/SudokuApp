using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuCollection
    {
        public SudokuCollection(Sudoku sudoku, int index) : this(sudoku, 3, index)
        {
        }

        public SudokuCollection(Sudoku sudoku, int n, int index)
        {
            this.n = n;
            this.n2 = n * n;
            this.Index = index;
            this.Sudoku = sudoku;
            this.Items = new SudokuItem[n2];
        }

        public int n { get; }

        public int n2 { get; }

        public int Index { get; }

        public SudokuItem[] Items { get; }

        public Sudoku Sudoku { get; }

        public SudokuItem GetItemByValue(int v)
        {
            return Items.FirstOrDefault(x => x.value == v);
        }

        public bool IsComplete()
        {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (SudokuItem item in Items)
            {
                if (item.value > 0)
                {
                    if (map.ContainsKey(item.value))
                    {
                        map[item.value] = map[item.value] + 1;
                    }
                    else
                    {
                        map[item.value] = 1;
                    }
                }
            }
            return map.Count == n2 && map.All(x => x.Value == 1);
        }

        public bool ClearCandidate(int c)
        {
            bool rtVal = false;
            for (int i = 0; i < Items.Length; i++)
            {
                SudokuItem item = Items[i];
                rtVal |= item.ClearCandidate(c);
            }
            return rtVal;
        }

        public bool ClearCandidates(IEnumerable<int> lst, IEnumerable<SudokuItem> excludeList)
        {
            bool rtVal = false;
            for (int i = 0; i < Items.Length; i++)
            {
                SudokuItem item = Items[i];
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
            return rtVal;
        }

        public bool TryHiddenSingles()
        {
            bool rtVal = false;
            Dictionary<int, List<SudokuItem>> candidates = new Dictionary<int, List<SudokuItem>>();
            for (int k = 1; k <= n2; k++)
            {
                candidates[k] = new List<SudokuItem>();
            }
            foreach (SudokuItem item in Items)
            {
                if (item.value > 0)
                {
                    continue;
                }

                if (item.HasSingleCandidate(out int c))
                {
                    item.SetValue(c);
                    rtVal = true;
                    return rtVal;
                }

                for (int k = 1; k < item.Candidates.Length; k++)
                {
                    if (item.Candidates[k] && !candidates[k].Contains(item))
                    {
                        candidates[k].Add(item);
                    }
                }
            }
            foreach (var pair in candidates)
            {
                if (pair.Value.Count == 1)
                {
                    rtVal = true;
                    SudokuItem item = pair.Value[0];
                    item.SetValue(pair.Key);
                }
            }
            return rtVal;
        }

        public bool TryPairs()
        {
            bool rtVal = false;
            Dictionary<int, List<SudokuItem>> candidates = new Dictionary<int, List<SudokuItem>>();
            for (int k = 1; k <= n2; k++)
            {
                candidates[k] = new List<SudokuItem>();
            }
            foreach (SudokuItem item in Items)
            {
                if (item.value > 0)
                {
                    continue;
                }

                for (int k = 1; k < item.Candidates.Length; k++)
                {
                    if (item.Candidates[k] && !candidates[k].Contains(item))
                    {
                        candidates[k].Add(item);
                    }
                }
            }

            int[] keys = candidates.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                int count = candidates[keys[i]].Count;
                if (count != 2)
                {
                    candidates.Remove(keys[i]);
                }
            }

            List<int> lstKeys = candidates.Keys.ToList();
            for (int k = 0; k < lstKeys.Count - 1; k++)
            {
                int c1 = lstKeys[k];
                List<SudokuItem> l1 = candidates[c1];

                for (int l = k + 1; l < lstKeys.Count; l++)
                {
                    int c2 = lstKeys[l];
                    List<SudokuItem> l2 = candidates[c2];

                    SudokuItem[] result = l1.Union(l2).ToArray();
                    if (result.Count() != 2)
                    {
                        continue;
                    }

                    int[] finalkeys = new int[] { c1, c2 };

                    // 如果是HiddenPair，先清除自己单元的其他候选项
                    foreach (var item in result)
                    {
                        if (item.GetCandidateNumbers().Length > 2)
                        {
                            rtVal |= item.ClearCandidatesExcept(finalkeys);
                        }
                    }

                    // 清除其他单元的候选项
                    rtVal |= ClearCandidates(finalkeys, result);

                    // 清除区块中其他单元的候选项
                    SudokuBlock block = Sudoku.GetBlock(result[0].i, result[0].j);
                    SudokuBlock block2 = Sudoku.GetBlock(result[1].i, result[1].j);
                    if (block == block2)
                    {
                        rtVal |= block.ClearCandidates(finalkeys, result);
                    }

                    if (rtVal)
                    {
                        return rtVal;
                    }
                }
            }

            return rtVal;
        }

        public bool TryTriples()
        {
            bool rtVal = false;
            Dictionary<int, List<SudokuItem>> candidates = new Dictionary<int, List<SudokuItem>>();
            for (int k = 1; k <= n2; k++)
            {
                candidates[k] = new List<SudokuItem>();
            }
            foreach (SudokuItem item in Items)
            {
                if (item.value > 0)
                {
                    continue;
                }

                for (int k = 1; k < item.Candidates.Length; k++)
                {
                    if (item.Candidates[k] && !candidates[k].Contains(item))
                    {
                        candidates[k].Add(item);
                    }
                }
            }

            int[] keys = candidates.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                int count = candidates[keys[i]].Count;
                if (count < 2 || count > 3)
                {
                    candidates.Remove(keys[i]);
                }
            }

            List<int> lstKeys = candidates.Keys.ToList();
            for (int k = 0; k < lstKeys.Count - 2; k++)
            {
                int c1 = lstKeys[k];
                List<SudokuItem> l1 = candidates[c1];

                for (int l = k + 1; l < lstKeys.Count - 1; l++)
                {
                    int c2 = lstKeys[l];
                    List<SudokuItem> l2 = candidates[c2];

                    for (int m = l + 1; m < lstKeys.Count; m++)
                    {
                        int c3 = lstKeys[m];
                        List<SudokuItem> l3 = candidates[c3];

                        SudokuItem[] result = l1.Union(l2).Union(l3).ToArray();
                        if (result.Count() != 3)
                        {
                            continue;
                        }

                        int[] finalkeys = new int[] { c1, c2, c3 };

                        // 如果是HiddenTriples，先清除自己单元的其他候选项
                        foreach (var item in result)
                        {
                            if (item.GetCandidateNumbers().Length > 2)
                            {
                                rtVal |= item.ClearCandidatesExcept(finalkeys);
                            }
                        }

                        // 清除其他单元的候选项
                        rtVal |= ClearCandidates(finalkeys, result);

                        // 清除区块中其他单元的候选项
                        SudokuBlock block = Sudoku.GetBlock(result[0].i, result[0].j);
                        SudokuBlock block2 = Sudoku.GetBlock(result[1].i, result[1].j);
                        SudokuBlock block3 = Sudoku.GetBlock(result[2].i, result[2].j);
                        if (block == block2 && block2 == block3)
                        {
                            rtVal |= block.ClearCandidates(finalkeys, result);
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
    }
}