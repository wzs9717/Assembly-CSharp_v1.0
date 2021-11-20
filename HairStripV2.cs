using System;
using UnityEngine;

public class HairStripV2
{
	public enum HairDrawType
	{
		Sheet,
		Quads,
		Tube,
		LineStrip,
		Lines,
		GPULines
	}

	public enum HairBundleType
	{
		Rectangular,
		Circular
	}

	public HairGlobalSettings globalSettings;

	public Vector3[] hmverts;

	public Vector3[] hmnormals;

	public Vector4[] hmtangents;

	public Vector2[] hmuvs;

	public int[] hmtriangles;

	public int numVertices;

	public int numTrianglePoints;

	public Vector3 anchor;

	public Vector3 root;

	public Matrix4x4 rootMatrix;

	public Matrix4x4 rootChangeMatrix;

	public Vector3 anchorToRoot;

	public Vector3 anchorTangent;

	public Vector3 anchorTangent2;

	public bool enableDraw = true;

	public bool debug;

	private LinkedPoint anchorPoint;

	private LinkedPoint rootPoint;

	private LinkedPoint lastPoint;

	private LinkedPoint back2Point;

	private LinkedPoint thisPoint;

	private bool rootMoved;

	private int numHairs;

	private Mesh hm;

	private Vector3[] subHairXYZOffsets;

	private Vector3 cameraPosition;

	private int numVerticesPerHair;

	private int numTrianglePointsPerHair;

	private int vertInc;

	private int triInc;

	private float oneMinusSheetHairRoundness;

	private float sqrtSheetHairRoundness;

	private float sqrtOneMinusSheetHairRoundness;

	private Transform cameraTransform;

	private LinkedPoint LastPoint;

	private LinkedPoint ThisPoint;

	private Ray r;

	private float staticMoveDistanceSqr;

	private float randomStiffnessVariance;

	private float stiffnessEndVaried;

	private float stiffnessRootVaried;

	private float lastStiffnessVariance;

	private void SetLineSegmentPointRoot(int index1, Vector3 p2)
	{
		hmverts[index1] = p2;
		ref Vector3 reference = ref hmnormals[index1];
		reference = anchorToRoot;
		if (globalSettings.createTangents)
		{
			ref Vector4 reference2 = ref hmtangents[index1];
			reference2 = anchorTangent;
		}
		if (debug)
		{
			Debug.DrawLine(p2, p2 + anchorToRoot * globalSettings.debugWidth, Color.cyan);
			Debug.DrawLine(p2, p2 + anchorTangent * globalSettings.debugWidth, Color.white);
			Debug.DrawLine(p2, p2 + anchorTangent2 * globalSettings.debugWidth, Color.yellow);
		}
	}

	private void SetLineSegmentPoint(int index1, Vector3 p1, Vector3 p2)
	{
		Vector3 vector = default(Vector3);
		vector.x = p2.x - p1.x;
		vector.y = p2.y - p1.y;
		vector.z = p2.z - p1.z;
		float num = 1f / vector.magnitude;
		vector.x *= num;
		vector.y *= num;
		vector.z *= num;
		Vector3 vector2 = default(Vector3);
		vector2.x = vector.y * anchorToRoot.z - vector.z * (anchorToRoot.y + globalSettings.quarterSegmentLength);
		vector2.y = vector.z * anchorToRoot.x - vector.x * anchorToRoot.z;
		vector2.z = vector.x * (anchorToRoot.y + globalSettings.quarterSegmentLength) - vector.y * anchorToRoot.x;
		num = 1f / vector2.magnitude;
		vector2.x *= num;
		vector2.y *= num;
		vector2.z *= num;
		if (vector2.sqrMagnitude <= 0.5f)
		{
			vector2.x = vector.y * anchorToRoot.z - vector.z * (anchorToRoot.y + globalSettings.quarterSegmentLength * 2f);
			vector2.y = vector.z * anchorToRoot.x - vector.x * anchorToRoot.z;
			vector2.z = vector.x * (anchorToRoot.y + globalSettings.quarterSegmentLength * 2f) - vector.y * anchorToRoot.x;
			num = 1f / vector2.magnitude;
			vector2.x *= num;
			vector2.y *= num;
			vector2.z *= num;
			if (vector2.sqrMagnitude <= 0.5f)
			{
				vector2.x = vector.y * anchorToRoot.z - vector.z * anchorToRoot.y;
				vector2.y = vector.z * (anchorToRoot.x + globalSettings.quarterSegmentLength) - vector.x * anchorToRoot.z;
				vector2.z = vector.x * anchorToRoot.y - vector.y * (anchorToRoot.x + globalSettings.quarterSegmentLength);
				num = 1f / vector2.magnitude;
				vector2.x *= num;
				vector2.y *= num;
				vector2.z *= num;
			}
		}
		Vector3 vector3 = default(Vector3);
		vector3.x = vector2.y * vector.z - vector2.z * vector.y;
		vector3.y = vector2.z * vector.x - vector2.x * vector.z;
		vector3.z = vector2.x * vector.y - vector2.y * vector.x;
		hmverts[index1] = p2;
		hmnormals[index1] = vector3;
		if (debug)
		{
			Debug.DrawLine(p2, p2 + vector3 * globalSettings.debugWidth, Color.cyan);
			Debug.DrawLine(p2, p2 + vector2 * globalSettings.debugWidth, Color.white);
		}
		if (globalSettings.createTangents)
		{
			hmtangents[index1].x = vector2.x;
			hmtangents[index1].y = vector2.y;
			hmtangents[index1].z = vector2.z;
			hmtangents[index1].w = 1f;
		}
	}

