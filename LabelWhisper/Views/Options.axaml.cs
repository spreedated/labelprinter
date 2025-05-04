using Avalonia.Controls;
using LabelWhisper.ViewModels;
using neXn.Ui.Avalonia;

namespace LabelWhisper.Views;

public partial class Options : Window
{
    private readonly WindowDragHandler dragHandler;
    public Options()
    {
        this.InitializeComponent();
        this.dragHandler = new(this);
        ((OptionsViewModel)this.DataContext).Instance = this;
    }

    private void ComboBox_PointerEntered(object sender, Avalonia.Input.PointerEventArgs e)
    {
        this.dragHandler.IsEnabled = false;
    }

    private void ComboBox_PointerExited_1(object sender, Avalonia.Input.PointerEventArgs e)
    {
        this.dragHandler.IsEnabled = true;
    }
}