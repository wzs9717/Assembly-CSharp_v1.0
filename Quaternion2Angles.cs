using UnityEngine;

public class Quaternion2Angles
{
	public enum RotationOrder
	{
		XYZ,
		XZY,
		YXZ,
		YZX,
		ZXY,
		ZYX
	}

	public static Vector3 GetAngles(Quaternion q, RotationOrder ro)
	{
		float w = q.w;
		float num;
		float num2;
		float num3;
		float num4;
		switch (ro)
		{
		case RotationOrder.XYZ:
			num = 1f;
			num2 = q.x;
			num3 = q.y;
			num4 = q.z;
			break;
		case RotationOrder.XZY:
			num = -1f;
			num2 = q.x;
			num3 = q.z;
			num4 = q.y;
			break;
		case RotationOrder.YXZ:
			num = -1f;
			num2 = q.y;
			num3 = q.x;
			num4 = q.z;
			break;
		case RotationOrder.YZX:
			num = 1f;
			num2 = q.y;
			num3 = q.z;
			num4 = q.x;
			break;
		case RotationOrder.ZXY:
			num = 1f;
			num2 = q.z;
			num3 = q.x;
			num4 = q.y;
			break;
		case RotationOrder.ZYX:
			num = -1f;
			num2 = q.z;
			num3 = q.y;
			num4 = q.x;
			break;
		default:
			num = 1f;
			num2 = q.x;
			num3 = q.y;
			num4 = q.z;
			break;
		}
		float num5 = num2 * num2;
		float num6 = num3 * num3;
		float num7 = num4 * num4;
		float num8 = Mathf.Atan2(2f * (w * num2 - num * num3 * num4), 1f - 2f * (num5 + num6));
		float num9 = Mathf.Asin(2f * (w * num3 + num * num2 * num4));
		float num10 = Mathf.Atan2(2f * (w * num4 - num * num2 * num3), 1f - 2f * (num6 + num7));
		Vector3 result = default(Vector3);
		switch (ro)
		{
		case RotationOrder.XYZ:
			result.x = num8;
			result.y = num9;
			result.z = num10;
			break;
		case RotationOrder.XZY:
			result.x = num8;
			result.z = num9;
			result.y = num10;
			break;
		case RotationOrder.YXZ:
			result.y = num8;
			result.x = num9;
			result.z = num10;
			break;
		case RotationOrder.YZX:
			result.y = num8;
			result.z = num9;
			result.x = num10;
			break;
		case RotationOrder.ZXY:
			result.z = num8;
			result.x = num9;
			result.y = num10;
			break;
		case RotationOrder.ZYX:
			result.z = num8;
			result.y = num9;
			result.x = num10;
			break;
		default:
			return Vector3.zero;
		}
		return result;
	}
}
