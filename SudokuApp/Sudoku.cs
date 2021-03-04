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
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = new SudokuItem[n2];
                for (int j = 0; j < Items[i].Length; j++)
                {
                    Items[i][j] = new SudokuItem(n, i, j, 0);
                }
            }

            Blocks = new SudokuBlock[n][];
            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = new SudokuBlock[n];
                for (int j = 0; j < Blocks[i].Length; j++)
                {
                    Blocks[i][j] = new SudokuBlock(i, j, this);
                }
            }
            for (int i = 0; i < Blocks.Length; i++)
            {
                for (int j = 0; j < Blocks[i].Length; j++)
                {
                    SudokuBlock block = Blocks[i][j];
                    for (int k = 0; k < block.Items.Length; k++)
                    {
                        for (int l = 0; l < block.Items[k].Length; l++)
                        {
                            block.Items[k][l] = Items[i * n + k][j * n + l];
                        }
                    }
                }
            }

            Rows = new SudokuRow[n2];
            for (int i = 0; i < n2; i++)
            {
                Rows[i] = new SudokuRow(n2, i, this);
                for (int j = 0; j < Rows[i].Items.Length; j++)
                {
                    Rows[i].Items[j] = Items[i][j];
                }
            }

            Columns = new SudokuColumn[n2];
            for (int j = 0; j < n2; j++)
            {
                Columns[j] = new SudokuColumn(n2, j, this);
                for (int i = 0; i < Columns[j].Items.Length; i++)
                {
                    Columns[j].Items[i] = Items[i][j];
                }
            }
        }

        public int n { get; }

        public int n2 { get; }

        public SudokuItem[][] Items { get; }

        public SudokuBlock[][] Blocks { get; }

        public SudokuRow[] Rows { get; }

        public SudokuColumn[] Columns { get; }

        public bool Init(int i, int j, int v)
        {
            SudokuItem item = GetItem(i, j);
            if (item != null && v >= 1 && v <= n2)
            {
                item.SetValue(v);
                return true;
            }
            return false;
        }

        public SudokuItem GetItem(int i, int j)
        {
            if (i < 0 || i >= n * n || j < 0 || j >= n * n)
            {
                return null;
            }

            return Items[i][j];
        }

        public SudokuBlock GetBlock(int i, int j)
        {
            if (i < 0 || i >= n * n || j < 0 || j >= n * n)
            {
                return null;
            }

            return Blocks[i / n][j / n];
        }

        public static int GetSum(int n)
        {
            return (1 + n) * n / 2;
        }

        public bool GetNextNumber()
        {
            return TryRows() || TryColumns() || TryBlocks() || Try3Rows() || Try3Columns();
        }

        public bool GetNextNumber2()
        {
            while (GetNextNumber())
            {
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

        public bool Try3Rows()
        {
            bool rtVal = false;
            for (int i = 0; i < n; i++)
            {
                rtVal |= Try3Rows(i);
                if (rtVal)
                {
                    return rtVal;
                }
            }
            return rtVal;
        }

        public bool Try3Rows(int i)
        {
            bool rtVal = false;
            SudokuBlock[] blocks = new SudokuBlock[n];
            for (int k = 0; k < n; k++)
            {
                blocks[k] = Blocks[i][k];
            }
            SudokuRow[] rows = new SudokuRow[n];
            for (int k = 0; k < n; k++)
            {
                rows[k] = Rows[i * n + k];
            }
            for (int k = 1; k <= n2; k++)
            {
                rtVal |= Try3RowsWithValue(blocks, rows, k);
                if (rtVal)
                {
                    return rtVal;
                }
            }
            return rtVal;
        }

        public bool Try3RowsWithValue(SudokuBlock[] blocks, SudokuRow[] rows, int v)
        {
            bool rtVal = false;
            SudokuItem[] items = new SudokuItem[n];
            for (int i = 0; i < rows.Length; i++)
            {
                items[i] = rows[i].GetItemByValue(v);
            }
            int count = items.Count(x => x != null);
            if (count == n - 1)
            {
                int[] blockIndex = new int[n];
                int rowIndex = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == null)
                    {
                        rowIndex = i;
                    }
                    else
                    {
                        SudokuBlock tempBlock = GetBlock(items[i].i, items[i].j);
                        blockIndex[tempBlock.j] = 1;
                    }
                }
                SudokuBlock block = null;
                for (int i = 0; i < n; i++)
                {
                    if (blockIndex[i] == 0)
                    {
                        block = blocks[i];
                        break;
                    }
                }
                if (block.TryRowCompleteWithValue(rowIndex, v))
                {
                    rtVal = true;
                }
            }
            return rtVal;
        }

        public bool Try3Columns()
        {
            bool rtVal = false;
            for (int j = 0; j < n; j++)
            {
                rtVal |= Try3Columns(j);
                if (rtVal)
                {
                    return rtVal;
                }
            }
            return rtVal;
        }

        public bool Try3Columns(int j)
        {
            bool rtVal = false;
            SudokuBlock[] blocks = new SudokuBlock[n];
            for (int k = 0; k < n; k++)
            {
                blocks[k] = Blocks[k][j];
            }
            SudokuColumn[] columns = new SudokuColumn[n];
            for (int k = 0; k < n; k++)
            {
                columns[k] = Columns[j * n + k];
            }
            for (int k = 1; k <= n2; k++)
            {
                rtVal |= Try3ColumnsWithValue(blocks, columns, k);
                if (rtVal)
                {
                    return rtVal;
                }
            }
            return rtVal;
        }

        public bool Try3ColumnsWithValue(SudokuBlock[] blocks, SudokuColumn[] columns, int v)
        {
            bool rtVal = false;
            SudokuItem[] items = new SudokuItem[n];
            for (int i = 0; i < columns.Length; i++)
            {
                items[i] = columns[i].GetItemByValue(v);
            }
            int count = items.Count(x => x != null);
            if (count == n - 1)
            {
                int[] blockIndex = new int[n];
                int columnIndex = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == null)
                    {
                        columnIndex = i;
                    }
                    else
                    {
                        SudokuBlock tempBlock = GetBlock(items[i].i, items[i].j);
                        blockIndex[tempBlock.i] = 1;
                    }
                }
                SudokuBlock block = null;
                for (int i = 0; i < n; i++)
                {
                    if (blockIndex[i] == 0)
                    {
                        block = blocks[i];
                        break;
                    }
                }
                if (block.TryColumnCompleteWithValue(columnIndex, v))
                {
                    rtVal = true;
                }
            }
            return rtVal;
        }

        public bool IsComplete()
        {
            bool rtVal = false;
            rtVal = Rows.All(x => x.IsComplete());
            rtVal = Columns.All(x => x.IsComplete());
            return rtVal;
        }

        public string Print2String()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < n2; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    string strNumber = Items[i][j].value == 0 ? " " : Items[i][j].value.ToString();
                    builder.Append($"{strNumber:D1} ");
                    if (j > 0 && (j + 1) % n == 0 && j < n2 - 1)
                    {
                        builder.Append("| ");
                    }
                }
                builder.AppendLine();
                if (i > 0 && (i + 1) % n == 0 && i < n2 - 1)
                {
                    builder.AppendLine("---------------------");
                }
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
            for (int i = 0; i < n2; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    Items[i][j].Reset();
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
                for (int i = 0; i < n2; i++)
                {
                    string[] parts = lines[i].Split(' ');
                    if (parts.Length < n2)
                    {
                        return false;
                    }
                    for (int j = 0; j < n2; j++)
                    {
                        if (int.TryParse(parts[j], out int v))
                        {
                            Items[i][j].SetValue(v);
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
    }
}