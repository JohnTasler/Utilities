using Tasler.Interop.RawInput;

namespace RawInputExplorer.ViewModels
{
	public class HumanDeviceViewModel : DeviceViewModelBase<InterfaceDeviceHuman>
	{
		internal HumanDeviceViewModel(InterfaceDeviceHuman model)
			: base(model)
		{
		}
	}
}
