using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ControlCatalog.ViewModels;

namespace ControlCatalog.Pages
{
    public class RefreshViewPage : UserControl
    {
        private RefreshViewViewModel _viewModel;

        public RefreshViewPage()
        {
            this.InitializeComponent();

            _viewModel = new RefreshViewViewModel();

            DataContext = _viewModel;
        }

        private async void RefreshViewPage_RefreshRequested(object? sender, RefreshRequestedEventArgs e)
        {
            var deferral = e.RefreshCompletionDeferral;

            await _viewModel.Refresh();

            deferral.Complete();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
