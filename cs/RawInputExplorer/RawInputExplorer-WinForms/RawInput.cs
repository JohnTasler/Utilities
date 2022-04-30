using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tasler.Win32.RawInput;

namespace Tasler.Windows.Input 
{
	public static class RawInput
	{
		private static NativeMethods.RAWINPUTDEVICELIST[] GetDeviceHandles() 
		{
			NativeMethods.RAWINPUTDEVICELIST[] ridl = null;
			uint numDevices = 0;
			uint sizeInBytes = (uint)Marshal.SizeOf(typeof(NativeMethods.RAWINPUTDEVICELIST));

			// Get the number of devices
			UnsafeNativeMethods.GetRawInputDeviceList(ridl, ref numDevices, sizeInBytes);

			// Allocate the array large enough for all of the devices
			ridl = new NativeMethods.RAWINPUTDEVICELIST[numDevices];

			// Get the device handles
			UnsafeNativeMethods.GetRawInputDeviceList(ridl, ref numDevices, sizeInBytes);
			return ridl;
		}

		private static NativeMethods.RAWINPUTDEVICELIST[] GetDeviceHandles(RawInputType typeFilter) 
		{
			// Count the number of matching device types
			NativeMethods.RAWINPUTDEVICELIST[] deviceHandles = GetDeviceHandles();
			int count = 0;
			foreach (NativeMethods.RAWINPUTDEVICELIST deviceHandle in deviceHandles)
			{
				if (deviceHandle.type == typeFilter)
				{
					++count;
				}
			}

			// Allocate the result array
			NativeMethods.RAWINPUTDEVICELIST[] filteredDeviceHandles = new NativeMethods.RAWINPUTDEVICELIST[count];
			count = 0;
			foreach (NativeMethods.RAWINPUTDEVICELIST deviceHandle in deviceHandles)
			{
				if (deviceHandle.type == typeFilter)
				{
					filteredDeviceHandles[count] = deviceHandle;
					++count;
				}
			}

			// Return the filtered arary
			return filteredDeviceHandles;
		}

		public static RawInputDeviceInfo[] Devices 
		{
			get 
			{
				NativeMethods.RAWINPUTDEVICELIST[] deviceHandles = GetDeviceHandles();
				RawInputDeviceInfo[] deviceInfos = new RawInputDeviceInfo[deviceHandles.Length];
				for (int i = 0; i < deviceHandles.Length; ++i)
				{
					switch (deviceHandles[i].type)
					{
						case RawInputType.Mouse:
							deviceInfos[i] = new RawInputMouseDeviceInfo(deviceHandles[i]);
							break;
						case RawInputType.Keyboard:
							deviceInfos[i] = new RawInputKeyboardDeviceInfo(deviceHandles[i]);
							break;
						case RawInputType.HID:
							deviceInfos[i] = new RawInputHumanInterfaceDeviceInfo(deviceHandles[i]);
							break;
						default:
							Debug.Fail("Unknown RawInput device type: " + (int)deviceHandles[i].type);
							break;
					}
				}

				return deviceInfos;
			}
		}

		public static RawInputMouseDeviceInfo[] MouseDevices 
		{
			get 
			{
				NativeMethods.RAWINPUTDEVICELIST[] deviceHandles = GetDeviceHandles(RawInputType.Mouse);
				RawInputMouseDeviceInfo[] deviceInfos = new RawInputMouseDeviceInfo[deviceHandles.Length];
				for (int i = 0; i < deviceHandles.Length; ++i)
				{
					deviceInfos[i] = new RawInputMouseDeviceInfo(deviceHandles[i]);
				}

				return deviceInfos;
			}
		}

		public static RawInputKeyboardDeviceInfo[] KeyboardDevices 
		{
			get 
			{
				NativeMethods.RAWINPUTDEVICELIST[] deviceHandles = GetDeviceHandles(RawInputType.Keyboard);
				RawInputKeyboardDeviceInfo[] deviceInfos = new RawInputKeyboardDeviceInfo[deviceHandles.Length];
				for (int i = 0; i < deviceHandles.Length; ++i)
				{
					deviceInfos[i] = new RawInputKeyboardDeviceInfo(deviceHandles[i]);
				}

				return deviceInfos;
			}
		}

		public static RawInputHumanInterfaceDeviceInfo[] HumanInterfaceDevices 
		{
			get 
			{
				NativeMethods.RAWINPUTDEVICELIST[] deviceHandles = GetDeviceHandles(RawInputType.HID);
				RawInputHumanInterfaceDeviceInfo[] deviceInfos = new RawInputHumanInterfaceDeviceInfo[deviceHandles.Length];
				for (int i = 0; i < deviceHandles.Length; ++i)
				{
					deviceInfos[i] = new RawInputHumanInterfaceDeviceInfo(deviceHandles[i]);
				}

				return deviceInfos;
			}
		}
	}
}
