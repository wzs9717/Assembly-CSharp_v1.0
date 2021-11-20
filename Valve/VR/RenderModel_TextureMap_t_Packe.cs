using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RenderModel_TextureMap_t_Packed
	{
		public char unWidth;

		public char unHeight;

		public IntPtr rubTextureMapData;

		public void Unpack(ref RenderModel_TextureMap_t unpacked)
		{
			unpacked.unWidth = unWidth;
			unpacked.unHeight = unHeight;
			unpacked.rubTextureMapData = rubTextureMapData;
		}
	}
}
