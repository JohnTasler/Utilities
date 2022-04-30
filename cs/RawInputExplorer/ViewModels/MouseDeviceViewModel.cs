using Tasler.Interop.RawInput;

namespace RawInputExplorer.ViewModels
{
	public class MouseDeviceViewModel : DeviceViewModelBase<InterfaceDeviceMouse>
	{
		internal MouseDeviceViewModel(InterfaceDeviceMouse model)
			: base(model)
		{
		}
	}
}
