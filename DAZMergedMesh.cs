using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DAZMergedMesh : DAZMesh
{
	public enum GraftMethod
	{
		Closest,
		Boundary,
		ClosestPoly,
		ClosestVertAndPoly
	}

	public enum GraftSymmetryAxis
	{
		X,
		Y,
		Z
	}

	[Serializable]
	protected class FreeVertGraftWeight
	{
		public int freeVert;

		public int graftVert;

		public float weight;

		public int graftPoly;

		public float graftVertToPolyRatio;
	}

	public DAZMesh targetMesh;

	public DAZMesh graftMesh;

	public DAZMesh graft2Mesh;

	[SerializeField]
	private bool hasGraft2;

	public bool staticMesh;

	public DAZMergedMesh copyGraftOptionsFromMesh;

	public GraftSymmetryAxis graftSymmetryAxis;

	public bool useGraftSymmetry = true;

	public float graftSymmetryDistance = 0.001f;

	public bool graftToCenterlineVerts;

	public GraftMethod graftMethod;

	private bool graftFactorsDirty;

	[SerializeField]
	private float _graftXFactor = 0.2f;

	[SerializeField]
	private float _graftYFactor = 1f;

	[SerializeField]
	private float _graftZFactor = 0.5f;

	public string[] graftMeshMorphNamesForGrafting;

	public float[] graftMeshMorphValuesForGrafting;

	public bool drawGraftMorphedMesh;

	[SerializeField]
	private float[] _graftWeights;

	[SerializeField]
	private float[] _graft2Weights;

	[SerializeField]
	private bool[] _graftIsFreeVert;

	[SerializeField]
	private bool[] _graft2IsFreeVert;

	[SerializeField]
	private FreeVertGraftWeight[] _freeVertGraftWeights;

	public float freeVertexDistance = 0.1f;

	public int numTargetBaseVertices;

	public int numGraftBaseVertices;

	public int numGraftUVVertices;

	public int numGraft2BaseVertices;

	public int numTargetUVVertices;

	public int startGraftVertIndex;

	public int startGraft2VertIndex;

	private Vector3[] _graftMovements;

	private Vector3[] _graft2Movements;

	private Vector3[] _graftMovements2;

	private Vector3[] _graft2Movements2;

	private Mesh _graftMorphedMesh;

	private Vector3[] _graftMorphedMeshVertices;

	protected bool isPlaying;

	public bool has2ndGraft => hasGraft2;

	public float graftXFactor
	{
		get
		{
			return _graftXFactor;
		}
		set
		{
			if (_graftXFactor != value)
			{
				_graftXFactor = value;
				graftFactorsDirty = true;
			}
		}
	}

	public float graftYFactor
	{
		get
		{
			return _graftYFactor;
		}
		set
		{
			if (_graftYFactor != value)
			{
				_graftYFactor = value;
				graftFactorsDirty = true;
			}
		}
	}

	public float graftZFactor
	{
		get
		{
			return _graftZFactor;
		}
		set
		{
			if (_graftZFactor != value)
			{
				_graftZFactor = value;
				graftFactorsDirty = true;
			}
		}
	}

	public void CopyGraftOptions()
	{
		if (copyGraftOptionsFromMesh != null)
		{
			graftMethod = copyGraftOptionsFromMesh.graftMethod;
			graftSymmetryAxis = copyGraftOptionsFromMesh.graftSymmetryAxis;
			useGraftSymmetry = copyGraftOptionsFromMesh.useGraftSymmetry;
			graftSymmetryDistance = copyGraftOptionsFromMesh.graftSymmetryDistance;
			graftToCenterlineVerts = copyGraftOptionsFromMesh.graftToCenterlineVerts;
			freeVertexDistance = copyGraftOptionsFromMesh.freeVertexDistance;
			graftXFactor = copyGraftOptionsFromMesh.graftXFactor;
			graftYFactor = copyGraftOptionsFromMesh.graftYFactor;
			graftZFactor = copyGraftOptionsFromMesh.graftZFactor;
			graftMeshMorphNamesForGrafting = new string[copyGraftOptionsFromMesh.graftMeshMorphNamesForGrafting.Length];
			for (int i = 0; i < copyGraftOptionsFromMesh.graftMeshMorphNamesForGrafting.Length; i++)
			{
				graftMeshMorphNamesForGrafting[i] = copyGraftOptionsFromMesh.graftMeshMorphNamesForGrafting[i];
			}
			graftMeshMorphValuesForGrafting = new float[copyGraftOptionsFromMesh.graftMeshMorphValuesForGrafting.Length];
			for (int j = 0; j < copyGraftOptionsFromMesh.graftMeshMorphValuesForGrafting.Length; j++)
			{
				graftMeshMorphValuesForGrafting[j] = copyGraftOptionsFromMesh.graftMeshMorphValuesForGrafting[j];
			}
		}
	}

	public override void DeriveMeshes()
	{
		base.DeriveMeshes();
		_graftMovements = new Vector3[numGraftBaseVertices];
		_graftMovements2 = new Vector3[numGraftBaseVertices];
		if (hasGraft2)
		{
			_graft2Movements = new Vector3[numGraft2BaseVertices];
			_graft2Movements2 = new Vector3[numGraft2BaseVertices];
		}
		UpdateVertices(force: true);
		if (graftMesh != null)
		{
			Vector3[] array = graftMesh.baseVertices;
			_graftMorphedMeshVertices = new Vector3[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				ref Vector3 reference = ref _graftMorphedMeshVertices[i];
				reference = array[i];
			}
			Mesh mesh = graftMesh.baseMesh;
			_graftMorphedMesh = new Mesh();
			_graftMorphedMesh.vertices = _graftMorphedMeshVertices;
			_graftMorphedMesh.subMeshCount = graftMesh.numMaterials;
			for (int j = 0; j < graftMesh.numMaterials; j++)
			{
				_graftMorphedMesh.SetIndices(mesh.GetIndices(j), MeshTopology.Triangles, j);
			}
			_graftMorphedMesh.RecalculateBounds();
			_graftMorphedMesh.normals = mesh.normals;
		}
	}

	public void CalculateFreeVertGraftWeightsViaClosest()
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		DAZMeshGraftVertexPair[] vertexPairs = graftMesh.meshGraft.vertexPairs;
		foreach (DAZMeshGraftVertexPair dAZMeshGraftVertexPair in vertexPairs)
		{
			dictionary.Add(dAZMeshGraftVertexPair.vertexNum, dAZMeshGraftVertexPair.graftToVertexNum);
		}
		MeshPoly[] array = targetMesh.basePolyList;
		Vector3[] array2 = targetMesh.baseVertices;
		Vector3[] array3 = graftMesh.baseVertices;
		Vector3[] array4 = new Vector3[array3.Length];
		Vector3[] array5 = new Vector3[array.Length];
		List<int> list = new List<int>();
		int[] hiddenPolys = graftMesh.meshGraft.hiddenPolys;
		foreach (int num in hiddenPolys)
		{
			int[] vertices = array[num].vertices;
			foreach (int item in vertices)
			{
				list.Add(item);
			}
		}
		for (int l = 0; l < array3.Length; l++)
		{
			ref Vector3 reference = ref array4[l];
			reference = array3[l];
		}
		if (graftMeshMorphNamesForGrafting != null && graftMesh.morphBank != null)
		{
			if (graftMeshMorphNamesForGrafting.Length == graftMeshMorphValuesForGrafting.Length)
			{
				for (int m = 0; m < graftMeshMorphNamesForGrafting.Length; m++)
				{
					float num2 = graftMeshMorphValuesForGrafting[m];
					DAZMorph morph = graftMesh.morphBank.GetMorph(graftMeshMorphNamesForGrafting[m]);
					if (morph != null && morph.deltas.Length > 0)
					{
						DAZMorphVertex[] deltas = morph.deltas;
						foreach (DAZMorphVertex dAZMorphVertex in deltas)
						{
							Vector3 vector = dAZMorphVertex.delta * num2;
							array4[dAZMorphVertex.vertex] += vector;
						}
					}
					else
					{
						Debug.LogError("Could not find graft morph " + graftMeshMorphNamesForGrafting[m]);
					}
				}
			}
			else
			{
				Debug.LogError("Graft mesh morph names and morph values are not same length");
			}
		}
		int[] hiddenPolys2 = graftMesh.meshGraft.hiddenPolys;
		foreach (int num4 in hiddenPolys2)
		{
			int[] vertices2 = array[num4].vertices;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			int num8 = vertices2.Length;
			float num9 = 1f / (float)num8;
			for (int num10 = 0; num10 < num8; num10++)
			{
				int num11 = vertices2[num10];
				num5 += array2[num11].x * num9;
				num6 += array2[num11].y * num9;
				num7 += array2[num11].z * num9;
			}
			array5[num4].x = num5;
			array5[num4].y = num6;
			array5[num4].z = num7;
		}
		_freeVertGraftWeights = new FreeVertGraftWeight[numGraftBaseVertices];
		for (int num12 = 0; num12 < numGraftBaseVertices; num12++)
		{
			float num13 = 1000000f;
			float num14 = 1000000f;
			int graftVert = -1;
			int graftPoly = -1;
			int[] hiddenPolys3 = graftMesh.meshGraft.hiddenPolys;
			foreach (int num16 in hiddenPolys3)
			{
				Vector3 vector2 = array5[num16];
				float magnitude = (vector2 - array4[num12]).magnitude;
				if (magnitude < num14)
				{
					graftPoly = num16;
					num14 = magnitude;
				}
			}
			float num17 = 0f;
			switch (graftSymmetryAxis)
			{
			case GraftSymmetryAxis.X:
				num17 = Mathf.Abs(array4[num12].x);
				break;
			case GraftSymmetryAxis.Y:
				num17 = Mathf.Abs(array4[num12].y);
				break;
			case GraftSymmetryAxis.Z:
				num17 = Mathf.Abs(array4[num12].z);
				break;
			}
			if (useGraftSymmetry)
			{
				if (num17 < graftSymmetryDistance)
				{
					foreach (int item2 in list)
					{
						Vector3 vector3 = array2[item2];
						float num18 = 0f;
						switch (graftSymmetryAxis)
						{
						case GraftSymmetryAxis.X:
							num18 = Mathf.Abs(vector3.x);
							break;
						case GraftSymmetryAxis.Y:
							num18 = Mathf.Abs(vector3.y);
							break;
						case GraftSymmetryAxis.Z:
							num18 = Mathf.Abs(vector3.z);
							break;
						}
						if (num18 < graftSymmetryDistance)
						{
							float magnitude2 = (vector3 - array4[num12]).magnitude;
							if (magnitude2 < num13)
							{
								graftVert = item2;
								num13 = magnitude2;
							}
						}
					}
				}
				else
				{
					foreach (int item3 in list)
					{
						Vector3 vector4 = array2[item3];
						float num19 = 0f;
						switch (graftSymmetryAxis)
						{
						case GraftSymmetryAxis.X:
							num19 = Mathf.Abs(vector4.x);
							break;
						case GraftSymmetryAxis.Y:
							num19 = Mathf.Abs(vector4.y);
							break;
						case GraftSymmetryAxis.Z:
							num19 = Mathf.Abs(vector4.z);
							break;
						}
						if (graftToCenterlineVerts || (!graftToCenterlineVerts && num19 > graftSymmetryDistance))
						{
							float magnitude3 = (vector4 - array4[num12]).magnitude;
							if (magnitude3 < num13)
							{
								graftVert = item3;
								num13 = magnitude3;
							}
						}
					}
				}
			}
			else
			{
				foreach (int item4 in list)
				{
					Vector3 vector5 = array2[item4];
					float magnitude4 = (vector5 - array4[num12]).magnitude;
					if (magnitude4 < num13)
					{
						graftVert = item4;
						num13 = magnitude4;
					}
				}
			}
			FreeVertGraftWeight freeVertGraftWeight = new FreeVertGraftWeight();
			freeVertGraftWeight.freeVert = num12;
			if (freeVertexDistance != 0f)
			{
				float num20 = (freeVertGraftWeight.weight = Mathf.Clamp01(1f - num13 / freeVertexDistance));
			}
			else
			{
				freeVertGraftWeight.weight = 0f;
			}
			freeVertGraftWeight.graftVert = graftVert;
			freeVertGraftWeight.graftPoly = graftPoly;
			if (useGraftSymmetry)
			{
				switch (graftSymmetryAxis)
				{
				case GraftSymmetryAxis.X:
					num17 = Mathf.Abs(array4[num12].x);
					break;
				case GraftSymmetryAxis.Y:
					num17 = Mathf.Abs(array4[num12].y);
					break;
				case GraftSymmetryAxis.Z:
					num17 = Mathf.Abs(array4[num12].z);
					break;
				}
				if (num17 < graftSymmetryDistance)
				{
					freeVertGraftWeight.graftVertToPolyRatio = 0f;
				}
				else
				{
					freeVertGraftWeight.graftVertToPolyRatio = num13 / (num13 + num14);
				}
			}
			else
			{
				freeVertGraftWeight.graftVertToPolyRatio = num13 / (num13 + num14);
			}
			_freeVertGraftWeights[num12] = freeVertGraftWeight;
		}
	}

	public void Merge()
	{
		meshSmooth = null;
		DAZMesh[] components = GetComponents<DAZMesh>();
		bool flag = false;
		hasGraft2 = false;
		DAZMesh[] array = components;
		foreach (DAZMesh dAZMesh in array)
		{
			if (dAZMesh.meshGraft == null || !(dAZMesh.graftTo != null) || !(dAZMesh != this))
			{
				continue;
			}
			if (flag)
			{
				hasGraft2 = true;
				graft2Mesh = dAZMesh;
				if (dAZMesh.graftTo != targetMesh)
				{
					Debug.LogError("2nd graft mesh " + dAZMesh.geometryId + " uses a different target mesh " + dAZMesh.graftTo.geometryId + " than 1st graft mesh " + targetMesh.geometryId);
					Debug.LogError("Merge aborted");
					return;
				}
				geometryId = geometryId + ":" + graft2Mesh.geometryId;
			}
			else
			{
				flag = true;
				graftMesh = dAZMesh;
				targetMesh = dAZMesh.graftTo;
				geometryId = targetMesh.geometryId + ":" + graftMesh.geometryId;
			}
		}
		if (!(targetMesh != null) || !(graftMesh != null))
		{
			return;
		}
		Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		int[] hiddenPolys = graftMesh.meshGraft.hiddenPolys;
		foreach (int key in hiddenPolys)
		{
			dictionary.Add(key, value: true);
		}
		if (hasGraft2)
		{
			int[] hiddenPolys2 = graft2Mesh.meshGraft.hiddenPolys;
			foreach (int key2 in hiddenPolys2)
			{
				dictionary.Add(key2, value: true);
			}
		}
		Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
		DAZMeshGraftVertexPair[] vertexPairs = graftMesh.meshGraft.vertexPairs;
		foreach (DAZMeshGraftVertexPair dAZMeshGraftVertexPair in vertexPairs)
		{
			dictionary2.Add(dAZMeshGraftVertexPair.vertexNum, dAZMeshGraftVertexPair.graftToVertexNum);
		}
		Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
		if (hasGraft2)
		{
			DAZMeshGraftVertexPair[] vertexPairs2 = graft2Mesh.meshGraft.vertexPairs;
			foreach (DAZMeshGraftVertexPair dAZMeshGraftVertexPair2 in vertexPairs2)
			{
				dictionary3.Add(dAZMeshGraftVertexPair2.vertexNum, dAZMeshGraftVertexPair2.graftToVertexNum);
			}
		}
		Dictionary<int, int> dictionary4 = new Dictionary<int, int>();
		List<DAZVertexMap> list = new List<DAZVertexMap>();
		numTargetBaseVertices = targetMesh.numBaseVertices;
		numTargetUVVertices = targetMesh.numUVVertices;
		numGraftBaseVertices = graftMesh.numBaseVertices;
		if (hasGraft2)
		{
			numGraft2BaseVertices = graft2Mesh.numBaseVertices;
		}
		else
		{
			numGraft2BaseVertices = 0;
		}
		numGraftUVVertices = graftMesh.numUVVertices;
		startGraftVertIndex = numTargetBaseVertices;
		if (hasGraft2)
		{
			startGraft2VertIndex = numTargetBaseVertices + numGraftBaseVertices;
		}
		if (hasGraft2)
		{
			_numBaseVertices = numTargetBaseVertices + numGraftBaseVertices + numGraft2BaseVertices;
		}
		else
		{
			_numBaseVertices = numTargetBaseVertices + numGraftBaseVertices;
		}
		_baseVertices = new Vector3[_numBaseVertices];
		Vector3[] array2 = targetMesh.baseVertices;
		for (int n = 0; n < numTargetBaseVertices; n++)
		{
			ref Vector3 reference = ref _baseVertices[n];
			reference = array2[n];
		}
		Vector3[] array3 = graftMesh.baseVertices;
		DAZMeshGraftVertexPair[] vertexPairs3 = graftMesh.meshGraft.vertexPairs;
		int num = vertexPairs3.Length;
		_graftWeights = new float[numGraftBaseVertices * num];
		_graftIsFreeVert = new bool[numGraftBaseVertices];
		CalculateFreeVertGraftWeightsViaClosest();
		for (int num2 = 0; num2 < numGraftBaseVertices; num2++)
		{
			if (dictionary2.TryGetValue(num2, out var value))
			{
				_graftIsFreeVert[num2] = false;
				DAZVertexMap dAZVertexMap = new DAZVertexMap();
				dAZVertexMap.fromvert = value;
				dAZVertexMap.tovert = startGraftVertIndex + num2;
				list.Add(dAZVertexMap);
				dictionary4.Add(dAZVertexMap.tovert, dAZVertexMap.fromvert);
				ref Vector3 reference2 = ref _baseVertices[startGraftVertIndex + num2];
				reference2 = array2[value];
				continue;
			}
			_graftIsFreeVert[num2] = true;
			ref Vector3 reference3 = ref _baseVertices[startGraftVertIndex + num2];
			reference3 = array3[num2];
			float num3 = 0f;
			for (int num4 = 0; num4 < num; num4++)
			{
				int vertexNum = vertexPairs3[num4].vertexNum;
				float num5 = array3[num2].x - array3[vertexNum].x;
				float num6 = array3[num2].y - array3[vertexNum].y;
				float num7 = array3[num2].z - array3[vertexNum].z;
				float num8 = num5 * num5 + num6 * num6 + num7 * num7;
				float num9 = num8 * num8;
				num3 += num9;
				int num10 = num4 * numGraftBaseVertices + num2;
				_graftWeights[num10] = num9;
			}
			float num11 = 0f;
			for (int num12 = 0; num12 < num; num12++)
			{
				int num13 = num12 * numGraftBaseVertices + num2;
				float num14 = num3 / _graftWeights[num13];
				_graftWeights[num13] = num14;
				num11 += num14;
			}
			float num15 = 1f / num11;
			for (int num16 = 0; num16 < num; num16++)
			{
				int num17 = num16 * numGraftBaseVertices + num2;
				_graftWeights[num17] *= num15;
			}
		}
		if (hasGraft2)
		{
			Vector3[] array4 = graft2Mesh.baseVertices;
			DAZMeshGraftVertexPair[] vertexPairs4 = graft2Mesh.meshGraft.vertexPairs;
			int num18 = vertexPairs4.Length;
			_graft2Weights = new float[numGraft2BaseVertices * num18];
			_graft2IsFreeVert = new bool[numGraft2BaseVertices];
			for (int num19 = 0; num19 < numGraft2BaseVertices; num19++)
			{
				if (dictionary3.TryGetValue(num19, out var value2))
				{
					_graft2IsFreeVert[num19] = false;
					DAZVertexMap dAZVertexMap2 = new DAZVertexMap();
					dAZVertexMap2.fromvert = value2;
					dAZVertexMap2.tovert = startGraft2VertIndex + num19;
					list.Add(dAZVertexMap2);
					dictionary4.Add(dAZVertexMap2.tovert, dAZVertexMap2.fromvert);
					ref Vector3 reference4 = ref _baseVertices[startGraft2VertIndex + num19];
					reference4 = array2[value2];
					continue;
				}
				_graft2IsFreeVert[num19] = true;
				ref Vector3 reference5 = ref _baseVertices[startGraft2VertIndex + num19];
				reference5 = array4[num19];
				float num20 = 0f;
				for (int num21 = 0; num21 < num18; num21++)
				{
					int vertexNum2 = vertexPairs4[num21].vertexNum;
					float num22 = array4[num19].x - array4[vertexNum2].x;
					float num23 = array4[num19].y - array4[vertexNum2].y;
					float num24 = array4[num19].z - array4[vertexNum2].z;
					float num25 = num22 * num22 + num23 * num23 + num24 * num24;
					float num26 = num25 * num25;
					num20 += num26;
					int num27 = num21 * numGraft2BaseVertices + num19;
					_graft2Weights[num27] = num26;
				}
				float num28 = 0f;
				for (int num29 = 0; num29 < num18; num29++)
				{
					int num30 = num29 * numGraft2BaseVertices + num19;
					float num31 = num20 / _graft2Weights[num30];
					_graft2Weights[num30] = num31;
					num28 += num31;
				}
				float num32 = 1f / num28;
				for (int num33 = 0; num33 < num18; num33++)
				{
					int num34 = num33 * numGraft2BaseVertices + num19;
					_graft2Weights[num34] *= num32;
				}
			}
		}
		int num35 = targetMesh.numMaterials;
		_numMaterials = num35 + graftMesh.numMaterials;
		if (hasGraft2)
		{
			_numMaterials += graft2Mesh.numMaterials;
		}
		materials = new Material[_numMaterials];
		materialsEnabled = new bool[_numMaterials];
		_materialNames = new string[_numMaterials];
		string[] array5 = targetMesh.materialNames;
		string[] array6 = graftMesh.materialNames;
		string[] array7 = null;
		if (hasGraft2)
		{
			array7 = graft2Mesh.materialNames;
		}
		for (int num36 = 0; num36 < targetMesh.numMaterials; num36++)
		{
			materials[num36] = targetMesh.materials[num36];
			materialsEnabled[num36] = true;
			_materialNames[num36] = array5[num36];
		}
		int num37 = graftMesh.numMaterials;
		for (int num38 = 0; num38 < num37; num38++)
		{
			materials[num35 + num38] = graftMesh.materials[num38];
			materialsEnabled[num35 + num38] = true;
			_materialNames[num35 + num38] = array6[num38];
		}
		if (hasGraft2)
		{
			for (int num39 = 0; num39 < graft2Mesh.numMaterials; num39++)
			{
				materials[num35 + num37 + num39] = graft2Mesh.materials[num39];
				materialsEnabled[num35 + num37 + num39] = true;
				_materialNames[num35 + num37 + num39] = array7[num39];
			}
		}
		MeshPoly[] array8 = targetMesh.basePolyList;
		MeshPoly[] array9 = graftMesh.basePolyList;
		MeshPoly[] array10 = null;
		if (hasGraft2)
		{
			array10 = graft2Mesh.basePolyList;
		}
		int num40 = array8.Length;
		int num41 = array9.Length;
		int num42 = 0;
		if (hasGraft2)
		{
			num42 = array10.Length;
		}
		_numBasePolygons = num40 + num41 - graftMesh.meshGraft.hiddenPolys.Length;
		if (hasGraft2)
		{
			_numBasePolygons += num42 - graft2Mesh.meshGraft.hiddenPolys.Length;
		}
		_basePolyList = new MeshPoly[_numBasePolygons];
		int num43 = 0;
		for (int num44 = 0; num44 < num40; num44++)
		{
			if (!dictionary.TryGetValue(num44, out var _))
			{
				MeshPoly meshPoly = array8[num44];
				_basePolyList[num43] = meshPoly;
				num43++;
			}
		}
		for (int num45 = 0; num45 < num41; num45++)
		{
			MeshPoly meshPoly2 = array9[num45];
			MeshPoly meshPoly3 = new MeshPoly();
			meshPoly3.materialNum = meshPoly2.materialNum + num35;
			meshPoly3.vertices = new int[meshPoly2.vertices.Length];
			for (int num46 = 0; num46 < meshPoly2.vertices.Length; num46++)
			{
				if (dictionary2.TryGetValue(meshPoly2.vertices[num46], out var value4))
				{
					meshPoly3.vertices[num46] = value4;
				}
				else
				{
					meshPoly3.vertices[num46] = meshPoly2.vertices[num46] + startGraftVertIndex;
				}
			}
			_basePolyList[num43] = meshPoly3;
			num43++;
		}
		if (hasGraft2)
		{
			for (int num47 = 0; num47 < num42; num47++)
			{
				MeshPoly meshPoly4 = array10[num47];
				MeshPoly meshPoly5 = new MeshPoly();
				meshPoly5.materialNum = meshPoly4.materialNum + num35 + num37;
				meshPoly5.vertices = new int[meshPoly4.vertices.Length];
				for (int num48 = 0; num48 < meshPoly4.vertices.Length; num48++)
				{
					if (dictionary3.TryGetValue(meshPoly4.vertices[num48], out var value5))
					{
						meshPoly5.vertices[num48] = value5;
					}
					else
					{
						meshPoly5.vertices[num48] = meshPoly4.vertices[num48] + startGraft2VertIndex;
					}
				}
				_basePolyList[num43] = meshPoly5;
				num43++;
			}
		}
		_numUVVertices = numTargetUVVertices + numGraftUVVertices;
		if (hasGraft2)
		{
			_numUVVertices += graft2Mesh.numUVVertices;
		}
		_UV = new Vector2[_numUVVertices];
		_OrigUV = new Vector2[_numUVVertices];
		_UVVertices = new Vector3[_numUVVertices];
		_UVPolyList = new MeshPoly[_numBasePolygons];
		_morphedUVVertices = new Vector3[_numUVVertices];
		Vector2[] uV = targetMesh.UV;
		Vector2[] uV2 = graftMesh.UV;
		Vector2[] array11 = null;
		if (hasGraft2)
		{
			array11 = graft2Mesh.UV;
		}
		Vector3[] uVVertices = targetMesh.UVVertices;
		Vector3[] uVVertices2 = graftMesh.UVVertices;
		Vector3[] array12 = null;
		if (hasGraft2)
		{
			array12 = graft2Mesh.UVVertices;
		}
		Vector3[] array13 = targetMesh.morphedUVVertices;
		Vector3[] array14 = graftMesh.morphedUVVertices;
		Vector3[] array15 = null;
		if (hasGraft2)
		{
			array15 = graft2Mesh.morphedUVVertices;
		}
		DAZVertexMap[] array16 = targetMesh.baseVerticesToUVVertices;
		DAZVertexMap[] array17 = graftMesh.baseVerticesToUVVertices;
		DAZVertexMap[] array18 = null;
		if (hasGraft2)
		{
			array18 = graft2Mesh.baseVerticesToUVVertices;
		}
		int num49 = array16.Length;
		for (int num50 = 0; num50 < num49; num50++)
		{
			DAZVertexMap dAZVertexMap3 = array16[num50];
			DAZVertexMap dAZVertexMap4 = new DAZVertexMap();
			dAZVertexMap4.tovert = dAZVertexMap3.tovert + numGraftBaseVertices + numGraft2BaseVertices;
			if (dictionary4.TryGetValue(dAZVertexMap3.fromvert, out var value6))
			{
				dAZVertexMap4.fromvert = value6;
			}
			else
			{
				dAZVertexMap4.fromvert = dAZVertexMap3.fromvert;
			}
			dictionary4.Add(dAZVertexMap4.tovert, dAZVertexMap4.fromvert);
			list.Add(dAZVertexMap4);
		}
		foreach (DAZVertexMap dAZVertexMap5 in array17)
		{
			DAZVertexMap dAZVertexMap6 = new DAZVertexMap();
			dAZVertexMap6.fromvert = dAZVertexMap5.fromvert + numTargetBaseVertices;
			dAZVertexMap6.tovert = dAZVertexMap5.tovert + numTargetUVVertices + numGraft2BaseVertices;
			if (dictionary4.TryGetValue(dAZVertexMap6.fromvert, out var value7))
			{
				dAZVertexMap6.fromvert = value7;
			}
			dictionary4.Add(dAZVertexMap6.tovert, dAZVertexMap6.fromvert);
			list.Add(dAZVertexMap6);
		}
		if (hasGraft2)
		{
			foreach (DAZVertexMap dAZVertexMap7 in array18)
			{
				DAZVertexMap dAZVertexMap8 = new DAZVertexMap();
				dAZVertexMap8.fromvert = dAZVertexMap7.fromvert + numTargetBaseVertices + numGraftBaseVertices;
				dAZVertexMap8.tovert = dAZVertexMap7.tovert + numTargetUVVertices + numGraftUVVertices;
				if (dictionary4.TryGetValue(dAZVertexMap8.fromvert, out var value8))
				{
					dAZVertexMap8.fromvert = value8;
				}
				dictionary4.Add(dAZVertexMap8.tovert, dAZVertexMap8.fromvert);
				list.Add(dAZVertexMap8);
			}
		}
		for (int num53 = 0; num53 < numTargetUVVertices; num53++)
		{
			int num54 = num53;
			if (num53 >= numTargetBaseVertices)
			{
				num54 += numGraftBaseVertices + numGraft2BaseVertices;
			}
			ref Vector2 reference6 = ref _OrigUV[num54];
			reference6 = uV[num53];
			ref Vector2 reference7 = ref _UV[num54];
			reference7 = uV[num53];
			ref Vector3 reference8 = ref _UVVertices[num54];
			reference8 = uVVertices[num53];
			ref Vector3 reference9 = ref _morphedUVVertices[num54];
			reference9 = array13[num53];
		}
		for (int num55 = 0; num55 < graftMesh.numUVVertices; num55++)
		{
			int num56 = num55 + numTargetBaseVertices;
			if (num55 >= numGraftBaseVertices)
			{
				num56 = num55 + numTargetUVVertices + numGraft2BaseVertices;
			}
			ref Vector2 reference10 = ref _OrigUV[num56];
			reference10 = uV2[num55];
			ref Vector2 reference11 = ref _UV[num56];
			reference11 = uV2[num55];
			ref Vector3 reference12 = ref _UVVertices[num56];
			reference12 = uVVertices2[num55];
			ref Vector3 reference13 = ref _morphedUVVertices[num56];
			reference13 = array14[num55];
		}
		if (hasGraft2)
		{
			for (int num57 = 0; num57 < graft2Mesh.numUVVertices; num57++)
			{
				int num58 = num57 + numTargetBaseVertices + numGraftBaseVertices;
				if (num57 >= numGraft2BaseVertices)
				{
					num58 = num57 + numTargetUVVertices + numGraftUVVertices;
				}
				ref Vector2 reference14 = ref _OrigUV[num58];
				reference14 = array11[num57];
				ref Vector2 reference15 = ref _UV[num58];
				reference15 = array11[num57];
				ref Vector3 reference16 = ref _UVVertices[num58];
				reference16 = array12[num57];
				ref Vector3 reference17 = ref _morphedUVVertices[num58];
				reference17 = array15[num57];
			}
		}
		MeshPoly[] uVPolyList = targetMesh.UVPolyList;
		MeshPoly[] uVPolyList2 = graftMesh.UVPolyList;
		MeshPoly[] array19 = null;
		if (hasGraft2)
		{
			array19 = graft2Mesh.UVPolyList;
		}
		num43 = 0;
		for (int num59 = 0; num59 < num40; num59++)
		{
			if (dictionary.TryGetValue(num59, out var _))
			{
				continue;
			}
			MeshPoly meshPoly6 = uVPolyList[num59];
			MeshPoly meshPoly7 = new MeshPoly();
			meshPoly7.materialNum = meshPoly6.materialNum;
			meshPoly7.vertices = new int[meshPoly6.vertices.Length];
			for (int num60 = 0; num60 < meshPoly6.vertices.Length; num60++)
			{
				if (meshPoly6.vertices[num60] >= numTargetBaseVertices)
				{
					meshPoly7.vertices[num60] = meshPoly6.vertices[num60] + numGraftBaseVertices + numGraft2BaseVertices;
				}
				else
				{
					meshPoly7.vertices[num60] = meshPoly6.vertices[num60];
				}
			}
			_UVPolyList[num43] = meshPoly7;
			num43++;
		}
		for (int num61 = 0; num61 < num41; num61++)
		{
			MeshPoly meshPoly8 = uVPolyList2[num61];
			MeshPoly meshPoly9 = new MeshPoly();
			meshPoly9.materialNum = meshPoly8.materialNum + num35;
			meshPoly9.vertices = new int[meshPoly8.vertices.Length];
			for (int num62 = 0; num62 < meshPoly8.vertices.Length; num62++)
			{
				if (meshPoly8.vertices[num62] >= numGraftBaseVertices)
				{
					meshPoly9.vertices[num62] = meshPoly8.vertices[num62] + numTargetUVVertices + numGraft2BaseVertices;
				}
				else
				{
					meshPoly9.vertices[num62] = meshPoly8.vertices[num62] + numTargetBaseVertices;
				}
			}
			_UVPolyList[num43] = meshPoly9;
			num43++;
		}
		if (hasGraft2)
		{
			for (int num63 = 0; num63 < num42; num63++)
			{
				MeshPoly meshPoly10 = array19[num63];
				MeshPoly meshPoly11 = new MeshPoly();
				meshPoly11.materialNum = meshPoly10.materialNum + num35 + num37;
				meshPoly11.vertices = new int[meshPoly10.vertices.Length];
				for (int num64 = 0; num64 < meshPoly10.vertices.Length; num64++)
				{
					if (meshPoly10.vertices[num64] >= numGraft2BaseVertices)
					{
						meshPoly11.vertices[num64] = meshPoly10.vertices[num64] + numTargetUVVertices + numGraftUVVertices;
					}
					else
					{
						meshPoly11.vertices[num64] = meshPoly10.vertices[num64] + numTargetBaseVertices + numGraftBaseVertices;
					}
				}
				_UVPolyList[num43] = meshPoly11;
				num43++;
			}
		}
		_baseVerticesToUVVertices = list.ToArray();
		DeriveMeshes();
	}

	public new void RecalculateMorphedMeshTangents(bool forceAll = false)
	{
		base.RecalculateMorphedMeshTangents(forceAll);
		DAZMeshGraftVertexPair[] vertexPairs = graftMesh.meshGraft.vertexPairs;
		int num = vertexPairs.Length;
		for (int i = 0; i < num; i++)
		{
			int graftToVertexNum = vertexPairs[i].graftToVertexNum;
			int vertexNum = vertexPairs[i].vertexNum;
			ref Vector4 reference = ref _morphedUVTangents[graftToVertexNum];
			reference = _morphedUVTangents[startGraftVertIndex + vertexNum];
		}
	}

	public void UpdateVertices(bool force = false)
	{
		UpdateVerticesPre();
		UpdateVerticesThreaded(force);
		UpdateVerticesPost();
	}

	public void UpdateVerticesPre()
	{
		_verticesChangedLastFrame = _verticesChangedThisFrame;
		_verticesChangedThisFrame = false;
		_visibleVerticesChangedLastFrame = _visibleVerticesChangedThisFrame;
		_visibleVerticesChangedThisFrame = false;
	}

	public void UpdateVerticesThreaded(bool force = false)
	{
		if (targetMesh == null || graftMesh == null)
		{
			return;
		}
		if (graftFactorsDirty)
		{
			force = true;
			graftFactorsDirty = false;
		}
		bool flag = false;
		Vector3[] array = targetMesh.morphedUVVertices;
		Vector3[] array2 = targetMesh.visibleMorphedUVVertices;
		Vector3[] array3 = targetMesh.morphedBaseVertices;
		int num = graftMesh.numUVVertices;
		if (targetMesh.verticesChangedThisFrame || force)
		{
			Vector3[] array4 = targetMesh.morphedUVNormals;
			for (int i = 0; i < numTargetBaseVertices; i++)
			{
				ref Vector3 reference = ref _morphedUVVertices[i];
				reference = array[i];
				ref Vector3 reference2 = ref _visibleMorphedUVVertices[i];
				reference2 = array2[i];
				ref Vector3 reference3 = ref _morphedBaseVertices[i];
				reference3 = array3[i];
				if ((targetMesh.recalcNormalsOnMorph && targetMesh.normalsDirtyThisFrame) || force)
				{
					flag = true;
					ref Vector3 reference4 = ref _morphedUVNormals[i];
					reference4 = array4[i];
				}
			}
			if ((targetMesh.recalcTangentsOnMorph && targetMesh.tangentsDirtyThisFrame) || force)
			{
				Vector4[] array5 = targetMesh.morphedUVTangents;
				for (int j = 0; j < numTargetUVVertices; j++)
				{
					int num2 = j;
					if (j >= numTargetBaseVertices)
					{
						num2 += numGraftBaseVertices + numGraft2BaseVertices;
					}
					ref Vector4 reference5 = ref _morphedUVTangents[num2];
					reference5 = array5[j];
				}
			}
		}
		if ((graftMesh.normalsDirtyThisFrame && graftMesh.recalcNormalsOnMorph) || force)
		{
			flag = true;
			Vector3[] array6 = graftMesh.morphedUVNormals;
			for (int k = 0; k < numGraftBaseVertices; k++)
			{
				ref Vector3 reference6 = ref _morphedUVNormals[startGraftVertIndex + k];
				reference6 = array6[k];
			}
		}
		if ((graftMesh.tangentsDirtyThisFrame && graftMesh.recalcTangentsOnMorph) || force)
		{
			Vector4[] array7 = graftMesh.morphedUVTangents;
			for (int l = 0; l < graftMesh.numUVVertices; l++)
			{
				int num3 = ((l < numGraftBaseVertices) ? (l + startGraftVertIndex) : (l + numTargetUVVertices + numGraft2BaseVertices));
				ref Vector4 reference7 = ref _morphedUVTangents[num3];
				reference7 = array7[l];
			}
		}
		if (hasGraft2 && ((graft2Mesh.normalsDirtyThisFrame && graftMesh.recalcNormalsOnMorph) || force))
		{
			flag = true;
			Vector3[] array8 = graft2Mesh.morphedUVNormals;
			for (int m = 0; m < numGraft2BaseVertices; m++)
			{
				ref Vector3 reference8 = ref _morphedUVNormals[startGraft2VertIndex + m];
				reference8 = array8[m];
			}
		}
		if (hasGraft2 && ((graft2Mesh.tangentsDirtyThisFrame && graft2Mesh.recalcTangentsOnMorph) || force))
		{
			Vector4[] array9 = graft2Mesh.morphedUVTangents;
			for (int n = 0; n < graft2Mesh.numUVVertices; n++)
			{
				int num4 = ((n < numGraft2BaseVertices) ? (n + startGraft2VertIndex) : (n + numTargetUVVertices + num));
				ref Vector4 reference9 = ref _morphedUVTangents[num4];
				reference9 = array9[n];
			}
		}
		if (flag || force)
		{
			_updateDuplicateMorphedUVNormals();
		}
		bool flag2 = false;
		Vector3[] array10 = graftMesh.morphedUVVertices;
		Vector3[] array11 = graftMesh.visibleMorphedUVVertices;
		Vector3[] array12 = targetMesh.baseVertices;
		DAZMeshGraftVertexPair[] vertexPairs = graftMesh.meshGraft.vertexPairs;
		int num5 = vertexPairs.Length;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		float num11 = 0f;
		if (targetMesh.verticesChangedThisFrame || force)
		{
			for (int num12 = 0; num12 < num5; num12++)
			{
				int graftToVertexNum = vertexPairs[num12].graftToVertexNum;
				int vertexNum = vertexPairs[num12].vertexNum;
				int num13 = startGraftVertIndex + vertexNum;
				float num14 = array[graftToVertexNum].x - array12[graftToVertexNum].x;
				float num15 = array[graftToVertexNum].y - array12[graftToVertexNum].y;
				float num16 = array[graftToVertexNum].z - array12[graftToVertexNum].z;
				num6 += num14;
				num7 += num15;
				num8 += num16;
				if (_graftMovements[vertexNum].x != num14)
				{
					flag2 = true;
					_graftMovements[vertexNum].x = num14;
				}
				if (_graftMovements[vertexNum].y != num15)
				{
					flag2 = true;
					_graftMovements[vertexNum].y = num15;
				}
				if (_graftMovements[vertexNum].z != num16)
				{
					flag2 = true;
					_graftMovements[vertexNum].z = num16;
				}
				float num17 = array2[graftToVertexNum].x - array12[graftToVertexNum].x;
				float num18 = array2[graftToVertexNum].y - array12[graftToVertexNum].y;
				float num19 = array2[graftToVertexNum].z - array12[graftToVertexNum].z;
				num9 += num17;
				num10 += num18;
				num11 += num19;
				if (_graftMovements2[vertexNum].x != num17)
				{
					_graftMovements2[vertexNum].x = num17;
				}
				if (_graftMovements2[vertexNum].y != num18)
				{
					_graftMovements2[vertexNum].y = num18;
				}
				if (_graftMovements2[vertexNum].z != num19)
				{
					_graftMovements2[vertexNum].z = num19;
				}
				ref Vector3 reference10 = ref _morphedUVVertices[num13];
				reference10 = array[graftToVertexNum];
				ref Vector3 reference11 = ref _morphedBaseVertices[num13];
				reference11 = array3[graftToVertexNum];
				ref Vector3 reference12 = ref _visibleMorphedUVVertices[num13];
				reference12 = array2[graftToVertexNum];
			}
		}
		float num20 = num6 / (float)num5;
		float num21 = num7 / (float)num5;
		float num22 = num8 / (float)num5;
		float num23 = num9 / (float)num5;
		float num24 = num10 / (float)num5;
		float num25 = num11 / (float)num5;
		morphedNormalsDirty = false;
		_visibleVerticesChangedThisFrame = targetMesh.visibleVerticesChangedThisFrame || graftMesh.visibleVerticesChangedThisFrame || targetMesh.visibleVerticesChangedLastFrame || graftMesh.visibleVerticesChangedLastFrame;
		if (targetMesh.verticesChangedThisFrame || graftMesh.verticesChangedThisFrame || force)
		{
			_verticesChangedThisFrame = true;
			MeshPoly[] array13 = targetMesh.basePolyList;
			switch (graftMethod)
			{
			case GraftMethod.Closest:
			{
				for (int num58 = 0; num58 < numGraftBaseVertices; num58++)
				{
					if (_graftIsFreeVert[num58])
					{
						int num59 = startGraftVertIndex + num58;
						FreeVertGraftWeight freeVertGraftWeight3 = _freeVertGraftWeights[num58];
						int graftVert2 = freeVertGraftWeight3.graftVert;
						float weight3 = freeVertGraftWeight3.weight;
						if (weight3 > 0f)
						{
							float num60 = 1f - weight3;
							float num61 = array[graftVert2].x - array12[graftVert2].x;
							float num62 = array[graftVert2].y - array12[graftVert2].y;
							float num63 = array[graftVert2].z - array12[graftVert2].z;
							_graftMovements[num58].x = num20 * num60 + num61 * weight3;
							_graftMovements[num58].y = num21 * num60 + num62 * weight3;
							_graftMovements[num58].z = num22 * num60 + num63 * weight3;
							float num64 = array2[graftVert2].x - array12[graftVert2].x;
							float num65 = array2[graftVert2].y - array12[graftVert2].y;
							float num66 = array2[graftVert2].z - array12[graftVert2].z;
							_graftMovements2[num58].x = num23 * num60 + num64 * weight3;
							_graftMovements2[num58].y = num24 * num60 + num65 * weight3;
							_graftMovements2[num58].z = num25 * num60 + num66 * weight3;
						}
						else
						{
							_graftMovements[num58].x = num20;
							_graftMovements[num58].y = num21;
							_graftMovements[num58].z = num22;
							_graftMovements2[num58].x = num23;
							_graftMovements2[num58].y = num24;
							_graftMovements2[num58].z = num25;
						}
						_morphedUVVertices[num59].x = array10[num58].x + _graftMovements[num58].x;
						_morphedUVVertices[num59].y = array10[num58].y + _graftMovements[num58].y;
						_morphedUVVertices[num59].z = array10[num58].z + _graftMovements[num58].z;
						ref Vector3 reference16 = ref _morphedBaseVertices[num59];
						reference16 = _morphedUVVertices[num59];
						_visibleMorphedUVVertices[num59].x = array10[num58].x + _graftMovements2[num58].x;
						_visibleMorphedUVVertices[num59].y = array10[num58].y + _graftMovements2[num58].y;
						_visibleMorphedUVVertices[num59].z = array10[num58].z + _graftMovements2[num58].z;
					}
				}
				break;
			}
			case GraftMethod.ClosestPoly:
			{
				for (int num31 = 0; num31 < numGraftBaseVertices; num31++)
				{
					if (!_graftIsFreeVert[num31])
					{
						continue;
					}
					int num32 = startGraftVertIndex + num31;
					FreeVertGraftWeight freeVertGraftWeight = _freeVertGraftWeights[num31];
					int graftPoly = freeVertGraftWeight.graftPoly;
					float weight = freeVertGraftWeight.weight;
					if (weight > 0f)
					{
						float num33 = 1f - weight;
						float num34 = 0f;
						float num35 = 0f;
						float num36 = 0f;
						float num37 = 0f;
						float num38 = 0f;
						float num39 = 0f;
						int[] vertices = array13[graftPoly].vertices;
						int num40 = vertices.Length;
						float num41 = 1f / (float)num40;
						for (int num42 = 0; num42 < num40; num42++)
						{
							int num43 = vertices[num42];
							num34 += (array[num43].x - array12[num43].x) * num41;
							num35 += (array[num43].y - array12[num43].y) * num41;
							num36 += (array[num43].z - array12[num43].z) * num41;
							num37 += (array2[num43].x - array12[num43].x) * num41;
							num38 += (array2[num43].y - array12[num43].y) * num41;
							num39 += (array2[num43].z - array12[num43].z) * num41;
						}
						_graftMovements[num31].x = num20 * num33 + num34 * weight;
						_graftMovements[num31].y = num21 * num33 + num35 * weight;
						_graftMovements[num31].z = num22 * num33 + num36 * weight;
						_graftMovements2[num31].x = num23 * num33 + num37 * weight;
						_graftMovements2[num31].y = num24 * num33 + num38 * weight;
						_graftMovements2[num31].z = num25 * num33 + num39 * weight;
					}
					else
					{
						_graftMovements[num31].x = num20;
						_graftMovements[num31].y = num21;
						_graftMovements[num31].z = num22;
						_graftMovements2[num31].x = num23;
						_graftMovements2[num31].y = num24;
						_graftMovements2[num31].z = num25;
					}
					_morphedUVVertices[num32].x = array10[num31].x + _graftMovements[num31].x;
					_morphedUVVertices[num32].y = array10[num31].y + _graftMovements[num31].y;
					_morphedUVVertices[num32].z = array10[num31].z + _graftMovements[num31].z;
					ref Vector3 reference14 = ref _morphedBaseVertices[num32];
					reference14 = _morphedUVVertices[num32];
					_visibleMorphedUVVertices[num32].x = array10[num31].x + _graftMovements2[num31].x;
					_visibleMorphedUVVertices[num32].y = array10[num31].y + _graftMovements2[num31].y;
					_visibleMorphedUVVertices[num32].z = array10[num31].z + _graftMovements2[num31].z;
				}
				break;
			}
			case GraftMethod.ClosestVertAndPoly:
			{
				for (int num44 = 0; num44 < numGraftBaseVertices; num44++)
				{
					if (!_graftIsFreeVert[num44])
					{
						continue;
					}
					int num45 = startGraftVertIndex + num44;
					FreeVertGraftWeight freeVertGraftWeight2 = _freeVertGraftWeights[num44];
					int graftPoly2 = freeVertGraftWeight2.graftPoly;
					int graftVert = freeVertGraftWeight2.graftVert;
					float weight2 = freeVertGraftWeight2.weight;
					if (weight2 > 0f)
					{
						float num46 = 1f - weight2;
						float num47 = 1f - freeVertGraftWeight2.graftVertToPolyRatio;
						float num48 = (array[graftVert].x - array12[graftVert].x) * num47;
						float num49 = (array[graftVert].y - array12[graftVert].y) * num47;
						float num50 = (array[graftVert].z - array12[graftVert].z) * num47;
						float num51 = (array2[graftVert].x - array12[graftVert].x) * num47;
						float num52 = (array2[graftVert].y - array12[graftVert].y) * num47;
						float num53 = (array2[graftVert].z - array12[graftVert].z) * num47;
						if (freeVertGraftWeight2.graftVertToPolyRatio > 0f && graftPoly2 != -1)
						{
							int[] vertices2 = array13[graftPoly2].vertices;
							int num54 = vertices2.Length;
							float graftVertToPolyRatio = freeVertGraftWeight2.graftVertToPolyRatio;
							float num55 = 1f / (float)num54 * graftVertToPolyRatio;
							for (int num56 = 0; num56 < num54; num56++)
							{
								int num57 = vertices2[num56];
								num48 += (array[num57].x - array12[num57].x) * num55;
								num49 += (array[num57].y - array12[num57].y) * num55;
								num50 += (array[num57].z - array12[num57].z) * num55;
								num51 += (array2[num57].x - array12[num57].x) * num55;
								num52 += (array2[num57].y - array12[num57].y) * num55;
								num53 += (array2[num57].z - array12[num57].z) * num55;
							}
						}
						_graftMovements[num44].x = num20 * num46 + num48 * weight2;
						_graftMovements[num44].y = num21 * num46 + num49 * weight2;
						_graftMovements[num44].z = num22 * num46 + num50 * weight2;
						_graftMovements2[num44].x = num23 * num46 + num51 * weight2;
						_graftMovements2[num44].y = num24 * num46 + num52 * weight2;
						_graftMovements2[num44].z = num25 * num46 + num53 * weight2;
					}
					else
					{
						_graftMovements[num44].x = num20;
						_graftMovements[num44].y = num21;
						_graftMovements[num44].z = num22;
						_graftMovements2[num44].x = num23;
						_graftMovements2[num44].y = num24;
						_graftMovements2[num44].z = num25;
					}
					_morphedUVVertices[num45].x = array10[num44].x + _graftMovements[num44].x;
					_morphedUVVertices[num45].y = array10[num44].y + _graftMovements[num44].y;
					_morphedUVVertices[num45].z = array10[num44].z + _graftMovements[num44].z;
					ref Vector3 reference15 = ref _morphedBaseVertices[num45];
					reference15 = _morphedUVVertices[num45];
					_visibleMorphedUVVertices[num45].x = array10[num44].x + _graftMovements2[num44].x;
					_visibleMorphedUVVertices[num45].y = array10[num44].y + _graftMovements2[num44].y;
					_visibleMorphedUVVertices[num45].z = array10[num44].z + _graftMovements2[num44].z;
				}
				break;
			}
			case GraftMethod.Boundary:
			{
				Vector3 vector = default(Vector3);
				Vector3 vector2 = default(Vector3);
				for (int num26 = 0; num26 < numGraftBaseVertices; num26++)
				{
					if (!_graftIsFreeVert[num26])
					{
						continue;
					}
					int num27 = startGraftVertIndex + num26;
					if (flag2 || force)
					{
						vector.x = 0f;
						vector.y = 0f;
						vector.z = 0f;
						vector2.x = 0f;
						vector2.y = 0f;
						vector2.z = 0f;
						for (int num28 = 0; num28 < num5; num28++)
						{
							int vertexNum2 = vertexPairs[num28].vertexNum;
							int num29 = num28 * numGraftBaseVertices + num26;
							float num30 = _graftWeights[num29];
							vector.x += _graftMovements[vertexNum2].x * num30 * _graftXFactor;
							vector.y += _graftMovements[vertexNum2].y * num30 * _graftYFactor;
							vector.z += _graftMovements[vertexNum2].z * num30 * _graftZFactor;
							vector2.x += _graftMovements2[vertexNum2].x * num30 * _graftXFactor;
							vector2.y += _graftMovements2[vertexNum2].y * num30 * _graftYFactor;
							vector2.z += _graftMovements2[vertexNum2].z * num30 * _graftZFactor;
						}
						_graftMovements[num26] = vector;
						_graftMovements2[num26] = vector2;
					}
					_morphedUVVertices[num27].x = array10[num26].x + _graftMovements[num26].x;
					_morphedUVVertices[num27].y = array10[num26].y + _graftMovements[num26].y;
					_morphedUVVertices[num27].z = array10[num26].z + _graftMovements[num26].z;
					ref Vector3 reference13 = ref _morphedBaseVertices[num27];
					reference13 = _morphedUVVertices[num27];
					_visibleMorphedUVVertices[num27].x = array10[num26].x + _graftMovements2[num26].x;
					_visibleMorphedUVVertices[num27].y = array10[num26].y + _graftMovements2[num26].y;
					_visibleMorphedUVVertices[num27].z = array10[num26].z + _graftMovements2[num26].z;
				}
				break;
			}
			}
			if ((flag2 || force) && recalcNormalsOnMorph)
			{
				for (int num67 = 0; num67 < numGraftBaseVertices; num67++)
				{
					morphedBaseDirtyVertices[startGraftVertIndex + num67] = true;
				}
				for (int num68 = 0; num68 < num5; num68++)
				{
					int graftToVertexNum2 = vertexPairs[num68].graftToVertexNum;
					morphedBaseDirtyVertices[graftToVertexNum2] = true;
				}
				morphedNormalsDirty = true;
			}
			if ((flag2 || force) && recalcTangentsOnMorph)
			{
				for (int num69 = 0; num69 < numGraftBaseVertices; num69++)
				{
					morphedUVDirtyVertices[startGraftVertIndex + num69] = true;
				}
				for (int num70 = 0; num70 < num5; num70++)
				{
					int graftToVertexNum3 = vertexPairs[num70].graftToVertexNum;
					morphedUVDirtyVertices[graftToVertexNum3] = true;
				}
				morphedTangentsDirty = true;
			}
		}
		if (hasGraft2)
		{
			Vector3[] array14 = graft2Mesh.morphedUVVertices;
			Vector3[] array15 = graft2Mesh.visibleMorphedUVVertices;
			DAZMeshGraftVertexPair[] vertexPairs2 = graft2Mesh.meshGraft.vertexPairs;
			int num71 = vertexPairs2.Length;
			bool flag3 = false;
			if (targetMesh.verticesChangedThisFrame || force)
			{
				for (int num72 = 0; num72 < num71; num72++)
				{
					int graftToVertexNum4 = vertexPairs2[num72].graftToVertexNum;
					int vertexNum3 = vertexPairs2[num72].vertexNum;
					int num73 = startGraft2VertIndex + vertexNum3;
					float num74 = array[graftToVertexNum4].x - array12[graftToVertexNum4].x;
					float num75 = array[graftToVertexNum4].y - array12[graftToVertexNum4].y;
					float num76 = array[graftToVertexNum4].z - array12[graftToVertexNum4].z;
					float num77 = array2[graftToVertexNum4].x - array12[graftToVertexNum4].x;
					float num78 = array2[graftToVertexNum4].y - array12[graftToVertexNum4].y;
					float num79 = array2[graftToVertexNum4].z - array12[graftToVertexNum4].z;
					if (_graft2Movements[vertexNum3].x != num74)
					{
						flag3 = true;
						_graft2Movements[vertexNum3].x = num74;
					}
					if (_graft2Movements[vertexNum3].y != num75)
					{
						flag3 = true;
						_graft2Movements[vertexNum3].y = num75;
					}
					if (_graft2Movements[vertexNum3].z != num76)
					{
						flag3 = true;
						_graft2Movements[vertexNum3].z = num76;
					}
					if (_graft2Movements2[vertexNum3].x != num77)
					{
						_graft2Movements2[vertexNum3].x = num77;
					}
					if (_graft2Movements2[vertexNum3].y != num78)
					{
						_graft2Movements2[vertexNum3].y = num78;
					}
					if (_graft2Movements2[vertexNum3].z != num79)
					{
						_graft2Movements2[vertexNum3].z = num79;
					}
					ref Vector3 reference17 = ref _morphedUVVertices[num73];
					reference17 = array[graftToVertexNum4];
					ref Vector3 reference18 = ref _morphedBaseVertices[num73];
					reference18 = array[graftToVertexNum4];
					ref Vector3 reference19 = ref _visibleMorphedUVVertices[num73];
					reference19 = array2[graftToVertexNum4];
				}
			}
			if (targetMesh.verticesChangedThisFrame || graft2Mesh.verticesChangedThisFrame || force)
			{
				_verticesChangedThisFrame = true;
				_visibleVerticesChangedThisFrame = _visibleVerticesChangedThisFrame || graft2Mesh.visibleVerticesChangedThisFrame;
				Vector3 vector3 = default(Vector3);
				Vector3 vector4 = default(Vector3);
				for (int num80 = 0; num80 < numGraft2BaseVertices; num80++)
				{
					if (!_graft2IsFreeVert[num80])
					{
						continue;
					}
					int num81 = startGraft2VertIndex + num80;
					if (flag3 || force)
					{
						vector3.x = 0f;
						vector3.y = 0f;
						vector3.z = 0f;
						vector4.x = 0f;
						vector4.y = 0f;
						vector4.z = 0f;
						for (int num82 = 0; num82 < num71; num82++)
						{
							int vertexNum4 = vertexPairs2[num82].vertexNum;
							int num83 = num82 * numGraft2BaseVertices + num80;
							float num84 = _graft2Weights[num83];
							vector3.x += _graft2Movements[vertexNum4].x * num84 * _graftXFactor;
							vector3.y += _graft2Movements[vertexNum4].y * num84 * _graftYFactor;
							vector3.z += _graft2Movements[vertexNum4].z * num84 * _graftZFactor;
							vector4.x += _graft2Movements2[vertexNum4].x * num84 * _graftXFactor;
							vector4.y += _graft2Movements2[vertexNum4].y * num84 * _graftYFactor;
							vector4.z += _graft2Movements2[vertexNum4].z * num84 * _graftZFactor;
						}
						_graft2Movements[num80] = vector3;
						_graft2Movements2[num80] = vector4;
					}
					_morphedUVVertices[num81].x = array14[num80].x + _graft2Movements[num80].x;
					_morphedUVVertices[num81].y = array14[num80].y + _graft2Movements[num80].y;
					_morphedUVVertices[num81].z = array14[num80].z + _graft2Movements[num80].z;
					ref Vector3 reference20 = ref _morphedBaseVertices[num81];
					reference20 = _morphedUVVertices[num81];
					_visibleMorphedUVVertices[num81].x = array15[num80].x + _graft2Movements2[num80].x;
					_visibleMorphedUVVertices[num81].y = array15[num80].y + _graft2Movements2[num80].y;
					_visibleMorphedUVVertices[num81].z = array15[num80].z + _graft2Movements2[num80].z;
				}
			}
			if ((flag3 || force) && recalcNormalsOnMorph)
			{
				for (int num85 = 0; num85 < numGraft2BaseVertices; num85++)
				{
					morphedBaseDirtyVertices[startGraft2VertIndex + num85] = true;
				}
				for (int num86 = 0; num86 < num71; num86++)
				{
					int graftToVertexNum5 = vertexPairs2[num86].graftToVertexNum;
					morphedBaseDirtyVertices[graftToVertexNum5] = true;
				}
				morphedNormalsDirty = true;
			}
			if ((flag3 || force) && recalcTangentsOnMorph)
			{
				for (int num87 = 0; num87 < numGraft2BaseVertices; num87++)
				{
					morphedUVDirtyVertices[startGraft2VertIndex + num87] = true;
				}
				for (int num88 = 0; num88 < num71; num88++)
				{
					int graftToVertexNum6 = vertexPairs2[num88].graftToVertexNum;
					morphedUVDirtyVertices[graftToVertexNum6] = true;
				}
				morphedTangentsDirty = true;
			}
		}
		_triggerNormalAndTangentRecalc();
	}

	public void UpdateVerticesPost()
	{
		if (_useSmoothing)
		{
			InitMeshSmooth();
			meshSmooth.LaplacianSmooth(_morphedUVVertices, _smoothedMorphedUVVertices);
			meshSmooth.HCCorrection(_morphedUVVertices, _smoothedMorphedUVVertices, 0.5f);
			_updateDuplicateSmoothedMorphedUVVertices();
			if (_drawMorphedUVMappedMesh || !Application.isPlaying)
			{
				_morphedUVMappedMesh.vertices = _smoothedMorphedUVVertices;
				_morphedUVMappedMesh.normals = _morphedUVNormals;
				_morphedUVMappedMesh.tangents = _morphedUVTangents;
			}
		}
		else
		{
			_updateDuplicateMorphedUVVertices();
			if (_drawMorphedUVMappedMesh || !Application.isPlaying)
			{
				_morphedUVMappedMesh.vertices = _morphedUVVertices;
				_morphedUVMappedMesh.normals = _morphedUVNormals;
				_morphedUVMappedMesh.tangents = _morphedUVTangents;
			}
		}
		if (base.drawMorphedBaseMesh)
		{
			_morphedBaseMesh.vertices = _morphedBaseVertices;
			_morphedBaseMesh.normals = _morphedBaseNormals;
		}
	}

	public new void Draw()
	{
		base.Draw();
		if (!drawGraftMorphedMesh || !(graftMesh != null) || !(_graftMorphedMesh != null))
		{
			return;
		}
		Vector3[] array = graftMesh.baseVertices;
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector3 reference = ref _graftMorphedMeshVertices[i];
			reference = array[i];
		}
		if (graftMeshMorphNamesForGrafting != null && graftMesh.morphBank != null)
		{
			if (graftMeshMorphNamesForGrafting.Length == graftMeshMorphValuesForGrafting.Length)
			{
				for (int j = 0; j < graftMeshMorphNamesForGrafting.Length; j++)
				{
					float num = graftMeshMorphValuesForGrafting[j];
					DAZMorph morph = graftMesh.morphBank.GetMorph(graftMeshMorphNamesForGrafting[j]);
					if (morph != null && morph.deltas.Length > 0)
					{
						DAZMorphVertex[] deltas = morph.deltas;
						foreach (DAZMorphVertex dAZMorphVertex in deltas)
						{
							Vector3 vector = dAZMorphVertex.delta * num;
							_graftMorphedMeshVertices[dAZMorphVertex.vertex] += vector;
						}
					}
					else
					{
						Debug.LogError("Could not find graft morph " + graftMeshMorphNamesForGrafting[j]);
					}
				}
				_graftMorphedMesh.vertices = _graftMorphedMeshVertices;
			}
			else
			{
				Debug.LogError("Graft mesh morph names and morph values are not same length");
			}
		}
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		if (Application.isPlaying && drawFromBone != null)
		{
			localToWorldMatrix *= drawFromBone.changeFromOriginalMatrix;
		}
		if (simpleMaterial != null)
		{
			for (int l = 0; l < _graftMorphedMesh.subMeshCount; l++)
			{
				Graphics.DrawMesh(_graftMorphedMesh, localToWorldMatrix, simpleMaterial, 0, null, l, null, castShadows: false, receiveShadows: false);
			}
		}
		else
		{
			Debug.LogWarning("Draw Graft Morphed Mesh is enabled but simple material is not set");
		}
	}

	public void ManualUpdate()
	{
		Update();
	}

	private void Update()
	{
		if (!staticMesh)
		{
			UpdateVertices();
		}
		Draw();
	}

	public override void Init()
	{
		if (!_wasInit)
		{
			if (targetMesh != null)
			{
				targetMesh.Init();
			}
			if (graftMesh != null)
			{
				graftMesh.Init();
			}
			if (graft2Mesh != null)
			{
				graft2Mesh.Init();
			}
			base.Init();
		}
	}
}
