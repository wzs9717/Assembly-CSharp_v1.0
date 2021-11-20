using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[ExecuteInEditMode]
public class DAZMesh : MonoBehaviour
{
	protected const float geoScale = 0.01f;

	public bool drawBaseMesh;

	[SerializeField]
	protected bool _drawMorphedBaseMesh;

	public bool drawUVMappedMesh;

	[SerializeField]
	protected bool _drawMorphedUVMappedMesh;

	public bool drawInEditorWhenNotPlaying;

	public bool recalcNormalsOnMorph;

	public bool recalcTangentsOnMorph;

	public bool useUnityRecalcNormals;

	[SerializeField]
	protected bool _useSmoothing;

	public bool useSimpleMaterial;

	public Material simpleMaterial;

	public bool showMaterials;

	public Material[] materials;

	public bool use2PassMaterials;

	public Material[] materialsPass1;

	public Material[] materialsPass2;

	public bool[] materialsEnabled;

	public bool[] materialsPass1Enabled;

	public bool[] materialsPass2Enabled;

	public bool[] materialsShadowCastEnabled;

	public DAZMesh copyMaterialsFrom;

	public bool castShadows = true;

	public bool receiveShadows = true;

	public string nodeId;

	public string sceneNodeId;

	public string geometryId;

	public string sceneGeometryId;

	[SerializeField]
	protected int _numBaseVertices;

	[SerializeField]
	protected int _numBasePolygons;

	[SerializeField]
	protected int _numUVVertices;

	[SerializeField]
	protected int _numMaterials;

	[SerializeField]
	protected string[] _materialNames;

	public DAZMorphBank morphBank;

	public float vertexNormalOffset;

	protected bool _verticesChangedLastFrame;

	protected bool _visibleVerticesChangedLastFrame;

	protected bool _verticesChangedThisFrame;

	protected bool _visibleVerticesChangedThisFrame;

	private bool _normalsDirtyThisFrame;

	private bool _tangentsDirtyThisFrame;

	protected Mesh _baseMesh;

	[SerializeField]
	protected Vector3[] _baseVertices;

	protected Vector3[] _baseNormals;

	protected Vector3[] _baseSurfaceNormals;

	[SerializeField]
	protected MeshPoly[] _basePolyList;

	protected int[] _baseTriangles;

	protected int[][] _baseMaterialVertices;

	protected Mesh _morphedBaseMesh;

	protected Vector3[] _morphedBaseVertices;

	protected Vector3[] _morphedBaseNormals;

	protected Vector3[] _morphedBaseSurfaceNormals;

	public bool debugGrafting;

	public DAZMeshGraft meshGraft;

	public DAZMesh graftTo;

	[SerializeField]
	protected DAZVertexMap[] _baseVerticesToUVVertices;

	protected Mesh _uvMappedMesh;

	[SerializeField]
	protected Vector3[] _UVVertices;

	protected Vector3[] _UVNormals;

	protected Vector4[] _UVTangents;

	[SerializeField]
	protected MeshPoly[] _UVPolyList;

	protected int[] _UVTriangles;

	[SerializeField]
	protected Vector2[] _UV;

	[SerializeField]
	protected Vector2[] _OrigUV;

	[SerializeField]
	protected bool _usePatches;

	public int numUVPatches;

	public DAZUVPatch[] UVPatches;

	protected MeshSmooth meshSmooth;

	protected Mesh _morphedUVMappedMesh;

	protected Mesh _visibleMorphedUVMappedMesh;

	[SerializeField]
	protected bool _drawVisibleMorphedUVMappedMesh;

	protected Vector3[] _visibleMorphedUVVertices;

	protected Vector3[] _morphedUVVertices;

	protected Vector3[] _smoothedMorphedUVVertices;

	public bool[] morphedBaseDirtyVertices;

	public bool[] morphedUVDirtyVertices;

	public bool morphedNormalsDirty;

	public bool morphedTangentsDirty;

	protected Vector3[] _morphedUVNormals;

	protected Vector4[] _morphedUVTangents;

	protected bool useUnityMaterialOrdering;

	protected Matrix4x4 lastMatrix1 = Matrix4x4.identity;

	protected Matrix4x4 lastMatrix2 = Matrix4x4.identity;

	public DAZBone drawFromBone;

	public int delayDisplayFrameCount;

	protected bool _wasInit;

	public bool drawMorphedBaseMesh
	{
		get
		{
			return _drawMorphedBaseMesh;
		}
		set
		{
			if (_drawMorphedBaseMesh != value)
			{
				_drawMorphedBaseMesh = value;
				if (value)
				{
					ApplyMorphs(force: true);
				}
			}
		}
	}

	public bool drawMorphedUVMappedMesh
	{
		get
		{
			return _drawMorphedUVMappedMesh;
		}
		set
		{
			if (_drawMorphedUVMappedMesh != value)
			{
				_drawMorphedUVMappedMesh = value;
				if (value)
				{
					ApplyMorphs(force: true);
				}
			}
		}
	}

	public bool useSmoothing
	{
		get
		{
			return _useSmoothing;
		}
		set
		{
			if (_useSmoothing != value)
			{
				_useSmoothing = value;
				ApplyMorphs(force: true);
			}
		}
	}

	public int numBaseVertices => _numBaseVertices;

	public int numBasePolygons => _numBasePolygons;

	public int numUVVertices => _numUVVertices;

	public int numMaterials => _numMaterials;

	public string[] materialNames => _materialNames;

	public bool verticesChangedLastFrame => _verticesChangedLastFrame;

	public bool visibleVerticesChangedLastFrame => _visibleVerticesChangedLastFrame;

	public bool verticesChangedThisFrame => _verticesChangedThisFrame;

	public bool visibleVerticesChangedThisFrame => _visibleVerticesChangedThisFrame;

