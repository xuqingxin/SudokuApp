using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuItem
    {
        public SudokuItem(int i, int j, int v) : this(3, i, j, v)
        {
        }

        public SudokuItem(int n, int i, int j, int v)
        {
            this.n = n;
            this.i = i;
            this.j = j;
            value = v;
            Candidates = new bool[n * n + 1];
        }

        public int n { get; }

        public int i { get; }

        public int j { get; }

        public int value { get; private set; }

        public bool[] Candidates { get; }

        public void SetValue(int v)
        {
            value = v;
            ClearCandidates();
        }

        public void SetCandidate(int c)
        {
            Candidates[c] = true;
        }

        public void ClearCandidates()
        {
            for (int i = 0; i < Candidates.Length; i++)
            {
                Candidates[i] = false;
            }
        }

        public void ResetCandidates()
        {
            for (int i = 0; i < Candidates.Length; i++)
            {
                Candidates[i] = true;
            }
        }

        public void Reset()
        {
            value = 0;
            ResetCandidates();
        }

        public override string ToString()
        {
            return $"({i},{j})={value}";
        }
    }
}