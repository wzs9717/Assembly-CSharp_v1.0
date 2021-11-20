using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRNotifications
	{
		private IVRNotifications FnTable;

		internal CVRNotifications(IntPtr pInterface)
		{
			FnTable = (IVRNotifications)Marshal.PtrToStructure(pInterface, typeof(IVRNotifications));
		}

		public EVRNotificationError CreateNotification(ulong ulOverlayHandle, ulong ulUserValue, EVRNotificationType type, string pchText, EVRNotificationStyle style, ref NotificationBitmap_t pImage, ref uint pNotificationId)
		{
			pNotificationId = 0u;
			return FnTable.CreateNotification(ulOverlayHandle, ulUserValue, type, pchText, style, ref pImage, ref pNotificationId);
		}

		public EVRNotificationError RemoveNotification(uint notificationId)
		{
			return FnTable.RemoveNotification(notificationId);
		}
	}
}
