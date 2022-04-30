using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SwitchUser
{
	class Program
	{
		#region Static Fields
		private static readonly bool IsConsoleApplication;
		#endregion Static Fields

		#region Instance Fields
		private readonly string UserName;
		private readonly string Password;
		private readonly int CurrentSessionId = GetCurrentSessionId();
		private readonly WTS_SESSION_INFO_1[] SessionList = GetSessionList();
		#endregion Instance Fields

		#region Constructors
		static Program()
		{
			// Determine if the program was compiled as a Console Application or a Windows Application
			try
			{
				Console.TreatControlCAsInput = false;
				IsConsoleApplication = true;
			}
			catch
			{
			}
		}

		private Program(string[] args)
		{
			// Parse the command line
			this.UserName = (args.Length > 0) ? args[0] : string.Empty;
			this.Password = (args.Length > 1) ? args[1] : string.Empty;

			Trace.Write(this.FormatSessionList());
		}
		#endregion Constructors

		#region Static Entry Point
		static void Main(string[] args)
		{
			try
			{
				var program = new Program(args);
				program.Run();
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			if (IsConsoleApplication && Debugger.IsAttached)
			{
				Console.WriteLine("Press any key to exit...");
				Console.ReadKey(true);
			}
		}
		#endregion Static Entry Point

		#region Instance Entry Point
		private void Run()
		{
			var currentSessions = this.SessionList.Where(s => s.SessionId == this.CurrentSessionId);
			var sessionUserName = string.Empty;
			if (currentSessions.Any())
			{
				var currentSession = currentSessions.First();
				if (string.IsNullOrWhiteSpace(currentSession.pDomainName))
					currentSession.pDomainName = Environment.MachineName;
				sessionUserName = currentSession.pDomainName + "\\" + currentSession.pUserName;
			}

			var sessionId = FindMatchingSessionId(this.UserName);
			if (sessionId < 0)
			{
				if (!WTSDisconnectSession(WTS_CURRENT_SERVER_HANDLE, WTS_CURRENT_SESSION, true))
					throw new Exception("disconnect the user " + sessionUserName, new Win32Exception());

				var disconnectedUserMessage =
					string.IsNullOrWhiteSpace(sessionUserName) ? string.Empty : (": " + sessionUserName);
				ReportLine("Disconnected user" + disconnectedUserMessage);
			}
			else
			{
				var session = SessionList.First(s => s.SessionId == sessionId);
				var domainUserName = session.pDomainName + '\\' + session.pUserName;

				if (!WTSConnectSession(sessionId, WTS_CURRENT_SESSION, this.Password, true))
					throw new Exception("switch to user " + domainUserName, new Win32Exception());

				ReportLine("Switched to user: " + domainUserName);
			}
		}
		#endregion Instance Entry Point

		private static int GetCurrentSessionId()
		{
			var currentSessionId = WTS_CURRENT_SESSION;
			IntPtr pSessionId;
			int bytesReturned;
			if (!WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, WTS_CURRENT_SESSION, WTS_INFO_CLASS.SessionId, out pSessionId, out bytesReturned))
				throw new Exception("query the current session information", new Win32Exception());
			if (bytesReturned == sizeof(int))
				currentSessionId = Marshal.ReadInt32(pSessionId);
			return currentSessionId;
		}

		private static WTS_SESSION_INFO_1[] GetSessionList()
		{
			int level = 1;
			int count;
			WTS_SESSION_INFO_1[] sessionList;
			if (!WTSEnumerateSessionsEx(WTS_CURRENT_SERVER_HANDLE, ref level, 0, out sessionList, out count))
				throw new Exception("get the session list", new Win32Exception());
			return sessionList;
		}

		private string FormatSessionList()
		{
			var sb = new StringBuilder();

			int index = 0;
			sb.AppendFormat("{0} sessions:", this.SessionList.Length).AppendLine();
			foreach (var info in this.SessionList)
			{
				sb.AppendFormat("{0}:  ExecEnvId    = {1}", ++index, info.ExecEnvId).AppendLine();
				sb.AppendFormat(  "    SessionId    = {0}", info.SessionId         ).AppendLine();
				sb.AppendFormat(  "    State        = {0}", info.State             ).AppendLine();
				sb.AppendFormat(  "    pSessionName = {0}", info.pSessionName ?? "<null>").AppendLine();
				sb.AppendFormat(  "    pHostName    = {0}", info.pHostName    ?? "<null>").AppendLine();
				sb.AppendFormat(  "    pUserName    = {0}", info.pUserName    ?? "<null>").AppendLine();
				sb.AppendFormat(  "    pDomainName  = {0}", info.pDomainName  ?? "<null>").AppendLine();
				sb.AppendFormat(  "    pFarmName    = {0}", info.pFarmName    ?? "<null>").AppendLine();
				sb.AppendLine();
			}

			return sb.ToString();
		}

		private int FindMatchingSessionId(string userName)
		{
			var matching =
				from s in this.SessionList
				where userName.Equals(s.pUserName, StringComparison.OrdinalIgnoreCase)
				   || userName.Equals(s.pDomainName + "\\" + s.pUserName, StringComparison.OrdinalIgnoreCase)
				select s;

			return matching.Any() ? matching.First().SessionId : -1;
		}

		private static void ReportText(string text)
		{
			if (IsConsoleApplication)
				Console.Write(text);
			else
				Trace.Write(text);
		}

		private static void ReportLine(string text)
		{
			if (IsConsoleApplication)
				Console.WriteLine(text);
			else
				Trace.WriteLine(text);
		}

		private static void ReportException(Exception ex)
		{
			var message = "An error occurred";
			if (ex.InnerException != null)
				message += " while attempting to " + ex.Message + "." + Environment.NewLine + Environment.NewLine + ex.InnerException.Message;
			else
				message += "." + Environment.NewLine + Environment.NewLine + ex.Message;

			Trace.WriteLine(message + Environment.NewLine + ex);

			if (IsConsoleApplication)
			{
				Console.Write(message);
			}
			else
			{
				message += Environment.NewLine + Environment.NewLine + "You may press the Ctrl+C key combination to copy this message to the clipboard.";
				message += " This would allow you, for example, to paste this error message into an email.";
				MessageBox(IntPtr.Zero, message, "Error - Switch User",
					MessageBoxFlags.OK | MessageBoxFlags.ICONHAND | MessageBoxFlags.SETFOREGROUND);
			}
		}

		#region Platform Invoke

		#region Constants and Static Fields
		private const int WTS_CURRENT_SESSION = -1;
		private static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
		#endregion Constants and Static Fields

		#region Enumerations

		private enum WTS_CONNECTSTATE_CLASS
		{
			Active,              // User logged on to WinStation
			Connected,           // WinStation connected to client
			ConnectQuery,        // In the process of connecting to client
			Shadow,              // Shadowing another WinStation
			Disconnected,        // WinStation logged on without client
			Idle,                // Waiting for client to connect
			Listen,              // WinStation is listening for connection
			Reset,               // WinStation is being reset
			Down,                // WinStation is down due to error
			Init,                // WinStation in initialization
		}

		enum WTS_INFO_CLASS
		{
			//InitialProgram = 0,
			//ApplicationName = 1,
			//WorkingDirectory = 2,
			//OEMId = 3,
			SessionId = 4,
			//UserName = 5,
			//WinStationName = 6,
			//DomainName = 7,
			//ConnectState = 8,
			//ClientBuildNumber = 9,
			//ClientName = 10,
			//ClientDirectory = 11,
			//ClientProductId = 12,
			//ClientHardwareId = 13,
			//ClientAddress = 14,
			//ClientDisplay = 15,
			//ClientProtocolType = 16,
			//IdleTime = 17,
			//LogonTime = 18,
			//IncomingBytes = 19,
			//OutgoingBytes = 20,
			//IncomingFrames = 21,
			//OutgoingFrames = 22,
			//ClientInfo = 23,
			//SessionInfo = 24,
			//SessionInfoEx = 25,
			//ConfigInfo = 26,
			//ValidationInfo = 27,
			//SessionAddressV4 = 28,
			//IsRemoteSession = 29,
		}

		enum LogonType
		{
			INTERACTIVE       = 2,
			NETWORK           = 3,
			BATCH             = 4,
			SERVICE           = 5,
			UNLOCK            = 7,
			NETWORK_CLEARTEXT = 8,
			NEW_CREDENTIALS   = 9,
		}

		enum LogonProvider
		{
			DEFAULT = 0,
			WINNT35 = 1,
			WINNT40 = 2,
			WINNT50 = 3,
			VIRTUAL = 4,
		}

		[Flags]
		enum MessageBoxFlags : uint
		{
			OK                   = 0x00000000,
			//OKCANCEL             = 0x00000001,
			//ABORTRETRYIGNORE     = 0x00000002,
			//YESNOCANCEL          = 0x00000003,
			//YESNO                = 0x00000004,
			//RETRYCANCEL          = 0x00000005,
			//CANCELTRYCONTINUE    = 0x00000006,

			ICONHAND             = 0x00000010,
			//ICONQUESTION         = 0x00000020,
			//ICONEXCLAMATION      = 0x00000030,
			//ICONASTERISK         = 0x00000040,

			//USERICON             = 0x00000080,
			//ICONWARNING          = ICONEXCLAMATION,
			//ICONERROR            = ICONHAND,

			//ICONINFORMATION      = ICONASTERISK,
			//ICONSTOP             = ICONHAND,

			//DEFBUTTON1           = 0x00000000,
			//DEFBUTTON2           = 0x00000100,
			//DEFBUTTON3           = 0x00000200,
			//DEFBUTTON4           = 0x00000300,

			//APPLMODAL            = 0x00000000,
			//SYSTEMMODAL          = 0x00001000,
			//TASKMODAL            = 0x00002000,
			//HELP                 = 0x00004000,

			//NOFOCUS              = 0x00008000,
			SETFOREGROUND        = 0x00010000,
			//DEFAULT_DESKTOP_ONLY = 0x00020000,

			//TOPMOST              = 0x00040000,
			//RIGHT                = 0x00080000,
			//RTLREADING           = 0x00100000,

			//SERVICE_NOTIFICATION = 0x00200000,

			//TYPEMASK             = 0x0000000F,
			//ICONMASK             = 0x000000F0,
			//DEFMASK              = 0x00000F00,
			//MODEMASK             = 0x00003000,
			//MISCMASK             = 0x0000C000,
		}

		#endregion Enumerations

		#region Structures
		private struct WTS_SESSION_INFO_1
		{
			public int ExecEnvId;
			public WTS_CONNECTSTATE_CLASS State;
			public int SessionId;
			[MarshalAs(UnmanagedType.LPWStr)] public string pSessionName;
			[MarshalAs(UnmanagedType.LPWStr)] public string pHostName;
			[MarshalAs(UnmanagedType.LPWStr)] public string pUserName;
			[MarshalAs(UnmanagedType.LPWStr)] public string pDomainName;
			[MarshalAs(UnmanagedType.LPWStr)] public string pFarmName;
		}
		#endregion Structures

		#region External Functions

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("Wtsapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern bool WTSQuerySessionInformation(
			IntPtr hServer,
			int SessionId,
			WTS_INFO_CLASS infoClass,
			out IntPtr ppBuffer,
			out int pBytesReturned);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("Wtsapi32.dll", SetLastError=true, CharSet = CharSet.Auto)]
		private static extern bool WTSEnumerateSessionsEx(
			IntPtr hServer,
			ref int pLevel,
			int Filter,
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] out WTS_SESSION_INFO_1[] infos,
			out int pCount);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("Wtsapi32.dll", SetLastError=true, CharSet = CharSet.Auto)]
		private static extern bool WTSConnectSession(
			int LogonId,
			int TargetLogonId,
			[MarshalAs(UnmanagedType.LPWStr)] string pPassword,
			[MarshalAs(UnmanagedType.Bool)] bool bWait);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("Wtsapi32.dll", SetLastError=true, ExactSpelling=true)]
		private static extern bool WTSDisconnectSession(
			IntPtr hServer,
			int SessionId,
			[MarshalAs(UnmanagedType.Bool)] bool bWait);

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern int MessageBox(
			IntPtr hWnd,
			[MarshalAs(UnmanagedType.LPWStr)] string lpText,
			[MarshalAs(UnmanagedType.LPWStr)] string lpCaption,
			MessageBoxFlags uType);

		#endregion External Functions

		#endregion Platform Invoke
	}
}
