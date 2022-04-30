using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Tasler.Interop;
using Tasler.Interop.RawInput;
using Tasler.Interop.RawInput.User;
using Tasler.Interop.User;
using Tasler.Windows.Input;
using Tasler.Windows.Interop;

namespace RawInputExplorer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		#region Instance Fields
		private HwndSourceMessageSubclass windowHook;
		private Dictionary<IntPtr, ObservableCollection<RawInputBase>> inputListMap;
		private bool includeMouseDevices = false;
		private bool includeKeyboardDevices = true;
		private bool includeHumanInputDevices = true;
		private bool passToDefaultHandler = false;
		#endregion Instance Fields

		#region Construction
		public MainWindow()
		{
			this.InitializeComponent();
			this.windowHook = new HwndSourceMessageSubclass(this);
			this.windowHook.HandleChanged += this.windowHook_HandleChanged;
			this.windowHook[WM.INPUT] += this.WindowHook_WM_INPUT;
			this.deviceListBox.ItemsSource = InterfaceDeviceCollection.Instance;
			this.deviceListBox.Items.Filter = this.DeviceFilter;

			this.inputListMap = new Dictionary<IntPtr, ObservableCollection<RawInputBase>>(InterfaceDeviceCollection.Instance.Count);
			foreach (InterfaceDevice device in InterfaceDeviceCollection.Instance)
				this.inputListMap.Add(device.Handle, new ObservableCollection<RawInputBase>());
		}
		#endregion Construction

		private bool DeviceFilter(object device)
		{
			return device is InterfaceDevice && this.DeviceFilter((InterfaceDevice)device);
		}

		private bool DeviceFilter(InterfaceDevice device)
		{
			if (device.DeviceType == InterfaceDeviceType.Mouse)
				return this.includeMouseDevices;
			else if (device.DeviceType == InterfaceDeviceType.Keyboard)
				return this.includeKeyboardDevices;
			else if (device.DeviceType == InterfaceDeviceType.HID)
				return this.includeHumanInputDevices;
			else
				return true;
		}

		#region Event Handlers
		private void windowHook_HandleChanged(object sender, EventArgs e)
		{
			var devices = new List<RAWINPUTDEVICE>(InterfaceDeviceCollection.Instance.Count);
			foreach (var device in InterfaceDeviceCollection.Instance.Where(this.DeviceFilter))
			{
				var inputDevice = new RAWINPUTDEVICE()
				{
					Usage = device.Usage,
					UsagePage = device.UsagePage,
					WindowHandle = this.windowHook.Handle,
					Flags = RegistrationFlags.InputSink,
				};

				if (device.DeviceType == InterfaceDeviceType.Keyboard)
					inputDevice.Flags |= RegistrationFlags.NoLegacy | RegistrationFlags.AppKeys;

				devices.Add(inputDevice);
			}

			RawInputApi.RegisterRawInputDevices(devices.ToArray(), devices.Count, Marshal.SizeOf(typeof(RAWINPUTDEVICE)));
		}

		private void WindowHook_WM_INPUT(object sender, WindowMessageEventArgs args)
		{
            Debug.Assert(args.Message == WM.INPUT);
			RAWINPUTHEADER header;
			var input = RawInputBase.FromHandle(args.LParam, out header);
			if (input != null)
			{
				ObservableCollection<RawInputBase> inputList;
				if (this.inputListMap.TryGetValue(header.Device, out inputList))
					inputList.Add(input);
			}

			// Allow default processing
            args.Handled= !passToDefaultHandler;
		}

		private void deviceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.messageItems.ItemsSource = null;
			var device = this.deviceListBox.SelectedItem as InterfaceDevice;
			if (device != null)
			{
				this.messageItems.DataContext = device;
				this.messageItems.ItemsSource = this.inputListMap[device.Handle];
			}
		}
		#endregion Event Handlers
	}

	public class InputMessage
	{

	}

}
