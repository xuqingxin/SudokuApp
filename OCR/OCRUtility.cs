using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace OCR
{
    public class OcrUtility
    {
        public static OcrUtility Instance { get; set; } = new OcrUtility();

        private TesseractEngine _tesseract;

        private OcrUtility()
        {
            _tesseract = new TesseractEngine("tessdata", "eng", EngineMode.Default);//构建对象并加载训练好的数据
            _tesseract.DefaultPageSegMode = PageSegMode.SingleChar;//设为单个字符
        }

        public int OCR(Bitmap bmp)
        {
            int result = -1;
            Page page = _tesseract.Process(bmp);//处理图片
            //if (page.GetMeanConfidence() > 0.5 && int.TryParse(page.GetText(), out int v))
            if (page.GetMeanConfidence() > 0.4)
            {
                if (int.TryParse(page.GetText(), out int v))
                {
                    result = v;
                }
                else
                {
                    result = 0;
                }
            }
            page.Dispose();
            return result;
        }

        public int OCRByCmd(string bmpFilename)
        {
            int result = -1;
            try
            {
                //创建一个进程
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();//启动程序

                string strCMD = $"Tesseract-OCR\\winpath.exe tesseract {bmpFilename} ocr --psm 10";
                //向cmd窗口发送输入信息
                p.StandardInput.WriteLine(strCMD + "&exit");

                p.StandardInput.AutoFlush = true;

                //获取cmd窗口的输出信息
                string output = p.StandardOutput.ReadToEnd();
                //等待程序执行完退出进程
                p.WaitForExit();
                p.Close();

                string text = File.ReadAllText("ocr.txt");
                if (text.Length > 1)
                {
                    if (int.TryParse(text.Substring(0, 1), out int v))
                    {
                        result = v;
                    }
                    else
                    {
                        result = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                //
            }
            return result;
        }

        public void Uninit()
        {
            _tesseract.Dispose();
        }
    }
}