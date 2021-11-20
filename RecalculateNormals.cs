using System.Threading;
using UnityEngine;

public class RecalculateNormals
{
	private static bool[] reusableVertexMarkerArray;

	private int[] triangles;

	private Vector3[] vertices;

	private Vector3[] normals;

	private Vector3[] surfaceNormals;

	private bool[] thisMarkerArray;

	private bool _useSleep;

	public RecalculateNormals(int[] tris, Vector3[] verts, Vector3[] norms, Vector3[] surfaceNorms = null, bool useSleep = false)
	{
		triangles = tris;
		vertices = verts;
		normals = norms;
		if (surfaceNorms == null)
		{
			surfaceNorms = new Vector3[triangles.Length / 3];
		}
		surfaceNormals = surfaceNorms;
		thisMarkerArray = new bool[vertices.Length];
		_useSleep = useSleep;
	}

	public void recalculateNormals(bool[] vertexDirty = null)
	{
		recalculateNormals(triangles, vertices, normals, surfaceNormals, vertexDirty, thisMarkerArray, _useSleep);
	}

	public static void recalculateNormals(int[] triangles, Vector3[] vertices, Vector3[] normals, Vector3[] surfaceNormals, bool[] vertexDirty = null, bool[] markerArray = null, bool useSleep = false, bool normalizeSurfaceNormals = true)
	{
		int num = triangles.Length;
		int num2 = vertices.Length;
		if (markerArray == null)
		{
			if (reusableVertexMarkerArray == null || reusableVertexMarkerArray.Length < num2)
			{
				reusableVertexMarkerArray = new bool[num2];
			}
			markerArray = reusableVertexMarkerArray;
		}
		if (vertexDirty != null)
		{
			for (int i = 0; i < num; i += 3)
			{
				int num3 = triangles[i];
				int num4 = triangles[i + 1];
				int num5 = triangles[i + 2];
				if (vertexDirty[num3] || vertexDirty[num4] || vertexDirty[num5])
				{
					bool flag;
					vertexDirty[num4] = (flag = (vertexDirty[num5] = true));
					vertexDirty[num3] = flag;
				}
			}
		}
		int num6 = 0;
		int num7 = 0;
		for (int j = 0; j < num; j += 3)
		{
			num6++;
			if (useSleep && num6 > 5000)
			{
				num6 = 0;
				Thread.Sleep(0);
			}
			int num8 = triangles[j];
			int num9 = triangles[j + 1];
			int num10 = triangles[j + 2];
			if (vertexDirty == null || vertexDirty[num8] || vertexDirty[num9] || vertexDirty[num10])
			{
				float num11 = vertices[num9].x - vertices[num8].x;
				float num12 = vertices[num9].y - vertices[num8].y;
				float num13 = vertices[num9].z - vertices[num8].z;
				float num14 = vertices[num10].x - vertices[num8].x;
				float num15 = vertices[num10].y - vertices[num8].y;
				float num16 = vertices[num10].z - vertices[num8].z;
				surfaceNormals[num7].x = num12 * num16 - num13 * num15;
				surfaceNormals[num7].y = num13 * num14 - num11 * num16;
				surfaceNormals[num7].z = num11 * num15 - num12 * num14;
				if (normalizeSurfaceNormals)
				{
					float num17 = Mathf.Sqrt(surfaceNormals[num7].x * surfaceNormals[num7].x + surfaceNormals[num7].y * surfaceNormals[num7].y + surfaceNormals[num7].z * surfaceNormals[num7].z);
					float num18 = 1f / num17;
					surfaceNormals[num7].x *= num18;
					surfaceNormals[num7].y *= num18;
					surfaceNormals[num7].z *= num18;
				}
				Vector3 vector = surfaceNormals[num7];
				if (vertexDirty == null || vertexDirty[num8])
				{
					if (!markerArray[num8])
					{
						markerArray[num8] = true;
						normals[num8].x = vector.x;
						normals[num8].y = vector.y;
						normals[num8].z = vector.z;
					}
					else
					{
						normals[num8].x += vector.x;
						normals[num8].y += vector.y;
						normals[num8].z += vector.z;
					}
				}
				if (vertexDirty == null || vertexDirty[num9])
				{
					if (!markerArray[num9])
					{
						markerArray[num9] = true;
						normals[num9].x = vector.x;
						normals[num9].y = vector.y;
						normals[num9].z = vector.z;
					}
					else
					{
						normals[num9].x += vector.x;
						normals[num9].y += vector.y;
						normals[num9].z += vector.z;
					}
				}
				if (vertexDirty == null || vertexDirty[num10])
				{
					if (!markerArray[num10])
					{
						markerArray[num10] = true;
						normals[num10].x = vector.x;
						normals[num10].y = vector.y;
						normals[num10].z = vector.z;
					}
					else
					{
						normals[num10].x += vector.x;
						normals[num10].y += vector.y;
						normals[num10].z += vector.z;
					}
				}
			}
			num7++;
		}
		for (int k = 0; k < num2; k++)
		{
			num6++;
			if (useSleep && num6 > 5000)
			{
				num6 = 0;
				Thread.Sleep(0);
			}
			if (markerArray[k])
			{
				float num19 = Mathf.Sqrt(normals[k].x * normals[k].x + normals[k].y * normals[k].y + normals[k].z * normals[k].z);
				float num20 = 1f / num19;
				normals[k].x *= num20;
				normals[k].y *= num20;
				normals[k].z *= num20;
				markerArray[k] = false;
				if (vertexDirty != null)
				{
					vertexDirty[k] = false;
				}
			}
		}
	}

