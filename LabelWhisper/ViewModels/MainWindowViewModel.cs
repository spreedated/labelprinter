using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabelWhisper.Logic;
using LabelWhisper.Views;
using Microsoft.Extensions.Logging;
using neXn.Ui.Animation;
using Serilog.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LabelWhisper.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly TextWaitingAnimation textWaitingAnimation;
        private string currentStatus = "System ready";
        private readonly ILogger logger = new SerilogLoggerProvider().CreateLogger("MainWindowViewModel");

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private Window instance;

        [ObservableProperty]
        private int labelWidth;

        [ObservableProperty]
        private int labelHeight;

        [ObservableProperty]
        private string printername;

        [ObservableProperty]
        private string groupBoxLabelText;

        [ObservableProperty]
        private string status;

        [ObservableProperty]
        private string appTitle;

        [ObservableProperty]
        private string appVersion;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private BindingList<int> availableTextSizes = new([16, 18, 20, 22, 24, 28, 32, 36, 40]);

        [ObservableProperty]
        private int selectedTextSize;
        partial void OnSelectedTextSizeChanged(int value)
        {
            Globals.UserConfig.RuntimeConfiguration.LastUsedTextsize = value;
            Task.Run(() => Globals.UserConfig.Save());
            Dispatcher.UIThread.Invoke(async () => await this.RenderImage());
        }

        [ObservableProperty]
        private string line1;

        partial void OnLine1Changed(string value)
        {
            Dispatcher.UIThread.Invoke(async () => await this.RenderImage());
        }

        [ObservableProperty]
        private string line2;

        partial void OnLine2Changed(string value)
        {
            Dispatcher.UIThread.Invoke(async () => await this.RenderImage());
        }

        [ObservableProperty]
        private string line3;

        partial void OnLine3Changed(string value)
        {
            Dispatcher.UIThread.Invoke(async () => await this.RenderImage());
        }

        [ObservableProperty]
        private string line4;

        partial void OnLine4Changed(string value)
        {
            Dispatcher.UIThread.Invoke(async () => await this.RenderImage());
        }

        [ObservableProperty]
        private string freeText;

        partial void OnFreeTextChanged(string value)
        {
            Dispatcher.UIThread.Invoke(async () => await this.RenderImage());
        }

        [ObservableProperty]
        private bool useTextfield;

        partial void OnUseTextfieldChanged(bool value)
        {
            Dispatcher.UIThread.Invoke(async () => await this.RenderImage());
        }

        [ObservableProperty]
        private bool drawEmpfaenger = true;

        partial void OnDrawEmpfaengerChanged(bool value)
        {
            Dispatcher.UIThread.Invoke(async () => await this.RenderImage());
        }

        [ObservableProperty]
        private Bitmap renderedImage;

        [ObservableProperty]
        private int printCount = 1;

        public MainWindowViewModel()
        {
            this.AppTitle = Globals.Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
            this.AppVersion = $"v{Globals.Assembly.GetName().Version}";
            if (Globals.UserConfig != null)
            {
                this.SelectedTextSize = Globals.UserConfig.RuntimeConfiguration.LastUsedTextsize == default ? this.AvailableTextSizes[^1] : Globals.UserConfig.RuntimeConfiguration.LastUsedTextsize;
            }
            this.Title = $"{this.appTitle} {this.AppVersion}";

            this.textWaitingAnimation = new()
            {
                UseBrackets = true,
                AnimationType = TextWaitingAnimation.AnimationTypes.BlockChars,
                Interval = 400
            };
            this.textWaitingAnimation.AnimationChanged += this.TextWaitingAnimation_AnimationChanged;
            this.textWaitingAnimation.Start();

            this.RefreshOptionDisplay();
        }

        private void TextWaitingAnimation_AnimationChanged(object sender, string e)
        {
            this.Status = $"{e} {this.currentStatus}";
        }

        public async Task RenderImage()
        {
            if (this.Instance == null)
            {
                return;
            }

            using (ImageRender ir = new((int)Conversions.MillimeterToPixel(this.LabelWidth), (int)Conversions.MillimeterToPixel(this.LabelHeight), new SerilogLoggerProvider().CreateLogger("imageRender")))
            {
                ir.DrawBackground = true;

                _ = await ir.Render(this.SelectedTextSize,
                    this.UseTextfield ? [this.FreeText?.Replace("\r", "")] : typeof(MainWindowViewModel).GetProperties().Where(p => p.Name.StartsWith("Line") && p.CanWrite).Select(x => x.GetValue(this)?.ToString()),
                    this.DrawEmpfaenger);
                this.RenderedImage = await ir.SaveAsAvaloniaImage();
            }
        }

        public void RefreshOptionDisplay()
        {
            if (Globals.UserConfig != null)
            {
                this.LabelHeight = Globals.UserConfig.RuntimeConfiguration.LabelHeight;
                this.LabelWidth = Globals.UserConfig.RuntimeConfiguration.LabelWidth;
                this.Printername = Globals.UserConfig.RuntimeConfiguration.PrinterName;
            }

            this.GroupBoxLabelText = $"Label {this.LabelWidth}x{this.LabelHeight}mm";
            Dispatcher.UIThread.Invoke(() => this.RenderImage());

            this.logger?.LogInformation("[{Name}] Options refreshed", "MainWindowViewModel");
        }

        [RelayCommand]
        private async Task Print()
        {
            this.logger?.LogInformation("[{Name}] Printing...", "MainWindowViewModel");
            Stopwatch sw = Stopwatch.StartNew();

            this.IsBusy = true;
            this.currentStatus = "Printing...";
            this.textWaitingAnimation.AnimationType = TextWaitingAnimation.AnimationTypes.ClockCircle;

            await Printing.Print((float)this.LabelWidth, (float)this.LabelHeight, this.RenderedImage, this.PrintCount);

            this.textWaitingAnimation.AnimationType = TextWaitingAnimation.AnimationTypes.BlockChars;
            this.currentStatus = "System ready";
            this.IsBusy = false;

            sw.Stop();
            this.logger?.LogInformation("[{Name}] Printing finished in {Elapsed}ms", "MainWindowViewModel", sw.ElapsedMilliseconds);
        }

        [RelayCommand]
        private void ClearAll()
        {
            if (this.UseTextfield)
            {
                this.FreeText = null;
                this.logger?.LogInformation("[{Name}] Freetext textfield cleared", "MainWindowViewModel");
                return;
            }

            foreach (PropertyInfo p in typeof(MainWindowViewModel).GetProperties().Where(p => p.Name.StartsWith("Line") && p.CanWrite))
            {
                p.SetValue(this, string.Empty);
            }

            this.logger?.LogInformation("[{Name}] Line textboxes cleared", "MainWindowViewModel");
        }

        [RelayCommand]
        private void ShowOptions()
        {
            Options options = new();
            options.ShowDialog(this.Instance);
        }
    }
}
