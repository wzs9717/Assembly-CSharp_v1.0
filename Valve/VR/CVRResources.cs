using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRResources
	{
		private IVRResources FnTable;

		internal CVRResources(IntPtr pInterface)
		{
			FnTable = (IVRResources)Marshal.PtrToStructure(pInterface, typeof(IVRResources));
		}

		public uint LoadSharedResource(string pchResourceName, string pchBuffer, uint unBufferLen)
		{
			return FnTable.LoadSharedResource(pchResourceName, pchBuffer, unBufferLen);
		}

		public uint GetResourceFullPath(string pchResourceName, string pchResourceTypeDirectory, string pchPathBuffer, uint unBufferLen)
		{
			return FnTable.GetResourceFullPath(pchResourceName, pchResourceTypeDirectory, pchPathBuffer, unBufferLen);
		}
	}
}
