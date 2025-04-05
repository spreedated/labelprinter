using Avalonia.Controls;
using LabelPrinter.ViewLogic;
using LabelPrinter.ViewModels;

namespace LabelPrinter.Views;

public partial class Options : Window
{
    public Options()
    {
        this.InitializeComponent();
        _ = new WindowDragHandler(this);
        ((OptionsViewModel)this.DataContext).Instance = this;
    }
}