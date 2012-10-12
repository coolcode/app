using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Linkknil.Services {
    public class ImageService {

        public Stream RoundedCorner(Stream imageStream) {
            var targetStream = new MemoryStream();
            var originalImage = Image.FromStream(imageStream);
            var targetImage = new Bitmap(57, 57);
            var g = Graphics.FromImage(targetImage);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            var rect = new Rectangle(0, 0, targetImage.Width, targetImage.Height);
            var rectPath = GetRoundPath(0, 0, targetImage.Width, targetImage.Height, targetImage.Width / 9f);

            var r = new Region(rectPath);
            g.Clip = r;
            Brush b = new SolidBrush(Color.FromArgb(30, 255, 255, 255));

            //图片缩放   
            g.DrawImage(originalImage, rect, new Rectangle(0, 0, originalImage.Width, originalImage.Height), GraphicsUnit.Pixel);

            g.DrawPath(new Pen(b), rectPath);
            g.FillPie(b, -targetImage.Width * (0.309f), -targetImage.Height / 2f, targetImage.Width * (1 / 0.618f), targetImage.Height, 0, 360);
            //g.FillPath(b, rectPath);
            g.Dispose();
            targetImage.Save(targetStream, ImageFormat.Png);
            targetImage.Dispose();
            originalImage.Dispose();
            
            targetStream.Position = 0;

            return targetStream;
        }

        public GraphicsPath GetRoundPath(float X, float Y, float width, float height, float radius) {
            GraphicsPath gp = new GraphicsPath();

            gp.AddLine(X + radius, Y, X + width - (radius * 2), Y);

            gp.AddArc(X + width - (radius * 2), Y, radius * 2, radius * 2, 270, 90);

            gp.AddLine(X + width, Y + radius, X + width, Y + height - (radius * 2));

            gp.AddArc(X + width - (radius * 2), Y + height - (radius * 2), radius * 2, radius * 2, 0, 90);

            gp.AddLine(X + width - (radius * 2), Y + height, X + radius, Y + height);

            gp.AddArc(X, Y + height - (radius * 2), radius * 2, radius * 2, 90, 90);

            gp.AddLine(X, Y + height - (radius * 2), X, Y + radius);

            gp.AddArc(X, Y, radius * 2, radius * 2, 180, 90);

            gp.CloseFigure();

            return gp;
        }
    }
}
