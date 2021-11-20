using UnityEngine;

public class MapVerticesGPU
{
	protected struct VertexMapGPU
	{
		public int fromvert;

		public int tovert;
	}

	protected int mappingGroupSize = 256;

	protected int numMappingGroups;

	protected ComputeShader computeShader;

	protected int mappingKernel;

	protected ComputeBuffer mappingBuffer;

	protected string _mappingBufferName;

	protected VertexMapGPU[] vertexMapping;

	public MapVerticesGPU(ComputeShader cs, VertexMap[] vertexMap)
	{
		mappingKernel = cs.FindKernel("MapVertices");
		if (mappingKernel != -1)
		{
			computeShader = cs;
			int num = vertexMap.Length;
			if (num == 0)
			{
				numMappingGroups = 0;
				return;
			}
			numMappingGroups = num / mappingGroupSize;
			if (num % mappingGroupSize != 0)
			{
				numMappingGroups++;
			}
			int num2 = numMappingGroups * mappingGroupSize;
			mappingBuffer = new ComputeBuffer(num2, 8);
			vertexMapping = new VertexMapGPU[num2];
			for (int i = 0; i < num2; i++)
			{
				if (i < num)
				{
					VertexMap vertexMap2 = vertexMap[i];
					vertexMapping[i].fromvert = vertexMap2.fromvert;
					vertexMapping[i].tovert = vertexMap2.tovert;
				}
				else
				{
					vertexMapping[i].fromvert = 0;
					vertexMapping[i].tovert = 0;
				}
			}
			mappingBuffer.SetData(vertexMapping);
		}
		else
		{
			Debug.LogError("Compute Shader does not have MapVertices kernel");
		}
	}

	public void Map(ComputeBuffer vertsBuffer)
	{
		if (computeShader != null && numMappingGroups != 0)
		{
			computeShader.SetBuffer(mappingKernel, "vertexMapping", mappingBuffer);
			computeShader.SetBuffer(mappingKernel, "outVerts", vertsBuffer);
			computeShader.Dispatch(mappingKernel, numMappingGroups, 1, 1);
		}
	}

	public void Release()
	{
		if (mappingBuffer != null)
		{
			mappingBuffer.Release();
		}
	}
}
