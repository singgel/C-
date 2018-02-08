using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Catarc.Adc.NewEnergyAccountSys.OfficeHelper;

namespace Catarc.Adc.NewEnergyAccountSys.Common
{
    public class ImageTool
    {
        public static  int size = 1000;
        public static string MakeThumbnail(string filesource,string path)
        {
            string temp = "";
            try
            {
                Image img = Image.FromFile(filesource);
                String PictureFileName_ALL = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + ".jpg";
                if (img.Width > img.Height)
                {
                    if (img.Height > size)
                    {
                        ImageTool.MakeThumbnail(filesource, path+PictureFileName_ALL, size, size, "H");
                    }
                    else
                    {
                        SysIOHelper.CopyFile(filesource, path, PictureFileName_ALL);
                    }
                }
                else
                {
                    if (img.Width > size)
                    {
                        ImageTool.MakeThumbnail(filesource, path + PictureFileName_ALL, size, size, "W");
                    }
                    else
                    {
                        SysIOHelper.CopyFile(filesource, path, PictureFileName_ALL);
                    }
                }
                img.Dispose();
                temp = PictureFileName_ALL;
            }
            catch (System.Exception ex)
            {
                temp = String.Empty;
                throw ex;
            }
            return temp;
        }

        public static string MakeThumbnail(string filesource, string path, string fileName)
        {
            string temp = "";
            fileName = fileName + ".jpg";
            try
            {
                Image img = Image.FromFile(filesource);
                if (img.Width > img.Height)
                {
                    if (img.Height > size)
                    {
                        ImageTool.MakeThumbnail(filesource, path + fileName, size, size, "H");
                    }
                    else
                    {
                        SysIOHelper.CopyFile(filesource, path, fileName);
                    }
                }
                else
                {
                    if (img.Width > size)
                    {
                        ImageTool.MakeThumbnail(filesource, path + fileName, size, size, "W");
                    }
                    else
                    {
                        SysIOHelper.CopyFile(filesource, path, fileName);
                    }
                }
                img.Dispose();
                temp = fileName;
            }
            catch (System.Exception ex)
            {
                temp = String.Empty;
                throw ex;
            }
            return temp;
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param> 
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(originalImagePath);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）   
                    break;
                case "W"://指定宽，高按比例   
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）   
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }
            //新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
             new Rectangle(x, y, ow, oh),
             GraphicsUnit.Pixel);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(thumbnailPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(thumbnailPath));
                }
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

    }
}
