using System;
using Tasler.ComponentModel;
using Tasler.Interop.RawInput;
using Tasler.Interop.RawInput.User;
using Tasler.Windows.ComponentModel;

namespace RawInputExplorer.ViewModels
{
	public abstract class DeviceViewModelBase
		: TreeTerminalNodeViewModel<DeviceTypeViewModel>
		, IModelContainer<InterfaceDevice>
	{
		protected DeviceViewModelBase(InterfaceDevice model)
		{
			this.Model = model;
		}

		internal static DeviceViewModelBase CreateInstance(InterfaceDevice model, DeviceTypeViewModel parent)
		{
			if (model == null)
				throw new ArgumentNullException("model");
			if (parent == null)
				throw new ArgumentNullException("parent");

			var newInstance = (DeviceViewModelBase)null;

			switch (model.DeviceType)
			{
				case InterfaceDeviceType.Mouse:
					newInstance = new MouseDeviceViewModel((InterfaceDeviceMouse)model);
					break;

				case InterfaceDeviceType.Keyboard:
					newInstance = new KeyboardDeviceViewModel((InterfaceDeviceKeyboard)model);
					break;

				case InterfaceDeviceType.HID:
					newInstance = new HumanDeviceViewModel((InterfaceDeviceHuman)model);
					break;
			}

			if (newInstance == null)
				throw new ArgumentException("~!~!~MESSAGE~!~!~", "model");

			newInstance.SetParent(parent);
			return newInstance;
		}

		public InterfaceDevice Model { get; private set; }
	}

	public abstract class DeviceViewModelBase<T> : DeviceViewModelBase
		where T : InterfaceDevice
	{
		internal DeviceViewModelBase(T model)
			: base(model)
		{
		}

		public T TypedModel
		{
			get { return (T)base.Model; }
		}
	}

}
