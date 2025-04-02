using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabelPrinter.Logic;
using LabelPrinter.Views;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace LabelPrinter.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly TextWaitingAnimation textWaitingAnimation;

        [ObservableProperty]
        private Window instance;

        [ObservableProperty]
        private string status;

        [ObservableProperty]
        private string appTitle;

        [ObservableProperty]
        private string appVersion;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private BindingList<int> availableTextSizes = new([16, 18, 20, 22, 24]);

        [ObservableProperty]
        private int selectedTextSize;

        partial void OnSelectedTextSizeChanged(int value)
        {
            Dispatcher.UIThread.Invoke(async () => await this.RenderImage());
        }

        [ObservableProperty]
        private string line1;

        [ObservableProperty]
        private string line2;

        [ObservableProperty]
        private string line3;

        [ObservableProperty]
        private string line4;

        [ObservableProperty]
        private Bitmap renderedImage;

        public MainWindowViewModel()
        {
            this.AppTitle = Globals.Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
            this.AppVersion = $"v{Globals.Assembly.GetName().Version}";
            this.SelectedTextSize = this.AvailableTextSizes[^1];
            this.Title = $"{this.appTitle} {this.AppVersion}";

            this.textWaitingAnimation = new()
            {
                UseBrackets = true,
                AnimationType = TextWaitingAnimation.AnimationTypes.BlockChars,
                Interval = 400
            };
            this.textWaitingAnimation.AnimationChanged += this.TextWaitingAnimation_AnimationChanged;
            this.textWaitingAnimation.Start();
        }

        private void TextWaitingAnimation_AnimationChanged(object sender, string e)
        {
            this.Status = $"{e} Ready";
        }

        public async Task RenderImage()
        {
            if (this.Instance == null)
            {
                return;
            }

            using (ImageRender ir = new(337, 227))
            {
                ir.DrawBackground = true;

                _ = await ir.Render(this.SelectedTextSize);
                this.RenderedImage = await ir.SaveAsAvaloniaImage();
            }
        }

        [RelayCommand]
        private async Task Print()
        {
            await this.RenderImage();
        }
    }
}
