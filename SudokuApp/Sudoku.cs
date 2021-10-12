using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class Sudoku
    {
        public Sudoku() : this(3)
        {
        }

        public Sudoku(int n)
        {
            this.n = n;
            this.n2 = n * n;

            Items = new SudokuItem[n2][];
            for (int j = 0; j < Items.Length; j++)
            {
                Items[j] = new SudokuItem[n2];
                for (int i = 0; i < Items[j].Length; i++)
                {
                    Items[j][i] = new SudokuItem(this, n, i, j, 0);
                }
            }

            Blocks = new SudokuBlock[n][];
            for (int j = 0; j < Blocks.Length; j++)
            {
                Blocks[j] = new SudokuBlock[n];
                for (int i = 0; i < Blocks[j].Length; i++)
                {
                    Blocks[j][i] = new SudokuBlock(this, n, i, j);
                }
            }
            for (int bj = 0; bj < Blocks.Length; bj++)
            {
                for (int bi = 0; bi < Blocks[bj].Length; bi++)
                {
                    SudokuBlock block = Blocks[bi][bj];
                    for (int k = 0; k < block.Items.Length; k++)
                    {
                        int i = k % n;
                        int j = k / n;
                        block.Items[k] = Items[bi * n + i][bj * n + j];
                    }
                }
            }

            Rows = new SudokuRow[n2];
            for (int j = 0; j < n2; j++)
            {
                Rows[j] = new SudokuRow(this, n, j);
                for (int i = 0; i < Rows[j].Items.Length; i++)
                {
                    Rows[j].Items[i] = Items[j][i];
                }
            }

            Columns = new SudokuColumn[n2];
            for (int i = 0; i < n2; i++)
            {
                Columns[i] = new SudokuColumn(this, n, i);
                for (int j = 0; j < Columns[i].Items.Length; j++)
                {
                    Columns[i].Items[j] = Items[j][i];
                }
            }
        }

        public int n { get; }

        public int n2 { get; }

        public SudokuItem[][] Items { get; }

        public SudokuBlock[][] Blocks { get; }

        public SudokuRow[] Rows { get; }

        public SudokuColumn[] Columns { get; }

        public SudokuItem GetItem(int i, int j)
        {
            if (i < 0 || i >= n * n || j < 0 || j >= n * n)
            {
                return null;
            }

            return Items[j][i];
        }

        public SudokuBlock GetBlock(int i, int j)
        {
            if (i < 0 || i >= n * n || j < 0 || j >= n * n)
            {
                return null;
            }

            return Blocks[j / n][i / n];
        }

        public bool GetNextNumber()
        {
            return TryRows() || TryColumns() || TryBlocks();
        }

        public bool GetNextNumber2()
        {
            while (GetNextNumber())
            {
                // Do nothing
            }
            return false;
        }

        public bool TryRows()
        {
            bool rtVal = false;
            for (int i = 0; i < n2; i++)
            {
                rtVal |= Rows[i].TryComplete();
                if (rtVal)
                {
                    return rtVal;
                }
            }
            return rtVal;
        }

        public bool TryColumns()
        {
            bool rtVal = false;
            for (int i = 0; i < n2; i++)
            {
                rtVal |= Columns[i].TryComplete();
                if (rtVal)
                {
                    return rtVal;
                }
            }
            return rtVal;
        }

        public bool TryBlocks()
        {
            bool rtVal = false;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    rtVal |= Blocks[i][j].TryComplete();
                    if (rtVal)
                    {
                        return rtVal;
                    }
                }
            }
            return rtVal;
        }

        public bool IsComplete()
        {
            bool rtVal = false;
            rtVal = Rows.All(x => x.IsComplete());
            rtVal &= Columns.All(x => x.IsComplete());
            return rtVal;
        }

        public string Print2String()
        {
            StringBuilder builder = new StringBuilder();
            for (int j = 0; j < n2; j++)
            {
                for (int i = 0; i < n2; i++)
                {
                    string strNumber = Items[j][i].value == 0 ? " " : Items[j][i].value.ToString();
                    builder.Append($"{strNumber:D1} ");
                    if (i > 0 && (i + 1) % n == 0 && i < n2 - 1)
                    {
                        builder.Append("| ");
                    }
                }
                builder.AppendLine();
                if (j > 0 && (j + 1) % n == 0 && j < n2 - 1)
                {
                    builder.AppendLine("---------------------");
                }
            }
            builder.AppendLine("");
            return builder.ToString();
        }

        public string Print2StringBy0()
        {
            StringBuilder builder = new StringBuilder();
            for (int j = 0; j < n2; j++)
            {
                for (int i = 0; i < n2; i++)
                {
                    builder.Append($"{Items[j][i].value} ");
                }
                builder.AppendLine();
            }
            builder.AppendLine("");
            return builder.ToString();
        }

        public void Print()
        {
            Console.WriteLine(Print2String());
        }

        public void Reset()
        {
            for (int j = 0; j < n2; j++)
            {
                for (int i = 0; i < n2; i++)
                {
                    Items[j][i].Reset();
                }
            }
        }

        public bool Load(string filename)
        {
            Reset();
            try
            {
                string[] lines = File.ReadAllLines(filename);
                if (lines.Length < n2)
                {
                    return false;
                }
                for (int j = 0; j < n2; j++)
                {
                    string[] parts = lines[j].Split(' ');
                    if (parts.Length < n2)
                    {
                        return false;
                    }
                    for (int i = 0; i < n2; i++)
                    {
                        if (int.TryParse(parts[i], out int v) && v > 0)
                        {
                            Items[j][i].SetValue(v);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Save(string filename)
        {
            File.WriteAllText(filename, Print2String());
        }

        public void SaveBy0(string filename)
        {
            File.WriteAllText(filename, Print2StringBy0());
        }
    }
}