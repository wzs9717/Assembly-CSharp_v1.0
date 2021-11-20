using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using SimpleJSON;
using UnityEngine;

[ExecuteInEditMode]
public class DAZSkinV2 : MonoBehaviour
{
	public enum PhysicsType
	{
		None,
		Static,
		Rigidbody,
		KinematicRigidbody
	}

	public enum SkinMethod
	{
		CPU,
		GPU,
		CPUAndGPU
	}

	protected struct Bone
	{
		public Vector3 rotationAngles;

		public Matrix4x4 worldToLocal;

		public Matrix4x4 localToWorld;

		public Matrix4x4 changeFromOriginal;

		public float xposleftbulge;

		public float xnegleftbulge;

		public float xposrightbulge;

		public float xnegrightbulge;

		public float yposleftbulge;

		public float ynegleftbulge;

		public float yposrightbulge;

		public float ynegrightbulge;

		public float zposleftbulge;

		public float znegleftbulge;

		public float zposrightbulge;

		public float znegrightbulge;
	}

	protected struct BoneGeneralWeights
	{
		public int vertex;

		public float weight;
	}

	protected struct BoneWeights
	{
		public int vertex;

		public float xweight;

		public float yweight;

		public float zweight;

		public float xleftbulge;

		public float xrightbulge;

		public float yleftbulge;

		public float yrightbulge;

		public float zleftbulge;

		public float zrightbulge;
	}

	protected struct BoneFullWeights
	{
		public int vertex;
	}

	protected struct BaseVertToUVVert
	{
		public int baseVertex;

		public int UVVertex;
	}

	public bool skin;

	public bool draw = true;

	[SerializeField]
	protected PhysicsType _physicsType;

	public float mass = 1000f;

	protected MeshCollider meshCollider;

	protected Rigidbody RB;

	public SkinMethod skinMethod;

	public bool delayDisplayOneFrame = true;

	[SerializeField]
	protected bool _hasGeneralWeights;

	[SerializeField]
	protected bool _useGeneralWeights;

	public string skinUrl;

	public string skinId;

	public string sceneGeometryId;

	public DAZBones root;

	public Vector3 drawOffset;

	[SerializeField]
	private bool _useSmoothing;

	[SerializeField]
	private bool _recalculateTangents;

	[SerializeField]
	private bool _recalculateNormals = true;

	[SerializeField]
	protected int _numBones;

	public DAZSkinV2Node[] nodes;

	protected List<DAZSkinV2Node> importNodes;

	protected Dictionary<string, int> boneNameToIndexMap;

	protected Dictionary<string, Dictionary<int, DAZSkinV2VertexWeights>> boneWeightsMap;

	protected Dictionary<string, Dictionary<int, DAZSkinV2GeneralVertexWeights>> boneGeneralWeightsMap;

	protected Dictionary<int, bool> vertexDoneAccumulating;

	protected bool accumlationStarted;

	protected DAZBone[] dazBones;

	protected Vector3[] boneRotationAngles;

	protected Matrix4x4[] boneChangeFromOriginalMatrices;

	protected Matrix4x4[] boneWorldToLocalMatrices;

	protected Matrix4x4[] boneLocalToWorldMatrices;

	[NonSerialized]
	public DAZBone[] strongestDAZBone;

	protected float[] strongestBoneWeight;

	protected Mesh mesh;

	public DAZMesh dazMesh;

	public bool showNodes;

	public bool showMaterials = true;

	public bool GPUuseSimpleMaterial;

	public Material GPUsimpleMaterial;

	public Material[] GPUmaterials;

	public bool GPUAutoSwapShader = true;

	public int GPUAutoSwapCopyNum;

	public bool[] materialsEnabled;

	public bool[] materialsShadowCastEnabled;

	[SerializeField]
	protected int _numMaterials;

	[SerializeField]
	protected string[] _materialNames;

	protected Vector3[] startVerts;

	protected Vector3[] startVertsCopy;

	protected Vector3[] startNormals;

	protected Vector4[] startTangents;

	[NonSerialized]
	public Vector3[] drawVerts;

	public bool allowPostSkinMorph;

	protected Vector3[] workingVerts;

	protected Vector3[] workingVerts2;

	[NonSerialized]
	public Vector3[] rawSkinnedWorkingVerts;

	[NonSerialized]
	public Vector3[] rawSkinnedVerts;

	protected Vector3[] smoothedVerts;

	protected Vector3[] unsmoothedVerts;

	protected bool[] isBaseVert;

	protected int[] baseVertIndices;

	protected int numBaseVerts;

	protected int numUVVerts;

	protected int numUVOnlyVerts;

	protected DAZVertexMap[] baseVertsToUVVertsCopy;

	protected Vector3[] workingNormals;

	protected Vector3[] workingSurfaceNormals;

	[NonSerialized]
	public Vector3[] drawNormals;

	[NonSerialized]
	public Vector3[] drawSurfaceNormals;

	protected Vector4[] workingTangents;

	[NonSerialized]
	public Vector4[] drawTangents;

	protected MeshSmooth meshSmooth;

	protected MeshSmoothGPU meshSmoothGPU;

	protected RecalculateNormals recalcNormals;

	protected RecalculateNormals postSkinRecalcNormals;

	protected RecalculateNormalsGPU recalcNormalsGPU;

	protected RecalculateTangents recalcTangents;

	protected RecalculateTangentsGPU recalcTangentsGPU;

	public bool useThreadControlNumThreads;

	[SerializeField]
	protected int _numSubThreads;

	public bool useMultithreadedSkinning = true;

	public bool useMultithreadedSmoothing;

	public bool useAsynchronousThreadedSkinning;

	public bool useAsynchronousNormalTangentRecalc = true;

	public bool useSmoothVertsForNormalTangentRecalc;

	protected float f;

	public float debugStartTime;

	public float debugTime;

	public float debugStopTime;

	public float updateStartTime;

	public float updateStopTime;

	public float updateTime;

	public float lastFrameSkinStartTime;

	public float skinStartTime;

	public float skinStopTime;

	public float skinTime;

	public float mainThreadSkinStartTime;

	public float mainThreadSkinStopTime;

	public float mainThreadSkinTime;

	public float[] threadSkinStartTime;

	public float[] threadSkinStopTime;

	public float[] threadSkinTime;

	public int[] threadSkinVertsCount;

	public float mainThreadSmoothStartTime;

	public float mainThreadSmoothStopTime;

	public float mainThreadSmoothTime;

	public float[] threadSmoothStartTime;

	public float[] threadSmoothStopTime;

	public float[] threadSmoothTime;

	public float mainThreadSmoothCorrectionStartTime;

	public float mainThreadSmoothCorrectionStopTime;

	public float mainThreadSmoothCorrectionTime;

	public float[] threadSmoothCorrectionStartTime;

	public float[] threadSmoothCorrectionStopTime;

	public float[] threadSmoothCorrectionTime;

	public float threadRecalcNormalTangentStartTime;

	public float threadRecalcNormalTangentTime;

	public float threadRecalcNormalTangentStopTime;

	public float threadMainSkinStartTime;

	public float threadMainSkinTime;

	public float threadMainSkinStopTime;

	public float bulgeScale = 0.0015f;

	public int smoothOuterLoops = 1;

	public int laplacianSmoothPasses = 2;

	public int springSmoothPasses;

	public float laplacianSmoothBeta = 0.5f;

	public float springSmoothFactor = 0.2f;

	protected DAZSkinTaskInfo[] tasks;

	protected DAZSkinTaskInfo normalTangentTask;

	protected DAZSkinTaskInfo mainSkinTask;

	protected DAZSkinTaskInfo postSkinMorphTask;

	protected bool _threadsRunning;

	protected const int skinGroupSize = 256;

	protected const int vertGroupSize = 256;

	protected const int baseVertToUVVertGroupSize = 256;

	protected int[] numGeneralSkinThreadGroups;

	protected int[] numSkinThreadGroups;

	protected int[] numSkinFinishThreadGroups;

	protected int numVertThreadGroups;

	public ComputeShader GPUSkinner;

	public ComputeShader GPUMeshCompute;

	protected ComputeBuffer[] boneBuffer;

	protected ComputeBuffer[] generalWeightsBuffer;

	protected ComputeBuffer[] weightsBuffer;

	protected ComputeBuffer[] fullWeightsBuffer;

	protected ComputeBuffer startVertsBuffer;

	public ComputeBuffer rawVertsBuffer;

	protected ComputeBuffer postSkinMorphsBuffer;

	public ComputeBuffer _verticesBuffer1;

	public ComputeBuffer _verticesBuffer2;

	public ComputeBuffer smoothedVertsBuffer;

	public ComputeBuffer delayedVertsBuffer;

	public ComputeBuffer delayedNormalsBuffer;

	public ComputeBuffer delayedTangentsBuffer;

	protected ComputeBuffer _normalsBuffer;

	protected ComputeBuffer _tangentsBuffer;

	protected ComputeBuffer _surfaceNormalsBuffer;

	protected int _zeroKernel;

	protected int _skinGeneralKernel;

	protected int _initKernel;

	protected int _copyKernel;

	protected int _copyTangentsKernel;

	protected int _skinXYZKernel;

	protected int _skinXZYKernel;

	protected int _skinYXZKernel;

	protected int _skinYZXKernel;

	protected int _skinZXYKernel;

	protected int _skinZYXKernel;

	protected int _skinFinishKernel;

	protected int _postSkinMorphKernel;

	protected int _nullVertexIndex;

	protected MapVerticesGPU mapVerticesGPU;

	public float bloat;

	[NonSerialized]
	public bool[] postSkinVerts;

	[NonSerialized]
	public bool[] postSkinNormalVerts;

	protected bool needsPostSkinNormals;

	protected bool[] postSkinNeededVerts;

	protected int[] postSkinNeededTriangles;

	protected int[] postSkinNeededVertsList;

	protected bool[] postSkinBones;

	[NonSerialized]
	public Vector3[] postSkinMorphs;

	protected Vector3[] postSkinWorkingNormals;

	[NonSerialized]
	public Vector3[] postSkinNormals;

	public bool postSkinVertsChanged = true;

	public bool postSkinVertsChangedThreaded;

	public bool useEarlyFinish;

	protected int totalFrameCount;

	protected int missedFrameCount;

	public int debugVertex = -1;

	protected Stopwatch stopwatch;

	protected bool _wasInit;

	public PhysicsType physicsType
	{
		get
		{
			return _physicsType;
		}
		set
		{
			if (value != _physicsType)
			{
				_physicsType = value;
				InitPhysicsObjects();
			}
		}
	}

	public bool hasGeneralWeights => _hasGeneralWeights;

	public bool useGeneralWeights
	{
		get
		{
			return _useGeneralWeights;
		}
		set
		{
			_useGeneralWeights = value;
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
			_useSmoothing = value;
		}
	}

	public bool recalculateTangents
	{
		get
		{
			return _recalculateTangents;
		}
		set
		{
			_recalculateTangents = value;
		}
	}

	public bool recalculateNormals
	{
		get
		{
			return _recalculateNormals;
		}
		set
		{
			_recalculateNormals = value;
		}
	}

	public int numBones => _numBones;

	public int numMaterials => _numMaterials;

	public string[] materialNames => _materialNames;

	public int numSubThreads
	{
		get
		{
			return _numSubThreads;
		}
		set
		{
			if (_numSubThreads != value)
			{
				bool flag = _threadsRunning;
				if (flag)
				{
					StopThreads();
				}
				_numSubThreads = value;
				InitSkinTimes();
				if (flag)
				{
					StartThreads();
				}
			}
		}
	}

	public bool threadsRunning => _threadsRunning;

	public ComputeBuffer normalsBuffer => _normalsBuffer;

	public ComputeBuffer tangentsBuffer => _tangentsBuffer;

	public ComputeBuffer surfaceNormalsBuffer => _surfaceNormalsBuffer;

	public bool wasInit => _wasInit;

	public void ImportStart()
	{
		importNodes = new List<DAZSkinV2Node>();
	}

