using OCR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        private Sudoku sudoku = new Sudoku();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<int> rows = new List<int>();
            List<int> cols = new List<int>();
            Bitmap bmp = (Bitmap)Image.FromFile("Sudoku.png");
            if (bmp.PixelFormat == PixelFormat.Format32bppArgb)
            {
                ImageUtility.ARGB2Gray(ref bmp);
            }
            else if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
            {
                ImageUtility.RGB2Gray(ref bmp);
            }
            ImageUtility.Gray2Mono(ref bmp);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            int length = bmpData.Stride * bmp.Height;
            byte[] byteData = new byte[length];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, byteData, 0, length);
            bmp.UnlockBits(bmpData);
            for (int j = 0; j < bmp.Height; j++)
            {
                for (int i = 0; i < bmp.Width; i++)
                {
                    byte b = byteData[j * bmpData.Stride + i];
                    if (b < 128)
                    {
                        if (!rows.Contains(j))
                        {
                            rows.Add(j);
                        }
                        if (!cols.Contains(i))
                        {
                            cols.Add(i);
                        }
                    }
                }
            }
            rows.Sort();
            cols.Sort();
            Dictionary<int, List<int>> rowData = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> colData = new Dictionary<int, List<int>>();
            int lastRow = 0;
            for (int i = 0; i < rows.Count - 1; i++)
            {
                int d = rows[i + 1] - rows[i];
                if (d > 1)
                {
                    rowData[rowData.Count + 1] = new List<int>();
                    for (int k = lastRow; k < i + 1; k++)
                    {
                        rowData[rowData.Count].Add(rows[k]);
                    }
                    lastRow = i + 1;
                }
            }
            rowData[rowData.Count + 1] = new List<int>();
            for (int k = lastRow; k < rows.Count; k++)
            {
                rowData[rowData.Count].Add(rows[k]);
            }

            // 去除干扰点的影响
            int rowSum = 0;
            foreach (var pair in rowData)
            {
                rowSum += pair.Value.Count();
            }
            int rowAvg = rowSum / rowData.Count;
            int[] rowKeys = rowData.Keys.ToArray();
            for (int i = 0; i < rowKeys.Length; i++)
            {
                if (rowData[rowKeys[i]].Count < rowAvg)
                {
                    rowData.Remove(rowKeys[i]);
                }
            }
            rowKeys = rowData.Keys.ToArray();

            int lastCol = 0;
            for (int j = 0; j < cols.Count - 1; j++)
            {
                int d = cols[j + 1] - cols[j];
                if (d > 1)
                {
                    colData[colData.Count + 1] = new List<int>();
                    for (int k = lastCol; k < j + 1; k++)
                    {
                        colData[colData.Count].Add(cols[k]);
                    }
                    lastCol = j + 1;
                }
            }
            colData[colData.Count + 1] = new List<int>();
            for (int k = lastCol; k < cols.Count; k++)
            {
                colData[colData.Count].Add(cols[k]);
            }

            // 去除干扰点的影响
            int colSum = 0;
            foreach (var pair in colData)
            {
                colSum += pair.Value.Count();
            }
            int colAvg = colSum / colData.Count;
            int[] colKeys = colData.Keys.ToArray();
            for (int i = 0; i < colKeys.Length; i++)
            {
                if (colData[colKeys[i]].Count < colAvg)
                {
                    colData.Remove(colKeys[i]);
                }
            }
            colKeys = colData.Keys.ToArray();

            // 计算数字中心点
            int margin = 8;
            int[] cxs = new int[colData.Count];
            int[] dxs = new int[colData.Count];
            for (int i = 0; i < colKeys.Length; i++)
            {
                List<int> lst = colData[colKeys[i]];
                cxs[i] = lst[lst.Count / 2];
                dxs[i] = lst[lst.Count - 1] - lst[0];
            }
            int dx = dxs.Max() + margin;

            int[] cys = new int[rowData.Count];
            int[] dys = new int[rowData.Count];
            for (int i = 0; i < rowKeys.Length; i++)
            {
                List<int> lst = rowData[rowKeys[i]];
                cys[i] = lst[lst.Count / 2];
                dys[i] = lst[lst.Count - 1] - lst[0];
            }
            int dy = dys.Max() + margin;

            sudoku.Reset();
            Bitmap img = bmp;
            for (int j = 0; j < dys.Length; j++)
            {
                for (int i = 0; i < dxs.Length; i++)
                {
                    ImageFormat imageFormat = ImageFormat.Png;
                    string filename = $"{i + 1}-{j + 1}.{imageFormat.ToString().ToLower()}";
                    Bitmap sub = img.Clone(new Rectangle(cxs[i] - dx / 2, cys[j] - dy / 2, dx, dy), bmp.PixelFormat);
                    sub.Save(filename, imageFormat);
                    int value = OcrUtility.Instance.OCR(sub);
                    //int value = OcrUtility.Instance.OCRByCmd(filename);
                    if (value > 0)
                    {
                        sudoku.GetItem(i, j).SetValue(value);
                    }
                    else if (value == 0)
                    {
                        Console.WriteLine($"OCR error:{i + 1}-{j + 1}");
                    }
                }
            }
            img.Dispose();

            sudoku.Print();
            sudoku.SaveBy0("Sudoku.txt");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool rtVal = sudoku.GetNextNumber();
            if (rtVal)
            {
                sudoku.Print();
            }
            else
            {
                if (sudoku.IsComplete())
                {
                    Console.WriteLine("Done");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool rtVal = sudoku.GetNextNumber2();
            sudoku.Print();
            if (sudoku.IsComplete())
            {
                Console.WriteLine("Done");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (sudoku.Load("Sudoku.txt"))
            {
                sudoku.Print();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            OcrUtility.Instance.Uninit();
        }
    }
}