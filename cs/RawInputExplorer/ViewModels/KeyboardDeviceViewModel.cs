using Tasler.Interop.RawInput;

namespace RawInputExplorer.ViewModels
{
	public class KeyboardDeviceViewModel : DeviceViewModelBase<InterfaceDeviceKeyboard>
	{
		internal KeyboardDeviceViewModel(InterfaceDeviceKeyboard model)
			: base(model)
		{
		}
	}
}