	private void SetQuad(int index1, Vector3 p1, Vector3 p2)
	{
		int num = index1 + 1;
		int num2 = num + 1;
		int num3 = num2 + 1;
		Vector3 vector = default(Vector3);
		vector.x = cameraPosition.x - p1.x;
		vector.y = cameraPosition.y - p1.y;
		vector.z = cameraPosition.z - p1.z;
		Vector3 vector2 = default(Vector3);
		vector2.x = cameraPosition.x - p2.x;
		vector2.y = cameraPosition.y - p2.y;
		vector2.z = cameraPosition.z - p2.z;
		Vector3 vector3 = default(Vector3);
		vector3.x = vector2.x - vector.x;
		vector3.y = vector2.y - vector.y;
		vector3.z = vector2.z - vector.z;
		Vector3 vector4 = default(Vector3);
		vector4.x = vector2.y * vector.z - vector2.z * vector.y;
		vector4.y = vector2.z * vector.x - vector2.x * vector.z;
		vector4.z = vector2.x * vector.y - vector2.y * vector.x;
		Vector3 vector5 = default(Vector3);
		vector5.x = vector4.y * vector3.z - vector4.z * vector3.y;
		vector5.y = vector4.z * vector3.x - vector4.x * vector3.z;
		vector5.z = vector4.x * vector3.y - vector4.y * vector3.x;
		float num4 = 1f / vector4.magnitude;
		vector4.x *= num4;
		vector4.y *= num4;
		vector4.z *= num4;
		num4 = 1f / vector5.magnitude;
		vector5.x *= num4;
		vector5.y *= num4;
		vector5.z *= num4;
		Vector3 vector6 = default(Vector3);
		vector6.x = vector4.x * globalSettings.hairHalfWidth;
		vector6.y = vector4.y * globalSettings.hairHalfWidth;
		vector6.z = vector4.z * globalSettings.hairHalfWidth;
		hmverts[index1].x = p1.x + vector6.x;
		hmverts[index1].y = p1.y + vector6.y;
		hmverts[index1].z = p1.z + vector6.z;
		hmverts[num].x = p1.x - vector6.x;
		hmverts[num].y = p1.y - vector6.y;
		hmverts[num].z = p1.z - vector6.z;
		hmverts[num2].x = p2.x + vector6.x;
		hmverts[num2].y = p2.y + vector6.y;
		hmverts[num2].z = p2.z + vector6.z;
		hmverts[num3].x = p2.x - vector6.x;
		hmverts[num3].y = p2.y - vector6.y;
		hmverts[num3].z = p2.z - vector6.z;
		hmnormals[index1] = vector5;
		hmnormals[num] = vector5;
		hmnormals[num2] = vector5;
		hmnormals[num3] = vector5;
		if (globalSettings.createTangents)
		{
			hmtangents[index1].x = vector4.x;
			hmtangents[index1].y = vector4.y;
			hmtangents[index1].z = vector4.z;
			hmtangents[index1].w = 1f;
			hmtangents[num].x = vector4.x;
			hmtangents[num].y = vector4.y;
			hmtangents[num].z = vector4.z;
			hmtangents[num].w = 1f;
			hmtangents[num2].x = vector4.x;
			hmtangents[num2].y = vector4.y;
			hmtangents[num2].z = vector4.z;
			hmtangents[num2].w = 1f;
			hmtangents[num3].x = vector4.x;
			hmtangents[num3].y = vector4.y;
			hmtangents[num3].z = vector4.z;
			hmtangents[num3].w = 1f;
		}
	}

	private void SetSheetLayer(int index1, Vector3 p1, Vector3 p2)
	{
		int num = index1 + 1;
		Vector3 vector = default(Vector3);
		vector.x = cameraPosition.x - p1.x;
		vector.y = cameraPosition.y - p1.y;
		vector.z = cameraPosition.z - p1.z;
		Vector3 vector2 = default(Vector3);
		vector2.x = cameraPosition.x - p2.x;
		vector2.y = cameraPosition.y - p2.y;
		vector2.z = cameraPosition.z - p2.z;
		Vector3 vector3 = default(Vector3);
		vector3.x = vector2.x - vector.x;
		vector3.y = vector2.y - vector.y;
		vector3.z = vector2.z - vector.z;
		Vector3 vector4 = default(Vector3);
		vector4.x = vector2.y * vector.z - vector2.z * vector.y;
		vector4.y = vector2.z * vector.x - vector2.x * vector.z;
		vector4.z = vector2.x * vector.y - vector2.y * vector.x;
		float num2 = 1f / vector4.magnitude;
		vector4.x *= num2;
		vector4.y *= num2;
		vector4.z *= num2;
		Vector3 vector5 = default(Vector3);
		vector5.x = vector4.y * vector3.z - vector4.z * vector3.y;
		vector5.y = vector4.z * vector3.x - vector4.x * vector3.z;
		vector5.z = vector4.x * vector3.y - vector4.y * vector3.x;
		num2 = 1f / vector5.magnitude;
		vector5.x *= num2;
		vector5.y *= num2;
		vector5.z *= num2;
		Vector3 vector6 = default(Vector3);
		vector6.x = vector4.x * globalSettings.hairHalfWidth;
		vector6.y = vector4.y * globalSettings.hairHalfWidth;
		vector6.z = vector4.z * globalSettings.hairHalfWidth;
		hmverts[index1].x = p2.x + vector6.x;
		hmverts[index1].y = p2.y + vector6.y;
		hmverts[index1].z = p2.z + vector6.z;
		hmverts[num].x = p2.x - vector6.x;
		hmverts[num].y = p2.y - vector6.y;
		hmverts[num].z = p2.z - vector6.z;
		if (globalSettings.roundSheetHairs)
		{
			float num3 = sqrtOneMinusSheetHairRoundness * vector5.x;
			float num4 = sqrtOneMinusSheetHairRoundness * vector5.y;
			float num5 = sqrtOneMinusSheetHairRoundness * vector5.z;
			float num6 = sqrtSheetHairRoundness * vector4.x;
			float num7 = sqrtSheetHairRoundness * vector4.y;
			float num8 = sqrtSheetHairRoundness * vector4.z;
			Vector3 vector7 = default(Vector3);
			vector7.x = num3 + num6;
			vector7.y = num4 + num7;
			vector7.z = num5 + num8;
			Vector3 vector8 = default(Vector3);
			vector8.x = num3 - num6;
			vector8.y = num4 - num7;
			vector8.z = num5 - num8;
			hmnormals[index1] = vector7;
			hmnormals[num] = vector8;
		}
		else
		{
			hmnormals[index1] = vector5;
			hmnormals[num] = vector5;
		}
		if (globalSettings.createTangents)
		{
			hmtangents[index1].x = vector4.x;
			hmtangents[index1].y = vector4.y;
			hmtangents[index1].z = vector4.z;
			hmtangents[index1].w = 1f;
			hmtangents[num].x = vector4.x;
			hmtangents[num].y = vector4.y;
			hmtangents[num].z = vector4.z;
			hmtangents[num].w = 1f;
		}
	}

