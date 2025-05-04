using Avalonia.Controls;
using LabelWhisper.ViewLogic;
using LabelWhisper.ViewModels;

namespace LabelWhisper.Views;

public partial class Options : Window
{
    public Options()
    {
        this.InitializeComponent();
        _ = new WindowDragHandler(this);
        ((OptionsViewModel)this.DataContext).Instance = this;
    }
}