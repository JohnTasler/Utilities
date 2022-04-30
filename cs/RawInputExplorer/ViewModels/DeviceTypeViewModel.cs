using System.Collections.ObjectModel;
using System.Windows.Data;
using Tasler.ComponentModel;
using Tasler.Interop.RawInput;
using Tasler.Windows.ComponentModel;

namespace RawInputExplorer.ViewModels
{
	public class DeviceTypeViewModel
		: TreeNodeViewModel<DeviceListViewModel, ObservableCollection<DeviceViewModelBase>, CollectionView>
		, IModelContainer<CollectionViewGroup>
	{
		private DeviceTypeViewModel(CollectionViewGroup model)
		{
			this.Model = model;
		}

		internal static DeviceTypeViewModel CreateInstance(CollectionViewGroup model, DeviceListViewModel parent)
		{
			var newInstance = new DeviceTypeViewModel(model);
			newInstance.SetParent(parent);
			return newInstance;
		}

		#region IModelContainer<CollectionViewGroup> Members

		public CollectionViewGroup Model { get; private set; }

		#endregion IModelContainer<CollectionViewGroup> Members

		protected override ObservableCollection<DeviceViewModelBase> CreateChildCollection()
		{
			var childCollection =
				this.Model.Items.CreateTranslatingCollection<ReadOnlyObservableCollection<object>, InterfaceDevice, DeviceViewModelBase>(
					si => DeviceViewModelBase.CreateInstance(si, this));

			return childCollection;
		}

		protected override CollectionView CreateCollectionView(ObservableCollection<DeviceViewModelBase> childCollection)
		{
			return new CollectionView(childCollection);
		}
	}
}
