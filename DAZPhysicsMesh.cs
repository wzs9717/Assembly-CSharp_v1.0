using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DAZPhysicsMesh : JSONStorable, ISerializationCallbackReceiver
{
	public enum DAZPhysicsMeshTaskType
	{
		UpdateSoftJointTargets,
		MorphVertices
	}

	public class DAZPhysicsMeshTaskInfo
	{
		public DAZPhysicsMeshTaskType taskType;

		public int threadIndex;

		public string name;

		public AutoResetEvent resetEvent;

		public Thread thread;

		public volatile bool working;

		public volatile bool kill;

		public int index1;

		public int index2;
	}

	public enum SelectionMode
	{
		HardTarget,
		HardTargetAuto,
		SoftTarget,
		SoftAnchor,
		SoftInfluenced,
		SoftAuto,
		SoftLink,
		SoftSelect,
		SoftSpringSet,
		ColliderEditEnd1,
		ColliderEditEnd2,
		ColliderEditFront,
		SoftSizeSet,
		SoftLimitSet,
		SoftAutoRadius
	}

	public static bool globalEnable = true;

	public bool editorDirty;

	protected List<AsyncFlag> waitResumeSimulationFlags;

	protected int delayResumeSimulation;

	public Transform transformToEnableWhenOn;

	public Transform transformToEnableWhenOff;

	protected bool _globalOn;

	public Toggle onToggle;

	[SerializeField]
	protected bool _on = true;

	public DAZMorphBank morphBank1ForResizeTrigger;

	public DAZMorphBank morphBank2ForResizeTrigger;

	protected bool morphsChanged;

	public bool useThreading;

	protected DAZPhysicsMeshTaskInfo physicsMeshTask;

	protected bool _threadsRunning;

	[SerializeField]
	protected bool _collisionEnabled = true;

	[SerializeField]
	protected bool _useInterpolation = true;

	[SerializeField]
	protected Transform _skinTransform;

	protected Dictionary<int, List<int>> _baseVertToUVVertFullMap;

	[SerializeField]
	protected DAZSkinV2 _skin;

	[SerializeField]
	protected bool _showHandles;

	[SerializeField]
	protected bool _showBackfaceHandles;

	[SerializeField]
	protected bool _showLinkLines;

	[SerializeField]
	protected bool _showColliders;

	[SerializeField]
	protected bool _showCurrentSoftGroupOnly;

	[SerializeField]
	protected bool _showCurrentSoftSetOnly;

	[SerializeField]
	protected float _handleSize = 0.0002f;

	[SerializeField]
	protected float _softSpringMultiplierSetValue = 1f;

	[SerializeField]
	protected float _softSizeMultiplierSetValue = 1f;

	[SerializeField]
	protected float _softLimitMultiplierSetValue = 1f;

	[SerializeField]
	protected SelectionMode _selectionMode;

	[SerializeField]
	protected int _subMeshSelection = -1;

	[SerializeField]
	protected int _subMeshSelection2 = -1;

	[SerializeField]
	protected bool _showHardGroups;

	[SerializeField]
	protected int _currentHardVerticesGroupIndex;

	[SerializeField]
	protected bool _showSoftGroups;

	[SerializeField]
	protected int _currentSoftVerticesGroupIndex;

	[SerializeField]
	protected bool _showColliderGroups;

	[SerializeField]
	protected int _currentColliderGroupIndex;

	[SerializeField]
	protected List<DAZPhysicsMeshHardVerticesGroup> _hardVerticesGroups;

	[SerializeField]
	protected List<DAZPhysicsMeshSoftVerticesGroup> _softVerticesGroups;

	[SerializeField]
	protected List<DAZPhysicsMeshColliderGroup> _colliderGroups;

	public bool useCombinedSpringAndDamper;

	public Slider softVerticesCombinedSpringSlider;

	public Slider softVerticesCombinedSpringSliderAlt;

	[SerializeField]
	protected float _softVerticesCombinedSpring = 80f;

	public Slider softVerticesCombinedDamperSlider;

	public Slider softVerticesCombinedDamperSliderAlt;

	[SerializeField]
	protected float _softVerticesCombinedDamper = 1f;

	public Slider softVerticesNormalSpringSlider;

	[SerializeField]
	protected float _softVerticesNormalSpring = 10f;

	public Slider softVerticesNormalDamperSlider;

	[SerializeField]
	protected float _softVerticesNormalDamper = 1f;

	public Slider softVerticesTangentSpringSlider;

	[SerializeField]
	protected float _softVerticesTangentSpring = 10f;

	public Slider softVerticesTangentDamperSlider;

	[SerializeField]
	protected float _softVerticesTangentDamper = 1f;

	public Slider softVerticesSpringMaxForceSlider;

	[SerializeField]
	protected float _softVerticesSpringMaxForce = 10f;

	public Slider softVerticesMassSlider;

	public Slider softVerticesMassSliderAlt;

	[SerializeField]
	protected float _softVerticesMass = 0.1f;

	public Slider softVerticesBackForceSlider;

	[SerializeField]
	protected float _softVerticesBackForce;

	public Slider softVerticesBackForceThresholdDistanceSlider;

	[SerializeField]
	protected float _softVerticesBackForceThresholdDistance = 0.01f;

	public Slider softVerticesBackForceMaxForceSlider;

	[SerializeField]
	protected float _softVerticesBackForceMaxForce;

	public Toggle softVerticesUseUniformLimitToggle;

	[SerializeField]
	protected bool _softVerticesUseUniformLimit;

	public Slider softVerticesTangentLimitSlider;

	[SerializeField]
	protected float _softVerticesTangentLimit;

	public Slider softVerticesNormalLimitSlider;

	[SerializeField]
	protected float _softVerticesNormalLimit;

	public Slider softVerticesNegativeNormalLimitSlider;

	[SerializeField]
	protected float _softVerticesNegativeNormalLimit;

	public bool softVerticesUseAutoColliderRadius;

	public int softVerticesAutoColliderVertex1 = -1;

	public int softVerticesAutoColliderVertex2 = -1;

	public float softVerticesAutoColliderMinRadius = 0.02f;

	public float softVerticesAutoColliderMaxRadius = 0.05f;

	public float softVerticesAutoColliderRadiusMultiplier = 1f;

	public float softVerticesAutoColliderRadiusOffset;

	public Slider softVerticesColliderRadiusSlider;

	[SerializeField]
	protected float _softVerticesColliderRadius;

	protected Dictionary<int, DAZPhysicsMeshHardVerticesGroup> _hardTargetVerticesDict;

	protected Dictionary<int, DAZPhysicsMeshSoftVerticesSet> _softTargetVerticesDict;

	protected Dictionary<int, List<DAZPhysicsMeshSoftVerticesSet>> _softAnchorVerticesDict;

	protected Dictionary<int, DAZPhysicsMeshSoftVerticesSet> _softInfluenceVerticesDict;

	protected Dictionary<int, int> _uvVertToBaseVertDict;

	protected DAZPhysicsMeshSoftVerticesSet startSoftLinkSet;

	protected Vector3 zeroVector = Vector3.zero;

	public DAZPhysicsMesh[] ignorePhysicsMeshes;

	protected Mesh _editorMeshForFocus;

	protected bool _wasInit;

	protected bool fixedUpdateDone;

	public bool on
	{
		get
		{
			return _on;
		}
		set
		{
			if (_on != value)
			{
				_on = value;
				SyncOn();
			}
		}
	}

	public bool collisionEnabled
	{
		get
		{
			return _collisionEnabled;
		}
		set
		{
			if (_collisionEnabled != value)
			{
				_collisionEnabled = value;
				SyncCollisionEnabled();
			}
		}
	}

	public bool useInterpolation
	{
		get
		{
			return _useInterpolation;
		}
		set
		{
			if (_useInterpolation != value)
			{
				_useInterpolation = value;
				SyncUseInterpolation();
			}
		}
	}

	public Transform skinTransform
	{
		get
		{
			return _skinTransform;
		}
		set
		{
			if (_skinTransform != value)
			{
				_skinTransform = value;
			}
		}
	}

	public DAZSkinV2 skin
	{
		get
		{
			return _skin;
		}
		set
		{
			if (!(_skin != value))
			{
				return;
			}
			_skin = value;
			if (!(_skin != null))
			{
				return;
			}
			Init();
			if (!(_skin.dazMesh != null))
			{
				return;
			}
			_skin.Init();
			_baseVertToUVVertFullMap = _skin.dazMesh.baseVertToUVVertFullMap;
			foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
			{
				softVerticesGroup.skin = _skin;
			}
		}
	}

	public bool showHandles
	{
		get
		{
			return _showHandles;
		}
		set
		{
			if (_showHandles != value)
			{
				_showHandles = value;
			}
		}
	}

	public bool showBackfaceHandles
	{
		get
		{
			return _showBackfaceHandles;
		}
		set
		{
			if (_showBackfaceHandles != value)
			{
				_showBackfaceHandles = value;
			}
		}
	}

	public bool showLinkLines
	{
		get
		{
			return _showLinkLines;
		}
		set
		{
			if (_showLinkLines != value)
			{
				_showLinkLines = value;
			}
		}
	}

	public bool showColliders
	{
		get
		{
			return _showColliders;
		}
		set
		{
			if (_showColliders != value)
			{
				_showColliders = value;
			}
		}
	}

	public bool showCurrentSoftGroupOnly
	{
		get
		{
			return _showCurrentSoftGroupOnly;
		}
		set
		{
			if (_showCurrentSoftGroupOnly != value)
			{
				_showCurrentSoftGroupOnly = value;
			}
		}
	}

	public bool showCurrentSoftSetOnly
	{
		get
		{
			return _showCurrentSoftSetOnly;
		}
		set
		{
			if (_showCurrentSoftSetOnly != value)
			{
				_showCurrentSoftSetOnly = value;
			}
		}
	}

	public float handleSize
	{
		get
		{
			return _handleSize;
		}
		set
		{
			if (_handleSize != value)
			{
				_handleSize = value;
			}
		}
	}

	public float softSpringMultiplierSetValue
	{
		get
		{
			return _softSpringMultiplierSetValue;
		}
		set
		{
			if (_softSpringMultiplierSetValue != value)
			{
				_softSpringMultiplierSetValue = value;
			}
		}
	}

	public float softSizeMultiplierSetValue
	{
		get
		{
			return _softSizeMultiplierSetValue;
		}
		set
		{
			if (_softSizeMultiplierSetValue != value)
			{
				_softSizeMultiplierSetValue = value;
			}
		}
	}

	public float softLimitMultiplierSetValue
	{
		get
		{
			return _softLimitMultiplierSetValue;
		}
		set
		{
			if (_softLimitMultiplierSetValue != value)
			{
				_softLimitMultiplierSetValue = value;
			}
		}
	}

	public SelectionMode selectionMode
	{
		get
		{
			return _selectionMode;
		}
		set
		{
			if (_selectionMode != value)
			{
				_selectionMode = value;
			}
		}
	}

	public int subMeshSelection
	{
		get
		{
			return _subMeshSelection;
		}
		set
		{
			if (value != _subMeshSelection)
			{
				_subMeshSelection = value;
			}
		}
	}

	public int subMeshSelection2
	{
		get
		{
			return _subMeshSelection2;
		}
		set
		{
			if (value != _subMeshSelection2)
			{
				_subMeshSelection2 = value;
			}
		}
	}

	public bool showHardGroups
	{
		get
		{
			return _showHardGroups;
		}
		set
		{
			if (_showHardGroups != value)
			{
				_showHardGroups = value;
			}
		}
	}

	public int currentHardVerticesGroupIndex
	{
		get
		{
			return _currentHardVerticesGroupIndex;
		}
		set
		{
			if (_currentHardVerticesGroupIndex != value)
			{
				_currentHardVerticesGroupIndex = value;
			}
		}
	}

	public DAZPhysicsMeshHardVerticesGroup currentHardVerticesGroup
	{
		get
		{
			if (_currentHardVerticesGroupIndex >= 0 && _currentHardVerticesGroupIndex < _hardVerticesGroups.Count)
			{
				return _hardVerticesGroups[_currentHardVerticesGroupIndex];
			}
			return null;
		}
	}

	public bool showSoftGroups
	{
		get
		{
			return _showSoftGroups;
		}
		set
		{
			if (_showSoftGroups != value)
			{
				_showSoftGroups = value;
			}
		}
	}

	public int currentSoftVerticesGroupIndex
	{
		get
		{
			return _currentSoftVerticesGroupIndex;
		}
		set
		{
			if (_currentSoftVerticesGroupIndex != value)
			{
				_currentSoftVerticesGroupIndex = value;
			}
		}
	}

	public DAZPhysicsMeshSoftVerticesGroup currentSoftVerticesGroup
	{
		get
		{
			if (_currentSoftVerticesGroupIndex >= 0 && _currentSoftVerticesGroupIndex < _softVerticesGroups.Count)
			{
				return _softVerticesGroups[_currentSoftVerticesGroupIndex];
			}
			return null;
		}
	}

	public bool showColliderGroups
	{
		get
		{
			return _showColliderGroups;
		}
		set
		{
			if (_showColliderGroups != value)
			{
				_showColliderGroups = value;
			}
		}
	}

	public int currentColliderGroupIndex
	{
		get
		{
			return _currentColliderGroupIndex;
		}
		set
		{
			if (_currentColliderGroupIndex != value)
			{
				_currentColliderGroupIndex = value;
			}
		}
	}

	public List<DAZPhysicsMeshHardVerticesGroup> hardVerticesGroups => _hardVerticesGroups;

	public List<DAZPhysicsMeshSoftVerticesGroup> softVerticesGroups => _softVerticesGroups;

	public List<DAZPhysicsMeshColliderGroup> colliderGroups => _colliderGroups;

	public float softVerticesCombinedSpring
	{
		get
		{
			return _softVerticesCombinedSpring;
		}
		set
		{
			if (useCombinedSpringAndDamper && _softVerticesCombinedSpring != value)
			{
				_softVerticesCombinedSpring = value;
				if (softVerticesCombinedSpringSlider != null)
				{
					softVerticesCombinedSpringSlider.value = _softVerticesCombinedSpring;
				}
				if (softVerticesCombinedSpringSliderAlt != null)
				{
					softVerticesCombinedSpringSliderAlt.value = _softVerticesCombinedSpring;
				}
				SyncSoftVerticesCombinedSpring();
			}
		}
	}

	public float softVerticesCombinedDamper
	{
		get
		{
			return _softVerticesCombinedDamper;
		}
		set
		{
			if (useCombinedSpringAndDamper && _softVerticesCombinedDamper != value)
			{
				_softVerticesCombinedDamper = value;
				if (softVerticesCombinedDamperSlider != null)
				{
					softVerticesCombinedDamperSlider.value = _softVerticesCombinedDamper;
				}
				if (softVerticesCombinedDamperSliderAlt != null)
				{
					softVerticesCombinedDamperSliderAlt.value = _softVerticesCombinedDamper;
				}
				SyncSoftVerticesCombinedDamper();
			}
		}
	}

	public float softVerticesNormalSpring
	{
		get
		{
			return _softVerticesNormalSpring;
		}
		set
		{
			if (!useCombinedSpringAndDamper && _softVerticesNormalSpring != value)
			{
				_softVerticesNormalSpring = value;
				if (softVerticesNormalSpringSlider != null)
				{
					softVerticesNormalSpringSlider.value = _softVerticesNormalSpring;
				}
				SyncSoftVerticesNormalSpring();
			}
		}
	}

	public float softVerticesNormalDamper
	{
		get
		{
			return _softVerticesNormalDamper;
		}
		set
		{
			if (!useCombinedSpringAndDamper && _softVerticesNormalDamper != value)
			{
				_softVerticesNormalDamper = value;
				if (softVerticesNormalDamperSlider != null)
				{
					softVerticesNormalDamperSlider.value = _softVerticesNormalDamper;
				}
				SyncSoftVerticesNormalDamper();
			}
		}
	}

	public float softVerticesTangentSpring
	{
		get
		{
			return _softVerticesTangentSpring;
		}
		set
		{
			if (!useCombinedSpringAndDamper && _softVerticesTangentSpring != value)
			{
				_softVerticesTangentSpring = value;
				if (softVerticesTangentSpringSlider != null)
				{
					softVerticesTangentSpringSlider.value = _softVerticesTangentSpring;
				}
				SyncSoftVerticesTangentSpring();
			}
		}
	}

	public float softVerticesTangentDamper
	{
		get
		{
			return _softVerticesTangentDamper;
		}
		set
		{
			if (!useCombinedSpringAndDamper && _softVerticesTangentDamper != value)
			{
				_softVerticesTangentDamper = value;
				if (softVerticesTangentDamperSlider != null)
				{
					softVerticesTangentDamperSlider.value = _softVerticesTangentDamper;
				}
				SyncSoftVerticesTangentDamper();
			}
		}
	}

	public float softVerticesSpringMaxForce
	{
		get
		{
			return _softVerticesSpringMaxForce;
		}
		set
		{
			if (_softVerticesSpringMaxForce != value)
			{
				_softVerticesSpringMaxForce = value;
				if (softVerticesSpringMaxForceSlider != null)
				{
					softVerticesSpringMaxForceSlider.value = _softVerticesSpringMaxForce;
				}
				SyncSoftVerticesSpringMaxForce();
			}
		}
	}

	public float softVerticesMass
	{
		get
		{
			return _softVerticesMass;
		}
		set
		{
			if (_softVerticesMass != value)
			{
				_softVerticesMass = value;
				if (softVerticesMassSlider != null)
				{
					softVerticesMassSlider.value = _softVerticesMass;
				}
				if (softVerticesMassSliderAlt != null)
				{
					softVerticesMassSliderAlt.value = _softVerticesMass;
				}
				SyncSoftVerticesMass();
			}
		}
	}

	public float softVerticesBackForce
	{
		get
		{
			return _softVerticesBackForce;
		}
		set
		{
			if (_softVerticesBackForce != value)
			{
				_softVerticesBackForce = value;
				if (softVerticesBackForceSlider != null)
				{
					softVerticesBackForceSlider.value = _softVerticesBackForce;
				}
				SyncSoftVerticesBackForce();
			}
		}
	}

	public float softVerticesBackForceThresholdDistance
	{
		get
		{
			return _softVerticesBackForceThresholdDistance;
		}
		set
		{
			if (_softVerticesBackForceThresholdDistance != value)
			{
				_softVerticesBackForceThresholdDistance = value;
				if (softVerticesBackForceThresholdDistanceSlider != null)
				{
					softVerticesBackForceThresholdDistanceSlider.value = _softVerticesBackForceThresholdDistance;
				}
				SyncSoftVerticesBackForceThresholdDistance();
			}
		}
	}

	public float softVerticesBackForceMaxForce
	{
		get
		{
			return _softVerticesBackForceMaxForce;
		}
		set
		{
			if (_softVerticesBackForceMaxForce != value)
			{
				_softVerticesBackForceMaxForce = value;
				if (softVerticesBackForceMaxForceSlider != null)
				{
					softVerticesBackForceMaxForceSlider.value = _softVerticesBackForceMaxForce;
				}
				SyncSoftVerticesBackForceMaxForce();
			}
		}
	}

	public bool softVerticesUseUniformLimit
	{
		get
		{
			return _softVerticesUseUniformLimit;
		}
		set
		{
			if (_softVerticesUseUniformLimit != value)
			{
				_softVerticesUseUniformLimit = value;
				if (softVerticesUseUniformLimitToggle != null)
				{
					softVerticesUseUniformLimitToggle.isOn = _softVerticesUseUniformLimit;
				}
				SyncSoftVerticesUseUniformLimit();
			}
		}
	}

	public float softVerticesTangentLimit
	{
		get
		{
			return _softVerticesTangentLimit;
		}
		set
		{
			if (_softVerticesTangentLimit != value)
			{
				_softVerticesTangentLimit = value;
				if (softVerticesTangentLimitSlider != null)
				{
					softVerticesTangentLimitSlider.value = _softVerticesTangentLimit;
				}
				SyncSoftVerticesTangentLimit();
			}
		}
	}

	public float softVerticesNormalLimit
	{
		get
		{
			return _softVerticesNormalLimit;
		}
		set
		{
			if (_softVerticesNormalLimit != value)
			{
				_softVerticesNormalLimit = value;
				if (softVerticesNormalLimitSlider != null)
				{
					softVerticesNormalLimitSlider.value = _softVerticesNormalLimit;
				}
				SyncSoftVerticesNormalLimit();
			}
		}
	}

	public float softVerticesNegativeNormalLimit
	{
		get
		{
			return _softVerticesNegativeNormalLimit;
		}
		set
		{
			if (_softVerticesNegativeNormalLimit != value)
			{
				_softVerticesNegativeNormalLimit = value;
				if (softVerticesNegativeNormalLimitSlider != null)
				{
					softVerticesNegativeNormalLimitSlider.value = _softVerticesNegativeNormalLimit;
				}
				SyncSoftVerticesNegativeNormalLimit();
			}
		}
	}

	public float softVerticesColliderRadius
	{
		get
		{
			return _softVerticesColliderRadius;
		}
		set
		{
			if (_softVerticesColliderRadius != value)
			{
				_softVerticesColliderRadius = value;
				if (softVerticesColliderRadiusSlider != null)
				{
					softVerticesColliderRadiusSlider.value = _softVerticesColliderRadius;
				}
				SyncSoftVerticesColliderRadius();
			}
		}
	}

	public Mesh editorMeshForFocus => _editorMeshForFocus;

	public bool wasInit => _wasInit;

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
	}

	public void PauseSimulation()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.pauseSimulation = true;
		}
	}

	protected void ResumeSimulation()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.pauseSimulation = false;
		}
	}

	protected void DelayResumeSimulation(int count)
	{
		if (count > delayResumeSimulation)
		{
			delayResumeSimulation = count;
		}
	}

	protected void CheckResumeSimulation()
	{
		if (delayResumeSimulation >= 0)
		{
			delayResumeSimulation--;
		}
		if (waitResumeSimulationFlags == null)
		{
			waitResumeSimulationFlags = new List<AsyncFlag>();
		}
		bool flag = false;
		if (waitResumeSimulationFlags.Count > 0)
		{
			List<AsyncFlag> list = new List<AsyncFlag>();
			foreach (AsyncFlag waitResumeSimulationFlag in waitResumeSimulationFlags)
			{
				if (waitResumeSimulationFlag.flag)
				{
					list.Add(waitResumeSimulationFlag);
					flag = true;
				}
			}
			foreach (AsyncFlag item in list)
			{
				waitResumeSimulationFlags.Remove(item);
			}
		}
		if (delayResumeSimulation > 0 || waitResumeSimulationFlags.Count > 0)
		{
			PauseSimulation();
		}
		else if (delayResumeSimulation == 0 || flag)
		{
			ResumeSimulation();
		}
	}

	public void PauseSimulation(AsyncFlag waitFor)
	{
		if (waitResumeSimulationFlags == null)
		{
			waitResumeSimulationFlags = new List<AsyncFlag>();
		}
		waitResumeSimulationFlags.Add(waitFor);
		PauseSimulation();
	}

	public void PauseSimulation(int numFrames)
	{
		DelayResumeSimulation(numFrames);
		PauseSimulation();
	}

	protected void SyncOn()
	{
		_globalOn = globalEnable;
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.on = _on && globalEnable;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (_globalOn && on)
		{
			if (transformToEnableWhenOn != null)
			{
				transformToEnableWhenOn.gameObject.SetActive(value: true);
			}
			if (transformToEnableWhenOff != null)
			{
				transformToEnableWhenOff.gameObject.SetActive(value: false);
			}
			base.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: true);
			InitColliders();
		}
		else
		{
			if (transformToEnableWhenOn != null)
			{
				transformToEnableWhenOn.gameObject.SetActive(value: false);
			}
			if (transformToEnableWhenOff != null)
			{
				transformToEnableWhenOff.gameObject.SetActive(value: true);
			}
		}
	}

	protected void MTTask(object info)
	{
		DAZPhysicsMeshTaskInfo dAZPhysicsMeshTaskInfo = (DAZPhysicsMeshTaskInfo)info;
		while (_threadsRunning)
		{
			dAZPhysicsMeshTaskInfo.resetEvent.WaitOne(-1, exitContext: true);
			if (dAZPhysicsMeshTaskInfo.kill)
			{
				break;
			}
			if (dAZPhysicsMeshTaskInfo.taskType == DAZPhysicsMeshTaskType.UpdateSoftJointTargets)
			{
				Thread.Sleep(0);
				UpdateSoftJointsThreaded();
			}
			else if (dAZPhysicsMeshTaskInfo.taskType == DAZPhysicsMeshTaskType.MorphVertices)
			{
				Thread.Sleep(0);
				MorphSoftVerticesThreaded();
			}
			dAZPhysicsMeshTaskInfo.working = false;
		}
	}

	protected void StopThreads()
	{
		_threadsRunning = false;
		if (physicsMeshTask != null)
		{
			physicsMeshTask.kill = true;
			physicsMeshTask.resetEvent.Set();
			while (physicsMeshTask.thread.IsAlive)
			{
			}
			physicsMeshTask = null;
		}
	}

	protected void StartThreads()
	{
		if (useThreading && !_threadsRunning)
		{
			_threadsRunning = true;
			physicsMeshTask = new DAZPhysicsMeshTaskInfo();
			physicsMeshTask.threadIndex = 0;
			physicsMeshTask.name = "UpdateSoftJointTargetsTask";
			physicsMeshTask.resetEvent = new AutoResetEvent(initialState: false);
			physicsMeshTask.thread = new Thread(MTTask);
			physicsMeshTask.thread.Priority = System.Threading.ThreadPriority.BelowNormal;
			physicsMeshTask.taskType = DAZPhysicsMeshTaskType.UpdateSoftJointTargets;
			physicsMeshTask.thread.Start(physicsMeshTask);
		}
	}

	protected void SyncCollisionEnabled()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.collisionEnabled = _collisionEnabled;
		}
	}

	protected void SyncUseInterpolation()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.useInterpolation = _useInterpolation;
		}
	}

	protected void SyncSoftVerticesCombinedSpring()
	{
		if (!useCombinedSpringAndDamper)
		{
			return;
		}
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointSpringNormal = _softVerticesCombinedSpring;
				softVerticesGroup.jointSpringTangent = _softVerticesCombinedSpring;
				softVerticesGroup.jointSpringTangent2 = _softVerticesCombinedSpring;
			}
		}
	}

	protected void SyncSoftVerticesCombinedDamper()
	{
		if (!useCombinedSpringAndDamper)
		{
			return;
		}
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointDamperNormal = _softVerticesCombinedDamper;
				softVerticesGroup.jointDamperTangent = _softVerticesCombinedDamper;
				softVerticesGroup.jointDamperTangent2 = _softVerticesCombinedDamper;
			}
		}
	}

	protected void SyncSoftVerticesNormalSpring()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointSpringNormal = _softVerticesNormalSpring;
			}
		}
	}

	protected void SyncSoftVerticesNormalDamper()
	{
		if (useCombinedSpringAndDamper)
		{
			return;
		}
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointDamperNormal = _softVerticesNormalDamper;
			}
		}
	}

	protected void SyncSoftVerticesTangentSpring()
	{
		if (useCombinedSpringAndDamper)
		{
			return;
		}
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointSpringTangent = _softVerticesTangentSpring;
				softVerticesGroup.jointSpringTangent2 = _softVerticesTangentSpring;
			}
		}
	}

	protected void SyncSoftVerticesTangentDamper()
	{
		if (useCombinedSpringAndDamper)
		{
			return;
		}
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointDamperTangent = _softVerticesTangentDamper;
				softVerticesGroup.jointDamperTangent2 = _softVerticesTangentDamper;
			}
		}
	}

	protected void SyncSoftVerticesSpringMaxForce()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointSpringMaxForce = _softVerticesSpringMaxForce;
			}
		}
	}

	protected void SyncSoftVerticesMass()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointMass = _softVerticesMass;
			}
		}
	}

	protected void SyncSoftVerticesBackForce()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointBackForce = _softVerticesBackForce;
			}
		}
	}

	protected void SyncSoftVerticesBackForceThresholdDistance()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointBackForceThresholdDistance = _softVerticesBackForceThresholdDistance;
			}
		}
	}

	protected void SyncSoftVerticesBackForceMaxForce()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.jointBackForceMaxForce = _softVerticesBackForceMaxForce;
			}
		}
	}

	protected void SyncSoftVerticesUseUniformLimit()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.useUniformLimit = _softVerticesUseUniformLimit;
			}
		}
	}

	protected void SyncSoftVerticesTangentLimit()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.tangentDistanceLimit = _softVerticesTangentLimit;
				softVerticesGroup.tangentNegativeDistanceLimit = _softVerticesTangentLimit;
				softVerticesGroup.tangent2DistanceLimit = _softVerticesTangentLimit;
				softVerticesGroup.tangent2NegativeDistanceLimit = _softVerticesTangentLimit;
			}
		}
	}

	protected void SyncSoftVerticesNormalLimit()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.normalDistanceLimit = _softVerticesNormalLimit;
			}
		}
	}

	protected void SyncSoftVerticesNegativeNormalLimit()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings)
			{
				softVerticesGroup.normalNegativeDistanceLimit = _softVerticesNegativeNormalLimit;
			}
		}
	}

	protected void SyncSoftVerticesColliderRadius()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			if (softVerticesGroup.useParentSettings && softVerticesGroup.useParentColliderSettings)
			{
				softVerticesGroup.colliderRadius = _softVerticesColliderRadius;
				softVerticesGroup.colliderNormalOffset = _softVerticesColliderRadius;
			}
		}
	}

	protected void SoftVerticesSetAutoRadius()
	{
		if (skin != null && softVerticesUseAutoColliderRadius && softVerticesAutoColliderVertex1 != -1 && softVerticesAutoColliderVertex2 != -1)
		{
			Vector3[] visibleMorphedUVVertices = skin.dazMesh.visibleMorphedUVVertices;
			float num = (visibleMorphedUVVertices[softVerticesAutoColliderVertex1] - visibleMorphedUVVertices[softVerticesAutoColliderVertex2]).magnitude * softVerticesAutoColliderRadiusMultiplier + softVerticesAutoColliderRadiusOffset;
			if (num < softVerticesAutoColliderMinRadius)
			{
				num = softVerticesAutoColliderMinRadius;
			}
			if (num > softVerticesAutoColliderMaxRadius)
			{
				num = softVerticesAutoColliderMaxRadius;
			}
			softVerticesColliderRadius = num;
		}
	}

	public int AddHardVerticesGroup()
	{
		DAZPhysicsMeshHardVerticesGroup item = new DAZPhysicsMeshHardVerticesGroup();
		_hardVerticesGroups.Add(item);
		return _currentHardVerticesGroupIndex = _hardVerticesGroups.Count - 1;
	}

	public void RemoveHardVerticesGroup(int index)
	{
		DAZPhysicsMeshHardVerticesGroup dAZPhysicsMeshHardVerticesGroup = _hardVerticesGroups[index];
		for (int i = 0; i < dAZPhysicsMeshHardVerticesGroup.vertices.Length; i++)
		{
			int key = dAZPhysicsMeshHardVerticesGroup.vertices[i];
			_hardTargetVerticesDict.Remove(key);
		}
		_hardVerticesGroups.RemoveAt(index);
		if (_currentHardVerticesGroupIndex >= _hardVerticesGroups.Count)
		{
			_currentHardVerticesGroupIndex = _hardVerticesGroups.Count - 1;
		}
	}

	public void MoveHardVerticesGroup(int fromindex, int toindex)
	{
		if (toindex >= 0 && toindex < _hardVerticesGroups.Count)
		{
			DAZPhysicsMeshHardVerticesGroup item = _hardVerticesGroups[fromindex];
			_hardVerticesGroups.RemoveAt(fromindex);
			_hardVerticesGroups.Insert(toindex, item);
			if (_currentHardVerticesGroupIndex == fromindex)
			{
				_currentHardVerticesGroupIndex = toindex;
			}
		}
	}

	public int AddSoftVerticesGroup()
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = new DAZPhysicsMeshSoftVerticesGroup();
		_softVerticesGroups.Add(dAZPhysicsMeshSoftVerticesGroup);
		dAZPhysicsMeshSoftVerticesGroup.parent = this;
		return _currentSoftVerticesGroupIndex = _softVerticesGroups.Count - 1;
	}

	public void RemoveSoftVerticesGroup(int index)
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = _softVerticesGroups[index];
		for (int i = 0; i < dAZPhysicsMeshSoftVerticesGroup.softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = dAZPhysicsMeshSoftVerticesGroup.softVerticesSets[i];
			if (dAZPhysicsMeshSoftVerticesSet.targetVertex != -1)
			{
				_softTargetVerticesDict.Remove(dAZPhysicsMeshSoftVerticesSet.targetVertex);
			}
			if (dAZPhysicsMeshSoftVerticesSet.anchorVertex != -1 && _softAnchorVerticesDict.TryGetValue(dAZPhysicsMeshSoftVerticesSet.anchorVertex, out var value))
			{
				RemoveSoftAnchor(value, dAZPhysicsMeshSoftVerticesSet);
			}
			for (int j = 0; j < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; j++)
			{
				_softInfluenceVerticesDict.Remove(dAZPhysicsMeshSoftVerticesSet.influenceVertices[j]);
			}
		}
		_softVerticesGroups.RemoveAt(index);
		if (_currentSoftVerticesGroupIndex >= _softVerticesGroups.Count)
		{
			_currentSoftVerticesGroupIndex = _softVerticesGroups.Count - 1;
		}
	}

	public void RemoveSoftVerticesSet(int groupIndex, int setIndex)
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = _softVerticesGroups[groupIndex];
		DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = dAZPhysicsMeshSoftVerticesGroup.softVerticesSets[setIndex];
		if (dAZPhysicsMeshSoftVerticesSet.targetVertex != -1)
		{
			_softTargetVerticesDict.Remove(dAZPhysicsMeshSoftVerticesSet.targetVertex);
		}
		if (dAZPhysicsMeshSoftVerticesSet.anchorVertex != -1 && _softAnchorVerticesDict.TryGetValue(dAZPhysicsMeshSoftVerticesSet.anchorVertex, out var value))
		{
			RemoveSoftAnchor(value, dAZPhysicsMeshSoftVerticesSet);
		}
		for (int i = 0; i < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; i++)
		{
			_softInfluenceVerticesDict.Remove(dAZPhysicsMeshSoftVerticesSet.influenceVertices[i]);
		}
		dAZPhysicsMeshSoftVerticesGroup.RemoveSet(setIndex);
	}

	public void MoveSoftVerticesGroup(int fromindex, int toindex)
	{
		if (toindex >= 0 && toindex < _softVerticesGroups.Count)
		{
			DAZPhysicsMeshSoftVerticesGroup item = _softVerticesGroups[fromindex];
			_softVerticesGroups.RemoveAt(fromindex);
			_softVerticesGroups.Insert(toindex, item);
			if (_currentSoftVerticesGroupIndex == fromindex)
			{
				_currentSoftVerticesGroupIndex = toindex;
			}
		}
	}

	public int AddColliderGroup()
	{
		DAZPhysicsMeshColliderGroup item = new DAZPhysicsMeshColliderGroup();
		_colliderGroups.Add(item);
		return _currentColliderGroupIndex = _colliderGroups.Count - 1;
	}

	public void RemoveColliderGroup(int index)
	{
		_colliderGroups.RemoveAt(index);
		if (_currentColliderGroupIndex >= _colliderGroups.Count)
		{
			_currentColliderGroupIndex = _colliderGroups.Count - 1;
		}
	}

	public void MoveColliderGroup(int fromindex, int toindex)
	{
		if (toindex >= 0 && toindex < _colliderGroups.Count)
		{
			DAZPhysicsMeshColliderGroup item = _colliderGroups[fromindex];
			_colliderGroups.RemoveAt(fromindex);
			_colliderGroups.Insert(toindex, item);
			if (_currentColliderGroupIndex == fromindex)
			{
				_currentColliderGroupIndex = toindex;
			}
		}
	}

	public DAZPhysicsMeshSoftVerticesSet GetSoftSetByID(string uid)
	{
		if (_softVerticesGroups != null)
		{
			foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
			{
				DAZPhysicsMeshSoftVerticesSet setByID = softVerticesGroup.GetSetByID(uid, skipCheckParent: true);
				if (setByID != null)
				{
					return setByID;
				}
			}
		}
		return null;
	}

	protected void InitCaches(bool force = false)
	{
		if (_hardVerticesGroups == null)
		{
			_hardVerticesGroups = new List<DAZPhysicsMeshHardVerticesGroup>();
		}
		if (_softVerticesGroups == null)
		{
			_softVerticesGroups = new List<DAZPhysicsMeshSoftVerticesGroup>();
		}
		if (_colliderGroups == null)
		{
			_colliderGroups = new List<DAZPhysicsMeshColliderGroup>();
		}
		if (_hardTargetVerticesDict == null || force)
		{
			_hardTargetVerticesDict = new Dictionary<int, DAZPhysicsMeshHardVerticesGroup>();
			if (_hardVerticesGroups != null)
			{
				foreach (DAZPhysicsMeshHardVerticesGroup hardVerticesGroup in _hardVerticesGroups)
				{
					int[] vertices = hardVerticesGroup.vertices;
					foreach (int key in vertices)
					{
						_hardTargetVerticesDict.Add(key, hardVerticesGroup);
					}
				}
			}
		}
		if (skin != null && skin.dazMesh != null)
		{
			_uvVertToBaseVertDict = skin.dazMesh.uvVertToBaseVert;
		}
		else
		{
			_uvVertToBaseVertDict = new Dictionary<int, int>();
		}
		if (_softTargetVerticesDict == null || force)
		{
			_softTargetVerticesDict = new Dictionary<int, DAZPhysicsMeshSoftVerticesSet>();
			if (_softVerticesGroups != null)
			{
				foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
				{
					softVerticesGroup.parent = this;
					foreach (DAZPhysicsMeshSoftVerticesSet softVerticesSet in softVerticesGroup.softVerticesSets)
					{
						if (softVerticesSet.targetVertex != -1 && !_softTargetVerticesDict.ContainsKey(softVerticesSet.targetVertex))
						{
							_softTargetVerticesDict.Add(softVerticesSet.targetVertex, softVerticesSet);
						}
					}
				}
			}
		}
		if (_softAnchorVerticesDict == null || force)
		{
			_softAnchorVerticesDict = new Dictionary<int, List<DAZPhysicsMeshSoftVerticesSet>>();
			if (_softVerticesGroups != null)
			{
				foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup2 in _softVerticesGroups)
				{
					foreach (DAZPhysicsMeshSoftVerticesSet softVerticesSet2 in softVerticesGroup2.softVerticesSets)
					{
						if (softVerticesSet2.anchorVertex != -1)
						{
							if (_softAnchorVerticesDict.TryGetValue(softVerticesSet2.anchorVertex, out var value))
							{
								value.Add(softVerticesSet2);
								continue;
							}
							value = new List<DAZPhysicsMeshSoftVerticesSet>();
							value.Add(softVerticesSet2);
							_softAnchorVerticesDict.Add(softVerticesSet2.anchorVertex, value);
						}
					}
				}
			}
		}
		if (_softInfluenceVerticesDict != null && !force)
		{
			return;
		}
		_softInfluenceVerticesDict = new Dictionary<int, DAZPhysicsMeshSoftVerticesSet>();
		if (_softVerticesGroups == null)
		{
			return;
		}
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup3 in _softVerticesGroups)
		{
			foreach (DAZPhysicsMeshSoftVerticesSet softVerticesSet3 in softVerticesGroup3.softVerticesSets)
			{
				int[] influenceVertices = softVerticesSet3.influenceVertices;
				foreach (int key2 in influenceVertices)
				{
					if (!_softInfluenceVerticesDict.ContainsKey(key2))
					{
						_softInfluenceVerticesDict.Add(key2, softVerticesSet3);
					}
				}
			}
		}
	}

	public bool IsHardTargetVertex(int vid)
	{
		return _hardTargetVerticesDict.ContainsKey(vid);
	}

	public DAZPhysicsMeshHardVerticesGroup GetHardVertexGroup(int vid)
	{
		if (_hardTargetVerticesDict.TryGetValue(vid, out var value))
		{
			return value;
		}
		return null;
	}

	public bool IsSoftTargetVertex(int vid)
	{
		return _softTargetVerticesDict.ContainsKey(vid);
	}

	public float GetSoftTargetVertexSpringMultipler(int vid)
	{
		if (_softTargetVerticesDict.TryGetValue(vid, out var value))
		{
			return value.springMultiplier;
		}
		return 0f;
	}

	public float GetSoftTargetVertexSizeMultipler(int vid)
	{
		if (_softTargetVerticesDict.TryGetValue(vid, out var value))
		{
			return value.sizeMultiplier;
		}
		return 0f;
	}

	public float GetSoftTargetVertexLimitMultipler(int vid)
	{
		if (_softTargetVerticesDict.TryGetValue(vid, out var value))
		{
			return value.limitMultiplier;
		}
		return 0f;
	}

	public bool IsSoftAnchorVertex(int vid)
	{
		return _softAnchorVerticesDict.ContainsKey(vid);
	}

	public bool IsSoftInfluenceVertex(int vid)
	{
		bool result = false;
		List<DAZPhysicsMeshSoftVerticesSet> value;
		if (_softInfluenceVerticesDict.ContainsKey(vid))
		{
			result = true;
		}
		else if (_softAnchorVerticesDict.TryGetValue(vid, out value))
		{
			foreach (DAZPhysicsMeshSoftVerticesSet item in value)
			{
				if (item.autoInfluenceAnchor)
				{
					return true;
				}
			}
			return result;
		}
		return result;
	}

	public bool IsVertexInCurrentSoftSet(int vid)
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = currentSoftVerticesGroup;
		if (dAZPhysicsMeshSoftVerticesGroup != null)
		{
			DAZPhysicsMeshSoftVerticesSet currentSet = dAZPhysicsMeshSoftVerticesGroup.currentSet;
			List<DAZPhysicsMeshSoftVerticesSet> value2;
			if (_softInfluenceVerticesDict.TryGetValue(vid, out var value))
			{
				if (value == currentSet)
				{
					return true;
				}
			}
			else if (_softTargetVerticesDict.TryGetValue(vid, out value))
			{
				if (value == currentSet)
				{
					return true;
				}
			}
			else if (_softAnchorVerticesDict.TryGetValue(vid, out value2) && value2.Contains(currentSet))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsVertexCurrentSoftSetAnchor(int vid)
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = currentSoftVerticesGroup;
		if (dAZPhysicsMeshSoftVerticesGroup != null)
		{
			DAZPhysicsMeshSoftVerticesSet currentSet = dAZPhysicsMeshSoftVerticesGroup.currentSet;
			if (_softAnchorVerticesDict.TryGetValue(vid, out var value) && value.Contains(currentSet))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsVertexInCurrentHardGroup(int vid)
	{
		DAZPhysicsMeshHardVerticesGroup dAZPhysicsMeshHardVerticesGroup = currentHardVerticesGroup;
		if (dAZPhysicsMeshHardVerticesGroup != null && _hardTargetVerticesDict.TryGetValue(vid, out var value) && dAZPhysicsMeshHardVerticesGroup == value)
		{
			return true;
		}
		return false;
	}

	public void DrawLinkLines()
	{
		Color color = new Color(0.8f, 0.8f, 0.8f);
		Color color2 = new Color(0.5f, 0.5f, 0.5f);
		if (_softVerticesGroups == null || !(skin != null) || !(skin.dazMesh != null))
		{
			return;
		}
		Vector3[] array;
		Vector3[] array2;
		if (Application.isPlaying)
		{
			array = skin.drawVerts;
			array2 = skin.drawNormals;
		}
		else
		{
			array = skin.dazMesh.morphedUVVertices;
			array2 = skin.dazMesh.morphedUVNormals;
		}
		for (int i = 0; i < _softVerticesGroups.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = _softVerticesGroups[i];
			if (((_showCurrentSoftGroupOnly || _showCurrentSoftSetOnly) && i != _currentSoftVerticesGroupIndex) || !dAZPhysicsMeshSoftVerticesGroup.useLinkJoints)
			{
				continue;
			}
			foreach (DAZPhysicsMeshSoftVerticesSet softVerticesSet in dAZPhysicsMeshSoftVerticesGroup.softVerticesSets)
			{
				if ((_showCurrentSoftSetOnly && softVerticesSet != dAZPhysicsMeshSoftVerticesGroup.currentSet) || softVerticesSet.targetVertex == -1 || softVerticesSet.anchorVertex == -1)
				{
					continue;
				}
				Vector3 vector = array[softVerticesSet.targetVertex];
				if (softVerticesSet.links == null)
				{
					continue;
				}
				for (int j = 0; j < softVerticesSet.links.Count; j++)
				{
					DAZPhysicsMeshSoftVerticesSet softSetByID = GetSoftSetByID(softVerticesSet.links[j]);
					if (softSetByID != null && softSetByID.targetVertex != -1)
					{
						Debug.DrawLine(vector, array[softSetByID.targetVertex], Color.yellow);
						Debug.DrawLine((vector + 3f * array[softSetByID.targetVertex]) * 0.25f + array2[softVerticesSet.targetVertex] * 0.003f, array[softSetByID.targetVertex], Color.green);
					}
					else
					{
						Debug.LogError("Soft vertices set " + softVerticesSet.uid + " has broken link to " + softVerticesSet.links[j]);
					}
				}
			}
		}
	}

	protected int FindOrCreateHardGroup(DAZBone db)
	{
		DAZPhysicsMeshHardVerticesGroup dAZPhysicsMeshHardVerticesGroup;
		for (int i = 0; i < _hardVerticesGroups.Count; i++)
		{
			dAZPhysicsMeshHardVerticesGroup = _hardVerticesGroups[i];
			if (dAZPhysicsMeshHardVerticesGroup.bone == db)
			{
				return i;
			}
		}
		int num = AddHardVerticesGroup();
		dAZPhysicsMeshHardVerticesGroup = _hardVerticesGroups[num];
		dAZPhysicsMeshHardVerticesGroup.bone = db;
		dAZPhysicsMeshHardVerticesGroup.name = db.name;
		return num;
	}

	public void ToggleHardVertices(int vid, bool auto = false)
	{
		if (auto)
		{
			DAZBone dAZBone = skin.strongestDAZBone[vid];
			if (dAZBone == null)
			{
				Debug.LogError("Could not find DAZBone for vertex " + vid);
				return;
			}
			_currentHardVerticesGroupIndex = FindOrCreateHardGroup(dAZBone);
		}
		DAZPhysicsMeshHardVerticesGroup dAZPhysicsMeshHardVerticesGroup = currentHardVerticesGroup;
		if (dAZPhysicsMeshHardVerticesGroup == null)
		{
			return;
		}
		if (_hardTargetVerticesDict.TryGetValue(vid, out var value))
		{
			if (value != dAZPhysicsMeshHardVerticesGroup)
			{
				value.RemoveVertex(vid);
				_hardTargetVerticesDict.Remove(vid);
				dAZPhysicsMeshHardVerticesGroup.AddVertex(vid);
				_hardTargetVerticesDict.Add(vid, dAZPhysicsMeshHardVerticesGroup);
			}
			else
			{
				value.RemoveVertex(vid);
				_hardTargetVerticesDict.Remove(vid);
			}
		}
		else
		{
			dAZPhysicsMeshHardVerticesGroup.AddVertex(vid);
			_hardTargetVerticesDict.Add(vid, dAZPhysicsMeshHardVerticesGroup);
		}
	}

	public void OnHardVertices(int vid, bool auto = false)
	{
		if (auto)
		{
			DAZBone dAZBone = skin.strongestDAZBone[vid];
			if (dAZBone == null)
			{
				Debug.LogError("Could not find DAZBone for vertex " + vid);
				return;
			}
			_currentHardVerticesGroupIndex = FindOrCreateHardGroup(dAZBone);
		}
		DAZPhysicsMeshHardVerticesGroup dAZPhysicsMeshHardVerticesGroup = currentHardVerticesGroup;
		if (dAZPhysicsMeshHardVerticesGroup == null)
		{
			return;
		}
		if (_hardTargetVerticesDict.TryGetValue(vid, out var value))
		{
			if (value != dAZPhysicsMeshHardVerticesGroup)
			{
				value.RemoveVertex(vid);
				_hardTargetVerticesDict.Remove(vid);
				dAZPhysicsMeshHardVerticesGroup.AddVertex(vid);
				_hardTargetVerticesDict.Add(vid, dAZPhysicsMeshHardVerticesGroup);
			}
		}
		else
		{
			dAZPhysicsMeshHardVerticesGroup.AddVertex(vid);
			_hardTargetVerticesDict.Add(vid, dAZPhysicsMeshHardVerticesGroup);
		}
	}

	public void OffHardVertices(int vid, bool auto = false)
	{
		DAZPhysicsMeshHardVerticesGroup dAZPhysicsMeshHardVerticesGroup = currentHardVerticesGroup;
		if (dAZPhysicsMeshHardVerticesGroup != null && _hardTargetVerticesDict.TryGetValue(vid, out var value))
		{
			if (auto)
			{
				value.RemoveVertex(vid);
				_hardTargetVerticesDict.Remove(vid);
			}
			else if (value == dAZPhysicsMeshHardVerticesGroup)
			{
				value.RemoveVertex(vid);
				_hardTargetVerticesDict.Remove(vid);
			}
		}
	}

	public void ToggleSoftTargetVertex(int vid)
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = currentSoftVerticesGroup;
		if (dAZPhysicsMeshSoftVerticesGroup == null)
		{
			return;
		}
		DAZPhysicsMeshSoftVerticesSet currentSet = dAZPhysicsMeshSoftVerticesGroup.currentSet;
		if (currentSet == null)
		{
			return;
		}
		if (_softTargetVerticesDict.TryGetValue(vid, out var value))
		{
			if (value != currentSet)
			{
				value.targetVertex = -1;
				_softTargetVerticesDict.Remove(vid);
				if (currentSet.targetVertex != -1)
				{
					_softTargetVerticesDict.Remove(currentSet.targetVertex);
				}
				currentSet.targetVertex = vid;
				_softTargetVerticesDict.Add(vid, currentSet);
			}
			else
			{
				value.targetVertex = -1;
				_softTargetVerticesDict.Remove(vid);
			}
		}
		else
		{
			if (currentSet.targetVertex != -1)
			{
				_softTargetVerticesDict.Remove(currentSet.targetVertex);
			}
			currentSet.targetVertex = vid;
			_softTargetVerticesDict.Add(vid, currentSet);
		}
	}

	protected bool RemoveSoftAnchor(List<DAZPhysicsMeshSoftVerticesSet> ssl, DAZPhysicsMeshSoftVerticesSet ss)
	{
		if (ssl.Contains(ss))
		{
			ssl.Remove(ss);
			int anchorVertex = ss.anchorVertex;
			ss.anchorVertex = -1;
			if (ssl.Count == 0)
			{
				_softAnchorVerticesDict.Remove(anchorVertex);
			}
			return true;
		}
		return false;
	}

	protected void AddSoftAnchor(int vid, DAZPhysicsMeshSoftVerticesSet ss)
	{
		if (ss.anchorVertex != -1 && _softAnchorVerticesDict.TryGetValue(ss.anchorVertex, out var value))
		{
			RemoveSoftAnchor(value, ss);
		}
		ss.anchorVertex = vid;
		if (_softAnchorVerticesDict.TryGetValue(vid, out value))
		{
			if (!value.Contains(ss))
			{
				value.Add(ss);
			}
		}
		else
		{
			value = new List<DAZPhysicsMeshSoftVerticesSet>();
			value.Add(ss);
			_softAnchorVerticesDict.Add(vid, value);
		}
	}

	public void ToggleSoftAnchorVertex(int vid)
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = currentSoftVerticesGroup;
		if (dAZPhysicsMeshSoftVerticesGroup == null)
		{
			return;
		}
		DAZPhysicsMeshSoftVerticesSet currentSet = dAZPhysicsMeshSoftVerticesGroup.currentSet;
		if (currentSet == null)
		{
			return;
		}
		if (_softAnchorVerticesDict.TryGetValue(vid, out var value))
		{
			if (!RemoveSoftAnchor(value, currentSet))
			{
				AddSoftAnchor(vid, currentSet);
			}
		}
		else
		{
			AddSoftAnchor(vid, currentSet);
		}
	}

	public void ToggleSoftInfluenceVertices(int vid)
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = currentSoftVerticesGroup;
		if (dAZPhysicsMeshSoftVerticesGroup == null)
		{
			return;
		}
		DAZPhysicsMeshSoftVerticesSet currentSet = dAZPhysicsMeshSoftVerticesGroup.currentSet;
		if (currentSet == null)
		{
			return;
		}
		if (_softInfluenceVerticesDict.TryGetValue(vid, out var value))
		{
			if (value != currentSet)
			{
				value.RemoveInfluenceVertex(vid);
				_softInfluenceVerticesDict.Remove(vid);
				currentSet.AddInfluenceVertex(vid);
				_softInfluenceVerticesDict.Add(vid, currentSet);
			}
			else
			{
				value.RemoveInfluenceVertex(vid);
				_softInfluenceVerticesDict.Remove(vid);
			}
		}
		else
		{
			currentSet.AddInfluenceVertex(vid);
			_softInfluenceVerticesDict.Add(vid, currentSet);
		}
	}

	public void OnSoftInfluenceVertices(int vid)
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = currentSoftVerticesGroup;
		if (dAZPhysicsMeshSoftVerticesGroup == null || _softTargetVerticesDict.ContainsKey(vid))
		{
			return;
		}
		DAZPhysicsMeshSoftVerticesSet currentSet = dAZPhysicsMeshSoftVerticesGroup.currentSet;
		if (currentSet == null)
		{
			return;
		}
		if (_softInfluenceVerticesDict.TryGetValue(vid, out var value))
		{
			if (value != currentSet)
			{
				value.RemoveInfluenceVertex(vid);
				_softInfluenceVerticesDict.Remove(vid);
				currentSet.AddInfluenceVertex(vid);
				_softInfluenceVerticesDict.Add(vid, currentSet);
			}
		}
		else
		{
			currentSet.AddInfluenceVertex(vid);
			_softInfluenceVerticesDict.Add(vid, currentSet);
		}
	}

	public void OffSoftInfluenceVertices(int vid)
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = currentSoftVerticesGroup;
		if (dAZPhysicsMeshSoftVerticesGroup != null)
		{
			DAZPhysicsMeshSoftVerticesSet currentSet = dAZPhysicsMeshSoftVerticesGroup.currentSet;
			if (currentSet != null && _softInfluenceVerticesDict.TryGetValue(vid, out var value) && value == currentSet)
			{
				value.RemoveInfluenceVertex(vid);
				_softInfluenceVerticesDict.Remove(vid);
			}
		}
	}

	public void SoftAutoRadius(int vid)
	{
		if (softVerticesUseAutoColliderRadius)
		{
			if (softVerticesAutoColliderVertex1 == vid)
			{
				softVerticesAutoColliderVertex1 = -1;
			}
			else if (softVerticesAutoColliderVertex2 == vid)
			{
				softVerticesAutoColliderVertex2 = -1;
			}
			else if (softVerticesAutoColliderVertex1 == -1)
			{
				softVerticesAutoColliderVertex1 = vid;
			}
			else if (softVerticesAutoColliderVertex2 == -1)
			{
				softVerticesAutoColliderVertex2 = vid;
			}
			SoftVerticesSetAutoRadius();
		}
	}

	public void SoftSelect(int vid)
	{
		if (_softTargetVerticesDict.TryGetValue(vid, out var value) && currentSoftVerticesGroup != null)
		{
			currentSoftVerticesGroup.currentSet = value;
		}
	}

	public void SoftSpringSet(int vid)
	{
		if (_softTargetVerticesDict.TryGetValue(vid, out var value))
		{
			value.springMultiplier = _softSpringMultiplierSetValue;
			if (currentSoftVerticesGroup != null)
			{
				currentSoftVerticesGroup.currentSet = value;
			}
		}
	}

	public void SoftSizeSet(int vid)
	{
		if (_softTargetVerticesDict.TryGetValue(vid, out var value))
		{
			value.sizeMultiplier = _softSizeMultiplierSetValue;
			if (currentSoftVerticesGroup != null)
			{
				currentSoftVerticesGroup.currentSet = value;
			}
		}
	}

	public void SoftLimitSet(int vid)
	{
		if (_softTargetVerticesDict.TryGetValue(vid, out var value))
		{
			value.limitMultiplier = _softLimitMultiplierSetValue;
			if (currentSoftVerticesGroup != null)
			{
				currentSoftVerticesGroup.currentSet = value;
			}
		}
	}

	public void AutoSoftVertex(int vid)
	{
		DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = currentSoftVerticesGroup;
		if (dAZPhysicsMeshSoftVerticesGroup == null)
		{
			AddSoftVerticesGroup();
			dAZPhysicsMeshSoftVerticesGroup = currentSoftVerticesGroup;
		}
		DAZPhysicsMeshSoftVerticesSet currentSet = dAZPhysicsMeshSoftVerticesGroup.currentSet;
		if (currentSet == null)
		{
			dAZPhysicsMeshSoftVerticesGroup.AddSet();
			currentSet = dAZPhysicsMeshSoftVerticesGroup.currentSet;
		}
		DAZPhysicsMeshSoftVerticesSet value;
		List<DAZPhysicsMeshSoftVerticesSet> value2;
		if (currentSet.targetVertex == -1)
		{
			if (_softTargetVerticesDict.TryGetValue(vid, out value))
			{
				value.targetVertex = -1;
				_softTargetVerticesDict.Remove(vid);
			}
			currentSet.targetVertex = vid;
			_softTargetVerticesDict.Add(vid, currentSet);
		}
		else if (currentSet.anchorVertex == -1)
		{
			AddSoftAnchor(vid, currentSet);
		}
		else if (_softAnchorVerticesDict.TryGetValue(vid, out value2) && value2.Contains(currentSet))
		{
			RemoveSoftAnchor(value2, currentSet);
		}
		else if (_softTargetVerticesDict.TryGetValue(vid, out value))
		{
			if (dAZPhysicsMeshSoftVerticesGroup.currentSet != value)
			{
				dAZPhysicsMeshSoftVerticesGroup.currentSet = value;
				return;
			}
			value.targetVertex = -1;
			_softTargetVerticesDict.Remove(vid);
		}
		else if (_softInfluenceVerticesDict.TryGetValue(vid, out value))
		{
			dAZPhysicsMeshSoftVerticesGroup.currentSet = value;
			value.RemoveInfluenceVertex(vid);
			_softInfluenceVerticesDict.Remove(vid);
		}
		else
		{
			currentSet.AddInfluenceVertex(vid);
			_softInfluenceVerticesDict.Add(vid, currentSet);
		}
	}

	public void StartSoftLink(int vid)
	{
		if (!_softTargetVerticesDict.TryGetValue(vid, out startSoftLinkSet))
		{
		}
	}

	public void EndSoftLink(int vid)
	{
		if (startSoftLinkSet != null && _softTargetVerticesDict.TryGetValue(vid, out var value) && !startSoftLinkSet.links.Remove(value.uid) && startSoftLinkSet.uid != value.uid)
		{
			startSoftLinkSet.links.Add(value.uid);
		}
	}

	public void ClearLinks(DAZPhysicsMeshSoftVerticesSet ss)
	{
		currentSoftVerticesGroup.ClearLinks(ss);
	}

	public void ClickVertex(int vid)
	{
		if (_uvVertToBaseVertDict.TryGetValue(vid, out var value))
		{
			vid = value;
		}
		switch (_selectionMode)
		{
		case SelectionMode.HardTarget:
			ToggleHardVertices(vid);
			break;
		case SelectionMode.HardTargetAuto:
			ToggleHardVertices(vid, auto: true);
			break;
		case SelectionMode.SoftTarget:
			ToggleSoftTargetVertex(vid);
			break;
		case SelectionMode.SoftAnchor:
			ToggleSoftAnchorVertex(vid);
			break;
		case SelectionMode.SoftInfluenced:
			ToggleSoftInfluenceVertices(vid);
			break;
		case SelectionMode.SoftAuto:
			AutoSoftVertex(vid);
			break;
		case SelectionMode.SoftLink:
			StartSoftLink(vid);
			break;
		case SelectionMode.SoftSelect:
			SoftSelect(vid);
			break;
		case SelectionMode.SoftSpringSet:
			SoftSpringSet(vid);
			break;
		case SelectionMode.SoftSizeSet:
			SoftSizeSet(vid);
			break;
		case SelectionMode.SoftLimitSet:
			SoftLimitSet(vid);
			break;
		case SelectionMode.SoftAutoRadius:
			SoftAutoRadius(vid);
			break;
		case SelectionMode.ColliderEditEnd1:
		case SelectionMode.ColliderEditEnd2:
		case SelectionMode.ColliderEditFront:
			break;
		}
	}

	public void UpclickVertex(int vid)
	{
		if (_uvVertToBaseVertDict.TryGetValue(vid, out var value))
		{
			vid = value;
		}
		SelectionMode selectionMode = _selectionMode;
		if (selectionMode == SelectionMode.SoftLink)
		{
			EndSoftLink(vid);
		}
	}

	public void OnVertex(int vid)
	{
		if (_uvVertToBaseVertDict.TryGetValue(vid, out var value))
		{
			vid = value;
		}
		switch (_selectionMode)
		{
		case SelectionMode.HardTarget:
			OnHardVertices(vid);
			break;
		case SelectionMode.HardTargetAuto:
			OnHardVertices(vid, auto: true);
			break;
		case SelectionMode.SoftTarget:
			break;
		case SelectionMode.SoftAnchor:
			break;
		case SelectionMode.SoftInfluenced:
		case SelectionMode.SoftAuto:
			OnSoftInfluenceVertices(vid);
			break;
		}
	}

	public void OffVertex(int vid)
	{
		if (_uvVertToBaseVertDict.TryGetValue(vid, out var value))
		{
			vid = value;
		}
		switch (_selectionMode)
		{
		case SelectionMode.HardTarget:
			OffHardVertices(vid);
			break;
		case SelectionMode.HardTargetAuto:
			OffHardVertices(vid, auto: true);
			break;
		case SelectionMode.SoftTarget:
			break;
		case SelectionMode.SoftAnchor:
			break;
		case SelectionMode.SoftInfluenced:
		case SelectionMode.SoftAuto:
			OffSoftInfluenceVertices(vid);
			break;
		}
	}

	public int GetBaseVertex(int vid)
	{
		if (_uvVertToBaseVertDict.TryGetValue(vid, out var value))
		{
			vid = value;
		}
		return vid;
	}

	public bool IsBaseVertex(int vid)
	{
		if (_uvVertToBaseVertDict != null)
		{
			return !_uvVertToBaseVertDict.ContainsKey(vid);
		}
		return true;
	}

	protected void CreateHardVerticesColliders()
	{
		foreach (DAZPhysicsMeshHardVerticesGroup hardVerticesGroup in _hardVerticesGroups)
		{
			hardVerticesGroup.CreateColliders(base.transform, _skin);
		}
	}

	protected void UpdateHardVerticesColliders()
	{
		foreach (DAZPhysicsMeshHardVerticesGroup hardVerticesGroup in _hardVerticesGroups)
		{
			hardVerticesGroup.UpdateColliders();
		}
	}

	protected void InitSoftJoints()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.Init(base.transform, skin);
		}
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup2 in _softVerticesGroups)
		{
			softVerticesGroup2.InitPass2();
		}
	}

	protected void ResetSoftJoints()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.ResetJoints();
		}
	}

	protected void UpdateSimulationSoftJoints()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			if (softVerticesGroup.useSimulation)
			{
				softVerticesGroup.UpdateJoints();
			}
		}
	}

	protected void UpdateNonSimulationSoftJoints()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			if (!softVerticesGroup.useSimulation)
			{
				softVerticesGroup.UpdateJoints();
			}
		}
	}

	protected void PrepareSoftUpdateJointsThreaded()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.PrepareUpdateJointsThreaded();
		}
	}

	protected void UpdateSoftJointsThreaded()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.UpdateJointTargetsThreaded();
		}
	}

	protected void ApplySoftJointBackForces()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.ResetAdjustJoints();
		}
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup2 in _softVerticesGroups)
		{
			softVerticesGroup2.ApplyBackForce();
		}
	}

	protected void MorphSoftVertices(bool updateRB)
	{
		float num = Time.time - Time.fixedTime;
		float interpFactor = num / Time.fixedDeltaTime;
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.MorphVertices(interpFactor, updateRB);
		}
	}

	protected void PrepareSoftMorphVerticesThreaded(bool updateRB)
	{
		float num = Time.time - Time.fixedTime;
		float interpFactor = num / Time.fixedDeltaTime;
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.PrepareMorphVerticesThreaded(interpFactor, updateRB);
		}
	}

	protected void MorphSoftVerticesThreaded()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.MorphVerticesThreaded();
		}
	}

	protected void RecalculateLinkJoints()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.AdjustInitialTargetPositions();
		}
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup2 in _softVerticesGroups)
		{
			softVerticesGroup2.AdjustLinkJointDistances();
		}
	}

	public void InitColliders()
	{
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in _softVerticesGroups)
		{
			softVerticesGroup.InitColliders();
		}
		if (_softVerticesGroups.Count > 1)
		{
			for (int i = 0; i < _softVerticesGroups.Count - 1; i++)
			{
				DAZPhysicsMeshSoftVerticesGroup dAZPhysicsMeshSoftVerticesGroup = _softVerticesGroups[i];
				for (int j = i + 1; j < _softVerticesGroups.Count; j++)
				{
					DAZPhysicsMeshSoftVerticesGroup otherGroup = _softVerticesGroups[j];
					dAZPhysicsMeshSoftVerticesGroup.IgnoreOtherSoftGroupColliders(otherGroup);
				}
			}
		}
		DAZPhysicsMesh[] array = ignorePhysicsMeshes;
		foreach (DAZPhysicsMesh dAZPhysicsMesh in array)
		{
			foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup2 in _softVerticesGroups)
			{
				foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup3 in dAZPhysicsMesh.softVerticesGroups)
				{
					softVerticesGroup2.IgnoreOtherSoftGroupColliders(softVerticesGroup3);
				}
			}
		}
		foreach (DAZPhysicsMeshHardVerticesGroup hardVerticesGroup in _hardVerticesGroups)
		{
			hardVerticesGroup.InitColliders();
		}
	}

	protected void InitUI()
	{
		if (onToggle != null)
		{
			onToggle.onValueChanged.AddListener(delegate
			{
				on = onToggle.isOn;
			});
			onToggle.isOn = on;
			SyncOn();
		}
		if (useCombinedSpringAndDamper)
		{
			if (softVerticesCombinedSpringSlider != null)
			{
				softVerticesCombinedSpringSlider.value = _softVerticesCombinedSpring;
				softVerticesCombinedSpringSlider.onValueChanged.AddListener(delegate
				{
					softVerticesCombinedSpring = softVerticesCombinedSpringSlider.value;
				});
				SliderControl component = softVerticesCombinedSpringSlider.GetComponent<SliderControl>();
				if (component != null)
				{
					component.defaultValue = _softVerticesCombinedSpring;
				}
				SyncSoftVerticesCombinedSpring();
			}
			if (softVerticesCombinedSpringSliderAlt != null)
			{
				softVerticesCombinedSpringSliderAlt.value = _softVerticesCombinedSpring;
				softVerticesCombinedSpringSliderAlt.onValueChanged.AddListener(delegate
				{
					softVerticesCombinedSpring = softVerticesCombinedSpringSliderAlt.value;
				});
				SliderControl component2 = softVerticesCombinedSpringSliderAlt.GetComponent<SliderControl>();
				if (component2 != null)
				{
					component2.defaultValue = _softVerticesCombinedSpring;
				}
				SyncSoftVerticesCombinedSpring();
			}
			if (softVerticesCombinedDamperSlider != null)
			{
				softVerticesCombinedDamperSlider.value = _softVerticesCombinedDamper;
				softVerticesCombinedDamperSlider.onValueChanged.AddListener(delegate
				{
					softVerticesCombinedDamper = softVerticesCombinedDamperSlider.value;
				});
				SliderControl component3 = softVerticesCombinedDamperSlider.GetComponent<SliderControl>();
				if (component3 != null)
				{
					component3.defaultValue = _softVerticesCombinedDamper;
				}
				SyncSoftVerticesCombinedDamper();
			}
			if (softVerticesCombinedDamperSliderAlt != null)
			{
				softVerticesCombinedDamperSliderAlt.value = _softVerticesCombinedDamper;
				softVerticesCombinedDamperSliderAlt.onValueChanged.AddListener(delegate
				{
					softVerticesCombinedDamper = softVerticesCombinedDamperSliderAlt.value;
				});
				SliderControl component4 = softVerticesCombinedDamperSliderAlt.GetComponent<SliderControl>();
				if (component4 != null)
				{
					component4.defaultValue = _softVerticesCombinedDamper;
				}
			}
		}
		else
		{
			if (softVerticesNormalSpringSlider != null)
			{
				softVerticesNormalSpringSlider.value = _softVerticesNormalSpring;
				softVerticesNormalSpringSlider.onValueChanged.AddListener(delegate
				{
					softVerticesNormalSpring = softVerticesNormalSpringSlider.value;
				});
				SliderControl component5 = softVerticesNormalSpringSlider.GetComponent<SliderControl>();
				if (component5 != null)
				{
					component5.defaultValue = _softVerticesNormalSpring;
				}
				SyncSoftVerticesNormalSpring();
			}
			if (softVerticesNormalDamperSlider != null)
			{
				softVerticesNormalDamperSlider.value = _softVerticesNormalDamper;
				softVerticesNormalDamperSlider.onValueChanged.AddListener(delegate
				{
					softVerticesNormalDamper = softVerticesNormalDamperSlider.value;
				});
				SliderControl component6 = softVerticesNormalDamperSlider.GetComponent<SliderControl>();
				if (component6 != null)
				{
					component6.defaultValue = _softVerticesNormalDamper;
				}
				SyncSoftVerticesNormalDamper();
			}
			if (softVerticesTangentSpringSlider != null)
			{
				softVerticesTangentSpringSlider.value = _softVerticesTangentSpring;
				softVerticesTangentSpringSlider.onValueChanged.AddListener(delegate
				{
					softVerticesTangentSpring = softVerticesTangentSpringSlider.value;
				});
				SliderControl component7 = softVerticesTangentSpringSlider.GetComponent<SliderControl>();
				if (component7 != null)
				{
					component7.defaultValue = _softVerticesTangentSpring;
				}
				SyncSoftVerticesTangentSpring();
			}
			if (softVerticesTangentDamperSlider != null)
			{
				softVerticesTangentDamperSlider.value = _softVerticesTangentDamper;
				softVerticesTangentDamperSlider.onValueChanged.AddListener(delegate
				{
					softVerticesTangentDamper = softVerticesTangentDamperSlider.value;
				});
				SliderControl component8 = softVerticesTangentDamperSlider.GetComponent<SliderControl>();
				if (component8 != null)
				{
					component8.defaultValue = _softVerticesTangentDamper;
				}
				SyncSoftVerticesTangentDamper();
			}
		}
		if (softVerticesSpringMaxForceSlider != null)
		{
			softVerticesSpringMaxForceSlider.value = _softVerticesSpringMaxForce;
			softVerticesSpringMaxForceSlider.onValueChanged.AddListener(delegate
			{
				softVerticesSpringMaxForce = softVerticesSpringMaxForceSlider.value;
			});
			SliderControl component9 = softVerticesSpringMaxForceSlider.GetComponent<SliderControl>();
			if (component9 != null)
			{
				component9.defaultValue = _softVerticesSpringMaxForce;
			}
			SyncSoftVerticesSpringMaxForce();
		}
		if (softVerticesMassSlider != null)
		{
			softVerticesMassSlider.value = _softVerticesMass;
			softVerticesMassSlider.onValueChanged.AddListener(delegate
			{
				softVerticesMass = softVerticesMassSlider.value;
			});
			SliderControl component10 = softVerticesMassSlider.GetComponent<SliderControl>();
			if (component10 != null)
			{
				component10.defaultValue = _softVerticesMass;
			}
			SyncSoftVerticesMass();
		}
		if (softVerticesMassSliderAlt != null)
		{
			softVerticesMassSliderAlt.value = _softVerticesMass;
			softVerticesMassSliderAlt.onValueChanged.AddListener(delegate
			{
				softVerticesMass = softVerticesMassSliderAlt.value;
			});
			SliderControl component11 = softVerticesMassSliderAlt.GetComponent<SliderControl>();
			if (component11 != null)
			{
				component11.defaultValue = _softVerticesMass;
			}
		}
		if (softVerticesBackForceSlider != null)
		{
			softVerticesBackForceSlider.value = _softVerticesBackForce;
			softVerticesBackForceSlider.onValueChanged.AddListener(delegate
			{
				softVerticesBackForce = softVerticesBackForceSlider.value;
			});
			SliderControl component12 = softVerticesBackForceSlider.GetComponent<SliderControl>();
			if (component12 != null)
			{
				component12.defaultValue = _softVerticesBackForce;
			}
			SyncSoftVerticesBackForce();
		}
		if (softVerticesBackForceThresholdDistanceSlider != null)
		{
			softVerticesBackForceThresholdDistanceSlider.value = _softVerticesBackForceThresholdDistance;
			softVerticesBackForceThresholdDistanceSlider.onValueChanged.AddListener(delegate
			{
				softVerticesBackForceThresholdDistance = softVerticesBackForceThresholdDistanceSlider.value;
			});
			SliderControl component13 = softVerticesBackForceThresholdDistanceSlider.GetComponent<SliderControl>();
			if (component13 != null)
			{
				component13.defaultValue = _softVerticesBackForceThresholdDistance;
			}
			SyncSoftVerticesBackForceThresholdDistance();
		}
		if (softVerticesBackForceMaxForceSlider != null)
		{
			softVerticesBackForceMaxForceSlider.value = _softVerticesBackForceMaxForce;
			softVerticesBackForceMaxForceSlider.onValueChanged.AddListener(delegate
			{
				softVerticesBackForceMaxForce = softVerticesBackForceMaxForceSlider.value;
			});
			SliderControl component14 = softVerticesBackForceMaxForceSlider.GetComponent<SliderControl>();
			if (component14 != null)
			{
				component14.defaultValue = _softVerticesBackForceMaxForce;
			}
			SyncSoftVerticesBackForceMaxForce();
		}
		if (softVerticesColliderRadiusSlider != null)
		{
			softVerticesColliderRadiusSlider.value = _softVerticesColliderRadius;
			softVerticesColliderRadiusSlider.onValueChanged.AddListener(delegate
			{
				softVerticesColliderRadius = softVerticesColliderRadiusSlider.value;
			});
			SliderControl component15 = softVerticesColliderRadiusSlider.GetComponent<SliderControl>();
			if (component15 != null)
			{
				component15.defaultValue = _softVerticesColliderRadius;
			}
			SyncSoftVerticesColliderRadius();
		}
		if (softVerticesUseUniformLimitToggle != null)
		{
			softVerticesUseUniformLimitToggle.onValueChanged.AddListener(delegate
			{
				softVerticesUseUniformLimit = softVerticesUseUniformLimitToggle.isOn;
			});
			softVerticesUseUniformLimitToggle.isOn = _softVerticesUseUniformLimit;
			SyncSoftVerticesUseUniformLimit();
		}
		if (softVerticesNormalLimitSlider != null)
		{
			softVerticesNormalLimitSlider.value = _softVerticesNormalLimit;
			softVerticesNormalLimitSlider.onValueChanged.AddListener(delegate
			{
				softVerticesNormalLimit = softVerticesNormalLimitSlider.value;
			});
			SliderControl component16 = softVerticesNormalLimitSlider.GetComponent<SliderControl>();
			if (component16 != null)
			{
				component16.defaultValue = _softVerticesNormalLimit;
			}
			SyncSoftVerticesNormalLimit();
		}
		if (softVerticesNegativeNormalLimitSlider != null)
		{
			softVerticesNegativeNormalLimitSlider.value = _softVerticesNegativeNormalLimit;
			softVerticesNegativeNormalLimitSlider.onValueChanged.AddListener(delegate
			{
				softVerticesNegativeNormalLimit = softVerticesNegativeNormalLimitSlider.value;
			});
			SliderControl component17 = softVerticesNegativeNormalLimitSlider.GetComponent<SliderControl>();
			if (component17 != null)
			{
				component17.defaultValue = _softVerticesNegativeNormalLimit;
			}
			SyncSoftVerticesNegativeNormalLimit();
		}
		if (softVerticesTangentLimitSlider != null)
		{
			softVerticesTangentLimitSlider.value = _softVerticesTangentLimit;
			softVerticesTangentLimitSlider.onValueChanged.AddListener(delegate
			{
				softVerticesTangentLimit = softVerticesTangentLimitSlider.value;
			});
			SliderControl component18 = softVerticesTangentLimitSlider.GetComponent<SliderControl>();
			if (component18 != null)
			{
				component18.defaultValue = _softVerticesTangentLimit;
			}
			SyncSoftVerticesTangentLimit();
		}
	}

	public void Init()
	{
		if (_wasInit || !(_skin != null))
		{
			return;
		}
		_wasInit = true;
		if (Application.isPlaying)
		{
			DAZPhysicsMeshEarlyUpdate component = GetComponent<DAZPhysicsMeshEarlyUpdate>();
			if (component == null)
			{
				component = base.gameObject.AddComponent<DAZPhysicsMeshEarlyUpdate>();
				component.dazPhysicsMesh = this;
			}
			InitUI();
			_skin.Init();
			InitSoftJoints();
			CreateHardVerticesColliders();
			InitColliders();
			DelayResumeSimulation(5);
		}
		else if (_skin.dazMesh != null)
		{
			_skin.Init();
			_baseVertToUVVertFullMap = _skin.dazMesh.baseVertToUVVertFullMap;
		}
	}

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			InitColliders();
		}
		if (Application.isEditor)
		{
			InitCaches(force: true);
		}
	}

	private void Start()
	{
		Init();
	}

	private void LateUpdate()
	{
		if (!_wasInit || !Application.isPlaying)
		{
			return;
		}
		foreach (DAZPhysicsMeshSoftVerticesGroup softVerticesGroup in softVerticesGroups)
		{
			softVerticesGroup.useThreading = useThreading;
		}
		if (useThreading)
		{
			StartThreads();
			while (physicsMeshTask.working)
			{
				Thread.Sleep(0);
			}
			PrepareSoftUpdateJointsThreaded();
			physicsMeshTask.taskType = DAZPhysicsMeshTaskType.UpdateSoftJointTargets;
			physicsMeshTask.working = true;
			physicsMeshTask.resetEvent.Set();
		}
	}

	private void FixedUpdate()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (_globalOn != globalEnable)
		{
			SyncOn();
		}
		UpdateHardVerticesColliders();
		if (useThreading)
		{
			if (physicsMeshTask != null)
			{
				while (physicsMeshTask.working)
				{
					Thread.Sleep(0);
				}
				UpdateSimulationSoftJoints();
			}
		}
		else
		{
			UpdateSimulationSoftJoints();
		}
		ApplySoftJointBackForces();
		fixedUpdateDone = true;
	}

	public void EarlyUpdate()
	{
		if (useThreading)
		{
			StartThreads();
			while (physicsMeshTask.working)
			{
				Thread.Sleep(0);
			}
			UpdateNonSimulationSoftJoints();
			PrepareSoftMorphVerticesThreaded(fixedUpdateDone);
			fixedUpdateDone = false;
			physicsMeshTask.taskType = DAZPhysicsMeshTaskType.MorphVertices;
			physicsMeshTask.working = true;
			physicsMeshTask.resetEvent.Set();
		}
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			if (_globalOn != globalEnable)
			{
				SyncOn();
			}
			if (!useThreading)
			{
				UpdateNonSimulationSoftJoints();
			}
			CheckResumeSimulation();
			if (skin != null && (skin.dazMesh.visibleVerticesChangedThisFrame || skin.dazMesh.visibleVerticesChangedLastFrame))
			{
				SoftVerticesSetAutoRadius();
				RecalculateLinkJoints();
			}
			if (useThreading)
			{
				while (physicsMeshTask.working)
				{
					Thread.Sleep(0);
				}
			}
			else
			{
				MorphSoftVertices(fixedUpdateDone);
				fixedUpdateDone = false;
			}
			return;
		}
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component == null)
		{
			component = base.gameObject.AddComponent<MeshRenderer>();
		}
		if (_editorMeshForFocus == null)
		{
			_editorMeshForFocus = new Mesh();
		}
		meshFilter.mesh = _editorMeshForFocus;
		if (skin != null && skin.dazMesh != null && currentSoftVerticesGroup != null && currentSoftVerticesGroup.currentSet != null && currentSoftVerticesGroup.currentSet.targetVertex != -1)
		{
			Vector3 center = skin.dazMesh.morphedUVVertices[currentSoftVerticesGroup.currentSet.targetVertex];
			Vector3 size = default(Vector3);
			size.x = _handleSize * 50f;
			size.y = size.x;
			size.z = size.x;
			Bounds bounds = new Bounds(center, size);
			_editorMeshForFocus.bounds = bounds;
		}
	}

	protected void OnApplicationQuit()
	{
		if (Application.isPlaying)
		{
			StopThreads();
			StopAllCoroutines();
		}
	}
}
