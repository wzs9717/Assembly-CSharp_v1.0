using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DAZSkinWrap : MonoBehaviour
{
	[Serializable]
	public struct SkinWrapVert
	{
		public int closestTriangle;

		public float surfaceNormalProjection;

		public float surfaceTangent1Projection;

		public float surfaceTangent2Projection;

		public float surfaceNormalWrapNormalDot;

		public float surfaceTangent1WrapNormalDot;

		public float surfaceTangent2WrapNormalDot;
	}

	protected struct Triangle
	{
		public int vert1;

		public int vert2;

		public int vert3;
	}

	public string wrapName;

	public bool draw;

	public DAZMesh dazMesh;

	public Transform skinTransform;

	public DAZSkinV2 skin;

	public float additionalThicknessMultiplier;

	public float surfaceOffset;

	public int smoothOuterLoops = 1;

	public int laplacianSmoothPasses = 1;

	public float laplacianSmoothBeta = 0.5f;

	public int springSmoothPasses = 1;

	public float springSmoothFactor = 0.2f;

	public bool useSpring2;

	public float spring2SmoothFactor = 1f;

	public bool moveToSurface = true;

	public float moveToSurfaceOffset;

	[SerializeField]
	protected SkinWrapVert[] _wrapVertices;

	protected Vector3[] _verts1;

	protected Vector3[] _verts2;

	protected Vector3[] _drawVerts;

	protected int _wrapProgress;

	[SerializeField]
	protected List<DAZMorph> _usedMorphs;

	[SerializeField]
	protected List<float> _usedMorphValues;

	protected MeshSmooth meshSmooth;

	protected MeshSmoothGPU meshSmoothGPU;

	protected Mesh mesh;

	protected bool meshWasInit;

	protected const int vertGroupSize = 256;

	protected int numVertThreadGroups;

	public ComputeShader GPUSkinWrapper;

	public ComputeShader GPUMeshCompute;

	protected ComputeBuffer _wrapVerticesBuffer;

	protected ComputeBuffer _verticesBuffer1;

	protected ComputeBuffer _verticesBuffer2;

	protected ComputeBuffer _drawVerticesBuffer;

	protected ComputeBuffer _delayedVertsBuffer;

	protected ComputeBuffer _delayedNormalsBuffer;

	protected ComputeBuffer _delayedTangentsBuffer;

	private Triangle[] _skinToTrianglesStruct;

	protected ComputeBuffer _skinToTrianglesBuffer;

	protected MapVerticesGPU mapVerticesGPU;

	protected int _skinWrapKernel;

	protected int _copyKernel;

	protected int _copyTangentsKernel;

	protected int _nullVertexIndex;

	protected int[] numSubsetVertThreadGroups;

	public bool showMaterials = true;

	public bool GPUuseSimpleMaterial;

	public Material GPUsimpleMaterial;

	public Material[] GPUmaterials;

	public bool GPUAutoSwapShader = true;

	public bool onlyUpdateEnabledMaterials = true;

	public bool[] materialsEnabled;

	[SerializeField]
	protected int _numMaterials;

	[SerializeField]
	protected string[] _materialNames;

	public bool recalculateNormals = true;

	public bool recalculateTangents;

	protected RecalculateNormalsGPU recalcNormalsGPU;

	protected RecalculateTangentsGPU recalcTangentsGPU;

	protected ComputeBuffer _normalsBuffer;

	protected ComputeBuffer _tangentsBuffer;

	protected ComputeBuffer _surfaceNormalsBuffer;

	protected float[] savedSurfaceNormalProjections;

	protected bool currentMoveToSurface;

	protected float currentMoveToSurfaceOffset;

	public SkinWrapVert[] wrapVertices => _wrapVertices;

	public int wrapProgress => _wrapProgress;

	public List<DAZMorph> usedMorphs => _usedMorphs;

	public List<float> usedMorphValue => _usedMorphValues;

	public int numMaterials => _numMaterials;

	public string[] materialNames => _materialNames;

	public void SetMeshMorphsFromUsedValue()
	{
		if (!(dazMesh != null) || !(dazMesh.morphBank != null))
		{
			return;
		}
		foreach (DAZMorph morph in dazMesh.morphBank.morphs)
		{
			morph.morphValue = 0f;
		}
		int num = 0;
		foreach (DAZMorph usedMorph in _usedMorphs)
		{
			Debug.Log("Set " + usedMorph.morphName + " to " + _usedMorphValues[num]);
			usedMorph.morphValue = _usedMorphValues[num];
			num++;
		}
	}

	public void Wrap()
	{
		if (!(dazMesh != null) || !(skin != null) || !(skin.dazMesh != null))
		{
			return;
		}
		_usedMorphs = new List<DAZMorph>();
		_usedMorphValues = new List<float>();
		if (dazMesh.morphBank != null)
		{
			foreach (DAZMorph morph in dazMesh.morphBank.morphs)
			{
				if (morph.morphValue != 0f)
				{
					_usedMorphs.Add(morph);
					_usedMorphValues.Add(morph.morphValue);
				}
			}
		}
		_wrapVertices = new SkinWrapVert[dazMesh.numUVVertices];
		Vector3[] morphedBaseVertices = dazMesh.morphedBaseVertices;
		Vector3[] morphedUVNormals = dazMesh.morphedUVNormals;
		Vector3[] baseSurfaceNormals = skin.dazMesh.baseSurfaceNormals;
		int[] baseTriangles = skin.dazMesh.baseTriangles;
		Vector3[] baseVertices = skin.dazMesh.baseVertices;
		for (int i = 0; i < dazMesh.numUVVertices; i++)
		{
			_wrapVertices[i] = default(SkinWrapVert);
		}
		for (int j = 0; j < dazMesh.numBaseVertices; j++)
		{
			int num = -1;
			float num2 = 10000f;
			int num3 = 0;
			for (int k = 0; k < baseTriangles.Length; k += 3)
			{
				int num4 = baseTriangles[k];
				int num5 = baseTriangles[k + 1];
				int num6 = baseTriangles[k + 2];
				Vector3 vector = (baseVertices[num4] + baseVertices[num5] + baseVertices[num6]) * 0.33333f;
				float magnitude = (vector - morphedBaseVertices[j]).magnitude;
				if (magnitude < num2)
				{
					num = num3;
					num2 = magnitude;
				}
				num3++;
			}
			SkinWrapVert skinWrapVert = _wrapVertices[j];
			skinWrapVert.closestTriangle = num;
			int num7 = num * 3;
			int num8 = baseTriangles[num7];
			int num9 = baseTriangles[num7 + 1];
			int num10 = baseTriangles[num7 + 2];
			Vector3 vector2 = (baseVertices[num8] + baseVertices[num9] + baseVertices[num10]) * 0.33333f;
			Vector3 vector3 = baseVertices[num8] - vector2;
			Vector3 rhs = baseSurfaceNormals[num];
			Vector3 rhs2 = Vector3.Cross(vector3, rhs);
			Vector3 lhs = morphedBaseVertices[j] - vector2;
			skinWrapVert.surfaceNormalProjection = Vector3.Dot(lhs, rhs) / rhs.sqrMagnitude;
			skinWrapVert.surfaceTangent1Projection = Vector3.Dot(lhs, vector3) / vector3.sqrMagnitude;
			skinWrapVert.surfaceTangent2Projection = Vector3.Dot(lhs, rhs2) / rhs2.sqrMagnitude;
			skinWrapVert.surfaceNormalWrapNormalDot = Vector3.Dot(morphedUVNormals[j], rhs);
			skinWrapVert.surfaceTangent1WrapNormalDot = Vector3.Dot(morphedUVNormals[j], vector3) / vector3.sqrMagnitude;
			skinWrapVert.surfaceTangent2WrapNormalDot = Vector3.Dot(morphedUVNormals[j], rhs2) / rhs2.sqrMagnitude;
			_wrapVertices[j] = skinWrapVert;
		}
		DAZVertexMap[] baseVerticesToUVVertices = dazMesh.baseVerticesToUVVertices;
		foreach (DAZVertexMap dAZVertexMap in baseVerticesToUVVertices)
		{
			_wrapVertices[dAZVertexMap.tovert].closestTriangle = _wrapVertices[dAZVertexMap.fromvert].closestTriangle;
			_wrapVertices[dAZVertexMap.tovert].surfaceNormalProjection = _wrapVertices[dAZVertexMap.fromvert].surfaceNormalProjection;
			_wrapVertices[dAZVertexMap.tovert].surfaceTangent1Projection = _wrapVertices[dAZVertexMap.fromvert].surfaceTangent1Projection;
			_wrapVertices[dAZVertexMap.tovert].surfaceTangent2Projection = _wrapVertices[dAZVertexMap.fromvert].surfaceTangent2Projection;
			_wrapVertices[dAZVertexMap.tovert].surfaceNormalWrapNormalDot = _wrapVertices[dAZVertexMap.fromvert].surfaceNormalWrapNormalDot;
			_wrapVertices[dAZVertexMap.tovert].surfaceTangent1WrapNormalDot = _wrapVertices[dAZVertexMap.fromvert].surfaceTangent1WrapNormalDot;
			_wrapVertices[dAZVertexMap.tovert].surfaceTangent2WrapNormalDot = _wrapVertices[dAZVertexMap.fromvert].surfaceTangent2WrapNormalDot;
		}
	}

	protected void InitSmoothing()
	{
		if (meshSmooth == null)
		{
			meshSmooth = new MeshSmooth(dazMesh.baseVertices, dazMesh.basePolyList);
		}
		if (meshSmoothGPU == null && GPUMeshCompute != null && Application.isPlaying)
		{
			meshSmoothGPU = new MeshSmoothGPU(GPUMeshCompute, dazMesh.baseVertices, dazMesh.basePolyList);
		}
	}

	public void InitMesh(bool force = false)
	{
		if (dazMesh != null && (force || !meshWasInit))
		{
			dazMesh.Init();
			meshWasInit = true;
			_verts1 = (Vector3[])dazMesh.morphedUVVertices.Clone();
			_verts2 = (Vector3[])dazMesh.morphedUVVertices.Clone();
			mesh = UnityEngine.Object.Instantiate(dazMesh.morphedUVMappedMesh);
			Bounds bounds = new Bounds(size: new Vector3(10000f, 10000f, 10000f), center: base.transform.position);
			mesh.bounds = bounds;
		}
	}

	public void CopyMaterials()
	{
		if (dazMesh != null)
		{
			_numMaterials = dazMesh.materials.Length;
			GPUsimpleMaterial = dazMesh.simpleMaterial;
			GPUmaterials = new Material[_numMaterials];
			materialsEnabled = new bool[_numMaterials];
			_materialNames = new string[_numMaterials];
			for (int i = 0; i < _numMaterials; i++)
			{
				GPUmaterials[i] = dazMesh.materials[i];
				materialsEnabled[i] = dazMesh.materialsEnabled[i];
				_materialNames[i] = dazMesh.materialNames[i];
			}
		}
	}

	protected void InitRecalcNormalsTangents()
	{
		if (recalculateNormals && recalcNormalsGPU == null)
		{
			recalcNormalsGPU = new RecalculateNormalsGPU(GPUMeshCompute, dazMesh.baseTriangles, dazMesh.numUVVertices, dazMesh.baseVerticesToUVVertices);
			_normalsBuffer = recalcNormalsGPU.normalsBuffer;
			_surfaceNormalsBuffer = recalcNormalsGPU.surfaceNormalsBuffer;
		}
		if (recalculateTangents && recalcTangentsGPU == null)
		{
			recalcTangentsGPU = new RecalculateTangentsGPU(GPUMeshCompute, dazMesh.UVTriangles, dazMesh.UV, dazMesh.numUVVertices);
			_tangentsBuffer = recalcTangentsGPU.tangentsBuffer;
		}
	}

	protected void GPURecheckSurfaceNormalOffsets()
	{
		if (currentMoveToSurface == moveToSurface && currentMoveToSurfaceOffset == moveToSurfaceOffset)
		{
			return;
		}
		if (moveToSurface)
		{
			for (int i = 0; i < dazMesh.numBaseVertices; i++)
			{
				if (_wrapVertices[i].surfaceNormalProjection < moveToSurfaceOffset)
				{
					_wrapVertices[i].surfaceNormalProjection = moveToSurfaceOffset;
				}
				else
				{
					_wrapVertices[i].surfaceNormalProjection = savedSurfaceNormalProjections[i];
				}
			}
		}
		else
		{
			for (int j = 0; j < dazMesh.numBaseVertices; j++)
			{
				_wrapVertices[j].surfaceNormalProjection = savedSurfaceNormalProjections[j];
			}
		}
		currentMoveToSurface = moveToSurface;
		currentMoveToSurfaceOffset = moveToSurfaceOffset;
	}

	protected void SkinWrapGPUInit()
	{
		if (_wrapVerticesBuffer != null)
		{
			return;
		}
		_skinWrapKernel = GPUSkinWrapper.FindKernel("SkinWrap");
		_copyKernel = GPUSkinWrapper.FindKernel("SkinWrapCopyVerts");
		_copyTangentsKernel = GPUMeshCompute.FindKernel("CopyTangents");
		int[] baseTriangles = skin.dazMesh.baseTriangles;
		_skinToTrianglesStruct = new Triangle[baseTriangles.Length];
		int num = 0;
		for (int i = 0; i < baseTriangles.Length; i += 3)
		{
			Triangle triangle = default(Triangle);
			triangle.vert1 = baseTriangles[i];
			triangle.vert2 = baseTriangles[i + 1];
			triangle.vert3 = baseTriangles[i + 2];
			_skinToTrianglesStruct[num] = triangle;
			num++;
		}
		_skinToTrianglesBuffer = new ComputeBuffer(num, 12);
		_skinToTrianglesBuffer.SetData(_skinToTrianglesStruct);
		int numUVVertices = dazMesh.numUVVertices;
		numVertThreadGroups = numUVVertices / 256;
		if (numUVVertices % 256 != 0)
		{
			numVertThreadGroups++;
		}
		savedSurfaceNormalProjections = new float[dazMesh.numBaseVertices];
		for (int j = 0; j < dazMesh.numBaseVertices; j++)
		{
			savedSurfaceNormalProjections[j] = _wrapVertices[j].surfaceNormalProjection;
		}
		GPURecheckSurfaceNormalOffsets();
		int count = numVertThreadGroups * 256;
		_wrapVerticesBuffer = new ComputeBuffer(count, 28);
		_wrapVerticesBuffer.SetData(_wrapVertices);
		_verticesBuffer1 = new ComputeBuffer(count, 12);
		_verticesBuffer2 = new ComputeBuffer(count, 12);
		_delayedVertsBuffer = new ComputeBuffer(count, 12);
		_delayedNormalsBuffer = new ComputeBuffer(count, 12);
		_delayedTangentsBuffer = new ComputeBuffer(count, 16);
		mapVerticesGPU = new MapVerticesGPU(GPUMeshCompute, dazMesh.baseVerticesToUVVertices);
		if (!GPUAutoSwapShader)
		{
			return;
		}
		for (int k = 0; k < GPUmaterials.Length; k++)
		{
			Shader shader = GPUmaterials[k].shader;
			Shader shader2 = Shader.Find(shader.name + "ComputeBuff");
			if (shader2 != null)
			{
				Material material = new Material(GPUmaterials[k]);
				material.shader = shader2;
				GPUmaterials[k] = material;
			}
		}
	}

	public void UpdateVerts()
	{
		InitMesh();
		Vector3[] array;
		Vector3[] array2;
		if (Application.isPlaying)
		{
			array = skin.drawVerts;
			array2 = skin.drawSurfaceNormals;
		}
		else
		{
			array = skin.dazMesh.morphedUVVertices;
			array2 = skin.dazMesh.baseSurfaceNormals;
		}
		int[] baseTriangles = skin.dazMesh.baseTriangles;
		bool flag = true;
		for (int i = 0; i < _numMaterials; i++)
		{
			if (!dazMesh.materialsEnabled[i])
			{
				flag = false;
				break;
			}
		}
		if (flag || !onlyUpdateEnabledMaterials)
		{
			for (int j = 0; j < dazMesh.numBaseVertices; j++)
			{
				SkinWrapVert skinWrapVert = _wrapVertices[j];
				int closestTriangle = skinWrapVert.closestTriangle;
				int num = closestTriangle * 3;
				int num2 = baseTriangles[num];
				int num3 = baseTriangles[num + 1];
				int num4 = baseTriangles[num + 2];
				Vector3 vector = (array[num2] + array[num3] + array[num4]) * 0.33333f;
				Vector3 vector2 = array[num2] - vector;
				Vector3 vector3 = array2[closestTriangle];
				Vector3 vector4 = Vector3.Cross(vector2, vector3);
				float surfaceNormalProjection = skinWrapVert.surfaceNormalProjection;
				if (moveToSurface && surfaceNormalProjection < moveToSurfaceOffset)
				{
					surfaceNormalProjection = moveToSurfaceOffset;
				}
				Vector3 vector5 = vector + vector2 * (skinWrapVert.surfaceTangent1Projection + skinWrapVert.surfaceTangent1WrapNormalDot * additionalThicknessMultiplier) + vector4 * (skinWrapVert.surfaceTangent2Projection + skinWrapVert.surfaceTangent2WrapNormalDot * additionalThicknessMultiplier) + vector3 * (surfaceNormalProjection + surfaceOffset + skinWrapVert.surfaceNormalWrapNormalDot * additionalThicknessMultiplier);
				_verts1[j] = vector5;
			}
		}
		else
		{
			int[][] baseMaterialVertices = dazMesh.baseMaterialVertices;
			for (int k = 0; k < _numMaterials; k++)
			{
				if (!dazMesh.materialsEnabled[k])
				{
					continue;
				}
				for (int l = 0; l < baseMaterialVertices[k].Length; l++)
				{
					int num5 = baseMaterialVertices[k][l];
					SkinWrapVert skinWrapVert2 = _wrapVertices[num5];
					int closestTriangle2 = skinWrapVert2.closestTriangle;
					int num6 = closestTriangle2 * 3;
					int num7 = baseTriangles[num6];
					int num8 = baseTriangles[num6 + 1];
					int num9 = baseTriangles[num6 + 2];
					Vector3 vector6 = (array[num7] + array[num8] + array[num9]) * 0.33333f;
					Vector3 vector7 = array[num7] - vector6;
					Vector3 vector8 = array2[closestTriangle2];
					Vector3 vector9 = Vector3.Cross(vector7, vector8);
					float surfaceNormalProjection2 = skinWrapVert2.surfaceNormalProjection;
					if (moveToSurface && surfaceNormalProjection2 < moveToSurfaceOffset)
					{
						surfaceNormalProjection2 = moveToSurfaceOffset;
					}
					Vector3 vector10 = vector6 + vector7 * skinWrapVert2.surfaceTangent1Projection + vector9 * skinWrapVert2.surfaceTangent2Projection + vector8 * (surfaceNormalProjection2 + surfaceOffset + skinWrapVert2.surfaceNormalWrapNormalDot * additionalThicknessMultiplier);
					_verts1[num5] = vector10;
				}
			}
		}
		if (smoothOuterLoops > 0 && (laplacianSmoothPasses > 0 || springSmoothPasses > 0))
		{
			InitSmoothing();
			int num10 = 0;
			for (int m = 0; m < smoothOuterLoops; m++)
			{
				for (int n = 0; n < laplacianSmoothPasses; n++)
				{
					if (num10 % 2 == 0)
					{
						meshSmooth.LaplacianSmooth(_verts1, _verts2);
						meshSmooth.HCCorrection(_verts1, _verts2, 0.5f);
						_drawVerts = _verts2;
					}
					else
					{
						meshSmooth.LaplacianSmooth(_verts2, _verts1);
						meshSmooth.HCCorrection(_verts2, _verts1, 0.5f);
						_drawVerts = _verts1;
					}
					num10++;
				}
				for (int num11 = 0; num11 < springSmoothPasses; num11++)
				{
					if (num10 % 2 == 0)
					{
						meshSmooth.SpringSmooth(_verts1, _verts2, springSmoothFactor);
						_drawVerts = _verts2;
					}
					else
					{
						meshSmooth.SpringSmooth(_verts2, _verts1, springSmoothFactor);
						_drawVerts = _verts1;
					}
					num10++;
				}
			}
		}
		else
		{
			_drawVerts = _verts1;
		}
		DAZVertexMap[] baseVerticesToUVVertices = dazMesh.baseVerticesToUVVertices;
		foreach (DAZVertexMap dAZVertexMap in baseVerticesToUVVertices)
		{
			ref Vector3 reference = ref _drawVerts[dAZVertexMap.tovert];
			reference = _drawVerts[dAZVertexMap.fromvert];
		}
		mesh.vertices = _drawVerts;
	}

	protected void UpdateVertsGPU()
	{
		if (!(skin != null) || !skin.isActiveAndEnabled || !(GPUSkinWrapper != null))
		{
			return;
		}
		InitMesh();
		SkinWrapGPUInit();
		GPURecheckSurfaceNormalOffsets();
		GPUSkinWrapper.SetBuffer(_skinWrapKernel, "skinToTriangles", _skinToTrianglesBuffer);
		if (skin.useSmoothing)
		{
			GPUSkinWrapper.SetBuffer(_skinWrapKernel, "skinToVertices", skin.smoothedVertsBuffer);
		}
		else
		{
			GPUSkinWrapper.SetBuffer(_skinWrapKernel, "skinToVertices", skin.rawVertsBuffer);
		}
		GPUSkinWrapper.SetBuffer(_skinWrapKernel, "skinToSurfaceNormals", skin.surfaceNormalsBuffer);
		GPUSkinWrapper.SetBuffer(_skinWrapKernel, "skinWrapVerts", _wrapVerticesBuffer);
		GPUSkinWrapper.SetBuffer(_skinWrapKernel, "outVerts", _verticesBuffer1);
		GPUSkinWrapper.SetFloat("skinWrapNormalOffset", surfaceOffset);
		GPUSkinWrapper.SetFloat("skinWrapThicknessMultiplier", additionalThicknessMultiplier);
		GPUSkinWrapper.Dispatch(_skinWrapKernel, numVertThreadGroups, 1, 1);
		if (smoothOuterLoops > 0 && (laplacianSmoothPasses > 0 || springSmoothPasses > 0))
		{
			InitSmoothing();
			int num = 0;
			for (int i = 0; i < smoothOuterLoops; i++)
			{
				for (int j = 0; j < laplacianSmoothPasses; j++)
				{
					if (num % 2 == 0)
					{
						meshSmoothGPU.LaplacianSmoothGPU(_verticesBuffer1, _verticesBuffer2);
						meshSmoothGPU.HCCorrectionGPU(_verticesBuffer1, _verticesBuffer2, laplacianSmoothBeta);
						_drawVerticesBuffer = _verticesBuffer2;
					}
					else
					{
						meshSmoothGPU.LaplacianSmoothGPU(_verticesBuffer2, _verticesBuffer1);
						meshSmoothGPU.HCCorrectionGPU(_verticesBuffer2, _verticesBuffer1, laplacianSmoothBeta);
						_drawVerticesBuffer = _verticesBuffer1;
					}
					num++;
				}
				for (int k = 0; k < springSmoothPasses; k++)
				{
					if (num % 2 == 0)
					{
						if (useSpring2)
						{
							meshSmoothGPU.SpringSmooth2GPU(_verticesBuffer1, _verticesBuffer2, spring2SmoothFactor);
						}
						else
						{
							meshSmoothGPU.SpringSmoothGPU(_verticesBuffer1, _verticesBuffer2, springSmoothFactor);
						}
						_drawVerticesBuffer = _verticesBuffer2;
					}
					else
					{
						if (useSpring2)
						{
							meshSmoothGPU.SpringSmooth2GPU(_verticesBuffer2, _verticesBuffer1, spring2SmoothFactor);
						}
						else
						{
							meshSmoothGPU.SpringSmoothGPU(_verticesBuffer2, _verticesBuffer1, springSmoothFactor);
						}
						_drawVerticesBuffer = _verticesBuffer1;
					}
					num++;
				}
			}
		}
		else
		{
			_drawVerticesBuffer = _verticesBuffer1;
		}
		mapVerticesGPU.Map(_drawVerticesBuffer);
		InitRecalcNormalsTangents();
		if (recalculateNormals)
		{
			recalcNormalsGPU.RecalculateNormals(_drawVerticesBuffer);
		}
		if (recalculateTangents)
		{
			recalcTangentsGPU.RecalculateTangents(_drawVerticesBuffer, _normalsBuffer);
		}
	}

	public void DrawMesh()
	{
		if (!(mesh != null))
		{
			return;
		}
		Matrix4x4 matrix = (Application.isPlaying ? Matrix4x4.identity : ((!(skin != null) || !(skin.root != null)) ? base.transform.localToWorldMatrix : skin.root.transform.localToWorldMatrix));
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			if (dazMesh.useSimpleMaterial && (bool)dazMesh.simpleMaterial)
			{
				Graphics.DrawMesh(mesh, matrix, dazMesh.simpleMaterial, 0, null, i, null, dazMesh.castShadows, dazMesh.receiveShadows);
			}
			else if (dazMesh.materialsEnabled[i] && dazMesh.materials[i] != null)
			{
				Graphics.DrawMesh(mesh, matrix, dazMesh.materials[i], 0, null, i, null, dazMesh.castShadows, dazMesh.receiveShadows);
			}
		}
	}

	protected void DrawMeshGPU()
	{
		if (!(mesh != null))
		{
			return;
		}
		Matrix4x4 identity = Matrix4x4.identity;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			if (GPUuseSimpleMaterial && (bool)GPUsimpleMaterial)
			{
				if (skin.delayDisplayOneFrame)
				{
					GPUsimpleMaterial.SetBuffer("verts", _delayedVertsBuffer);
					GPUsimpleMaterial.SetBuffer("normals", _delayedNormalsBuffer);
					GPUsimpleMaterial.SetBuffer("tangents", _delayedTangentsBuffer);
				}
				else
				{
					GPUsimpleMaterial.SetBuffer("verts", _drawVerticesBuffer);
					GPUsimpleMaterial.SetBuffer("normals", _normalsBuffer);
					GPUsimpleMaterial.SetBuffer("tangents", _tangentsBuffer);
				}
				Graphics.DrawMesh(mesh, identity, GPUsimpleMaterial, 0, null, i, null, dazMesh.castShadows, dazMesh.receiveShadows);
			}
			else if (materialsEnabled != null && materialsEnabled[i] && GPUmaterials[i] != null)
			{
				if (skin.delayDisplayOneFrame)
				{
					GPUmaterials[i].SetBuffer("verts", _delayedVertsBuffer);
					GPUmaterials[i].SetBuffer("normals", _delayedNormalsBuffer);
					GPUmaterials[i].SetBuffer("tangents", _delayedTangentsBuffer);
				}
				else
				{
					GPUmaterials[i].SetBuffer("verts", _drawVerticesBuffer);
					GPUmaterials[i].SetBuffer("normals", _normalsBuffer);
					GPUmaterials[i].SetBuffer("tangents", _tangentsBuffer);
				}
				Graphics.DrawMesh(mesh, identity, GPUmaterials[i], 0, null, i, null, dazMesh.castShadows, dazMesh.receiveShadows);
			}
		}
	}

	protected void GPUCleanup()
	{
		if (meshSmoothGPU != null)
		{
			meshSmoothGPU.Release();
			meshSmoothGPU = null;
		}
		if (mapVerticesGPU != null)
		{
			mapVerticesGPU.Release();
			mapVerticesGPU = null;
		}
		if (recalcNormalsGPU != null)
		{
			recalcNormalsGPU.Release();
			recalcNormalsGPU = null;
		}
		if (recalcTangentsGPU != null)
		{
			recalcTangentsGPU.Release();
			recalcTangentsGPU = null;
		}
		if (_skinToTrianglesBuffer != null)
		{
			_skinToTrianglesBuffer.Release();
			_skinToTrianglesBuffer = null;
		}
		if (_wrapVerticesBuffer != null)
		{
			_wrapVerticesBuffer.Release();
			_wrapVerticesBuffer = null;
		}
		if (_verticesBuffer1 != null)
		{
			_verticesBuffer1.Release();
			_verticesBuffer1 = null;
		}
		if (_verticesBuffer2 != null)
		{
			_verticesBuffer2.Release();
			_verticesBuffer2 = null;
		}
		if (_delayedVertsBuffer != null)
		{
			_delayedVertsBuffer.Release();
			_delayedVertsBuffer = null;
		}
		if (_delayedNormalsBuffer != null)
		{
			_delayedNormalsBuffer.Release();
			_delayedNormalsBuffer = null;
		}
		if (_delayedTangentsBuffer != null)
		{
			_delayedTangentsBuffer.Release();
			_delayedTangentsBuffer = null;
		}
	}

	protected void OnDestroy()
	{
		GPUCleanup();
	}

	private void Start()
	{
	}

	private void LateUpdate()
	{
		if (!draw || !(skin != null) || !(dazMesh != null) || _wrapVertices == null)
		{
			return;
		}
		if (skin.skinMethod == DAZSkinV2.SkinMethod.CPU || !Application.isPlaying)
		{
			UpdateVerts();
			DrawMesh();
			return;
		}
		if (skin.delayDisplayOneFrame && GPUSkinWrapper != null && _delayedVertsBuffer != null)
		{
			GPUSkinWrapper.SetBuffer(_copyKernel, "inVerts", _drawVerticesBuffer);
			GPUSkinWrapper.SetBuffer(_copyKernel, "outVerts", _delayedVertsBuffer);
			GPUSkinWrapper.Dispatch(_copyKernel, numVertThreadGroups, 1, 1);
			if (_normalsBuffer != null)
			{
				GPUSkinWrapper.SetBuffer(_copyKernel, "inVerts", _normalsBuffer);
				GPUSkinWrapper.SetBuffer(_copyKernel, "outVerts", _delayedNormalsBuffer);
				GPUSkinWrapper.Dispatch(_copyKernel, numVertThreadGroups, 1, 1);
			}
			if (_tangentsBuffer != null)
			{
				GPUMeshCompute.SetBuffer(_copyTangentsKernel, "inTangents", _tangentsBuffer);
				GPUMeshCompute.SetBuffer(_copyTangentsKernel, "outTangents", _delayedTangentsBuffer);
				GPUMeshCompute.Dispatch(_copyTangentsKernel, numVertThreadGroups, 1, 1);
			}
		}
		UpdateVertsGPU();
		DrawMeshGPU();
	}
}