	private void SetCylinderLayer(int index1, Vector3 p1, Vector3 p2)
	{
		int num = index1 + 1;
		int num2 = num + 1;
		int num3 = num2 + 1;
		Vector3 vector = default(Vector3);
		vector.x = cameraPosition.x - p1.x;
		vector.y = cameraPosition.y - p1.y;
		vector.z = cameraPosition.z - p1.z;
		Vector3 vector2 = default(Vector3);
		vector2.x = cameraPosition.x - p2.x;
		vector2.y = cameraPosition.y - p2.y;
		vector2.z = cameraPosition.z - p2.z;
		Vector3 vector3 = default(Vector3);
		vector3.x = vector2.x - vector.x;
		vector3.y = vector2.y - vector.y;
		vector3.z = vector2.z - vector.z;
		Vector3 vector4 = default(Vector3);
		vector4.x = vector2.y * vector.z - vector2.z * vector.y;
		vector4.y = vector2.z * vector.x - vector2.x * vector.z;
		vector4.z = vector2.x * vector.y - vector2.y * vector.x;
		Vector3 vector5 = default(Vector3);
		vector5.x = vector4.y * vector3.z - vector4.z * vector3.y;
		vector5.y = vector4.z * vector3.x - vector4.x * vector3.z;
		vector5.z = vector4.x * vector3.y - vector4.y * vector3.x;
		float num4 = 1f / vector3.magnitude;
		vector3.x *= num4;
		vector3.y *= num4;
		vector3.z *= num4;
		num4 = 1f / vector4.magnitude;
		vector4.x *= num4;
		vector4.y *= num4;
		vector4.z *= num4;
		num4 = 1f / vector5.magnitude;
		vector5.x *= num4;
		vector5.y *= num4;
		vector5.z *= num4;
		Vector3 vector6 = default(Vector3);
		vector6.x = vector4.x * globalSettings.hairHalfWidth;
		vector6.y = vector4.y * globalSettings.hairHalfWidth;
		vector6.z = vector4.z * globalSettings.hairHalfWidth;
		Vector3 vector7 = default(Vector3);
		vector7.x = vector5.x * globalSettings.hairHalfWidth;
		vector7.y = vector5.y * globalSettings.hairHalfWidth;
		vector7.z = vector5.z * globalSettings.hairHalfWidth;
		hmverts[index1].x = p2.x + vector6.x;
		hmverts[index1].y = p2.y + vector6.y;
		hmverts[index1].z = p2.z + vector6.z;
		hmverts[num].x = p2.x + vector7.x;
		hmverts[num].y = p2.y + vector7.y;
		hmverts[num].z = p2.z + vector7.z;
		hmverts[num2].x = p2.x - vector6.x;
		hmverts[num2].y = p2.y - vector6.y;
		hmverts[num2].z = p2.z - vector6.z;
		hmverts[num3].x = p2.x - vector7.x;
		hmverts[num3].y = p2.y - vector7.y;
		hmverts[num3].z = p2.z - vector7.z;
		hmnormals[index1] = vector4;
		hmnormals[num] = vector5;
		hmnormals[num2].x = 0f - vector4.x;
		hmnormals[num2].y = 0f - vector4.y;
		hmnormals[num2].z = 0f - vector4.z;
		hmnormals[num3].x = 0f - vector5.x;
		hmnormals[num3].y = 0f - vector5.y;
		hmnormals[num3].z = 0f - vector5.z;
		if (globalSettings.createTangents)
		{
			hmtangents[index1].x = vector4.x;
			hmtangents[index1].y = vector4.y;
			hmtangents[index1].z = vector4.z;
			hmtangents[index1].w = 1f;
			hmtangents[num].x = vector5.x;
			hmtangents[num].y = vector5.y;
			hmtangents[num].z = vector5.z;
			hmtangents[num].w = 1f;
			hmtangents[num2].x = vector4.x;
			hmtangents[num2].y = vector4.y;
			hmtangents[num2].z = vector4.z;
			hmtangents[num2].w = 1f;
			hmtangents[num3].x = vector5.x;
			hmtangents[num3].y = vector5.y;
			hmtangents[num3].z = vector5.z;
			hmtangents[num3].w = 1f;
		}
	}

	private void SetQuadTriangles(int tindex, int vindex)
	{
		hmtriangles[tindex] = vindex;
		hmtriangles[tindex + 1] = vindex + 1;
		hmtriangles[tindex + 2] = vindex + 2;
		hmtriangles[tindex + 3] = vindex + 1;
		hmtriangles[tindex + 4] = vindex + 3;
		hmtriangles[tindex + 5] = vindex + 2;
	}

	private void SetSheetLayerTriangles(int tindex, int vindex)
	{
		hmtriangles[tindex] = vindex - 2;
		hmtriangles[tindex + 1] = vindex - 1;
		hmtriangles[tindex + 2] = vindex;
		hmtriangles[tindex + 3] = vindex - 1;
		hmtriangles[tindex + 4] = vindex + 1;
		hmtriangles[tindex + 5] = vindex;
	}

	private void SetCylinderLayerTriangles(int tindex, int vindex)
	{
		hmtriangles[tindex] = vindex - 4;
		hmtriangles[tindex + 1] = vindex - 3;
		hmtriangles[tindex + 2] = vindex + 1;
		hmtriangles[tindex + 3] = vindex - 4;
		hmtriangles[tindex + 4] = vindex + 1;
		hmtriangles[tindex + 5] = vindex;
		hmtriangles[tindex + 6] = vindex - 3;
		hmtriangles[tindex + 7] = vindex - 2;
		hmtriangles[tindex + 8] = vindex + 2;
		hmtriangles[tindex + 9] = vindex - 3;
		hmtriangles[tindex + 10] = vindex + 2;
		hmtriangles[tindex + 11] = vindex + 1;
		hmtriangles[tindex + 12] = vindex - 2;
		hmtriangles[tindex + 13] = vindex - 1;
		hmtriangles[tindex + 14] = vindex + 3;
		hmtriangles[tindex + 15] = vindex - 2;
		hmtriangles[tindex + 16] = vindex + 3;
		hmtriangles[tindex + 17] = vindex + 2;
		hmtriangles[tindex + 18] = vindex - 1;
		hmtriangles[tindex + 19] = vindex - 4;
		hmtriangles[tindex + 20] = vindex;
		hmtriangles[tindex + 21] = vindex - 1;
		hmtriangles[tindex + 22] = vindex;
		hmtriangles[tindex + 23] = vindex + 3;
	}

	private void determineNumVerticesRequired()
	{
		if (globalSettings.hairDrawType == HairDrawType.Tube)
		{
			numVerticesPerHair = 4 * (globalSettings.numberSegments + 1);
			vertInc = 4;
		}
		else if (globalSettings.hairDrawType == HairDrawType.Sheet)
		{
			numVerticesPerHair = 2 * (globalSettings.numberSegments + 1);
			vertInc = 2;
		}
		else if (globalSettings.hairDrawType == HairDrawType.LineStrip || globalSettings.hairDrawType == HairDrawType.GPULines)
		{
			numVerticesPerHair = globalSettings.numberSegments + 1;
			vertInc = 1;
		}
		else if (globalSettings.hairDrawType == HairDrawType.Lines)
		{
			numVerticesPerHair = globalSettings.numberSegments + 1;
			vertInc = 1;
		}
		else
		{
			numVerticesPerHair = 4 * globalSettings.numberSegments;
			vertInc = 4;
		}
		numVertices = numVerticesPerHair * numHairs;
	}

	private void determineNumTrianglePointsRequired()
	{
		if (globalSettings.hairDrawType == HairDrawType.Tube)
		{
			numTrianglePointsPerHair = 24 * globalSettings.numberSegments;
			triInc = 24;
		}
		else if (globalSettings.hairDrawType == HairDrawType.Sheet)
		{
			numTrianglePointsPerHair = 6 * globalSettings.numberSegments;
			triInc = 6;
		}
		else if (globalSettings.hairDrawType == HairDrawType.LineStrip || globalSettings.hairDrawType == HairDrawType.GPULines)
		{
			numTrianglePointsPerHair = globalSettings.numberSegments + 1;
			triInc = 1;
		}
		else if (globalSettings.hairDrawType == HairDrawType.Lines)
		{
			numTrianglePointsPerHair = 2 * globalSettings.numberSegments;
			triInc = 2;
		}
		else
		{
			numTrianglePointsPerHair = 6 * globalSettings.numberSegments;
			triInc = 6;
		}
		numTrianglePoints = numTrianglePointsPerHair * numHairs;
	}

