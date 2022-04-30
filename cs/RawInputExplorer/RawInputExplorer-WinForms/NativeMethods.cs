using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using Tasler.Windows.Input;

namespace Tasler.Win32.RawInput 
{
	internal class NativeMethods
	{
		#region Raw Input Device Information
		public const uint RIDI_PREPARSEDDATA = 0x20000005;
		public const uint RIDI_DEVICENAME    = 0x20000007;
		public const uint RIDI_DEVICEINFO    = 0x2000000b;
		#endregion

		#region Structures

		/// <summary>
		/// Value type for raw input.
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		public struct RAWINPUT 
		{
			/// <summary>Header for the data.</summary>
			[FieldOffset(0)]
			public RAWINPUTHEADER Header;
			/// <summary>Mouse raw input data.</summary>
			[FieldOffset(16)]
			public RAWINPUTMOUSE Mouse;
			/// <summary>Keyboard raw input data.</summary>
			[FieldOffset(16)]
			public RAWINPUTKEYBOARD Keyboard;
			/// <summary>HID raw input data.</summary>
			[FieldOffset(16)]
			public RAWINPUTHID HID;
		}

		/// <summary>
		/// Value type for raw input devices.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct RAWINPUTDEVICE 
		{
			/// <summary>Top level collection Usage page for the raw input device.</summary>
			public HIDUsagePage UsagePage;
			/// <summary>Top level collection Usage for the raw input device. </summary>
			public HIDUsage Usage;
			/// <summary>Mode flag that specifies how to interpret the information provided by UsagePage and Usage.</summary>
			public RawInputDeviceFlags Flags;
			/// <summary>Handle to the target device. If NULL, it follows the keyboard focus.</summary>
			public IntPtr WindowHandle;
		}

