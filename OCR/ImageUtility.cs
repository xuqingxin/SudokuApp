using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR
{
    public static class ImageUtility
    {
        public static void ARGB2Gray(ref Bitmap srcBitmap)
        {
            Bitmap newBitmap = ARGB2Gray(srcBitmap);
            srcBitmap.Dispose();
            srcBitmap = newBitmap;
        }

        public static Bitmap ARGB2Gray(Bitmap srcBitmap)
        {
            return null;
        }

        public static void RGB2Gray(ref Bitmap srcBitmap)
        {
            Bitmap newBitmap = RGB2Gray(srcBitmap);
            srcBitmap.Dispose();
            srcBitmap = newBitmap;
        }

        public static Bitmap RGB2Gray(Bitmap srcBitmap)
        {
            int width = srcBitmap.Width;

            int height = srcBitmap.Height;

            Rectangle rect = new Rectangle(0, 0, width, height);

            //将Bitmap锁定到系统内存中,获得BitmapData

            BitmapData srcBmData = srcBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //创建Bitmap

            Bitmap dstBitmap = CreateGrayscaleImage(width, height);//这个函数在后面有定义

            BitmapData dstBmData = dstBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行

            System.IntPtr srcPtr = srcBmData.Scan0;

            System.IntPtr dstPtr = dstBmData.Scan0;

            //将Bitmap对象的信息存放到byte数组中

            int src_bytes = srcBmData.Stride * height;

            byte[] srcValues = new byte[src_bytes];

            int dst_bytes = dstBmData.Stride * height;

            byte[] dstValues = new byte[dst_bytes];

            //复制GRB信息到byte数组

            System.Runtime.InteropServices.Marshal.Copy(srcPtr, srcValues, 0, src_bytes);

            System.Runtime.InteropServices.Marshal.Copy(dstPtr, dstValues, 0, dst_bytes);

            //根据Y=0.299*R+0.587G+0.114*B,Y为亮度

            for (int i = 0; i < height; i++)

                for (int j = 0; j < width; j++)

                {
                    //只处理每行中图像像素数据,舍弃未用空间

                    //注意位图结构中RGB按BGR的顺序存储

                    int k = 3 * j;

                    byte temp = (byte)(srcValues[i * srcBmData.Stride + k + 2] * .299 + srcValues[i * srcBmData.Stride + k + 1] * .587 + srcValues[i * srcBmData.Stride + k] * .114);

                    dstValues[i * dstBmData.Stride + j] = temp;
                }

            System.Runtime.InteropServices.Marshal.Copy(dstValues, 0, dstPtr, dst_bytes);

            //解锁位图

            srcBitmap.UnlockBits(srcBmData);

            dstBitmap.UnlockBits(dstBmData);

            return dstBitmap;
        }

        public static void Gray2Mono(ref Bitmap srcBitmap, int threshold = 128)
        {
            // check pixel format

            if (srcBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException();

            int width = srcBitmap.Width;

            int height = srcBitmap.Height;

            Rectangle rect = new Rectangle(0, 0, width, height);

            //将Bitmap锁定到系统内存中,获得BitmapData

            BitmapData srcBmData = srcBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            System.IntPtr srcPtr = srcBmData.Scan0;

            //将Bitmap对象的信息存放到byte数组中

            int src_bytes = srcBmData.Stride * height;

            byte[] srcValues = new byte[src_bytes];

            System.Runtime.InteropServices.Marshal.Copy(srcPtr, srcValues, 0, src_bytes);

            for (int i = 0; i < srcValues.Length; i++)
            {
                srcValues[i] = (byte)(srcValues[i] >= threshold ? 255 : 0);
            }

            System.Runtime.InteropServices.Marshal.Copy(srcValues, 0, srcPtr, src_bytes);

            //解锁位图

            srcBitmap.UnlockBits(srcBmData);
        }

        public static Bitmap Gray2Mono(Bitmap srcBitmap, int threshold = 128)
        {
            // check pixel format

            if (srcBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException();

            int width = srcBitmap.Width;

            int height = srcBitmap.Height;

            Rectangle rect = new Rectangle(0, 0, width, height);

            //将Bitmap锁定到系统内存中,获得BitmapData

            BitmapData srcBmData = srcBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            //创建Bitmap

            Bitmap dstBitmap = CreateGrayscaleImage(width, height);//这个函数在后面有定义

            BitmapData dstBmData = dstBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            System.IntPtr srcPtr = srcBmData.Scan0;

            System.IntPtr dstPtr = dstBmData.Scan0;

            //将Bitmap对象的信息存放到byte数组中

            int src_bytes = srcBmData.Stride * height;

            byte[] srcValues = new byte[src_bytes];

            int dst_bytes = dstBmData.Stride * height;

            byte[] dstValues = new byte[dst_bytes];

            //复制GRB信息到byte数组

            System.Runtime.InteropServices.Marshal.Copy(srcPtr, srcValues, 0, src_bytes);

            System.Runtime.InteropServices.Marshal.Copy(dstPtr, dstValues, 0, dst_bytes);

            for (int i = 0; i < dstValues.Length; i++)
            {
                dstValues[i] = (byte)(srcValues[i] >= threshold ? 255 : 0);
            }

            System.Runtime.InteropServices.Marshal.Copy(dstValues, 0, dstPtr, dst_bytes);

            //解锁位图

            srcBitmap.UnlockBits(srcBmData);

            dstBitmap.UnlockBits(dstBmData);

            return dstBitmap;
        }

        public static Bitmap CreateGrayscaleImage(int width, int height)

        {
            // create new image

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // set palette to grayscale

            SetGrayscalePalette(bmp);

            // return new image

            return bmp;
        }

        ///<summary>
        /// Set pallete of the image to grayscale
        ///</summary>

        public static void SetGrayscalePalette(Bitmap srcImg)
        {
            // check pixel format
            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)

                throw new ArgumentException();

            // get palette
            ColorPalette cp = srcImg.Palette;

            // init palette
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }

            srcImg.Palette = cp;
        }
    }
}