	private void CreateMeshDataFromPoints(int vindex, int tindex)
	{
		int num = vindex;
		int num2 = tindex;
		float num3 = 1f;
		float num4 = -1f / (float)globalSettings.numberSegments;
		float x = UnityEngine.Random.Range(0f, 1f);
		if (globalSettings.ownMesh)
		{
			hmverts = new Vector3[numVertices];
			hmnormals = new Vector3[numVertices];
			if (globalSettings.createTangents)
			{
				hmtangents = new Vector4[numVertices];
			}
			hmtriangles = new int[numTrianglePoints];
			hmuvs = new Vector2[numVertices];
		}
		if (globalSettings.hairDrawType == HairDrawType.Tube)
		{
			if (globalSettings.drawFromAnchor)
			{
				SetCylinderLayer(vindex, rootPoint.position, anchor);
			}
			else
			{
				SetCylinderLayer(vindex, anchor, rootPoint.position);
			}
			hmuvs[vindex].x = 0f;
			hmuvs[vindex].y = num3;
			hmuvs[vindex + 1].x = 1f;
			hmuvs[vindex + 1].y = num3;
			hmuvs[vindex + 2].x = 0f;
			hmuvs[vindex + 2].y = num3;
			hmuvs[vindex + 3].x = 1f;
			hmuvs[vindex + 3].y = num3;
			vindex += vertInc;
			num3 += num4;
		}
		else if (globalSettings.hairDrawType == HairDrawType.Sheet)
		{
			if (globalSettings.drawFromAnchor)
			{
				SetSheetLayer(vindex, rootPoint.position, anchor);
			}
			else
			{
				SetSheetLayer(vindex, anchor, rootPoint.position);
			}
			hmuvs[vindex].x = 0f;
			hmuvs[vindex].y = num3;
			hmuvs[vindex + 1].x = 1f;
			hmuvs[vindex + 1].y = num3;
			vindex += vertInc;
			num3 += num4;
		}
		else if (globalSettings.hairDrawType == HairDrawType.LineStrip || globalSettings.hairDrawType == HairDrawType.GPULines)
		{
			if (globalSettings.drawFromAnchor)
			{
				SetLineSegmentPointRoot(vindex, anchor);
			}
			else
			{
				SetLineSegmentPointRoot(vindex, rootPoint.position);
			}
			hmtriangles[tindex] = vindex;
			hmuvs[vindex].x = x;
			hmuvs[vindex].y = num3;
			vindex += vertInc;
			tindex += triInc;
			num3 += num4;
		}
		else if (globalSettings.hairDrawType == HairDrawType.Lines)
		{
			if (globalSettings.drawFromAnchor)
			{
				SetLineSegmentPointRoot(vindex, anchor);
			}
			else
			{
				SetLineSegmentPointRoot(vindex, rootPoint.position);
			}
			hmuvs[vindex].x = 0f;
			hmuvs[vindex].y = num3;
			vindex += vertInc;
			num3 += num4;
		}
		Vector3 p = ((!globalSettings.drawFromAnchor) ? rootPoint.position : anchor);
		thisPoint = rootPoint.next;
		while (thisPoint != null)
		{
			if (globalSettings.hairDrawType == HairDrawType.Tube)
			{
				SetCylinderLayer(vindex, p, thisPoint.position);
				SetCylinderLayerTriangles(tindex, vindex);
				hmuvs[vindex].x = 0f;
				hmuvs[vindex].y = num3;
				hmuvs[vindex + 1].x = 1f;
				hmuvs[vindex + 1].y = num3;
				hmuvs[vindex + 2].x = 0f;
				hmuvs[vindex + 2].y = num3;
				hmuvs[vindex + 3].x = 1f;
				hmuvs[vindex + 3].y = num3;
			}
			else if (globalSettings.hairDrawType == HairDrawType.Sheet)
			{
				SetSheetLayer(vindex, p, thisPoint.position);
				SetSheetLayerTriangles(tindex, vindex);
				hmuvs[vindex].x = 0f;
				hmuvs[vindex].y = num3;
				hmuvs[vindex + 1].x = 1f;
				hmuvs[vindex + 1].y = num3;
			}
			else if (globalSettings.hairDrawType == HairDrawType.LineStrip)
			{
				SetLineSegmentPoint(vindex, p, thisPoint.position);
				hmtriangles[tindex] = vindex;
				hmuvs[vindex].x = x;
				hmuvs[vindex].y = num3;
			}
			else if (globalSettings.hairDrawType == HairDrawType.GPULines)
			{
				SetLineSegmentPoint(vindex, p, thisPoint.position);
				hmtriangles[tindex] = vindex;
				hmuvs[vindex].x = x;
				hmuvs[vindex].y = num3;
			}
			else if (globalSettings.hairDrawType == HairDrawType.Lines)
			{
				SetLineSegmentPoint(vindex, p, thisPoint.position);
				hmtriangles[tindex] = vindex - 1;
				hmtriangles[tindex + 1] = vindex;
				hmuvs[vindex].x = 0f;
				hmuvs[vindex].y = num3;
			}
			else
			{
				SetQuad(vindex, p, thisPoint.position);
				SetQuadTriangles(tindex, vindex);
				hmuvs[vindex].x = 0f;
				hmuvs[vindex].y = num3;
				hmuvs[vindex + 1].x = 1f;
				hmuvs[vindex + 1].y = num3;
				hmuvs[vindex + 2].x = 0f;
				hmuvs[vindex + 2].y = num3 + num4;
				hmuvs[vindex + 3].x = 1f;
				hmuvs[vindex + 3].y = num3 + num4;
			}
			p = thisPoint.position;
			thisPoint = thisPoint.next;
			vindex += vertInc;
			tindex += triInc;
			num3 += num4;
		}
		if (numHairs <= 1)
		{
			return;
		}
		for (int i = 1; i < numHairs; i++)
		{
			Vector3 zero = Vector3.zero;
			float x2 = subHairXYZOffsets[i].x;
			float y = subHairXYZOffsets[i].y;
			float z = subHairXYZOffsets[i].z;
			if (x2 != 0f)
			{
				zero.x += anchorTangent.x * x2;
				zero.y += anchorTangent.y * x2;
				zero.z += anchorTangent.z * x2;
			}
			if (y != 0f)
			{
				zero.x += anchorTangent2.x * y;
				zero.y += anchorTangent2.y * y;
				zero.z += anchorTangent2.z * y;
			}
			if (z != 0f)
			{
				zero.x += anchorToRoot.x * z;
				zero.y += anchorToRoot.y * z;
				zero.z += anchorToRoot.z * z;
			}
			for (int j = num; j < num + numVerticesPerHair; j++)
			{
				hmverts[vindex].x = hmverts[j].x + zero.x;
				hmverts[vindex].y = hmverts[j].y + zero.y;
				hmverts[vindex].z = hmverts[j].z + zero.z;
				ref Vector3 reference = ref hmnormals[vindex];
				reference = hmnormals[j];
				if (globalSettings.createTangents)
				{
					ref Vector4 reference2 = ref hmtangents[vindex];
					reference2 = hmtangents[j];
				}
				ref Vector2 reference3 = ref hmuvs[vindex];
				reference3 = hmuvs[j];
				vindex++;
			}
		}
		for (int k = 1; k < numHairs; k++)
		{
			int num5 = numVerticesPerHair * k;
			for (int l = num2; l < num2 + numTrianglePointsPerHair; l++)
			{
				hmtriangles[tindex] = hmtriangles[l] + num5;
				tindex++;
			}
		}
	}

