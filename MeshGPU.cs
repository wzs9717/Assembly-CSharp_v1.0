using UnityEngine;

public class MeshGPU
{
	protected struct BaseVertToUVVert
	{
		public int baseVertex;

		public int UVVertex;
	}

	protected struct Triangle
	{
		public int vert1;

		public int vert2;

		public int vert3;
	}

	public ComputeShader GPUMeshCompute;

	protected Mesh mesh;

	protected Vector3[] _vertices;

	protected int[] _triangles;

	public bool castShadows = true;

	public bool receiveShadows = true;

	protected const int vertGroupSize = 256;

	protected const int baseVertToUVVertGroupSize = 256;

	protected int numVertThreadGroups;

	protected int numBaseVertToUVVertThreadGroups;

	protected ComputeBuffer _wrapVerticesBuffer;

	protected Triangle[] _skinToTrianglesStruct;

	protected ComputeBuffer _skinToTrianglesBuffer;

	protected BaseVertToUVVert[] baseVertToUVVerts;

	protected ComputeBuffer baseVertToUVVertsBuffer;

	protected int _smoothKernel;

	protected int _hcp1Kernel;

	protected int _hcp2Kernel;

	protected int _baseVertsToUVVertsKernel;

	protected int _recalcSurfaceNormalsKernel;

	protected int _recalcVertexNormalsKernel;

	protected int _baseNormalsToUVNormalsKernel;

	public bool showMaterials;

	public bool GPUuseSimpleMaterial;

	public Material GPUsimpleMaterial;

	public Material[] GPUmaterials;

	public bool[] materialsEnabled;

	[SerializeField]
	protected int _numMaterials;

	[SerializeField]
	protected string[] _materialNames;

	protected MeshSmoothGPU meshSmooth;

	protected RecalculateNormalsGPU recalcNormals;

	public ComputeBuffer smoothedVertsBuffer;

	protected ComputeBuffer _normalsBuffer;

	protected ComputeBuffer _surfaceNormalsBuffer;

	protected int _nullVertexIndex;

	public int numMaterials => _numMaterials;

	public string[] materialNames => _materialNames;

	public ComputeBuffer normalsBuffer => _normalsBuffer;

	public ComputeBuffer surfaceNormalsBuffer => _surfaceNormalsBuffer;

	public MeshGPU(ComputeShader cs, Mesh m, Vector3[] vertices, int[] triangles, VertexMap[] vertMap, MeshPoly[] polys = null)
	{
		mesh = m;
		meshSmooth = new MeshSmoothGPU(cs, vertices, polys);
		recalcNormals = new RecalculateNormalsGPU(cs, triangles, vertices.Length, vertMap);
		_surfaceNormalsBuffer = recalcNormals.surfaceNormalsBuffer;
		_normalsBuffer = recalcNormals.normalsBuffer;
		numVertThreadGroups = vertices.Length / 256;
		if (vertices.Length % 256 != 0)
		{
			numVertThreadGroups++;
		}
	}

	public void CopyMaterials(Material[] materials, string[] matNames = null, Material simpleMaterial = null, bool[] enabled = null)
	{
		_numMaterials = materials.Length;
		if (simpleMaterial != null)
		{
			GPUsimpleMaterial = simpleMaterial;
		}
		GPUmaterials = new Material[_numMaterials];
		materialsEnabled = new bool[_numMaterials];
		_materialNames = new string[_numMaterials];
		for (int i = 0; i < _numMaterials; i++)
		{
			GPUmaterials[i] = materials[i];
			if (enabled != null)
			{
				materialsEnabled[i] = enabled[i];
			}
			if (matNames != null)
			{
				_materialNames[i] = matNames[i];
			}
		}
	}

	public void VerticesUpdated(ComputeBuffer vertsBuffer)
	{
		recalcNormals.RecalculateNormals(vertsBuffer);
	}

	public void Draw(ComputeBuffer vertsBuffer)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		for (int i = 0; i < _numMaterials; i++)
		{
			if (GPUuseSimpleMaterial && (bool)GPUsimpleMaterial)
			{
				GPUsimpleMaterial.SetBuffer("verts", vertsBuffer);
				GPUsimpleMaterial.SetBuffer("normals", normalsBuffer);
				Graphics.DrawMesh(mesh, identity, GPUsimpleMaterial, 0, null, i, null, castShadows, receiveShadows);
			}
			else if (materialsEnabled[i] && GPUmaterials[i] != null)
			{
				GPUmaterials[i].SetBuffer("verts", smoothedVertsBuffer);
				GPUmaterials[i].SetBuffer("normals", normalsBuffer);
				Graphics.DrawMesh(mesh, identity, GPUmaterials[i], 0, null, i, null, castShadows, receiveShadows);
			}
		}
	}
}
