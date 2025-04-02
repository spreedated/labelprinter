using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrinter.Logic
{
    public class ImageRender : IDisposable
    {
        private FontFamily mainFont;
        internal Image renderResult;
        private bool disposedValue;

        public int ImageWidth { get; init; }
        public int ImageHeight { get; init; }
        public bool DrawBackground { get; set; }

        #region Ctor
        public ImageRender(int imageWidth, int imageHeight)
        {
            if (imageWidth == default || imageWidth <= -1)
            {
                throw new ArgumentException("Width cannot be null or less", nameof(imageWidth));
            }

            if (imageHeight == default || imageHeight <= -1)
            {
                throw new ArgumentException("Height cannot be null or less", nameof(imageHeight));
            }

            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;

            this.LoadTextOptions();
        }
        #endregion

        private void LoadTextOptions()
        {
            FontCollection fonts = new();

            using (Stream s = Globals.Assembly.GetManifestResourceStream("LabelPrinter.Assets.GEFORCE-BOLD.TTF"))
            {
                this.mainFont = fonts.Add(s);
            }
        }

        public async Task<Bitmap> SaveAsAvaloniaImage()
        {
            if (this.renderResult == null)
            {
                return null;
            }

            Bitmap result = null;

            using (MemoryStream ms = new())
            {
                await this.renderResult.SaveAsync(ms, new PngEncoder());
                ms.Position = 0;
                ms.Seek(0, SeekOrigin.Begin);

                result = new(ms);
            }

            return result;
        }

        public async Task<Image> Render(float textSize)
        {
            this.renderResult?.Dispose();

            Image image = new Image<Rgba32>(this.ImageWidth, this.ImageHeight, this.DrawBackground ? Color.White : Color.Transparent);

            RichTextOptions textOptions = new(this.mainFont.CreateFont(textSize, FontStyle.Regular))
            {
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            await Task.Run(() =>
            {
                image.Mutate(x => x.DrawText(textOptions, "Empfänger:", new SolidBrush(Color.Black)));
            });

            this.renderResult = image;

            return this.renderResult;
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.renderResult?.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