	private void UpdateMeshDataFromPoints(int vindex)
	{
		int num = vindex;
		if (globalSettings.hairDrawType == HairDrawType.Tube)
		{
			if (enableDraw)
			{
				if (globalSettings.drawFromAnchor)
				{
					SetCylinderLayer(vindex, rootPoint.position, anchor);
				}
				else
				{
					SetCylinderLayer(vindex, anchor, rootPoint.position);
				}
			}
			else
			{
				ref Vector3 reference = ref hmverts[vindex];
				reference = Vector3.zero;
				ref Vector3 reference2 = ref hmverts[vindex + 1];
				reference2 = Vector3.zero;
				ref Vector3 reference3 = ref hmverts[vindex + 2];
				reference3 = Vector3.zero;
				ref Vector3 reference4 = ref hmverts[vindex + 3];
				reference4 = Vector3.zero;
			}
			vindex += 4;
		}
		else if (globalSettings.hairDrawType == HairDrawType.Sheet)
		{
			if (enableDraw)
			{
				if (globalSettings.drawFromAnchor)
				{
					SetSheetLayer(vindex, rootPoint.position, anchor);
				}
				else
				{
					SetSheetLayer(vindex, anchor, rootPoint.position);
				}
			}
			else
			{
				ref Vector3 reference5 = ref hmverts[vindex];
				reference5 = Vector3.zero;
				ref Vector3 reference6 = ref hmverts[vindex + 1];
				reference6 = Vector3.zero;
			}
			vindex += 2;
		}
		else if (globalSettings.hairDrawType == HairDrawType.LineStrip)
		{
			if (enableDraw)
			{
				if (globalSettings.drawFromAnchor)
				{
					SetLineSegmentPoint(vindex, rootPoint.position, anchor);
				}
				else
				{
					SetLineSegmentPoint(vindex, anchor, rootPoint.position);
				}
			}
			else
			{
				ref Vector3 reference7 = ref hmverts[vindex];
				reference7 = Vector3.zero;
			}
			vindex++;
		}
		else if (globalSettings.hairDrawType == HairDrawType.GPULines)
		{
			if (enableDraw)
			{
				if (globalSettings.drawFromAnchor)
				{
					SetLineSegmentPointRoot(vindex, anchor);
				}
				else
				{
					SetLineSegmentPointRoot(vindex, rootPoint.position);
				}
			}
			else
			{
				ref Vector3 reference8 = ref hmverts[vindex];
				reference8 = Vector3.zero;
			}
			vindex++;
		}
		else if (globalSettings.hairDrawType == HairDrawType.Lines)
		{
			if (enableDraw)
			{
				if (globalSettings.drawFromAnchor)
				{
					SetLineSegmentPoint(vindex, rootPoint.position, anchor);
				}
				else
				{
					SetLineSegmentPoint(vindex, anchor, rootPoint.position);
				}
			}
			else
			{
				ref Vector3 reference9 = ref hmverts[vindex];
				reference9 = Vector3.zero;
			}
			vindex++;
		}
		Vector3 p = ((!globalSettings.drawFromAnchor) ? rootPoint.position : anchor);
		for (thisPoint = rootPoint.next; thisPoint != null; thisPoint = thisPoint.next)
		{
			if (globalSettings.hairDrawType == HairDrawType.Tube)
			{
				if (enableDraw)
				{
					SetCylinderLayer(vindex, p, thisPoint.position);
				}
				else
				{
					ref Vector3 reference10 = ref hmverts[vindex];
					reference10 = Vector3.zero;
					ref Vector3 reference11 = ref hmverts[vindex + 1];
					reference11 = Vector3.zero;
					ref Vector3 reference12 = ref hmverts[vindex + 2];
					reference12 = Vector3.zero;
					ref Vector3 reference13 = ref hmverts[vindex + 3];
					reference13 = Vector3.zero;
				}
				vindex += 4;
			}
			else if (globalSettings.hairDrawType == HairDrawType.Sheet)
			{
				if (enableDraw)
				{
					SetSheetLayer(vindex, p, thisPoint.position);
				}
				else
				{
					ref Vector3 reference14 = ref hmverts[vindex];
					reference14 = Vector3.zero;
					ref Vector3 reference15 = ref hmverts[vindex + 1];
					reference15 = Vector3.zero;
				}
				vindex += 2;
			}
			else if (globalSettings.hairDrawType == HairDrawType.LineStrip || globalSettings.hairDrawType == HairDrawType.GPULines)
			{
				if (enableDraw)
				{
					SetLineSegmentPoint(vindex, p, thisPoint.position);
				}
				else
				{
					ref Vector3 reference16 = ref hmverts[vindex];
					reference16 = Vector3.zero;
				}
				vindex++;
			}
			else if (globalSettings.hairDrawType == HairDrawType.Lines)
			{
				if (enableDraw)
				{
					SetLineSegmentPoint(vindex, p, thisPoint.position);
				}
				else
				{
					ref Vector3 reference17 = ref hmverts[vindex];
					reference17 = Vector3.zero;
				}
				vindex++;
			}
			else
			{
				if (enableDraw)
				{
					SetQuad(vindex, p, thisPoint.position);
				}
				else
				{
					ref Vector3 reference18 = ref hmverts[vindex];
					reference18 = Vector3.zero;
					ref Vector3 reference19 = ref hmverts[vindex + 1];
					reference19 = Vector3.zero;
					ref Vector3 reference20 = ref hmverts[vindex + 2];
					reference20 = Vector3.zero;
					ref Vector3 reference21 = ref hmverts[vindex + 3];
					reference21 = Vector3.zero;
				}
				vindex += 4;
			}
			p = thisPoint.position;
		}
		if (numHairs <= 1)
		{
			return;
		}
		for (int i = 1; i < numHairs; i++)
		{
			Vector3 zero = Vector3.zero;
			float x = subHairXYZOffsets[i].x;
			float y = subHairXYZOffsets[i].y;
			float z = subHairXYZOffsets[i].z;
			if (x != 0f)
			{
				zero.x += anchorTangent.x * x;
				zero.y += anchorTangent.y * x;
				zero.z += anchorTangent.z * x;
			}
			if (y != 0f)
			{
				zero.x += anchorTangent2.x * y;
				zero.y += anchorTangent2.y * y;
				zero.z += anchorTangent2.z * y;
			}
			if (z != 0f)
			{
				zero.x += anchorToRoot.x * z;
				zero.y += anchorToRoot.y * z;
				zero.z += anchorToRoot.z * z;
			}
			if (debug)
			{
				MyDebug.DrawWireCube(hmverts[num], globalSettings.debugWidth * 0.2f, Color.red);
				MyDebug.DrawWireCube(hmverts[num] + zero, globalSettings.debugWidth * 0.2f, Color.green);
			}
			for (int j = num; j < num + numVerticesPerHair; j++)
			{
				hmverts[vindex].x = hmverts[j].x + zero.x;
				hmverts[vindex].y = hmverts[j].y + zero.y;
				hmverts[vindex].z = hmverts[j].z + zero.z;
				ref Vector3 reference22 = ref hmnormals[vindex];
				reference22 = hmnormals[j];
				if (globalSettings.createTangents)
				{
					ref Vector4 reference23 = ref hmtangents[vindex];
					reference23 = hmtangents[j];
				}
				vindex++;
			}
		}
	}