	public void recalculateNormals(int[] trianglesToUse, int[] verticesToUse)
	{
		recalculateNormals(triangles, vertices, normals, surfaceNormals, trianglesToUse, verticesToUse, thisMarkerArray);
	}

	public static void recalculateNormals(int[] triangles, Vector3[] vertices, Vector3[] normals, Vector3[] surfaceNormals, int[] trianglesToUse, int[] verticesToUse, bool[] markerArray = null, bool normalizeSurfaceNormals = true)
	{
		int num = vertices.Length;
		if (markerArray == null)
		{
			if (reusableVertexMarkerArray == null || reusableVertexMarkerArray.Length < num)
			{
				reusableVertexMarkerArray = new bool[num];
			}
			markerArray = reusableVertexMarkerArray;
		}
		foreach (int num2 in trianglesToUse)
		{
			if (num2 < triangles.Length)
			{
				int num3 = num2 / 3;
				int num4 = triangles[num2];
				int num5 = triangles[num2 + 1];
				int num6 = triangles[num2 + 2];
				float num7 = vertices[num5].x - vertices[num4].x;
				float num8 = vertices[num5].y - vertices[num4].y;
				float num9 = vertices[num5].z - vertices[num4].z;
				float num10 = vertices[num6].x - vertices[num4].x;
				float num11 = vertices[num6].y - vertices[num4].y;
				float num12 = vertices[num6].z - vertices[num4].z;
				surfaceNormals[num3].x = num8 * num12 - num9 * num11;
				surfaceNormals[num3].y = num9 * num10 - num7 * num12;
				surfaceNormals[num3].z = num7 * num11 - num8 * num10;
				if (normalizeSurfaceNormals)
				{
					float num13 = Mathf.Sqrt(surfaceNormals[num3].x * surfaceNormals[num3].x + surfaceNormals[num3].y * surfaceNormals[num3].y + surfaceNormals[num3].z * surfaceNormals[num3].z);
					float num14 = 1f / num13;
					surfaceNormals[num3].x *= num14;
					surfaceNormals[num3].y *= num14;
					surfaceNormals[num3].z *= num14;
				}
				Vector3 vector = surfaceNormals[num3];
				if (!markerArray[num4])
				{
					markerArray[num4] = true;
					normals[num4].x = vector.x;
					normals[num4].y = vector.y;
					normals[num4].z = vector.z;
				}
				else
				{
					normals[num4].x += vector.x;
					normals[num4].y += vector.y;
					normals[num4].z += vector.z;
				}
				if (!markerArray[num5])
				{
					markerArray[num5] = true;
					normals[num5].x = vector.x;
					normals[num5].y = vector.y;
					normals[num5].z = vector.z;
				}
				else
				{
					normals[num5].x += vector.x;
					normals[num5].y += vector.y;
					normals[num5].z += vector.z;
				}
				if (!markerArray[num6])
				{
					markerArray[num6] = true;
					normals[num6].x = vector.x;
					normals[num6].y = vector.y;
					normals[num6].z = vector.z;
				}
				else
				{
					normals[num6].x += vector.x;
					normals[num6].y += vector.y;
					normals[num6].z += vector.z;
				}
			}
		}
		foreach (int num15 in verticesToUse)
		{
			if (num15 < markerArray.Length && markerArray[num15])
			{
				float num16 = Mathf.Sqrt(normals[num15].x * normals[num15].x + normals[num15].y * normals[num15].y + normals[num15].z * normals[num15].z);
				float num17 = 1f / num16;
				normals[num15].x *= num17;
				normals[num15].y *= num17;
				normals[num15].z *= num17;
				markerArray[num15] = false;
			}
		}
	}

	public static void recalculateNormals(Mesh mesh)
	{
		int[] array = mesh.triangles;
		Vector3[] array2 = mesh.vertices;
		Vector3[] array3 = mesh.normals;
		Vector3[] array4 = new Vector3[array.Length / 3];
		recalculateNormals(array, array2, array3, array4);
		mesh.normals = array3;
	}
}
