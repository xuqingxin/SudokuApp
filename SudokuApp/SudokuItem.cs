using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuItem
    {
        public SudokuItem(Sudoku sudoku, int i, int j, int v) : this(sudoku, 3, i, j, v)
        {
        }

        public SudokuItem(Sudoku sudoku, int n, int i, int j, int v)
        {
            this.Sudoku = sudoku;
            this.n = n;
            this.i = i;
            this.j = j;
            value = v;
            Candidates = new bool[n * n + 1];
        }

        public Sudoku Sudoku { get; }

        public int n { get; }

        public int i { get; }

        public int j { get; }

        public int value { get; private set; }

        public bool[] Candidates { get; }

        public void SetValue(int v)
        {
            value = v;
            ClearCandidates();
            ClearRowColumnBlockCandidates(value);
        }

        public void SetCandidate(int c)
        {
            Candidates[c] = true;
        }

        public void ClearCandidate(int c)
        {
            Candidates[c] = false;
        }

        public bool ClearCandidates(IEnumerable<int> lst)
        {
            bool rtVal = false;
            foreach (int c in lst)
            {
                if (Candidates[c])
                {
                    Candidates[c] = false;
                    rtVal = true;
                }
            }
            return rtVal;
        }

        public void ClearCandidates()
        {
            for (int i = 1; i < Candidates.Length; i++)
            {
                Candidates[i] = false;
            }
        }

        public void ClearRowColumnBlockCandidates(int v)
        {
            for (int k = 0; k < n * n; k++)
            {
                SudokuItem item = Sudoku.GetItem(k, j);
                item.ClearCandidate(v);
            }
            for (int k = 0; k < n * n; k++)
            {
                SudokuItem item = Sudoku.GetItem(i, k);
                item.ClearCandidate(v);
            }
            SudokuBlock block = Sudoku.GetBlock(i, j);
            block.ClearCandidate(v);
        }

        public void ResetCandidates()
        {
            for (int i = 1; i < Candidates.Length; i++)
            {
                Candidates[i] = true;
            }
        }

        public int[] GetCandidateNumbers()
        {
            List<int> lst = new List<int>();
            for (int i = 1; i < Candidates.Length; i++)
            {
                if (Candidates[i])
                {
                    lst.Add(i);
                }
            }
            return lst.ToArray();
        }

        public void Reset()
        {
            value = 0;
            ResetCandidates();
        }

        public bool HasSameCandidates(SudokuItem item)
        {
            bool rtVal = false;
            if (item != null)
            {
                List<int> lst = new List<int>();
                for (int k = 1; k < Candidates.Length; k++)
                {
                    if (Candidates[k])
                    {
                        lst.Add(k);
                    }
                }
                for (int k = 1; k < item.Candidates.Length; k++)
                {
                    if (item.Candidates[k])
                    {
                        if (lst.Contains(k))
                        {
                            lst.Remove(k);
                        }
                        else
                        {
                            return rtVal;
                        }
                    }
                }
                rtVal = lst.Count == 0;
            }
            return rtVal;
        }

        public bool HasSingleCandidate(out int c)
        {
            bool rtVal = false;
            c = 0;
            List<int> lst = new List<int>();
            for (int k = 1; k < Candidates.Length; k++)
            {
                if (Candidates[k])
                {
                    lst.Add(k);
                }
            }
            if (lst.Count == 1)
            {
                c = lst[0];
                rtVal = true;
            }
            return rtVal;
        }

        public override string ToString()
        {
            return $"({i},{j})={value}";
        }
    }
}