	private void CreateMesh()
	{
		if (globalSettings.ownMesh)
		{
			hm = new Mesh();
			hm.vertices = hmverts;
			hm.normals = hmnormals;
			if (globalSettings.createTangents)
			{
				hm.tangents = hmtangents;
			}
			hm.uv = hmuvs;
			if (globalSettings.hairDrawType == HairDrawType.LineStrip)
			{
				hm.SetIndices(hmtriangles, MeshTopology.LineStrip, 0);
			}
			else if (globalSettings.hairDrawType == HairDrawType.GPULines)
			{
				hm.SetIndices(hmtriangles, MeshTopology.Quads, 0);
			}
			else if (globalSettings.hairDrawType == HairDrawType.Lines)
			{
				hm.SetIndices(hmtriangles, MeshTopology.Lines, 0);
			}
			else
			{
				hm.triangles = hmtriangles;
			}
			float num = globalSettings.hairLength * 2f;
			hm.bounds = new Bounds(rootPoint.position, new Vector3(num, num, num));
		}
	}

	private void UpdateMesh()
	{
		if (globalSettings.ownMesh)
		{
			hm.vertices = hmverts;
			hm.normals = hmnormals;
			if (globalSettings.createTangents)
			{
				hm.tangents = hmtangents;
			}
			float num = globalSettings.hairLength * 2f;
			hm.bounds = new Bounds(rootPoint.position, new Vector3(num, num, num));
		}
	}

	private void CreatePoints()
	{
		anchorPoint = new LinkedPoint(anchor);
		rootPoint = new LinkedPoint(root);
		lastPoint = rootPoint;
		for (int i = 0; i < globalSettings.numberSegments; i++)
		{
			thisPoint = new LinkedPoint(lastPoint.position + anchorToRoot * globalSettings.segmentLength);
			thisPoint.previous = lastPoint;
			thisPoint.force = new Vector3(0f, 0f, 0f);
			lastPoint.next = thisPoint;
			lastPoint = thisPoint;
		}
	}

	private void MoveThisPointPhysically(float stiffnessGraded)
	{
		thisPoint.previous_position = Vector3.Lerp(thisPoint.position, thisPoint.stiff_position, stiffnessGraded);
		float num = globalSettings.velocityFactor * globalSettings.deltaTime;
		thisPoint.unconstrained_position.x = thisPoint.previous_position.x + num * thisPoint.velocity.x + globalSettings.deltaTimeSqr * (thisPoint.force.x + globalSettings.gravityForce.x);
		thisPoint.unconstrained_position.y = thisPoint.previous_position.y + num * thisPoint.velocity.y + globalSettings.deltaTimeSqr * (thisPoint.force.y + globalSettings.gravityForce.y);
		thisPoint.unconstrained_position.z = thisPoint.previous_position.z + num * thisPoint.velocity.z + globalSettings.deltaTimeSqr * (thisPoint.force.z + globalSettings.gravityForce.z);
		thisPoint.position = thisPoint.unconstrained_position;
	}

	private void MoveThisPointLegalTo2BackPoint()
	{
		if (lastPoint.previous != null)
		{
			LinkedPoint previous = lastPoint.previous;
			Vector3 vector = default(Vector3);
			vector.x = thisPoint.position.x - previous.position.x;
			vector.y = thisPoint.position.y - previous.position.y;
			vector.z = thisPoint.position.z - previous.position.z;
			float num = globalSettings.segmentLength / vector.magnitude;
			if (num > 1f)
			{
				vector.x *= num;
				vector.y *= num;
				vector.z *= num;
				thisPoint.position.x = previous.position.x + vector.x;
				thisPoint.position.y = previous.position.y + vector.y;
				thisPoint.position.z = previous.position.z + vector.z;
			}
		}
	}

	private void MoveThisPointLegalToLastPoint()
	{
		Vector3 vector = default(Vector3);
		vector.x = thisPoint.position.x - lastPoint.position.x;
		vector.y = thisPoint.position.y - lastPoint.position.y;
		vector.z = thisPoint.position.z - lastPoint.position.z;
		float num = globalSettings.segmentLength / vector.magnitude;
		vector.x *= num;
		vector.y *= num;
		vector.z *= num;
		thisPoint.position.x = lastPoint.position.x + vector.x;
		thisPoint.position.y = lastPoint.position.y + vector.y;
		thisPoint.position.z = lastPoint.position.z + vector.z;
	}

	private void MoveThisPointToCapsuleColliderSurface(int index)
	{
		ExtendedCapsuleCollider extendedCapsuleCollider = globalSettings.extendedColliders[index];
		float num = extendedCapsuleCollider.endPoint2.x - extendedCapsuleCollider.endPoint1.x;
		float num2 = extendedCapsuleCollider.endPoint2.y - extendedCapsuleCollider.endPoint1.y;
		float num3 = extendedCapsuleCollider.endPoint2.z - extendedCapsuleCollider.endPoint1.z;
		float num4 = thisPoint.position.x - extendedCapsuleCollider.endPoint1.x;
		float num5 = thisPoint.position.y - extendedCapsuleCollider.endPoint1.y;
		float num6 = thisPoint.position.z - extendedCapsuleCollider.endPoint1.z;
		float num7 = num4 * num + num5 * num2 + num6 * num3;
		float num8;
		if (num7 < 0f)
		{
			num8 = num4 * num4 + num5 * num5 + num6 * num6;
			if (!(num8 > extendedCapsuleCollider.radiusSquared))
			{
				float num9 = 1f / Mathf.Sqrt(num8);
				thisPoint.position.x = extendedCapsuleCollider.endPoint1.x + num4 * num9 * extendedCapsuleCollider.radius;
				thisPoint.position.y = extendedCapsuleCollider.endPoint1.y + num5 * num9 * extendedCapsuleCollider.radius;
				thisPoint.position.z = extendedCapsuleCollider.endPoint1.z + num6 * num9 * extendedCapsuleCollider.radius;
				thisPoint.collided = true;
				if (debug)
				{
					MyDebug.DrawWireCube(thisPoint.position, 0.005f, Color.green);
				}
			}
			return;
		}
		if (num7 > extendedCapsuleCollider.lengthSquared)
		{
			num4 = thisPoint.position.x - extendedCapsuleCollider.endPoint2.x;
			num5 = thisPoint.position.y - extendedCapsuleCollider.endPoint2.y;
			num6 = thisPoint.position.z - extendedCapsuleCollider.endPoint2.z;
			num8 = num4 * num4 + num5 * num5 + num6 * num6;
			if (!(num8 > extendedCapsuleCollider.radiusSquared))
			{
				float num10 = 1f / Mathf.Sqrt(num8);
				thisPoint.position.x = extendedCapsuleCollider.endPoint2.x + num4 * num10 * extendedCapsuleCollider.radius;
				thisPoint.position.y = extendedCapsuleCollider.endPoint2.y + num5 * num10 * extendedCapsuleCollider.radius;
				thisPoint.position.z = extendedCapsuleCollider.endPoint2.z + num6 * num10 * extendedCapsuleCollider.radius;
				thisPoint.collided = true;
				if (debug)
				{
					MyDebug.DrawWireCube(thisPoint.position, 0.005f, Color.yellow);
				}
			}
			return;
		}
		num8 = num4 * num4 + num5 * num5 + num6 * num6 - num7 * num7 * extendedCapsuleCollider.oneOverLengthSquared;
		if (!(num8 > extendedCapsuleCollider.radiusSquared))
		{
			float num11 = num7 * extendedCapsuleCollider.oneOverLengthSquared;
			float num12 = extendedCapsuleCollider.endPoint1.x + num * num11;
			float num13 = extendedCapsuleCollider.endPoint1.y + num2 * num11;
			float num14 = extendedCapsuleCollider.endPoint1.z + num3 * num11;
			if (debug)
			{
				Vector3 position = default(Vector3);
				position.x = num12;
				position.y = num13;
				position.z = num14;
				MyDebug.DrawWireCube(position, 0.005f, Color.blue);
			}
			float num15 = thisPoint.position.x - num12;
			float num16 = thisPoint.position.y - num13;
			float num17 = thisPoint.position.z - num14;
			float num18 = 1f / Mathf.Sqrt(num15 * num15 + num16 * num16 + num17 * num17);
			thisPoint.collided = true;
			thisPoint.position.x = num12 + num15 * num18 * extendedCapsuleCollider.radius;
			thisPoint.position.y = num13 + num16 * num18 * extendedCapsuleCollider.radius;
			thisPoint.position.z = num14 + num17 * num18 * extendedCapsuleCollider.radius;
			if (debug)
			{
				MyDebug.DrawWireCube(thisPoint.position, 0.005f, Color.red);
			}
		}
	}