	public void ImportNode(JSONNode jn, string url)
	{
		DAZSkinV2Node dAZSkinV2Node = new DAZSkinV2Node();
		dAZSkinV2Node.url = url;
		dAZSkinV2Node.id = jn["id"];
		dAZSkinV2Node.name = jn["name"];
		string text = jn["rotation_order"];
		Quaternion2Angles.RotationOrder rotationOrder;
		switch (text)
		{
		case "XYZ":
			rotationOrder = Quaternion2Angles.RotationOrder.ZYX;
			break;
		case "XZY":
			rotationOrder = Quaternion2Angles.RotationOrder.YZX;
			break;
		case "YXZ":
			rotationOrder = Quaternion2Angles.RotationOrder.ZXY;
			break;
		case "YZX":
			rotationOrder = Quaternion2Angles.RotationOrder.XZY;
			break;
		case "ZXY":
			rotationOrder = Quaternion2Angles.RotationOrder.YXZ;
			break;
		case "ZYX":
			rotationOrder = Quaternion2Angles.RotationOrder.XYZ;
			break;
		default:
			UnityEngine.Debug.LogError("Bad rotation order in json: " + text);
			rotationOrder = Quaternion2Angles.RotationOrder.XYZ;
			break;
		}
		dAZSkinV2Node.rotationOrder = rotationOrder;
		bool flag = false;
		if (importNodes == null)
		{
			importNodes = new List<DAZSkinV2Node>();
		}
		for (int i = 0; i < importNodes.Count; i++)
		{
			if (importNodes[i].url == dAZSkinV2Node.url)
			{
				importNodes[i] = dAZSkinV2Node;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			importNodes.Add(dAZSkinV2Node);
		}
	}

	protected DAZSkinV2Node FindNodeByUrl(string url)
	{
		string text = url;
		if (Regex.IsMatch(url, "^#"))
		{
			text = DAZImport.DAZurlToPathKey(skinUrl) + url;
		}
		for (int i = 0; i < importNodes.Count; i++)
		{
			if (importNodes[i].url == text)
			{
				return importNodes[i];
			}
		}
		UnityEngine.Debug.LogWarning("Could not find node by url " + text);
		return null;
	}

	protected DAZSkinV2Node FindNodeById(string id)
	{
		for (int i = 0; i < importNodes.Count; i++)
		{
			if (importNodes[i].id == id)
			{
				return importNodes[i];
			}
		}
		UnityEngine.Debug.LogWarning("Could not find node by id " + id);
		return null;
	}

	protected DAZSkinV2Node FindNodeByName(string name)
	{
		for (int i = 0; i < importNodes.Count; i++)
		{
			if (importNodes[i].name == name)
			{
				return importNodes[i];
			}
		}
		UnityEngine.Debug.LogWarning("Could not find node by name " + name);
		return null;
	}

	protected Dictionary<int, DAZSkinV2VertexWeights> WalkBonesAndAccumulateWeights(Transform bone)
	{
		accumlationStarted = false;
		vertexDoneAccumulating = new Dictionary<int, bool>();
		return WalkBonesAndAccumulateWeightsRecursive(bone);
	}

	protected Dictionary<int, DAZSkinV2VertexWeights> WalkBonesAndAccumulateWeightsRecursive(Transform bone)
	{
		if (boneWeightsMap.TryGetValue(bone.name, out var value))
		{
			accumlationStarted = true;
			{
				foreach (Transform item in bone)
				{
					Dictionary<int, DAZSkinV2VertexWeights> dictionary = WalkBonesAndAccumulateWeightsRecursive(item);
					if (dictionary == null)
					{
						continue;
					}
					foreach (int key in dictionary.Keys)
					{
						if (vertexDoneAccumulating.ContainsKey(key) || !dictionary.TryGetValue(key, out var value2))
						{
							continue;
						}
						if (value.TryGetValue(key, out var value3))
						{
							value3.xweight += value2.xweight;
							value3.yweight += value2.yweight;
							value3.zweight += value2.zweight;
							if (value3.xweight > 0.99999f && value3.yweight > 0.99999f && value3.zweight > 0.99999f)
							{
								value3.xweight = 1f;
								value3.yweight = 1f;
								value3.zweight = 1f;
								vertexDoneAccumulating.Add(key, value: true);
							}
							value.Remove(key);
							value.Add(key, value3);
						}
						else if (value2.xweight > 0.99999f && value2.yweight > 0.99999f && value2.zweight > 0.99999f)
						{
							value2.xweight = 1f;
							value2.yweight = 1f;
							value2.zweight = 1f;
							vertexDoneAccumulating.Add(key, value: true);
						}
						else
						{
							DAZSkinV2VertexWeights dAZSkinV2VertexWeights = new DAZSkinV2VertexWeights();
							dAZSkinV2VertexWeights.vertex = value2.vertex;
							dAZSkinV2VertexWeights.xweight = value2.xweight;
							dAZSkinV2VertexWeights.yweight = value2.yweight;
							dAZSkinV2VertexWeights.zweight = value2.zweight;
							value.Add(key, dAZSkinV2VertexWeights);
						}
					}
				}
				return value;
			}
		}
		if (!accumlationStarted)
		{
			foreach (Transform item2 in bone)
			{
				Dictionary<int, DAZSkinV2VertexWeights> dictionary2 = WalkBonesAndAccumulateWeightsRecursive(item2);
				if (dictionary2 != null)
				{
					return dictionary2;
				}
			}
			return null;
		}
		return null;
	}

	protected void CreateBoneWeightsArray()
	{
		foreach (string key in boneWeightsMap.Keys)
		{
			if (!boneWeightsMap.TryGetValue(key, out var value) || !boneNameToIndexMap.TryGetValue(key, out var value2))
			{
				continue;
			}
			DAZSkinV2Node dAZSkinV2Node = nodes[value2];
			List<DAZSkinV2VertexWeights> list = new List<DAZSkinV2VertexWeights>();
			List<int> list2 = new List<int>();
			foreach (int key2 in value.Keys)
			{
				if (value.TryGetValue(key2, out var value3))
				{
					if (value3.xweight == 1f && value3.yweight == 1f && value3.zweight == 1f)
					{
						list2.Add(value3.vertex);
					}
					else
					{
						list.Add(value3);
					}
				}
			}
			dAZSkinV2Node.weights = list.ToArray();
			dAZSkinV2Node.fullyWeightedVertices = list2.ToArray();
		}
		foreach (string key3 in boneGeneralWeightsMap.Keys)
		{
			if (!boneGeneralWeightsMap.TryGetValue(key3, out var value4))
			{
				continue;
			}
			int num = 0;
			if (!boneNameToIndexMap.TryGetValue(key3, out var value5))
			{
				continue;
			}
			DAZSkinV2Node dAZSkinV2Node2 = nodes[value5];
			dAZSkinV2Node2.generalWeights = new DAZSkinV2GeneralVertexWeights[value4.Count];
			foreach (int key4 in value4.Keys)
			{
				if (value4.TryGetValue(key4, out var value6))
				{
					dAZSkinV2Node2.generalWeights[num] = value6;
					num++;
				}
			}
		}
	}

	public void CheckGeneralWeights()
	{
		int numBaseVertices = dazMesh.numBaseVertices;
		UnityEngine.Debug.Log("Num base verts:" + numBaseVertices + " Num UV verts:" + dazMesh.numUVVertices);
		float[] array = new float[numBaseVertices];
		for (int i = 0; i < numBaseVertices; i++)
		{
			array[i] = 0f;
		}
		DAZSkinV2Node[] array2 = nodes;
		foreach (DAZSkinV2Node dAZSkinV2Node in array2)
		{
			DAZSkinV2GeneralVertexWeights[] generalWeights = dAZSkinV2Node.generalWeights;
			foreach (DAZSkinV2GeneralVertexWeights dAZSkinV2GeneralVertexWeights in generalWeights)
			{
				if (dAZSkinV2GeneralVertexWeights.vertex < numBaseVertices)
				{
					array[dAZSkinV2GeneralVertexWeights.vertex] += dAZSkinV2GeneralVertexWeights.weight;
				}
				else
				{
					UnityEngine.Debug.LogError("Vertex " + dAZSkinV2GeneralVertexWeights.vertex + " in generalWeights is out of range");
				}
			}
		}
		for (int l = 0; l < numBaseVertices; l++)
		{
			if (array[l] < 0.999f)
			{
				UnityEngine.Debug.LogError("Vertex " + l + " weights don't add up to 1");
			}
		}
	}

	public void Import(JSONNode jn)
	{
		if (root == null)
		{
			UnityEngine.Debug.LogError("Root bone not set. Can't import skin");
			return;
		}
		JSONNode jSONNode = jn["skin"]["joints"];
		_numBones = jSONNode.Count;
		DAZMesh[] components = GetComponents<DAZMesh>();
		dazMesh = null;
		DAZMesh[] array = components;
		foreach (DAZMesh dAZMesh in array)
		{
			if (dAZMesh.sceneGeometryId == sceneGeometryId)
			{
				dazMesh = dAZMesh;
				break;
			}
		}
		if (dazMesh == null)
		{
			UnityEngine.Debug.LogError("Could not find DAZMesh component with geometryID " + sceneGeometryId);
			return;
		}
		Dictionary<int, List<int>> baseVertToUVVertFullMap = dazMesh.baseVertToUVVertFullMap;
		InitPhysicsObjects();
		boneNameToIndexMap = new Dictionary<string, int>();
		boneWeightsMap = new Dictionary<string, Dictionary<int, DAZSkinV2VertexWeights>>();
		boneGeneralWeightsMap = new Dictionary<string, Dictionary<int, DAZSkinV2GeneralVertexWeights>>();
		nodes = new DAZSkinV2Node[_numBones];
		int num = 0;
		foreach (JSONNode item in jSONNode.AsArray)
		{
			DAZSkinV2Node dAZSkinV2Node = FindNodeByUrl(DAZImport.DAZurlFix(item["node"]));
			string key = dAZSkinV2Node.name;
			nodes[num] = dAZSkinV2Node;
			boneNameToIndexMap.Add(dAZSkinV2Node.name, num);
			Dictionary<int, DAZSkinV2VertexWeights> dictionary = new Dictionary<int, DAZSkinV2VertexWeights>();
			Dictionary<int, DAZSkinV2GeneralVertexWeights> dictionary2 = new Dictionary<int, DAZSkinV2GeneralVertexWeights>();
			foreach (JSONNode item2 in item["node_weights"]["values"].AsArray)
			{
				_hasGeneralWeights = true;
				int asInt = item2[0].AsInt;
				float asFloat = item2[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt, out var value))
				{
					continue;
				}
				foreach (int item3 in value)
				{
					if (dictionary2.TryGetValue(item3, out var value2))
					{
						value2.weight = asFloat;
						dictionary2.Remove(item3);
						dictionary2.Add(item3, value2);
					}
					else
					{
						value2 = new DAZSkinV2GeneralVertexWeights();
						value2.vertex = item3;
						value2.weight = asFloat;
						dictionary2.Add(item3, value2);
					}
				}
			}
			foreach (JSONNode item4 in item["local_weights"]["x"]["values"].AsArray)
			{
				int asInt2 = item4[0].AsInt;
				float asFloat2 = item4[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt2, out var value3))
				{
					continue;
				}
				foreach (int item5 in value3)
				{
					if (dictionary.TryGetValue(item5, out var value4))
					{
						value4.xweight = asFloat2;
						dictionary.Remove(item5);
						dictionary.Add(item5, value4);
					}
					else
					{
						value4 = new DAZSkinV2VertexWeights();
						value4.vertex = item5;
						value4.xweight = asFloat2;
						dictionary.Add(item5, value4);
					}
				}
			}
			foreach (JSONNode item6 in item["local_weights"]["y"]["values"].AsArray)
			{
				int asInt3 = item6[0].AsInt;
				float asFloat3 = item6[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt3, out var value5))
				{
					continue;
				}
				foreach (int item7 in value5)
				{
					if (dictionary.TryGetValue(item7, out var value6))
					{
						value6.yweight = asFloat3;
						dictionary.Remove(item7);
						dictionary.Add(item7, value6);
					}
					else
					{
						value6 = new DAZSkinV2VertexWeights();
						value6.vertex = item7;
						value6.yweight = asFloat3;
						dictionary.Add(item7, value6);
					}
				}
			}
			foreach (JSONNode item8 in item["local_weights"]["z"]["values"].AsArray)
			{
				int asInt4 = item8[0].AsInt;
				float asFloat4 = item8[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt4, out var value7))
				{
					continue;
				}
				foreach (int item9 in value7)
				{
					if (dictionary.TryGetValue(item9, out var value8))
					{
						value8.zweight = asFloat4;
						dictionary.Remove(item9);
						dictionary.Add(item9, value8);
					}
					else
					{
						value8 = new DAZSkinV2VertexWeights();
						value8.vertex = item9;
						value8.zweight = asFloat4;
						dictionary.Add(item9, value8);
					}
				}
			}
			DAZSkinV2BulgeFactors dAZSkinV2BulgeFactors = new DAZSkinV2BulgeFactors();
			dAZSkinV2BulgeFactors.name = key;
			foreach (JSONNode item10 in item["bulge_weights"]["x"]["bulges"].AsArray)
			{
				switch ((string)item10["id"])
				{
				case "positive-left":
					dAZSkinV2BulgeFactors.xposleft = item10["value"].AsFloat;
					break;
				case "positive-right":
					dAZSkinV2BulgeFactors.xposright = item10["value"].AsFloat;
					break;
				case "negative-left":
					dAZSkinV2BulgeFactors.xnegleft = 0f - item10["value"].AsFloat;
					break;
				case "negative-right":
					dAZSkinV2BulgeFactors.xnegright = 0f - item10["value"].AsFloat;
					break;
				}
			}
			foreach (JSONNode item11 in item["bulge_weights"]["x"]["left_map"]["values"].AsArray)
			{
				int asInt5 = item11[0].AsInt;
				float asFloat5 = item11[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt5, out var value9))
				{
					continue;
				}
				foreach (int item12 in value9)
				{
					if (dictionary.TryGetValue(item12, out var value10))
					{
						value10.xleftbulge = asFloat5;
						dictionary.Remove(item12);
						dictionary.Add(item12, value10);
					}
					else
					{
						value10 = new DAZSkinV2VertexWeights();
						value10.vertex = item12;
						value10.xleftbulge = asFloat5;
						dictionary.Add(item12, value10);
					}
				}
			}
			foreach (JSONNode item13 in item["bulge_weights"]["x"]["right_map"]["values"].AsArray)
			{
				int asInt6 = item13[0].AsInt;
				float asFloat6 = item13[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt6, out var value11))
				{
					continue;
				}
				foreach (int item14 in value11)
				{
					if (dictionary.TryGetValue(item14, out var value12))
					{
						value12.xrightbulge = asFloat6;
						dictionary.Remove(item14);
						dictionary.Add(item14, value12);
					}
					else
					{
						value12 = new DAZSkinV2VertexWeights();
						value12.vertex = item14;
						value12.xrightbulge = asFloat6;
						dictionary.Add(item14, value12);
					}
				}
			}
			foreach (JSONNode item15 in item["bulge_weights"]["y"]["bulges"].AsArray)
			{
				switch ((string)item15["id"])
				{
				case "positive-left":
					dAZSkinV2BulgeFactors.ynegleft = 0f - item15["value"].AsFloat;
					break;
				case "positive-right":
					dAZSkinV2BulgeFactors.ynegright = 0f - item15["value"].AsFloat;
					break;
				case "negative-left":
					dAZSkinV2BulgeFactors.yposleft = item15["value"].AsFloat;
					break;
				case "negative-right":
					dAZSkinV2BulgeFactors.yposright = item15["value"].AsFloat;
					break;
				}
			}
			foreach (JSONNode item16 in item["bulge_weights"]["y"]["left_map"]["values"].AsArray)
			{
				int asInt7 = item16[0].AsInt;
				float asFloat7 = item16[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt7, out var value13))
				{
					continue;
				}
				foreach (int item17 in value13)
				{
					if (dictionary.TryGetValue(item17, out var value14))
					{
						value14.yleftbulge = asFloat7;
						dictionary.Remove(item17);
						dictionary.Add(item17, value14);
					}
					else
					{
						value14 = new DAZSkinV2VertexWeights();
						value14.vertex = item17;
						value14.yleftbulge = asFloat7;
						dictionary.Add(item17, value14);
					}
				}
			}
			foreach (JSONNode item18 in item["bulge_weights"]["y"]["right_map"]["values"].AsArray)
			{
				int asInt8 = item18[0].AsInt;
				float asFloat8 = item18[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt8, out var value15))
				{
					continue;
				}
				foreach (int item19 in value15)
				{
					if (dictionary.TryGetValue(item19, out var value16))
					{
						value16.yrightbulge = asFloat8;
						dictionary.Remove(item19);
						dictionary.Add(item19, value16);
					}
					else
					{
						value16 = new DAZSkinV2VertexWeights();
						value16.vertex = item19;
						value16.yrightbulge = asFloat8;
						dictionary.Add(item19, value16);
					}
				}
			}
			foreach (JSONNode item20 in item["bulge_weights"]["z"]["bulges"].AsArray)
			{
				switch ((string)item20["id"])
				{
				case "positive-left":
					dAZSkinV2BulgeFactors.znegleft = 0f - item20["value"].AsFloat;
					break;
				case "positive-right":
					dAZSkinV2BulgeFactors.znegright = 0f - item20["value"].AsFloat;
					break;
				case "negative-left":
					dAZSkinV2BulgeFactors.zposleft = item20["value"].AsFloat;
					break;
				case "negative-right":
					dAZSkinV2BulgeFactors.zposright = item20["value"].AsFloat;
					break;
				}
			}
			foreach (JSONNode item21 in item["bulge_weights"]["z"]["left_map"]["values"].AsArray)
			{
				int asInt9 = item21[0].AsInt;
				float asFloat9 = item21[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt9, out var value17))
				{
					continue;
				}
				foreach (int item22 in value17)
				{
					if (dictionary.TryGetValue(item22, out var value18))
					{
						value18.zleftbulge = asFloat9;
						dictionary.Remove(item22);
						dictionary.Add(item22, value18);
					}
					else
					{
						value18 = new DAZSkinV2VertexWeights();
						value18.vertex = item22;
						value18.zleftbulge = asFloat9;
						dictionary.Add(item22, value18);
					}
				}
			}
			foreach (JSONNode item23 in item["bulge_weights"]["z"]["right_map"]["values"].AsArray)
			{
				int asInt10 = item23[0].AsInt;
				float asFloat10 = item23[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt10, out var value19))
				{
					continue;
				}
				foreach (int item24 in value19)
				{
					if (dictionary.TryGetValue(item24, out var value20))
					{
						value20.zrightbulge = asFloat10;
						dictionary.Remove(item24);
						dictionary.Add(item24, value20);
					}
					else
					{
						value20 = new DAZSkinV2VertexWeights();
						value20.vertex = item24;
						value20.zrightbulge = asFloat10;
						dictionary.Add(item24, value20);
					}
				}
			}
			nodes[num].bulgeFactors = dAZSkinV2BulgeFactors;
			boneWeightsMap.Add(key, dictionary);
			boneGeneralWeightsMap.Add(key, dictionary2);
			num++;
		}
		WalkBonesAndAccumulateWeights(root.transform);
		CreateBoneWeightsArray();
	}

	protected void CalculateStrongestWeights()
	{
		numUVVerts = dazMesh.numUVVertices;
		strongestDAZBone = new DAZBone[numUVVerts];
		strongestBoneWeight = new float[numUVVerts];
		for (int i = 0; i < numBones; i++)
		{
			DAZSkinV2VertexWeights[] weights = nodes[i].weights;
			foreach (DAZSkinV2VertexWeights dAZSkinV2VertexWeights in weights)
			{
				float num = (dAZSkinV2VertexWeights.xweight + dAZSkinV2VertexWeights.yweight + dAZSkinV2VertexWeights.zweight) * 0.33f;
				if (num > strongestBoneWeight[dAZSkinV2VertexWeights.vertex])
				{
					strongestBoneWeight[dAZSkinV2VertexWeights.vertex] = num;
					strongestDAZBone[dAZSkinV2VertexWeights.vertex] = dazBones[i];
				}
			}
		}
	}

	protected void InitBones()
	{
		dazBones = new DAZBone[numBones];
		boneRotationAngles = new Vector3[numBones];
		boneChangeFromOriginalMatrices = new Matrix4x4[numBones];
		boneWorldToLocalMatrices = new Matrix4x4[numBones];
		boneLocalToWorldMatrices = new Matrix4x4[numBones];
		if (root != null)
		{
			for (int i = 0; i < numBones; i++)
			{
				string text = nodes[i].name;
				DAZBone dAZBone = root.GetDAZBone(text);
				if (dAZBone != null)
				{
					dazBones[i] = dAZBone;
				}
				else
				{
					UnityEngine.Debug.LogError("Could not find DazBone for bone " + text + " for skin " + sceneGeometryId);
				}
			}
		}
		else
		{
			UnityEngine.Debug.LogError("Could not init bones since root DazBones is not set for skin " + sceneGeometryId);
		}
		CalculateStrongestWeights();
	}

	public void CopyMaterials()
	{
		if (dazMesh != null)
		{
			_numMaterials = dazMesh.materials.Length;
			GPUsimpleMaterial = dazMesh.simpleMaterial;
			GPUmaterials = new Material[_numMaterials];
			materialsEnabled = new bool[_numMaterials];
			materialsShadowCastEnabled = new bool[_numMaterials];
			_materialNames = new string[_numMaterials];
			for (int i = 0; i < _numMaterials; i++)
			{
				GPUmaterials[i] = dazMesh.materials[i];
				materialsEnabled[i] = dazMesh.materialsEnabled[i];
				materialsShadowCastEnabled[i] = dazMesh.materialsShadowCastEnabled[i];
				_materialNames[i] = dazMesh.materialNames[i];
			}
		}
	}

	protected void InitSmoothing()
	{
		if (meshSmooth == null)
		{
			meshSmooth = new MeshSmooth(dazMesh.baseVertices, dazMesh.basePolyList);
		}
		if (meshSmoothGPU == null)
		{
			meshSmoothGPU = new MeshSmoothGPU(GPUMeshCompute, dazMesh.baseVertices, dazMesh.basePolyList);
		}
	}

	protected void InitRecalcNormalsTangents()
	{
		if (_recalculateNormals && recalcNormals == null)
		{
			recalcNormals = new RecalculateNormals(dazMesh.baseTriangles, workingVerts2, workingNormals, workingSurfaceNormals, useSleep: true);
		}
		if (postSkinRecalcNormals == null)
		{
			postSkinRecalcNormals = new RecalculateNormals(dazMesh.baseTriangles, rawSkinnedWorkingVerts, postSkinWorkingNormals);
		}
		if (recalcNormalsGPU == null)
		{
			recalcNormalsGPU = new RecalculateNormalsGPU(GPUMeshCompute, dazMesh.baseTriangles, numUVVerts, dazMesh.baseVerticesToUVVertices);
			_normalsBuffer = recalcNormalsGPU.normalsBuffer;
			_surfaceNormalsBuffer = recalcNormalsGPU.surfaceNormalsBuffer;
			_normalsBuffer.SetData(startNormals);
		}
		if (_recalculateTangents && recalcTangents == null)
		{
			recalcTangents = new RecalculateTangents(dazMesh.UVTriangles, workingVerts2, workingNormals, dazMesh.UV, workingTangents, useSleep: true);
		}
		if (recalcTangentsGPU == null)
		{
			recalcTangentsGPU = new RecalculateTangentsGPU(GPUMeshCompute, dazMesh.UVTriangles, dazMesh.UV, numUVVerts);
			_tangentsBuffer = recalcTangentsGPU.tangentsBuffer;
			_tangentsBuffer.SetData(startTangents);
		}
	}

	protected void InitMesh()
	{
		if (dazMesh != null)
		{
			if (!dazMesh.wasInit)
			{
				dazMesh.Init();
			}
			mesh = UnityEngine.Object.Instantiate(dazMesh.morphedUVMappedMesh);
			startVerts = dazMesh.morphedUVVertices;
			startVertsCopy = (Vector3[])startVerts.Clone();
			numUVVerts = dazMesh.numUVVertices;
			numBaseVerts = dazMesh.numBaseVertices;
			if (root != null)
			{
				for (int i = 0; i < numBones; i++)
				{
					DAZSkinV2VertexWeights[] weights = nodes[i].weights;
					foreach (DAZSkinV2VertexWeights dAZSkinV2VertexWeights in weights)
					{
						float num = 1f / Vector3.Distance(startVerts[dAZSkinV2VertexWeights.vertex], dazBones[i].worldPosition);
						dAZSkinV2VertexWeights.xleftbulge *= num;
						dAZSkinV2VertexWeights.xrightbulge *= num;
						dAZSkinV2VertexWeights.yleftbulge *= num;
						dAZSkinV2VertexWeights.yrightbulge *= num;
						dAZSkinV2VertexWeights.zleftbulge *= num;
						dAZSkinV2VertexWeights.zrightbulge *= num;
					}
				}
			}
			drawVerts = (Vector3[])startVerts.Clone();
			postSkinVerts = new bool[numUVVerts];
			postSkinNeededVerts = new bool[numUVVerts];
			postSkinNeededVertsList = new int[0];
			postSkinNormalVerts = new bool[numUVVerts];
			postSkinMorphs = new Vector3[numUVVerts];
			workingVerts = (Vector3[])startVerts.Clone();
			workingVerts2 = (Vector3[])startVerts.Clone();
			rawSkinnedWorkingVerts = (Vector3[])startVerts.Clone();
			rawSkinnedVerts = (Vector3[])startVerts.Clone();
			smoothedVerts = (Vector3[])startVerts.Clone();
			unsmoothedVerts = (Vector3[])startVerts.Clone();
			baseVertsToUVVertsCopy = (DAZVertexMap[])dazMesh.baseVerticesToUVVertices.Clone();
			isBaseVert = new bool[numUVVerts];
			for (int k = 0; k < numUVVerts; k++)
			{
				isBaseVert[k] = true;
			}
			numUVOnlyVerts = 0;
			for (int l = 0; l < baseVertsToUVVertsCopy.Length; l++)
			{
				DAZVertexMap dAZVertexMap = baseVertsToUVVertsCopy[l];
				int tovert = dAZVertexMap.tovert;
				isBaseVert[tovert] = false;
			}
			startNormals = dazMesh.morphedUVNormals;
			drawNormals = (Vector3[])startNormals.Clone();
			workingNormals = (Vector3[])startNormals.Clone();
			postSkinWorkingNormals = (Vector3[])startNormals.Clone();
			postSkinNormals = (Vector3[])startNormals.Clone();
			workingSurfaceNormals = new Vector3[dazMesh.baseTriangles.Length / 3];
			drawSurfaceNormals = new Vector3[dazMesh.baseTriangles.Length / 3];
			startTangents = dazMesh.morphedUVTangents;
			drawTangents = (Vector4[])startTangents.Clone();
			workingTangents = (Vector4[])startTangents.Clone();
			Bounds bounds = new Bounds(size: new Vector3(10000f, 10000f, 10000f), center: base.transform.position);
			mesh.bounds = bounds;
		}
		else
		{
			UnityEngine.Debug.LogError("Could not find mesh matching sceneGeometryId: " + sceneGeometryId);
		}
	}

	public void SetNumSubThreads(int num)
	{
		numSubThreads = num;
	}

	public void InitSkinTimes()
	{
		threadSkinTime = new float[_numSubThreads];
		threadSkinVertsCount = new int[_numSubThreads];
		threadSkinStartTime = new float[_numSubThreads];
		threadSkinStopTime = new float[_numSubThreads];
		threadSmoothTime = new float[_numSubThreads];
		threadSmoothStartTime = new float[_numSubThreads];
		threadSmoothStopTime = new float[_numSubThreads];
		threadSmoothCorrectionTime = new float[_numSubThreads];
		threadSmoothCorrectionStartTime = new float[_numSubThreads];
		threadSmoothCorrectionStopTime = new float[_numSubThreads];
	}

	protected void StopThreads()
	{
		_threadsRunning = false;
		if (tasks != null)
		{
			for (int i = 0; i < tasks.Length; i++)
			{
				tasks[i].kill = true;
				tasks[i].resetEvent.Set();
				while (tasks[i].thread.IsAlive)
				{
				}
			}
		}
		tasks = null;
		if (normalTangentTask != null)
		{
			normalTangentTask.kill = true;
			normalTangentTask.resetEvent.Set();
			while (normalTangentTask.thread.IsAlive)
			{
			}
			normalTangentTask = null;
		}
		if (mainSkinTask != null)
		{
			mainSkinTask.kill = true;
			mainSkinTask.resetEvent.Set();
			while (mainSkinTask.thread.IsAlive)
			{
			}
			mainSkinTask = null;
		}
		if (postSkinMorphTask != null)
		{
			postSkinMorphTask.kill = true;
			postSkinMorphTask.resetEvent.Set();
			while (postSkinMorphTask.thread.IsAlive)
			{
			}
			postSkinMorphTask = null;
		}
	}

	protected void StartThreads()
	{
		if (_threadsRunning)
		{
			return;
		}
		_threadsRunning = true;
		if (_numSubThreads > 0)
		{
			tasks = new DAZSkinTaskInfo[_numSubThreads];
			for (int i = 0; i < _numSubThreads; i++)
			{
				DAZSkinTaskInfo dAZSkinTaskInfo = new DAZSkinTaskInfo();
				dAZSkinTaskInfo.threadIndex = i;
				dAZSkinTaskInfo.name = "Task" + i;
				dAZSkinTaskInfo.resetEvent = new AutoResetEvent(initialState: false);
				dAZSkinTaskInfo.thread = new Thread(MTTask);
				dAZSkinTaskInfo.thread.Priority = System.Threading.ThreadPriority.Normal;
				dAZSkinTaskInfo.thread.Start(dAZSkinTaskInfo);
				tasks[i] = dAZSkinTaskInfo;
			}
		}
		normalTangentTask = new DAZSkinTaskInfo();
		normalTangentTask.threadIndex = 0;
		normalTangentTask.name = "NormalTangentTask";
		normalTangentTask.resetEvent = new AutoResetEvent(initialState: false);
		normalTangentTask.thread = new Thread(MTTask);
		normalTangentTask.thread.Priority = System.Threading.ThreadPriority.BelowNormal;
		normalTangentTask.taskType = DAZSkinTaskType.NormalTangentRecalc;
		normalTangentTask.thread.Start(normalTangentTask);
		mainSkinTask = new DAZSkinTaskInfo();
		mainSkinTask.threadIndex = 0;
		mainSkinTask.name = "MainSkinTask";
		mainSkinTask.resetEvent = new AutoResetEvent(initialState: false);
		mainSkinTask.thread = new Thread(MTTask);
		mainSkinTask.thread.Priority = System.Threading.ThreadPriority.Normal;
		mainSkinTask.taskType = DAZSkinTaskType.MainSkin;
		mainSkinTask.thread.Start(mainSkinTask);
		postSkinMorphTask = new DAZSkinTaskInfo();
		postSkinMorphTask.threadIndex = 0;
		postSkinMorphTask.name = "PostSkinMorphTask";
		postSkinMorphTask.resetEvent = new AutoResetEvent(initialState: false);
		postSkinMorphTask.thread = new Thread(MTTask);
		postSkinMorphTask.thread.Priority = System.Threading.ThreadPriority.Normal;
		postSkinMorphTask.taskType = DAZSkinTaskType.PostSkinMorph;
		postSkinMorphTask.thread.Start(postSkinMorphTask);
	}

	protected void OnApplicationQuit()
	{
		if (Application.isPlaying)
		{
			StopThreads();
		}
	}

	protected void MTTask(object info)
	{
		DAZSkinTaskInfo dAZSkinTaskInfo = (DAZSkinTaskInfo)info;
		while (_threadsRunning)
		{
			dAZSkinTaskInfo.resetEvent.WaitOne(-1, exitContext: true);
			if (dAZSkinTaskInfo.kill)
			{
				break;
			}
			if (dAZSkinTaskInfo.taskType == DAZSkinTaskType.Skin)
			{
				threadSkinStartTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * f;
				SkinMeshPart(dAZSkinTaskInfo.index1, dAZSkinTaskInfo.index2, isBaseVert);
				threadSkinStopTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * f;
				threadSkinTime[dAZSkinTaskInfo.threadIndex] = threadSkinStopTime[dAZSkinTaskInfo.threadIndex] - threadSkinStartTime[dAZSkinTaskInfo.threadIndex];
			}
			else if (dAZSkinTaskInfo.taskType == DAZSkinTaskType.Smooth)
			{
				threadSmoothStartTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * f;
				meshSmooth.LaplacianSmooth(workingVerts, smoothedVerts, dAZSkinTaskInfo.index1, dAZSkinTaskInfo.index2);
				threadSmoothStopTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * f;
				threadSmoothTime[dAZSkinTaskInfo.threadIndex] = threadSmoothStopTime[dAZSkinTaskInfo.threadIndex] - threadSmoothStartTime[dAZSkinTaskInfo.threadIndex];
			}
			else if (dAZSkinTaskInfo.taskType == DAZSkinTaskType.SmoothCorrection)
			{
				threadSmoothCorrectionStartTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * f;
				meshSmooth.HCCorrection(workingVerts, smoothedVerts, laplacianSmoothBeta, dAZSkinTaskInfo.index1, dAZSkinTaskInfo.index2);
				threadSmoothCorrectionStopTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * f;
				threadSmoothCorrectionTime[dAZSkinTaskInfo.threadIndex] = threadSmoothCorrectionStopTime[dAZSkinTaskInfo.threadIndex] - threadSmoothCorrectionStartTime[dAZSkinTaskInfo.threadIndex];
			}
			else if (dAZSkinTaskInfo.taskType == DAZSkinTaskType.NormalTangentRecalc)
			{
				threadRecalcNormalTangentStartTime = (float)stopwatch.ElapsedTicks * f;
				Thread.Sleep(0);
				NormalTangentRecalc();
				threadRecalcNormalTangentStopTime = (float)stopwatch.ElapsedTicks * f;
				threadRecalcNormalTangentTime = threadRecalcNormalTangentStopTime - threadRecalcNormalTangentStartTime;
			}
			else if (dAZSkinTaskInfo.taskType == DAZSkinTaskType.MainSkin)
			{
				Thread.Sleep(0);
				SkinMeshThreaded();
			}
			else if (dAZSkinTaskInfo.taskType == DAZSkinTaskType.PostSkinMorph)
			{
				Thread.Sleep(0);
				SkinMeshGPUPostSkinVertsThreaded();
			}
			dAZSkinTaskInfo.working = false;
		}
	}

	protected void NormalTangentRecalc()
	{
		if (_recalculateNormals)
		{
			recalcNormals.recalculateNormals();
			DAZVertexMap[] array = baseVertsToUVVertsCopy;
			foreach (DAZVertexMap dAZVertexMap in array)
			{
				ref Vector3 reference = ref workingNormals[dAZVertexMap.tovert];
				reference = workingNormals[dAZVertexMap.fromvert];
			}
		}
		if (_recalculateTangents)
		{
			recalcTangents.recalculateTangents();
		}
	}

	protected void SkinMeshGPUMaterialInit()
	{
		if (!GPUAutoSwapShader)
		{
			return;
		}
		for (int i = 0; i < GPUmaterials.Length; i++)
		{
			if (!(GPUmaterials[i] != null))
			{
				continue;
			}
			Shader shader = GPUmaterials[i].shader;
			Shader shader2;
			if (GPUAutoSwapCopyNum > 0)
			{
				shader2 = Shader.Find(shader.name + "ComputeBuffCopy" + GPUAutoSwapCopyNum);
				if (shader2 == null)
				{
					shader2 = Shader.Find(shader.name + "ComputeBuff");
				}
			}
			else
			{
				shader2 = Shader.Find(shader.name + "ComputeBuff");
			}
			Material material = new Material(GPUmaterials[i]);
			if (shader2 != null)
			{
				material.shader = shader2;
			}
			GPUmaterials[i] = material;
		}
		if (GPUsimpleMaterial != null)
		{
			Shader shader3 = GPUsimpleMaterial.shader;
			Shader shader4 = Shader.Find(shader3.name + "ComputeBuff");
			Material material2 = new Material(GPUsimpleMaterial);
			if (shader4 != null)
			{
				material2.shader = shader4;
			}
			GPUsimpleMaterial = material2;
		}
	}

	protected void SkinMeshGPUInit()
	{
		if (startVertsBuffer != null)
		{
			return;
		}
		_zeroKernel = GPUSkinner.FindKernel("ZeroVerts");
		_skinGeneralKernel = GPUSkinner.FindKernel("GeneralSkin");
		_initKernel = GPUSkinner.FindKernel("InitVerts");
		_copyKernel = GPUSkinner.FindKernel("CopyVerts");
		_skinXYZKernel = GPUSkinner.FindKernel("SkinXYZ");
		_skinXZYKernel = GPUSkinner.FindKernel("SkinXZY");
		_skinYXZKernel = GPUSkinner.FindKernel("SkinYXZ");
		_skinYZXKernel = GPUSkinner.FindKernel("SkinYZX");
		_skinZXYKernel = GPUSkinner.FindKernel("SkinZXY");
		_skinZYXKernel = GPUSkinner.FindKernel("SkinZYX");
		_skinFinishKernel = GPUSkinner.FindKernel("SkinFinish");
		_postSkinMorphKernel = GPUSkinner.FindKernel("PostSkinMorph");
		_copyTangentsKernel = GPUMeshCompute.FindKernel("CopyTangents");
		boneBuffer = new ComputeBuffer[numBones];
		numVertThreadGroups = numUVVerts / 256;
		if (numUVVerts % 256 != 0)
		{
			numVertThreadGroups++;
		}
		int num = (_nullVertexIndex = numVertThreadGroups * 256);
		startVertsBuffer = new ComputeBuffer(num + 1, 12);
		rawVertsBuffer = new ComputeBuffer(num + 1, 12);
		postSkinMorphsBuffer = new ComputeBuffer(num + 1, 12);
		_verticesBuffer1 = new ComputeBuffer(num + 1, 12);
		_verticesBuffer2 = new ComputeBuffer(num + 1, 12);
		delayedVertsBuffer = new ComputeBuffer(num + 1, 12);
		delayedNormalsBuffer = new ComputeBuffer(num + 1, 12);
		delayedTangentsBuffer = new ComputeBuffer(num + 1, 16);
		weightsBuffer = new ComputeBuffer[numBones];
		fullWeightsBuffer = new ComputeBuffer[numBones];
		generalWeightsBuffer = new ComputeBuffer[numBones];
		numSkinThreadGroups = new int[numBones];
		numSkinFinishThreadGroups = new int[numBones];
		numGeneralSkinThreadGroups = new int[numBones];
		for (int i = 0; i < numBones; i++)
		{
			boneBuffer[i] = new ComputeBuffer(1, 252);
			int num2 = nodes[i].weights.Length;
			numSkinThreadGroups[i] = num2 / 256;
			if (num2 % 256 != 0)
			{
				numSkinThreadGroups[i]++;
			}
			int num3 = numSkinThreadGroups[i] * 256;
			if (num3 > 0)
			{
				weightsBuffer[i] = new ComputeBuffer(num3, 40);
				BoneWeights[] array = new BoneWeights[num3];
				for (int j = 0; j < num3; j++)
				{
					if (j < num2)
					{
						array[j].vertex = nodes[i].weights[j].vertex;
						array[j].xweight = nodes[i].weights[j].xweight;
						array[j].yweight = nodes[i].weights[j].yweight;
						array[j].zweight = nodes[i].weights[j].zweight;
						array[j].xleftbulge = nodes[i].weights[j].xleftbulge;
						array[j].xrightbulge = nodes[i].weights[j].xrightbulge;
						array[j].yleftbulge = nodes[i].weights[j].yleftbulge;
						array[j].yrightbulge = nodes[i].weights[j].yrightbulge;
						array[j].zleftbulge = nodes[i].weights[j].zleftbulge;
						array[j].zrightbulge = nodes[i].weights[j].zrightbulge;
					}
					else
					{
						array[j].vertex = _nullVertexIndex;
						array[j].xweight = 0f;
						array[j].yweight = 0f;
						array[j].zweight = 0f;
						array[j].xleftbulge = 0f;
						array[j].xrightbulge = 0f;
						array[j].yleftbulge = 0f;
						array[j].yrightbulge = 0f;
						array[j].zleftbulge = 0f;
						array[j].zrightbulge = 0f;
					}
				}
				weightsBuffer[i].SetData(array);
			}
			int num4 = nodes[i].fullyWeightedVertices.Length;
			numSkinFinishThreadGroups[i] = num4 / 256;
			if (num4 % 256 != 0)
			{
				numSkinFinishThreadGroups[i]++;
			}
			int num5 = numSkinFinishThreadGroups[i] * 256;
			if (num5 > 0)
			{
				fullWeightsBuffer[i] = new ComputeBuffer(num5, 4);
				BoneFullWeights[] array2 = new BoneFullWeights[num5];
				for (int k = 0; k < num5; k++)
				{
					if (k < num4)
					{
						array2[k].vertex = nodes[i].fullyWeightedVertices[k];
					}
					else
					{
						array2[k].vertex = _nullVertexIndex;
					}
				}
				fullWeightsBuffer[i].SetData(array2);
			}
			int num6 = nodes[i].generalWeights.Length;
			numGeneralSkinThreadGroups[i] = num6 / 256;
			if (num6 % 256 != 0)
			{
				numGeneralSkinThreadGroups[i]++;
			}
			int num7 = numGeneralSkinThreadGroups[i] * 256;
			if (num7 <= 0)
			{
				continue;
			}
			generalWeightsBuffer[i] = new ComputeBuffer(num7, 8);
			BoneGeneralWeights[] array3 = new BoneGeneralWeights[num7];
			for (int l = 0; l < num7; l++)
			{
				if (l < num6)
				{
					array3[l].vertex = nodes[i].generalWeights[l].vertex;
					array3[l].weight = nodes[i].generalWeights[l].weight;
				}
				else
				{
					array3[l].vertex = _nullVertexIndex;
					array3[l].weight = 0f;
				}
			}
			generalWeightsBuffer[i].SetData(array3);
		}
		mapVerticesGPU = new MapVerticesGPU(GPUMeshCompute, dazMesh.baseVerticesToUVVertices);
	}

	protected void SkinMeshGPUCleanup()
	{
		if (mapVerticesGPU != null)
		{
			mapVerticesGPU.Release();
			mapVerticesGPU = null;
		}
		if (meshSmoothGPU != null)
		{
			meshSmoothGPU.Release();
			meshSmoothGPU = null;
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
		if (boneBuffer != null)
		{
			for (int i = 0; i < numBones; i++)
			{
				boneBuffer[i].Release();
				boneBuffer[i] = null;
			}
			boneBuffer = null;
		}
		if (startVertsBuffer != null)
		{
			startVertsBuffer.Release();
			startVertsBuffer = null;
		}
		if (rawVertsBuffer != null)
		{
			rawVertsBuffer.Release();
			rawVertsBuffer = null;
		}
		if (postSkinMorphsBuffer != null)
		{
			postSkinMorphsBuffer.Release();
			postSkinMorphsBuffer = null;
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
		if (delayedVertsBuffer != null)
		{
			delayedVertsBuffer.Release();
			delayedVertsBuffer = null;
		}
		if (delayedNormalsBuffer != null)
		{
			delayedNormalsBuffer.Release();
			delayedNormalsBuffer = null;
		}
		if (delayedTangentsBuffer != null)
		{
			delayedTangentsBuffer.Release();
			delayedTangentsBuffer = null;
		}
		if (weightsBuffer != null)
		{
			for (int j = 0; j < numBones; j++)
			{
				if (weightsBuffer[j] != null)
				{
					weightsBuffer[j].Release();
					weightsBuffer[j] = null;
				}
			}
			weightsBuffer = null;
		}
		if (fullWeightsBuffer != null)
		{
			for (int k = 0; k < numBones; k++)
			{
				if (fullWeightsBuffer[k] != null)
				{
					fullWeightsBuffer[k].Release();
					fullWeightsBuffer[k] = null;
				}
			}
			fullWeightsBuffer = null;
		}
		if (generalWeightsBuffer == null)
		{
			return;
		}
		for (int l = 0; l < numBones; l++)
		{
			if (generalWeightsBuffer[l] != null)
			{
				generalWeightsBuffer[l].Release();
				generalWeightsBuffer[l] = null;
			}
		}
		generalWeightsBuffer = null;
	}

	protected void RecalculatePostSkinNeededVertsAndTriangles()
	{
		for (int i = 0; i < numUVVerts; i++)
		{
			postSkinNeededVerts[i] = postSkinVerts[i];
		}
		needsPostSkinNormals = false;
		List<int> list = new List<int>();
		int[] baseTriangles = dazMesh.baseTriangles;
		for (int j = 0; j < baseTriangles.Length; j += 3)
		{
			int num = baseTriangles[j];
			int num2 = baseTriangles[j + 1];
			int num3 = baseTriangles[j + 2];
			if (postSkinNormalVerts[num] || postSkinNormalVerts[num2] || postSkinNormalVerts[num3])
			{
				needsPostSkinNormals = true;
				list.Add(j);
				postSkinNeededVerts[num] = true;
				postSkinNeededVerts[num2] = true;
				postSkinNeededVerts[num3] = true;
			}
		}
		List<int> list2 = new List<int>();
		for (int k = 0; k < numUVVerts; k++)
		{
			if (postSkinNeededVerts[k])
			{
				list2.Add(k);
			}
		}
		postSkinNeededTriangles = list.ToArray();
		postSkinNeededVertsList = list2.ToArray();
	}

	protected bool[] DetermineUsedBonesForVerts(bool[] usedVerts)
	{
		bool[] array = new bool[_numBones];
		for (int i = 0; i < _numBones; i++)
		{
			if (_useGeneralWeights)
			{
				DAZSkinV2GeneralVertexWeights[] generalWeights = nodes[i].generalWeights;
				for (int j = 0; j < generalWeights.Length; j++)
				{
					if (usedVerts[generalWeights[j].vertex])
					{
						array[i] = true;
						break;
					}
				}
				continue;
			}
			DAZSkinV2VertexWeights[] weights = nodes[i].weights;
			bool flag = false;
			for (int k = 0; k < weights.Length; k++)
			{
				if (usedVerts[weights[k].vertex])
				{
					array[i] = true;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			int[] fullyWeightedVertices = nodes[i].fullyWeightedVertices;
			for (int l = 0; l < fullyWeightedVertices.Length; l++)
			{
				if (usedVerts[fullyWeightedVertices[l]])
				{
					array[i] = true;
					break;
				}
			}
		}
		return array;
	}

	public void SkinMeshGPUPostSkinVertsStart()
	{
		if (_useGeneralWeights)
		{
			for (int i = 0; i < postSkinNeededVertsList.Length; i++)
			{
				int num = postSkinNeededVertsList[i];
				ref Vector3 reference = ref startVertsCopy[num];
				reference = startVerts[num];
				workingVerts[num].x = 0f;
				workingVerts[num].y = 0f;
				workingVerts[num].z = 0f;
			}
		}
		else
		{
			for (int j = 0; j < postSkinNeededVertsList.Length; j++)
			{
				int num2 = postSkinNeededVertsList[j];
				ref Vector3 reference2 = ref workingVerts[num2];
				reference2 = startVerts[num2];
			}
		}
		postSkinVertsChangedThreaded = postSkinVertsChanged;
		postSkinVertsChanged = false;
	}

	public void SkinMeshGPUPostSkinVertsThreaded()
	{
		if (postSkinVertsChangedThreaded)
		{
			RecalculatePostSkinNeededVertsAndTriangles();
			postSkinBones = DetermineUsedBonesForVerts(postSkinNeededVerts);
			postSkinVertsChangedThreaded = false;
		}
		SkinMeshPart(0, numBaseVerts - 1, postSkinNeededVerts, postSkinBones);
		for (int i = 0; i < postSkinNeededVertsList.Length; i++)
		{
			int num = postSkinNeededVertsList[i];
			ref Vector3 reference = ref rawSkinnedWorkingVerts[num];
			reference = workingVerts[num];
		}
		if (needsPostSkinNormals)
		{
			postSkinRecalcNormals.recalculateNormals(postSkinNeededTriangles, postSkinNeededVertsList);
		}
	}

	public void SkinMeshGPUPostSkinVertsFinish()
	{
		for (int i = 0; i < postSkinNeededVertsList.Length; i++)
		{
			int num = postSkinNeededVertsList[i];
			ref Vector3 reference = ref rawSkinnedVerts[num];
			reference = rawSkinnedWorkingVerts[num];
			ref Vector3 reference2 = ref postSkinNormals[num];
			reference2 = postSkinWorkingNormals[num];
		}
	}

	public void SkinMeshGPUPostSkinVerts()
	{
		if (useAsynchronousThreadedSkinning)
		{
			if (!postSkinMorphTask.working)
			{
				SkinMeshGPUPostSkinVertsFinish();
				SkinMeshGPUPostSkinVertsStart();
				postSkinMorphTask.working = true;
				postSkinMorphTask.resetEvent.Set();
			}
		}
		else
		{
			SkinMeshGPUPostSkinVertsStart();
			SkinMeshGPUPostSkinVertsThreaded();
			SkinMeshGPUPostSkinVertsFinish();
		}
	}

	protected void SkinMeshGPUFrame()
	{
		startVertsBuffer.SetData(startVerts);
		if (_useGeneralWeights)
		{
			GPUSkinner.SetBuffer(_zeroKernel, "outVerts", rawVertsBuffer);
			GPUSkinner.Dispatch(_zeroKernel, numVertThreadGroups, 1, 1);
		}
		else
		{
			GPUSkinner.SetBuffer(_initKernel, "inVerts", startVertsBuffer);
			GPUSkinner.SetBuffer(_initKernel, "outVerts", rawVertsBuffer);
			GPUSkinner.Dispatch(_initKernel, numVertThreadGroups, 1, 1);
		}
		Bone[] array = new Bone[1] { default(Bone) };
		for (int i = 0; i < numBones; i++)
		{
			DAZBone dAZBone = dazBones[i];
			array[0].changeFromOriginal = dAZBone.changeFromOriginalMatrix;
			ref Matrix4x4 reference = ref boneChangeFromOriginalMatrices[i];
			reference = dAZBone.changeFromOriginalMatrix;
			if (!_useGeneralWeights)
			{
				DAZSkinV2BulgeFactors bulgeFactors = nodes[i].bulgeFactors;
				Vector3 angles = dAZBone.GetAngles();
				array[0].rotationAngles = angles;
				array[0].worldToLocal = dAZBone.morphedWorldToLocalMatrix;
				array[0].localToWorld = dAZBone.morphedLocalToWorldMatrix;
				boneRotationAngles[i] = angles;
				ref Matrix4x4 reference2 = ref boneWorldToLocalMatrices[i];
				reference2 = dAZBone.morphedWorldToLocalMatrix;
				ref Matrix4x4 reference3 = ref boneLocalToWorldMatrices[i];
				reference3 = dAZBone.morphedLocalToWorldMatrix;
				if (angles.x >= 0f)
				{
					array[0].xposleftbulge = bulgeFactors.xposleft * angles.x * bulgeScale;
					array[0].xposrightbulge = bulgeFactors.xposright * angles.x * bulgeScale;
					array[0].xnegleftbulge = 0f;
					array[0].xnegrightbulge = 0f;
				}
				else
				{
					array[0].xposleftbulge = 0f;
					array[0].xposrightbulge = 0f;
					array[0].xnegleftbulge = bulgeFactors.xnegleft * angles.x * bulgeScale;
					array[0].xnegrightbulge = bulgeFactors.xnegright * angles.x * bulgeScale;
				}
				if (angles.y >= 0f)
				{
					array[0].yposleftbulge = bulgeFactors.yposleft * angles.y * bulgeScale;
					array[0].yposrightbulge = bulgeFactors.yposright * angles.y * bulgeScale;
					array[0].ynegleftbulge = 0f;
					array[0].ynegrightbulge = 0f;
				}
				else
				{
					array[0].yposleftbulge = 0f;
					array[0].yposrightbulge = 0f;
					array[0].ynegleftbulge = bulgeFactors.ynegleft * angles.y * bulgeScale;
					array[0].ynegrightbulge = bulgeFactors.ynegright * angles.y * bulgeScale;
				}
				if (angles.z >= 0f)
				{
					array[0].zposleftbulge = bulgeFactors.zposleft * angles.z * bulgeScale;
					array[0].zposrightbulge = bulgeFactors.zposright * angles.z * bulgeScale;
					array[0].znegleftbulge = 0f;
					array[0].znegrightbulge = 0f;
				}
				else
				{
					array[0].zposleftbulge = 0f;
					array[0].zposrightbulge = 0f;
					array[0].znegleftbulge = bulgeFactors.znegleft * angles.z * bulgeScale;
					array[0].znegrightbulge = bulgeFactors.znegright * angles.z * bulgeScale;
				}
			}
			boneBuffer[i].SetData(array);
			if (_useGeneralWeights)
			{
				if (numGeneralSkinThreadGroups[i] > 0)
				{
					GPUSkinner.SetBuffer(_skinGeneralKernel, "bone", boneBuffer[i]);
					GPUSkinner.SetBuffer(_skinGeneralKernel, "generalWeights", generalWeightsBuffer[i]);
					GPUSkinner.SetBuffer(_skinGeneralKernel, "inVerts", startVertsBuffer);
					GPUSkinner.SetBuffer(_skinGeneralKernel, "outVerts", rawVertsBuffer);
					GPUSkinner.Dispatch(_skinGeneralKernel, numGeneralSkinThreadGroups[i], 1, 1);
				}
				continue;
			}
			if (numSkinThreadGroups[i] > 0)
			{
				switch (nodes[i].rotationOrder)
				{
				case Quaternion2Angles.RotationOrder.XYZ:
					GPUSkinner.SetBuffer(_skinXYZKernel, "bone", boneBuffer[i]);
					GPUSkinner.SetBuffer(_skinXYZKernel, "weights", weightsBuffer[i]);
					GPUSkinner.SetBuffer(_skinXYZKernel, "outVerts", rawVertsBuffer);
					GPUSkinner.Dispatch(_skinXYZKernel, numSkinThreadGroups[i], 1, 1);
					break;
				case Quaternion2Angles.RotationOrder.XZY:
					GPUSkinner.SetBuffer(_skinXZYKernel, "bone", boneBuffer[i]);
					GPUSkinner.SetBuffer(_skinXZYKernel, "weights", weightsBuffer[i]);
					GPUSkinner.SetBuffer(_skinXZYKernel, "outVerts", rawVertsBuffer);
					GPUSkinner.Dispatch(_skinXZYKernel, numSkinThreadGroups[i], 1, 1);
					break;
				case Quaternion2Angles.RotationOrder.YXZ:
					GPUSkinner.SetBuffer(_skinYXZKernel, "bone", boneBuffer[i]);
					GPUSkinner.SetBuffer(_skinYXZKernel, "weights", weightsBuffer[i]);
					GPUSkinner.SetBuffer(_skinYXZKernel, "outVerts", rawVertsBuffer);
					GPUSkinner.Dispatch(_skinYXZKernel, numSkinThreadGroups[i], 1, 1);
					break;
				case Quaternion2Angles.RotationOrder.YZX:
					GPUSkinner.SetBuffer(_skinYZXKernel, "bone", boneBuffer[i]);
					GPUSkinner.SetBuffer(_skinYZXKernel, "weights", weightsBuffer[i]);
					GPUSkinner.SetBuffer(_skinYZXKernel, "outVerts", rawVertsBuffer);
					GPUSkinner.Dispatch(_skinYZXKernel, numSkinThreadGroups[i], 1, 1);
					break;
				case Quaternion2Angles.RotationOrder.ZXY:
					GPUSkinner.SetBuffer(_skinZXYKernel, "bone", boneBuffer[i]);
					GPUSkinner.SetBuffer(_skinZXYKernel, "weights", weightsBuffer[i]);
					GPUSkinner.SetBuffer(_skinZXYKernel, "outVerts", rawVertsBuffer);
					GPUSkinner.Dispatch(_skinZXYKernel, numSkinThreadGroups[i], 1, 1);
					break;
				case Quaternion2Angles.RotationOrder.ZYX:
					GPUSkinner.SetBuffer(_skinZYXKernel, "bone", boneBuffer[i]);
					GPUSkinner.SetBuffer(_skinZYXKernel, "weights", weightsBuffer[i]);
					GPUSkinner.SetBuffer(_skinZYXKernel, "outVerts", rawVertsBuffer);
					GPUSkinner.Dispatch(_skinZYXKernel, numSkinThreadGroups[i], 1, 1);
					break;
				}
			}
			if (numSkinFinishThreadGroups[i] > 0)
			{
				GPUSkinner.SetBuffer(_skinFinishKernel, "bone", boneBuffer[i]);
				GPUSkinner.SetBuffer(_skinFinishKernel, "fullWeights", fullWeightsBuffer[i]);
				GPUSkinner.SetBuffer(_skinFinishKernel, "outVerts", rawVertsBuffer);
				GPUSkinner.Dispatch(_skinFinishKernel, numSkinFinishThreadGroups[i], 1, 1);
			}
		}
	}

	protected void SkinMeshGPU()
	{
		lastFrameSkinStartTime = skinStartTime;
		skinStartTime = (float)stopwatch.ElapsedTicks * f;
		if (mesh != null && root != null && GPUSkinner != null)
		{
			StartThreads();
			SkinMeshGPUInit();
			SkinMeshGPUFrame();
			InitRecalcNormalsTangents();
			if (allowPostSkinMorph)
			{
				SkinMeshGPUPostSkinVerts();
				postSkinMorphsBuffer.SetData(postSkinMorphs);
				GPUSkinner.SetBuffer(_postSkinMorphKernel, "postSkinMorphs", postSkinMorphsBuffer);
				GPUSkinner.SetBuffer(_postSkinMorphKernel, "outVerts", rawVertsBuffer);
				GPUSkinner.Dispatch(_postSkinMorphKernel, numVertThreadGroups, 1, 1);
			}
			if (_useSmoothing)
			{
				InitSmoothing();
				int num = 0;
				smoothedVertsBuffer = rawVertsBuffer;
				if (UserPreferences.singleton != null)
				{
					smoothOuterLoops = UserPreferences.singleton.smoothPasses;
				}
				for (int i = 0; i < smoothOuterLoops; i++)
				{
					for (int j = 0; j < laplacianSmoothPasses; j++)
					{
						if (num == 0)
						{
							meshSmoothGPU.LaplacianSmoothGPU(rawVertsBuffer, _verticesBuffer2);
							meshSmoothGPU.HCCorrectionGPU(rawVertsBuffer, _verticesBuffer2, laplacianSmoothBeta);
							smoothedVertsBuffer = _verticesBuffer2;
						}
						else if (num % 2 == 0)
						{
							meshSmoothGPU.LaplacianSmoothGPU(_verticesBuffer1, _verticesBuffer2);
							meshSmoothGPU.HCCorrectionGPU(_verticesBuffer1, _verticesBuffer2, laplacianSmoothBeta);
							smoothedVertsBuffer = _verticesBuffer2;
						}
						else
						{
							meshSmoothGPU.LaplacianSmoothGPU(_verticesBuffer2, _verticesBuffer1);
							meshSmoothGPU.HCCorrectionGPU(_verticesBuffer2, _verticesBuffer1, laplacianSmoothBeta);
							smoothedVertsBuffer = _verticesBuffer1;
						}
						num++;
					}
					for (int k = 0; k < springSmoothPasses; k++)
					{
						if (num == 0)
						{
							meshSmoothGPU.SpringSmoothGPU(rawVertsBuffer, _verticesBuffer2, springSmoothFactor);
							smoothedVertsBuffer = _verticesBuffer2;
						}
						else if (num % 2 == 0)
						{
							meshSmoothGPU.SpringSmoothGPU(_verticesBuffer1, _verticesBuffer2, springSmoothFactor);
							smoothedVertsBuffer = _verticesBuffer2;
						}
						else
						{
							meshSmoothGPU.SpringSmoothGPU(_verticesBuffer2, _verticesBuffer1, springSmoothFactor);
							smoothedVertsBuffer = _verticesBuffer1;
						}
						num++;
					}
				}
				mapVerticesGPU.Map(smoothedVertsBuffer);
			}
			else
			{
				mapVerticesGPU.Map(rawVertsBuffer);
			}
			if (_recalculateNormals)
			{
				if (_useSmoothing && useSmoothVertsForNormalTangentRecalc)
				{
					recalcNormalsGPU.RecalculateNormals(smoothedVertsBuffer);
				}
				else
				{
					recalcNormalsGPU.RecalculateNormals(rawVertsBuffer);
				}
			}
			if (_recalculateTangents)
			{
				if (_useSmoothing && useSmoothVertsForNormalTangentRecalc)
				{
					recalcTangentsGPU.RecalculateTangents(smoothedVertsBuffer, _normalsBuffer);
				}
				else
				{
					recalcTangentsGPU.RecalculateTangents(rawVertsBuffer, _normalsBuffer);
				}
			}
		}
		skinStopTime = (float)stopwatch.ElapsedTicks * f;
		skinTime = skinStopTime - skinStartTime;
	}

	protected void SkinMeshCPUandGPUFinishFrame()
	{
		if (allowPostSkinMorph)
		{
			for (int i = 0; i < postSkinNeededVertsList.Length; i++)
			{
				int num = postSkinNeededVertsList[i];
				workingVerts[num] += postSkinMorphs[num];
			}
		}
		if (debugVertex != -1)
		{
			MyDebug.DrawWireCube(workingVerts[debugVertex], 0.002f, Color.white);
		}
		rawVertsBuffer.SetData(workingVerts);
		if (_useSmoothing)
		{
			InitSmoothing();
			int num2 = 0;
			smoothedVertsBuffer = rawVertsBuffer;
			smoothedVertsBuffer = rawVertsBuffer;
			if (UserPreferences.singleton != null)
			{
				smoothOuterLoops = UserPreferences.singleton.smoothPasses;
			}
			for (int j = 0; j < smoothOuterLoops; j++)
			{
				for (int k = 0; k < laplacianSmoothPasses; k++)
				{
					if (num2 == 0)
					{
						meshSmoothGPU.LaplacianSmoothGPU(rawVertsBuffer, _verticesBuffer2);
						meshSmoothGPU.HCCorrectionGPU(rawVertsBuffer, _verticesBuffer2, laplacianSmoothBeta);
						smoothedVertsBuffer = _verticesBuffer2;
					}
					else if (num2 % 2 == 0)
					{
						meshSmoothGPU.LaplacianSmoothGPU(_verticesBuffer1, _verticesBuffer2);
						meshSmoothGPU.HCCorrectionGPU(_verticesBuffer1, _verticesBuffer2, laplacianSmoothBeta);
						smoothedVertsBuffer = _verticesBuffer2;
					}
					else
					{
						meshSmoothGPU.LaplacianSmoothGPU(_verticesBuffer2, _verticesBuffer1);
						meshSmoothGPU.HCCorrectionGPU(_verticesBuffer2, _verticesBuffer1, laplacianSmoothBeta);
						smoothedVertsBuffer = _verticesBuffer1;
					}
					num2++;
				}
				for (int l = 0; l < springSmoothPasses; l++)
				{
					if (num2 == 0)
					{
						meshSmoothGPU.SpringSmoothGPU(rawVertsBuffer, _verticesBuffer2, springSmoothFactor);
						smoothedVertsBuffer = _verticesBuffer2;
					}
					else if (num2 % 2 == 0)
					{
						meshSmoothGPU.SpringSmoothGPU(_verticesBuffer1, _verticesBuffer2, springSmoothFactor);
						smoothedVertsBuffer = _verticesBuffer2;
					}
					else
					{
						meshSmoothGPU.SpringSmoothGPU(_verticesBuffer2, _verticesBuffer1, springSmoothFactor);
						smoothedVertsBuffer = _verticesBuffer1;
					}
					num2++;
				}
			}
			mapVerticesGPU.Map(smoothedVertsBuffer);
		}
		else
		{
			mapVerticesGPU.Map(rawVertsBuffer);
		}
		InitRecalcNormalsTangents();
		if (_recalculateNormals)
		{
			if (_useSmoothing && useSmoothVertsForNormalTangentRecalc)
			{
				recalcNormalsGPU.RecalculateNormals(smoothedVertsBuffer);
			}
			else
			{
				recalcNormalsGPU.RecalculateNormals(rawVertsBuffer);
			}
		}
		if (_recalculateTangents)
		{
			if (_useSmoothing && useSmoothVertsForNormalTangentRecalc)
			{
				recalcTangentsGPU.RecalculateTangents(smoothedVertsBuffer, _normalsBuffer);
			}
			else
			{
				recalcTangentsGPU.RecalculateTangents(rawVertsBuffer, _normalsBuffer);
			}
		}
	}

	protected void SkinMeshCPUandGPUEarlyFinish()
	{
		if (!useAsynchronousThreadedSkinning)
		{
			return;
		}
		totalFrameCount++;
		while (mainSkinTask.working)
		{
			Thread.Sleep(0);
		}
		if (allowPostSkinMorph)
		{
			for (int i = 0; i < postSkinNeededVertsList.Length; i++)
			{
				int num = postSkinNeededVertsList[i];
				ref Vector3 reference = ref rawSkinnedVerts[num];
				reference = rawSkinnedWorkingVerts[num];
				ref Vector3 reference2 = ref postSkinNormals[num];
				reference2 = postSkinWorkingNormals[num];
			}
		}
	}

	protected void SkinMeshCPUandGPU(bool forceSynchronous = false)
	{
		lastFrameSkinStartTime = skinStartTime;
		skinStartTime = (float)stopwatch.ElapsedTicks * f;
		if (mesh != null && root != null && GPUSkinner != null)
		{
			if (!forceSynchronous)
			{
				StartThreads();
			}
			SkinMeshGPUInit();
			if (useAsynchronousThreadedSkinning && !forceSynchronous)
			{
				totalFrameCount++;
				if (true)
				{
					while (mainSkinTask.working)
					{
						Thread.Sleep(0);
					}
				}
				if (!mainSkinTask.working)
				{
					SkinMeshCPUandGPUFinishFrame();
					RecalcBones();
					SkinMeshStartFrame();
					mainSkinTask.working = true;
					debugStartTime = (float)stopwatch.ElapsedTicks * f;
					mainSkinTask.resetEvent.Set();
					debugStopTime = (float)stopwatch.ElapsedTicks * f;
					debugTime = debugStopTime - debugStartTime;
				}
				else if (OVRManager.isHmdPresent)
				{
					missedFrameCount++;
					float num = (float)stopwatch.ElapsedTicks * f;
					float num2 = num - threadMainSkinStartTime;
					UnityEngine.Debug.LogWarning("Skinning did not complete in 1 frame for " + sceneGeometryId + ". Last thread time " + threadMainSkinTime + " Current thread time " + num2 + " missed " + missedFrameCount + " out of total " + totalFrameCount);
					UnityEngine.Debug.LogWarning("Current time " + num + ". Thread start time " + threadMainSkinStartTime + ". Last thread stop time " + threadMainSkinStopTime + ". Last frame skin start time " + lastFrameSkinStartTime);
					DebugHUD.Msg("Skin miss " + missedFrameCount + " out of total " + totalFrameCount);
					DebugHUD.Alert2();
				}
			}
			else
			{
				RecalcBones();
				SkinMeshStartFrame();
				SkinMeshThreaded(forceSynchronous);
				SkinMeshCPUandGPUFinishFrame();
			}
		}
		skinStopTime = (float)stopwatch.ElapsedTicks * f;
		skinTime = skinStopTime - skinStartTime;
	}

	protected void SkinMeshStartFrame()
	{
		if (_useGeneralWeights)
		{
			for (int i = 0; i < numBaseVerts; i++)
			{
				ref Vector3 reference = ref startVertsCopy[i];
				reference = startVerts[i];
			}
		}
		else
		{
			for (int j = 0; j < numBaseVerts; j++)
			{
				ref Vector3 reference2 = ref workingVerts[j];
				reference2 = startVerts[j];
			}
		}
		postSkinVertsChangedThreaded = postSkinVertsChanged;
		postSkinVertsChanged = false;
	}

	protected void SkinMeshFinishFrame()
	{
		if (allowPostSkinMorph)
		{
			for (int i = 0; i < postSkinNeededVertsList.Length; i++)
			{
				int num = postSkinNeededVertsList[i];
				ref Vector3 reference = ref rawSkinnedVerts[num];
				reference = rawSkinnedWorkingVerts[num];
				ref Vector3 reference2 = ref postSkinNormals[num];
				reference2 = postSkinWorkingNormals[num];
			}
		}
		if (_useSmoothing)
		{
			if (useSmoothVertsForNormalTangentRecalc)
			{
				for (int j = 0; j < numUVVerts; j++)
				{
					ref Vector3 reference3 = ref drawVerts[j];
					reference3 = smoothedVerts[j];
				}
			}
			else
			{
				for (int k = 0; k < numUVVerts; k++)
				{
					ref Vector3 reference4 = ref drawVerts[k];
					reference4 = smoothedVerts[k];
					ref Vector3 reference5 = ref unsmoothedVerts[k];
					reference5 = workingVerts[k];
				}
			}
		}
		else
		{
			for (int l = 0; l < numUVVerts; l++)
			{
				ref Vector3 reference6 = ref drawVerts[l];
				reference6 = workingVerts[l];
			}
		}
		mesh.vertices = drawVerts;
	}

	protected void RecalcBones()
	{
		for (int i = 0; i < numBones; i++)
		{
			DAZBone dAZBone = dazBones[i];
			ref Matrix4x4 reference = ref boneWorldToLocalMatrices[i];
			reference = dAZBone.morphedWorldToLocalMatrix;
			ref Matrix4x4 reference2 = ref boneLocalToWorldMatrices[i];
			reference2 = dAZBone.morphedLocalToWorldMatrix;
			ref Matrix4x4 reference3 = ref boneChangeFromOriginalMatrices[i];
			reference3 = dAZBone.changeFromOriginalMatrix;
			if (!_useGeneralWeights)
			{
				ref Vector3 reference4 = ref boneRotationAngles[i];
				reference4 = dAZBone.GetAngles();
			}
		}
	}

	public void SkinMesh(bool forceSynchronous = false)
	{
		lastFrameSkinStartTime = skinStartTime;
		skinStartTime = (float)stopwatch.ElapsedTicks * f;
		if (mesh != null && root != null)
		{
			if (!forceSynchronous)
			{
				StartThreads();
			}
			InitRecalcNormalsTangents();
			if (_useSmoothing)
			{
				InitSmoothing();
			}
			if (!forceSynchronous && !normalTangentTask.working && useAsynchronousNormalTangentRecalc)
			{
				if (_recalculateNormals)
				{
					for (int i = 0; i < numUVVerts; i++)
					{
						ref Vector3 reference = ref drawNormals[i];
						reference = workingNormals[i];
					}
					for (int j = 0; j < workingSurfaceNormals.Length; j++)
					{
						ref Vector3 reference2 = ref drawSurfaceNormals[j];
						reference2 = workingSurfaceNormals[j];
					}
					mesh.normals = drawNormals;
				}
				if (_recalculateTangents)
				{
					for (int k = 0; k < numUVVerts; k++)
					{
						ref Vector4 reference3 = ref drawTangents[k];
						reference3 = workingTangents[k];
					}
					mesh.tangents = drawTangents;
				}
				if (_recalculateNormals || _recalculateTangents)
				{
					if (useSmoothVertsForNormalTangentRecalc || !_useSmoothing)
					{
						for (int l = 0; l < numUVVerts; l++)
						{
							ref Vector3 reference4 = ref workingVerts2[l];
							reference4 = drawVerts[l];
						}
					}
					else
					{
						for (int m = 0; m < numUVVerts; m++)
						{
							ref Vector3 reference5 = ref workingVerts2[m];
							reference5 = unsmoothedVerts[m];
						}
					}
					normalTangentTask.working = true;
					normalTangentTask.resetEvent.Set();
				}
			}
			if (useAsynchronousThreadedSkinning && !forceSynchronous)
			{
				if (!mainSkinTask.working)
				{
					SkinMeshFinishFrame();
					RecalcBones();
					SkinMeshStartFrame();
					mainSkinTask.working = true;
					debugStartTime = (float)stopwatch.ElapsedTicks * f;
					mainSkinTask.resetEvent.Set();
					debugStopTime = (float)stopwatch.ElapsedTicks * f;
					debugTime = debugStopTime - debugStartTime;
				}
			}
			else
			{
				RecalcBones();
				SkinMeshStartFrame();
				SkinMeshThreaded(forceSynchronous);
				SkinMeshFinishFrame();
			}
		}
		skinStopTime = (float)stopwatch.ElapsedTicks * f;
		skinTime = skinStopTime - skinStartTime;
	}

	protected void SkinMeshThreaded(bool forceSynchronous = false)
	{
		threadMainSkinStartTime = (float)stopwatch.ElapsedTicks * f;
		int num = numBaseVerts;
		if (_useGeneralWeights)
		{
			for (int i = 0; i < num; i++)
			{
				workingVerts[i].x = 0f;
				workingVerts[i].y = 0f;
				workingVerts[i].z = 0f;
			}
		}
		if (useMultithreadedSkinning && !forceSynchronous && _numSubThreads > 0)
		{
			int num2 = (numUVVerts - numUVOnlyVerts) / _numSubThreads;
			for (int j = 0; j < _numSubThreads; j++)
			{
				tasks[j].taskType = DAZSkinTaskType.Skin;
				tasks[j].index1 = j * num2;
				if (j == _numSubThreads - 1)
				{
					tasks[j].index2 = num - 1;
				}
				else
				{
					tasks[j].index2 = (j + 1) * num2 - 1;
				}
				threadSkinVertsCount[j] = tasks[j].index2 - tasks[j].index1 + 1;
				tasks[j].working = true;
				tasks[j].resetEvent.Set();
			}
			bool flag;
			do
			{
				flag = false;
				for (int k = 0; k < _numSubThreads; k++)
				{
					if (tasks[k].working)
					{
						flag = true;
					}
				}
				if (flag)
				{
					Thread.Sleep(0);
				}
			}
			while (flag);
		}
		else
		{
			mainThreadSkinStartTime = (float)stopwatch.ElapsedTicks * f;
			SkinMeshPart(0, num - 1, isBaseVert);
			mainThreadSkinStopTime = (float)stopwatch.ElapsedTicks * f;
			mainThreadSkinTime = mainThreadSkinStopTime - mainThreadSkinStartTime;
		}
		if (postSkinVertsChangedThreaded)
		{
			RecalculatePostSkinNeededVertsAndTriangles();
			postSkinBones = DetermineUsedBonesForVerts(postSkinNeededVerts);
			postSkinVertsChangedThreaded = false;
		}
		if (allowPostSkinMorph)
		{
			for (int l = 0; l < postSkinNeededVertsList.Length; l++)
			{
				int num3 = postSkinNeededVertsList[l];
				ref Vector3 reference = ref rawSkinnedWorkingVerts[num3];
				reference = workingVerts[num3];
			}
		}
		if (needsPostSkinNormals)
		{
			threadRecalcNormalTangentStartTime = (float)stopwatch.ElapsedTicks * f;
			postSkinRecalcNormals.recalculateNormals(postSkinNeededTriangles, postSkinNeededVertsList);
			threadRecalcNormalTangentStopTime = (float)stopwatch.ElapsedTicks * f;
			threadRecalcNormalTangentTime = threadRecalcNormalTangentStopTime - threadRecalcNormalTangentStartTime;
		}
		if (_useSmoothing && skinMethod == SkinMethod.CPU)
		{
			InitSmoothing();
			if (useMultithreadedSmoothing && !forceSynchronous && _numSubThreads > 0)
			{
				int num4 = numBaseVerts / _numSubThreads;
				for (int m = 0; m < _numSubThreads; m++)
				{
					tasks[m].taskType = DAZSkinTaskType.Smooth;
					tasks[m].index1 = m * num4;
					if (m == _numSubThreads - 1)
					{
						tasks[m].index2 = numBaseVerts - 1;
					}
					else
					{
						tasks[m].index2 = (m + 1) * num4 - 1;
					}
					tasks[m].working = true;
					tasks[m].resetEvent.Set();
				}
				bool flag2;
				do
				{
					flag2 = false;
					for (int n = 0; n < _numSubThreads; n++)
					{
						if (tasks[n].working)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						Thread.Sleep(0);
					}
				}
				while (flag2);
				for (int num5 = 0; num5 < _numSubThreads; num5++)
				{
					tasks[num5].taskType = DAZSkinTaskType.SmoothCorrection;
					tasks[num5].index1 = num5 * num4;
					if (num5 == _numSubThreads - 1)
					{
						tasks[num5].index2 = numBaseVerts - 1;
					}
					else
					{
						tasks[num5].index2 = (num5 + 1) * num4 - 1;
					}
					tasks[num5].working = true;
					tasks[num5].resetEvent.Set();
				}
				do
				{
					flag2 = false;
					for (int num6 = 0; num6 < _numSubThreads; num6++)
					{
						if (tasks[num6].working)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						Thread.Sleep(0);
					}
				}
				while (flag2);
				DAZVertexMap[] baseVerticesToUVVertices = dazMesh.baseVerticesToUVVertices;
				foreach (DAZVertexMap dAZVertexMap in baseVerticesToUVVertices)
				{
					ref Vector3 reference2 = ref smoothedVerts[dAZVertexMap.tovert];
					reference2 = smoothedVerts[dAZVertexMap.fromvert];
				}
			}
			else
			{
				if (useAsynchronousThreadedSkinning && !forceSynchronous)
				{
					Thread.Sleep(0);
				}
				mainThreadSmoothStartTime = (float)stopwatch.ElapsedTicks * f;
				meshSmooth.LaplacianSmooth(workingVerts, smoothedVerts);
				mainThreadSmoothStopTime = (float)stopwatch.ElapsedTicks * f;
				mainThreadSmoothTime = mainThreadSmoothStopTime - mainThreadSmoothStartTime;
				mainThreadSmoothCorrectionStartTime = (float)stopwatch.ElapsedTicks * f;
				meshSmooth.HCCorrection(workingVerts, smoothedVerts, laplacianSmoothBeta);
				mainThreadSmoothCorrectionStopTime = (float)stopwatch.ElapsedTicks * f;
				mainThreadSmoothCorrectionTime = mainThreadSmoothCorrectionStopTime - mainThreadSmoothCorrectionStartTime;
				DAZVertexMap[] baseVerticesToUVVertices2 = dazMesh.baseVerticesToUVVertices;
				foreach (DAZVertexMap dAZVertexMap2 in baseVerticesToUVVertices2)
				{
					ref Vector3 reference3 = ref smoothedVerts[dAZVertexMap2.tovert];
					reference3 = smoothedVerts[dAZVertexMap2.fromvert];
				}
			}
		}
		else
		{
			DAZVertexMap[] baseVerticesToUVVertices3 = dazMesh.baseVerticesToUVVertices;
			foreach (DAZVertexMap dAZVertexMap3 in baseVerticesToUVVertices3)
			{
				ref Vector3 reference4 = ref workingVerts[dAZVertexMap3.tovert];
				reference4 = workingVerts[dAZVertexMap3.fromvert];
			}
		}
		threadMainSkinStopTime = (float)stopwatch.ElapsedTicks * f;
		threadMainSkinTime = threadMainSkinStopTime - threadMainSkinStartTime;
	}

	protected void SkinMeshPart(int startIndex, int stopIndex, bool[] onlyVerts = null, bool[] onlyBones = null)
	{
		Vector3 vector14 = default(Vector3);
		Vector3 vector20 = default(Vector3);
		Vector3 vector8 = default(Vector3);
		Vector3 vector17 = default(Vector3);
		Vector3 vector11 = default(Vector3);
		Vector3 vector5 = default(Vector3);
		for (int i = 0; i < _numBones; i++)
		{
			if (onlyBones != null && !onlyBones[i])
			{
				continue;
			}
			DAZSkinV2VertexWeights[] weights = nodes[i].weights;
			int[] fullyWeightedVertices = nodes[i].fullyWeightedVertices;
			float m = boneChangeFromOriginalMatrices[i].m00;
			float m2 = boneChangeFromOriginalMatrices[i].m01;
			float m3 = boneChangeFromOriginalMatrices[i].m02;
			float m4 = boneChangeFromOriginalMatrices[i].m03;
			float m5 = boneChangeFromOriginalMatrices[i].m10;
			float m6 = boneChangeFromOriginalMatrices[i].m11;
			float m7 = boneChangeFromOriginalMatrices[i].m12;
			float m8 = boneChangeFromOriginalMatrices[i].m13;
			float m9 = boneChangeFromOriginalMatrices[i].m20;
			float m10 = boneChangeFromOriginalMatrices[i].m21;
			float m11 = boneChangeFromOriginalMatrices[i].m22;
			float m12 = boneChangeFromOriginalMatrices[i].m23;
			if (_useGeneralWeights)
			{
				DAZSkinV2GeneralVertexWeights[] generalWeights = nodes[i].generalWeights;
				foreach (DAZSkinV2GeneralVertexWeights dAZSkinV2GeneralVertexWeights in generalWeights)
				{
					if (dAZSkinV2GeneralVertexWeights.vertex >= startIndex && dAZSkinV2GeneralVertexWeights.vertex <= stopIndex)
					{
						Vector3 vector = startVertsCopy[dAZSkinV2GeneralVertexWeights.vertex];
						workingVerts[dAZSkinV2GeneralVertexWeights.vertex].x += (vector.x * m + vector.y * m2 + vector.z * m3 + m4) * dAZSkinV2GeneralVertexWeights.weight;
						workingVerts[dAZSkinV2GeneralVertexWeights.vertex].y += (vector.x * m5 + vector.y * m6 + vector.z * m7 + m8) * dAZSkinV2GeneralVertexWeights.weight;
						workingVerts[dAZSkinV2GeneralVertexWeights.vertex].z += (vector.x * m9 + vector.y * m10 + vector.z * m11 + m12) * dAZSkinV2GeneralVertexWeights.weight;
					}
				}
				continue;
			}
			DAZSkinV2BulgeFactors bulgeFactors = nodes[i].bulgeFactors;
			Quaternion2Angles.RotationOrder rotationOrder = nodes[i].rotationOrder;
			Matrix4x4 matrix4x = boneWorldToLocalMatrices[i];
			float m13 = matrix4x.m00;
			float m14 = matrix4x.m01;
			float m15 = matrix4x.m02;
			float m16 = matrix4x.m03;
			float m17 = matrix4x.m10;
			float m18 = matrix4x.m11;
			float m19 = matrix4x.m12;
			float m20 = matrix4x.m13;
			float m21 = matrix4x.m20;
			float m22 = matrix4x.m21;
			float m23 = matrix4x.m22;
			float m24 = matrix4x.m23;
			Matrix4x4 matrix4x2 = boneLocalToWorldMatrices[i];
			float m25 = matrix4x2.m00;
			float m26 = matrix4x2.m01;
			float m27 = matrix4x2.m02;
			float m28 = matrix4x2.m03;
			float m29 = matrix4x2.m10;
			float m30 = matrix4x2.m11;
			float m31 = matrix4x2.m12;
			float m32 = matrix4x2.m13;
			float m33 = matrix4x2.m20;
			float m34 = matrix4x2.m21;
			float m35 = matrix4x2.m22;
			float m36 = matrix4x2.m23;
			Vector3 vector2 = boneRotationAngles[i];
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			bool flag10 = false;
			bool flag11 = false;
			bool flag12 = false;
			bool flag13 = false;
			bool flag14 = false;
			bool flag15 = false;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			float num11 = 0f;
			float num12 = 0f;
			if (vector2.x > 0.01f)
			{
				if (bulgeFactors.xposleft != 0f)
				{
					flag = true;
					flag2 = true;
					num = bulgeFactors.xposleft * vector2.x * bulgeScale;
				}
				if (bulgeFactors.xposright != 0f)
				{
					flag = true;
					flag4 = true;
					num3 = bulgeFactors.xposright * vector2.x * bulgeScale;
				}
			}
			else if (vector2.x < -0.01f)
			{
				if (bulgeFactors.xnegleft != 0f)
				{
					flag = true;
					flag3 = true;
					num2 = bulgeFactors.xnegleft * vector2.x * bulgeScale;
				}
				if (bulgeFactors.xnegright != 0f)
				{
					flag = true;
					flag5 = true;
					num4 = bulgeFactors.xnegright * vector2.x * bulgeScale;
				}
			}
			if (vector2.y > 0.01f)
			{
				if (bulgeFactors.yposleft != 0f)
				{
					flag6 = true;
					flag7 = true;
					num5 = bulgeFactors.yposleft * vector2.y * bulgeScale;
				}
				if (bulgeFactors.yposright != 0f)
				{
					flag6 = true;
					flag9 = true;
					num7 = bulgeFactors.yposright * vector2.y * bulgeScale;
				}
			}
			else if (vector2.y < -0.01f)
			{
				if (bulgeFactors.ynegleft != 0f)
				{
					flag6 = true;
					flag8 = true;
					num6 = bulgeFactors.ynegleft * vector2.y * bulgeScale;
				}
				if (bulgeFactors.ynegright != 0f)
				{
					flag6 = true;
					flag10 = true;
					num8 = bulgeFactors.ynegright * vector2.y * bulgeScale;
				}
			}
			if (vector2.z > 0.01f)
			{
				if (bulgeFactors.zposleft != 0f)
				{
					flag11 = true;
					flag12 = true;
					num9 = bulgeFactors.zposleft * vector2.z * bulgeScale;
				}
				if (bulgeFactors.zposright != 0f)
				{
					flag11 = true;
					flag14 = true;
					num11 = bulgeFactors.zposright * vector2.z * bulgeScale;
				}
			}
			else if (vector2.z < -0.01f)
			{
				if (bulgeFactors.znegleft != 0f)
				{
					flag11 = true;
					flag13 = true;
					num10 = bulgeFactors.znegleft * vector2.z * bulgeScale;
				}
				if (bulgeFactors.znegright != 0f)
				{
					flag11 = true;
					flag15 = true;
					num12 = bulgeFactors.znegright * vector2.z * bulgeScale;
				}
			}
			switch (rotationOrder)
			{
			case Quaternion2Angles.RotationOrder.XYZ:
				foreach (DAZSkinV2VertexWeights dAZSkinV2VertexWeights4 in weights)
				{
					if (dAZSkinV2VertexWeights4.vertex < startIndex || dAZSkinV2VertexWeights4.vertex > stopIndex || (onlyVerts != null && !onlyVerts[dAZSkinV2VertexWeights4.vertex]))
					{
						continue;
					}
					if (dAZSkinV2VertexWeights4.xweight > 0.99999f && dAZSkinV2VertexWeights4.yweight > 0.99999f && dAZSkinV2VertexWeights4.zweight > 0.99999f)
					{
						Vector3 vector12 = workingVerts[dAZSkinV2VertexWeights4.vertex];
						workingVerts[dAZSkinV2VertexWeights4.vertex].x = vector12.x * m + vector12.y * m2 + vector12.z * m3 + m4;
						workingVerts[dAZSkinV2VertexWeights4.vertex].y = vector12.x * m5 + vector12.y * m6 + vector12.z * m7 + m8;
						workingVerts[dAZSkinV2VertexWeights4.vertex].z = vector12.x * m9 + vector12.y * m10 + vector12.z * m11 + m12;
						continue;
					}
					Vector3 vector13 = workingVerts[dAZSkinV2VertexWeights4.vertex];
					vector14.x = vector13.x * m13 + vector13.y * m14 + vector13.z * m15 + m16;
					vector14.y = vector13.x * m17 + vector13.y * m18 + vector13.z * m19 + m20;
					vector14.z = vector13.x * m21 + vector13.y * m22 + vector13.z * m23 + m24;
					if (dAZSkinV2VertexWeights4.zweight > 0f)
					{
						float num77 = vector2.z * dAZSkinV2VertexWeights4.zweight;
						float num78 = (float)Math.Sin(num77);
						float num79 = (float)Math.Cos(num77);
						float x7 = vector14.x * num79 - vector14.y * num78;
						vector14.y = vector14.x * num78 + vector14.y * num79;
						vector14.x = x7;
					}
					if (flag11)
					{
						if (flag12 && dAZSkinV2VertexWeights4.zleftbulge > 0f)
						{
							float num80 = num9 * dAZSkinV2VertexWeights4.zleftbulge;
							vector14.x += vector14.x * num80;
							vector14.y += vector14.y * num80;
						}
						if (flag14 && dAZSkinV2VertexWeights4.zrightbulge > 0f)
						{
							float num81 = num11 * dAZSkinV2VertexWeights4.zrightbulge;
							vector14.x += vector14.x * num81;
							vector14.y += vector14.y * num81;
						}
						if (flag13 && dAZSkinV2VertexWeights4.zleftbulge > 0f)
						{
							float num82 = num10 * dAZSkinV2VertexWeights4.zleftbulge;
							vector14.x += vector14.x * num82;
							vector14.y += vector14.y * num82;
						}
						if (flag15 && dAZSkinV2VertexWeights4.zrightbulge > 0f)
						{
							float num83 = num12 * dAZSkinV2VertexWeights4.zrightbulge;
							vector14.x += vector14.x * num83;
							vector14.y += vector14.y * num83;
						}
					}
					if (dAZSkinV2VertexWeights4.yweight > 0f)
					{
						float num84 = vector2.y * dAZSkinV2VertexWeights4.yweight;
						float num85 = (float)Math.Sin(num84);
						float num86 = (float)Math.Cos(num84);
						float x8 = vector14.x * num86 + vector14.z * num85;
						vector14.z = vector14.z * num86 - vector14.x * num85;
						vector14.x = x8;
					}
					if (flag6)
					{
						if (flag7 && dAZSkinV2VertexWeights4.yleftbulge > 0f)
						{
							float num87 = num5 * dAZSkinV2VertexWeights4.yleftbulge;
							vector14.x += vector14.x * num87;
							vector14.z += vector14.z * num87;
						}
						if (flag9 && dAZSkinV2VertexWeights4.yrightbulge > 0f)
						{
							float num88 = num7 * dAZSkinV2VertexWeights4.yrightbulge;
							vector14.x += vector14.x * num88;
							vector14.z += vector14.z * num88;
						}
						if (flag8 && dAZSkinV2VertexWeights4.yleftbulge > 0f)
						{
							float num89 = num6 * dAZSkinV2VertexWeights4.yleftbulge;
							vector14.x += vector14.x * num89;
							vector14.z += vector14.z * num89;
						}
						if (flag10 && dAZSkinV2VertexWeights4.yrightbulge > 0f)
						{
							float num90 = num8 * dAZSkinV2VertexWeights4.yrightbulge;
							vector14.x += vector14.x * num90;
							vector14.z += vector14.z * num90;
						}
					}
					if (dAZSkinV2VertexWeights4.xweight > 0f)
					{
						float num91 = vector2.x * dAZSkinV2VertexWeights4.xweight;
						float num92 = (float)Math.Sin(num91);
						float num93 = (float)Math.Cos(num91);
						float y4 = vector14.y * num93 - vector14.z * num92;
						vector14.z = vector14.y * num92 + vector14.z * num93;
						vector14.y = y4;
					}
					if (flag)
					{
						if (flag2 && dAZSkinV2VertexWeights4.xleftbulge > 0f)
						{
							float num94 = num * dAZSkinV2VertexWeights4.xleftbulge;
							vector14.y += vector14.y * num94;
							vector14.z += vector14.z * num94;
						}
						if (flag4 && dAZSkinV2VertexWeights4.xrightbulge > 0f)
						{
							float num95 = num3 * dAZSkinV2VertexWeights4.xrightbulge;
							vector14.y += vector14.y * num95;
							vector14.z += vector14.z * num95;
						}
						if (flag3 && dAZSkinV2VertexWeights4.xleftbulge > 0f)
						{
							float num96 = num2 * dAZSkinV2VertexWeights4.xleftbulge;
							vector14.y += vector14.y * num96;
							vector14.z += vector14.z * num96;
						}
						if (flag5 && dAZSkinV2VertexWeights4.xrightbulge > 0f)
						{
							float num97 = num4 * dAZSkinV2VertexWeights4.xrightbulge;
							vector14.y += vector14.y * num97;
							vector14.z += vector14.z * num97;
						}
					}
					workingVerts[dAZSkinV2VertexWeights4.vertex].x = vector14.x * m25 + vector14.y * m26 + vector14.z * m27 + m28;
					workingVerts[dAZSkinV2VertexWeights4.vertex].y = vector14.x * m29 + vector14.y * m30 + vector14.z * m31 + m32;
					workingVerts[dAZSkinV2VertexWeights4.vertex].z = vector14.x * m33 + vector14.y * m34 + vector14.z * m35 + m36;
				}
				break;
			case Quaternion2Angles.RotationOrder.XZY:
				foreach (DAZSkinV2VertexWeights dAZSkinV2VertexWeights6 in weights)
				{
					if (dAZSkinV2VertexWeights6.vertex < startIndex || dAZSkinV2VertexWeights6.vertex > stopIndex || (onlyVerts != null && !onlyVerts[dAZSkinV2VertexWeights6.vertex]))
					{
						continue;
					}
					if (dAZSkinV2VertexWeights6.xweight > 0.99999f && dAZSkinV2VertexWeights6.yweight > 0.99999f && dAZSkinV2VertexWeights6.zweight > 0.99999f)
					{
						Vector3 vector18 = workingVerts[dAZSkinV2VertexWeights6.vertex];
						workingVerts[dAZSkinV2VertexWeights6.vertex].x = vector18.x * m + vector18.y * m2 + vector18.z * m3 + m4;
						workingVerts[dAZSkinV2VertexWeights6.vertex].y = vector18.x * m5 + vector18.y * m6 + vector18.z * m7 + m8;
						workingVerts[dAZSkinV2VertexWeights6.vertex].z = vector18.x * m9 + vector18.y * m10 + vector18.z * m11 + m12;
						continue;
					}
					Vector3 vector19 = workingVerts[dAZSkinV2VertexWeights6.vertex];
					vector20.x = vector19.x * m13 + vector19.y * m14 + vector19.z * m15 + m16;
					vector20.y = vector19.x * m17 + vector19.y * m18 + vector19.z * m19 + m20;
					vector20.z = vector19.x * m21 + vector19.y * m22 + vector19.z * m23 + m24;
					if (dAZSkinV2VertexWeights6.yweight > 0f)
					{
						float num121 = vector2.y * dAZSkinV2VertexWeights6.yweight;
						float num122 = (float)Math.Sin(num121);
						float num123 = (float)Math.Cos(num121);
						float x11 = vector20.x * num123 + vector20.z * num122;
						vector20.z = vector20.z * num123 - vector20.x * num122;
						vector20.x = x11;
					}
					if (flag6)
					{
						if (flag7 && dAZSkinV2VertexWeights6.yleftbulge > 0f)
						{
							float num124 = num5 * dAZSkinV2VertexWeights6.yleftbulge;
							vector20.x += vector20.x * num124;
							vector20.z += vector20.z * num124;
						}
						if (flag9 && dAZSkinV2VertexWeights6.yrightbulge > 0f)
						{
							float num125 = num7 * dAZSkinV2VertexWeights6.yrightbulge;
							vector20.x += vector20.x * num125;
							vector20.z += vector20.z * num125;
						}
						if (flag8 && dAZSkinV2VertexWeights6.yleftbulge > 0f)
						{
							float num126 = num6 * dAZSkinV2VertexWeights6.yleftbulge;
							vector20.x += vector20.x * num126;
							vector20.z += vector20.z * num126;
						}
						if (flag10 && dAZSkinV2VertexWeights6.yrightbulge > 0f)
						{
							float num127 = num8 * dAZSkinV2VertexWeights6.yrightbulge;
							vector20.x += vector20.x * num127;
							vector20.z += vector20.z * num127;
						}
					}
					if (dAZSkinV2VertexWeights6.zweight > 0f)
					{
						float num128 = vector2.z * dAZSkinV2VertexWeights6.zweight;
						float num129 = (float)Math.Sin(num128);
						float num130 = (float)Math.Cos(num128);
						float x12 = vector20.x * num130 - vector20.y * num129;
						vector20.y = vector20.x * num129 + vector20.y * num130;
						vector20.x = x12;
					}
					if (flag11)
					{
						if (flag12 && dAZSkinV2VertexWeights6.zleftbulge > 0f)
						{
							float num131 = num9 * dAZSkinV2VertexWeights6.zleftbulge;
							vector20.x += vector20.x * num131;
							vector20.y += vector20.y * num131;
						}
						if (flag14 && dAZSkinV2VertexWeights6.zrightbulge > 0f)
						{
							float num132 = num11 * dAZSkinV2VertexWeights6.zrightbulge;
							vector20.x += vector20.x * num132;
							vector20.y += vector20.y * num132;
						}
						if (flag13 && dAZSkinV2VertexWeights6.zleftbulge > 0f)
						{
							float num133 = num10 * dAZSkinV2VertexWeights6.zleftbulge;
							vector20.x += vector20.x * num133;
							vector20.y += vector20.y * num133;
						}
						if (flag15 && dAZSkinV2VertexWeights6.zrightbulge > 0f)
						{
							float num134 = num12 * dAZSkinV2VertexWeights6.zrightbulge;
							vector20.x += vector20.x * num134;
							vector20.y += vector20.y * num134;
						}
					}
					if (dAZSkinV2VertexWeights6.xweight > 0f)
					{
						float num135 = vector2.x * dAZSkinV2VertexWeights6.xweight;
						float num136 = (float)Math.Sin(num135);
						float num137 = (float)Math.Cos(num135);
						float y6 = vector20.y * num137 - vector20.z * num136;
						vector20.z = vector20.y * num136 + vector20.z * num137;
						vector20.y = y6;
					}
					if (flag)
					{
						if (flag2 && dAZSkinV2VertexWeights6.xleftbulge > 0f)
						{
							float num138 = num * dAZSkinV2VertexWeights6.xleftbulge;
							vector20.y += vector20.y * num138;
							vector20.z += vector20.z * num138;
						}
						if (flag4 && dAZSkinV2VertexWeights6.xrightbulge > 0f)
						{
							float num139 = num3 * dAZSkinV2VertexWeights6.xrightbulge;
							vector20.y += vector20.y * num139;
							vector20.z += vector20.z * num139;
						}
						if (flag3 && dAZSkinV2VertexWeights6.xleftbulge > 0f)
						{
							float num140 = num2 * dAZSkinV2VertexWeights6.xleftbulge;
							vector20.y += vector20.y * num140;
							vector20.z += vector20.z * num140;
						}
						if (flag5 && dAZSkinV2VertexWeights6.xrightbulge > 0f)
						{
							float num141 = num4 * dAZSkinV2VertexWeights6.xrightbulge;
							vector20.y += vector20.y * num141;
							vector20.z += vector20.z * num141;
						}
					}
					workingVerts[dAZSkinV2VertexWeights6.vertex].x = vector20.x * m25 + vector20.y * m26 + vector20.z * m27 + m28;
					workingVerts[dAZSkinV2VertexWeights6.vertex].y = vector20.x * m29 + vector20.y * m30 + vector20.z * m31 + m32;
					workingVerts[dAZSkinV2VertexWeights6.vertex].z = vector20.x * m33 + vector20.y * m34 + vector20.z * m35 + m36;
				}
				break;
			case Quaternion2Angles.RotationOrder.YXZ:
				foreach (DAZSkinV2VertexWeights dAZSkinV2VertexWeights2 in weights)
				{
					if (dAZSkinV2VertexWeights2.vertex < startIndex || dAZSkinV2VertexWeights2.vertex > stopIndex || (onlyVerts != null && !onlyVerts[dAZSkinV2VertexWeights2.vertex]))
					{
						continue;
					}
					if (dAZSkinV2VertexWeights2.xweight > 0.99999f && dAZSkinV2VertexWeights2.yweight > 0.99999f && dAZSkinV2VertexWeights2.zweight > 0.99999f)
					{
						Vector3 vector6 = workingVerts[dAZSkinV2VertexWeights2.vertex];
						workingVerts[dAZSkinV2VertexWeights2.vertex].x = vector6.x * m + vector6.y * m2 + vector6.z * m3 + m4;
						workingVerts[dAZSkinV2VertexWeights2.vertex].y = vector6.x * m5 + vector6.y * m6 + vector6.z * m7 + m8;
						workingVerts[dAZSkinV2VertexWeights2.vertex].z = vector6.x * m9 + vector6.y * m10 + vector6.z * m11 + m12;
						continue;
					}
					Vector3 vector7 = workingVerts[dAZSkinV2VertexWeights2.vertex];
					vector8.x = vector7.x * m13 + vector7.y * m14 + vector7.z * m15 + m16;
					vector8.y = vector7.x * m17 + vector7.y * m18 + vector7.z * m19 + m20;
					vector8.z = vector7.x * m21 + vector7.y * m22 + vector7.z * m23 + m24;
					if (dAZSkinV2VertexWeights2.zweight > 0f)
					{
						float num34 = vector2.z * dAZSkinV2VertexWeights2.zweight;
						float num35 = (float)Math.Sin(num34);
						float num36 = (float)Math.Cos(num34);
						float x3 = vector8.x * num36 - vector8.y * num35;
						vector8.y = vector8.x * num35 + vector8.y * num36;
						vector8.x = x3;
					}
					if (flag11)
					{
						if (flag12 && dAZSkinV2VertexWeights2.zleftbulge > 0f)
						{
							float num37 = num9 * dAZSkinV2VertexWeights2.zleftbulge;
							vector8.x += vector8.x * num37;
							vector8.y += vector8.y * num37;
						}
						if (flag14 && dAZSkinV2VertexWeights2.zrightbulge > 0f)
						{
							float num38 = num11 * dAZSkinV2VertexWeights2.zrightbulge;
							vector8.x += vector8.x * num38;
							vector8.y += vector8.y * num38;
						}
						if (flag13 && dAZSkinV2VertexWeights2.zleftbulge > 0f)
						{
							float num39 = num10 * dAZSkinV2VertexWeights2.zleftbulge;
							vector8.x += vector8.x * num39;
							vector8.y += vector8.y * num39;
						}
						if (flag15 && dAZSkinV2VertexWeights2.zrightbulge > 0f)
						{
							float num40 = num12 * dAZSkinV2VertexWeights2.zrightbulge;
							vector8.x += vector8.x * num40;
							vector8.y += vector8.y * num40;
						}
					}
					if (dAZSkinV2VertexWeights2.xweight > 0f)
					{
						float num41 = vector2.x * dAZSkinV2VertexWeights2.xweight;
						float num42 = (float)Math.Sin(num41);
						float num43 = (float)Math.Cos(num41);
						float y2 = vector8.y * num43 - vector8.z * num42;
						vector8.z = vector8.y * num42 + vector8.z * num43;
						vector8.y = y2;
					}
					if (flag)
					{
						if (flag2 && dAZSkinV2VertexWeights2.xleftbulge > 0f)
						{
							float num44 = num * dAZSkinV2VertexWeights2.xleftbulge;
							vector8.y += vector8.y * num44;
							vector8.z += vector8.z * num44;
						}
						if (flag4 && dAZSkinV2VertexWeights2.xrightbulge > 0f)
						{
							float num45 = num3 * dAZSkinV2VertexWeights2.xrightbulge;
							vector8.y += vector8.y * num45;
							vector8.z += vector8.z * num45;
						}
						if (flag3 && dAZSkinV2VertexWeights2.xleftbulge > 0f)
						{
							float num46 = num2 * dAZSkinV2VertexWeights2.xleftbulge;
							vector8.y += vector8.y * num46;
							vector8.z += vector8.z * num46;
						}
						if (flag5 && dAZSkinV2VertexWeights2.xrightbulge > 0f)
						{
							float num47 = num4 * dAZSkinV2VertexWeights2.xrightbulge;
							vector8.y += vector8.y * num47;
							vector8.z += vector8.z * num47;
						}
					}
					if (dAZSkinV2VertexWeights2.yweight > 0f)
					{
						float num48 = vector2.y * dAZSkinV2VertexWeights2.yweight;
						float num49 = (float)Math.Sin(num48);
						float num50 = (float)Math.Cos(num48);
						float x4 = vector8.x * num50 + vector8.z * num49;
						vector8.z = vector8.z * num50 - vector8.x * num49;
						vector8.x = x4;
					}
					if (flag6)
					{
						if (flag7 && dAZSkinV2VertexWeights2.yleftbulge > 0f)
						{
							float num51 = num5 * dAZSkinV2VertexWeights2.yleftbulge;
							vector8.x += vector8.x * num51;
							vector8.z += vector8.z * num51;
						}
						if (flag9 && dAZSkinV2VertexWeights2.yrightbulge > 0f)
						{
							float num52 = num7 * dAZSkinV2VertexWeights2.yrightbulge;
							vector8.x += vector8.x * num52;
							vector8.z += vector8.z * num52;
						}
						if (flag8 && dAZSkinV2VertexWeights2.yleftbulge > 0f)
						{
							float num53 = num6 * dAZSkinV2VertexWeights2.yleftbulge;
							vector8.x += vector8.x * num53;
							vector8.z += vector8.z * num53;
						}
						if (flag10 && dAZSkinV2VertexWeights2.yrightbulge > 0f)
						{
							float num54 = num8 * dAZSkinV2VertexWeights2.yrightbulge;
							vector8.x += vector8.x * num54;
							vector8.z += vector8.z * num54;
						}
					}
					workingVerts[dAZSkinV2VertexWeights2.vertex].x = vector8.x * m25 + vector8.y * m26 + vector8.z * m27 + m28;
					workingVerts[dAZSkinV2VertexWeights2.vertex].y = vector8.x * m29 + vector8.y * m30 + vector8.z * m31 + m32;
					workingVerts[dAZSkinV2VertexWeights2.vertex].z = vector8.x * m33 + vector8.y * m34 + vector8.z * m35 + m36;
				}
				break;
			case Quaternion2Angles.RotationOrder.YZX:
				foreach (DAZSkinV2VertexWeights dAZSkinV2VertexWeights5 in weights)
				{
					if (dAZSkinV2VertexWeights5.vertex < startIndex || dAZSkinV2VertexWeights5.vertex > stopIndex || (onlyVerts != null && !onlyVerts[dAZSkinV2VertexWeights5.vertex]))
					{
						continue;
					}
					if (dAZSkinV2VertexWeights5.xweight > 0.99999f && dAZSkinV2VertexWeights5.yweight > 0.99999f && dAZSkinV2VertexWeights5.zweight > 0.99999f)
					{
						Vector3 vector15 = workingVerts[dAZSkinV2VertexWeights5.vertex];
						workingVerts[dAZSkinV2VertexWeights5.vertex].x = vector15.x * m + vector15.y * m2 + vector15.z * m3 + m4;
						workingVerts[dAZSkinV2VertexWeights5.vertex].y = vector15.x * m5 + vector15.y * m6 + vector15.z * m7 + m8;
						workingVerts[dAZSkinV2VertexWeights5.vertex].z = vector15.x * m9 + vector15.y * m10 + vector15.z * m11 + m12;
						continue;
					}
					Vector3 vector16 = workingVerts[dAZSkinV2VertexWeights5.vertex];
					vector17.x = vector16.x * m13 + vector16.y * m14 + vector16.z * m15 + m16;
					vector17.y = vector16.x * m17 + vector16.y * m18 + vector16.z * m19 + m20;
					vector17.z = vector16.x * m21 + vector16.y * m22 + vector16.z * m23 + m24;
					if (dAZSkinV2VertexWeights5.xweight > 0f)
					{
						float num99 = vector2.x * dAZSkinV2VertexWeights5.xweight;
						float num100 = (float)Math.Sin(num99);
						float num101 = (float)Math.Cos(num99);
						float y5 = vector17.y * num101 - vector17.z * num100;
						vector17.z = vector17.y * num100 + vector17.z * num101;
						vector17.y = y5;
					}
					if (flag)
					{
						if (flag2 && dAZSkinV2VertexWeights5.xleftbulge > 0f)
						{
							float num102 = num * dAZSkinV2VertexWeights5.xleftbulge;
							vector17.y += vector17.y * num102;
							vector17.z += vector17.z * num102;
						}
						if (flag4 && dAZSkinV2VertexWeights5.xrightbulge > 0f)
						{
							float num103 = num3 * dAZSkinV2VertexWeights5.xrightbulge;
							vector17.y += vector17.y * num103;
							vector17.z += vector17.z * num103;
						}
						if (flag3 && dAZSkinV2VertexWeights5.xleftbulge > 0f)
						{
							float num104 = num2 * dAZSkinV2VertexWeights5.xleftbulge;
							vector17.y += vector17.y * num104;
							vector17.z += vector17.z * num104;
						}
						if (flag5 && dAZSkinV2VertexWeights5.xrightbulge > 0f)
						{
							float num105 = num4 * dAZSkinV2VertexWeights5.xrightbulge;
							vector17.y += vector17.y * num105;
							vector17.z += vector17.z * num105;
						}
					}
					if (dAZSkinV2VertexWeights5.zweight > 0f)
					{
						float num106 = vector2.z * dAZSkinV2VertexWeights5.zweight;
						float num107 = (float)Math.Sin(num106);
						float num108 = (float)Math.Cos(num106);
						float x9 = vector17.x * num108 - vector17.y * num107;
						vector17.y = vector17.x * num107 + vector17.y * num108;
						vector17.x = x9;
					}
					if (flag11)
					{
						if (flag12 && dAZSkinV2VertexWeights5.zleftbulge > 0f)
						{
							float num109 = num9 * dAZSkinV2VertexWeights5.zleftbulge;
							vector17.x += vector17.x * num109;
							vector17.y += vector17.y * num109;
						}
						if (flag14 && dAZSkinV2VertexWeights5.zrightbulge > 0f)
						{
							float num110 = num11 * dAZSkinV2VertexWeights5.zrightbulge;
							vector17.x += vector17.x * num110;
							vector17.y += vector17.y * num110;
						}
						if (flag13 && dAZSkinV2VertexWeights5.zleftbulge > 0f)
						{
							float num111 = num10 * dAZSkinV2VertexWeights5.zleftbulge;
							vector17.x += vector17.x * num111;
							vector17.y += vector17.y * num111;
						}
						if (flag15 && dAZSkinV2VertexWeights5.zrightbulge > 0f)
						{
							float num112 = num12 * dAZSkinV2VertexWeights5.zrightbulge;
							vector17.x += vector17.x * num112;
							vector17.y += vector17.y * num112;
						}
					}
					if (dAZSkinV2VertexWeights5.yweight > 0f)
					{
						float num113 = vector2.y * dAZSkinV2VertexWeights5.yweight;
						float num114 = (float)Math.Sin(num113);
						float num115 = (float)Math.Cos(num113);
						float x10 = vector17.x * num115 + vector17.z * num114;
						vector17.z = vector17.z * num115 - vector17.x * num114;
						vector17.x = x10;
					}
					if (flag6)
					{
						if (flag7 && dAZSkinV2VertexWeights5.yleftbulge > 0f)
						{
							float num116 = num5 * dAZSkinV2VertexWeights5.yleftbulge;
							vector17.x += vector17.x * num116;
							vector17.z += vector17.z * num116;
						}
						if (flag9 && dAZSkinV2VertexWeights5.yrightbulge > 0f)
						{
							float num117 = num7 * dAZSkinV2VertexWeights5.yrightbulge;
							vector17.x += vector17.x * num117;
							vector17.z += vector17.z * num117;
						}
						if (flag8 && dAZSkinV2VertexWeights5.yleftbulge > 0f)
						{
							float num118 = num6 * dAZSkinV2VertexWeights5.yleftbulge;
							vector17.x += vector17.x * num118;
							vector17.z += vector17.z * num118;
						}
						if (flag10 && dAZSkinV2VertexWeights5.yrightbulge > 0f)
						{
							float num119 = num8 * dAZSkinV2VertexWeights5.yrightbulge;
							vector17.x += vector17.x * num119;
							vector17.z += vector17.z * num119;
						}
					}
					workingVerts[dAZSkinV2VertexWeights5.vertex].x = vector17.x * m25 + vector17.y * m26 + vector17.z * m27 + m28;
					workingVerts[dAZSkinV2VertexWeights5.vertex].y = vector17.x * m29 + vector17.y * m30 + vector17.z * m31 + m32;
					workingVerts[dAZSkinV2VertexWeights5.vertex].z = vector17.x * m33 + vector17.y * m34 + vector17.z * m35 + m36;
				}
				break;
			case Quaternion2Angles.RotationOrder.ZXY:
				foreach (DAZSkinV2VertexWeights dAZSkinV2VertexWeights3 in weights)
				{
					if (dAZSkinV2VertexWeights3.vertex < startIndex || dAZSkinV2VertexWeights3.vertex > stopIndex || (onlyVerts != null && !onlyVerts[dAZSkinV2VertexWeights3.vertex]))
					{
						continue;
					}
					if (dAZSkinV2VertexWeights3.xweight > 0.99999f && dAZSkinV2VertexWeights3.yweight > 0.99999f && dAZSkinV2VertexWeights3.zweight > 0.99999f)
					{
						Vector3 vector9 = workingVerts[dAZSkinV2VertexWeights3.vertex];
						workingVerts[dAZSkinV2VertexWeights3.vertex].x = vector9.x * m + vector9.y * m2 + vector9.z * m3 + m4;
						workingVerts[dAZSkinV2VertexWeights3.vertex].y = vector9.x * m5 + vector9.y * m6 + vector9.z * m7 + m8;
						workingVerts[dAZSkinV2VertexWeights3.vertex].z = vector9.x * m9 + vector9.y * m10 + vector9.z * m11 + m12;
						continue;
					}
					Vector3 vector10 = workingVerts[dAZSkinV2VertexWeights3.vertex];
					vector11.x = vector10.x * m13 + vector10.y * m14 + vector10.z * m15 + m16;
					vector11.y = vector10.x * m17 + vector10.y * m18 + vector10.z * m19 + m20;
					vector11.z = vector10.x * m21 + vector10.y * m22 + vector10.z * m23 + m24;
					if (dAZSkinV2VertexWeights3.yweight > 0f)
					{
						float num55 = vector2.y * dAZSkinV2VertexWeights3.yweight;
						float num56 = (float)Math.Sin(num55);
						float num57 = (float)Math.Cos(num55);
						float x5 = vector11.x * num57 + vector11.z * num56;
						vector11.z = vector11.z * num57 - vector11.x * num56;
						vector11.x = x5;
					}
					if (flag6)
					{
						if (flag7 && dAZSkinV2VertexWeights3.yleftbulge > 0f)
						{
							float num58 = num5 * dAZSkinV2VertexWeights3.yleftbulge;
							vector11.x += vector11.x * num58;
							vector11.z += vector11.z * num58;
						}
						if (flag9 && dAZSkinV2VertexWeights3.yrightbulge > 0f)
						{
							float num59 = num7 * dAZSkinV2VertexWeights3.yrightbulge;
							vector11.x += vector11.x * num59;
							vector11.z += vector11.z * num59;
						}
						if (flag8 && dAZSkinV2VertexWeights3.yleftbulge > 0f)
						{
							float num60 = num6 * dAZSkinV2VertexWeights3.yleftbulge;
							vector11.x += vector11.x * num60;
							vector11.z += vector11.z * num60;
						}
						if (flag10 && dAZSkinV2VertexWeights3.yrightbulge > 0f)
						{
							float num61 = num8 * dAZSkinV2VertexWeights3.yrightbulge;
							vector11.x += vector11.x * num61;
							vector11.z += vector11.z * num61;
						}
					}
					if (dAZSkinV2VertexWeights3.xweight > 0f)
					{
						float num62 = vector2.x * dAZSkinV2VertexWeights3.xweight;
						float num63 = (float)Math.Sin(num62);
						float num64 = (float)Math.Cos(num62);
						float y3 = vector11.y * num64 - vector11.z * num63;
						vector11.z = vector11.y * num63 + vector11.z * num64;
						vector11.y = y3;
					}
					if (flag)
					{
						if (flag2 && dAZSkinV2VertexWeights3.xleftbulge > 0f)
						{
							float num65 = num * dAZSkinV2VertexWeights3.xleftbulge;
							vector11.y += vector11.y * num65;
							vector11.z += vector11.z * num65;
						}
						if (flag4 && dAZSkinV2VertexWeights3.xrightbulge > 0f)
						{
							float num66 = num3 * dAZSkinV2VertexWeights3.xrightbulge;
							vector11.y += vector11.y * num66;
							vector11.z += vector11.z * num66;
						}
						if (flag3 && dAZSkinV2VertexWeights3.xleftbulge > 0f)
						{
							float num67 = num2 * dAZSkinV2VertexWeights3.xleftbulge;
							vector11.y += vector11.y * num67;
							vector11.z += vector11.z * num67;
						}
						if (flag5 && dAZSkinV2VertexWeights3.xrightbulge > 0f)
						{
							float num68 = num4 * dAZSkinV2VertexWeights3.xrightbulge;
							vector11.y += vector11.y * num68;
							vector11.z += vector11.z * num68;
						}
					}
					if (dAZSkinV2VertexWeights3.zweight > 0f)
					{
						float num69 = vector2.z * dAZSkinV2VertexWeights3.zweight;
						float num70 = (float)Math.Sin(num69);
						float num71 = (float)Math.Cos(num69);
						float x6 = vector11.x * num71 - vector11.y * num70;
						vector11.y = vector11.x * num70 + vector11.y * num71;
						vector11.x = x6;
					}
					if (flag11)
					{
						if (flag12 && dAZSkinV2VertexWeights3.zleftbulge > 0f)
						{
							float num72 = num9 * dAZSkinV2VertexWeights3.zleftbulge;
							vector11.x += vector11.x * num72;
							vector11.y += vector11.y * num72;
						}
						if (flag14 && dAZSkinV2VertexWeights3.zrightbulge > 0f)
						{
							float num73 = num11 * dAZSkinV2VertexWeights3.zrightbulge;
							vector11.x += vector11.x * num73;
							vector11.y += vector11.y * num73;
						}
						if (flag13 && dAZSkinV2VertexWeights3.zleftbulge > 0f)
						{
							float num74 = num10 * dAZSkinV2VertexWeights3.zleftbulge;
							vector11.x += vector11.x * num74;
							vector11.y += vector11.y * num74;
						}
						if (flag15 && dAZSkinV2VertexWeights3.zrightbulge > 0f)
						{
							float num75 = num12 * dAZSkinV2VertexWeights3.zrightbulge;
							vector11.x += vector11.x * num75;
							vector11.y += vector11.y * num75;
						}
					}
					workingVerts[dAZSkinV2VertexWeights3.vertex].x = vector11.x * m25 + vector11.y * m26 + vector11.z * m27 + m28;
					workingVerts[dAZSkinV2VertexWeights3.vertex].y = vector11.x * m29 + vector11.y * m30 + vector11.z * m31 + m32;
					workingVerts[dAZSkinV2VertexWeights3.vertex].z = vector11.x * m33 + vector11.y * m34 + vector11.z * m35 + m36;
				}
				break;
			case Quaternion2Angles.RotationOrder.ZYX:
				foreach (DAZSkinV2VertexWeights dAZSkinV2VertexWeights in weights)
				{
					if (dAZSkinV2VertexWeights.vertex < startIndex || dAZSkinV2VertexWeights.vertex > stopIndex || (onlyVerts != null && !onlyVerts[dAZSkinV2VertexWeights.vertex]))
					{
						continue;
					}
					if (dAZSkinV2VertexWeights.xweight > 0.99999f && dAZSkinV2VertexWeights.yweight > 0.99999f && dAZSkinV2VertexWeights.zweight > 0.99999f)
					{
						Vector3 vector3 = workingVerts[dAZSkinV2VertexWeights.vertex];
						workingVerts[dAZSkinV2VertexWeights.vertex].x = vector3.x * m + vector3.y * m2 + vector3.z * m3 + m4;
						workingVerts[dAZSkinV2VertexWeights.vertex].y = vector3.x * m5 + vector3.y * m6 + vector3.z * m7 + m8;
						workingVerts[dAZSkinV2VertexWeights.vertex].z = vector3.x * m9 + vector3.y * m10 + vector3.z * m11 + m12;
						continue;
					}
					Vector3 vector4 = workingVerts[dAZSkinV2VertexWeights.vertex];
					vector5.x = vector4.x * m13 + vector4.y * m14 + vector4.z * m15 + m16;
					vector5.y = vector4.x * m17 + vector4.y * m18 + vector4.z * m19 + m20;
					vector5.z = vector4.x * m21 + vector4.y * m22 + vector4.z * m23 + m24;
					if (dAZSkinV2VertexWeights.xweight > 0f)
					{
						float num13 = vector2.x * dAZSkinV2VertexWeights.xweight;
						float num14 = (float)Math.Sin(num13);
						float num15 = (float)Math.Cos(num13);
						float y = vector5.y * num15 - vector5.z * num14;
						vector5.z = vector5.y * num14 + vector5.z * num15;
						vector5.y = y;
					}
					if (flag)
					{
						if (flag2 && dAZSkinV2VertexWeights.xleftbulge > 0f)
						{
							float num16 = num * dAZSkinV2VertexWeights.xleftbulge;
							vector5.y += vector5.y * num16;
							vector5.z += vector5.z * num16;
						}
						if (flag4 && dAZSkinV2VertexWeights.xrightbulge > 0f)
						{
							float num17 = num3 * dAZSkinV2VertexWeights.xrightbulge;
							vector5.y += vector5.y * num17;
							vector5.z += vector5.z * num17;
						}
						if (flag3 && dAZSkinV2VertexWeights.xleftbulge > 0f)
						{
							float num18 = num2 * dAZSkinV2VertexWeights.xleftbulge;
							vector5.y += vector5.y * num18;
							vector5.z += vector5.z * num18;
						}
						if (flag5 && dAZSkinV2VertexWeights.xrightbulge > 0f)
						{
							float num19 = num4 * dAZSkinV2VertexWeights.xrightbulge;
							vector5.y += vector5.y * num19;
							vector5.z += vector5.z * num19;
						}
					}
					if (dAZSkinV2VertexWeights.yweight > 0f)
					{
						float num20 = vector2.y * dAZSkinV2VertexWeights.yweight;
						float num21 = (float)Math.Sin(num20);
						float num22 = (float)Math.Cos(num20);
						float x = vector5.x * num22 + vector5.z * num21;
						vector5.z = vector5.z * num22 - vector5.x * num21;
						vector5.x = x;
					}
					if (flag6)
					{
						if (flag7 && dAZSkinV2VertexWeights.yleftbulge > 0f)
						{
							float num23 = num5 * dAZSkinV2VertexWeights.yleftbulge;
							vector5.x += vector5.x * num23;
							vector5.z += vector5.z * num23;
						}
						if (flag9 && dAZSkinV2VertexWeights.yrightbulge > 0f)
						{
							float num24 = num7 * dAZSkinV2VertexWeights.yrightbulge;
							vector5.x += vector5.x * num24;
							vector5.z += vector5.z * num24;
						}
						if (flag8 && dAZSkinV2VertexWeights.yleftbulge > 0f)
						{
							float num25 = num6 * dAZSkinV2VertexWeights.yleftbulge;
							vector5.x += vector5.x * num25;
							vector5.z += vector5.z * num25;
						}
						if (flag10 && dAZSkinV2VertexWeights.yrightbulge > 0f)
						{
							float num26 = num8 * dAZSkinV2VertexWeights.yrightbulge;
							vector5.x += vector5.x * num26;
							vector5.z += vector5.z * num26;
						}
					}
					if (dAZSkinV2VertexWeights.zweight > 0f)
					{
						float num27 = vector2.z * dAZSkinV2VertexWeights.zweight;
						float num28 = (float)Math.Sin(num27);
						float num29 = (float)Math.Cos(num27);
						float x2 = vector5.x * num29 - vector5.y * num28;
						vector5.y = vector5.x * num28 + vector5.y * num29;
						vector5.x = x2;
					}
					if (flag11)
					{
						if (flag12 && dAZSkinV2VertexWeights.zleftbulge > 0f)
						{
							float num30 = num9 * dAZSkinV2VertexWeights.zleftbulge;
							vector5.x += vector5.x * num30;
							vector5.y += vector5.y * num30;
						}
						if (flag14 && dAZSkinV2VertexWeights.zrightbulge > 0f)
						{
							float num31 = num11 * dAZSkinV2VertexWeights.zrightbulge;
							vector5.x += vector5.x * num31;
							vector5.y += vector5.y * num31;
						}
						if (flag13 && dAZSkinV2VertexWeights.zleftbulge > 0f)
						{
							float num32 = num10 * dAZSkinV2VertexWeights.zleftbulge;
							vector5.x += vector5.x * num32;
							vector5.y += vector5.y * num32;
						}
						if (flag15 && dAZSkinV2VertexWeights.zrightbulge > 0f)
						{
							float num33 = num12 * dAZSkinV2VertexWeights.zrightbulge;
							vector5.x += vector5.x * num33;
							vector5.y += vector5.y * num33;
						}
					}
					workingVerts[dAZSkinV2VertexWeights.vertex].x = vector5.x * m25 + vector5.y * m26 + vector5.z * m27 + m28;
					workingVerts[dAZSkinV2VertexWeights.vertex].y = vector5.x * m29 + vector5.y * m30 + vector5.z * m31 + m32;
					workingVerts[dAZSkinV2VertexWeights.vertex].z = vector5.x * m33 + vector5.y * m34 + vector5.z * m35 + m36;
				}
				break;
			}
			foreach (int num143 in fullyWeightedVertices)
			{
				if (num143 >= startIndex && num143 <= stopIndex && (onlyVerts == null || onlyVerts[num143]))
				{
					Vector3 vector21 = workingVerts[num143];
					workingVerts[num143].x = vector21.x * m + vector21.y * m2 + vector21.z * m3 + m4;
					workingVerts[num143].y = vector21.x * m5 + vector21.y * m6 + vector21.z * m7 + m8;
					workingVerts[num143].z = vector21.x * m9 + vector21.y * m10 + vector21.z * m11 + m12;
				}
			}
		}
	}

	protected void DrawMesh()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component != null)
		{
			DrawMeshNative();
		}
		else
		{
			if (!(mesh != null))
			{
				return;
			}
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m03 += drawOffset.x;
			identity.m13 += drawOffset.y;
			identity.m23 += drawOffset.z;
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				if (dazMesh.materialsEnabled[i])
				{
					if (dazMesh.useSimpleMaterial && (bool)dazMesh.simpleMaterial)
					{
						Graphics.DrawMesh(mesh, identity, dazMesh.simpleMaterial, 0, null, i, null, materialsShadowCastEnabled[i], dazMesh.receiveShadows);
					}
					else if (dazMesh.materials[i] != null)
					{
						Graphics.DrawMesh(mesh, identity, dazMesh.materials[i], 0, null, i, null, materialsShadowCastEnabled[i], dazMesh.receiveShadows);
					}
				}
			}
		}
	}

	protected void DrawMeshNative()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		MeshRenderer component2 = GetComponent<MeshRenderer>();
		if (!(component != null) || !(component2 != null))
		{
			return;
		}
		component2.enabled = true;
		if (component.sharedMesh != mesh)
		{
			component.sharedMesh = mesh;
		}
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			if (dazMesh.materialsEnabled[i])
			{
				component2.materials[i] = dazMesh.materials[i];
			}
			else
			{
				component2.materials[i] = null;
			}
		}
	}

	protected void DrawMeshGPU()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.enabled = false;
		}
		if (!(mesh != null))
		{
			return;
		}
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m03 += drawOffset.x;
		identity.m13 += drawOffset.y;
		identity.m23 += drawOffset.z;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			if (!materialsEnabled[i])
			{
				continue;
			}
			if (GPUuseSimpleMaterial && (bool)GPUsimpleMaterial)
			{
				if (delayDisplayOneFrame)
				{
					GPUsimpleMaterial.SetBuffer("verts", delayedVertsBuffer);
					if (_normalsBuffer != null)
					{
						GPUsimpleMaterial.SetBuffer("normals", delayedNormalsBuffer);
					}
					if (_tangentsBuffer != null)
					{
						GPUsimpleMaterial.SetBuffer("tangents", delayedTangentsBuffer);
					}
				}
				else
				{
					if (_useSmoothing)
					{
						GPUsimpleMaterial.SetBuffer("verts", smoothedVertsBuffer);
					}
					else
					{
						GPUsimpleMaterial.SetBuffer("verts", rawVertsBuffer);
					}
					if (_normalsBuffer != null)
					{
						GPUsimpleMaterial.SetBuffer("normals", _normalsBuffer);
					}
					if (_tangentsBuffer != null)
					{
						GPUsimpleMaterial.SetBuffer("tangents", _tangentsBuffer);
					}
				}
				Graphics.DrawMesh(mesh, identity, GPUsimpleMaterial, 0, null, i, null, materialsShadowCastEnabled[i], dazMesh.receiveShadows);
			}
			else
			{
				if (!(GPUmaterials[i] != null))
				{
					continue;
				}
				if (delayDisplayOneFrame)
				{
					GPUmaterials[i].SetBuffer("verts", delayedVertsBuffer);
					if (_normalsBuffer != null)
					{
						GPUmaterials[i].SetBuffer("normals", delayedNormalsBuffer);
					}
					if (_tangentsBuffer != null)
					{
						GPUmaterials[i].SetBuffer("tangents", delayedTangentsBuffer);
					}
				}
				else
				{
					if (_useSmoothing)
					{
						GPUmaterials[i].SetBuffer("verts", smoothedVertsBuffer);
					}
					else
					{
						GPUmaterials[i].SetBuffer("verts", rawVertsBuffer);
					}
					if (_normalsBuffer != null)
					{
						GPUmaterials[i].SetBuffer("normals", _normalsBuffer);
					}
					if (_tangentsBuffer != null)
					{
						GPUmaterials[i].SetBuffer("tangents", _tangentsBuffer);
					}
				}
				Graphics.DrawMesh(mesh, identity, GPUmaterials[i], 0, null, i, null, materialsShadowCastEnabled[i], dazMesh.receiveShadows);
			}
		}
	}

	private void InitCollider(bool convex = false)
	{
		if (root != null)
		{
			meshCollider = root.gameObject.GetComponent<MeshCollider>();
			if (meshCollider == null)
			{
				meshCollider = root.gameObject.AddComponent<MeshCollider>();
			}
			if (dazMesh != null)
			{
				meshCollider.sharedMesh = dazMesh.morphedUVMappedMesh;
			}
			meshCollider.convex = convex;
		}
	}

	private void InitRigidbody(bool kinematic = false)
	{
		if (root != null)
		{
			RB = root.gameObject.GetComponent<Rigidbody>();
			if (RB == null)
			{
				RB = root.gameObject.AddComponent<Rigidbody>();
			}
			RB.mass = mass;
			RB.constraints = RigidbodyConstraints.None;
			RB.isKinematic = kinematic;
		}
	}

	public void InitPhysicsObjects()
	{
		switch (_physicsType)
		{
		case PhysicsType.None:
		{
			Joint component = root.gameObject.GetComponent<Joint>();
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
			RB = root.gameObject.GetComponent<Rigidbody>();
			if (RB != null)
			{
				UnityEngine.Object.DestroyImmediate(RB);
			}
			meshCollider = root.gameObject.GetComponent<MeshCollider>();
			if (meshCollider != null)
			{
				UnityEngine.Object.DestroyImmediate(meshCollider);
			}
			break;
		}
		case PhysicsType.Static:
		{
			InitCollider();
			Joint component = root.gameObject.GetComponent<Joint>();
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
			RB = root.gameObject.GetComponent<Rigidbody>();
			if (RB != null)
			{
				UnityEngine.Object.DestroyImmediate(RB);
			}
			break;
		}
		case PhysicsType.Rigidbody:
			InitCollider(convex: true);
			InitRigidbody();
			break;
		case PhysicsType.KinematicRigidbody:
			InitCollider();
			InitRigidbody(kinematic: true);
			break;
		}
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			SkinMeshGPUMaterialInit();
		}
	}

	public void Init()
	{
		if (!_wasInit && Application.isPlaying && root != null)
		{
			_wasInit = true;
			stopwatch = new Stopwatch();
			f = 1000f / (float)Stopwatch.Frequency;
			stopwatch.Start();
			InitSkinTimes();
			if (useThreadControlNumThreads && ThreadControl.singleton != null)
			{
				ThreadControl singleton = ThreadControl.singleton;
				singleton.onNumSubThreadsChangedHandlers = (ThreadControl.OnNumSubThreadsChanged)Delegate.Combine(singleton.onNumSubThreadsChangedHandlers, new ThreadControl.OnNumSubThreadsChanged(SetNumSubThreads));
				numSubThreads = ThreadControl.singleton.numSubThreads;
			}
			InitBones();
			InitMesh();
			InitPhysicsObjects();
			if (skinMethod == SkinMethod.CPU)
			{
				SkinMesh(forceSynchronous: true);
			}
			else
			{
				SkinMeshCPUandGPU(forceSynchronous: true);
			}
		}
	}

	private void Start()
	{
		Init();
	}

	private void OnDestroy()
	{
		SkinMeshGPUCleanup();
		if (stopwatch != null)
		{
			stopwatch.Stop();
		}
		if (Application.isPlaying)
		{
			StopThreads();
		}
	}

	private void Update()
	{
		Init();
		if (skin && Application.isPlaying && _wasInit && useEarlyFinish && skinMethod == SkinMethod.CPUAndGPU)
		{
			StartThreads();
			SkinMeshCPUandGPUEarlyFinish();
		}
	}

	private void LateUpdate()
	{
		if (skin && Application.isPlaying && _wasInit)
		{
			updateStartTime = (float)stopwatch.ElapsedTicks * f;
			if (skinMethod == SkinMethod.CPU)
			{
				SkinMesh();
			}
			else
			{
				if (delayDisplayOneFrame && GPUSkinner != null && delayedVertsBuffer != null)
				{
					if (_useSmoothing)
					{
						GPUSkinner.SetBuffer(_copyKernel, "inVerts", smoothedVertsBuffer);
					}
					else
					{
						GPUSkinner.SetBuffer(_copyKernel, "inVerts", rawVertsBuffer);
					}
					GPUSkinner.SetBuffer(_copyKernel, "outVerts", delayedVertsBuffer);
					GPUSkinner.Dispatch(_copyKernel, numVertThreadGroups, 1, 1);
					if (_normalsBuffer != null)
					{
						GPUSkinner.SetBuffer(_copyKernel, "inVerts", _normalsBuffer);
						GPUSkinner.SetBuffer(_copyKernel, "outVerts", delayedNormalsBuffer);
						GPUSkinner.Dispatch(_copyKernel, numVertThreadGroups, 1, 1);
					}
					if (_tangentsBuffer != null)
					{
						GPUMeshCompute.SetBuffer(_copyTangentsKernel, "inTangents", _tangentsBuffer);
						GPUMeshCompute.SetBuffer(_copyTangentsKernel, "outTangents", delayedTangentsBuffer);
						GPUMeshCompute.Dispatch(_copyTangentsKernel, numVertThreadGroups, 1, 1);
					}
				}
				if (skinMethod == SkinMethod.CPUAndGPU)
				{
					StartThreads();
					SkinMeshCPUandGPUEarlyFinish();
					SkinMeshCPUandGPU();
				}
				else
				{
					SkinMeshGPU();
				}
			}
			if (draw)
			{
				if (skinMethod == SkinMethod.CPU)
				{
					DrawMesh();
				}
				else
				{
					DrawMeshGPU();
				}
			}
			updateStopTime = (float)stopwatch.ElapsedTicks * f;
			updateTime = updateStopTime - updateStartTime;
		}
		else if (dazMesh != null && root != null && nodes != null && draw)
		{
			dazMesh.DrawMorphedUVMappedMesh(root.transform.localToWorldMatrix);
		}
	}
}
