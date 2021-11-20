using System.Runtime.InteropServices;

namespace Valve.VR
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct VRControllerState_t_Packed
	{
		public uint unPacketNum;

		public ulong ulButtonPressed;

		public ulong ulButtonTouched;

		public VRControllerAxis_t rAxis0;

		public VRControllerAxis_t rAxis1;

		public VRControllerAxis_t rAxis2;

		public VRControllerAxis_t rAxis3;

		public VRControllerAxis_t rAxis4;

		public void Unpack(ref VRControllerState_t unpacked)
		{
			unpacked.unPacketNum = unPacketNum;
			unpacked.ulButtonPressed = ulButtonPressed;
			unpacked.ulButtonTouched = ulButtonTouched;
			unpacked.rAxis0 = rAxis0;
			unpacked.rAxis1 = rAxis1;
			unpacked.rAxis2 = rAxis2;
			unpacked.rAxis3 = rAxis3;
			unpacked.rAxis4 = rAxis4;
		}
	}
}
