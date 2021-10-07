using System;
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