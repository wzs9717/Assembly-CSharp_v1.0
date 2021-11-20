using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SimpleJSON;
using UnityEngine;

public class DAZSkin : MonoBehaviour
{
	public bool draw;

	public bool useOrientation = true;

	[SerializeField]
	protected bool _useGeneralWeights;

	[SerializeField]
	protected bool _useFastNormals;

	[SerializeField]
	protected bool _renormalize;

	public string geometryId;

	public Transform rootBone;

	public Vector3 drawOffset;

	[SerializeField]
	private bool _useSmoothing;

	[SerializeField]
	protected int _numBones;

	public DAZNode[] nodes;

	protected List<DAZNode> importNodes;

	protected Dictionary<string, int> boneNameToIndexMap;

	protected Dictionary<string, Dictionary<int, DAZMeshVertexWeights>> boneWeightsMap;

	protected Dictionary<int, bool> vertexDoneAccumulating;

	protected Transform[] boneTransforms;

	protected DAZBone[] dazBones;

	protected Matrix4x4[] boneMatrices;

	protected Vector3[] boneRotationAngles;

	protected Matrix4x4[] startingMatrices;

	protected Matrix4x4[] startingMatricesInverted;

	protected Mesh mesh;

	public DAZMesh dazMesh;

	protected Vector3[] startVerts;

	protected Vector3[] startNormals;

	protected Vector4[] startTangents;

	protected Vector3[] smoothedVerts;

	public Vector3[] drawVerts;

	public Vector3[] drawNormals;

	public Vector4[] drawTangents;

	protected int[] strongestBone;

	protected float[] strongestBoneWeight;

	protected MeshSmooth meshSmooth;

	public bool useThreadControlNumThreads;

	[SerializeField]
	protected int _numSubThreads;

	public float mainThreadSkinStartTime;

	public float mainThreadSkinStopTime;

	public float mainThreadSkinTime;

	public float[] threadSkinStartTime;

	public float[] threadSkinStopTime;

	public float[] threadSkinTime;

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

	public float mainThreadRenormalizeStartTime;

	public float mainThreadRenormalizeTime;

	public float mainThreadRenormalizeStopTime;

	public float[] threadRenormalizeStartTime;

	public float[] threadRenormalizeTime;

	public float[] threadRenormalizeStopTime;

	protected float bulgeScale = 0.007f;

	protected float smoothCorrectionBeta = 0.5f;

	protected DAZSkinTaskInfo[] tasks;

	protected bool _threadsRunning;

	protected Stopwatch stopwatch;

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

	public bool useFastNormals
	{
		get
		{
			return _useFastNormals;
		}
		set
		{
			_useFastNormals = value;
		}
	}

	public bool renormalize
	{
		get
		{
			return _renormalize;
		}
		set
		{
			_renormalize = value;
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

	public int numBones => _numBones;

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

	public void SetSmoothingFromUIToggle()
	{
	}

	public void ImportStart()
	{
		importNodes = new List<DAZNode>();
	}

	public void ImportNode(JSONNode jn)
	{
		DAZNode dAZNode = new DAZNode();
		dAZNode.name = jn["id"];
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
		dAZNode.rotationOrder = rotationOrder;
		bool flag = false;
		if (importNodes == null)
		{
			importNodes = new List<DAZNode>();
		}
		for (int i = 0; i < importNodes.Count; i++)
		{
			if (importNodes[i].name == dAZNode.name)
			{
				importNodes[i] = dAZNode;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			importNodes.Add(dAZNode);
		}
	}

	protected DAZNode FindNode(string name)
	{
		for (int i = 0; i < importNodes.Count; i++)
		{
			if (importNodes[i].name == name)
			{
				return importNodes[i];
			}
		}
		UnityEngine.Debug.LogWarning("Could not find node " + name);
		return null;
	}

	protected Dictionary<int, DAZMeshVertexWeights> WalkBonesAndAccumulateWeights(Transform bone)
	{
		vertexDoneAccumulating = new Dictionary<int, bool>();
		return WalkBonesAndAccumulateWeightsRecursive(bone);
	}

	protected Dictionary<int, DAZMeshVertexWeights> WalkBonesAndAccumulateWeightsRecursive(Transform bone)
	{
		if (boneWeightsMap.TryGetValue(bone.name, out var value))
		{
			foreach (Transform item in bone)
			{
				Dictionary<int, DAZMeshVertexWeights> dictionary = WalkBonesAndAccumulateWeightsRecursive(item);
				if (dictionary != null)
				{
					foreach (int key in dictionary.Keys)
					{
						if (!vertexDoneAccumulating.ContainsKey(key) && dictionary.TryGetValue(key, out var value2))
						{
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
								DAZMeshVertexWeights dAZMeshVertexWeights = new DAZMeshVertexWeights();
								dAZMeshVertexWeights.vertex = value2.vertex;
								dAZMeshVertexWeights.xweight = value2.xweight;
								dAZMeshVertexWeights.yweight = value2.yweight;
								dAZMeshVertexWeights.zweight = value2.zweight;
								value.Add(key, dAZMeshVertexWeights);
							}
						}
					}
				}
			}
			return value;
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
			int num = 0;
			DAZNode dAZNode = nodes[value2];
			dAZNode.weights = new DAZMeshVertexWeights[value.Count];
			foreach (int key2 in value.Keys)
			{
				if (value.TryGetValue(key2, out var value3))
				{
					dAZNode.weights[num] = value3;
					num++;
				}
			}
		}
	}

	public void Import(JSONNode jn)
	{
		if (rootBone == null)
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
			if (dAZMesh.geometryId == geometryId)
			{
				dazMesh = dAZMesh;
				break;
			}
		}
		if (dazMesh == null)
		{
			UnityEngine.Debug.LogError("Could not find DAZMesh component with geometryID " + geometryId);
			return;
		}
		Dictionary<int, List<int>> baseVertToUVVertFullMap = dazMesh.baseVertToUVVertFullMap;
		boneNameToIndexMap = new Dictionary<string, int>();
		boneWeightsMap = new Dictionary<string, Dictionary<int, DAZMeshVertexWeights>>();
		nodes = new DAZNode[_numBones];
		int num = 0;
		foreach (JSONNode item in jSONNode.AsArray)
		{
			string key = item["id"];
			DAZNode dAZNode = FindNode(key);
			nodes[num] = dAZNode;
			boneNameToIndexMap.Add(key, num);
			Dictionary<int, DAZMeshVertexWeights> dictionary = new Dictionary<int, DAZMeshVertexWeights>();
			foreach (JSONNode item2 in item["node_weights"]["values"].AsArray)
			{
				int asInt = item2[0].AsInt;
				float asFloat = item2[1].AsFloat;
				if (!baseVertToUVVertFullMap.TryGetValue(asInt, out var value))
				{
					continue;
				}
				foreach (int item3 in value)
				{
					if (dictionary.TryGetValue(item3, out var value2))
					{
						value2.weight = asFloat;
						dictionary.Remove(item3);
						dictionary.Add(item3, value2);
					}
					else
					{
						value2 = new DAZMeshVertexWeights();
						value2.vertex = item3;
						value2.weight = asFloat;
						dictionary.Add(item3, value2);
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
						value4 = new DAZMeshVertexWeights();
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
						value6 = new DAZMeshVertexWeights();
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
						value8 = new DAZMeshVertexWeights();
						value8.vertex = item9;
						value8.zweight = asFloat4;
						dictionary.Add(item9, value8);
					}
				}
			}
			DAZBulgeFactors dAZBulgeFactors = new DAZBulgeFactors();
			dAZBulgeFactors.name = key;
			foreach (JSONNode item10 in item["bulge_weights"]["x"]["bulges"].AsArray)
			{
				switch ((string)item10["id"])
				{
				case "positive-left":
					dAZBulgeFactors.xposleft = item10["value"].AsFloat;
					break;
				case "positive-right":
					dAZBulgeFactors.xposright = item10["value"].AsFloat;
					break;
				case "negative-left":
					dAZBulgeFactors.xnegleft = 0f - item10["value"].AsFloat;
					break;
				case "negative-right":
					dAZBulgeFactors.xnegright = 0f - item10["value"].AsFloat;
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
						value10 = new DAZMeshVertexWeights();
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
						value12 = new DAZMeshVertexWeights();
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
					dAZBulgeFactors.ynegleft = 0f - item15["value"].AsFloat;
					break;
				case "positive-right":
					dAZBulgeFactors.ynegright = 0f - item15["value"].AsFloat;
					break;
				case "negative-left":
					dAZBulgeFactors.yposleft = item15["value"].AsFloat;
					break;
				case "negative-right":
					dAZBulgeFactors.yposright = item15["value"].AsFloat;
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
						value14 = new DAZMeshVertexWeights();
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
						value16 = new DAZMeshVertexWeights();
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
					dAZBulgeFactors.znegleft = 0f - item20["value"].AsFloat;
					break;
				case "positive-right":
					dAZBulgeFactors.znegright = 0f - item20["value"].AsFloat;
					break;
				case "negative-left":
					dAZBulgeFactors.zposleft = item20["value"].AsFloat;
					break;
				case "negative-right":
					dAZBulgeFactors.zposright = item20["value"].AsFloat;
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
						value18 = new DAZMeshVertexWeights();
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
						value20 = new DAZMeshVertexWeights();
						value20.vertex = item24;
						value20.zrightbulge = asFloat10;
						dictionary.Add(item24, value20);
					}
				}
			}
			nodes[num].bulgeFactors = dAZBulgeFactors;
			boneWeightsMap.Add(key, dictionary);
			num++;
		}
		WalkBonesAndAccumulateWeights(rootBone);
		CreateBoneWeightsArray();
	}