	private void MoveThisPointToColliderSurface(int colliderNum, bool calculateTangent)
	{
		Collider collider = globalSettings.colliders[colliderNum];
		float num = 10f;
		Vector3 vector = globalSettings.colliderCenters[colliderNum];
		Vector3 direction = default(Vector3);
		direction.x = vector.x - thisPoint.position.x;
		direction.y = vector.y - thisPoint.position.y;
		direction.z = vector.z - thisPoint.position.z;
		r.origin = thisPoint.position;
		r.direction = direction;
		if (collider.Raycast(r, out var hitInfo, num))
		{
			return;
		}
		float num2 = num / direction.magnitude;
		Vector3 direction2 = default(Vector3);
		direction2.x = direction.x * num2;
		direction2.y = direction.y * num2;
		direction2.z = direction.z * num2;
		Vector3 origin = default(Vector3);
		origin.x = thisPoint.position.x - direction2.x;
		origin.y = thisPoint.position.y - direction2.y;
		origin.z = thisPoint.position.z - direction2.z;
		r.origin = origin;
		r.direction = direction2;
		if (!collider.Raycast(r, out hitInfo, num))
		{
			return;
		}
		Vector3 normal = hitInfo.normal;
		thisPoint.position = hitInfo.point;
		if (debug)
		{
			MyDebug.DrawWireCube(thisPoint.position, globalSettings.debugWidth, Color.red);
		}
		thisPoint.collided = true;
		if (!calculateTangent)
		{
			return;
		}
		Vector3 vector2 = default(Vector3);
		vector2.x = direction2.y * normal.z - direction2.z * normal.y;
		vector2.y = direction2.z * normal.x - direction2.x * normal.z;
		vector2.z = direction2.x * normal.y - direction2.y * normal.x;
		num2 = 1f / vector2.magnitude;
		vector2.x *= num2;
		vector2.y *= num2;
		vector2.z *= num2;
		if (vector2.sqrMagnitude <= 0.5f)
		{
			direction2.x += globalSettings.quarterSegmentLength;
			vector2.x = direction2.y * normal.z - direction2.z * normal.y;
			vector2.y = direction2.z * normal.x - direction2.x * normal.z;
			vector2.z = direction2.x * normal.y - direction2.y * normal.x;
			num2 = 1f / vector2.magnitude;
			vector2.x *= num2;
			vector2.y *= num2;
			vector2.z *= num2;
			if (vector2.sqrMagnitude <= 0.5f)
			{
				direction2.y += globalSettings.quarterSegmentLength;
				vector2.x = direction2.y * normal.z - direction2.z * normal.y;
				vector2.y = direction2.z * normal.x - direction2.x * normal.z;
				vector2.z = direction2.x * normal.y - direction2.y * normal.x;
				num2 = 1f / vector2.magnitude;
				vector2.x *= num2;
				vector2.y *= num2;
				vector2.z *= num2;
			}
		}
	}

	private void SimulateRoot()
	{
		anchorPoint.previous_position = anchorPoint.position;
		anchorPoint.position = anchor;
		rootPoint.previous_position = rootPoint.position;
		rootPoint.position = root;
		rootPoint.unconstrained_position = root;
		rootPoint.position = root;
		lastPoint = anchorPoint;
		thisPoint = rootPoint;
	}

	private void SimulateThisPoint(int ind)
	{
		MoveThisPointPhysically(Mathf.Lerp(stiffnessRootVaried, stiffnessEndVaried, (float)ind * globalSettings.invNumberSegments));
		MoveThisPointLegalTo2BackPoint();
		MoveThisPointLegalToLastPoint();
		if (globalSettings.enableCollision)
		{
			thisPoint.had_collided = thisPoint.collided;
			thisPoint.collided = false;
			if (globalSettings.extendedColliders != null && globalSettings.useExtendedColliders)
			{
				for (int i = 0; i < globalSettings.extendedColliders.Length; i++)
				{
					MoveThisPointToCapsuleColliderSurface(i);
				}
			}
			else
			{
				int num = 0;
				Collider[] colliders = globalSettings.colliders;
				foreach (Collider collider in colliders)
				{
					if (!(collider is MeshCollider))
					{
						MoveThisPointToColliderSurface(num, calculateTangent: false);
					}
					num++;
				}
			}
			MoveThisPointLegalToLastPoint();
			if (globalSettings.staticFriction && thisPoint.had_collided && thisPoint.collided)
			{
				Vector3 vector = default(Vector3);
				vector.x = thisPoint.position.x - thisPoint.stiff_position.x;
				vector.y = thisPoint.position.y - thisPoint.stiff_position.y;
				vector.z = thisPoint.position.z - thisPoint.stiff_position.z;
				if (vector.sqrMagnitude < globalSettings.staticMoveDistanceSqr)
				{
					thisPoint.position = thisPoint.stiff_position;
				}
			}
		}
		thisPoint.delta_position.x = thisPoint.unconstrained_position.x - thisPoint.position.x;
		thisPoint.delta_position.y = thisPoint.unconstrained_position.y - thisPoint.position.y;
		thisPoint.delta_position.z = thisPoint.unconstrained_position.z - thisPoint.position.z;
	}

