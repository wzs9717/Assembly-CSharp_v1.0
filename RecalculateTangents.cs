using System.Threading;
using UnityEngine;

public class RecalculateTangents
{
	private int[] _triangles;

	private Vector3[] _vertices;

	private Vector3[] _normals;

	private Vector2[] _uv;

	private Vector4[] _tangents;

	private bool[] _thisMarkerArray;

	private bool _useSleep;

	private static bool[] reusableMarkerArray;

	public RecalculateTangents(int[] tris, Vector3[] verts, Vector3[] norms, Vector2[] uv, Vector4[] tangents, bool useSleep = false)
	{
		_triangles = tris;
		_vertices = verts;
		_normals = norms;
		_uv = uv;
		_tangents = tangents;
		_thisMarkerArray = new bool[_vertices.Length];
		_useSleep = useSleep;
	}

	public void recalculateTangents(bool[] vertexDirty = null)
	{
		recalculateTangentsFast(_triangles, _vertices, _normals, _uv, _tangents, vertexDirty, _thisMarkerArray, _useSleep);
	}

	public void recalculateTangents(int[] trianglesToUse, int[] verticesToUse)
	{
		recalculateTangentsFast(_triangles, _vertices, _normals, _uv, _tangents, trianglesToUse, verticesToUse, _thisMarkerArray);
	}

	public static void recalculateTangentsAccurate(int[] triangles, Vector3[] vertices, Vector3[] normals, Vector2[] uv, ref Vector3[] tan1, ref Vector3[] tan2, Vector4[] tangents)
	{
		int num = triangles.Length;
		int num2 = vertices.Length;
		if (tan1 == null)
		{
			tan1 = new Vector3[num2];
		}
		if (tan2 == null)
		{
			tan2 = new Vector3[num2];
		}
		for (int i = 0; i < num2; i++)
		{
			tan1[i].x = 0f;
			tan1[i].y = 0f;
			tan1[i].z = 0f;
			tan2[i].x = 0f;
			tan2[i].y = 0f;
			tan2[i].z = 0f;
		}
		for (int j = 0; j < num; j += 3)
		{
			int num3 = triangles[j];
			int num4 = triangles[j + 1];
			int num5 = triangles[j + 2];
			float num6 = vertices[num4].x - vertices[num3].x;
			float num7 = vertices[num4].y - vertices[num3].y;
			float num8 = vertices[num4].z - vertices[num3].z;
			float num9 = vertices[num5].x - vertices[num3].x;
			float num10 = vertices[num5].y - vertices[num3].y;
			float num11 = vertices[num5].z - vertices[num3].z;
			float num12 = uv[num4].x - uv[num3].x;
			float num13 = uv[num5].x - uv[num3].x;
			float num14 = uv[num4].y - uv[num3].y;
			float num15 = uv[num5].y - uv[num3].y;
			float num16 = 1f / (num12 * num15 - num13 * num14);
			float num17 = (num15 * num6 - num14 * num9) * num16;
			float num18 = (num15 * num7 - num14 * num10) * num16;
			float num19 = (num15 * num8 - num14 * num11) * num16;
			float num20 = (num12 * num9 - num13 * num6) * num16;
			float num21 = (num12 * num10 - num13 * num7) * num16;
			float num22 = (num12 * num11 - num13 * num8) * num16;
			tan1[num3].x += num17;
			tan1[num3].y += num18;
			tan1[num3].z += num19;
			tan1[num4].x += num17;
			tan1[num4].y += num18;
			tan1[num4].z += num19;
			tan1[num5].x += num17;
			tan1[num5].y += num18;
			tan1[num5].z += num19;
			tan2[num3].x += num20;
			tan2[num3].y += num21;
			tan2[num3].z += num22;
			tan2[num4].x += num20;
			tan2[num4].y += num21;
			tan2[num4].z += num22;
			tan2[num5].x += num20;
			tan2[num5].y += num21;
			tan2[num5].z += num22;
		}
		for (int k = 0; k < num2; k++)
		{
			Vector3 vector = normals[k];
			Vector3 vector2 = tan1[k];
			Vector3 normalized = (vector2 - vector * Vector3.Dot(vector, vector2)).normalized;
			tangents[k].x = normalized.x;
			tangents[k].y = normalized.y;
			tangents[k].z = normalized.z;
			tangents[k].w = ((!(Vector3.Dot(Vector3.Cross(vector, vector2), tan2[k]) < 0f)) ? 1f : (-1f));
		}
	}

