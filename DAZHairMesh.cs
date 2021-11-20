using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DAZHairMesh : MonoBehaviour
{
	public class DAZHairMeshTaskInfo
	{
		public string name;

		public AutoResetEvent resetEvent;

		public Thread thread;

		public volatile bool working;

		public volatile bool kill;
	}

	[SerializeField]
	private DAZSkinV2MeshSelection _scalpSelectionLive;

	[SerializeField]
	private DAZSkinV2MeshSelection _scalpSelection;

	private List<int> vertexIndices;

	[SerializeField]
	private HairStripV2.HairDrawType _hairDrawTypeLive = HairStripV2.HairDrawType.GPULines;

	[SerializeField]
	private HairStripV2.HairDrawType _hairDrawType;

	[SerializeField]
	private int _numberSegmentsLive = 3;

	[SerializeField]
	private int _numberSegments;

	[SerializeField]
	private int _numSubHairsMinLive = 1;

	[SerializeField]
	private int _numSubHairsMin;

	[SerializeField]
	private int _numSubHairsMaxLive = 1;

	[SerializeField]
	private int _numSubHairsMax;

	[SerializeField]
	private HairStripV2.HairBundleType _bundleTypeLive = HairStripV2.HairBundleType.Circular;

	[SerializeField]
	private HairStripV2.HairBundleType _bundleType;

	[SerializeField]
	private float _subHairTangent1OffsetMaxLive = 0.01f;

	[SerializeField]
	private float _subHairTangent1OffsetMax;

	[SerializeField]
	private float _subHairTangent2OffsetMaxLive = 0.01f;

	[SerializeField]
	private float _subHairTangent2OffsetMax;

	[SerializeField]
	private float _subHairNormalOffsetBendLive;

	[SerializeField]
	private float _subHairNormalOffsetBend;

	public string[] _colliderLayers;

	public string[] colliderLayers;

	public CapsuleCollider[] capsuleColliders;

	public bool useExtendedColliders;

	private ExtendedCapsuleCollider[] extendedColliders;

	[SerializeField]
	private bool _createTangentsLive;

	[SerializeField]
	private bool _createTangents;

	public HairGlobalSettings globalSettings;

	public bool drawHairs = true;

	public bool drawFromAnchor = true;

	public float anchorOffset = 0.005f;

	public float hairLength = 0.15f;

	public float hairWidth = 0.0005f;

	public bool roundSheetHairs = true;

	public float sheetHairRoundness = 0.5f;

	public Material hairMaterial;

	public Material hairMaterialRuntime;

	public Vector3 appliedForce;

	public bool useGravity = true;

	public float gravityMultiplier = 0.1f;

	public float staticMoveDistance = 0.0005f;

	public bool staticFriction;

	public float velocityFactor = 0.92f;

	public float stiffnessRoot = 1f;

	public float stiffnessEnd = 0.3f;

	public float stiffnessVariance = 0.1f;

	public bool enableCollision = true;

	public bool enableSimulation = true;

	public float slowCollidingPoints;

	public float dampenFactor = 0.9f;

	public bool clampAcceleration;

	public bool clampVelocity;

	public float accelerationClamp = 0.015f;

	public float velocityClamp = 0.1f;

	public bool castShadows = true;

	public bool receiveShadows = true;

	public bool debug;

	public int debugHairNum;

	public float debugWidth = 0.005f;

	public bool debugOnlyDrawDebugHair = true;

	public bool debugAllPoints;

	private int _numHairs;

	private List<Mesh> hms;

	private List<Vector3[]> hmverts;

	private List<Vector3[]> hmnormals;

	private List<Vector4[]> hmtangents;

	private List<Vector2[]> hmuvs;

	private List<int[]> hmtriangles;

	private Vector3[] smv;

	private Vector3[] smn;

	private int[] referenceVerts;

	private List<int> postSkinVerts;

	private HairStripV2[] hs;

	private Vector3 meshCenter;

	private float meshRadius;

	private float minX;

	private float minY;

	private float minZ;

	private float maxX;

	private float maxY;

	private float maxZ;

	private Collider[] closeColliders;

	private Vector3[] closeColliderCenters;

	private int colliderMask;

	private int _totalVertices;

	private int totalTrianglePoints;

	private bool wasInit;

	private int initCount;

	public float simTime;

	public float setVarsTime;

	public float drawTime;

	private float deltaTime;

	private float deltaTimeSqr;

	private float invDeltaTime;

	private int maxVerticesPerMesh = 40000;

	public bool useThreading = true;

	protected DAZHairMeshTaskInfo mainHairTask;

	protected bool _threadsRunning;

	public bool needsInit
	{
		get
		{
			if (_bundleTypeLive != _bundleType)
			{
				return true;
			}
			if (_hairDrawTypeLive != _hairDrawType)
			{
				return true;
			}
			if (_numberSegmentsLive != _numberSegments)
			{
				return true;
			}
			if (_numSubHairsMaxLive != _numSubHairsMax)
			{
				return true;
			}
			if (_numSubHairsMinLive != _numSubHairsMin)
			{
				return true;
			}
			if (_scalpSelectionLive != _scalpSelection)
			{
				return true;
			}
			if (_subHairNormalOffsetBendLive != _subHairNormalOffsetBend)
			{
				return true;
			}
			if (_subHairTangent1OffsetMaxLive != _subHairTangent1OffsetMax)
			{
				return true;
			}
			if (_subHairTangent2OffsetMaxLive != _subHairTangent2OffsetMax)
			{
				return true;
			}
			if (_createTangentsLive != _createTangents)
			{
				return true;
			}
			if (colliderLayers.Length != _colliderLayers.Length)
			{
				return true;
			}
			for (int i = 0; i < colliderLayers.Length; i++)
			{
				if (colliderLayers[i] != _colliderLayers[i])
				{
					return true;
				}
			}
			if (_scalpSelection != null && _scalpSelection.changed)
			{
				return true;
			}
			return false;
		}
	}

	public DAZSkinV2MeshSelection scalpSelection
	{
		get
		{
			return _scalpSelectionLive;
		}
		set
		{
			_scalpSelectionLive = value;
		}
	}

	public DAZSkinV2MeshSelection scalpSelectionActive => _scalpSelection;

	public HairStripV2.HairDrawType hairDrawType
	{
		get
		{
			return _hairDrawTypeLive;
		}
		set
		{
			_hairDrawTypeLive = value;
		}
	}

	public HairStripV2.HairDrawType hairDrawTypeActive => _hairDrawType;

	public int numberSegments
	{
		get
		{
			return _numberSegmentsLive;
		}
		set
		{
			if (_hairDrawTypeLive == HairStripV2.HairDrawType.GPULines)
			{
				_numberSegmentsLive = value;
			}
			else
			{
				_numberSegmentsLive = value;
			}
		}
	}

	public int numberSegmentsActive => _numberSegments;

	public int numSubHairsMin
	{
		get
		{
			return _numSubHairsMinLive;
		}
		set
		{
			_numSubHairsMinLive = value;
			if (_numSubHairsMinLive > _numSubHairsMaxLive)
			{
				_numSubHairsMinLive = _numSubHairsMaxLive;
			}
		}
	}

	public int numSubHairsMinActive => _numSubHairsMin;

	public int numSubHairsMax
	{
		get
		{
			return _numSubHairsMaxLive;
		}
		set
		{
			_numSubHairsMaxLive = value;
			if (_numSubHairsMaxLive < _numSubHairsMinLive)
			{
				_numSubHairsMaxLive = _numSubHairsMinLive;
			}
		}
	}

	public int numSubHairsMaxActive => _numSubHairsMax;

	public HairStripV2.HairBundleType bundleType
	{
		get
		{
			return _bundleTypeLive;
		}
		set
		{
			_bundleTypeLive = value;
		}
	}

	public HairStripV2.HairBundleType bundleTypeActive => _bundleType;

	public float subHairTangent1OffsetMax
	{
		get
		{
			return _subHairTangent1OffsetMaxLive;
		}
		set
		{
			_subHairTangent1OffsetMaxLive = value;
		}
	}

	public float subHairTangent1OffsetMaxActive => _subHairTangent1OffsetMax;

	public float subHairTangent2OffsetMax
	{
		get
		{
			return _subHairTangent2OffsetMaxLive;
		}
		set
		{
			_subHairTangent2OffsetMaxLive = value;
		}
	}

	public float subHairTangent2OffsetMaxActive => _subHairTangent2OffsetMax;

	public float subHairNormalOffsetBend
	{
		get
		{
			return _subHairNormalOffsetBendLive;
		}
		set
		{
			_subHairNormalOffsetBendLive = value;
		}
	}

	public float subHairNormalOffsetBendActive => _subHairNormalOffsetBend;

	public bool createTangents
	{
		get
		{
			return _createTangentsLive;
		}
		set
		{
			_createTangentsLive = value;
		}
	}

	public bool createTangentsActive => _createTangents;

	public int numHairs => _numHairs;

	public int totalVertices => _totalVertices;

	public bool threadsRunning => _threadsRunning;

	private void SetGlobalLiveVars()
	{
		globalSettings.drawFromAnchor = drawFromAnchor;
		globalSettings.hairLength = hairLength;
		globalSettings.segmentLength = hairLength / (float)numberSegments;
		globalSettings.quarterSegmentLength = globalSettings.segmentLength * 0.25f;
		globalSettings.hairWidth = hairWidth;
		globalSettings.hairHalfWidth = hairWidth * 0.5f;
		globalSettings.roundSheetHairs = roundSheetHairs;
		globalSettings.sheetHairRoundness = sheetHairRoundness;
		globalSettings.hairMaterial = hairMaterial;
		if (useGravity)
		{
			globalSettings.gravityForce = Physics.gravity * gravityMultiplier + appliedForce;
		}
		else
		{
			globalSettings.gravityForce = appliedForce;
		}
		globalSettings.useExtendedColliders = useExtendedColliders;
		globalSettings.extendedColliders = extendedColliders;
		globalSettings.staticMoveDistance = staticMoveDistance;
		globalSettings.staticMoveDistanceSqr = staticMoveDistance * staticMoveDistance;
		globalSettings.staticFriction = staticFriction;
		globalSettings.velocityFactor = velocityFactor;
		globalSettings.stiffnessRoot = stiffnessRoot;
		globalSettings.stiffnessEnd = stiffnessEnd;
		globalSettings.stiffnessVariance = stiffnessVariance;
		globalSettings.enableSimulation = enableSimulation && (SuperController.singleton == null || !SuperController.singleton.freezeAnimation);
		globalSettings.deltaTime = deltaTime;
		globalSettings.deltaTimeSqr = deltaTimeSqr;
		globalSettings.invDeltaTime = invDeltaTime;
		globalSettings.invdtdampen = invDeltaTime * dampenFactor;
		globalSettings.slowCollidingPoints = slowCollidingPoints;
		globalSettings.dampenFactor = dampenFactor;
		globalSettings.clampAcceleration = clampAcceleration;
		globalSettings.clampVelocity = clampVelocity;
		globalSettings.accelerationClamp = accelerationClamp;
		globalSettings.velocityClamp = velocityClamp;
		globalSettings.castShadows = castShadows;
		globalSettings.receiveShadows = receiveShadows;
		globalSettings.debugWidth = debugWidth;
	}

	private void SetHairStripLiveVars(int i, int j, bool initial)
	{
		Vector3 vector = smv[j];
		Vector3 vector2 = smn[j];
		int num = referenceVerts[j];
		Vector3 vector3 = default(Vector3);
		vector3.x = smv[num].x - smv[j].x;
		vector3.y = smv[num].y - smv[j].y;
		vector3.z = smv[num].z - smv[j].z;
		float num2 = 1f / vector3.magnitude;
		vector3.x *= num2;
		vector3.y *= num2;
		vector3.z *= num2;
		Vector3 vector4 = default(Vector3);
		vector4.x = vector2.y * vector3.z - vector2.z * vector3.y;
		vector4.y = vector2.z * vector3.x - vector2.x * vector3.z;
		vector4.z = vector2.x * vector3.y - vector2.y * vector3.x;
		if (debug && debugHairNum == i)
		{
			Debug.DrawRay(vector, vector2, Color.blue);
			Debug.DrawRay(vector, vector3, Color.red);
			Debug.DrawRay(vector, vector4, Color.yellow);
		}
		Matrix4x4 inverse = hs[i].rootMatrix.inverse;
		hs[i].rootMatrix[0] = vector2.x;
		hs[i].rootMatrix[1] = vector2.y;
		hs[i].rootMatrix[2] = vector2.z;
		hs[i].rootMatrix[4] = vector3.x;
		hs[i].rootMatrix[5] = vector3.y;
		hs[i].rootMatrix[6] = vector3.z;
		hs[i].rootMatrix[8] = vector4.x;
		hs[i].rootMatrix[9] = vector4.y;
		hs[i].rootMatrix[10] = vector4.z;
		hs[i].rootMatrix[12] = vector.x;
		hs[i].rootMatrix[13] = vector.y;
		hs[i].rootMatrix[14] = vector.z;
		if (initial)
		{
			hs[i].rootChangeMatrix = Matrix4x4.identity;
		}
		else
		{
			hs[i].rootChangeMatrix = hs[i].rootMatrix * inverse;
		}
		if (anchorOffset == 0f)
		{
			anchorOffset = 0.0001f;
		}
		hs[i].anchor = vector;
		if (i == 0)
		{
			minX = vector.x;
			maxX = vector.x;
			minY = vector.y;
			maxY = vector.y;
			minZ = vector.z;
			maxZ = vector.z;
		}
		else
		{
			if (vector.x < minX)
			{
				minX = vector.x;
			}
			else if (vector.x > maxX)
			{
				maxX = vector.x;
			}
			if (vector.y < minY)
			{
				minY = vector.y;
			}
			else if (vector.y > maxY)
			{
				maxY = vector.y;
			}
			if (vector.z < minZ)
			{
				minZ = vector.z;
			}
			else if (vector.z > maxZ)
			{
				maxZ = vector.z;
			}
		}
		Vector3 root = default(Vector3);
		root.x = vector.x + vector2.x * anchorOffset;
		root.y = vector.y + vector2.y * anchorOffset;
		root.z = vector.z + vector2.z * anchorOffset;
		hs[i].root = root;
		hs[i].anchorToRoot = vector2;
		hs[i].anchorTangent = vector3;
		hs[i].anchorTangent2 = vector4;
		if (debug)
		{
			if (debugHairNum == i)
			{
				hs[i].debug = true;
				hs[i].enableDraw = drawHairs;
				return;
			}
			if (debugOnlyDrawDebugHair)
			{
				hs[i].enableDraw = false;
			}
			else
			{
				hs[i].enableDraw = true;
			}
			hs[i].debug = false;
		}
		else
		{
			hs[i].debug = false;
			hs[i].enableDraw = drawHairs;
		}
	}

	private void CreateColliderMask()
	{
		colliderMask = 0;
		if (_colliderLayers != null)
		{
			string[] array = _colliderLayers;
			foreach (string layerName in array)
			{
				colliderMask |= 1 << LayerMask.NameToLayer(layerName);
			}
		}
		else
		{
			colliderMask = -1;
		}
	}

	private void UpdateMeshCenterAndRadius()
	{
		meshCenter.x = (minX + maxX) * 0.5f;
		meshCenter.y = (minY + maxY) * 0.5f;
		meshCenter.z = (minZ + maxZ) * 0.5f;
		float num = meshCenter.x - minX;
		float num2 = meshCenter.y - minY;
		float num3 = meshCenter.z - minZ;
		if (num2 > num)
		{
			if (num3 > num2)
			{
				meshRadius = num3;
			}
			else
			{
				meshRadius = num2;
			}
		}
		else if (num3 > num)
		{
			meshRadius = num3;
		}
		else
		{
			meshRadius = num;
		}
	}

	private void FindCloseColliders()
	{
		closeColliders = Physics.OverlapSphere(meshCenter, meshRadius + hairLength, colliderMask);
		int num = 0;
		Collider[] array = closeColliders;
		foreach (Collider collider in array)
		{
			if (collider is SphereCollider)
			{
				SphereCollider sphereCollider = (SphereCollider)collider;
				ref Vector3 reference = ref closeColliderCenters[num];
				reference = collider.transform.TransformPoint(sphereCollider.center);
			}
			else if (collider is BoxCollider)
			{
				BoxCollider boxCollider = (BoxCollider)collider;
				ref Vector3 reference2 = ref closeColliderCenters[num];
				reference2 = collider.transform.TransformPoint(boxCollider.center);
			}
			else if (collider is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
				ref Vector3 reference3 = ref closeColliderCenters[num];
				reference3 = collider.transform.TransformPoint(capsuleCollider.center);
			}
			else
			{
				ref Vector3 reference4 = ref closeColliderCenters[num];
				reference4 = Vector3.zero;
			}
			num++;
			if (num >= 100)
			{
				break;
			}
		}
		globalSettings.colliders = closeColliders;
		globalSettings.colliderCenters = closeColliderCenters;
	}

	private void AddMesh(int numVertices, int numTrianglePoints)
	{
		Mesh item = new Mesh();
		hms.Add(item);
		Vector3[] item2 = new Vector3[numVertices];
		hmverts.Add(item2);
		Vector3[] item3 = new Vector3[numVertices];
		hmnormals.Add(item3);
		if (createTangents)
		{
			Vector4[] item4 = new Vector4[numVertices];
			hmtangents.Add(item4);
		}
		Vector2[] item5 = new Vector2[numVertices];
		hmuvs.Add(item5);
		int[] item6 = new int[numTrianglePoints];
		hmtriangles.Add(item6);
	}

	protected void StopThreads()
	{
		_threadsRunning = false;
		if (mainHairTask != null)
		{
			mainHairTask.kill = true;
			mainHairTask.resetEvent.Set();
			while (mainHairTask.thread.IsAlive)
			{
			}
			mainHairTask = null;
		}
	}

	protected void StartThreads()
	{
		if (!_threadsRunning)
		{
			_threadsRunning = true;
			mainHairTask = new DAZHairMeshTaskInfo();
			mainHairTask.name = "MainHairTask";
			mainHairTask.resetEvent = new AutoResetEvent(initialState: false);
			mainHairTask.thread = new Thread(MTTask);
			mainHairTask.thread.Priority = System.Threading.ThreadPriority.BelowNormal;
			mainHairTask.thread.Start(mainHairTask);
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
		DAZHairMeshTaskInfo dAZHairMeshTaskInfo = (DAZHairMeshTaskInfo)info;
		while (_threadsRunning)
		{
			dAZHairMeshTaskInfo.resetEvent.WaitOne(-1, exitContext: true);
			if (dAZHairMeshTaskInfo.kill)
			{
				break;
			}
			Thread.Sleep(0);
			UpdateHairStripsThreaded();
			dAZHairMeshTaskInfo.working = false;
		}
	}

	public void Reset()
	{
		wasInit = false;
	}

	public void init()
	{
		initCount = 1;
		globalSettings = new HairGlobalSettings();
		_scalpSelection = _scalpSelectionLive;
		_hairDrawType = _hairDrawTypeLive;
		_numberSegments = _numberSegmentsLive;
		_numSubHairsMin = _numSubHairsMinLive;
		_numSubHairsMax = _numSubHairsMaxLive;
		_bundleType = _bundleTypeLive;
		_subHairNormalOffsetBend = _subHairNormalOffsetBendLive;
		_subHairTangent1OffsetMax = _subHairTangent1OffsetMaxLive;
		_subHairTangent2OffsetMax = _subHairTangent2OffsetMaxLive;
		_createTangents = _createTangentsLive;
		_colliderLayers = colliderLayers;
		deltaTime = Time.deltaTime;
		globalSettings.deltaTime = deltaTime;
		deltaTimeSqr = deltaTime * deltaTime;
		globalSettings.deltaTimeSqr = deltaTimeSqr;
		invDeltaTime = 1f / deltaTime;
		globalSettings.invDeltaTime = invDeltaTime;
		if (!(_scalpSelection != null) || !(_scalpSelection.skin != null) || !_scalpSelection.skin.gameObject.activeInHierarchy)
		{
			return;
		}
		vertexIndices = new List<int>(_scalpSelection.selectedVertices);
		_numHairs = vertexIndices.Count;
		_scalpSelection.clearChanged();
		if (!Application.isPlaying || !_scalpSelection.skin.wasInit)
		{
			return;
		}
		wasInit = true;
		CreateColliderMask();
		closeColliderCenters = new Vector3[100];
		hms = new List<Mesh>();
		hmverts = new List<Vector3[]>();
		hmnormals = new List<Vector3[]>();
		hmtangents = new List<Vector4[]>();
		hmuvs = new List<Vector2[]>();
		hmtriangles = new List<int[]>();
		smv = (Vector3[])_scalpSelection.skin.drawVerts.Clone();
		smn = (Vector3[])_scalpSelection.skin.drawNormals.Clone();
		if (_hairDrawType == HairStripV2.HairDrawType.GPULines)
		{
			_createTangentsLive = true;
			_createTangents = true;
		}
		bool[] array = new bool[smv.Length];
		for (int i = 0; i < vertexIndices.Count; i++)
		{
			int num = vertexIndices[i];
			if (num >= smv.Length)
			{
				Debug.LogError("mesh selection vertex " + num + " is out of range of vertices: " + smv.Length);
			}
			else
			{
				array[num] = true;
			}
		}
		int[] baseTriangles = _scalpSelection.skin.dazMesh.baseTriangles;
		referenceVerts = new int[smv.Length];
		float[] array2 = new float[smv.Length];
		for (int j = 0; j < smv.Length; j++)
		{
			array2[j] = 100000f;
		}
		for (int k = 0; k < baseTriangles.Length; k += 3)
		{
			int num2 = baseTriangles[k];
			int num3 = baseTriangles[k + 1];
			int num4 = baseTriangles[k + 2];
			if (array[num2])
			{
				if (array[num3])
				{
					float num5 = Mathf.Abs(smv[num2].y - smv[num3].y);
					if (num5 < array2[num2])
					{
						array2[num2] = num5;
						referenceVerts[num2] = num3;
					}
					if (num5 < array2[num3])
					{
						array2[num3] = num5;
						referenceVerts[num3] = num2;
					}
				}
				if (array[num4])
				{
					float num6 = Mathf.Abs(smv[num2].y - smv[num4].y);
					if (num6 < array2[num2])
					{
						array2[num2] = num6;
						referenceVerts[num2] = num4;
					}
					if (num6 < array2[num4])
					{
						array2[num4] = num6;
						referenceVerts[num4] = num2;
					}
				}
			}
			if (array[num3] && array[num4])
			{
				float num7 = Mathf.Abs(smv[num3].y - smv[num4].y);
				if (num7 < array2[num3])
				{
					array2[num3] = num7;
					referenceVerts[num3] = num4;
				}
				if (num7 < array2[num4])
				{
					array2[num4] = num7;
					referenceVerts[num4] = num3;
				}
			}
		}
		hs = new HairStripV2[_numHairs];
		_totalVertices = 0;
		totalTrianglePoints = 0;
		globalSettings.ownMesh = false;
		globalSettings.hairDrawType = _hairDrawType;
		globalSettings.numberSegments = _numberSegments;
		globalSettings.invNumberSegments = 1f / (float)_numberSegments;
		globalSettings.numHairsMin = _numSubHairsMin;
		globalSettings.numHairsMax = _numSubHairsMax;
		globalSettings.bundleType = _bundleType;
		globalSettings.subHairXOffsetMax = _subHairTangent1OffsetMax;
		globalSettings.subHairYOffsetMax = _subHairTangent2OffsetMax;
		globalSettings.subHairZOffsetBend = _subHairNormalOffsetBend;
		globalSettings.createTangents = _createTangents;
		SetGlobalLiveVars();
		for (int l = 0; l < _numHairs; l++)
		{
			hs[l] = new HairStripV2();
			hs[l].globalSettings = globalSettings;
			int j2 = vertexIndices[l];
			SetHairStripLiveVars(l, j2, initial: true);
			hs[l].Init();
			if (_totalVertices + hs[l].numVertices > maxVerticesPerMesh)
			{
				AddMesh(_totalVertices, totalTrianglePoints);
				_totalVertices = 0;
				totalTrianglePoints = 0;
			}
			_totalVertices += hs[l].numVertices;
			totalTrianglePoints += hs[l].numTrianglePoints;
		}
		AddMesh(_totalVertices, totalTrianglePoints);
		UpdateMeshCenterAndRadius();
		FindCloseColliders();
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		for (int m = 0; m < _numHairs; m++)
		{
			if (num8 + hs[m].numVertices > maxVerticesPerMesh)
			{
				num10++;
				num8 = 0;
				num9 = 0;
			}
			hs[m].hmverts = hmverts[num10];
			hs[m].hmnormals = hmnormals[num10];
			if (createTangents)
			{
				hs[m].hmtangents = hmtangents[num10];
			}
			hs[m].hmtriangles = hmtriangles[num10];
			hs[m].hmuvs = hmuvs[num10];
			hs[m].Start(num8, num9);
			hs[m].Update(num8);
			num8 += hs[m].numVertices;
			num9 += hs[m].numTrianglePoints;
		}
		float num11 = (meshRadius + hairLength) * 2f;
		for (int n = 0; n < hmverts.Count; n++)
		{
			hms[n].vertices = hmverts[n];
			hms[n].normals = hmnormals[n];
			if (createTangents)
			{
				hms[n].tangents = hmtangents[n];
			}
			hms[n].uv = hmuvs[n];
			if (_hairDrawType == HairStripV2.HairDrawType.LineStrip)
			{
				hms[n].SetIndices(hmtriangles[n], MeshTopology.LineStrip, 0);
			}
			else if (_hairDrawType == HairStripV2.HairDrawType.GPULines)
			{
				hms[n].SetIndices(hmtriangles[n], MeshTopology.Quads, 0);
			}
			else if (_hairDrawType == HairStripV2.HairDrawType.Lines)
			{
				hms[n].SetIndices(hmtriangles[n], MeshTopology.Lines, 0);
			}
			else
			{
				hms[n].triangles = hmtriangles[n];
			}
			hms[n].bounds = new Bounds(meshCenter, new Vector3(num11, num11, num11));
		}
	}

	private void Start()
	{
		if (hairMaterial != null)
		{
			hairMaterialRuntime = new Material(hairMaterial);
		}
		if (colliderLayers == null)
		{
			colliderLayers = new string[0];
		}
		if (_colliderLayers == null)
		{
			_colliderLayers = new string[0];
		}
		if (capsuleColliders != null && capsuleColliders.Length > 0)
		{
			extendedColliders = new ExtendedCapsuleCollider[capsuleColliders.Length];
			for (int i = 0; i < capsuleColliders.Length; i++)
			{
				extendedColliders[i] = new ExtendedCapsuleCollider();
				extendedColliders[i].collider = capsuleColliders[i];
				extendedColliders[i].RecalculateVars();
			}
		}
	}

	private void Update()
	{
		if (extendedColliders != null && useExtendedColliders)
		{
			ExtendedCapsuleCollider[] array = extendedColliders;
			foreach (ExtendedCapsuleCollider extendedCapsuleCollider in array)
			{
				extendedCapsuleCollider.UpdateEndpoints();
				extendedCapsuleCollider.RecalculateVars();
			}
		}
	}

	private void UpdateHairStripsThreaded()
	{
		float elapsedMilliseconds = GlobalStopwatch.GetElapsedMilliseconds();
		for (int i = 0; i < _numHairs; i++)
		{
			int j = vertexIndices[i];
			if (initCount > 0)
			{
				SetHairStripLiveVars(i, j, initial: true);
			}
			else
			{
				SetHairStripLiveVars(i, j, initial: false);
			}
		}
		if (initCount > 0)
		{
			initCount--;
		}
		float elapsedMilliseconds2 = GlobalStopwatch.GetElapsedMilliseconds();
		setVarsTime = elapsedMilliseconds2 - elapsedMilliseconds;
		UpdateMeshCenterAndRadius();
		int num = 0;
		for (int k = 0; k < _numHairs; k++)
		{
			if (num + hs[k].numVertices > maxVerticesPerMesh)
			{
				num = 0;
			}
			hs[k].UpdateThreadSafe(num);
			num += hs[k].numVertices;
		}
		float elapsedMilliseconds3 = GlobalStopwatch.GetElapsedMilliseconds();
		simTime = elapsedMilliseconds3 - elapsedMilliseconds2;
	}

	private void StartFrame()
	{
		deltaTime = Time.deltaTime;
		deltaTimeSqr = deltaTime * deltaTime;
		invDeltaTime = 1f / deltaTime;
		simTime = 0f;
		SetGlobalLiveVars();
		_scalpSelection.skin.allowPostSkinMorph = true;
		foreach (int vertexIndex in vertexIndices)
		{
			if (!_scalpSelection.skin.postSkinVerts[vertexIndex])
			{
				_scalpSelection.skin.postSkinVerts[vertexIndex] = true;
				_scalpSelection.skin.postSkinVertsChanged = true;
			}
			if (!_scalpSelection.skin.postSkinNormalVerts[vertexIndex])
			{
				_scalpSelection.skin.postSkinNormalVerts[vertexIndex] = true;
				_scalpSelection.skin.postSkinVertsChanged = true;
			}
			ref Vector3 reference = ref smv[vertexIndex];
			reference = _scalpSelection.skin.rawSkinnedVerts[vertexIndex];
			ref Vector3 reference2 = ref smn[vertexIndex];
			reference2 = _scalpSelection.skin.postSkinNormals[vertexIndex];
		}
	}

	private void FinishFrame()
	{
		float num = (meshRadius + hairLength) * 2f;
		for (int i = 0; i < hmverts.Count; i++)
		{
			hms[i].vertices = hmverts[i];
			if (debugAllPoints)
			{
				for (int j = 0; j < hmverts[i].Length; j++)
				{
					MyDebug.DrawWireCube(hmverts[i][j], 0.001f, Color.cyan);
				}
			}
			hms[i].normals = hmnormals[i];
			if (createTangents)
			{
				hms[i].tangents = hmtangents[i];
			}
			hms[i].bounds = new Bounds(meshCenter, new Vector3(num, num, num));
		}
	}

	private void LateUpdate()
	{
		if (!wasInit)
		{
			init();
		}
		if (!wasInit || !(_scalpSelection != null) || !(_scalpSelection.skin != null) || !(hairMaterial != null))
		{
			return;
		}
		if (useThreading)
		{
			StartThreads();
			if (!mainHairTask.working)
			{
				FinishFrame();
				StartFrame();
				mainHairTask.working = true;
				mainHairTask.resetEvent.Set();
			}
		}
		else
		{
			StartFrame();
			UpdateHairStripsThreaded();
			FinishFrame();
		}
		float elapsedMilliseconds = GlobalStopwatch.GetElapsedMilliseconds();
		if (drawHairs)
		{
			for (int i = 0; i < hmverts.Count; i++)
			{
				Graphics.DrawMesh(hms[i], Matrix4x4.identity, hairMaterialRuntime, 0, null, 0, null, castShadows, receiveShadows);
			}
		}
		float elapsedMilliseconds2 = GlobalStopwatch.GetElapsedMilliseconds();
		drawTime = elapsedMilliseconds2 - elapsedMilliseconds;
	}
}
