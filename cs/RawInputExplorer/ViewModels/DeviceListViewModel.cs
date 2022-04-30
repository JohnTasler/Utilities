using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Tasler.ComponentModel;
using Tasler.Interop.RawInput;
using Tasler.Interop.RawInput.User;
using Tasler.Windows.ComponentModel;
using Tasler.Windows.Input;

namespace RawInputExplorer.ViewModels
{
	[Export]
	public class DeviceListViewModel
		: TreeRootNodeViewModelBase<ObservableCollection<DeviceTypeViewModel>, ListCollectionView>
	{
		public DeviceListViewModel()
		{
			this.Devices = new ListCollectionView(InterfaceDeviceCollection.Instance);
			this.Devices.GroupDescriptions.Add(new PropertyGroupDescription("DeviceType"));
			this.Devices.Filter = this.GetCurrentFilterPredicate();
			this.Devices.SortDescriptions.Add(new SortDescription("DeviceType", ListSortDirection.Ascending));
		}

		public ICollectionView Devices { get; private set; }

		public bool IsIncludingMice
		{
			get { return this.isIncludingMice; }
			set
			{
				if (this.SetProperty(ref this.isIncludingMice, value, () => IsIncludingMice))
					this.Devices.Filter = this.GetCurrentFilterPredicate();
			}
		}
		private bool isIncludingMice = true;

		public bool IsIncludingKeyboards
		{
			get { return this.isIncludingKeyboards; }
			set
			{
				if (this.SetProperty(ref this.isIncludingKeyboards, value, () => IsIncludingKeyboards))
					this.Devices.Filter = this.GetCurrentFilterPredicate();
			}
		}
		private bool isIncludingKeyboards = true;

		public bool IsIncludingHIDs
		{
			get { return this.isIncludingHIDs; }
			set
			{
				if (this.SetProperty(ref this.isIncludingHIDs, value, () => IsIncludingHIDs))
					this.Devices.Filter = this.GetCurrentFilterPredicate();
			}
		}
		private bool isIncludingHIDs = true;

		private Predicate<object> GetCurrentFilterPredicate()
		{
			if (!IsIncludingMice || !IsIncludingKeyboards || !IsIncludingHIDs)
				return this.IsDeviceIncluded;

			return null;
		}

		private bool IsDeviceIncluded(object item)
		{
			var device = (InterfaceDevice)item;

			switch (device.DeviceType)
			{
				case InterfaceDeviceType.Mouse:
					return this.IsIncludingMice;

				case InterfaceDeviceType.Keyboard:
					return this.IsIncludingKeyboards;

				case InterfaceDeviceType.HID:
					return this.IsIncludingHIDs;

			}

			return true;
		}

		#region Overrides
		protected override ObservableCollection<DeviceTypeViewModel> CreateChildCollection()
		{
			var childCollection = this.Devices.Groups.CreateTranslatingCollection<ReadOnlyObservableCollection<object>, CollectionViewGroup, DeviceTypeViewModel>(
				si => DeviceTypeViewModel.CreateInstance(si, this));

			return childCollection;
		}

		protected override ListCollectionView CreateCollectionView(ObservableCollection<DeviceTypeViewModel> childCollection)
		{
			return new ListCollectionView(childCollection);
		}
		#endregion Overrides
	}
}