		/// <summary>
		/// Contains information about a raw input device.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct RAWINPUTDEVICELIST 
		{
			/// <summary>Handle to the raw input device.</summary>
			public IntPtr hDevice;

			/// <summary>
			/// Type of device. This can be one of the following values. 
			/// <list type="table">
			/// <item>
			///     <term><see cref="NativeMethods.RawInputType.HID"/></term>
			///     <description>The device is an Human Interface Device (HID) that is not a keyboard
			///     and not a mouse.</description>
			/// </item>
			/// <item>
			///      <term><see cref="NativeMethods.RawInputType.Keyboard"/></term>
			///      <description>The device is a keyboard.</description>
			/// </item>
			/// <item>
			///     <term><see cref="NativeMethods.RawInputType.Mouse"/></term>
			///     <description>The device is a mouse.</description>
			/// </item>
			/// </list>
			/// </summary>
			public RawInputType type;
		}


		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		public struct RID_DEVICE_INFO 
		{
			/// <summary>Size, in bytes, of the <see cref="RID_DEVICE_INFO"/> structure.</summary>
			[FieldOffset(0)]
			public uint cbSize;

			/// <summary>
			/// Type of device. This can be one of the following values. 
			/// <list type="table">
			/// <item>
			///     <term><see cref="NativeMethods.RawInputType.HID"/></term>
			///     <description>The device is an Human Interface Device (HID) that is not a keyboard
			///     and not a mouse.</description>
			/// </item>
			/// <item>
			///      <term><see cref="NativeMethods.RawInputType.Keyboard"/></term>
			///      <description>The device is a keyboard.</description>
			/// </item>
			/// <item>
			///     <term><see cref="NativeMethods.RawInputType.Mouse"/></term>
			///     <description>The device is a mouse.</description>
			/// </item>
			/// </list>
			/// </summary>
			[FieldOffset(4)]
			public RawInputType type;

			/// <summary>
			/// If dwType is <see cref="RawInputType.Mouse"/>, this is the <see cref="RID_DEVICE_INFO_MOUSE"/>
			/// structure that defines the mouse.
			/// </summary>
			[FieldOffset(8)]
			public RID_DEVICE_INFO_MOUSE mouse;

			/// <summary>
			/// If dwType is <see cref="RawInputType.Keyboard"/>, this is the
			/// <see cref="RID_DEVICE_INFO_KEYBOARD"/> structure that defines the keyboard.
			/// </summary>
			[FieldOffset(8)]
			public RID_DEVICE_INFO_KEYBOARD keyboard;

			/// <summary>
			/// If dwType is <see cref="RawInputType.HID"/>, this is the <see cref="RID_DEVICE_INFO_HID"/>
			/// structure that defines the HID device.
			/// </summary>
			[FieldOffset(8)]
			public RID_DEVICE_INFO_HID hid;
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct RID_DEVICE_INFO_MOUSE 
		{
			public uint dwId;
			public uint dwNumberOfButtons;
			public uint dwSampleRate;
			public int fHasHorizontalWheel; 
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RID_DEVICE_INFO_KEYBOARD 
		{
			public uint dwType;
			public uint dwSubType;
			public uint dwKeyboardMode;
			public uint dwNumberOfFunctionKeys;
			public uint dwNumberOfIndicators;
			public uint dwNumberOfKeysTotal;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RID_DEVICE_INFO_HID 
		{
			public uint dwVendorId;
			public uint dwProductId;
			public uint dwVersionNumber;
			public HIDUsagePage usagePage;
			public HIDUsage usage;
		}

		/// <summary>
		/// Value type for a raw input header.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct RAWINPUTHEADER 
		{
			/// <summary>Type of device the input is coming from.</summary>
			public RawInputType Type;
			/// <summary>Size of the packet of data.</summary>
			public int Size;
			/// <summary>Handle to the device sending the data.</summary>
			public IntPtr Device;
			/// <summary>wParam from the window message.</summary>
			public IntPtr wParam;
		}

		/// <summary>
		/// Value type for raw input from a HID.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct RAWINPUTHID 
		{
			/// <summary>Size of the HID data in bytes.</summary>
			public int Size;
			/// <summary>Number of HID in Data.</summary>
			public int Count;
			/// <summary>Data for the HID.</summary>
			public IntPtr Data;
		}

		/// <summary>
		/// Contains information about the state of the keyboard.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct RAWINPUTKEYBOARD 
		{
			/// <summary>Scan code from the key depression. The scan code for keyboard overrun is
			/// KEYBOARD_OVERRUN_MAKE_CODE.</summary>
			public short MakeCode;
			/// <summary>Flags for scan code information.</summary>
			public RawKeyboardFlags Flags;
			/// <summary>Reserved; must be zero.</summary>
			public ushort Reserved;
			/// <summary>VMicrosoft Windows message compatible virtual-key code.</summary>
			public ushort VirtualKey;
			/// <summary>Corresponding window message, for example <c>WM_KEYDOWN</c>, <c>WM_SYSKEYDOWN</c>,
			/// and so forth.</summary>
			public int Message;
			/// <summary>Device-specific additional information for the event.</summary>
			public int ExtraInformation;
		}

		/// <summary>
		/// Value type for raw input from a mouse.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct RAWINPUTMOUSE 
		{
			/// <summary>Flags for the event.</summary>
			public RawMouseFlags Flags;
			/// <summary>If the mouse wheel is moved, this will contain the delta amount.</summary>
			public ushort ButtonData;
			/// <summary>Flags for the event.</summary>
			public RawMouseButtons ButtonFlags;
			/// <summary>Raw button data.</summary>
			public uint RawButtons;
			/// <summary>Relative direction of motion, depending on flags.</summary>
			public int LastX;
			/// <summary>Relative direction of motion, depending on flags.</summary>
			public int LastY;
			/// <summary>Extra information.</summary>
			public uint ExtraInformation;
		}

		#endregion
	}

	internal class UnsafeNativeMethods
	{
		#region Methods

		/// <summary>
		/// Calls the default raw input procedure to provide default processing for any raw input messages that an
		/// application does not process.
		/// </summary>
		/// <param name="paRawInput">Pointer to an array of <see cref="NativeMethods.RAWINPUT"/> structures.
		/// </param>
		/// <param name="nInput">Number of <see cref="NativeMethods.RAWINPUT"/> structures pointed to by
		/// <paramref name="paRawInput"/>.</param>
		/// <param name="cbSizeHeader">Size, in bytes, of the <see cref="NativeMethods.RAWINPUTHEADER"/>
		/// structure.</param>
		/// <remarks> This function ensures that every message is processed. <see cref="DefRawInputProc"/> is called
		/// with the same parameters received by the window procedure.</remarks>
		/// <returns>If successful, the function returns S_OK (zero). Otherwise it returns an error value.
		/// </returns>
		[SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr DefRawInputProc(
			IntPtr paRawInput,
			int nInput,
			uint cbSizeHeader);

		/// <summary>
		/// Gets the raw input from the specified device.
		/// </summary>
		/// <param name="hRawInput">Handle to the <see cref="NativeMethods.RAWINPUT"/> structure. This comes
		/// from the lParam in WM_INPUT.</param>
		/// <param name="uiCommand">Command flag. This parameter can be one of the following values.
		///	<list type="table">
		///		<item>
		///			<term><see cref="RawInputCommand.Input"/></term>
		///			<description>Get the raw data from the <see cref="NativeMethods.RAWINPUT"/> structure.
		///			</description>
		///     </item>
		///		<item>
		///			<term><see cref="RawInputCommand.Header"/></term>
		///			<description>Get the header information from the <see cref="NativeMethods.RAWINPUT"/>
		///			structure.</description>
		///     </item>
		/// </list>
		/// </param>
		/// <param name="pData">Pointer to the data that comes from the <see cref="NativeMethods.RAWINPUT"/>
		/// structure. This depends on the value of <paramref name="uiCommand"/>. If <paramref name="pData"/>
		/// is <c>null</c>, the required size of the buffer is returned in <paramref name="pcbSize"/>.</param>
		/// <param name="pcbSize">Variable that specifies the size, in bytes, of the data in pData.</param>
		/// <param name="cbSizeHeader">Size, in bytes, of <see cref="NativeMethods.RAWINPUTHEADER"/>.</param>
		/// <returns><para>If pData is <c>null</c> and the function is successful, the return value is 0.
		/// If <paramref name="pData"/> is not <c>null</c> and the function is successful, the return value is
		/// the number of bytes copied into <paramref name="pData"/>.</para>
		/// <para>If there is an error, the return value is -1.</para>
		/// </returns>
		[SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetRawInputData(
			IntPtr hRawInput,
			RawInputCommand uiCommand,
			out NativeMethods.RAWINPUT pData,
			[In, Out] ref int pcbSize,
			int cbSizeHeader);

		/// <summary>
		/// Gets the raw input from the specified device.
		/// </summary>
		/// <param name="hRawInput">Handle to the <see cref="NativeMethods.RAWINPUT"/> structure. This comes
		/// from the lParam in WM_INPUT.</param>
		/// <param name="uiCommand">Command flag. This parameter can be one of the following values.
		///	<list type="table">
		///		<item>
		///			<term><see cref="RawInputCommand.Input"/></term>
		///			<description>Get the raw data from the <see cref="NativeMethods.RAWINPUT"/> structure.
		///			</description>
		///     </item>
		///		<item>
		///			<term><see cref="RawInputCommand.Header"/></term>
		///			<description>Get the header information from the <see cref="NativeMethods.RAWINPUT"/>
		///			structure.</description>
		///     </item>
		/// </list>
		/// </param>
		/// <param name="pData">Pointer to the data that comes from the <see cref="NativeMethods.RAWINPUT"/>
		/// structure. This depends on the value of <paramref name="uiCommand"/>. If <paramref name="pData"/>
		/// is <c>null</c>, the required size of the buffer is returned in <paramref name="pcbSize"/>.</param>
		/// <param name="pcbSize">Variable that specifies the size, in bytes, of the data in pData.</param>
		/// <param name="cbSizeHeader">Size, in bytes, of <see cref="NativeMethods.RAWINPUTHEADER"/>.</param>
		/// <returns><para>If pData is <c>null</c> and the function is successful, the return value is 0.
		/// If <paramref name="pData"/> is not <c>null</c> and the function is successful, the return value is
		/// the number of bytes copied into <paramref name="pData"/>.</para>
		/// <para>If there is an error, the return value is -1.</para>
		/// </returns>
		[SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetRawInputData(
			IntPtr hRawInput,
			RawInputCommand uiCommand,
			byte[] pData,
			[In, Out] ref int pcbSize,
			int cbSizeHeader);

		/// <summary>
		/// Gets information about the raw input device.
		/// </summary>
		/// <param name="hDevice">Handle to the raw input device. This comes from the lParam of the WM_INPUT message,
		/// from <see cref="NativeMethods.RAWINPUTHEADER.hDevice"/>, or from <see cref="GetRawInputDeviceList"/>.
		/// It can also be NULL if an application inserts input data, for example, by using SendInput.</param>
		/// <param name="command">Specifies what data will be returned in pData. It can be one of the following values.
		/// <list type="table">
		/// <item>
		///		<term>RIDI_PREPARSEDDATA</term>
		///		<description>pData points to the previously parsed data.</description>
		/// </item>
		/// <item>
		///		<term>RIDI_DEVICENAME</term>
		///		<description><para>pData points to a string that contains the device name.</para>
		///     <para>For this <paramref name="command"/> only, the value in <paramref name="sizeInBytes"/>
		///     is the character count (rather than the byte count).</para>
		///     </description>
		/// </item>
		/// <item>
		///		<term>RIDI_DEVICEINFO</term>
		///		<description>pData points to an <see cref="NativeMethods.RID_DEVICE_INFO"/> structure.
		///		</description>
		/// </item>
		/// </list>
		/// </param>
		/// <param name="ridInfo"></param>
		/// <param name="sizeInBytes"></param>
		/// <returns><para>If successful, this function returns a non-negative number indicating the number of
		/// bytes copied to pData.</para>
		/// <para>
		/// If pData is not large enough for the data, the function returns -1. If pData is NULL, the function
		/// returns a value of zero. In both of these cases, <paramref name="sizeInBytes"/> is set to the
		/// minimum size required for the pData buffer.</para>
		/// <para>Call <see cref="Marshal.GetLastWin32Error"/> to identify any other errors.</para>
		/// </returns>
		[SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetRawInputDeviceInfo(
			IntPtr hDevice,
			uint command,
			[In] ref NativeMethods.RID_DEVICE_INFO ridInfo,
			ref uint sizeInBytes);

		/// <summary>
		/// Gets information about the raw input device.
		/// </summary>
		/// <param name="hDevice">Handle to the raw input device. This comes from the lParam of the WM_INPUT message,
		/// from <see cref="NativeMethods.RAWINPUTHEADER.hDevice"/>, or from <see cref="GetRawInputDeviceList"/>.
		/// It can also be NULL if an application inserts input data, for example, by using SendInput.</param>
		/// <param name="command">Specifies what data will be returned in pData. It can be one of the following values.
		/// <list type="table">
		/// <item>
		///		<term>RIDI_PREPARSEDDATA</term>
		///		<description>pData points to the previously parsed data.</description>
		/// </item>
		/// <item>
		///		<term>RIDI_DEVICENAME</term>
		///		<description><para>pData points to a string that contains the device name.</para>
		///     <para>For this <paramref name="command"/> only, the value in <paramref name="sizeInBytes"/>
		///     is the character count (rather than the byte count).</para>
		///     </description>
		/// </item>
		/// <item>
		///		<term>RIDI_DEVICEINFO</term>
		///		<description>pData points to an <see cref="NativeMethods.RID_DEVICE_INFO"/> structure.
		///		</description>
		/// </item>
		/// </list>
		/// </param>
		/// <param name="pData"></param>
		/// <param name="sizeInBytes"></param>
		/// <returns><para>If successful, this function returns a non-negative number indicating the number of
		/// bytes copied to pData.</para>
		/// <para>
		/// If pData is not large enough for the data, the function returns -1. If pData is NULL, the function
		/// returns a value of zero. In both of these cases, <paramref name="sizeInBytes"/> is set to the
		/// minimum size required for the pData buffer.</para>
		/// <para>Call <see cref="Marshal.GetLastWin32Error"/> to identify any other errors.</para>
		/// </returns>
		[SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetRawInputDeviceInfo(
			IntPtr hDevice,
			uint command,
			IntPtr pData,
			ref uint sizeInBytes);

		/// <summary>
		/// Enumerates the raw input devices attached to the system.
		/// </summary>
		/// <param name="ridl">Pointer to buffer that holds an array of
		/// <see cref="NativeMethods.RAWINPUTDEVICELIST"/> structures for the devices attached to the system.
		/// If NULL, the number of devices are returned in <paramref name="numDevices"/>.</param>
		/// <param name="numDevices">If <paramref name="ridl"/> is NULL, it specifies the number of devices
		/// attached to the system. Otherwise, it contains the size, in bytes, of the preallocated buffer
		/// pointed to by <paramref name="ridl"/>. However, if <paramref name="numDevices"/> is smaller than
		/// needed to contain cref="NativeMethods.RAWINPUTDEVICELIST"/> structures, the required buffer size is
		/// returned here.</param>
		/// <param name="sizeInBytes">Size of a cref="NativeMethods.RAWINPUTDEVICELIST"/> structure.</param>
		/// <returns><para>If the function is successful, the return value is the number of devices stored in
		/// the buffer pointed to by <paramref name="ridl"/>.</para>
		/// <para>If <paramref name="ridl"/> is <c>null</c>, the return value is zero.</para>
		/// <para>If <paramref name="numDevices"/> is smaller than needed to contain all the
		/// <see cref="NativeMethods.RAWINPUTDEVICELIST"/> structures, the return value is (uint) -1 and the
		/// required buffer is returned in <paramref name="numDevices"/>. Calling
		/// <see cref="Marshal.GetLastWin32Error"/> returns <c>ERROR_INSUFFICIENT_BUFFER</c>.</para>
		/// <para>On any other error, the function returns (uint) -1 and <see cref="Marshal.GetLastWin32Error"/>
		/// returns the error indication.</para>
		/// </returns>
		[SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetRawInputDeviceList(
			[In, Out] NativeMethods.RAWINPUTDEVICELIST[] ridl,
			[In, Out] ref uint numDevices,
			uint sizeInBytes);

		/// <summary>
		/// Registers the devices that supply the raw input data.
		/// </summary>
		/// <param name="pRawInputDevices">Pointer to an array of <see cref="NativeMethods.RAWINPUTDEVICE"/>
		/// structures that represent the devices that supply the raw input.</param>
		/// <param name="uiNumDevices">Number of <see cref="NativeMethods.RAWINPUTDEVICE"/> structures pointed
		/// to by <paramref name="pRawInputDevices"/>.</param>
		/// <param name="cbSize">Size of the <see cref="NativeMethods.RAWINPUTDEVICE"/> structure.</param>
		/// <returns><c>true</c> if the function succeeds; otherwise, <c>false</c>. If the function fails, call
		/// <see cref="Marshal.GetLastWin32Error"/> for more information.</returns>
		/// <remarks>To receive <c>WM_INPUT</c> messages, an application must first register the raw input devices
		/// using <see cref="RegisterRawInputDevices"/>. By default, an application does not receive raw input.
		/// </remarks>
		[DllImport("user32.dll")]
		public static extern bool RegisterRawInputDevices(
			NativeMethods.RAWINPUTDEVICE[] pRawInputDevices,
			int uiNumDevices,
			int cbSize);

		#endregion
	}

}
