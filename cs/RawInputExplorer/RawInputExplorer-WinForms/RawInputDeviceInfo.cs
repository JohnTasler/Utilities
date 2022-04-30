using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tasler.Win32.RawInput;

namespace Tasler.Windows.Input 
{
	public abstract class RawInputDeviceInfo 
	{
		#region Instance Fields
		private string name;
		internal NativeMethods.RID_DEVICE_INFO info;
		#endregion

		#region Construction
		internal RawInputDeviceInfo(NativeMethods.RAWINPUTDEVICELIST device) 
		{
			Debug.Assert(device.hDevice != IntPtr.Zero);

			// Get the number of characters needed for the device name
			uint sizeInCharacters = 0;
			UnsafeNativeMethods.GetRawInputDeviceInfo(device.hDevice, NativeMethods.RIDI_DEVICENAME,
				IntPtr.Zero, ref sizeInCharacters);

			// Allocate a chunk of memory to contain the name
			IntPtr pName = Marshal.AllocHGlobal(((int)sizeInCharacters + 1) * 2);

			// Get the name
			UnsafeNativeMethods.GetRawInputDeviceInfo(device.hDevice, NativeMethods.RIDI_DEVICENAME,
				pName, ref sizeInCharacters);

			// Save the name as a string
			this.name = Marshal.PtrToStringAuto(pName);

			// Free the chunk of memory
			Marshal.FreeHGlobal(pName);


			// Get the RID_DEVICE_INFO
			uint sizeInBytes = this.info.cbSize = (uint)Marshal.SizeOf(this.info);
			UnsafeNativeMethods.GetRawInputDeviceInfo(device.hDevice, NativeMethods.RIDI_DEVICEINFO,
				ref this.info, ref sizeInBytes);
		}
		#endregion

		#region Properties
		public string Name 
		{
			get { return this.name; }
		}

		public RawInputType DeviceType 
		{
			get { return this.info.type; }
		}
		#endregion
	}

	public sealed class RawInputMouseDeviceInfo : RawInputDeviceInfo 
	{
		#region Construction
		internal RawInputMouseDeviceInfo(NativeMethods.RAWINPUTDEVICELIST device)
			: base(device)
		{
			Debug.Assert(device.type == RawInputType.Mouse);
		}
		#endregion

		#region Properties
		public uint Id
		{
			get { return base.info.mouse.dwId; }
		}

		public uint NumberOfButtons
		{
			get { return base.info.mouse.dwNumberOfButtons; }
		}

		public uint SampleRate
		{
			get { return base.info.mouse.dwSampleRate; }
		}

		public bool HasHorizontalWheel
		{
			get { return base.info.mouse.fHasHorizontalWheel != 0; }
		}
		#endregion
	}

	public sealed class RawInputKeyboardDeviceInfo : RawInputDeviceInfo 
	{
		#region Construction
		internal RawInputKeyboardDeviceInfo(NativeMethods.RAWINPUTDEVICELIST device)
			: base(device) 
		{
			Debug.Assert(device.type == RawInputType.Keyboard);
		}
		#endregion

		#region Properties

		public uint Type 
		{
			get { return base.info.keyboard.dwType; }
		}

		public uint SubType 
		{
			get { return base.info.keyboard.dwSubType; }
		}

		public uint KeyboardMode 
		{
			get { return base.info.keyboard.dwKeyboardMode; }
		}

		public uint NumberOfFunctionKeys 
		{
			get { return base.info.keyboard.dwNumberOfFunctionKeys; }
		}

		public uint NumberOfIndicators 
		{
			get { return base.info.keyboard.dwNumberOfIndicators; }
		}

		public uint NumberOfKeysTotal 
		{
			get { return base.info.keyboard.dwNumberOfKeysTotal; }
		}

		#endregion
	}

	public sealed class RawInputHumanInterfaceDeviceInfo : RawInputDeviceInfo 
	{
		#region Construction
		internal RawInputHumanInterfaceDeviceInfo(NativeMethods.RAWINPUTDEVICELIST device)
			: base(device) 
		{
			Debug.Assert(device.type == RawInputType.HID);
		}
		#endregion

		#region Properties
		public uint VendorId 
		{
			get { return base.info.hid.dwVendorId; }
		}

		public uint ProductId 
		{
			get { return base.info.hid.dwProductId; }
		}

		public uint VersionNumber 
		{
			get { return base.info.hid.dwVersionNumber; }
		}

		public HIDUsagePage UsagePage 
		{
			get { return base.info.hid.usagePage; }
		}

		public HIDUsage Usage 
		{
			get { return base.info.hid.usage; }
		}

		#endregion
	}

}

