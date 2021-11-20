using System.Runtime.InteropServices;

namespace Valve.VR
{
	[StructLayout(LayoutKind.Explicit)]
	public struct VROverlayIntersectionMaskPrimitive_Data_t
	{
		[FieldOffset(0)]
		public IntersectionMaskRectangle_t m_Rectangle;

		[FieldOffset(0)]
		public IntersectionMaskCircle_t m_Circle;
	}
	public struct VROverlayIntersectionMaskPrimitive_t
	{
		public EVROverlayIntersectionMaskPrimitiveType m_nPrimitiveType;

		public VROverlayIntersectionMaskPrimitive_Data_t m_Primitive;
	}
}
