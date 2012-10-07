using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Linkknil.Web.Controllers
{
    public class ImageController : Controller
    {
        Random rand = new Random();

        //
        // GET: /Image/

        public ActionResult Format(int width, int height, string id1, string id2)
        {
            //定义绘制曲线的颜色
            Pen greenPen = new Pen(Color.Green, 2);
            //Pen blackPen = new Pen(Color.Pink, 1);
            //blackPen.DashStyle = DashStyle.Dot;

            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            //定义底色
            var bgColor = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
            g.Clear(bgColor);

            if (width == 30 && height == 30)
            {
                Pen pen = new Pen(Color.FromArgb(255 - bgColor.R, 255 - bgColor.G, 255 - bgColor.B), 2);
                g.DrawArc(pen, 5, 5, 20, 20, rand.Next(0, 100), rand.Next(100, 360));
            }
            else
            {
                g.DrawString("1:" + id1, new Font("宋体", 18), Brushes.White, new PointF(5, 5));
                g.DrawString("2:" + id2, new Font("宋体", 18), Brushes.White, new PointF(5, height/2));
            }

            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            var bytes = stream.ToArray();

            return File(bytes, "image/png");/*application/x-img*/
        }

    }
}