	public static void recalculateTangentsFast(int[] triangles, Vector3[] vertices, Vector3[] normals, Vector2[] uv, Vector4[] tangents, bool[] vertexDirty = null, bool[] markerArray = null, bool useSleep = false)
	{
		int num = triangles.Length;
		int num2 = vertices.Length;
		if (markerArray == null)
		{
			if (reusableMarkerArray == null || reusableMarkerArray.Length < num2)
			{
				reusableMarkerArray = new bool[num2];
			}
			markerArray = reusableMarkerArray;
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
		for (int j = 0; j < num; j += 3)
		{
			num6++;
			if (useSleep && num6 > 5000)
			{
				num6 = 0;
				Thread.Sleep(0);
			}
			int num7 = triangles[j];
			int num8 = triangles[j + 1];
			int num9 = triangles[j + 2];
			if (vertexDirty != null && !vertexDirty[num7] && !vertexDirty[num8] && !vertexDirty[num9])
			{
				continue;
			}
			float num10 = vertices[num8].x - vertices[num7].x;
			float num11 = vertices[num8].y - vertices[num7].y;
			float num12 = vertices[num8].z - vertices[num7].z;
			float num13 = vertices[num9].x - vertices[num7].x;
			float num14 = vertices[num9].y - vertices[num7].y;
			float num15 = vertices[num9].z - vertices[num7].z;
			float num16 = uv[num8].x - uv[num7].x;
			float num17 = uv[num9].x - uv[num7].x;
			float num18 = uv[num8].y - uv[num7].y;
			float num19 = uv[num9].y - uv[num7].y;
			float num20 = 1f / (num16 * num19 - num17 * num18);
			float num21 = (num19 * num10 - num18 * num13) * num20;
			float num22 = (num19 * num11 - num18 * num14) * num20;
			float num23 = (num19 * num12 - num18 * num15) * num20;
			if (vertexDirty == null || vertexDirty[num7])
			{
				if (!markerArray[num7])
				{
					markerArray[num7] = true;
					tangents[num7].x = num21;
					tangents[num7].y = num22;
					tangents[num7].z = num23;
				}
				else
				{
					tangents[num7].x += num21;
					tangents[num7].y += num22;
					tangents[num7].z += num23;
				}
			}
			if (vertexDirty == null || vertexDirty[num8])
			{
				if (!markerArray[num8])
				{
					markerArray[num8] = true;
					tangents[num8].x = num21;
					tangents[num8].y = num22;
					tangents[num8].z = num23;
				}
				else
				{
					tangents[num8].x += num21;
					tangents[num8].y += num22;
					tangents[num8].z += num23;
				}
			}
			if (vertexDirty == null || vertexDirty[num9])
			{
				if (!markerArray[num9])
				{
					markerArray[num9] = true;
					tangents[num9].x = num21;
					tangents[num9].y = num22;
					tangents[num9].z = num23;
				}
				else
				{
					tangents[num9].x += num21;
					tangents[num9].y += num22;
					tangents[num9].z += num23;
				}
			}
		}
		Vector3 vector = default(Vector3);
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
				float num24 = normals[k].x * tangents[k].x + normals[k].y * tangents[k].y + normals[k].z * tangents[k].z;
				vector.x = tangents[k].x - normals[k].x * num24;
				vector.y = tangents[k].y - normals[k].y * num24;
				vector.z = tangents[k].z - normals[k].z * num24;
				float num25 = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
				float num26 = 1f / num25;
				tangents[k].x = vector.x * num26;
				tangents[k].y = vector.y * num26;
				tangents[k].z = vector.z * num26;
				tangents[k].w = -1f;
				markerArray[k] = false;
				if (vertexDirty != null)
				{
					vertexDirty[k] = false;
				}
			}
		}
	}

	public static void recalculateTangentsFast(int[] triangles, Vector3[] vertices, Vector3[] normals, Vector2[] uv, Vector4[] tangents, int[] trianglesToUse, int[] verticesToUse, bool[] markerArray = null)
	{
		int num = vertices.Length;
		if (markerArray == null)
		{
			if (reusableMarkerArray == null || reusableMarkerArray.Length < num)
			{
				reusableMarkerArray = new bool[num];
			}
			markerArray = reusableMarkerArray;
		}
		foreach (int num2 in trianglesToUse)
		{
			int num3 = triangles[num2];
			int num4 = triangles[num2 + 1];
			int num5 = triangles[num2 + 2];
			float num6 = vertices[num4].x - vertices[num3].x;
			float num7 = vertices[num4].y - vertices[num3].y;
			float num8 = vertices[num4].z - vertices[num3].z;
			float num9 = vertices[num5].x - vertices[num3].x;
			float num10 = vertices[num5].y - vertices[num3].y;
			float num11 = vertices[num5].z - vertices[num3].z;
			float num12 = uv[num4].x - uv[num3].x;
			float num13 = uv[num5].x - uv[num3].x;
			float num14 = uv[num4].y - uv[num3].y;
			float num15 = uv[num5].y - uv[num3].y;
			float num16 = 1f / (num12 * num15 - num13 * num14);
			float num17 = (num15 * num6 - num14 * num9) * num16;
			float num18 = (num15 * num7 - num14 * num10) * num16;
			float num19 = (num15 * num8 - num14 * num11) * num16;
			if (!markerArray[num3])
			{
				markerArray[num3] = true;
				tangents[num3].x = num17;
				tangents[num3].y = num18;
				tangents[num3].z = num19;
			}
			else
			{
				tangents[num3].x += num17;
				tangents[num3].y += num18;
				tangents[num3].z += num19;
			}
			if (!markerArray[num4])
			{
				markerArray[num4] = true;
				tangents[num4].x = num17;
				tangents[num4].y = num18;
				tangents[num4].z = num19;
			}
			else
			{
				tangents[num4].x += num17;
				tangents[num4].y += num18;
				tangents[num4].z += num19;
			}
			if (!markerArray[num5])
			{
				markerArray[num5] = true;
				tangents[num5].x = num17;
				tangents[num5].y = num18;
				tangents[num5].z = num19;
			}
			else
			{
				tangents[num5].x += num17;
				tangents[num5].y += num18;
				tangents[num5].z += num19;
			}
		}
		Vector3 vector = default(Vector3);
		foreach (int num20 in verticesToUse)
		{
			float num21 = normals[num20].x * tangents[num20].x + normals[num20].y * tangents[num20].y + normals[num20].z * tangents[num20].z;
			vector.x = tangents[num20].x - normals[num20].x * num21;
			vector.y = tangents[num20].y - normals[num20].y * num21;
			vector.z = tangents[num20].z - normals[num20].z * num21;
			float num22 = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
			float num23 = 1f / num22;
			tangents[num20].x = vector.x * num23;
			tangents[num20].y = vector.y * num23;
			tangents[num20].z = vector.z * num23;
			tangents[num20].w = -1f;
			markerArray[num20] = false;
		}
	}

	public static void recalculateTangents(Mesh mesh)
	{
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
		int num = vertices.Length;
		Vector4[] tangents = new Vector4[num];
		Vector3[] tan = null;
		Vector3[] tan2 = null;
		recalculateTangentsAccurate(triangles, vertices, normals, uv, ref tan, ref tan2, tangents);
		mesh.tangents = tangents;
	}
}