	protected void InitBones()
	{
		boneTransforms = new Transform[numBones];
		dazBones = new DAZBone[numBones];
		boneMatrices = new Matrix4x4[numBones];
		boneRotationAngles = new Vector3[numBones];
		startingMatrices = new Matrix4x4[numBones];
		startingMatricesInverted = new Matrix4x4[numBones];
		Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
		Dictionary<string, Matrix4x4> dictionary2 = new Dictionary<string, Matrix4x4>();
		if ((bool)rootBone)
		{
			Transform[] componentsInChildren = rootBone.GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (!dictionary.ContainsKey(transform.name))
				{
					dictionary.Add(transform.name, transform);
					dictionary2.Add(transform.name, transform.worldToLocalMatrix * base.transform.localToWorldMatrix);
				}
			}
		}
		for (int j = 0; j < numBones; j++)
		{
			string text = nodes[j].name;
			if (dictionary.TryGetValue(text, out var value))
			{
				boneTransforms[j] = value;
				DAZBone component = value.GetComponent<DAZBone>();
				dazBones[j] = component;
			}
			else
			{
				UnityEngine.Debug.LogError("Could not find transform for bone " + text);
			}
			if (dictionary2.TryGetValue(text, out var value2))
			{
				startingMatrices[j] = value2;
				ref Matrix4x4 reference = ref startingMatricesInverted[j];
				reference = Matrix4x4.Inverse(value2);
			}
			else
			{
				UnityEngine.Debug.LogError("Could not find transform for bone " + text);
			}
		}
	}

	protected void InitSmoothing()
	{
		if (meshSmooth == null)
		{
			meshSmooth = new MeshSmooth(dazMesh.baseVertices, dazMesh.basePolyList);
		}
	}

	protected void InitMesh()
	{
		if (dazMesh != null)
		{
			mesh = UnityEngine.Object.Instantiate(dazMesh.morphedUVMappedMesh);
			startVerts = dazMesh.morphedUVVertices;
			strongestBone = new int[startVerts.Length];
			strongestBoneWeight = new float[startVerts.Length];
			for (int i = 0; i < numBones; i++)
			{
				DAZMeshVertexWeights[] weights = nodes[i].weights;
				foreach (DAZMeshVertexWeights dAZMeshVertexWeights in weights)
				{
					if (dAZMeshVertexWeights.weight > strongestBoneWeight[dAZMeshVertexWeights.vertex])
					{
						strongestBoneWeight[dAZMeshVertexWeights.vertex] = dAZMeshVertexWeights.weight;
						strongestBone[dAZMeshVertexWeights.vertex] = i;
					}
				}
			}
			smoothedVerts = (Vector3[])startVerts.Clone();
			drawVerts = new Vector3[startVerts.Length];
			startNormals = dazMesh.morphedUVNormals;
			drawNormals = new Vector3[startNormals.Length];
			startTangents = dazMesh.morphedUVTangents;
			drawTangents = new Vector4[startTangents.Length];
			Bounds bounds = new Bounds(size: new Vector3(10000f, 10000f, 10000f), center: base.transform.position);
			mesh.bounds = bounds;
		}
		else
		{
			UnityEngine.Debug.LogError("Could not find mesh matching geometryId: " + geometryId);
		}
	}

	public void SetNumSubThreads(int num)
	{
		numSubThreads = num;
	}

	public void InitSkinTimes()
	{
		threadSkinTime = new float[_numSubThreads];
		threadSkinStartTime = new float[_numSubThreads];
		threadSkinStopTime = new float[_numSubThreads];
		threadSmoothTime = new float[_numSubThreads];
		threadSmoothStartTime = new float[_numSubThreads];
		threadSmoothStopTime = new float[_numSubThreads];
		threadSmoothCorrectionTime = new float[_numSubThreads];
		threadSmoothCorrectionStartTime = new float[_numSubThreads];
		threadSmoothCorrectionStopTime = new float[_numSubThreads];
		threadRenormalizeTime = new float[_numSubThreads];
		threadRenormalizeStartTime = new float[_numSubThreads];
		threadRenormalizeStopTime = new float[_numSubThreads];
	}

	protected void StopThreads()
	{
		_threadsRunning = false;
		if (tasks != null)
		{
			for (int i = 0; i < tasks.Length; i++)
			{
				tasks[i].resetEvent.Set();
				while (tasks[i].thread.IsAlive)
				{
				}
			}
		}
		tasks = null;
	}

	protected void StartThreads()
	{
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
				dAZSkinTaskInfo.thread.Start(dAZSkinTaskInfo);
				tasks[i] = dAZSkinTaskInfo;
			}
		}
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
			dAZSkinTaskInfo.resetEvent.WaitOne(-1, exitContext: false);
			float num = 1000f / (float)Stopwatch.Frequency;
			if (dAZSkinTaskInfo.taskType == DAZSkinTaskType.Skin)
			{
				threadSkinStartTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * num;
				SkinMeshPart(dAZSkinTaskInfo.index1, dAZSkinTaskInfo.index2);
				threadSkinStopTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * num;
				threadSkinTime[dAZSkinTaskInfo.threadIndex] = threadSkinStopTime[dAZSkinTaskInfo.threadIndex] - threadSkinStartTime[dAZSkinTaskInfo.threadIndex];
			}
			else if (dAZSkinTaskInfo.taskType == DAZSkinTaskType.Smooth)
			{
				threadSmoothStartTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * num;
				meshSmooth.LaplacianSmooth(drawVerts, smoothedVerts, dAZSkinTaskInfo.index1, dAZSkinTaskInfo.index2);
				threadSmoothStopTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * num;
				threadSmoothTime[dAZSkinTaskInfo.threadIndex] = threadSmoothStopTime[dAZSkinTaskInfo.threadIndex] - threadSmoothStartTime[dAZSkinTaskInfo.threadIndex];
			}
			else if (dAZSkinTaskInfo.taskType == DAZSkinTaskType.SmoothCorrection)
			{
				threadSmoothCorrectionStartTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * num;
				meshSmooth.HCCorrection(drawVerts, smoothedVerts, smoothCorrectionBeta, dAZSkinTaskInfo.index1, dAZSkinTaskInfo.index2);
				threadSmoothCorrectionStopTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * num;
				threadSmoothCorrectionTime[dAZSkinTaskInfo.threadIndex] = threadSmoothCorrectionStopTime[dAZSkinTaskInfo.threadIndex] - threadSmoothCorrectionStartTime[dAZSkinTaskInfo.threadIndex];
			}
			else if (dAZSkinTaskInfo.taskType == DAZSkinTaskType.Renormalize)
			{
				threadRenormalizeStartTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * num;
				RenormalizePart(dAZSkinTaskInfo.index1, dAZSkinTaskInfo.index2);
				threadRenormalizeStopTime[dAZSkinTaskInfo.threadIndex] = (float)stopwatch.ElapsedTicks * num;
				threadRenormalizeTime[dAZSkinTaskInfo.threadIndex] = threadRenormalizeStopTime[dAZSkinTaskInfo.threadIndex] - threadRenormalizeStartTime[dAZSkinTaskInfo.threadIndex];
			}
			dAZSkinTaskInfo.working = false;
		}
	}

	protected void SkinMesh()
	{
		if (!(mesh != null))
		{
			return;
		}
		if (_useGeneralWeights)
		{
			for (int i = 0; i < drawVerts.Length; i++)
			{
				drawVerts[i].x = 0f;
				drawVerts[i].y = 0f;
				drawVerts[i].z = 0f;
				drawNormals[i].x = 0f;
				drawNormals[i].y = 0f;
				drawNormals[i].z = 0f;
				drawTangents[i].x = 0f;
				drawTangents[i].y = 0f;
				drawTangents[i].z = 0f;
				drawTangents[i].w = startTangents[i].w;
			}
		}
		else
		{
			for (int j = 0; j < drawVerts.Length; j++)
			{
				ref Vector3 reference = ref drawVerts[j];
				reference = startVerts[j];
				ref Vector3 reference2 = ref drawNormals[j];
				reference2 = startNormals[j];
				ref Vector4 reference3 = ref drawTangents[j];
				reference3 = startTangents[j];
			}
		}
		for (int k = 0; k < numBones; k++)
		{
			Transform transform = boneTransforms[k];
			ref Matrix4x4 reference4 = ref boneMatrices[k];
			reference4 = transform.localToWorldMatrix * startingMatrices[k];
			if (!_useGeneralWeights)
			{
				DAZBone dAZBone = dazBones[k];
				if (dAZBone == null)
				{
					ref Vector3 reference5 = ref boneRotationAngles[k];
					reference5 = Quaternion2Angles.GetAngles(transform.localRotation, nodes[k].rotationOrder);
				}
				else
				{
					ref Vector3 reference6 = ref boneRotationAngles[k];
					reference6 = dAZBone.GetAngles();
				}
			}
		}
		if (tasks == null)
		{
			StartThreads();
		}
		int num = drawVerts.Length;
		float num2 = 1000f / (float)Stopwatch.Frequency;
		if (_numSubThreads > 0)
		{
			int num3 = num / _numSubThreads;
			for (int l = 0; l < _numSubThreads; l++)
			{
				tasks[l].taskType = DAZSkinTaskType.Skin;
				tasks[l].index1 = l * num3;
				if (l == _numSubThreads - 1)
				{
					tasks[l].index2 = num - 1;
				}
				else
				{
					tasks[l].index2 = (l + 1) * num3 - 1;
				}
				tasks[l].working = true;
				tasks[l].resetEvent.Set();
			}
			bool flag;
			do
			{
				flag = false;
				for (int m = 0; m < _numSubThreads; m++)
				{
					if (tasks[m].working)
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
			mainThreadSkinStartTime = (float)stopwatch.ElapsedTicks * num2;
			SkinMeshPart(0, num - 1);
			mainThreadSkinStopTime = (float)stopwatch.ElapsedTicks * num2;
			mainThreadSkinTime = mainThreadSkinStopTime - mainThreadSkinStartTime;
		}
		if (useSmoothing)
		{
			InitSmoothing();
			if (_numSubThreads > 0)
			{
				int num4 = dazMesh.baseVertices.Length;
				int num5 = num4 / _numSubThreads;
				for (int n = 0; n < _numSubThreads; n++)
				{
					tasks[n].taskType = DAZSkinTaskType.Smooth;
					tasks[n].index1 = n * num5;
					if (n == _numSubThreads - 1)
					{
						tasks[n].index2 = num4 - 1;
					}
					else
					{
						tasks[n].index2 = (n + 1) * num5 - 1;
					}
					tasks[n].working = true;
					tasks[n].resetEvent.Set();
				}
				bool flag2;
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
				for (int num7 = 0; num7 < _numSubThreads; num7++)
				{
					tasks[num7].taskType = DAZSkinTaskType.SmoothCorrection;
					tasks[num7].index1 = num7 * num5;
					if (num7 == _numSubThreads - 1)
					{
						tasks[num7].index2 = num4 - 1;
					}
					else
					{
						tasks[num7].index2 = (num7 + 1) * num5 - 1;
					}
					tasks[num7].working = true;
					tasks[num7].resetEvent.Set();
				}
				do
				{
					flag2 = false;
					for (int num8 = 0; num8 < _numSubThreads; num8++)
					{
						if (tasks[num8].working)
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
					ref Vector3 reference7 = ref smoothedVerts[dAZVertexMap.tovert];
					reference7 = smoothedVerts[dAZVertexMap.fromvert];
				}
				mesh.vertices = smoothedVerts;
			}
			else
			{
				mainThreadSmoothStartTime = (float)stopwatch.ElapsedTicks * num2;
				meshSmooth.LaplacianSmooth(drawVerts, smoothedVerts);
				mainThreadSmoothStopTime = (float)stopwatch.ElapsedTicks * num2;
				mainThreadSmoothTime = mainThreadSmoothStopTime - mainThreadSmoothStartTime;
				mainThreadSmoothCorrectionStartTime = (float)stopwatch.ElapsedTicks * num2;
				meshSmooth.HCCorrection(drawVerts, smoothedVerts, 0.5f);
				mainThreadSmoothCorrectionStopTime = (float)stopwatch.ElapsedTicks * num2;
				mainThreadSmoothCorrectionTime = mainThreadSmoothCorrectionStopTime - mainThreadSmoothCorrectionStartTime;
				DAZVertexMap[] baseVerticesToUVVertices2 = dazMesh.baseVerticesToUVVertices;
				foreach (DAZVertexMap dAZVertexMap2 in baseVerticesToUVVertices2)
				{
					ref Vector3 reference8 = ref smoothedVerts[dAZVertexMap2.tovert];
					reference8 = smoothedVerts[dAZVertexMap2.fromvert];
				}
				mesh.vertices = smoothedVerts;
			}
		}
		else
		{
			DAZVertexMap[] baseVerticesToUVVertices3 = dazMesh.baseVerticesToUVVertices;
			foreach (DAZVertexMap dAZVertexMap3 in baseVerticesToUVVertices3)
			{
				ref Vector3 reference9 = ref drawVerts[dAZVertexMap3.tovert];
				reference9 = drawVerts[dAZVertexMap3.fromvert];
			}
			mesh.vertices = drawVerts;
		}
		if (_useGeneralWeights && !_useFastNormals && _renormalize)
		{
			if (_numSubThreads > 0)
			{
				int num12 = num / _numSubThreads;
				for (int num13 = 0; num13 < _numSubThreads; num13++)
				{
					tasks[num13].taskType = DAZSkinTaskType.Renormalize;
					tasks[num13].index1 = num13 * num12;
					if (num13 == _numSubThreads - 1)
					{
						tasks[num13].index2 = num - 1;
					}
					else
					{
						tasks[num13].index2 = (num13 + 1) * num12 - 1;
					}
					tasks[num13].working = true;
					tasks[num13].resetEvent.Set();
				}
				bool flag3;
				do
				{
					flag3 = false;
					for (int num14 = 0; num14 < _numSubThreads; num14++)
					{
						if (tasks[num14].working)
						{
							flag3 = true;
						}
					}
					if (flag3)
					{
						Thread.Sleep(0);
					}
				}
				while (flag3);
			}
			else
			{
				mainThreadRenormalizeStartTime = (float)stopwatch.ElapsedTicks * num2;
				Renormalize();
				mainThreadRenormalizeStopTime = (float)stopwatch.ElapsedTicks * num2;
				mainThreadRenormalizeTime = mainThreadRenormalizeStopTime - mainThreadRenormalizeStartTime;
			}
		}
		mesh.normals = drawNormals;
		mesh.tangents = drawTangents;
	}

	protected void RenormalizePart(int startIndex, int stopIndex)
	{
		for (int i = startIndex; i <= stopIndex; i++)
		{
			float num = 1f / Mathf.Sqrt(drawNormals[i].x * drawNormals[i].x + drawNormals[i].y * drawNormals[i].y + drawNormals[i].z * drawNormals[i].z);
			drawNormals[i].x *= num;
			drawNormals[i].y *= num;
			drawNormals[i].z *= num;
			num = 1f / Mathf.Sqrt(drawTangents[i].x * drawTangents[i].x + drawTangents[i].y * drawTangents[i].y + drawTangents[i].z * drawTangents[i].z);
			drawTangents[i].x *= num;
			drawTangents[i].y *= num;
			drawTangents[i].z *= num;
		}
	}

	protected void Renormalize()
	{
		RenormalizePart(0, drawNormals.Length - 1);
	}

	protected void SkinMeshPart(int startIndex, int stopIndex)
	{
		Vector3 vector32 = default(Vector3);
		Vector3 vector46 = default(Vector3);
		Vector3 vector18 = default(Vector3);
		Vector3 vector39 = default(Vector3);
		Vector3 vector25 = default(Vector3);
		Vector3 vector11 = default(Vector3);
		for (int i = 0; i < numBones; i++)
		{
			DAZMeshVertexWeights[] weights = nodes[i].weights;
			float m = boneMatrices[i].m00;
			float m2 = boneMatrices[i].m01;
			float m3 = boneMatrices[i].m02;
			float m4 = boneMatrices[i].m03;
			float m5 = boneMatrices[i].m10;
			float m6 = boneMatrices[i].m11;
			float m7 = boneMatrices[i].m12;
			float m8 = boneMatrices[i].m13;
			float m9 = boneMatrices[i].m20;
			float m10 = boneMatrices[i].m21;
			float m11 = boneMatrices[i].m22;
			float m12 = boneMatrices[i].m23;
			if (_useGeneralWeights)
			{
				foreach (DAZMeshVertexWeights dAZMeshVertexWeights in weights)
				{
					if (dAZMeshVertexWeights.vertex < startIndex || dAZMeshVertexWeights.vertex > stopIndex)
					{
						continue;
					}
					Vector3 vector = startVerts[dAZMeshVertexWeights.vertex];
					drawVerts[dAZMeshVertexWeights.vertex].x += (vector.x * m + vector.y * m2 + vector.z * m3 + m4) * dAZMeshVertexWeights.weight;
					drawVerts[dAZMeshVertexWeights.vertex].y += (vector.x * m5 + vector.y * m6 + vector.z * m7 + m8) * dAZMeshVertexWeights.weight;
					drawVerts[dAZMeshVertexWeights.vertex].z += (vector.x * m9 + vector.y * m10 + vector.z * m11 + m12) * dAZMeshVertexWeights.weight;
					if (_useFastNormals)
					{
						if (strongestBone[dAZMeshVertexWeights.vertex] == i)
						{
							Vector3 vector2 = startNormals[dAZMeshVertexWeights.vertex];
							drawNormals[dAZMeshVertexWeights.vertex].x = vector2.x * m + vector2.y * m2 + vector2.z * m3;
							drawNormals[dAZMeshVertexWeights.vertex].y = vector2.x * m5 + vector2.y * m6 + vector2.z * m7;
							drawNormals[dAZMeshVertexWeights.vertex].z = vector2.x * m9 + vector2.y * m10 + vector2.z * m11;
							Vector4 vector3 = startTangents[dAZMeshVertexWeights.vertex];
							drawTangents[dAZMeshVertexWeights.vertex].x = vector3.x * m + vector3.y * m2 + vector3.z * m3;
							drawTangents[dAZMeshVertexWeights.vertex].y = vector3.x * m5 + vector3.y * m6 + vector3.z * m7;
							drawTangents[dAZMeshVertexWeights.vertex].z = vector3.x * m9 + vector3.y * m10 + vector3.z * m11;
						}
					}
					else
					{
						Vector3 vector4 = startNormals[dAZMeshVertexWeights.vertex];
						drawNormals[dAZMeshVertexWeights.vertex].x += (vector4.x * m + vector4.y * m2 + vector4.z * m3) * dAZMeshVertexWeights.weight;
						drawNormals[dAZMeshVertexWeights.vertex].y += (vector4.x * m5 + vector4.y * m6 + vector4.z * m7) * dAZMeshVertexWeights.weight;
						drawNormals[dAZMeshVertexWeights.vertex].z += (vector4.x * m9 + vector4.y * m10 + vector4.z * m11) * dAZMeshVertexWeights.weight;
						Vector4 vector5 = startTangents[dAZMeshVertexWeights.vertex];
						drawTangents[dAZMeshVertexWeights.vertex].x += (vector5.x * m + vector5.y * m2 + vector5.z * m3) * dAZMeshVertexWeights.weight;
						drawTangents[dAZMeshVertexWeights.vertex].y += (vector5.x * m5 + vector5.y * m6 + vector5.z * m7) * dAZMeshVertexWeights.weight;
						drawTangents[dAZMeshVertexWeights.vertex].z += (vector5.x * m9 + vector5.y * m10 + vector5.z * m11) * dAZMeshVertexWeights.weight;
					}
				}
				continue;
			}
			DAZBulgeFactors bulgeFactors = nodes[i].bulgeFactors;
			Quaternion2Angles.RotationOrder rotationOrder = nodes[i].rotationOrder;
			Matrix4x4 matrix4x = startingMatrices[i];
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
			Matrix4x4 matrix4x2 = startingMatricesInverted[i];
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
			Vector3 vector6 = boneRotationAngles[i];
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
			if (vector6.x > 0.01f)
			{
				if (bulgeFactors.xposleft != 0f)
				{
					flag = true;
					flag2 = true;
					num = bulgeFactors.xposleft * vector6.x * bulgeScale;
				}
				if (bulgeFactors.xposright != 0f)
				{
					flag = true;
					flag4 = true;
					num3 = bulgeFactors.xposright * vector6.x * bulgeScale;
				}
			}
			else if (vector6.x < -0.01f)
			{
				if (bulgeFactors.xnegleft != 0f)
				{
					flag = true;
					flag3 = true;
					num2 = bulgeFactors.xnegleft * vector6.x * bulgeScale;
				}
				if (bulgeFactors.xnegright != 0f)
				{
					flag = true;
					flag5 = true;
					num4 = bulgeFactors.xnegright * vector6.x * bulgeScale;
				}
			}
			if (vector6.y > 0.01f)
			{
				if (bulgeFactors.yposleft != 0f)
				{
					flag6 = true;
					flag7 = true;
					num5 = bulgeFactors.yposleft * vector6.y * bulgeScale;
				}
				if (bulgeFactors.yposright != 0f)
				{
					flag6 = true;
					flag9 = true;
					num7 = bulgeFactors.yposright * vector6.y * bulgeScale;
				}
			}
			else if (vector6.y < -0.01f)
			{
				if (bulgeFactors.ynegleft != 0f)
				{
					flag6 = true;
					flag8 = true;
					num6 = bulgeFactors.ynegleft * vector6.y * bulgeScale;
				}
				if (bulgeFactors.ynegright != 0f)
				{
					flag6 = true;
					flag10 = true;
					num8 = bulgeFactors.ynegright * vector6.y * bulgeScale;
				}
			}
			if (vector6.z > 0.01f)
			{
				if (bulgeFactors.zposleft != 0f)
				{
					flag11 = true;
					flag12 = true;
					num9 = bulgeFactors.zposleft * vector6.z * bulgeScale;
				}
				if (bulgeFactors.zposright != 0f)
				{
					flag11 = true;
					flag14 = true;
					num11 = bulgeFactors.zposright * vector6.z * bulgeScale;
				}
			}
			else if (vector6.z < -0.01f)
			{
				if (bulgeFactors.znegleft != 0f)
				{
					flag11 = true;
					flag13 = true;
					num10 = bulgeFactors.znegleft * vector6.z * bulgeScale;
				}
				if (bulgeFactors.znegright != 0f)
				{
					flag11 = true;
					flag15 = true;
					num12 = bulgeFactors.znegright * vector6.z * bulgeScale;
				}
			}
			switch (rotationOrder)
			{
			case Quaternion2Angles.RotationOrder.XYZ:
				foreach (DAZMeshVertexWeights dAZMeshVertexWeights5 in weights)
				{
					if (dAZMeshVertexWeights5.vertex < startIndex || dAZMeshVertexWeights5.vertex > stopIndex)
					{
						continue;
					}
					if (dAZMeshVertexWeights5.xweight > 0.99999f && dAZMeshVertexWeights5.yweight > 0.99999f && dAZMeshVertexWeights5.zweight > 0.99999f)
					{
						Vector3 vector28 = drawVerts[dAZMeshVertexWeights5.vertex];
						drawVerts[dAZMeshVertexWeights5.vertex].x = vector28.x * m + vector28.y * m2 + vector28.z * m3 + m4;
						drawVerts[dAZMeshVertexWeights5.vertex].y = vector28.x * m5 + vector28.y * m6 + vector28.z * m7 + m8;
						drawVerts[dAZMeshVertexWeights5.vertex].z = vector28.x * m9 + vector28.y * m10 + vector28.z * m11 + m12;
						Vector3 vector29 = drawNormals[dAZMeshVertexWeights5.vertex];
						drawNormals[dAZMeshVertexWeights5.vertex].x = vector29.x * m + vector29.y * m2 + vector29.z * m3;
						drawNormals[dAZMeshVertexWeights5.vertex].y = vector29.x * m5 + vector29.y * m6 + vector29.z * m7;
						drawNormals[dAZMeshVertexWeights5.vertex].z = vector29.x * m9 + vector29.y * m10 + vector29.z * m11;
						Vector4 vector30 = drawTangents[dAZMeshVertexWeights5.vertex];
						drawTangents[dAZMeshVertexWeights5.vertex].x = vector30.x * m + vector30.y * m2 + vector30.z * m3;
						drawTangents[dAZMeshVertexWeights5.vertex].y = vector30.x * m5 + vector30.y * m6 + vector30.z * m7;
						drawTangents[dAZMeshVertexWeights5.vertex].z = vector30.x * m9 + vector30.y * m10 + vector30.z * m11;
						continue;
					}
					if (useOrientation)
					{
						Vector3 vector31 = drawVerts[dAZMeshVertexWeights5.vertex];
						vector32.x = vector31.x * m13 + vector31.y * m14 + vector31.z * m15 + m16;
						vector32.y = vector31.x * m17 + vector31.y * m18 + vector31.z * m19 + m20;
						vector32.z = vector31.x * m21 + vector31.y * m22 + vector31.z * m23 + m24;
					}
					else
					{
						vector32 = drawVerts[dAZMeshVertexWeights5.vertex];
						vector32.x += m16;
						vector32.y += m20;
						vector32.z += m24;
					}
					Vector3 vector33 = drawNormals[dAZMeshVertexWeights5.vertex];
					Vector4 vector34 = drawTangents[dAZMeshVertexWeights5.vertex];
					if (dAZMeshVertexWeights5.zweight > 0f)
					{
						float num77 = vector6.z * dAZMeshVertexWeights5.zweight;
						float num78 = (float)Math.Sin(num77);
						float num79 = (float)Math.Cos(num77);
						float x19 = vector32.x * num79 - vector32.y * num78;
						vector32.y = vector32.x * num78 + vector32.y * num79;
						vector32.x = x19;
						float x20 = vector33.x * num79 - vector33.y * num78;
						vector33.y = vector33.x * num78 + vector33.y * num79;
						vector33.x = x20;
						float x21 = vector34.x * num79 - vector34.y * num78;
						vector34.y = vector34.x * num78 + vector34.y * num79;
						vector34.x = x21;
					}
					if (flag11)
					{
						if (flag12 && dAZMeshVertexWeights5.zleftbulge > 0f)
						{
							float num80 = 1f + num9 * dAZMeshVertexWeights5.zleftbulge;
							vector32.x *= num80;
							vector32.y *= num80;
						}
						if (flag14 && dAZMeshVertexWeights5.zrightbulge > 0f)
						{
							float num81 = 1f + num11 * dAZMeshVertexWeights5.zrightbulge;
							vector32.x *= num81;
							vector32.y *= num81;
						}
						if (flag13 && dAZMeshVertexWeights5.zleftbulge > 0f)
						{
							float num82 = 1f + num10 * dAZMeshVertexWeights5.zleftbulge;
							vector32.x *= num82;
							vector32.y *= num82;
						}
						if (flag15 && dAZMeshVertexWeights5.zrightbulge > 0f)
						{
							float num83 = 1f + num12 * dAZMeshVertexWeights5.zrightbulge;
							vector32.x *= num83;
							vector32.y *= num83;
						}
					}
					if (dAZMeshVertexWeights5.yweight > 0f)
					{
						float num84 = vector6.y * dAZMeshVertexWeights5.yweight;
						float num85 = (float)Math.Sin(num84);
						float num86 = (float)Math.Cos(num84);
						float x22 = vector32.x * num86 + vector32.z * num85;
						vector32.z = vector32.z * num86 - vector32.x * num85;
						vector32.x = x22;
						float x23 = vector33.x * num86 + vector33.z * num85;
						vector33.z = vector33.z * num86 - vector33.x * num85;
						vector33.x = x23;
						float x24 = vector34.x * num86 + vector34.z * num85;
						vector34.z = vector34.z * num86 - vector34.x * num85;
						vector34.x = x24;
					}
					if (flag6)
					{
						if (flag7 && dAZMeshVertexWeights5.yleftbulge > 0f)
						{
							float num87 = 1f + num5 * dAZMeshVertexWeights5.yleftbulge;
							vector32.x *= num87;
							vector32.z *= num87;
						}
						if (flag9 && dAZMeshVertexWeights5.yrightbulge > 0f)
						{
							float num88 = 1f + num7 * dAZMeshVertexWeights5.yrightbulge;
							vector32.x *= num88;
							vector32.z *= num88;
						}
						if (flag8 && dAZMeshVertexWeights5.yleftbulge > 0f)
						{
							float num89 = 1f + num6 * dAZMeshVertexWeights5.yleftbulge;
							vector32.x *= num89;
							vector32.z *= num89;
						}
						if (flag10 && dAZMeshVertexWeights5.yrightbulge > 0f)
						{
							float num90 = 1f + num8 * dAZMeshVertexWeights5.yrightbulge;
							vector32.x *= num90;
							vector32.z *= num90;
						}
					}
					if (dAZMeshVertexWeights5.xweight > 0f)
					{
						float num91 = vector6.x * dAZMeshVertexWeights5.xweight;
						float num92 = (float)Math.Sin(num91);
						float num93 = (float)Math.Cos(num91);
						float y10 = vector32.y * num93 - vector32.z * num92;
						vector32.z = vector32.y * num92 + vector32.z * num93;
						vector32.y = y10;
						float y11 = vector33.y * num93 - vector33.z * num92;
						vector33.z = vector33.y * num92 + vector33.z * num93;
						vector33.y = y11;
						float y12 = vector34.y * num93 - vector34.z * num92;
						vector34.z = vector34.y * num92 + vector34.z * num93;
						vector34.y = y12;
					}
					if (flag)
					{
						if (flag2 && dAZMeshVertexWeights5.xleftbulge > 0f)
						{
							float num94 = 1f + num * dAZMeshVertexWeights5.xleftbulge;
							vector32.y *= num94;
							vector32.z *= num94;
						}
						if (flag4 && dAZMeshVertexWeights5.xrightbulge > 0f)
						{
							float num95 = 1f + num3 * dAZMeshVertexWeights5.xrightbulge;
							vector32.y *= num95;
							vector32.z *= num95;
						}
						if (flag3 && dAZMeshVertexWeights5.xleftbulge > 0f)
						{
							float num96 = 1f + num2 * dAZMeshVertexWeights5.xleftbulge;
							vector32.y *= num96;
							vector32.z *= num96;
						}
						if (flag5 && dAZMeshVertexWeights5.xrightbulge > 0f)
						{
							float num97 = 1f + num4 * dAZMeshVertexWeights5.xrightbulge;
							vector32.y *= num97;
							vector32.z *= num97;
						}
					}
					if (useOrientation)
					{
						drawVerts[dAZMeshVertexWeights5.vertex].x = vector32.x * m25 + vector32.y * m26 + vector32.z * m27 + m28;
						drawVerts[dAZMeshVertexWeights5.vertex].y = vector32.x * m29 + vector32.y * m30 + vector32.z * m31 + m32;
						drawVerts[dAZMeshVertexWeights5.vertex].z = vector32.x * m33 + vector32.y * m34 + vector32.z * m35 + m36;
					}
					else
					{
						drawVerts[dAZMeshVertexWeights5.vertex].x = vector32.x - m16;
						drawVerts[dAZMeshVertexWeights5.vertex].y = vector32.y - m20;
						drawVerts[dAZMeshVertexWeights5.vertex].z = vector32.z - m24;
					}
					drawNormals[dAZMeshVertexWeights5.vertex] = vector33;
					drawTangents[dAZMeshVertexWeights5.vertex] = vector34;
				}
				break;
			case Quaternion2Angles.RotationOrder.XZY:
				foreach (DAZMeshVertexWeights dAZMeshVertexWeights7 in weights)
				{
					if (dAZMeshVertexWeights7.vertex < startIndex || dAZMeshVertexWeights7.vertex > stopIndex)
					{
						continue;
					}
					if (dAZMeshVertexWeights7.xweight > 0.99999f && dAZMeshVertexWeights7.yweight > 0.99999f && dAZMeshVertexWeights7.zweight > 0.99999f)
					{
						Vector3 vector42 = drawVerts[dAZMeshVertexWeights7.vertex];
						drawVerts[dAZMeshVertexWeights7.vertex].x = vector42.x * m + vector42.y * m2 + vector42.z * m3 + m4;
						drawVerts[dAZMeshVertexWeights7.vertex].y = vector42.x * m5 + vector42.y * m6 + vector42.z * m7 + m8;
						drawVerts[dAZMeshVertexWeights7.vertex].z = vector42.x * m9 + vector42.y * m10 + vector42.z * m11 + m12;
						Vector3 vector43 = drawNormals[dAZMeshVertexWeights7.vertex];
						drawNormals[dAZMeshVertexWeights7.vertex].x = vector43.x * m + vector43.y * m2 + vector43.z * m3;
						drawNormals[dAZMeshVertexWeights7.vertex].y = vector43.x * m5 + vector43.y * m6 + vector43.z * m7;
						drawNormals[dAZMeshVertexWeights7.vertex].z = vector43.x * m9 + vector43.y * m10 + vector43.z * m11;
						Vector4 vector44 = drawTangents[dAZMeshVertexWeights7.vertex];
						drawTangents[dAZMeshVertexWeights7.vertex].x = vector44.x * m + vector44.y * m2 + vector44.z * m3;
						drawTangents[dAZMeshVertexWeights7.vertex].y = vector44.x * m5 + vector44.y * m6 + vector44.z * m7;
						drawTangents[dAZMeshVertexWeights7.vertex].z = vector44.x * m9 + vector44.y * m10 + vector44.z * m11;
						continue;
					}
					if (useOrientation)
					{
						Vector3 vector45 = drawVerts[dAZMeshVertexWeights7.vertex];
						vector46.x = vector45.x * m13 + vector45.y * m14 + vector45.z * m15 + m16;
						vector46.y = vector45.x * m17 + vector45.y * m18 + vector45.z * m19 + m20;
						vector46.z = vector45.x * m21 + vector45.y * m22 + vector45.z * m23 + m24;
					}
					else
					{
						vector46 = drawVerts[dAZMeshVertexWeights7.vertex];
						vector46.x += m16;
						vector46.y += m20;
						vector46.z += m24;
					}
					Vector3 vector47 = drawNormals[dAZMeshVertexWeights7.vertex];
					Vector4 vector48 = drawTangents[dAZMeshVertexWeights7.vertex];
					if (dAZMeshVertexWeights7.yweight > 0f)
					{
						float num121 = vector6.y * dAZMeshVertexWeights7.yweight;
						float num122 = (float)Math.Sin(num121);
						float num123 = (float)Math.Cos(num121);
						float x31 = vector46.x * num123 + vector46.z * num122;
						vector46.z = vector46.z * num123 - vector46.x * num122;
						vector46.x = x31;
						float x32 = vector47.x * num123 + vector47.z * num122;
						vector47.z = vector47.z * num123 - vector47.x * num122;
						vector47.x = x32;
						float x33 = vector48.x * num123 + vector48.z * num122;
						vector48.z = vector48.z * num123 - vector48.x * num122;
						vector48.x = x33;
					}
					if (flag6)
					{
						if (flag7 && dAZMeshVertexWeights7.yleftbulge > 0f)
						{
							float num124 = 1f + num5 * dAZMeshVertexWeights7.yleftbulge;
							vector46.x *= num124;
							vector46.z *= num124;
						}
						if (flag9 && dAZMeshVertexWeights7.yrightbulge > 0f)
						{
							float num125 = 1f + num7 * dAZMeshVertexWeights7.yrightbulge;
							vector46.x *= num125;
							vector46.z *= num125;
						}
						if (flag8 && dAZMeshVertexWeights7.yleftbulge > 0f)
						{
							float num126 = 1f + num6 * dAZMeshVertexWeights7.yleftbulge;
							vector46.x *= num126;
							vector46.z *= num126;
						}
						if (flag10 && dAZMeshVertexWeights7.yrightbulge > 0f)
						{
							float num127 = 1f + num8 * dAZMeshVertexWeights7.yrightbulge;
							vector46.x *= num127;
							vector46.z *= num127;
						}
					}
					if (dAZMeshVertexWeights7.zweight > 0f)
					{
						float num128 = vector6.z * dAZMeshVertexWeights7.zweight;
						float num129 = (float)Math.Sin(num128);
						float num130 = (float)Math.Cos(num128);
						float x34 = vector46.x * num130 - vector46.y * num129;
						vector46.y = vector46.x * num129 + vector46.y * num130;
						vector46.x = x34;
						float x35 = vector47.x * num130 - vector47.y * num129;
						vector47.y = vector47.x * num129 + vector47.y * num130;
						vector47.x = x35;
						float x36 = vector48.x * num130 - vector48.y * num129;
						vector48.y = vector48.x * num129 + vector48.y * num130;
						vector48.x = x36;
					}
					if (flag11)
					{
						if (flag12 && dAZMeshVertexWeights7.zleftbulge > 0f)
						{
							float num131 = 1f + num9 * dAZMeshVertexWeights7.zleftbulge;
							vector46.x *= num131;
							vector46.y *= num131;
						}
						if (flag14 && dAZMeshVertexWeights7.zrightbulge > 0f)
						{
							float num132 = 1f + num11 * dAZMeshVertexWeights7.zrightbulge;
							vector46.x *= num132;
							vector46.y *= num132;
						}
						if (flag13 && dAZMeshVertexWeights7.zleftbulge > 0f)
						{
							float num133 = 1f + num10 * dAZMeshVertexWeights7.zleftbulge;
							vector46.x *= num133;
							vector46.y *= num133;
						}
						if (flag15 && dAZMeshVertexWeights7.zrightbulge > 0f)
						{
							float num134 = 1f + num12 * dAZMeshVertexWeights7.zrightbulge;
							vector46.x *= num134;
							vector46.y *= num134;
						}
					}
					if (dAZMeshVertexWeights7.xweight > 0f)
					{
						float num135 = vector6.x * dAZMeshVertexWeights7.xweight;
						float num136 = (float)Math.Sin(num135);
						float num137 = (float)Math.Cos(num135);
						float y16 = vector46.y * num137 - vector46.z * num136;
						vector46.z = vector46.y * num136 + vector46.z * num137;
						vector46.y = y16;
						float y17 = vector47.y * num137 - vector47.z * num136;
						vector47.z = vector47.y * num136 + vector47.z * num137;
						vector47.y = y17;
						float y18 = vector48.y * num137 - vector48.z * num136;
						vector48.z = vector48.y * num136 + vector48.z * num137;
						vector48.y = y18;
					}
					if (flag)
					{
						if (flag2 && dAZMeshVertexWeights7.xleftbulge > 0f)
						{
							float num138 = 1f + num * dAZMeshVertexWeights7.xleftbulge;
							vector46.y *= num138;
							vector46.z *= num138;
						}
						if (flag4 && dAZMeshVertexWeights7.xrightbulge > 0f)
						{
							float num139 = 1f + num3 * dAZMeshVertexWeights7.xrightbulge;
							vector46.y *= num139;
							vector46.z *= num139;
						}
						if (flag3 && dAZMeshVertexWeights7.xleftbulge > 0f)
						{
							float num140 = 1f + num2 * dAZMeshVertexWeights7.xleftbulge;
							vector46.y *= num140;
							vector46.z *= num140;
						}
						if (flag5 && dAZMeshVertexWeights7.xrightbulge > 0f)
						{
							float num141 = 1f + num4 * dAZMeshVertexWeights7.xrightbulge;
							vector46.y *= num141;
							vector46.z *= num141;
						}
					}
					if (useOrientation)
					{
						drawVerts[dAZMeshVertexWeights7.vertex].x = vector46.x * m25 + vector46.y * m26 + vector46.z * m27 + m28;
						drawVerts[dAZMeshVertexWeights7.vertex].y = vector46.x * m29 + vector46.y * m30 + vector46.z * m31 + m32;
						drawVerts[dAZMeshVertexWeights7.vertex].z = vector46.x * m33 + vector46.y * m34 + vector46.z * m35 + m36;
					}
					else
					{
						drawVerts[dAZMeshVertexWeights7.vertex].x = vector46.x - m16;
						drawVerts[dAZMeshVertexWeights7.vertex].y = vector46.y - m20;
						drawVerts[dAZMeshVertexWeights7.vertex].z = vector46.z - m24;
					}
					drawNormals[dAZMeshVertexWeights7.vertex] = vector47;
					drawTangents[dAZMeshVertexWeights7.vertex] = vector48;
				}
				break;
			case Quaternion2Angles.RotationOrder.YXZ:
				foreach (DAZMeshVertexWeights dAZMeshVertexWeights3 in weights)
				{
					if (dAZMeshVertexWeights3.vertex < startIndex || dAZMeshVertexWeights3.vertex > stopIndex)
					{
						continue;
					}
					if (dAZMeshVertexWeights3.xweight > 0.99999f && dAZMeshVertexWeights3.yweight > 0.99999f && dAZMeshVertexWeights3.zweight > 0.99999f)
					{
						Vector3 vector14 = drawVerts[dAZMeshVertexWeights3.vertex];
						drawVerts[dAZMeshVertexWeights3.vertex].x = vector14.x * m + vector14.y * m2 + vector14.z * m3 + m4;
						drawVerts[dAZMeshVertexWeights3.vertex].y = vector14.x * m5 + vector14.y * m6 + vector14.z * m7 + m8;
						drawVerts[dAZMeshVertexWeights3.vertex].z = vector14.x * m9 + vector14.y * m10 + vector14.z * m11 + m12;
						Vector3 vector15 = drawNormals[dAZMeshVertexWeights3.vertex];
						drawNormals[dAZMeshVertexWeights3.vertex].x = vector15.x * m + vector15.y * m2 + vector15.z * m3;
						drawNormals[dAZMeshVertexWeights3.vertex].y = vector15.x * m5 + vector15.y * m6 + vector15.z * m7;
						drawNormals[dAZMeshVertexWeights3.vertex].z = vector15.x * m9 + vector15.y * m10 + vector15.z * m11;
						Vector4 vector16 = drawTangents[dAZMeshVertexWeights3.vertex];
						drawTangents[dAZMeshVertexWeights3.vertex].x = vector16.x * m + vector16.y * m2 + vector16.z * m3;
						drawTangents[dAZMeshVertexWeights3.vertex].y = vector16.x * m5 + vector16.y * m6 + vector16.z * m7;
						drawTangents[dAZMeshVertexWeights3.vertex].z = vector16.x * m9 + vector16.y * m10 + vector16.z * m11;
						continue;
					}
					if (useOrientation)
					{
						Vector3 vector17 = drawVerts[dAZMeshVertexWeights3.vertex];
						vector18.x = vector17.x * m13 + vector17.y * m14 + vector17.z * m15 + m16;
						vector18.y = vector17.x * m17 + vector17.y * m18 + vector17.z * m19 + m20;
						vector18.z = vector17.x * m21 + vector17.y * m22 + vector17.z * m23 + m24;
					}
					else
					{
						vector18 = drawVerts[dAZMeshVertexWeights3.vertex];
						vector18.x += m16;
						vector18.y += m20;
						vector18.z += m24;
					}
					Vector3 vector19 = drawNormals[dAZMeshVertexWeights3.vertex];
					Vector4 vector20 = drawTangents[dAZMeshVertexWeights3.vertex];
					if (dAZMeshVertexWeights3.zweight > 0f)
					{
						float num34 = vector6.z * dAZMeshVertexWeights3.zweight;
						float num35 = (float)Math.Sin(num34);
						float num36 = (float)Math.Cos(num34);
						float x7 = vector18.x * num36 - vector18.y * num35;
						vector18.y = vector18.x * num35 + vector18.y * num36;
						vector18.x = x7;
						float x8 = vector19.x * num36 - vector19.y * num35;
						vector19.y = vector19.x * num35 + vector19.y * num36;
						vector19.x = x8;
						float x9 = vector20.x * num36 - vector20.y * num35;
						vector20.y = vector20.x * num35 + vector20.y * num36;
						vector20.x = x9;
					}
					if (flag11)
					{
						if (flag12 && dAZMeshVertexWeights3.zleftbulge > 0f)
						{
							float num37 = 1f + num9 * dAZMeshVertexWeights3.zleftbulge;
							vector18.x *= num37;
							vector18.y *= num37;
						}
						if (flag14 && dAZMeshVertexWeights3.zrightbulge > 0f)
						{
							float num38 = 1f + num11 * dAZMeshVertexWeights3.zrightbulge;
							vector18.x *= num38;
							vector18.y *= num38;
						}
						if (flag13 && dAZMeshVertexWeights3.zleftbulge > 0f)
						{
							float num39 = 1f + num10 * dAZMeshVertexWeights3.zleftbulge;
							vector18.x *= num39;
							vector18.y *= num39;
						}
						if (flag15 && dAZMeshVertexWeights3.zrightbulge > 0f)
						{
							float num40 = 1f + num12 * dAZMeshVertexWeights3.zrightbulge;
							vector18.x *= num40;
							vector18.y *= num40;
						}
					}
					if (dAZMeshVertexWeights3.xweight > 0f)
					{
						float num41 = vector6.x * dAZMeshVertexWeights3.xweight;
						float num42 = (float)Math.Sin(num41);
						float num43 = (float)Math.Cos(num41);
						float y4 = vector18.y * num43 - vector18.z * num42;
						vector18.z = vector18.y * num42 + vector18.z * num43;
						vector18.y = y4;
						float y5 = vector19.y * num43 - vector19.z * num42;
						vector19.z = vector19.y * num42 + vector19.z * num43;
						vector19.y = y5;
						float y6 = vector20.y * num43 - vector20.z * num42;
						vector20.z = vector20.y * num42 + vector20.z * num43;
						vector20.y = y6;
					}
					if (flag)
					{
						if (flag2 && dAZMeshVertexWeights3.xleftbulge > 0f)
						{
							float num44 = 1f + num * dAZMeshVertexWeights3.xleftbulge;
							vector18.y *= num44;
							vector18.z *= num44;
						}
						if (flag4 && dAZMeshVertexWeights3.xrightbulge > 0f)
						{
							float num45 = 1f + num3 * dAZMeshVertexWeights3.xrightbulge;
							vector18.y *= num45;
							vector18.z *= num45;
						}
						if (flag3 && dAZMeshVertexWeights3.xleftbulge > 0f)
						{
							float num46 = 1f + num2 * dAZMeshVertexWeights3.xleftbulge;
							vector18.y *= num46;
							vector18.z *= num46;
						}
						if (flag5 && dAZMeshVertexWeights3.xrightbulge > 0f)
						{
							float num47 = 1f + num4 * dAZMeshVertexWeights3.xrightbulge;
							vector18.y *= num47;
							vector18.z *= num47;
						}
					}
					if (dAZMeshVertexWeights3.yweight > 0f)
					{
						float num48 = vector6.y * dAZMeshVertexWeights3.yweight;
						float num49 = (float)Math.Sin(num48);
						float num50 = (float)Math.Cos(num48);
						float x10 = vector18.x * num50 + vector18.z * num49;
						vector18.z = vector18.z * num50 - vector18.x * num49;
						vector18.x = x10;
						float x11 = vector19.x * num50 + vector19.z * num49;
						vector19.z = vector19.z * num50 - vector19.x * num49;
						vector19.x = x11;
						float x12 = vector20.x * num50 + vector20.z * num49;
						vector20.z = vector20.z * num50 - vector20.x * num49;
						vector20.x = x12;
					}
					if (flag6)
					{
						if (flag7 && dAZMeshVertexWeights3.yleftbulge > 0f)
						{
							float num51 = 1f + num5 * dAZMeshVertexWeights3.yleftbulge;
							vector18.x *= num51;
							vector18.z *= num51;
						}
						if (flag9 && dAZMeshVertexWeights3.yrightbulge > 0f)
						{
							float num52 = 1f + num7 * dAZMeshVertexWeights3.yrightbulge;
							vector18.x *= num52;
							vector18.z *= num52;
						}
						if (flag8 && dAZMeshVertexWeights3.yleftbulge > 0f)
						{
							float num53 = 1f + num6 * dAZMeshVertexWeights3.yleftbulge;
							vector18.x *= num53;
							vector18.z *= num53;
						}
						if (flag10 && dAZMeshVertexWeights3.yrightbulge > 0f)
						{
							float num54 = 1f + num8 * dAZMeshVertexWeights3.yrightbulge;
							vector18.x *= num54;
							vector18.z *= num54;
						}
					}
					if (useOrientation)
					{
						drawVerts[dAZMeshVertexWeights3.vertex].x = vector18.x * m25 + vector18.y * m26 + vector18.z * m27 + m28;
						drawVerts[dAZMeshVertexWeights3.vertex].y = vector18.x * m29 + vector18.y * m30 + vector18.z * m31 + m32;
						drawVerts[dAZMeshVertexWeights3.vertex].z = vector18.x * m33 + vector18.y * m34 + vector18.z * m35 + m36;
					}
					else
					{
						drawVerts[dAZMeshVertexWeights3.vertex].x = vector18.x - m16;
						drawVerts[dAZMeshVertexWeights3.vertex].y = vector18.y - m20;
						drawVerts[dAZMeshVertexWeights3.vertex].z = vector18.z - m24;
					}
					drawNormals[dAZMeshVertexWeights3.vertex] = vector19;
					drawTangents[dAZMeshVertexWeights3.vertex] = vector20;
				}
				break;
			case Quaternion2Angles.RotationOrder.YZX:
				foreach (DAZMeshVertexWeights dAZMeshVertexWeights6 in weights)
				{
					if (dAZMeshVertexWeights6.vertex < startIndex || dAZMeshVertexWeights6.vertex > stopIndex)
					{
						continue;
					}
					if (dAZMeshVertexWeights6.xweight > 0.99999f && dAZMeshVertexWeights6.yweight > 0.99999f && dAZMeshVertexWeights6.zweight > 0.99999f)
					{
						Vector3 vector35 = drawVerts[dAZMeshVertexWeights6.vertex];
						drawVerts[dAZMeshVertexWeights6.vertex].x = vector35.x * m + vector35.y * m2 + vector35.z * m3 + m4;
						drawVerts[dAZMeshVertexWeights6.vertex].y = vector35.x * m5 + vector35.y * m6 + vector35.z * m7 + m8;
						drawVerts[dAZMeshVertexWeights6.vertex].z = vector35.x * m9 + vector35.y * m10 + vector35.z * m11 + m12;
						Vector3 vector36 = drawNormals[dAZMeshVertexWeights6.vertex];
						drawNormals[dAZMeshVertexWeights6.vertex].x = vector36.x * m + vector36.y * m2 + vector36.z * m3;
						drawNormals[dAZMeshVertexWeights6.vertex].y = vector36.x * m5 + vector36.y * m6 + vector36.z * m7;
						drawNormals[dAZMeshVertexWeights6.vertex].z = vector36.x * m9 + vector36.y * m10 + vector36.z * m11;
						Vector4 vector37 = drawTangents[dAZMeshVertexWeights6.vertex];
						drawTangents[dAZMeshVertexWeights6.vertex].x = vector37.x * m + vector37.y * m2 + vector37.z * m3;
						drawTangents[dAZMeshVertexWeights6.vertex].y = vector37.x * m5 + vector37.y * m6 + vector37.z * m7;
						drawTangents[dAZMeshVertexWeights6.vertex].z = vector37.x * m9 + vector37.y * m10 + vector37.z * m11;
						continue;
					}
					if (useOrientation)
					{
						Vector3 vector38 = drawVerts[dAZMeshVertexWeights6.vertex];
						vector39.x = vector38.x * m13 + vector38.y * m14 + vector38.z * m15 + m16;
						vector39.y = vector38.x * m17 + vector38.y * m18 + vector38.z * m19 + m20;
						vector39.z = vector38.x * m21 + vector38.y * m22 + vector38.z * m23 + m24;
					}
					else
					{
						vector39 = drawVerts[dAZMeshVertexWeights6.vertex];
						vector39.x += m16;
						vector39.y += m20;
						vector39.z += m24;
					}
					Vector3 vector40 = drawNormals[dAZMeshVertexWeights6.vertex];
					Vector4 vector41 = drawTangents[dAZMeshVertexWeights6.vertex];
					if (dAZMeshVertexWeights6.xweight > 0f)
					{
						float num99 = vector6.x * dAZMeshVertexWeights6.xweight;
						float num100 = (float)Math.Sin(num99);
						float num101 = (float)Math.Cos(num99);
						float y13 = vector39.y * num101 - vector39.z * num100;
						vector39.z = vector39.y * num100 + vector39.z * num101;
						vector39.y = y13;
						float y14 = vector40.y * num101 - vector40.z * num100;
						vector40.z = vector40.y * num100 + vector40.z * num101;
						vector40.y = y14;
						float y15 = vector41.y * num101 - vector41.z * num100;
						vector41.z = vector41.y * num100 + vector41.z * num101;
						vector41.y = y15;
					}
					if (flag)
					{
						if (flag2 && dAZMeshVertexWeights6.xleftbulge > 0f)
						{
							float num102 = 1f + num * dAZMeshVertexWeights6.xleftbulge;
							vector39.y *= num102;
							vector39.z *= num102;
						}
						if (flag4 && dAZMeshVertexWeights6.xrightbulge > 0f)
						{
							float num103 = 1f + num3 * dAZMeshVertexWeights6.xrightbulge;
							vector39.y *= num103;
							vector39.z *= num103;
						}
						if (flag3 && dAZMeshVertexWeights6.xleftbulge > 0f)
						{
							float num104 = 1f + num2 * dAZMeshVertexWeights6.xleftbulge;
							vector39.y *= num104;
							vector39.z *= num104;
						}
						if (flag5 && dAZMeshVertexWeights6.xrightbulge > 0f)
						{
							float num105 = 1f + num4 * dAZMeshVertexWeights6.xrightbulge;
							vector39.y *= num105;
							vector39.z *= num105;
						}
					}
					if (dAZMeshVertexWeights6.zweight > 0f)
					{
						float num106 = vector6.z * dAZMeshVertexWeights6.zweight;
						float num107 = (float)Math.Sin(num106);
						float num108 = (float)Math.Cos(num106);
						float x25 = vector39.x * num108 - vector39.y * num107;
						vector39.y = vector39.x * num107 + vector39.y * num108;
						vector39.x = x25;
						float x26 = vector40.x * num108 - vector40.y * num107;
						vector40.y = vector40.x * num107 + vector40.y * num108;
						vector40.x = x26;
						float x27 = vector41.x * num108 - vector41.y * num107;
						vector41.y = vector41.x * num107 + vector41.y * num108;
						vector41.x = x27;
					}
					if (flag11)
					{
						if (flag12 && dAZMeshVertexWeights6.zleftbulge > 0f)
						{
							float num109 = 1f + num9 * dAZMeshVertexWeights6.zleftbulge;
							vector39.x *= num109;
							vector39.y *= num109;
						}
						if (flag14 && dAZMeshVertexWeights6.zrightbulge > 0f)
						{
							float num110 = 1f + num11 * dAZMeshVertexWeights6.zrightbulge;
							vector39.x *= num110;
							vector39.y *= num110;
						}
						if (flag13 && dAZMeshVertexWeights6.zleftbulge > 0f)
						{
							float num111 = 1f + num10 * dAZMeshVertexWeights6.zleftbulge;
							vector39.x *= num111;
							vector39.y *= num111;
						}
						if (flag15 && dAZMeshVertexWeights6.zrightbulge > 0f)
						{
							float num112 = 1f + num12 * dAZMeshVertexWeights6.zrightbulge;
							vector39.x *= num112;
							vector39.y *= num112;
						}
					}
					if (dAZMeshVertexWeights6.yweight > 0f)
					{
						float num113 = vector6.y * dAZMeshVertexWeights6.yweight;
						float num114 = (float)Math.Sin(num113);
						float num115 = (float)Math.Cos(num113);
						float x28 = vector39.x * num115 + vector39.z * num114;
						vector39.z = vector39.z * num115 - vector39.x * num114;
						vector39.x = x28;
						float x29 = vector40.x * num115 + vector40.z * num114;
						vector40.z = vector40.z * num115 - vector40.x * num114;
						vector40.x = x29;
						float x30 = vector41.x * num115 + vector41.z * num114;
						vector41.z = vector41.z * num115 - vector41.x * num114;
						vector41.x = x30;
					}
					if (flag6)
					{
						if (flag7 && dAZMeshVertexWeights6.yleftbulge > 0f)
						{
							float num116 = 1f + num5 * dAZMeshVertexWeights6.yleftbulge;
							vector39.x *= num116;
							vector39.z *= num116;
						}
						if (flag9 && dAZMeshVertexWeights6.yrightbulge > 0f)
						{
							float num117 = 1f + num7 * dAZMeshVertexWeights6.yrightbulge;
							vector39.x *= num117;
							vector39.z *= num117;
						}
						if (flag8 && dAZMeshVertexWeights6.yleftbulge > 0f)
						{
							float num118 = 1f + num6 * dAZMeshVertexWeights6.yleftbulge;
							vector39.x *= num118;
							vector39.z *= num118;
						}
						if (flag10 && dAZMeshVertexWeights6.yrightbulge > 0f)
						{
							float num119 = 1f + num8 * dAZMeshVertexWeights6.yrightbulge;
							vector39.x *= num119;
							vector39.z *= num119;
						}
					}
					if (useOrientation)
					{
						drawVerts[dAZMeshVertexWeights6.vertex].x = vector39.x * m25 + vector39.y * m26 + vector39.z * m27 + m28;
						drawVerts[dAZMeshVertexWeights6.vertex].y = vector39.x * m29 + vector39.y * m30 + vector39.z * m31 + m32;
						drawVerts[dAZMeshVertexWeights6.vertex].z = vector39.x * m33 + vector39.y * m34 + vector39.z * m35 + m36;
					}
					else
					{
						drawVerts[dAZMeshVertexWeights6.vertex].x = vector39.x - m16;
						drawVerts[dAZMeshVertexWeights6.vertex].y = vector39.y - m20;
						drawVerts[dAZMeshVertexWeights6.vertex].z = vector39.z - m24;
					}
					drawNormals[dAZMeshVertexWeights6.vertex] = vector40;
					drawTangents[dAZMeshVertexWeights6.vertex] = vector41;
				}
				break;
			case Quaternion2Angles.RotationOrder.ZXY:
				foreach (DAZMeshVertexWeights dAZMeshVertexWeights4 in weights)
				{
					if (dAZMeshVertexWeights4.vertex < startIndex || dAZMeshVertexWeights4.vertex > stopIndex)
					{
						continue;
					}
					if (dAZMeshVertexWeights4.xweight > 0.99999f && dAZMeshVertexWeights4.yweight > 0.99999f && dAZMeshVertexWeights4.zweight > 0.99999f)
					{
						Vector3 vector21 = drawVerts[dAZMeshVertexWeights4.vertex];
						drawVerts[dAZMeshVertexWeights4.vertex].x = vector21.x * m + vector21.y * m2 + vector21.z * m3 + m4;
						drawVerts[dAZMeshVertexWeights4.vertex].y = vector21.x * m5 + vector21.y * m6 + vector21.z * m7 + m8;
						drawVerts[dAZMeshVertexWeights4.vertex].z = vector21.x * m9 + vector21.y * m10 + vector21.z * m11 + m12;
						Vector3 vector22 = drawNormals[dAZMeshVertexWeights4.vertex];
						drawNormals[dAZMeshVertexWeights4.vertex].x = vector22.x * m + vector22.y * m2 + vector22.z * m3;
						drawNormals[dAZMeshVertexWeights4.vertex].y = vector22.x * m5 + vector22.y * m6 + vector22.z * m7;
						drawNormals[dAZMeshVertexWeights4.vertex].z = vector22.x * m9 + vector22.y * m10 + vector22.z * m11;
						Vector4 vector23 = drawTangents[dAZMeshVertexWeights4.vertex];
						drawTangents[dAZMeshVertexWeights4.vertex].x = vector23.x * m + vector23.y * m2 + vector23.z * m3;
						drawTangents[dAZMeshVertexWeights4.vertex].y = vector23.x * m5 + vector23.y * m6 + vector23.z * m7;
						drawTangents[dAZMeshVertexWeights4.vertex].z = vector23.x * m9 + vector23.y * m10 + vector23.z * m11;
						continue;
					}
					if (useOrientation)
					{
						Vector3 vector24 = drawVerts[dAZMeshVertexWeights4.vertex];
						vector25.x = vector24.x * m13 + vector24.y * m14 + vector24.z * m15 + m16;
						vector25.y = vector24.x * m17 + vector24.y * m18 + vector24.z * m19 + m20;
						vector25.z = vector24.x * m21 + vector24.y * m22 + vector24.z * m23 + m24;
					}
					else
					{
						vector25 = drawVerts[dAZMeshVertexWeights4.vertex];
						vector25.x += m16;
						vector25.y += m20;
						vector25.z += m24;
					}
					Vector3 vector26 = drawNormals[dAZMeshVertexWeights4.vertex];
					Vector4 vector27 = drawTangents[dAZMeshVertexWeights4.vertex];
					if (dAZMeshVertexWeights4.yweight > 0f)
					{
						float num55 = vector6.y * dAZMeshVertexWeights4.yweight;
						float num56 = (float)Math.Sin(num55);
						float num57 = (float)Math.Cos(num55);
						float x13 = vector25.x * num57 + vector25.z * num56;
						vector25.z = vector25.z * num57 - vector25.x * num56;
						vector25.x = x13;
						float x14 = vector26.x * num57 + vector26.z * num56;
						vector26.z = vector26.z * num57 - vector26.x * num56;
						vector26.x = x14;
						float x15 = vector27.x * num57 + vector27.z * num56;
						vector27.z = vector27.z * num57 - vector27.x * num56;
						vector27.x = x15;
					}
					if (flag6)
					{
						if (flag7 && dAZMeshVertexWeights4.yleftbulge > 0f)
						{
							float num58 = 1f + num5 * dAZMeshVertexWeights4.yleftbulge;
							vector25.x *= num58;
							vector25.z *= num58;
						}
						if (flag9 && dAZMeshVertexWeights4.yrightbulge > 0f)
						{
							float num59 = 1f + num7 * dAZMeshVertexWeights4.yrightbulge;
							vector25.x *= num59;
							vector25.z *= num59;
						}
						if (flag8 && dAZMeshVertexWeights4.yleftbulge > 0f)
						{
							float num60 = 1f + num6 * dAZMeshVertexWeights4.yleftbulge;
							vector25.x *= num60;
							vector25.z *= num60;
						}
						if (flag10 && dAZMeshVertexWeights4.yrightbulge > 0f)
						{
							float num61 = 1f + num8 * dAZMeshVertexWeights4.yrightbulge;
							vector25.x *= num61;
							vector25.z *= num61;
						}
					}
					if (dAZMeshVertexWeights4.xweight > 0f)
					{
						float num62 = vector6.x * dAZMeshVertexWeights4.xweight;
						float num63 = (float)Math.Sin(num62);
						float num64 = (float)Math.Cos(num62);
						float y7 = vector25.y * num64 - vector25.z * num63;
						vector25.z = vector25.y * num63 + vector25.z * num64;
						vector25.y = y7;
						float y8 = vector26.y * num64 - vector26.z * num63;
						vector26.z = vector26.y * num63 + vector26.z * num64;
						vector26.y = y8;
						float y9 = vector27.y * num64 - vector27.z * num63;
						vector27.z = vector27.y * num63 + vector27.z * num64;
						vector27.y = y9;
					}
					if (flag)
					{
						if (flag2 && dAZMeshVertexWeights4.xleftbulge > 0f)
						{
							float num65 = 1f + num * dAZMeshVertexWeights4.xleftbulge;
							vector25.y *= num65;
							vector25.z *= num65;
						}
						if (flag4 && dAZMeshVertexWeights4.xrightbulge > 0f)
						{
							float num66 = 1f + num3 * dAZMeshVertexWeights4.xrightbulge;
							vector25.y *= num66;
							vector25.z *= num66;
						}
						if (flag3 && dAZMeshVertexWeights4.xleftbulge > 0f)
						{
							float num67 = 1f + num2 * dAZMeshVertexWeights4.xleftbulge;
							vector25.y *= num67;
							vector25.z *= num67;
						}
						if (flag5 && dAZMeshVertexWeights4.xrightbulge > 0f)
						{
							float num68 = 1f + num4 * dAZMeshVertexWeights4.xrightbulge;
							vector25.y *= num68;
							vector25.z *= num68;
						}
					}
					if (dAZMeshVertexWeights4.zweight > 0f)
					{
						float num69 = vector6.z * dAZMeshVertexWeights4.zweight;
						float num70 = (float)Math.Sin(num69);
						float num71 = (float)Math.Cos(num69);
						float x16 = vector25.x * num71 - vector25.y * num70;
						vector25.y = vector25.x * num70 + vector25.y * num71;
						vector25.x = x16;
						float x17 = vector26.x * num71 - vector26.y * num70;
						vector26.y = vector26.x * num70 + vector26.y * num71;
						vector26.x = x17;
						float x18 = vector27.x * num71 - vector27.y * num70;
						vector27.y = vector27.x * num70 + vector27.y * num71;
						vector27.x = x18;
					}
					if (flag11)
					{
						if (flag12 && dAZMeshVertexWeights4.zleftbulge > 0f)
						{
							float num72 = 1f + num9 * dAZMeshVertexWeights4.zleftbulge;
							vector25.x *= num72;
							vector25.y *= num72;
						}
						if (flag14 && dAZMeshVertexWeights4.zrightbulge > 0f)
						{
							float num73 = 1f + num11 * dAZMeshVertexWeights4.zrightbulge;
							vector25.x *= num73;
							vector25.y *= num73;
						}
						if (flag13 && dAZMeshVertexWeights4.zleftbulge > 0f)
						{
							float num74 = 1f + num10 * dAZMeshVertexWeights4.zleftbulge;
							vector25.x *= num74;
							vector25.y *= num74;
						}
						if (flag15 && dAZMeshVertexWeights4.zrightbulge > 0f)
						{
							float num75 = 1f + num12 * dAZMeshVertexWeights4.zrightbulge;
							vector25.x *= num75;
							vector25.y *= num75;
						}
					}
					if (useOrientation)
					{
						drawVerts[dAZMeshVertexWeights4.vertex].x = vector25.x * m25 + vector25.y * m26 + vector25.z * m27 + m28;
						drawVerts[dAZMeshVertexWeights4.vertex].y = vector25.x * m29 + vector25.y * m30 + vector25.z * m31 + m32;
						drawVerts[dAZMeshVertexWeights4.vertex].z = vector25.x * m33 + vector25.y * m34 + vector25.z * m35 + m36;
					}
					else
					{
						drawVerts[dAZMeshVertexWeights4.vertex].x = vector25.x - m16;
						drawVerts[dAZMeshVertexWeights4.vertex].y = vector25.y - m20;
						drawVerts[dAZMeshVertexWeights4.vertex].z = vector25.z - m24;
					}
					drawNormals[dAZMeshVertexWeights4.vertex] = vector26;
					drawTangents[dAZMeshVertexWeights4.vertex] = vector27;
				}
				break;
			case Quaternion2Angles.RotationOrder.ZYX:
				foreach (DAZMeshVertexWeights dAZMeshVertexWeights2 in weights)
				{
					if (dAZMeshVertexWeights2.vertex < startIndex || dAZMeshVertexWeights2.vertex > stopIndex)
					{
						continue;
					}
					if (dAZMeshVertexWeights2.xweight > 0.99999f && dAZMeshVertexWeights2.yweight > 0.99999f && dAZMeshVertexWeights2.zweight > 0.99999f)
					{
						Vector3 vector7 = drawVerts[dAZMeshVertexWeights2.vertex];
						drawVerts[dAZMeshVertexWeights2.vertex].x = vector7.x * m + vector7.y * m2 + vector7.z * m3 + m4;
						drawVerts[dAZMeshVertexWeights2.vertex].y = vector7.x * m5 + vector7.y * m6 + vector7.z * m7 + m8;
						drawVerts[dAZMeshVertexWeights2.vertex].z = vector7.x * m9 + vector7.y * m10 + vector7.z * m11 + m12;
						Vector3 vector8 = drawNormals[dAZMeshVertexWeights2.vertex];
						drawNormals[dAZMeshVertexWeights2.vertex].x = vector8.x * m + vector8.y * m2 + vector8.z * m3;
						drawNormals[dAZMeshVertexWeights2.vertex].y = vector8.x * m5 + vector8.y * m6 + vector8.z * m7;
						drawNormals[dAZMeshVertexWeights2.vertex].z = vector8.x * m9 + vector8.y * m10 + vector8.z * m11;
						Vector4 vector9 = drawTangents[dAZMeshVertexWeights2.vertex];
						drawTangents[dAZMeshVertexWeights2.vertex].x = vector9.x * m + vector9.y * m2 + vector9.z * m3;
						drawTangents[dAZMeshVertexWeights2.vertex].y = vector9.x * m5 + vector9.y * m6 + vector9.z * m7;
						drawTangents[dAZMeshVertexWeights2.vertex].z = vector9.x * m9 + vector9.y * m10 + vector9.z * m11;
						continue;
					}
					if (useOrientation)
					{
						Vector3 vector10 = drawVerts[dAZMeshVertexWeights2.vertex];
						vector11.x = vector10.x * m13 + vector10.y * m14 + vector10.z * m15 + m16;
						vector11.y = vector10.x * m17 + vector10.y * m18 + vector10.z * m19 + m20;
						vector11.z = vector10.x * m21 + vector10.y * m22 + vector10.z * m23 + m24;
					}
					else
					{
						vector11 = drawVerts[dAZMeshVertexWeights2.vertex];
						vector11.x += m16;
						vector11.y += m20;
						vector11.z += m24;
					}
					Vector3 vector12 = drawNormals[dAZMeshVertexWeights2.vertex];
					Vector4 vector13 = drawTangents[dAZMeshVertexWeights2.vertex];
					if (dAZMeshVertexWeights2.xweight > 0f)
					{
						float num13 = vector6.x * dAZMeshVertexWeights2.xweight;
						float num14 = (float)Math.Sin(num13);
						float num15 = (float)Math.Cos(num13);
						float y = vector11.y * num15 - vector11.z * num14;
						vector11.z = vector11.y * num14 + vector11.z * num15;
						vector11.y = y;
						float y2 = vector12.y * num15 - vector12.z * num14;
						vector12.z = vector12.y * num14 + vector12.z * num15;
						vector12.y = y2;
						float y3 = vector13.y * num15 - vector13.z * num14;
						vector13.z = vector13.y * num14 + vector13.z * num15;
						vector13.y = y3;
					}
					if (flag)
					{
						if (flag2 && dAZMeshVertexWeights2.xleftbulge > 0f)
						{
							float num16 = 1f + num * dAZMeshVertexWeights2.xleftbulge;
							vector11.y *= num16;
							vector11.z *= num16;
						}
						if (flag4 && dAZMeshVertexWeights2.xrightbulge > 0f)
						{
							float num17 = 1f + num3 * dAZMeshVertexWeights2.xrightbulge;
							vector11.y *= num17;
							vector11.z *= num17;
						}
						if (flag3 && dAZMeshVertexWeights2.xleftbulge > 0f)
						{
							float num18 = 1f + num2 * dAZMeshVertexWeights2.xleftbulge;
							vector11.y *= num18;
							vector11.z *= num18;
						}
						if (flag5 && dAZMeshVertexWeights2.xrightbulge > 0f)
						{
							float num19 = 1f + num4 * dAZMeshVertexWeights2.xrightbulge;
							vector11.y *= num19;
							vector11.z *= num19;
						}
					}
					if (dAZMeshVertexWeights2.yweight > 0f)
					{
						float num20 = vector6.y * dAZMeshVertexWeights2.yweight;
						float num21 = (float)Math.Sin(num20);
						float num22 = (float)Math.Cos(num20);
						float x = vector11.x * num22 + vector11.z * num21;
						vector11.z = vector11.z * num22 - vector11.x * num21;
						vector11.x = x;
						float x2 = vector12.x * num22 + vector12.z * num21;
						vector12.z = vector12.z * num22 - vector12.x * num21;
						vector12.x = x2;
						float x3 = vector13.x * num22 + vector13.z * num21;
						vector13.z = vector13.z * num22 - vector13.x * num21;
						vector13.x = x3;
					}
					if (flag6)
					{
						if (flag7 && dAZMeshVertexWeights2.yleftbulge > 0f)
						{
							float num23 = 1f + num5 * dAZMeshVertexWeights2.yleftbulge;
							vector11.x *= num23;
							vector11.z *= num23;
						}
						if (flag9 && dAZMeshVertexWeights2.yrightbulge > 0f)
						{
							float num24 = 1f + num7 * dAZMeshVertexWeights2.yrightbulge;
							vector11.x *= num24;
							vector11.z *= num24;
						}
						if (flag8 && dAZMeshVertexWeights2.yleftbulge > 0f)
						{
							float num25 = 1f + num6 * dAZMeshVertexWeights2.yleftbulge;
							vector11.x *= num25;
							vector11.z *= num25;
						}
						if (flag10 && dAZMeshVertexWeights2.yrightbulge > 0f)
						{
							float num26 = 1f + num8 * dAZMeshVertexWeights2.yrightbulge;
							vector11.x *= num26;
							vector11.z *= num26;
						}
					}
					if (dAZMeshVertexWeights2.zweight > 0f)
					{
						float num27 = vector6.z * dAZMeshVertexWeights2.zweight;
						float num28 = (float)Math.Sin(num27);
						float num29 = (float)Math.Cos(num27);
						float x4 = vector11.x * num29 - vector11.y * num28;
						vector11.y = vector11.x * num28 + vector11.y * num29;
						vector11.x = x4;
						float x5 = vector12.x * num29 - vector12.y * num28;
						vector12.y = vector12.x * num28 + vector12.y * num29;
						vector12.x = x5;
						float x6 = vector13.x * num29 - vector13.y * num28;
						vector13.y = vector13.x * num28 + vector13.y * num29;
						vector13.x = x6;
					}
					if (flag11)
					{
						if (flag12 && dAZMeshVertexWeights2.zleftbulge > 0f)
						{
							float num30 = 1f + num9 * dAZMeshVertexWeights2.zleftbulge;
							vector11.x *= num30;
							vector11.y *= num30;
						}
						if (flag14 && dAZMeshVertexWeights2.zrightbulge > 0f)
						{
							float num31 = 1f + num11 * dAZMeshVertexWeights2.zrightbulge;
							vector11.x *= num31;
							vector11.y *= num31;
						}
						if (flag13 && dAZMeshVertexWeights2.zleftbulge > 0f)
						{
							float num32 = 1f + num10 * dAZMeshVertexWeights2.zleftbulge;
							vector11.x *= num32;
							vector11.y *= num32;
						}
						if (flag15 && dAZMeshVertexWeights2.zrightbulge > 0f)
						{
							float num33 = 1f + num12 * dAZMeshVertexWeights2.zrightbulge;
							vector11.x *= num33;
							vector11.y *= num33;
						}
					}
					if (useOrientation)
					{
						drawVerts[dAZMeshVertexWeights2.vertex].x = vector11.x * m25 + vector11.y * m26 + vector11.z * m27 + m28;
						drawVerts[dAZMeshVertexWeights2.vertex].y = vector11.x * m29 + vector11.y * m30 + vector11.z * m31 + m32;
						drawVerts[dAZMeshVertexWeights2.vertex].z = vector11.x * m33 + vector11.y * m34 + vector11.z * m35 + m36;
					}
					else
					{
						drawVerts[dAZMeshVertexWeights2.vertex].x = vector11.x - m16;
						drawVerts[dAZMeshVertexWeights2.vertex].y = vector11.y - m20;
						drawVerts[dAZMeshVertexWeights2.vertex].z = vector11.z - m24;
					}
					drawNormals[dAZMeshVertexWeights2.vertex] = vector12;
					drawTangents[dAZMeshVertexWeights2.vertex] = vector13;
				}
				break;
			}
		}
	}

	protected void DrawMesh()
	{
		if (!draw || !(mesh != null))
		{
			return;
		}
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m03 += drawOffset.x;
		identity.m13 += drawOffset.y;
		identity.m23 += drawOffset.z;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			if (dazMesh.useSimpleMaterial && (bool)dazMesh.simpleMaterial)
			{
				Graphics.DrawMesh(mesh, identity, dazMesh.simpleMaterial, 0, null, i, null, dazMesh.castShadows, dazMesh.receiveShadows);
			}
			else
			{
				Graphics.DrawMesh(mesh, identity, dazMesh.materials[i], 0, null, i, null, dazMesh.castShadows, dazMesh.receiveShadows);
			}
		}
	}

	private void Start()
	{
		stopwatch = new Stopwatch();
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
	}

	private void OnDestroy()
	{
		if (stopwatch != null)
		{
			stopwatch.Stop();
		}
	}

	private void LateUpdate()
	{
		if (draw)
		{
			SkinMesh();
			DrawMesh();
		}
	}
}
