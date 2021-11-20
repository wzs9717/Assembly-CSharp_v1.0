using System.Collections.Generic;
using UnityEngine;

public class MeshSmoothGPU
{
	protected struct VertNeighbors
	{
		public int numNeighbors;

		public int neighbor0;

		public int neighbor1;

		public int neighbor2;

		public int neighbor3;

		public int neighbor4;

		public int neighbor5;

		public int neighbor6;

		public int neighbor7;

		public int neighbor8;

		public int neighbor9;

		public int neighbor10;

		public int neighbor11;

		public int neighbor12;

		public int neighbor13;

		public int neighbor14;

		public int neighbor15;

		public float neighbor0factor;

		public float neighbor1factor;

		public float neighbor2factor;

		public float neighbor3factor;

		public float neighbor4factor;

		public float neighbor5factor;

		public float neighbor6factor;

		public float neighbor7factor;

		public float neighbor8factor;

		public float neighbor9factor;

		public float neighbor10factor;

		public float neighbor11factor;

		public float neighbor12factor;

		public float neighbor13factor;

		public float neighbor14factor;

		public float neighbor15factor;

		public float neighbor0distance;

		public float neighbor1distance;

		public float neighbor2distance;

		public float neighbor3distance;

		public float neighbor4distance;

		public float neighbor5distance;

		public float neighbor6distance;

		public float neighbor7distance;

		public float neighbor8distance;

		public float neighbor9distance;

		public float neighbor10distance;

		public float neighbor11distance;

		public float neighbor12distance;

		public float neighbor13distance;

		public float neighbor14distance;

		public float neighbor15distance;
	}

	protected const int maxNumNeighbors = 16;

	protected const int vertGroupSize = 256;

	protected int numVertThreadGroups;

	protected int[][] vertexNeighbors;

	protected VertNeighbors[] vertexNeighborsStruct;

	protected ComputeShader computeShader;

	protected int _smoothKernel;

	protected int _springSmoothKernel;

	protected int _springSmooth2Kernel;

	protected int _hc1Kernel;

	protected int _hc2Kernel;

	protected ComputeBuffer vertNeighborBuffer;

	protected ComputeBuffer vertDiffBuffer;

	protected Vector3[] diffs;

