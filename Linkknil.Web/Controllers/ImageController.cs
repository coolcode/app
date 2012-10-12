using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using CoolCode.Data.Entity;
using CoolCode.ServiceModel.Mvc;
using Linkknil.Services;
using Linkknil.StreamStore;

namespace Linkknil.Web.Controllers {
    public class ImageController : SharedController {
        [HttpPost]
        public ActionResult UploadAppIcon(string id) {
            if (Request.Files.Count == 0) {
                return this.RedirectToError("上传图片错误！");
            }

            var file = Request.Files[0];
            var imageService = new ImageService();
            var iconStream = imageService.RoundedCorner(file.InputStream);
            var fileService = CurrentResolver.GetService<IFileService>();
            var filePath = DateTime.Now.ToString("yyyyMMddHHmmss-") + Path.GetFileNameWithoutExtension(file.FileName)+".png";
            fileService.Save(filePath, iconStream);

            db.Execute(@"update PF_App set IconPath=@IconPath where id = @id", new { id, IconPath = filePath });

            return RedirectToAction("AppDetails", "Developer", new { id = id });
        }

        [OutputCache(Duration = 300)]
        [ActionName("AppIcon")]
        public ActionResult GetAppIcon(string id) {
            try {
                var fileService = CurrentResolver.GetService<IFileService>();
                var fileStream = fileService.Get(id);

                return this.File(fileStream, "image/png");
            }
            catch (Exception ex) {
                var fileName = Server.MapPath("~/content/images/apps/2.png");

                return this.File(fileName, "image/png");
            }
        }

        Random rand = new Random();

        public ActionResult Format(int width, int height, string id1, string id2) {
            //定义绘制曲线的颜色
            Pen greenPen = new Pen(Color.Green, 2);
            //Pen blackPen = new Pen(Color.Pink, 1);
            //blackPen.DashStyle = DashStyle.Dot;

            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            //定义底色
            var bgColor = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
            g.Clear(bgColor);

            if (width == 30 && height == 30) {
                Pen pen = new Pen(Color.FromArgb(255 - bgColor.R, 255 - bgColor.G, 255 - bgColor.B), 2);
                g.DrawArc(pen, 5, 5, 20, 20, rand.Next(0, 100), rand.Next(100, 360));
            }
            else {
                g.DrawString("1:" + id1, new Font("宋体", 18), Brushes.White, new PointF(5, 5));
                g.DrawString("2:" + id2, new Font("宋体", 18), Brushes.White, new PointF(5, height / 2));
            }

            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            var bytes = stream.ToArray();

            return File(bytes, "image/png");/*application/x-img*/
        }

    }
}
