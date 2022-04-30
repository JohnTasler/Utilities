using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Tasler.ComponentModel;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AppDiagnosticInfoTestApp
{
    public enum ViewModelType
    {
        RunningApps,
        InstalledApps,
    }

    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel(ViewModelType viewModelType = ViewModelType.RunningApps)
        {
            if (viewModelType == ViewModelType.RunningApps)
            {
                var unused = StartModelAsync();
            }
            else
            {
                var unused = GetInstalledAsync();
            }

            _infos.Source = _infosSource;
            _infos.View.CurrentChanged += (s, e) =>
            {
                this.PropertyChanged.RaisePropertyChanged(this,
                    nameof(this.SelectedInfo),
                    nameof(this.HasSelectedInfo));
            };
        }

        private CoreDispatcher Dispatcher { get; } = Window.Current.Dispatcher;

        public DiagnosticAccessStatus AccessStatus { get; private set; }

        public ICollectionView Infos => _infos.View;
        private readonly CollectionViewSource _infos = new CollectionViewSource();
        private ObservableCollection<AppDiagnosticInfoViewModel> _infosSource = new ObservableCollection<AppDiagnosticInfoViewModel>();

        public int InfosCount => _infosSource.Count;

        public AppDiagnosticInfoViewModel SelectedInfo
        {
            get
            {
                return (AppDiagnosticInfoViewModel)this.Infos.CurrentItem;
            }
        }

        public bool HasSelectedInfo => this.SelectedInfo != null;

        private async Task StartModelAsync()
        {
            this.AccessStatus = await AppDiagnosticInfo.RequestAccessAsync();
            this.PropertyChanged.RaisePropertyChanged(this, nameof(this.AccessStatus));

            _infoWatcher.Added += this.InfoWatcher_Added;
            _infoWatcher.Removed += this.InfoWatcher_Removed;
            _infoWatcher.Start();
        }

        private async Task GetInstalledAsync()
        {
            this.AccessStatus = await AppDiagnosticInfo.RequestAccessAsync();
            this.PropertyChanged.RaisePropertyChanged(this, nameof(this.AccessStatus));

            foreach (var info in await AppDiagnosticInfo.RequestInfoForAppAsync())
            {
                _infosSource.Add(new AppDiagnosticInfoViewModel(info));
            }

            this.PropertyChanged.RaisePropertyChanged(this, nameof(this.InfosCount));
        }

        private void InfoWatcher_Added(AppDiagnosticInfoWatcher sender, AppDiagnosticInfoWatcherEventArgs args)
        {
            var unused = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _infosSource.Add(new AppDiagnosticInfoViewModel(args.AppDiagnosticInfo));
                this.PropertyChanged.RaisePropertyChanged(this, nameof(this.InfosCount));
            });
        }

        private void InfoWatcher_Removed(AppDiagnosticInfoWatcher sender, AppDiagnosticInfoWatcherEventArgs args)
        {
            var unused = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var removed = _infosSource.FirstOrDefault(i => i.AppInfo.AppUserModelId == args.AppDiagnosticInfo.AppInfo.AppUserModelId);
                _infosSource.Remove(removed);

                if ((removed != null) && (removed == this.SelectedInfo))
                {
                    removed.ResourceGroups.MoveCurrentToPrevious();
                    this.Infos.MoveCurrentTo(null);
                }
                this.PropertyChanged.RaisePropertyChanged(this, nameof(this.InfosCount));
            });
        }

        private AppDiagnosticInfoWatcher _infoWatcher = AppDiagnosticInfo.CreateWatcher();
    }
}