	public MeshSmoothGPU(ComputeShader cs, Vector3[] vertices, MeshPoly[] polys)
	{
		int num = vertices.Length;
		_smoothKernel = cs.FindKernel("LaplacianSmooth");
		_springSmoothKernel = cs.FindKernel("SpringSmooth");
		_springSmooth2Kernel = cs.FindKernel("SpringSmooth2");
		_hc1Kernel = cs.FindKernel("HCCorrectionP1");
		_hc2Kernel = cs.FindKernel("HCCorrectionP2");
		if (_smoothKernel != -1 && _hc1Kernel != -1 && _hc2Kernel != -1)
		{
			computeShader = cs;
			diffs = new Vector3[num];
			vertexNeighbors = new int[num][];
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			for (int i = 0; i < polys.Length; i++)
			{
				for (int j = 0; j < polys[i].vertices.Length; j++)
				{
					int key = polys[i].vertices[j];
					if (!dictionary.TryGetValue(key, out var value))
					{
						value = new List<int>();
						dictionary.Add(key, value);
					}
					value.Add(i);
				}
			}
			numVertThreadGroups = num / 256;
			if (num % 256 != 0)
			{
				numVertThreadGroups++;
			}
			int num2 = numVertThreadGroups * 256;
			vertexNeighborsStruct = new VertNeighbors[num2];
			for (int k = 0; k < num; k++)
			{
				Dictionary<int, bool> dictionary2 = new Dictionary<int, bool>();
				if (dictionary.TryGetValue(k, out var value2))
				{
					foreach (int item in value2)
					{
						for (int l = 0; l < polys[item].vertices.Length; l++)
						{
							int num3 = polys[item].vertices[l];
							if (num3 != k && !dictionary2.ContainsKey(num3))
							{
								dictionary2.Add(num3, value: true);
							}
						}
					}
				}
				vertexNeighbors[k] = new int[dictionary2.Count];
				vertexNeighborsStruct[k] = default(VertNeighbors);
				int count = dictionary2.Keys.Count;
				float num4;
				if (count > 16)
				{
					Debug.LogWarning("Vertex " + k + " has more neighbors " + count + " than the maximum " + 16);
					num4 = 0.0625f;
					vertexNeighborsStruct[k].numNeighbors = 16;
				}
				else
				{
					num4 = 1f / (float)count;
					vertexNeighborsStruct[k].numNeighbors = count;
				}
				int num5 = 0;
				vertexNeighborsStruct[k].neighbor0factor = 0f;
				vertexNeighborsStruct[k].neighbor1factor = 0f;
				vertexNeighborsStruct[k].neighbor2factor = 0f;
				vertexNeighborsStruct[k].neighbor3factor = 0f;
				vertexNeighborsStruct[k].neighbor4factor = 0f;
				vertexNeighborsStruct[k].neighbor5factor = 0f;
				vertexNeighborsStruct[k].neighbor6factor = 0f;
				vertexNeighborsStruct[k].neighbor7factor = 0f;
				vertexNeighborsStruct[k].neighbor8factor = 0f;
				vertexNeighborsStruct[k].neighbor9factor = 0f;
				vertexNeighborsStruct[k].neighbor10factor = 0f;
				vertexNeighborsStruct[k].neighbor11factor = 0f;
				vertexNeighborsStruct[k].neighbor12factor = 0f;
				vertexNeighborsStruct[k].neighbor13factor = 0f;
				vertexNeighborsStruct[k].neighbor14factor = 0f;
				vertexNeighborsStruct[k].neighbor15factor = 0f;
				vertexNeighborsStruct[k].neighbor0distance = 1f;
				vertexNeighborsStruct[k].neighbor1distance = 1f;
				vertexNeighborsStruct[k].neighbor2distance = 1f;
				vertexNeighborsStruct[k].neighbor3distance = 1f;
				vertexNeighborsStruct[k].neighbor4distance = 1f;
				vertexNeighborsStruct[k].neighbor5distance = 1f;
				vertexNeighborsStruct[k].neighbor6distance = 1f;
				vertexNeighborsStruct[k].neighbor7distance = 1f;
				vertexNeighborsStruct[k].neighbor8distance = 1f;
				vertexNeighborsStruct[k].neighbor9distance = 1f;
				vertexNeighborsStruct[k].neighbor10distance = 1f;
				vertexNeighborsStruct[k].neighbor11distance = 1f;
				vertexNeighborsStruct[k].neighbor12distance = 1f;
				vertexNeighborsStruct[k].neighbor13distance = 1f;
				vertexNeighborsStruct[k].neighbor14distance = 1f;
				vertexNeighborsStruct[k].neighbor15distance = 1f;
				foreach (int key2 in dictionary2.Keys)
				{
					vertexNeighbors[k][num5] = key2;
					switch (num5)
					{
					case 0:
						vertexNeighborsStruct[k].neighbor0 = key2;
						vertexNeighborsStruct[k].neighbor0distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor0factor = num4;
						break;
					case 1:
						vertexNeighborsStruct[k].neighbor1 = key2;
						vertexNeighborsStruct[k].neighbor1distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor1factor = num4;
						break;
					case 2:
						vertexNeighborsStruct[k].neighbor2 = key2;
						vertexNeighborsStruct[k].neighbor2distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor2factor = num4;
						break;
					case 3:
						vertexNeighborsStruct[k].neighbor3 = key2;
						vertexNeighborsStruct[k].neighbor3distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor3factor = num4;
						break;
					case 4:
						vertexNeighborsStruct[k].neighbor4 = key2;
						vertexNeighborsStruct[k].neighbor4distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor4factor = num4;
						break;
					case 5:
						vertexNeighborsStruct[k].neighbor5 = key2;
						vertexNeighborsStruct[k].neighbor5distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor5factor = num4;
						break;
					case 6:
						vertexNeighborsStruct[k].neighbor6 = key2;
						vertexNeighborsStruct[k].neighbor6distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor6factor = num4;
						break;
					case 7:
						vertexNeighborsStruct[k].neighbor7 = key2;
						vertexNeighborsStruct[k].neighbor7distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor7factor = num4;
						break;
					case 8:
						vertexNeighborsStruct[k].neighbor8 = key2;
						vertexNeighborsStruct[k].neighbor8distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor8factor = num4;
						break;
					case 9:
						vertexNeighborsStruct[k].neighbor9 = key2;
						vertexNeighborsStruct[k].neighbor9distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor9factor = num4;
						break;
					case 10:
						vertexNeighborsStruct[k].neighbor10 = key2;
						vertexNeighborsStruct[k].neighbor10distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor10factor = num4;
						break;
					case 11:
						vertexNeighborsStruct[k].neighbor11 = key2;
						vertexNeighborsStruct[k].neighbor11distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor11factor = num4;
						break;
					case 12:
						vertexNeighborsStruct[k].neighbor12 = key2;
						vertexNeighborsStruct[k].neighbor12distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor12factor = num4;
						break;
					case 13:
						vertexNeighborsStruct[k].neighbor13 = key2;
						vertexNeighborsStruct[k].neighbor13distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor13factor = num4;
						break;
					case 14:
						vertexNeighborsStruct[k].neighbor14 = key2;
						vertexNeighborsStruct[k].neighbor14distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor14factor = num4;
						break;
					case 15:
						vertexNeighborsStruct[k].neighbor15 = key2;
						vertexNeighborsStruct[k].neighbor15distance = (vertices[key2] - vertices[k]).magnitude;
						vertexNeighborsStruct[k].neighbor15factor = num4;
						break;
					}
					num5++;
				}
			}
			for (int m = num; m < num2; m++)
			{
				vertexNeighborsStruct[m] = default(VertNeighbors);
				vertexNeighborsStruct[m].neighbor0 = m;
				vertexNeighborsStruct[m].neighbor0factor = 1f;
				vertexNeighborsStruct[m].neighbor1factor = 0f;
				vertexNeighborsStruct[m].neighbor2factor = 0f;
				vertexNeighborsStruct[m].neighbor3factor = 0f;
				vertexNeighborsStruct[m].neighbor4factor = 0f;
				vertexNeighborsStruct[m].neighbor5factor = 0f;
				vertexNeighborsStruct[m].neighbor6factor = 0f;
				vertexNeighborsStruct[m].neighbor7factor = 0f;
				vertexNeighborsStruct[m].neighbor8factor = 0f;
				vertexNeighborsStruct[m].neighbor9factor = 0f;
				vertexNeighborsStruct[m].neighbor10factor = 0f;
				vertexNeighborsStruct[m].neighbor11factor = 0f;
				vertexNeighborsStruct[m].neighbor12factor = 0f;
				vertexNeighborsStruct[m].neighbor13factor = 0f;
				vertexNeighborsStruct[m].neighbor14factor = 0f;
				vertexNeighborsStruct[m].neighbor15factor = 0f;
			}
			vertNeighborBuffer = new ComputeBuffer(num2, 196);
			vertNeighborBuffer.SetData(vertexNeighborsStruct);
			vertDiffBuffer = new ComputeBuffer(num2, 12);
		}
		else
		{
			Debug.LogError("Compute Shader does not have LaplacianSmooth or HCCorrectionP1 or HCCorrectionP2 kernel");
		}
	}

