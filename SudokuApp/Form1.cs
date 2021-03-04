using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            if (sudoku.Load("D:\\sudoku.txt"))
            {
                sudoku.Print();
            }
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
    }
}