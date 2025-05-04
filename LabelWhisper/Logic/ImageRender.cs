using Avalonia.Media.Imaging;
using Microsoft.Extensions.Logging;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LabelWhisper.Logic
{
    public class ImageRender : IDisposable
    {
        private FontFamily mainFont;
        internal Image renderResult;
        private bool disposedValue;
        private readonly ILogger logger;

        public int ImageWidth { get; init; }
        public int ImageHeight { get; init; }
        public bool DrawBackground { get; set; }

        #region Ctor
        public ImageRender(int imageWidth, int imageHeight, ILogger logger = null)
        {
            if (imageWidth == default || imageWidth <= -1)
            {
                throw new ArgumentException("Width cannot be null or less", nameof(imageWidth));
            }

            if (imageHeight == default || imageHeight <= -1)
            {
                throw new ArgumentException("Height cannot be null or less", nameof(imageHeight));
            }

            this.logger = logger;

            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;

            this.LoadTextOptions();
        }
        #endregion

        private void LoadTextOptions()
        {
            FontCollection fonts = new();

            using (Stream s = Globals.Assembly.GetManifestResourceStream($"{Globals.Assembly.GetName().Name}.Assets.GEFORCE-BOLD.TTF"))
            {
                this.mainFont = fonts.Add(s);
            }
            this.logger?.LogTrace("[{Name}] Loaded font", "ImageRenderer");
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

            this.logger?.LogTrace("[{Name}] Image converted & saved to AvaloniaBitmap", "ImageRenderer");

            return result;
        }

        public async Task<Image> Render(float textSize, IEnumerable<string> rows, bool addEmpfaenger = true)
        {
            this.logger?.LogTrace("[{Name}] Start rendering...", "ImageRenderer");
            Stopwatch sw = Stopwatch.StartNew();

            this.renderResult?.Dispose();

            Image image = new Image<Rgba32>(this.ImageWidth, this.ImageHeight, this.DrawBackground ? Color.White : Color.Transparent);

            RichTextOptions textOptions = new(this.mainFont.CreateFont(textSize, FontStyle.Regular))
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Origin = new PointF(12, 12),
            };

            await Task.Run(() =>
            {
                if (addEmpfaenger)
                {
                    image.Mutate(x => x.DrawText(textOptions, "Empfänger:", new SolidBrush(Color.Black)));
                }

                textOptions.Origin = addEmpfaenger ? new PointF(16, 24) : new PointF(12, 4);

                foreach (string r in rows)
                {
                    if (string.IsNullOrEmpty(r) && rows.Count() <= 1)
                    {
                        continue;
                    }

                    if (rows.Count() >= 2 || addEmpfaenger)
                    {
                        textOptions.Origin = new PointF(textOptions.Origin.X, textOptions.Origin.Y + textSize + 2);
                    }

                    if (rows.Count() >= 2 && string.IsNullOrEmpty(r))
                    {
                        continue;
                    }

                    image.Mutate(x => x.DrawText(textOptions, r, new SolidBrush(Color.Black)));
                }
            });

            this.renderResult = image;

            sw.Stop();
            this.logger?.LogTrace("[{Name}] Rendered image in {Elapsed}ms", "ImageRenderer", sw.ElapsedMilliseconds);

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
