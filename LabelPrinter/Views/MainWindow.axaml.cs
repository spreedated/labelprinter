#pragma warning disable S1244

using Avalonia.Controls;
using Avalonia.Threading;
using LabelPrinter.ViewModels;

namespace LabelPrinter.Views
{
    public partial class MainWindow : Window
    {
        private bool isLoaded;

        public MainWindow()
        {
            this.InitializeComponent();
            ((MainWindowViewModel)this.DataContext).Instance = this;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.isLoaded && this.Width != default && this.Height != default)
            {
                Dispatcher.UIThread.Invoke(() => ((MainWindowViewModel)this.DataContext).RenderImage());
            }

        }

        private void Window_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.isLoaded = true;
            Dispatcher.UIThread.Invoke(() => ((MainWindowViewModel)this.DataContext).RenderImage());
        }
    }
}