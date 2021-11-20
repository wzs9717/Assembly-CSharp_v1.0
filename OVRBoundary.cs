using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class OVRBoundary
{
	public enum Node
	{
		HandLeft = 3,
		HandRight = 4,
		Head = 9
	}

	public enum BoundaryType
	{
		OuterBoundary = 1,
		PlayArea = 0x100
	}

	public struct BoundaryTestResult
	{
		public bool IsTriggering;

		public float ClosestDistance;

		public Vector3 ClosestPoint;

		public Vector3 ClosestPointNormal;
	}

	public struct BoundaryLookAndFeel
	{
		public Color Color;
	}

	private static int cachedVector3fSize = Marshal.SizeOf(typeof(OVRPlugin.Vector3f));

	private static OVRNativeBuffer cachedGeometryNativeBuffer = new OVRNativeBuffer(0);

	private static float[] cachedGeometryManagedBuffer = new float[0];

	public bool GetConfigured()
	{
		return OVRPlugin.GetBoundaryConfigured();
	}

	public BoundaryTestResult TestNode(Node node, BoundaryType boundaryType)
	{
		OVRPlugin.BoundaryTestResult boundaryTestResult = OVRPlugin.TestBoundaryNode((OVRPlugin.Node)node, (OVRPlugin.BoundaryType)boundaryType);
		BoundaryTestResult result = default(BoundaryTestResult);
		result.IsTriggering = boundaryTestResult.IsTriggering == OVRPlugin.Bool.True;
		result.ClosestDistance = boundaryTestResult.ClosestDistance;
		result.ClosestPoint = boundaryTestResult.ClosestPoint.FromFlippedZVector3f();
		result.ClosestPointNormal = boundaryTestResult.ClosestPointNormal.FromFlippedZVector3f();
		return result;
	}

	public BoundaryTestResult TestPoint(Vector3 point, BoundaryType boundaryType)
	{
		OVRPlugin.BoundaryTestResult boundaryTestResult = OVRPlugin.TestBoundaryPoint(point.ToFlippedZVector3f(), (OVRPlugin.BoundaryType)boundaryType);
		BoundaryTestResult result = default(BoundaryTestResult);
		result.IsTriggering = boundaryTestResult.IsTriggering == OVRPlugin.Bool.True;
		result.ClosestDistance = boundaryTestResult.ClosestDistance;
		result.ClosestPoint = boundaryTestResult.ClosestPoint.FromFlippedZVector3f();
		result.ClosestPointNormal = boundaryTestResult.ClosestPointNormal.FromFlippedZVector3f();
		return result;
	}

	public void SetLookAndFeel(BoundaryLookAndFeel lookAndFeel)
	{
		OVRPlugin.BoundaryLookAndFeel boundaryLookAndFeel = default(OVRPlugin.BoundaryLookAndFeel);
		boundaryLookAndFeel.Color = lookAndFeel.Color.ToColorf();
		OVRPlugin.BoundaryLookAndFeel boundaryLookAndFeel2 = boundaryLookAndFeel;
		OVRPlugin.SetBoundaryLookAndFeel(boundaryLookAndFeel2);
	}

	public void ResetLookAndFeel()
	{
		OVRPlugin.ResetBoundaryLookAndFeel();
	}

	public Vector3[] GetGeometry(BoundaryType boundaryType)
	{
		int pointsCount = 0;
		if (OVRPlugin.GetBoundaryGeometry2((OVRPlugin.BoundaryType)boundaryType, IntPtr.Zero, ref pointsCount))
		{
			int num = pointsCount * cachedVector3fSize;
			if (cachedGeometryNativeBuffer.GetCapacity() < num)
			{
				cachedGeometryNativeBuffer.Reset(num);
			}
			int num2 = pointsCount * 3;
			if (cachedGeometryManagedBuffer.Length < num2)
			{
				cachedGeometryManagedBuffer = new float[num2];
			}
			if (OVRPlugin.GetBoundaryGeometry2((OVRPlugin.BoundaryType)boundaryType, cachedGeometryNativeBuffer.GetPointer(), ref pointsCount))
			{
				Marshal.Copy(cachedGeometryNativeBuffer.GetPointer(), cachedGeometryManagedBuffer, 0, num2);
				Vector3[] array = new Vector3[pointsCount];
				for (int i = 0; i < pointsCount; i++)
				{
					ref Vector3 reference = ref array[i];
					reference = new OVRPlugin.Vector3f
					{
						x = cachedGeometryManagedBuffer[3 * i],
						y = cachedGeometryManagedBuffer[3 * i + 1],
						z = cachedGeometryManagedBuffer[3 * i + 2]
					}.FromFlippedZVector3f();
				}
				return array;
			}
		}
		return new Vector3[0];
	}

	public Vector3 GetDimensions(BoundaryType boundaryType)
	{
		return OVRPlugin.GetBoundaryDimensions((OVRPlugin.BoundaryType)boundaryType).FromVector3f();
	}

	public bool GetVisible()
	{
		return OVRPlugin.GetBoundaryVisible();
	}

	public void SetVisible(bool value)
	{
		OVRPlugin.SetBoundaryVisible(value);
	}
}
