using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Tasler.ComponentModel;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml.Data;

namespace AppDiagnosticInfoTestApp
{
    public class AppDiagnosticInfoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AppDiagnosticInfoViewModel(AppDiagnosticInfo model)
        {
            this.Model = model;
            _resourceGroupsSource = model.GetResourceGroups().Select(g => new AppResourceGroupInfoViewModel(g)).ToList();

            this._resourceGroupsViewSource.Source = _resourceGroupsSource;
            this.ResourceGroups.CurrentChanged += (s, e) =>
            {
                if (this.ResourceGroups.CurrentPosition < 0)
                {
                    this.ResourceGroups.MoveCurrentToFirst();
                }

                this.PropertyChanged.RaisePropertyChanged(this,
                    nameof(this.SelectedResourceGroup),
                    nameof(this.HasSelectedResourceGroup));
            };
        }

        public AppDiagnosticInfo Model { get; }

        public AppInfo AppInfo => this.Model.AppInfo;

        public ICollectionView ResourceGroups => _resourceGroupsViewSource.View;
        private readonly List<AppResourceGroupInfoViewModel> _resourceGroupsSource;
        private readonly CollectionViewSource _resourceGroupsViewSource = new CollectionViewSource();

        public int ResourceGroupsCount => _resourceGroupsSource.Count;

        public AppResourceGroupInfoViewModel SelectedResourceGroup => (AppResourceGroupInfoViewModel)this.ResourceGroups.CurrentItem;

        public bool HasSelectedResourceGroup => this.ResourceGroups.CurrentPosition >= 0;
    }
}
