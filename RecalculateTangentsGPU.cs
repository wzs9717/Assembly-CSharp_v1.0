using UnityEngine;

public class RecalculateTangentsGPU
{
	protected struct Triangle
	{
		public int vert1;

		public int vert2;

		public int vert3;
	}

	protected struct VertTriangles
	{
		public int triangle0;

		public int triangle1;

		public int triangle2;

		public int triangle3;

		public int triangle4;

		public int triangle5;

		public float triangle0factor;

		public float triangle1factor;

		public float triangle2factor;

		public float triangle3factor;

		public float triangle4factor;

		public float triangle5factor;
	}

	protected const int maxNumTriangles = 6;

	protected const int triangleGroupSize = 256;

	protected const int vertGroupSize = 256;

	protected Triangle[] trianglesStruct;

	protected VertTriangles[] vertTrianglesStruct;

	protected ComputeShader computeShader;

	protected int _recalcTriangleTangentDirsKernel;

	protected int _recalcVertexTangentsKernel;

	protected ComputeBuffer _tangentsBuffer;

	protected ComputeBuffer _uvBuffer;

	protected ComputeBuffer _triangleTangentDirsBuffer;

	protected ComputeBuffer trianglesBuffer;

	protected ComputeBuffer vertTrianglesBuffer;

	protected int numTriangleThreadGroups;

	protected int numVertThreadGroups;

	public ComputeBuffer tangentsBuffer => _tangentsBuffer;

	public RecalculateTangentsGPU(ComputeShader cs, int[] tris, Vector2[] uvs, int numVertices)
	{
		_recalcVertexTangentsKernel = cs.FindKernel("RecalcVertexTangents");
		_recalcTriangleTangentDirsKernel = cs.FindKernel("RecalcTriangleTangentDirs");
		if (_recalcVertexTangentsKernel != -1 && _recalcTriangleTangentDirsKernel != -1)
		{
			computeShader = cs;
			numVertThreadGroups = numVertices / 256;
			if (numVertices % 256 != 0)
			{
				numVertThreadGroups++;
			}
			int num = numVertThreadGroups * 256;
			int num2 = tris.Length / 3;
			numTriangleThreadGroups = num2 / 256;
			if (num2 % 256 != 0)
			{
				numTriangleThreadGroups++;
			}
			int num3 = numTriangleThreadGroups * 256;
			vertTrianglesStruct = new VertTriangles[num];
			for (int i = 0; i < num; i++)
			{
				vertTrianglesStruct[i] = default(VertTriangles);
			}
			trianglesStruct = new Triangle[num3];
			int[] array = new int[numVertices];
			for (int j = 0; j < num3; j++)
			{
				if (j < num2)
				{
					int num4 = j * 3;
					trianglesStruct[j] = default(Triangle);
					int num5 = tris[num4];
					SetVertexTriangle(array[num5], num5, j);
					array[num5]++;
					trianglesStruct[j].vert1 = num5;
					int num6 = tris[num4 + 1];
					SetVertexTriangle(array[num6], num6, j);
					array[num6]++;
					trianglesStruct[j].vert2 = num6;
					int num7 = tris[num4 + 2];
					SetVertexTriangle(array[num7], num7, j);
					array[num7]++;
					trianglesStruct[j].vert3 = num7;
				}
				else
				{
					trianglesStruct[j] = default(Triangle);
					trianglesStruct[j].vert1 = numVertices;
					trianglesStruct[j].vert2 = numVertices;
					trianglesStruct[j].vert3 = numVertices;
				}
			}
			trianglesBuffer = new ComputeBuffer(num3, 12);
			trianglesBuffer.SetData(trianglesStruct);
			vertTrianglesBuffer = new ComputeBuffer(num, 48);
			vertTrianglesBuffer.SetData(vertTrianglesStruct);
			_uvBuffer = new ComputeBuffer(num, 8);
			_uvBuffer.SetData(uvs);
			_tangentsBuffer = new ComputeBuffer(num, 16);
			_triangleTangentDirsBuffer = new ComputeBuffer(num3, 24);
		}
		else
		{
			Debug.LogError("Compute Shader does not have RecalcTangents* kernels");
		}
	}

	protected void SetVertexTriangle(int count, int vert, int triangle)
	{
		switch (count)
		{
		case 0:
			vertTrianglesStruct[vert].triangle0 = triangle;
			vertTrianglesStruct[vert].triangle0factor = 1f;
			break;
		case 1:
			vertTrianglesStruct[vert].triangle1 = triangle;
			vertTrianglesStruct[vert].triangle1factor = 1f;
			break;
		case 2:
			vertTrianglesStruct[vert].triangle2 = triangle;
			vertTrianglesStruct[vert].triangle2factor = 1f;
			break;
		case 3:
			vertTrianglesStruct[vert].triangle3 = triangle;
			vertTrianglesStruct[vert].triangle3factor = 1f;
			break;
		case 4:
			vertTrianglesStruct[vert].triangle4 = triangle;
			vertTrianglesStruct[vert].triangle4factor = 1f;
			break;
		case 5:
			vertTrianglesStruct[vert].triangle5 = triangle;
			vertTrianglesStruct[vert].triangle5factor = 1f;
			break;
		}
	}

	public void RecalculateTangents(ComputeBuffer inVertBuffer, ComputeBuffer inNormalBuffer)
	{
		if (computeShader != null)
		{
			computeShader.SetBuffer(_recalcTriangleTangentDirsKernel, "inVerts", inVertBuffer);
			computeShader.SetBuffer(_recalcTriangleTangentDirsKernel, "triangles", trianglesBuffer);
			computeShader.SetBuffer(_recalcTriangleTangentDirsKernel, "inUV", _uvBuffer);
			computeShader.SetBuffer(_recalcTriangleTangentDirsKernel, "triangleTangentDirs", _triangleTangentDirsBuffer);
			computeShader.Dispatch(_recalcTriangleTangentDirsKernel, numTriangleThreadGroups, 1, 1);
			computeShader.SetBuffer(_recalcVertexTangentsKernel, "vertTriangles", vertTrianglesBuffer);
			computeShader.SetBuffer(_recalcVertexTangentsKernel, "normals", inNormalBuffer);
			computeShader.SetBuffer(_recalcVertexTangentsKernel, "triangleTangentDirs", _triangleTangentDirsBuffer);
			computeShader.SetBuffer(_recalcVertexTangentsKernel, "tangents", _tangentsBuffer);
			computeShader.Dispatch(_recalcVertexTangentsKernel, numVertThreadGroups, 1, 1);
		}
	}

	public void Release()
	{
		if (trianglesBuffer != null)
		{
			trianglesBuffer.Release();
			trianglesBuffer = null;
		}
		if (_tangentsBuffer != null)
		{
			_tangentsBuffer.Release();
			_tangentsBuffer = null;
		}
		if (_triangleTangentDirsBuffer != null)
		{
			_triangleTangentDirsBuffer.Release();
			_triangleTangentDirsBuffer = null;
		}
		if (_uvBuffer != null)
		{
			_uvBuffer.Release();
			_uvBuffer = null;
		}
		if (vertTrianglesBuffer != null)
		{
			vertTrianglesBuffer.Release();
			vertTrianglesBuffer = null;
		}
	}
}