	private void SimulatePoints()
	{
		if (debug)
		{
			MyDebug.DrawWireCube(anchor, globalSettings.debugWidth, Color.green);
		}
		SimulateRoot();
		lastPoint = rootPoint;
		thisPoint = rootPoint.next;
		int num = 0;
		while (thisPoint != null)
		{
			if (globalSettings.enableSimulation)
			{
				thisPoint.stiff_position = rootChangeMatrix.MultiplyPoint3x4(thisPoint.position);
				SimulateThisPoint(num);
			}
			else
			{
				thisPoint.previous_position = thisPoint.position;
				thisPoint.position = rootChangeMatrix.MultiplyPoint3x4(thisPoint.position);
			}
			if (debug)
			{
				MyDebug.DrawWireCube(thisPoint.position, globalSettings.debugWidth, Color.blue);
			}
			lastPoint = thisPoint;
			thisPoint = thisPoint.next;
			num++;
		}
		if (!globalSettings.enableSimulation || !(globalSettings.velocityFactor > 0f))
		{
			return;
		}
		thisPoint = rootPoint.next;
		num = 0;
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		while (thisPoint != null)
		{
			vector.x = thisPoint.velocity.x;
			vector.y = thisPoint.velocity.y;
			vector.z = thisPoint.velocity.z;
			vector2.x = thisPoint.position.x - thisPoint.previous_position.x;
			vector2.y = thisPoint.position.y - thisPoint.previous_position.y;
			vector2.z = thisPoint.position.z - thisPoint.previous_position.z;
			thisPoint.velocity.x = vector2.x * globalSettings.invDeltaTime;
			thisPoint.velocity.y = vector2.y * globalSettings.invDeltaTime;
			thisPoint.velocity.z = vector2.z * globalSettings.invDeltaTime;
			if (thisPoint.next != null)
			{
				thisPoint.velocity.x += globalSettings.invdtdampen * thisPoint.next.delta_position.x;
				thisPoint.velocity.y += globalSettings.invdtdampen * thisPoint.next.delta_position.y;
				thisPoint.velocity.z += globalSettings.invdtdampen * thisPoint.next.delta_position.z;
			}
			vector3.x = thisPoint.velocity.x - vector.x;
			vector3.y = thisPoint.velocity.y - vector.y;
			vector3.z = thisPoint.velocity.z - vector.z;
			if (globalSettings.clampAcceleration)
			{
				vector3.x = Mathf.Clamp(vector3.x, 0f - globalSettings.accelerationClamp, globalSettings.accelerationClamp);
				vector3.y = Mathf.Clamp(vector3.y, 0f - globalSettings.accelerationClamp, globalSettings.accelerationClamp);
				vector3.z = Mathf.Clamp(vector3.z, 0f - globalSettings.accelerationClamp, globalSettings.accelerationClamp);
			}
			thisPoint.velocity.x = vector.x + vector3.x;
			thisPoint.velocity.y = vector.y + vector3.y;
			thisPoint.velocity.z = vector.z + vector3.z;
			if (globalSettings.clampVelocity)
			{
				thisPoint.velocity.x = Mathf.Clamp(thisPoint.velocity.x, 0f - globalSettings.velocityClamp, globalSettings.velocityClamp);
				thisPoint.velocity.y = Mathf.Clamp(thisPoint.velocity.y, 0f - globalSettings.velocityClamp, globalSettings.velocityClamp);
				thisPoint.velocity.z = Mathf.Clamp(thisPoint.velocity.z, 0f - globalSettings.velocityClamp, globalSettings.velocityClamp);
			}
			thisPoint = thisPoint.next;
			num++;
		}
	}

	public void SetVarsThreadSafe()
	{
		if (lastStiffnessVariance != globalSettings.stiffnessVariance)
		{
			randomStiffnessVariance = UnityEngine.Random.Range(0f - globalSettings.stiffnessVariance, globalSettings.stiffnessVariance);
			lastStiffnessVariance = globalSettings.stiffnessVariance;
		}
		stiffnessRootVaried = Mathf.Clamp01(globalSettings.stiffnessRoot + randomStiffnessVariance);
		stiffnessEndVaried = Mathf.Clamp01(globalSettings.stiffnessEnd + randomStiffnessVariance);
	}

	public void SetVars()
	{
		if (cameraTransform != null)
		{
			cameraPosition = cameraTransform.position;
		}
		else
		{
			cameraPosition = Vector3.zero;
		}
		if (globalSettings.hairDrawType == HairDrawType.Sheet && globalSettings.roundSheetHairs)
		{
			oneMinusSheetHairRoundness = 1f - globalSettings.sheetHairRoundness;
			sqrtSheetHairRoundness = Mathf.Sqrt(globalSettings.sheetHairRoundness);
			sqrtOneMinusSheetHairRoundness = Mathf.Sqrt(oneMinusSheetHairRoundness);
		}
		SetVarsThreadSafe();
	}

	public void Init()
	{
		if (Camera.main != null)
		{
			cameraTransform = Camera.main.transform;
		}
		r = default(Ray);
		float f = UnityEngine.Random.Range(globalSettings.numHairsMin, (float)globalSettings.numHairsMax + 0.999f);
		numHairs = Mathf.FloorToInt(f);
		determineNumVerticesRequired();
		determineNumTrianglePointsRequired();
	}

	public void Start(int vindex, int tindex)
	{
		rootMatrix = Matrix4x4.identity;
		rootChangeMatrix = Matrix4x4.identity;
		SetVars();
		CreatePoints();
		subHairXYZOffsets = new Vector3[numHairs];
		ref Vector3 reference = ref subHairXYZOffsets[0];
		reference = Vector3.zero;
		for (int i = 1; i < numHairs; i++)
		{
			if (globalSettings.bundleType == HairBundleType.Circular)
			{
				float num = UnityEngine.Random.Range(0f, globalSettings.subHairXOffsetMax);
				float num2 = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
				float x = Mathf.Cos(num2) * num;
				float y = Mathf.Sin(num2) * num;
				float z = globalSettings.subHairZOffsetBend * num;
				ref Vector3 reference2 = ref subHairXYZOffsets[i];
				reference2 = new Vector3(x, y, z);
				if (debug)
				{
					Debug.Log("rnd is " + num + " angle is " + num2 + " offsets are " + subHairXYZOffsets[i].ToString("F3"));
				}
			}
			else
			{
				float num3 = UnityEngine.Random.Range(0f - globalSettings.subHairXOffsetMax, globalSettings.subHairXOffsetMax);
				float num4 = UnityEngine.Random.Range(0f - globalSettings.subHairYOffsetMax, globalSettings.subHairYOffsetMax);
				float num5 = Mathf.Sqrt(num3 * num3 + num4 * num4);
				float z2 = globalSettings.subHairZOffsetBend * num5;
				ref Vector3 reference3 = ref subHairXYZOffsets[i];
				reference3 = new Vector3(num3, num4, z2);
			}
		}
		CreateMeshDataFromPoints(vindex, tindex);
		CreateMesh();
		SimulatePoints();
		UpdateMeshDataFromPoints(vindex);
		UpdateMesh();
	}

	public void Update(int vindex)
	{
		SetVars();
		SimulatePoints();
		UpdateMeshDataFromPoints(vindex);
		if (globalSettings.ownMesh && enableDraw)
		{
			UpdateMesh();
			Matrix4x4 identity = Matrix4x4.identity;
			Graphics.DrawMesh(hm, identity, globalSettings.hairMaterial, 0, null, 0, null, globalSettings.castShadows, globalSettings.receiveShadows);
		}
	}

	public void UpdateThreadSafe(int vindex)
	{
		SetVarsThreadSafe();
		SimulatePoints();
		UpdateMeshDataFromPoints(vindex);
	}
}
