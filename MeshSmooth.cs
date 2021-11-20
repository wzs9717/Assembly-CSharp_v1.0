using System.Collections.Generic;
using UnityEngine;

public class MeshSmooth
{
	private int numVertices;

	private int[][] vertexNeighbors;

	private float[][] vertexNeighborDistances;

	private Vector3[] diffs;

	public MeshSmooth(Vector3[] baseVerts, MeshPoly[] basePolys)
	{
		numVertices = baseVerts.Length;
		diffs = new Vector3[numVertices];
		vertexNeighbors = new int[numVertices][];
		vertexNeighborDistances = new float[numVertices][];
		Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
		for (int i = 0; i < basePolys.Length; i++)
		{
			for (int j = 0; j < basePolys[i].vertices.Length; j++)
			{
				int key = basePolys[i].vertices[j];
				if (!dictionary.TryGetValue(key, out var value))
				{
					value = new List<int>();
					dictionary.Add(key, value);
				}
				value.Add(i);
			}
		}
		for (int k = 0; k < numVertices; k++)
		{
			Dictionary<int, bool> dictionary2 = new Dictionary<int, bool>();
			if (dictionary.TryGetValue(k, out var value2))
			{
				foreach (int item in value2)
				{
					for (int l = 0; l < basePolys[item].vertices.Length; l++)
					{
						int num = basePolys[item].vertices[l];
						if (num != k && !dictionary2.ContainsKey(num))
						{
							dictionary2.Add(num, value: true);
						}
					}
				}
			}
			vertexNeighbors[k] = new int[dictionary2.Count];
			vertexNeighborDistances[k] = new float[dictionary2.Count];
			int num2 = 0;
			foreach (int key2 in dictionary2.Keys)
			{
				vertexNeighbors[k][num2] = key2;
				Vector3 vector = baseVerts[k] - baseVerts[key2];
				float num3 = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
				vertexNeighborDistances[k][num2] = num3;
				num2++;
			}
		}
	}

	public MeshSmooth(Vector3[] baseVerts, int[] baseTriangles)
	{
		vertexNeighbors = new int[baseVerts.Length][];
		for (int i = 0; i < baseVerts.Length; i++)
		{
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			for (int j = 0; j < baseTriangles.Length; j += 3)
			{
				if (baseTriangles[j] == i || baseTriangles[j + 1] == i || baseTriangles[j + 2] == i)
				{
					if (baseTriangles[j] != i && !dictionary.ContainsKey(baseTriangles[j]))
					{
						dictionary.Add(baseTriangles[j], value: true);
					}
					if (baseTriangles[j + 1] != i && !dictionary.ContainsKey(baseTriangles[j + 1]))
					{
						dictionary.Add(baseTriangles[j + 1], value: true);
					}
					if (baseTriangles[j + 2] != i && !dictionary.ContainsKey(baseTriangles[j + 2]))
					{
						dictionary.Add(baseTriangles[j + 2], value: true);
					}
				}
			}
			vertexNeighbors[i] = new int[dictionary.Count];
			int num = 0;
			foreach (int key in dictionary.Keys)
			{
				vertexNeighbors[i][num] = key;
				num++;
			}
		}
	}

	public void LaplacianSmooth(Vector3[] inVerts, Vector3[] outVerts, int startIndex = 0, int stopIndex = 100000000)
	{
		int num = numVertices - 1;
		if (num > stopIndex)
		{
			num = stopIndex;
		}
		for (int i = startIndex; i <= num; i++)
		{
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int[] array = vertexNeighbors[i];
			int num5 = array.Length;
			if (num5 > 0)
			{
				for (int j = 0; j < num5; j++)
				{
					int num6 = array[j];
					num2 += inVerts[num6].x;
					num3 += inVerts[num6].y;
					num4 += inVerts[num6].z;
				}
				float num7 = 1f / (float)num5;
				outVerts[i].x = num2 * num7;
				outVerts[i].y = num3 * num7;
				outVerts[i].z = num4 * num7;
			}
		}
	}

	public void HCCorrection(Vector3[] inVerts, Vector3[] outVerts, float beta, int startIndex = 0, int stopIndex = 1000000000)
	{
		int num = numVertices - 1;
		if (num > stopIndex)
		{
			num = stopIndex;
		}
		for (int i = startIndex; i <= num; i++)
		{
			diffs[i].x = outVerts[i].x - inVerts[i].x;
			diffs[i].y = outVerts[i].y - inVerts[i].y;
			diffs[i].z = outVerts[i].z - inVerts[i].z;
		}
		for (int j = startIndex; j <= num; j++)
		{
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int[] array = vertexNeighbors[j];
			int num5 = array.Length;
			if (num5 > 0)
			{
				for (int k = 0; k < num5; k++)
				{
					int num6 = array[k];
					num2 += diffs[num6].x;
					num3 += diffs[num6].y;
					num4 += diffs[num6].z;
				}
				float num7 = (1f - beta) / (float)num5;
				outVerts[j].x -= beta * diffs[j].x + num2 * num7;
				outVerts[j].y -= beta * diffs[j].y + num3 * num7;
				outVerts[j].z -= beta * diffs[j].z + num4 * num7;
			}
		}
	}

	public void SpringSmooth(Vector3[] inVerts, Vector3[] outVerts, float springFactor, int startIndex = 0, int stopIndex = 100000000)
	{
		int num = numVertices - 1;
		if (num > stopIndex)
		{
			num = stopIndex;
		}
		Vector3 vector = default(Vector3);
		Vector3 vector3 = default(Vector3);
		for (int i = startIndex; i <= num; i++)
		{
			vector.x = 0f;
			vector.y = 0f;
			vector.z = 0f;
			int[] array = vertexNeighbors[i];
			float[] array2 = vertexNeighborDistances[i];
			int num2 = array.Length;
			if (num2 > 0)
			{
				for (int j = 0; j < num2; j++)
				{
					int num3 = array[j];
					float num4 = array2[j];
					Vector3 vector2 = inVerts[num3] - inVerts[i];
					float num5 = Mathf.Sqrt(vector2.x * vector2.x + vector2.y * vector2.y + vector2.z * vector2.z);
					if (num5 != 0f)
					{
						float num6 = 1f / num5;
						vector3.x = vector2.x * num6;
						vector3.y = vector2.y * num6;
						vector3.z = vector2.z * num6;
						vector += vector3 * (num5 - num4) * springFactor;
					}
				}
			}
			ref Vector3 reference = ref outVerts[i];
			reference = inVerts[i] + vector;
		}
	}
}
