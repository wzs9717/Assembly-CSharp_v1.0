using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRResources
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _LoadSharedResource(string pchResourceName, string pchBuffer, uint unBufferLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetResourceFullPath(string pchResourceName, string pchResourceTypeDirectory, string pchPathBuffer, uint unBufferLen);

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _LoadSharedResource LoadSharedResource;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _GetResourceFullPath GetResourceFullPath;
	}
}
