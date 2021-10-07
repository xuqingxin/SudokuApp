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
            Bitmap bmp = (Bitmap)Image.FromFile("Sudoku.bmp");
            if (bmp.PixelFormat == PixelFormat.Format32bppRgb)
            {
                ImageUtility.ARGB2Gray(ref bmp);
            }
            else if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
            {
                ImageUtility.RGB2Gray(ref bmp);
            }

            Bitmap mono = ImageUtility.Gray2Mono(bmp);
            bmp.Dispose();
            bmp = mono;
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

            int margin = 8;
            int[] cxs = new int[colData.Count];
            int[] dxs = new int[colData.Count];
            for (int i = 0; i < colData.Count; i++)
            {
                List<int> lst = colData[i + 1];
                cxs[i] = lst[lst.Count / 2];
                dxs[i] = lst[lst.Count - 1] - lst[0];
            }
            int dx = dxs.Max() + margin;

            int[] cys = new int[rowData.Count];
            int[] dys = new int[rowData.Count];
            for (int i = 0; i < rowData.Count; i++)
            {
                List<int> lst = rowData[i + 1];
                cys[i] = lst[lst.Count / 2];
                dys[i] = lst[lst.Count - 1] - lst[0];
            }
            int dy = dys.Max() + margin;

            sudoku.Reset();

            Bitmap img = bmp;// (Bitmap)Image.FromFile("Sudoku.bmp");
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
            OcrUtility.Instance.Uninit();
            img.Dispose();

            sudoku.Print();
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
    }
}