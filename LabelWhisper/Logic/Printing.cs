using Avalonia.Media.Imaging;
using QuestPDF.Fluent;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelWhisper.Logic
{
    public static class Printing
    {
        public static async Task Print(float labelWidth, float labelHeight, Bitmap image, int printCount = 1)
        {
            StringBuilder randomFilename = new();

            for (int i = 0; i < 6; i++)
            {
                Random rnd = new(BitConverter.ToInt32(Guid.NewGuid().ToByteArray()));
                randomFilename.Append((char)rnd.Next(65, 90));
            }

            randomFilename.Append(".pdf");

            string pdfPath = Path.Combine(Program.AppLocalBasePath, "tmp", randomFilename.ToString());

            if (!Directory.Exists(Path.GetDirectoryName(pdfPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(pdfPath));
            }

            using (MemoryStream ms = new())
            {
                image.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                ms.Position = 0;

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(labelWidth, labelHeight, QuestPDF.Infrastructure.Unit.Millimetre);
                        page.Margin(0);
                        page.Content().Element(c =>
                        {
                            c.Width(labelWidth, QuestPDF.Infrastructure.Unit.Millimetre).Height(labelHeight, QuestPDF.Infrastructure.Unit.Millimetre).Image(ms).FitArea();
                        });
                    });
                }).GeneratePdf(pdfPath);
            }

            if (OperatingSystem.IsWindows())
            {
                string sumatraFileName = string.Join('.', Globals.Assembly.GetManifestResourceNames().First(x =>
                                                                                                    x.Contains("sumatra", StringComparison.InvariantCultureIgnoreCase) &&
                                                                                                    x.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)).Split('.').Skip(2));

                await Process.Start(new ProcessStartInfo()
                {
                    FileName = Path.Combine(Program.AppLocalBasePath, sumatraFileName),
                    Arguments = $"-print-settings \"{printCount}x\" -exit-when-done -print-to \"{Globals.UserConfig.RuntimeConfiguration.PrinterName}\" \"{pdfPath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                }).WaitForExitAsync();
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                Process.Start("lp", pdfPath);
            }
        }
    }
}
