using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using ControlCatalog.Pages;
using MiniMvvm;

namespace ControlCatalog.ViewModels
{
    public class RefreshViewViewModel : ViewModelBase
    {
        public ObservableCollection<string> Items { get; }

        public RefreshViewViewModel()
        {
            Items = new ObservableCollection<string>(Enumerable.Range(1, 200).Select(i => $"Item {i}"));
        }

        public async Task Refresh()
        {
            Items.Clear();

            await Task.Delay(1000);

            foreach (var item in Enumerable.Range(1, 200).Select(i => $"Item {i}"))
            {
                Items.Add(item);
            }
        }
    }
}
