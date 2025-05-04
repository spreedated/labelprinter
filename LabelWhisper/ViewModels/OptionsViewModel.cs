using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabelWhisper.Logic;
using System.Threading.Tasks;

namespace LabelWhisper.ViewModels
{
    public partial class OptionsViewModel : ObservableObject
    {
        [ObservableProperty]
        private Window instance;

        [ObservableProperty]
        private string printerName;

        [ObservableProperty]
        private int labelWidth;

        [ObservableProperty]
        private int labelHeight;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveAndExitCommand))]
        private bool isBusy;

        #region Ctor
        public OptionsViewModel()
        {
            this.LabelHeight = Globals.UserConfig.RuntimeConfiguration.LabelHeight;
            this.LabelWidth = Globals.UserConfig.RuntimeConfiguration.LabelWidth;
            this.PrinterName = Globals.UserConfig.RuntimeConfiguration.PrinterName;
        }
        #endregion

        private bool CanExecuteSaveAndExit()
        {
            return !this.IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteSaveAndExit))]
        private async Task SaveAndExit()
        {
            this.IsBusy = true;

            Globals.UserConfig.RuntimeConfiguration.LabelHeight = this.LabelHeight;
            Globals.UserConfig.RuntimeConfiguration.LabelWidth = this.LabelWidth;
            Globals.UserConfig.RuntimeConfiguration.PrinterName = this.PrinterName;
            ((MainWindowViewModel)this.Instance.Owner.DataContext).RefreshOptionDisplay();
            await Globals.UserConfig.Save();

            this.IsBusy = false;
            this.Instance.Close();
        }
    }
}
