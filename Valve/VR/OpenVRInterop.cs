using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class OpenVRInterop
	{
		[DllImport("openvr_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "VR_InitInternal")]
		internal static extern uint InitInternal(ref EVRInitError peError, EVRApplicationType eApplicationType);

		[DllImport("openvr_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "VR_ShutdownInternal")]
		internal static extern void ShutdownInternal();

		[DllImport("openvr_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "VR_IsHmdPresent")]
		internal static extern bool IsHmdPresent();

		[DllImport("openvr_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "VR_IsRuntimeInstalled")]
		internal static extern bool IsRuntimeInstalled();

		[DllImport("openvr_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "VR_GetStringForHmdError")]
		internal static extern IntPtr GetStringForHmdError(EVRInitError error);

		[DllImport("openvr_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "VR_GetGenericInterface")]
		internal static extern IntPtr GetGenericInterface([In][MarshalAs(UnmanagedType.LPStr)] string pchInterfaceVersion, ref EVRInitError peError);

		[DllImport("openvr_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "VR_IsInterfaceVersionValid")]
		internal static extern bool IsInterfaceVersionValid([In][MarshalAs(UnmanagedType.LPStr)] string pchInterfaceVersion);

		[DllImport("openvr_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "VR_GetInitToken")]
		internal static extern uint GetInitToken();
	}
}
