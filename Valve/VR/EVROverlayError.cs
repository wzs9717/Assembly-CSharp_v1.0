namespace Valve.VR
{
	public enum EVROverlayError
	{
		None = 0,
		UnknownOverlay = 10,
		InvalidHandle = 11,
		PermissionDenied = 12,
		OverlayLimitExceeded = 13,
		WrongVisibilityType = 14,
		KeyTooLong = 0xF,
		NameTooLong = 0x10,
		KeyInUse = 17,
		WrongTransformType = 18,
		InvalidTrackedDevice = 19,
		InvalidParameter = 20,
		ThumbnailCantBeDestroyed = 21,
		ArrayTooSmall = 22,
		RequestFailed = 23,
		InvalidTexture = 24,
		UnableToLoadFile = 25,
		KeyboardAlreadyInUse = 26,
		NoNeighbor = 27,
		TooManyMaskPrimitives = 29,
		BadMaskPrimitive = 30
	}
}