	public bool normalsDirtyThisFrame => _normalsDirtyThisFrame;

	public bool tangentsDirtyThisFrame => _tangentsDirtyThisFrame;

	public Mesh baseMesh
	{
		get
		{
			if (_baseMesh == null)
			{
				Init();
			}
			return _baseMesh;
		}
	}

	public Vector3[] baseVertices => _baseVertices;

	public Vector3[] baseNormals => _baseNormals;

	public Vector3[] baseSurfaceNormals => _baseSurfaceNormals;

	public MeshPoly[] basePolyList => _basePolyList;

	public int[] baseTriangles => _baseTriangles;

	public int[][] baseMaterialVertices => _baseMaterialVertices;

	public Vector3[] morphedBaseVertices => _morphedBaseVertices;

	public DAZVertexMap[] baseVerticesToUVVertices => _baseVerticesToUVVertices;

	public Dictionary<int, List<int>> baseVertToUVVertFullMap
	{
		get
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			for (int i = 0; i < _numBaseVertices; i++)
			{
				List<int> list = new List<int>();
				list.Add(i);
				dictionary.Add(i, list);
			}
			for (int j = 0; j < _baseVerticesToUVVertices.Length; j++)
			{
				int fromvert = _baseVerticesToUVVertices[j].fromvert;
				int tovert = _baseVerticesToUVVertices[j].tovert;
				if (dictionary.TryGetValue(fromvert, out var value))
				{
					value.Add(tovert);
				}
			}
			return dictionary;
		}
	}

	public Dictionary<int, int> uvVertToBaseVert
	{
		get
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 0; i < _baseVerticesToUVVertices.Length; i++)
			{
				int fromvert = _baseVerticesToUVVertices[i].fromvert;
				int tovert = _baseVerticesToUVVertices[i].tovert;
				if (!dictionary.ContainsKey(tovert))
				{
					dictionary.Add(tovert, fromvert);
				}
			}
			return dictionary;
		}
	}

	public Mesh uvMappedMesh => _uvMappedMesh;

	public Vector3[] UVVertices => _UVVertices;

	public Vector3[] UVNormals => _UVNormals;

	public Vector4[] UVTangents => _UVTangents;

	public MeshPoly[] UVPolyList => _UVPolyList;

	public int[] UVTriangles => _UVTriangles;

	public Vector2[] UV => _UV;

	public Vector2[] OrigUV => _OrigUV;

	public bool usePatches
	{
		get
		{
			return _usePatches;
		}
		set
		{
			if (_usePatches != value)
			{
				_usePatches = value;
				ApplyUVPatches();
				RecalculateMorphedMeshTangents(forceAll: true);
			}
		}
	}

	public Mesh morphedUVMappedMesh => _morphedUVMappedMesh;

	public bool drawVisibleMorphedUVMappedMesh
	{
		get
		{
			return _drawVisibleMorphedUVMappedMesh;
		}
		set
		{
			if (_drawVisibleMorphedUVMappedMesh != value)
			{
				_drawVisibleMorphedUVMappedMesh = value;
				if (value)
				{
					ApplyMorphs(force: true);
				}
			}
		}
	}

	public Vector3[] visibleMorphedUVVertices => _visibleMorphedUVVertices;

	public Vector3[] rawMorphedUVVertices => _morphedUVVertices;

	public Vector3[] morphedUVVertices
	{
		get
		{
			if (useSmoothing)
			{
				return _smoothedMorphedUVVertices;
			}
			return _morphedUVVertices;
		}
	}

	public Vector3[] morphedUVNormals => _morphedUVNormals;

	public Vector4[] morphedUVTangents => _morphedUVTangents;

	public bool wasInit => _wasInit;

	public void CopyMaterials()
	{
		if (copyMaterialsFrom != null)
		{
			materials = new Material[copyMaterialsFrom.materials.Length];
			materialsPass1 = new Material[copyMaterialsFrom.materialsPass1.Length];
			materialsPass2 = new Material[copyMaterialsFrom.materialsPass2.Length];
			materialsEnabled = new bool[copyMaterialsFrom.materials.Length];
			materialsPass1Enabled = new bool[copyMaterialsFrom.materialsPass1.Length];
			materialsPass2Enabled = new bool[copyMaterialsFrom.materialsPass2.Length];
			materialsShadowCastEnabled = new bool[copyMaterialsFrom.materials.Length];
			for (int i = 0; i < copyMaterialsFrom.materials.Length; i++)
			{
				materials[i] = copyMaterialsFrom.materials[i];
				materialsEnabled[i] = copyMaterialsFrom.materialsEnabled[i];
				materialsPass2Enabled[i] = copyMaterialsFrom.materialsPass2Enabled[i];
				materialsShadowCastEnabled[i] = copyMaterialsFrom.materialsShadowCastEnabled[i];
			}
			for (int j = 0; j < copyMaterialsFrom.materialsPass1.Length; j++)
			{
				materialsPass1[j] = copyMaterialsFrom.materialsPass1[j];
			}
			for (int k = 0; k < copyMaterialsFrom.materialsPass2.Length; k++)
			{
				materialsPass2[k] = copyMaterialsFrom.materialsPass2[k];
			}
		}
	}

	public void ApplyUVPatches()
	{
		for (int i = 0; i < _numUVVertices; i++)
		{
			ref Vector2 reference = ref _UV[i];
			reference = _OrigUV[i];
		}
		if (_usePatches)
		{
			for (int j = 0; j < numUVPatches; j++)
			{
				int vertexNum = UVPatches[j].vertexNum;
				if (vertexNum >= 0 && vertexNum < _numUVVertices)
				{
					ref Vector2 reference2 = ref _UV[vertexNum];
					reference2 = UVPatches[j].uv;
				}
			}
		}
		_uvMappedMesh.uv = _UV;
		_morphedUVMappedMesh.uv = _UV;
	}

	protected void InitMeshSmooth()
	{
		if (meshSmooth == null && _baseVertices != null && _basePolyList != null && _baseVerticesToUVVertices != null)
		{
			meshSmooth = new MeshSmooth(_baseVertices, _basePolyList);
		}
	}

	protected void PolyListToTriangleIndexes(MeshPoly[] polylist, List<List<int>> indexes, List<HashSet<int>> materialVertices = null)
	{
		Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		int num = -1;
		foreach (MeshPoly meshPoly in polylist)
		{
			int materialNum = meshPoly.materialNum;
			if (useUnityMaterialOrdering)
			{
				if (!dictionary.TryGetValue(materialNum, out var _))
				{
					num++;
					dictionary.Add(materialNum, value: true);
				}
			}
			else
			{
				num = materialNum;
			}
			int item = meshPoly.vertices[0];
			int item2 = meshPoly.vertices[1];
			int item3 = meshPoly.vertices[2];
			indexes[num].Add(item3);
			indexes[num].Add(item2);
			indexes[num].Add(item);
			if (materialVertices != null)
			{
				materialVertices[num].Add(item);
				materialVertices[num].Add(item2);
				materialVertices[num].Add(item3);
			}
			if (meshPoly.vertices.Length == 4)
			{
				int item4 = meshPoly.vertices[3];
				indexes[num].Add(item);
				indexes[num].Add(item4);
				indexes[num].Add(item3);
				materialVertices?[num].Add(item4);
			}
		}
	}

	public virtual void DeriveMeshes()
	{
		_baseMesh = new Mesh();
		_morphedBaseMesh = new Mesh();
		_uvMappedMesh = new Mesh();
		_morphedUVMappedMesh = new Mesh();
		_visibleMorphedUVMappedMesh = new Mesh();
		_morphedBaseVertices = (Vector3[])_baseVertices.Clone();
		_morphedUVVertices = (Vector3[])_UVVertices.Clone();
		_visibleMorphedUVVertices = (Vector3[])_UVVertices.Clone();
		_smoothedMorphedUVVertices = (Vector3[])_morphedUVVertices.Clone();
		_baseMesh.vertices = _baseVertices;
		_morphedBaseMesh.vertices = _baseVertices;
		_uvMappedMesh.vertices = _UVVertices;
		_morphedUVMappedMesh.vertices = _morphedUVVertices;
		_visibleMorphedUVMappedMesh.vertices = _visibleMorphedUVVertices;
		List<List<int>> list = new List<List<int>>();
		List<HashSet<int>> list2 = new List<HashSet<int>>();
		List<List<int>> list3 = new List<List<int>>();
		list.Capacity = _numMaterials;
		list2.Capacity = _numMaterials;
		list3.Capacity = _numMaterials;
		for (int i = 0; i < _numMaterials; i++)
		{
			list.Add(new List<int>());
			list2.Add(new HashSet<int>());
			list3.Add(new List<int>());
		}
		_baseMesh.subMeshCount = _numMaterials;
		_morphedBaseMesh.subMeshCount = _numMaterials;
		_uvMappedMesh.subMeshCount = _numMaterials;
		_morphedUVMappedMesh.subMeshCount = _numMaterials;
		_visibleMorphedUVMappedMesh.subMeshCount = _numMaterials;
		PolyListToTriangleIndexes(_basePolyList, list, list2);
		PolyListToTriangleIndexes(_UVPolyList, list3);
		_baseMaterialVertices = new int[_numMaterials][];
		for (int j = 0; j < _numMaterials; j++)
		{
			_baseMaterialVertices[j] = new int[list2[j].Count];
			list2[j].CopyTo(_baseMaterialVertices[j]);
			int[] indices = list[j].ToArray();
			_baseMesh.SetIndices(indices, MeshTopology.Triangles, j);
			_morphedBaseMesh.SetIndices(indices, MeshTopology.Triangles, j);
			int[] indices2 = list3[j].ToArray();
			_uvMappedMesh.SetIndices(indices2, MeshTopology.Triangles, j);
			_morphedUVMappedMesh.SetIndices(indices2, MeshTopology.Triangles, j);
			_visibleMorphedUVMappedMesh.SetIndices(indices2, MeshTopology.Triangles, j);
		}
		_baseTriangles = _baseMesh.triangles;
		_UVTriangles = _uvMappedMesh.triangles;
		_baseMesh.RecalculateBounds();
		_morphedBaseMesh.bounds = _baseMesh.bounds;
		_uvMappedMesh.bounds = _baseMesh.bounds;
		_morphedUVMappedMesh.bounds = _baseMesh.bounds;
		_visibleMorphedUVMappedMesh.bounds = _baseMesh.bounds;
		ApplyUVPatches();
		_baseNormals = new Vector3[_numBaseVertices];
		_baseSurfaceNormals = new Vector3[_baseTriangles.Length / 3];
		RecalculateNormals.recalculateNormals(_baseTriangles, _baseVertices, _baseNormals, _baseSurfaceNormals);
		_baseMesh.normals = _baseNormals;
		_morphedBaseNormals = (Vector3[])_baseNormals.Clone();
		_morphedBaseSurfaceNormals = (Vector3[])_baseSurfaceNormals.Clone();
		_morphedBaseMesh.normals = _morphedBaseNormals;
		_morphedUVNormals = new Vector3[_numUVVertices];
		for (int k = 0; k < _morphedBaseNormals.Length; k++)
		{
			ref Vector3 reference = ref _morphedUVNormals[k];
			reference = _morphedBaseNormals[k];
		}
		_updateDuplicateMorphedUVNormals();
		_UVNormals = (Vector3[])_morphedUVNormals.Clone();
		_uvMappedMesh.normals = _UVNormals;
		_morphedUVMappedMesh.normals = _morphedUVNormals;
		_visibleMorphedUVMappedMesh.normals = _morphedUVNormals;
		_UVTangents = new Vector4[_numUVVertices];
		RecalculateTangents.recalculateTangentsFast(_UVTriangles, _UVVertices, _UVNormals, _UV, _UVTangents);
		_morphedUVTangents = (Vector4[])_UVTangents.Clone();
		_uvMappedMesh.tangents = _UVTangents;
		_morphedUVMappedMesh.tangents = _morphedUVTangents;
		_visibleMorphedUVMappedMesh.tangents = _morphedUVTangents;
		morphedBaseDirtyVertices = new bool[_morphedUVNormals.Length];
		morphedUVDirtyVertices = new bool[_morphedUVNormals.Length];
	}

	public void RecalculateMorphedMeshNormals(bool forceAll = false)
	{
		if (useUnityRecalcNormals)
		{
			_morphedBaseMesh.vertices = _morphedBaseVertices;
			_morphedBaseMesh.RecalculateNormals();
			_morphedBaseNormals = _morphedBaseMesh.normals;
		}
		else
		{
			Vector3[] vertices = ((!_useSmoothing) ? _smoothedMorphedUVVertices : _morphedUVVertices);
			if (forceAll)
			{
				RecalculateNormals.recalculateNormals(_baseTriangles, vertices, _morphedBaseNormals, _morphedBaseSurfaceNormals);
			}
			else
			{
				RecalculateNormals.recalculateNormals(_baseTriangles, vertices, _morphedBaseNormals, _morphedBaseSurfaceNormals, morphedBaseDirtyVertices);
			}
			if (_drawMorphedBaseMesh)
			{
				_morphedBaseMesh.normals = _morphedBaseNormals;
			}
		}
		for (int i = 0; i < _morphedBaseNormals.Length; i++)
		{
			ref Vector3 reference = ref _morphedUVNormals[i];
			reference = _morphedBaseNormals[i];
		}
		_updateDuplicateMorphedUVNormals();
		if (_drawMorphedUVMappedMesh || !Application.isPlaying)
		{
			_morphedUVMappedMesh.normals = _morphedUVNormals;
		}
		if (_drawMorphedUVMappedMesh || !Application.isPlaying)
		{
			_visibleMorphedUVMappedMesh.normals = _morphedUVNormals;
		}
	}

	public void RecalculateMorphedMeshTangentsAccurate()
	{
		RecalculateTangents.recalculateTangents(_morphedUVMappedMesh);
	}

	public void RecalculateMorphedMeshTangents(bool forceAll = false)
	{
		Vector3[] vertices = ((!_useSmoothing) ? _smoothedMorphedUVVertices : _morphedUVVertices);
		if (forceAll)
		{
			RecalculateTangents.recalculateTangentsFast(_UVTriangles, vertices, _morphedUVNormals, _UV, _morphedUVTangents);
		}
		else
		{
			RecalculateTangents.recalculateTangentsFast(_UVTriangles, vertices, _morphedUVNormals, _UV, _morphedUVTangents, morphedUVDirtyVertices);
		}
		if (_drawMorphedUVMappedMesh || !Application.isPlaying)
		{
			_morphedUVMappedMesh.tangents = _morphedUVTangents;
		}
		if (_drawVisibleMorphedUVMappedMesh || !Application.isPlaying)
		{
			_visibleMorphedUVMappedMesh.tangents = _morphedUVTangents;
		}
	}

	protected void _triggerNormalAndTangentRecalc()
	{
		if (recalcNormalsOnMorph && morphedNormalsDirty)
		{
			RecalculateMorphedMeshNormals();
			morphedNormalsDirty = false;
			_normalsDirtyThisFrame = true;
		}
		if (recalcTangentsOnMorph && morphedTangentsDirty)
		{
			DAZVertexMap[] array = _baseVerticesToUVVertices;
			foreach (DAZVertexMap dAZVertexMap in array)
			{
				morphedUVDirtyVertices[dAZVertexMap.tovert] = morphedUVDirtyVertices[dAZVertexMap.fromvert];
			}
			RecalculateMorphedMeshTangents();
			morphedTangentsDirty = false;
			_tangentsDirtyThisFrame = true;
		}
	}

	public void Import(JSONNode jsonGeometry, DAZUVMap uvmap, Dictionary<string, Material> materialMap)
	{
		meshSmooth = null;
		JSONNode jSONNode = jsonGeometry["vertices"];
		_numBaseVertices = jSONNode["count"].AsInt;
		_baseVertices = new Vector3[_numBaseVertices];
		int num = 0;
		foreach (JSONNode item in jSONNode["values"].AsArray)
		{
			float asFloat = item[0].AsFloat;
			float asFloat2 = item[1].AsFloat;
			float asFloat3 = item[2].AsFloat;
			_baseVertices[num].x = (0f - asFloat) * 0.01f;
			_baseVertices[num].y = asFloat2 * 0.01f;
			_baseVertices[num].z = asFloat3 * 0.01f;
			num++;
		}
		_numMaterials = jsonGeometry["polygon_material_groups"]["count"].AsInt;
		if (materials == null || materials.Length != _numMaterials)
		{
			materials = new Material[_numMaterials];
			_materialNames = new string[_numMaterials];
		}
		if (materialsPass1 == null || materialsPass1.Length != _numMaterials)
		{
			materialsPass1 = new Material[_numMaterials];
		}
		if (materialsPass2 == null || materialsPass2.Length != _numMaterials)
		{
			materialsPass2 = new Material[_numMaterials];
		}
		if (materialsEnabled == null || materialsEnabled.Length != _numMaterials)
		{
			materialsEnabled = new bool[_numMaterials];
		}
		if (materialsPass1Enabled == null || materialsPass1Enabled.Length != _numMaterials)
		{
			materialsPass1Enabled = new bool[_numMaterials];
		}
		if (materialsPass2Enabled == null || materialsPass2Enabled.Length != _numMaterials)
		{
			materialsPass2Enabled = new bool[_numMaterials];
		}
		if (materialsShadowCastEnabled == null || materialsShadowCastEnabled.Length != _numMaterials)
		{
			materialsShadowCastEnabled = new bool[_numMaterials];
		}
		num = 0;
		foreach (JSONNode item2 in jsonGeometry["polygon_material_groups"]["values"].AsArray)
		{
			_materialNames[num] = item2;
			materialsEnabled[num] = true;
			materialsShadowCastEnabled[num] = true;
			if (materialMap.TryGetValue(item2, out var value))
			{
				materials[num] = value;
			}
			num++;
		}
		JSONNode jSONNode4 = jsonGeometry["polylist"];
		_numBasePolygons = jSONNode4["count"].AsInt;
		_basePolyList = new MeshPoly[_numBasePolygons];
		_UVPolyList = new MeshPoly[_numBasePolygons];
		num = 0;
		foreach (JSONNode item3 in jSONNode4["values"].AsArray)
		{
			int asInt = item3[1].AsInt;
			int asInt2 = item3[2].AsInt;
			int asInt3 = item3[3].AsInt;
			int asInt4 = item3[4].AsInt;
			MeshPoly meshPoly = new MeshPoly();
			if (item3.Count == 6)
			{
				int asInt5 = item3[5].AsInt;
				meshPoly.vertices = new int[4];
				meshPoly.vertices[3] = asInt5;
			}
			else
			{
				meshPoly.vertices = new int[3];
			}
			meshPoly.materialNum = asInt;
			meshPoly.vertices[0] = asInt2;
			meshPoly.vertices[1] = asInt3;
			meshPoly.vertices[2] = asInt4;
			_basePolyList[num] = meshPoly;
			MeshPoly meshPoly2 = new MeshPoly();
			meshPoly2.materialNum = meshPoly.materialNum;
			meshPoly2.vertices = new int[meshPoly.vertices.Length];
			for (int i = 0; i < meshPoly.vertices.Length; i++)
			{
				meshPoly2.vertices[i] = meshPoly.vertices[i];
			}
			_UVPolyList[num] = meshPoly2;
			num++;
		}
		num = 0;
		if (jsonGeometry["graft"] != null)
		{
			DAZMeshGraft dAZMeshGraft = new DAZMeshGraft();
			JSONNode jSONNode6 = jsonGeometry["graft"]["vertex_pairs"];
			if (jSONNode6 != null)
			{
				int asInt6 = jSONNode6["count"].AsInt;
				dAZMeshGraft.vertexPairs = new DAZMeshGraftVertexPair[asInt6];
				num = 0;
				foreach (JSONNode item4 in jSONNode6["values"].AsArray)
				{
					DAZMeshGraftVertexPair dAZMeshGraftVertexPair = new DAZMeshGraftVertexPair();
					dAZMeshGraftVertexPair.vertexNum = item4[0].AsInt;
					dAZMeshGraftVertexPair.graftToVertexNum = item4[1].AsInt;
					dAZMeshGraft.vertexPairs[num] = dAZMeshGraftVertexPair;
					num++;
				}
				JSONNode jSONNode8 = jsonGeometry["graft"]["hidden_polys"];
				int asInt7 = jSONNode8["count"].AsInt;
				dAZMeshGraft.hiddenPolys = new int[asInt7];
				num = 0;
				foreach (JSONNode item5 in jSONNode8["values"].AsArray)
				{
					dAZMeshGraft.hiddenPolys[num] = item5.AsInt;
					num++;
				}
				meshGraft = dAZMeshGraft;
			}
		}
		if (uvmap.uvs == null)
		{
			_OrigUV = new Vector2[_numBaseVertices];
			_UV = new Vector2[_numBaseVertices];
			_numUVVertices = _numBaseVertices;
			_UVVertices = new Vector3[_numUVVertices];
			_morphedUVVertices = new Vector3[_numUVVertices];
			_baseVerticesToUVVertices = new DAZVertexMap[0];
			for (int j = 0; j < _baseVertices.Length; j++)
			{
				ref Vector3 reference = ref _UVVertices[j];
				reference = _baseVertices[j];
				ref Vector3 reference2 = ref _morphedUVVertices[j];
				reference2 = _baseVertices[j];
			}
		}
		else
		{
			_OrigUV = uvmap.uvs;
			_UV = (Vector2[])uvmap.uvs.Clone();
			_numUVVertices = uvmap.uvs.Length;
			_UVVertices = new Vector3[_numUVVertices];
			_morphedUVVertices = new Vector3[_numUVVertices];
			_baseVerticesToUVVertices = new DAZVertexMap[_numUVVertices - _numBaseVertices];
			for (int k = 0; k < _baseVertices.Length; k++)
			{
				ref Vector3 reference3 = ref _UVVertices[k];
				reference3 = _baseVertices[k];
				ref Vector3 reference4 = ref _morphedUVVertices[k];
				reference4 = _baseVertices[k];
			}
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			int num2 = 0;
			DAZVertexMap[] vertexMap = uvmap.vertexMap;
			foreach (DAZVertexMap dAZVertexMap in vertexMap)
			{
				ref Vector3 reference5 = ref _UVVertices[dAZVertexMap.tovert];
				reference5 = _baseVertices[dAZVertexMap.fromvert];
				ref Vector3 reference6 = ref _morphedUVVertices[dAZVertexMap.tovert];
				reference6 = _baseVertices[dAZVertexMap.fromvert];
				if (!dictionary.TryGetValue(dAZVertexMap.tovert, out var _))
				{
					_baseVerticesToUVVertices[num2] = dAZVertexMap;
					dictionary.Add(dAZVertexMap.tovert, value: true);
					num2++;
				}
				MeshPoly meshPoly3 = _UVPolyList[dAZVertexMap.polyindex];
				for (int m = 0; m < meshPoly3.vertices.Length; m++)
				{
					if (meshPoly3.vertices[m] == dAZVertexMap.fromvert)
					{
						meshPoly3.vertices[m] = dAZVertexMap.tovert;
					}
				}
			}
		}
		DeriveMeshes();
	}

	public void ReduceMeshToSingleMaterial(int materialNum)
	{
		if (materialNum < 0 || materialNum >= _numMaterials)
		{
			return;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
		int num = _baseMaterialVertices[materialNum].Length;
		Vector3[] array = new Vector3[num];
		for (int i = 0; i < num; i++)
		{
			int num2 = _baseMaterialVertices[materialNum][i];
			dictionary.Add(num2, i);
			dictionary2.Add(i, num2);
			ref Vector3 reference = ref array[i];
			reference = _baseVertices[num2];
		}
		int num3 = 1;
		Material[] array2 = new Material[num3];
		array2[0] = materials[materialNum];
		string[] array3 = new string[num3];
		array3[0] = _materialNames[materialNum];
		Material[] array4 = new Material[num3];
		array4[0] = materialsPass1[materialNum];
		Material[] array5 = new Material[num3];
		array5[0] = materialsPass2[materialNum];
		bool[] array6 = new bool[num3];
		array6[0] = materialsEnabled[materialNum];
		bool[] array7 = new bool[num3];
		array7[0] = materialsPass1Enabled[materialNum];
		bool[] array8 = new bool[num3];
		array8[0] = materialsPass2Enabled[materialNum];
		bool[] array9 = new bool[num3];
		array9[0] = materialsShadowCastEnabled[materialNum];
		List<MeshPoly> list = new List<MeshPoly>();
		List<MeshPoly> list2 = new List<MeshPoly>();
		List<DAZVertexMap> list3 = new List<DAZVertexMap>();
		int num4 = num;
		int num5 = 0;
		for (int j = 0; j < _numBasePolygons; j++)
		{
			MeshPoly meshPoly = _basePolyList[j];
			MeshPoly meshPoly2 = _UVPolyList[j];
			if (meshPoly.materialNum != materialNum)
			{
				continue;
			}
			MeshPoly meshPoly3 = new MeshPoly();
			MeshPoly meshPoly4 = new MeshPoly();
			meshPoly3.vertices = new int[meshPoly.vertices.Length];
			meshPoly4.vertices = new int[meshPoly.vertices.Length];
			for (int k = 0; k < meshPoly.vertices.Length; k++)
			{
				if (dictionary.TryGetValue(meshPoly.vertices[k], out var value))
				{
					meshPoly3.vertices[k] = value;
					if (dictionary.TryGetValue(meshPoly2.vertices[k], out var value2))
					{
						meshPoly4.vertices[k] = value2;
						continue;
					}
					meshPoly4.vertices[k] = num4;
					dictionary.Add(meshPoly2.vertices[j], num4);
					dictionary2.Add(num4, meshPoly2.vertices[j]);
					num4++;
					DAZVertexMap dAZVertexMap = new DAZVertexMap();
					dAZVertexMap.polyindex = num5;
					dAZVertexMap.fromvert = value;
					dAZVertexMap.tovert = num4;
					list3.Add(dAZVertexMap);
				}
				else
				{
					Debug.LogError("Could not find vert index " + meshPoly.vertices[k] + " in old vert to new vert map, but it should be there");
				}
			}
			meshPoly3.materialNum = 0;
			list.Add(meshPoly3);
			meshPoly4.materialNum = 0;
			list2.Add(meshPoly4);
			num5++;
		}
		int count = list.Count;
		MeshPoly[] array10 = list.ToArray();
		MeshPoly[] uVPolyList = list2.ToArray();
		int num6 = num4;
		Vector3[] array11 = new Vector3[num6];
		Vector3[] array12 = new Vector3[num6];
		for (int l = 0; l < num; l++)
		{
			ref Vector3 reference2 = ref array11[l];
			reference2 = array[l];
			ref Vector3 reference3 = ref array12[l];
			reference3 = array[l];
		}
		foreach (DAZVertexMap item in list3)
		{
			ref Vector3 reference4 = ref array11[item.tovert];
			reference4 = array[item.fromvert];
			ref Vector3 reference5 = ref array12[item.tovert];
			reference5 = array[item.fromvert];
		}
		Vector2[] array13 = new Vector2[num6];
		Vector2[] array14 = new Vector2[num6];
		for (int m = 0; m < num6; m++)
		{
			if (dictionary2.TryGetValue(m, out var value3))
			{
				ref Vector2 reference6 = ref array13[m];
				reference6 = _OrigUV[value3];
				ref Vector2 reference7 = ref array14[m];
				reference7 = _UV[value3];
			}
			else
			{
				Debug.LogError("Could not find new vert index " + m + " in newVertToOldVert map, but it should be there");
			}
		}
		_numBaseVertices = num;
		_baseVertices = array;
		_numMaterials = num3;
		_materialNames = array3;
		materials = array2;
		materialsEnabled = array6;
		materialsPass1 = array4;
		materialsPass2 = array5;
		materialsPass1Enabled = array7;
		materialsPass2Enabled = array8;
		materialsShadowCastEnabled = array9;
		_numBasePolygons = count;
		_basePolyList = array10;
		_UVPolyList = uVPolyList;
		_numUVVertices = num6;
		_UVVertices = array11;
		_morphedUVVertices = array12;
		_baseVerticesToUVVertices = list3.ToArray();
		_OrigUV = array13;
		_UV = array14;
		DeriveMeshes();
	}

	protected void _updateDuplicateMorphedUVVertices()
	{
		if (_baseVerticesToUVVertices != null)
		{
			DAZVertexMap[] array = _baseVerticesToUVVertices;
			foreach (DAZVertexMap dAZVertexMap in array)
			{
				ref Vector3 reference = ref _morphedUVVertices[dAZVertexMap.tovert];
				reference = _morphedUVVertices[dAZVertexMap.fromvert];
				ref Vector3 reference2 = ref _visibleMorphedUVVertices[dAZVertexMap.tovert];
				reference2 = _visibleMorphedUVVertices[dAZVertexMap.fromvert];
			}
		}
	}

	protected void _updateDuplicateSmoothedMorphedUVVertices()
	{
		if (_baseVerticesToUVVertices != null)
		{
			DAZVertexMap[] array = _baseVerticesToUVVertices;
			foreach (DAZVertexMap dAZVertexMap in array)
			{
				ref Vector3 reference = ref _smoothedMorphedUVVertices[dAZVertexMap.tovert];
				reference = _smoothedMorphedUVVertices[dAZVertexMap.fromvert];
				ref Vector3 reference2 = ref _visibleMorphedUVVertices[dAZVertexMap.tovert];
				reference2 = _visibleMorphedUVVertices[dAZVertexMap.fromvert];
			}
		}
	}

	protected void _updateDuplicateMorphedUVNormals()
	{
		if (_baseVerticesToUVVertices != null)
		{
			DAZVertexMap[] array = _baseVerticesToUVVertices;
			foreach (DAZVertexMap dAZVertexMap in array)
			{
				ref Vector3 reference = ref _morphedUVNormals[dAZVertexMap.tovert];
				reference = _morphedUVNormals[dAZVertexMap.fromvert];
			}
		}
	}

	public void StartMorph()
	{
		_verticesChangedLastFrame = _verticesChangedThisFrame;
		_visibleVerticesChangedLastFrame = _visibleVerticesChangedThisFrame;
		_verticesChangedThisFrame = false;
		_visibleVerticesChangedThisFrame = false;
	}

	public void ApplyMorphVertices(bool visibleVerticesChanged)
	{
		_verticesChangedThisFrame = true;
		_visibleVerticesChangedThisFrame = visibleVerticesChanged;
		if (useSmoothing)
		{
			InitMeshSmooth();
			meshSmooth.LaplacianSmooth(_morphedUVVertices, _smoothedMorphedUVVertices);
			meshSmooth.HCCorrection(_morphedUVVertices, _smoothedMorphedUVVertices, 0.5f);
			_updateDuplicateSmoothedMorphedUVVertices();
			if (_drawMorphedUVMappedMesh || !Application.isPlaying)
			{
				_morphedUVMappedMesh.vertices = _smoothedMorphedUVVertices;
			}
			_triggerNormalAndTangentRecalc();
		}
		else
		{
			_updateDuplicateMorphedUVVertices();
			if (_drawMorphedUVMappedMesh || !Application.isPlaying)
			{
				_morphedUVMappedMesh.vertices = _morphedUVVertices;
			}
			_triggerNormalAndTangentRecalc();
		}
		if (_drawMorphedBaseMesh)
		{
			_morphedBaseMesh.vertices = _morphedBaseVertices;
		}
		if (_drawVisibleMorphedUVMappedMesh || !Application.isPlaying)
		{
			_visibleMorphedUVMappedMesh.vertices = _visibleMorphedUVVertices;
		}
	}

	public void ResetMorphs()
	{
		if (morphBank != null)
		{
			morphBank.ResetMorphs();
		}
	}

	public void ApplyMorphs(bool force = false)
	{
		if (morphBank != null)
		{
			morphBank.ApplyMorphs(force);
		}
	}

	public void ResetMorphedVertices()
	{
		if (!_wasInit)
		{
			return;
		}
		_verticesChangedThisFrame = true;
		_visibleVerticesChangedThisFrame = true;
		if (vertexNormalOffset == 0f)
		{
			for (int i = 0; i < _numUVVertices; i++)
			{
				ref Vector3 reference = ref _morphedUVVertices[i];
				reference = _UVVertices[i];
				ref Vector3 reference2 = ref _visibleMorphedUVVertices[i];
				reference2 = _UVVertices[i];
			}
		}
		else
		{
			for (int j = 0; j < _numUVVertices; j++)
			{
				ref Vector3 reference3 = ref _morphedUVVertices[j];
				reference3 = _UVVertices[j] + _UVNormals[j] * vertexNormalOffset;
				ref Vector3 reference4 = ref _visibleMorphedUVVertices[j];
				reference4 = _morphedUVVertices[j];
			}
		}
		if (_morphedUVMappedMesh != null && _morphedUVVertices != null)
		{
			_morphedUVMappedMesh.vertices = _morphedUVVertices;
		}
		if (_visibleMorphedUVMappedMesh != null && _visibleMorphedUVVertices != null)
		{
			_visibleMorphedUVMappedMesh.vertices = _visibleMorphedUVVertices;
		}
	}

	public void DrawMorphedUVMappedMesh(Matrix4x4 m)
	{
		MeshFilter component = GetComponent<MeshFilter>();
		MeshRenderer component2 = GetComponent<MeshRenderer>();
		if (component != null && component2 != null && component2.enabled)
		{
			if (component.sharedMesh != morphedUVMappedMesh)
			{
				component.sharedMesh = morphedUVMappedMesh;
			}
			component2.sharedMaterials = materials;
			return;
		}
		if (use2PassMaterials)
		{
			for (int i = 0; i < morphedUVMappedMesh.subMeshCount; i++)
			{
				Material material = null;
				if (materialsPass1 != null)
				{
					material = materialsPass1[i];
				}
				if (material == null || useSimpleMaterial)
				{
					material = simpleMaterial;
				}
				if (material != null && materialsPass1Enabled[i])
				{
					Graphics.DrawMesh(morphedUVMappedMesh, m, material, 0, null, i, null, castShadows && materialsShadowCastEnabled[i], receiveShadows);
				}
			}
			for (int j = 0; j < morphedUVMappedMesh.subMeshCount; j++)
			{
				Material material2 = null;
				if (materialsPass2 != null)
				{
					material2 = materialsPass2[j];
				}
				if (material2 == null || useSimpleMaterial)
				{
					material2 = simpleMaterial;
				}
				if (material2 != null && materialsPass2Enabled[j])
				{
					Graphics.DrawMesh(morphedUVMappedMesh, m, material2, 0, null, j, null, castShadows && materialsShadowCastEnabled[j], receiveShadows);
				}
			}
			return;
		}
		for (int k = 0; k < morphedUVMappedMesh.subMeshCount; k++)
		{
			Material material3 = null;
			if (materials != null)
			{
				material3 = materials[k];
			}
			if (material3 == null || useSimpleMaterial)
			{
				material3 = simpleMaterial;
			}
			if (material3 != null && materialsEnabled[k])
			{
				Graphics.DrawMesh(morphedUVMappedMesh, m, material3, 0, null, k, null, castShadows && materialsShadowCastEnabled[k], receiveShadows);
			}
		}
	}

	public void Draw()
	{
		if (!drawBaseMesh && !drawMorphedBaseMesh && !drawUVMappedMesh && !_drawMorphedUVMappedMesh && !_drawVisibleMorphedUVMappedMesh && !debugGrafting && (!drawInEditorWhenNotPlaying || Application.isPlaying))
		{
			return;
		}
		Matrix4x4 matrix4x = base.transform.localToWorldMatrix;
		if (Application.isPlaying && drawFromBone != null)
		{
			matrix4x *= drawFromBone.changeFromOriginalMatrix;
		}
		if (delayDisplayFrameCount == 2)
		{
			Matrix4x4 matrix4x2 = lastMatrix2;
			lastMatrix2 = lastMatrix1;
			lastMatrix1 = matrix4x;
			matrix4x = matrix4x2;
		}
		else if (delayDisplayFrameCount == 1)
		{
			Matrix4x4 matrix4x3 = lastMatrix1;
			lastMatrix1 = matrix4x;
			matrix4x = matrix4x3;
		}
		if (drawBaseMesh && simpleMaterial != null)
		{
			for (int i = 0; i < baseMesh.subMeshCount; i++)
			{
				Graphics.DrawMesh(baseMesh, matrix4x, simpleMaterial, 0, null, i, null, castShadows, receiveShadows);
			}
		}
		if (drawMorphedBaseMesh && simpleMaterial != null)
		{
			for (int j = 0; j < _morphedBaseMesh.subMeshCount; j++)
			{
				Graphics.DrawMesh(_morphedBaseMesh, matrix4x, simpleMaterial, 0, null, j, null, castShadows, receiveShadows);
			}
		}
		if (debugGrafting && meshGraft != null && (bool)graftTo)
		{
			Vector3[] normals = baseMesh.normals;
			DAZMeshGraftVertexPair[] vertexPairs = meshGraft.vertexPairs;
			foreach (DAZMeshGraftVertexPair dAZMeshGraftVertexPair in vertexPairs)
			{
				Vector3 point = graftTo.morphedUVVertices[dAZMeshGraftVertexPair.graftToVertexNum];
				Vector3 vector = matrix4x.MultiplyPoint(point);
				Vector3 end = vector + normals[dAZMeshGraftVertexPair.vertexNum] * 0.01f;
				Debug.DrawLine(vector, end, Color.red);
			}
		}
		if (drawUVMappedMesh)
		{
			for (int l = 0; l < uvMappedMesh.subMeshCount; l++)
			{
				Material material = null;
				if (materials != null)
				{
					material = materials[l];
				}
				if (material == null || useSimpleMaterial)
				{
					material = simpleMaterial;
				}
				if (material != null && materialsEnabled[l])
				{
					Graphics.DrawMesh(uvMappedMesh, matrix4x, material, 0, null, l, null, castShadows && materialsShadowCastEnabled[l], receiveShadows);
				}
			}
		}
		if (_drawMorphedUVMappedMesh || (drawInEditorWhenNotPlaying && !Application.isPlaying))
		{
			DrawMorphedUVMappedMesh(matrix4x);
		}
		if (!_drawVisibleMorphedUVMappedMesh)
		{
			return;
		}
		for (int m = 0; m < _visibleMorphedUVMappedMesh.subMeshCount; m++)
		{
			Material material2 = null;
			if (materials != null)
			{
				material2 = materials[m];
			}
			if (material2 == null || useSimpleMaterial)
			{
				material2 = simpleMaterial;
			}
			if (material2 != null && materialsEnabled[m])
			{
				Graphics.DrawMesh(_visibleMorphedUVMappedMesh, matrix4x, material2, 0, null, m, null, castShadows && materialsShadowCastEnabled[m], receiveShadows);
			}
		}
	}

	public virtual void Init()
	{
		if (!_wasInit && _baseVertices != null)
		{
			_wasInit = true;
			DeriveMeshes();
		}
	}

	private void LateUpdate()
	{
		Draw();
	}

	private void OnDisable()
	{
	}

	private void OnEnable()
	{
		Init();
		if (morphBank != null)
		{
			morphBank.connectedMesh = this;
		}
	}

	private void Start()
	{
		Init();
	}
}
