using System;
using UnityEngine;

public abstract class SphereUtility
{
	public static float RadiusAtHeight(float yPos)
	{
		return Mathf.Abs(Mathf.Cos(Mathf.Asin(yPos)));
	}

	public static Vector3 SphericalToPoint(float yPosition, float radAngle)
	{
		float num = RadiusAtHeight(yPosition);
		return new Vector3(num * Mathf.Cos(radAngle), yPosition, num * Mathf.Sin(radAngle));
	}

	public static float RadAngleToPercent(float radAngle)
	{
		return radAngle / ((float)Math.PI * 2f);
	}

	public static float PercentToRadAngle(float percent)
	{
		return percent * ((float)Math.PI * 2f);
	}

	public static float HeightToPercent(float yValue)
	{
		return yValue / 2f + 0.5f;
	}

	public static float PercentToHeight(float hPercent)
	{
		return Mathf.Lerp(-1f, 1f, hPercent);
	}

	public static float AngleToReachTarget(Vector2 point, float targetAngle)
	{
		float num = Atan2Positive(point.y, point.x);
		return (float)Math.PI * 2f - num + targetAngle;
	}

	public static float Atan2Positive(float y, float x)
	{
		float num = Mathf.Atan2(y, x);
		if (num < 0f)
		{
			num = (float)Math.PI + ((float)Math.PI + num);
		}
		return num;
	}

	public static Vector3 RotateAroundXAxis(Vector3 point, float angle)
	{
		Vector2 vector = Rotate2d(new Vector2(point.z, point.y), angle);
		return new Vector3(point.x, vector.y, vector.x);
	}

	public static Vector3 RotateAroundYAxis(Vector3 point, float angle)
	{
		Vector2 vector = Rotate2d(new Vector2(point.x, point.z), angle);
		return new Vector3(vector.x, point.y, vector.y);
	}

	public static Vector3 RotatePoint(Vector3 point, float xAxisRotation, float yAxisRotation)
	{
		Vector3 point2 = RotateAroundYAxis(point, yAxisRotation);
		return RotateAroundXAxis(point2, xAxisRotation);
	}

	public static Vector2 Rotate2d(Vector2 pos, float angle)
	{
		Vector4 matrix = new Vector4(Mathf.Cos(angle), 0f - Mathf.Sin(angle), Mathf.Sin(angle), Mathf.Cos(angle));
		return Matrix2x2Mult(matrix, pos);
	}

	public static Vector2 Matrix2x2Mult(Vector4 matrix, Vector2 pos)
	{
		return new Vector2(matrix[0] * pos[0] + matrix[1] * pos[1], matrix[2] * pos[0] + matrix[3] * pos[1]);
	}

	public static void CalculateStarRotation(Vector3 star, out float xRotationAngle, out float yRotationAngle)
	{
		Vector3 point = new Vector3(star.x, star.y, star.z);
		yRotationAngle = AngleToReachTarget(new Vector2(point.x, point.z), (float)Math.PI / 2f);
		point = RotateAroundYAxis(point, yRotationAngle);
		xRotationAngle = AngleToReachTarget(new Vector3(point.z, point.y), 0f);
	}
}