	public void LaplacianSmoothGPU(ComputeBuffer inVertBuffer, ComputeBuffer outVertBuffer)
	{
		if (computeShader != null)
		{
			computeShader.SetBuffer(_smoothKernel, "vertNeighbors", vertNeighborBuffer);
			computeShader.SetBuffer(_smoothKernel, "inVerts", inVertBuffer);
			computeShader.SetBuffer(_smoothKernel, "outVerts", outVertBuffer);
			computeShader.Dispatch(_smoothKernel, numVertThreadGroups, 1, 1);
		}
	}

	public void SpringSmoothGPU(ComputeBuffer inVertBuffer, ComputeBuffer outVertBuffer, float springFactor)
	{
		if (computeShader != null)
		{
			computeShader.SetBuffer(_springSmoothKernel, "vertNeighbors", vertNeighborBuffer);
			computeShader.SetBuffer(_springSmoothKernel, "inVerts", inVertBuffer);
			computeShader.SetBuffer(_springSmoothKernel, "outVerts", outVertBuffer);
			computeShader.SetFloat("springFactor", springFactor);
			computeShader.Dispatch(_springSmoothKernel, numVertThreadGroups, 1, 1);
		}
	}

	public void SpringSmooth2GPU(ComputeBuffer inVertBuffer, ComputeBuffer outVertBuffer, float springFactor)
	{
		if (computeShader != null)
		{
			computeShader.SetBuffer(_springSmooth2Kernel, "vertNeighbors", vertNeighborBuffer);
			computeShader.SetBuffer(_springSmooth2Kernel, "inVerts", inVertBuffer);
			computeShader.SetBuffer(_springSmooth2Kernel, "outVerts", outVertBuffer);
			computeShader.SetFloat("springFactor", springFactor);
			computeShader.Dispatch(_springSmooth2Kernel, numVertThreadGroups, 1, 1);
		}
	}

	public void HCCorrectionGPU(ComputeBuffer inVertBuffer, ComputeBuffer outVertBuffer, float hcCorrectionBeta = 0.5f)
	{
		if (computeShader != null)
		{
			computeShader.SetBuffer(_hc1Kernel, "inVerts", inVertBuffer);
			computeShader.SetBuffer(_hc1Kernel, "outVerts", outVertBuffer);
			computeShader.SetBuffer(_hc1Kernel, "smoothDiffs", vertDiffBuffer);
			computeShader.Dispatch(_hc1Kernel, numVertThreadGroups, 1, 1);
			computeShader.SetFloat("HCCorrectionBeta", hcCorrectionBeta);
			computeShader.SetBuffer(_hc2Kernel, "vertNeighbors", vertNeighborBuffer);
			computeShader.SetBuffer(_hc2Kernel, "smoothDiffs", vertDiffBuffer);
			computeShader.SetBuffer(_hc2Kernel, "outVerts", outVertBuffer);
			computeShader.Dispatch(_hc2Kernel, numVertThreadGroups, 1, 1);
		}
	}

	public void Release()
	{
		if (vertNeighborBuffer != null)
		{
			vertNeighborBuffer.Release();
			vertNeighborBuffer = null;
		}
		if (vertDiffBuffer != null)
		{
			vertDiffBuffer.Release();
			vertDiffBuffer = null;
		}
	}
}
