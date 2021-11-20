using UnityEngine;

public class RecalculateNormalsGPU
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

	protected int _recalcSurfaceNormalsKernel;

	protected int _recalcVertexNormalsKernel;

	protected ComputeBuffer _normalsBuffer;

	protected ComputeBuffer trianglesBuffer;

	protected ComputeBuffer _surfaceNormalsBuffer;

	protected ComputeBuffer vertTrianglesBuffer;

	protected int numTriangleThreadGroups;

	protected int numVertThreadGroups;

	protected MapVerticesGPU mapVerticesGPU;

	public ComputeBuffer normalsBuffer => _normalsBuffer;

	public ComputeBuffer surfaceNormalsBuffer => _surfaceNormalsBuffer;

	public RecalculateNormalsGPU(ComputeShader cs, int[] tris, int numVertices, VertexMap[] vertexMap = null)
	{
		_recalcSurfaceNormalsKernel = cs.FindKernel("RecalcSurfaceNormals");
		_recalcVertexNormalsKernel = cs.FindKernel("RecalcVertexNormals");
		if (_recalcVertexNormalsKernel != -1 && _recalcSurfaceNormalsKernel != -1)
		{
			computeShader = cs;
			numVertThreadGroups = numVertices / 256;
			if (numVertices % 256 != 0)
			{
				numVertThreadGroups++;
			}
			int num = numVertThreadGroups * 256;
			vertTrianglesStruct = new VertTriangles[num];
			for (int i = 0; i < num; i++)
			{
				vertTrianglesStruct[i] = default(VertTriangles);
			}
			int num2 = tris.Length / 3;
			numTriangleThreadGroups = num2 / 256;
			if (num2 % 256 != 0)
			{
				numTriangleThreadGroups++;
			}
			int num3 = numTriangleThreadGroups * 256;
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
			_normalsBuffer = new ComputeBuffer(num, 12);
			_surfaceNormalsBuffer = new ComputeBuffer(num3, 12);
			vertTrianglesBuffer = new ComputeBuffer(num, 48);
			vertTrianglesBuffer.SetData(vertTrianglesStruct);
			if (vertexMap != null)
			{
				mapVerticesGPU = new MapVerticesGPU(computeShader, vertexMap);
			}
		}
		else
		{
			Debug.LogError("Compute Shader does not have RecalcSurfaceNormals or RecalcVertexNormals kernel");
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

	public void RecalculateNormals(ComputeBuffer inVertBuffer)
	{
		if (computeShader != null)
		{
			computeShader.SetBuffer(_recalcSurfaceNormalsKernel, "inVerts", inVertBuffer);
			computeShader.SetBuffer(_recalcSurfaceNormalsKernel, "triangles", trianglesBuffer);
			computeShader.SetBuffer(_recalcSurfaceNormalsKernel, "surfaceNormals", _surfaceNormalsBuffer);
			computeShader.Dispatch(_recalcSurfaceNormalsKernel, numTriangleThreadGroups, 1, 1);
			computeShader.SetBuffer(_recalcVertexNormalsKernel, "vertTriangles", vertTrianglesBuffer);
			computeShader.SetBuffer(_recalcVertexNormalsKernel, "surfaceNormals", _surfaceNormalsBuffer);
			computeShader.SetBuffer(_recalcVertexNormalsKernel, "normals", _normalsBuffer);
			computeShader.Dispatch(_recalcVertexNormalsKernel, numVertThreadGroups, 1, 1);
			if (mapVerticesGPU != null)
			{
				mapVerticesGPU.Map(_normalsBuffer);
			}
		}
	}

	public void Release()
	{
		if (mapVerticesGPU != null)
		{
			mapVerticesGPU.Release();
			mapVerticesGPU = null;
		}
		if (trianglesBuffer != null)
		{
			trianglesBuffer.Release();
			trianglesBuffer = null;
		}
		if (_normalsBuffer != null)
		{
			_normalsBuffer.Release();
			_normalsBuffer = null;
		}
		if (_surfaceNormalsBuffer != null)
		{
			_surfaceNormalsBuffer.Release();
			_surfaceNormalsBuffer = null;
		}
		if (vertTrianglesBuffer != null)
		{
			vertTrianglesBuffer.Release();
			vertTrianglesBuffer = null;
		}
	}
}
