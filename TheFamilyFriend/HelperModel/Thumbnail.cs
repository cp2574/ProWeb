using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.HelperModel
{
    public class Thumbnail
    {

        /// <summary>
        /// 生成缩略图(自动计算宽高)
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图保存路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="height">是否删除原图</param> 
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, bool isDeleteoriginalImage)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;       //源图片宽
            int oh = originalImage.Height;      //源图片高

            if (towidth >= ow && toheight >= oh)     //如果源图片等于或小于要生成图片的大小,则直接复制
            {
                File.Copy(originalImagePath, thumbnailPath,true);
                if (isDeleteoriginalImage)
                {
                    File.Delete(originalImagePath);
                }
                originalImage.Dispose();
                return;
            }

            double wh = towidth / toheight;
            double owh = ow / oh;

            //按等比宽计算生成图片的宽高
            toheight = oh * towidth / ow;       //得到按宽等比的高


            if (toheight > height) //如果等比的高还是大于生成缩略图的高
            {
                towidth = towidth * height / toheight;
                toheight = height;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;



            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);

            //开始定义打水印
            //System.Drawing.Font f = new System.Drawing.Font("Verdana", 16);
            //System.Drawing.Brush b = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
            //g.DrawString("点起床啊\r\nsfzdsfdsfdsfsdfdsfdsf111", f, b, 15, 15);


            try
            {
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

        /// <summary>
        /// 生成缩略图(自动计算宽高) 1024*768
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图保存路径（物理路径）</param>
        /// <param name="height">是否删除原图</param> 
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, bool isDeleteoriginalImage) {
            MakeThumbnail(originalImagePath, thumbnailPath, 1024,768,isDeleteoriginalImage);
        }



        /// <summary>
        /// 生成缩略图(自动计算宽高)
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图保存路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="height">是否删除原图</param> 
        public static void MakeThumbnail(Stream stream, string thumbnailPath, int width, int height)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(stream);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;       //源图片宽
            int oh = originalImage.Height;      //源图片高

            //if (towidth >= ow && toheight >= oh)     //如果源图片等于或小于要生成图片的大小,则直接复制
            //{
            //    File.Copy(originalImagePath, thumbnailPath);
            //    if (isDeleteoriginalImage)
            //    {
            //        File.Delete(originalImagePath);
            //    }
            //    originalImage.Dispose();
            //    return;
            //}

            double wh = towidth / toheight;
            double owh = ow / oh;

            //按等比宽计算生成图片的宽高
            toheight = oh * towidth / ow;       //得到按宽等比的高


            if (toheight > height) //如果等比的高还是大于生成缩略图的高
            {
                towidth = towidth * height / toheight;
                toheight = height;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;



            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);

            //开始定义打水印
            //System.Drawing.Font f = new System.Drawing.Font("Verdana", 16);
            //System.Drawing.Brush b = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
            //g.DrawString("点起床啊\r\nsfzdsfdsfdsfsdfdsfdsf111", f, b, 15, 15);


            try
            {
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



        /// <summary>
        /// 生成缩略图(自动计算宽高) 1024*768
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图保存路径（物理路径）</param>
        /// <param name="height">是否删除原图</param> 
        public static void MakeThumbnail(Stream stream, string thumbnailPath)
        {
            MakeThumbnail(stream, thumbnailPath, 1024, 768);
        }
    }
}