using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DAZPhysicsMeshSoftVerticesGroup
{
	public enum InfluenceType
	{
		Distance,
		DistanceAlongMoveVector,
		HardCopy
	}

	public enum LookAtOption
	{
		Anchor,
		NormalReference,
		VertexNormal,
		VertexNormalRefUp,
		VertexNormalAnchorUp
	}

	public enum ColliderOrient
	{
		Tangent2,
		Tangent,
		Normal
	}

	public enum ColliderType
	{
		Capsule,
		Sphere,
		Box
	}

	public enum MovementType
	{
		Free,
		Limit,
		Lock
	}

	public DAZPhysicsMesh parent;

	protected bool _on = true;

	public bool useParentSettings = true;

	public bool useParentColliderSettings = true;

	protected bool _collisionEnabled = true;

	public string name;

	public bool showSoftSets = true;

	[SerializeField]
	protected List<DAZPhysicsMeshSoftVerticesSet> _softVerticesSets;

	[SerializeField]
	private int _currentSetIndex;

	[SerializeField]
	private InfluenceType _influenceType;

	[SerializeField]
	private bool _autoInfluenceAnchor;

	public bool centerBetweenTargetAndAnchor;

	[SerializeField]
	private float _maxInfluenceDistance = 0.03f;

	[SerializeField]
	private LookAtOption _lookAtOption;

	[SerializeField]
	private float _falloffPower = 2f;

	[SerializeField]
	private float _weightMultiplier = 1f;

	[SerializeField]
	private float _jointSpringNormal = 10f;

	[SerializeField]
	private float _jointDamperNormal = 1f;

	[SerializeField]
	private float _jointSpringTangent = 10f;

	[SerializeField]
	private float _jointDamperTangent = 1f;

	[SerializeField]
	private float _jointSpringTangent2 = 10f;

	[SerializeField]
	private float _jointDamperTangent2 = 1f;

	[SerializeField]
	private float _jointSpringMaxForce = 1E+07f;

	[SerializeField]
	private float _jointMass = 0.01f;

	[SerializeField]
	private ColliderOrient _colliderOrient;

	[SerializeField]
	private ColliderType _colliderType;

	[SerializeField]
	private float _colliderRadius = 0.003f;

	[SerializeField]
	private float _colliderLength = 0.003f;

	[SerializeField]
	private float _colliderNormalOffset;

	[SerializeField]
	private float _colliderTangentOffset;

	[SerializeField]
	private float _colliderTangent2Offset;

	[SerializeField]
	private string _colliderLayer;

	[SerializeField]
	private bool _useSecondCollider;

	[SerializeField]
	private float _secondColliderRadius = 0.003f;

	[SerializeField]
	private float _secondColliderLength = 0.003f;

	[SerializeField]
	private float _secondColliderNormalOffset;

	[SerializeField]
	private float _secondColliderTangentOffset;

	[SerializeField]
	private float _secondColliderTangent2Offset;

	[SerializeField]
	private Transform[] _ignoreColliders;

	public AutoColliderGroup[] ignoreAutoColliderGroups;

	[SerializeField]
	private PhysicMaterial _colliderMaterial;

	[SerializeField]
	private float _weightBias;

	[SerializeField]
	private bool _useUniformLimit;

	[SerializeField]
	private MovementType _normalMovementType;

	[SerializeField]
	private MovementType _tangentMovementType;

	[SerializeField]
	private MovementType _tangent2MovementType;

	protected Vector3 _startingNormalReferencePosition;

	[SerializeField]
	private Transform _normalReference;

	private Vector3 _normalReferencePosition;

	[SerializeField]
	private float _normalDistanceLimit = 0.015f;

	[SerializeField]
	private float _normalNegativeDistanceLimit = 0.015f;

	[SerializeField]
	private float _tangentDistanceLimit = 0.015f;

	[SerializeField]
	private float _tangentNegativeDistanceLimit = 0.015f;

	[SerializeField]
	private float _tangent2DistanceLimit = 0.015f;

	[SerializeField]
	private float _tangent2NegativeDistanceLimit = 0.015f;

	public bool useLinkJoints = true;

	[SerializeField]
	private float _linkSpring = 1f;

	[SerializeField]
	private float _linkDamper = 0.1f;

	private bool _pauseSimulation;

	[SerializeField]
	private bool _useSimulation;

	public bool useCustomInterpolation = true;

	public bool embedJoints;

	[SerializeField]
	private bool _clampVelocity;

	[SerializeField]
	private float _maxSimulationVelocity = 1f;

	private float _maxSimulationVelocitySqr = 1f;

	[SerializeField]
	private bool _useInterpolation;

	public Rigidbody backForceRigidbody;

	public AdjustJoints backForceAdjustJoints;

	public bool backForceAdjustJointsUseJoint2;

	public float backForceAdjustJointsMaxAngle;

	public FreeControllerV3 controller;

	public bool useJointBackForce;

	[SerializeField]
	private float _jointBackForce;

	[SerializeField]
	private float _jointBackForceThresholdDistance;

	[SerializeField]
	private float _jointBackForceMaxForce;

	protected DAZSkinV2 _skin;

	private bool wasInit;

	private bool wasInit2;

	protected Vector3 softVertexBackForce;

	protected Transform embedTransform;

	protected Rigidbody embedRB;

	protected Vector3 _bufferedBackForce;

	protected Vector3 _appliedBackForce;

	public float backForceResponse = 1f;

	public bool useThreading;

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

	public List<DAZPhysicsMeshSoftVerticesSet> softVerticesSets => _softVerticesSets;

	public int currentSetIndex
	{
		get
		{
			return _currentSetIndex;
		}
		set
		{
			if (_currentSetIndex != value && value >= 0 && value < _softVerticesSets.Count)
			{
				_currentSetIndex = value;
			}
		}
	}

	public DAZPhysicsMeshSoftVerticesSet currentSet
	{
		get
		{
			if (_currentSetIndex >= 0 && _currentSetIndex < _softVerticesSets.Count)
			{
				return _softVerticesSets[_currentSetIndex];
			}
			return null;
		}
		set
		{
			if (value == _softVerticesSets[_currentSetIndex])
			{
				return;
			}
			for (int i = 0; i < _softVerticesSets.Count; i++)
			{
				if (value == _softVerticesSets[i])
				{
					_currentSetIndex = i;
					break;
				}
			}
		}
	}

	public InfluenceType influenceType
	{
		get
		{
			return _influenceType;
		}
		set
		{
			if (_influenceType != value)
			{
				_influenceType = value;
			}
		}
	}

	public bool autoInfluenceAnchor
	{
		get
		{
			return _autoInfluenceAnchor;
		}
		set
		{
			if (_autoInfluenceAnchor != value)
			{
				_autoInfluenceAnchor = value;
				for (int i = 0; i < _softVerticesSets.Count; i++)
				{
					_softVerticesSets[i].autoInfluenceAnchor = _autoInfluenceAnchor;
				}
			}
		}
	}

	public float maxInfluenceDistance
	{
		get
		{
			return _maxInfluenceDistance;
		}
		set
		{
			if (_maxInfluenceDistance != value)
			{
				_maxInfluenceDistance = value;
			}
		}
	}

	public LookAtOption lookAtOption
	{
		get
		{
			return _lookAtOption;
		}
		set
		{
			if (_lookAtOption != value)
			{
				_lookAtOption = value;
				SyncJointParams();
			}
		}
	}

	public float falloffPower
	{
		get
		{
			return _falloffPower;
		}
		set
		{
			if (_falloffPower != value)
			{
				_falloffPower = value;
			}
		}
	}

	public float weightMultiplier
	{
		get
		{
			return _weightMultiplier;
		}
		set
		{
			if (_weightMultiplier != value)
			{
				_weightMultiplier = value;
			}
		}
	}

	public float jointSpringNormal
	{
		get
		{
			return _jointSpringNormal;
		}
		set
		{
			if (_jointSpringNormal != value)
			{
				_jointSpringNormal = value;
				SyncJointParams();
			}
		}
	}

	public float jointDamperNormal
	{
		get
		{
			return _jointDamperNormal;
		}
		set
		{
			if (_jointDamperNormal != value)
			{
				_jointDamperNormal = value;
				SyncJointParams();
			}
		}
	}

	public float jointSpringTangent
	{
		get
		{
			return _jointSpringTangent;
		}
		set
		{
			if (_jointSpringTangent != value)
			{
				_jointSpringTangent = value;
				SyncJointParams();
			}
		}
	}

	public float jointDamperTangent
	{
		get
		{
			return _jointDamperTangent;
		}
		set
		{
			if (_jointDamperTangent != value)
			{
				_jointDamperTangent = value;
				SyncJointParams();
			}
		}
	}

	public float jointSpringTangent2
	{
		get
		{
			return _jointSpringTangent2;
		}
		set
		{
			if (_jointSpringTangent2 != value)
			{
				_jointSpringTangent2 = value;
				SyncJointParams();
			}
		}
	}

	public float jointDamperTangent2
	{
		get
		{
			return _jointDamperTangent2;
		}
		set
		{
			if (_jointDamperTangent2 != value)
			{
				_jointDamperTangent2 = value;
				SyncJointParams();
			}
		}
	}

	public float jointSpringMaxForce
	{
		get
		{
			return _jointSpringMaxForce;
		}
		set
		{
			if (_jointSpringMaxForce != value)
			{
				_jointSpringMaxForce = value;
				SyncJointParams();
			}
		}
	}

	public float jointMass
	{
		get
		{
			return _jointMass;
		}
		set
		{
			if (_jointMass != value)
			{
				_jointMass = value;
				SyncJointParams();
			}
		}
	}

	public ColliderOrient colliderOrient
	{
		get
		{
			return _colliderOrient;
		}
		set
		{
			if (_colliderOrient != value)
			{
				_colliderOrient = value;
				SyncJointParams();
			}
		}
	}

	public ColliderType colliderType
	{
		get
		{
			return _colliderType;
		}
		set
		{
			if (_colliderType != value)
			{
				if (!Application.isPlaying)
				{
					_colliderType = value;
				}
				else
				{
					Debug.LogWarning("Cannot change colliderType at runtime");
				}
			}
		}
	}

	public float colliderRadius
	{
		get
		{
			return _colliderRadius;
		}
		set
		{
			if (_colliderRadius != value)
			{
				_colliderRadius = value;
				SyncJointParams();
			}
		}
	}

	public float colliderLength
	{
		get
		{
			return _colliderLength;
		}
		set
		{
			if (_colliderLength != value)
			{
				_colliderLength = value;
				SyncJointParams();
			}
		}
	}

	public float colliderNormalOffset
	{
		get
		{
			return _colliderNormalOffset;
		}
		set
		{
			if (_colliderNormalOffset != value)
			{
				_colliderNormalOffset = value;
				SyncJointParams();
			}
		}
	}

	public float colliderTangentOffset
	{
		get
		{
			return _colliderTangentOffset;
		}
		set
		{
			if (_colliderTangentOffset != value)
			{
				_colliderTangentOffset = value;
				SyncJointParams();
			}
		}
	}

	public float colliderTangent2Offset
	{
		get
		{
			return _colliderTangent2Offset;
		}
		set
		{
			if (_colliderTangent2Offset != value)
			{
				_colliderTangent2Offset = value;
				SyncJointParams();
			}
		}
	}

	public string colliderLayer
	{
		get
		{
			return _colliderLayer;
		}
		set
		{
			if (_colliderLayer != value)
			{
				if (!Application.isPlaying)
				{
					_colliderLayer = value;
				}
				else
				{
					Debug.LogWarning("Cannot change colliderLayer at runtime");
				}
			}
		}
	}

	public bool useSecondCollider
	{
		get
		{
			return _useSecondCollider;
		}
		set
		{
			if (_useSecondCollider != value)
			{
				if (!Application.isPlaying)
				{
					_useSecondCollider = value;
				}
				else
				{
					Debug.LogWarning("Cannot change useSecondCollider at runtime");
				}
			}
		}
	}

	public float secondColliderRadius
	{
		get
		{
			return _secondColliderRadius;
		}
		set
		{
			if (_secondColliderRadius != value)
			{
				_secondColliderRadius = value;
				SyncJointParams();
			}
		}
	}

	public float secondColliderLength
	{
		get
		{
			return _secondColliderLength;
		}
		set
		{
			if (_secondColliderLength != value)
			{
				_secondColliderLength = value;
				SyncJointParams();
			}
		}
	}

	public float secondColliderNormalOffset
	{
		get
		{
			return _secondColliderNormalOffset;
		}
		set
		{
			if (_secondColliderNormalOffset != value)
			{
				_secondColliderNormalOffset = value;
				SyncJointParams();
			}
		}
	}

	public float secondColliderTangentOffset
	{
		get
		{
			return _secondColliderTangentOffset;
		}
		set
		{
			if (_secondColliderTangentOffset != value)
			{
				_secondColliderTangentOffset = value;
				SyncJointParams();
			}
		}
	}

	public float secondColliderTangent2Offset
	{
		get
		{
			return _secondColliderTangent2Offset;
		}
		set
		{
			if (_secondColliderTangent2Offset != value)
			{
				_secondColliderTangent2Offset = value;
				SyncJointParams();
			}
		}
	}

	public Transform[] ignoreColliders
	{
		get
		{
			return _ignoreColliders;
		}
		set
		{
			if (_ignoreColliders != value)
			{
				_ignoreColliders = value;
			}
		}
	}

	public PhysicMaterial colliderMaterial
	{
		get
		{
			return _colliderMaterial;
		}
		set
		{
			if (_colliderMaterial != value)
			{
				_colliderMaterial = value;
				SyncJointParams();
			}
		}
	}

	public float weightBias
	{
		get
		{
			return _weightBias;
		}
		set
		{
			if (_weightBias != value)
			{
				_weightBias = value;
			}
		}
	}

	public bool useUniformLimit
	{
		get
		{
			return _useUniformLimit;
		}
		set
		{
			if (_useUniformLimit != value)
			{
				_useUniformLimit = value;
				SyncJointParams();
			}
		}
	}

	public MovementType normalMovementType
	{
		get
		{
			return _normalMovementType;
		}
		set
		{
			if (_normalMovementType != value)
			{
				_normalMovementType = value;
				SyncJointParams();
			}
		}
	}

	public MovementType tangentMovementType
	{
		get
		{
			return _tangentMovementType;
		}
		set
		{
			if (_tangentMovementType != value)
			{
				_tangentMovementType = value;
				SyncJointParams();
			}
		}
	}

	public MovementType tangent2MovementType
	{
		get
		{
			return _tangent2MovementType;
		}
		set
		{
			if (_tangent2MovementType != value)
			{
				_tangent2MovementType = value;
				SyncJointParams();
			}
		}
	}

	public Transform normalReference
	{
		get
		{
			return _normalReference;
		}
		set
		{
			if (_normalReference != value)
			{
				_normalReference = value;
			}
		}
	}

	public float normalDistanceLimit
	{
		get
		{
			return _normalDistanceLimit;
		}
		set
		{
			if (_normalDistanceLimit != value)
			{
				_normalDistanceLimit = value;
				if (_useUniformLimit)
				{
					SyncJointParams();
				}
			}
		}
	}

	public float normalNegativeDistanceLimit
	{
		get
		{
			return _normalNegativeDistanceLimit;
		}
		set
		{
			if (_normalNegativeDistanceLimit != value)
			{
				_normalNegativeDistanceLimit = value;
			}
		}
	}

	public float tangentDistanceLimit
	{
		get
		{
			return _tangentDistanceLimit;
		}
		set
		{
			if (_tangentDistanceLimit != value)
			{
				_tangentDistanceLimit = value;
			}
		}
	}

	public float tangentNegativeDistanceLimit
	{
		get
		{
			return _tangentNegativeDistanceLimit;
		}
		set
		{
			if (_tangentNegativeDistanceLimit != value)
			{
				_tangentNegativeDistanceLimit = value;
			}
		}
	}

	public float tangent2DistanceLimit
	{
		get
		{
			return _tangent2DistanceLimit;
		}
		set
		{
			if (_tangent2DistanceLimit != value)
			{
				_tangent2DistanceLimit = value;
			}
		}
	}

	public float tangent2NegativeDistanceLimit
	{
		get
		{
			return _tangent2NegativeDistanceLimit;
		}
		set
		{
			if (_tangent2NegativeDistanceLimit != value)
			{
				_tangent2NegativeDistanceLimit = value;
			}
		}
	}

	public float linkSpring
	{
		get
		{
			return _linkSpring;
		}
		set
		{
			if (_linkSpring != value)
			{
				_linkSpring = value;
				SyncLinkParams();
			}
		}
	}

	public float linkDamper
	{
		get
		{
			return _linkDamper;
		}
		set
		{
			if (_linkDamper != value)
			{
				_linkDamper = value;
				SyncLinkParams();
			}
		}
	}

	public bool pauseSimulation
	{
		get
		{
			return _pauseSimulation;
		}
		set
		{
			if (_pauseSimulation != value)
			{
				_pauseSimulation = value;
				SyncCollisionEnabled();
			}
		}
	}

	public bool useSimulation
	{
		get
		{
			return _useSimulation;
		}
		set
		{
			if (_useSimulation != value)
			{
				if (!Application.isPlaying)
				{
					_useSimulation = value;
				}
				else
				{
					Debug.LogWarning("Cannot change useSimulation at runtime");
				}
			}
		}
	}

	public bool clampVelocity
	{
		get
		{
			return _clampVelocity;
		}
		set
		{
			if (_clampVelocity != value)
			{
				_clampVelocity = value;
			}
		}
	}

	public float maxSimulationVelocity
	{
		get
		{
			return _maxSimulationVelocity;
		}
		set
		{
			if (_maxSimulationVelocity != value)
			{
				_maxSimulationVelocity = value;
				_maxSimulationVelocitySqr = value * value;
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
				SyncJointParams();
			}
		}
	}

	public float jointBackForce
	{
		get
		{
			return _jointBackForce;
		}
		set
		{
			if (_jointBackForce != value)
			{
				_jointBackForce = value;
			}
		}
	}

	public float jointBackForceThresholdDistance
	{
		get
		{
			return _jointBackForceThresholdDistance;
		}
		set
		{
			if (_jointBackForceThresholdDistance != value)
			{
				_jointBackForceThresholdDistance = value;
			}
		}
	}

	public float jointBackForceMaxForce
	{
		get
		{
			return _jointBackForceMaxForce;
		}
		set
		{
			if (_jointBackForceMaxForce != value)
			{
				_jointBackForceMaxForce = value;
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
			if (_skin != value)
			{
				_skin = value;
			}
		}
	}

	public Vector3 bufferedBackForce => _bufferedBackForce;

	public DAZPhysicsMeshSoftVerticesGroup()
	{
		_softVerticesSets = new List<DAZPhysicsMeshSoftVerticesSet>();
	}

	protected void SyncOn()
	{
		if (_on)
		{
			ResetJoints();
			for (int i = 0; i < _softVerticesSets.Count; i++)
			{
				DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
				if (dAZPhysicsMeshSoftVerticesSet.jointTransform != null)
				{
					dAZPhysicsMeshSoftVerticesSet.jointTransform.gameObject.SetActive(value: true);
				}
			}
			return;
		}
		Vector3 zero = Vector3.zero;
		for (int j = 0; j < _softVerticesSets.Count; j++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet2 = _softVerticesSets[j];
			if (dAZPhysicsMeshSoftVerticesSet2.jointTransform != null)
			{
				dAZPhysicsMeshSoftVerticesSet2.jointTransform.gameObject.SetActive(value: false);
			}
			if (!(_skin != null))
			{
				continue;
			}
			if (_skin.wasInit)
			{
				_skin.postSkinMorphs[dAZPhysicsMeshSoftVerticesSet2.targetVertex] = zero;
				for (int k = 0; k < dAZPhysicsMeshSoftVerticesSet2.influenceVertices.Length; k++)
				{
					int num = dAZPhysicsMeshSoftVerticesSet2.influenceVertices[k];
					_skin.postSkinMorphs[num] = zero;
				}
			}
			_skin.postSkinVertsChanged = true;
		}
	}

	public void SyncCollisionEnabled()
	{
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			if (dAZPhysicsMeshSoftVerticesSet.jointRB != null)
			{
				dAZPhysicsMeshSoftVerticesSet.jointRB.detectCollisions = _collisionEnabled && !_pauseSimulation;
			}
		}
	}

	public int AddSet()
	{
		DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = new DAZPhysicsMeshSoftVerticesSet();
		dAZPhysicsMeshSoftVerticesSet.autoInfluenceAnchor = _autoInfluenceAnchor;
		_softVerticesSets.Add(dAZPhysicsMeshSoftVerticesSet);
		return _softVerticesSets.Count - 1;
	}

	public void RemoveSet(int index)
	{
		DAZPhysicsMeshSoftVerticesSet ss = _softVerticesSets[index];
		ClearLinks(ss);
		_softVerticesSets.RemoveAt(index);
		if (_currentSetIndex >= _softVerticesSets.Count)
		{
			_currentSetIndex--;
		}
	}

	public void ClearLinks(DAZPhysicsMeshSoftVerticesSet ss)
	{
		bool flag = false;
		foreach (DAZPhysicsMeshSoftVerticesSet softVerticesSet in softVerticesSets)
		{
			if (ss == softVerticesSet)
			{
				flag = true;
			}
		}
		if (flag)
		{
			ss.links.Clear();
		}
	}

	public void MoveSet(int fromindex, int toindex)
	{
		if (toindex >= 0 && toindex < _softVerticesSets.Count)
		{
			DAZPhysicsMeshSoftVerticesSet item = _softVerticesSets[fromindex];
			_softVerticesSets.RemoveAt(fromindex);
			_softVerticesSets.Insert(toindex, item);
			if (_currentSetIndex == fromindex)
			{
				_currentSetIndex = toindex;
			}
		}
	}

	public DAZPhysicsMeshSoftVerticesSet GetSetByID(string uid, bool skipCheckParent = false)
	{
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			if (_softVerticesSets[i].uid == uid)
			{
				return _softVerticesSets[i];
			}
		}
		if (parent != null && !skipCheckParent)
		{
			return parent.GetSoftSetByID(uid);
		}
		return null;
	}

	public void Init(Transform transform, DAZSkinV2 sk)
	{
		if (!wasInit && sk != null)
		{
			skin = sk;
			wasInit = true;
			CreateJoints(transform);
			InitWeights();
			SyncOn();
			ResetJoints();
		}
	}

	public void InitPass2()
	{
		if (!wasInit2)
		{
			CreateLinkJoints();
			wasInit2 = true;
		}
	}

	private void InitWeights()
	{
		if (_influenceType == InfluenceType.HardCopy)
		{
			return;
		}
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			if (dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length <= 0)
			{
				continue;
			}
			dAZPhysicsMeshSoftVerticesSet.influenceVerticesDistances = new float[dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length];
			dAZPhysicsMeshSoftVerticesSet.influenceVerticesWeights = new float[dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length];
			Vector3 vector = (embedJoints ? ((lookAtOption != 0) ? dAZPhysicsMeshSoftVerticesSet.jointTransform.up : dAZPhysicsMeshSoftVerticesSet.jointTransform.forward) : ((lookAtOption != 0) ? dAZPhysicsMeshSoftVerticesSet.kinematicTransform.up : dAZPhysicsMeshSoftVerticesSet.kinematicTransform.forward));
			for (int j = 0; j < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; j++)
			{
				Vector3 rhs = _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.influenceVertices[j]] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex];
				if (_influenceType == InfluenceType.Distance)
				{
					dAZPhysicsMeshSoftVerticesSet.influenceVerticesDistances[j] = rhs.magnitude;
				}
				else if (_influenceType == InfluenceType.DistanceAlongMoveVector)
				{
					Vector3 vector2 = vector * Vector3.Dot(vector, rhs);
					dAZPhysicsMeshSoftVerticesSet.influenceVerticesDistances[j] = vector2.magnitude;
				}
			}
		}
	}

	private void SyncJointParams()
	{
		if (wasInit)
		{
			for (int i = 0; i < _softVerticesSets.Count; i++)
			{
				DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
				SetMass(dAZPhysicsMeshSoftVerticesSet);
				SetInterpolation(dAZPhysicsMeshSoftVerticesSet);
				SetJointLimits(dAZPhysicsMeshSoftVerticesSet.joint, dAZPhysicsMeshSoftVerticesSet);
				SetJointDrive(dAZPhysicsMeshSoftVerticesSet.joint, dAZPhysicsMeshSoftVerticesSet);
				SetColliders(dAZPhysicsMeshSoftVerticesSet);
			}
		}
	}

	private void SyncLinkParams()
	{
		if (!wasInit || !useLinkJoints)
		{
			return;
		}
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			for (int j = 0; j < dAZPhysicsMeshSoftVerticesSet.linkJoints.Length; j++)
			{
				SpringJoint springJoint = dAZPhysicsMeshSoftVerticesSet.linkJoints[j];
				springJoint.spring = _linkSpring;
				springJoint.damper = _linkDamper;
			}
		}
	}

	private void CreateLinkJoints()
	{
		if (!useLinkJoints)
		{
			return;
		}
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			GameObject gameObject = dAZPhysicsMeshSoftVerticesSet.jointTransform.gameObject;
			if (dAZPhysicsMeshSoftVerticesSet.links == null)
			{
				continue;
			}
			dAZPhysicsMeshSoftVerticesSet.linkJoints = new SpringJoint[_softVerticesSets[i].links.Count];
			for (int j = 0; j < dAZPhysicsMeshSoftVerticesSet.links.Count; j++)
			{
				DAZPhysicsMeshSoftVerticesSet setByID = GetSetByID(dAZPhysicsMeshSoftVerticesSet.links[j]);
				if (setByID != null && setByID.jointRB != null)
				{
					SpringJoint springJoint = gameObject.AddComponent<SpringJoint>();
					dAZPhysicsMeshSoftVerticesSet.linkJoints[j] = springJoint;
					springJoint.spring = _linkSpring;
					springJoint.damper = _linkDamper;
					float num2 = (springJoint.maxDistance = (springJoint.minDistance = (dAZPhysicsMeshSoftVerticesSet.initialTargetPosition - setByID.initialTargetPosition).magnitude));
					springJoint.tolerance = 0.001f;
					springJoint.autoConfigureConnectedAnchor = false;
					springJoint.connectedBody = setByID.jointRB;
					springJoint.connectedAnchor = Vector3.zero;
				}
				else
				{
					Debug.LogError("Link joint is null");
				}
			}
		}
	}

	private void GetCollidersRecursive(Transform rootTransform, Transform t, List<Collider> colliders)
	{
		if (t != rootTransform && (bool)t.GetComponent<Rigidbody>())
		{
			return;
		}
		Collider[] components = t.GetComponents<Collider>();
		foreach (Collider collider in components)
		{
			if (collider != null && collider.gameObject.activeInHierarchy && collider.enabled)
			{
				colliders.Add(collider);
			}
		}
		foreach (Transform item in t)
		{
			GetCollidersRecursive(rootTransform, item, colliders);
		}
	}

	public void InitColliders()
	{
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			if (dAZPhysicsMeshSoftVerticesSet.jointCollider != null)
			{
				List<Collider> list = new List<Collider>();
				Transform[] array = _ignoreColliders;
				foreach (Transform transform in array)
				{
					GetCollidersRecursive(transform, transform, list);
				}
				foreach (Collider item in list)
				{
					Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet.jointCollider, item);
					if (dAZPhysicsMeshSoftVerticesSet.jointCollider2 != null)
					{
						Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet.jointCollider2, item);
					}
				}
			}
			if (dAZPhysicsMeshSoftVerticesSet.jointCollider != null && dAZPhysicsMeshSoftVerticesSet.jointCollider2 != null)
			{
				Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet.jointCollider, dAZPhysicsMeshSoftVerticesSet.jointCollider2);
			}
		}
		AutoColliderGroup[] array2 = ignoreAutoColliderGroups;
		foreach (AutoColliderGroup autoColliderGroup in array2)
		{
			if (!(autoColliderGroup != null))
			{
				continue;
			}
			AutoCollider[] autoColliders = autoColliderGroup.GetAutoColliders();
			AutoCollider[] array3 = autoColliders;
			foreach (AutoCollider autoCollider in array3)
			{
				if (!(autoCollider.jointCollider != null))
				{
					continue;
				}
				for (int m = 0; m < _softVerticesSets.Count; m++)
				{
					DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet2 = _softVerticesSets[m];
					if (dAZPhysicsMeshSoftVerticesSet2.jointCollider != null)
					{
						Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet2.jointCollider, autoCollider.jointCollider);
						if (dAZPhysicsMeshSoftVerticesSet2.jointCollider2 != null)
						{
							Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet2.jointCollider2, autoCollider.jointCollider);
						}
					}
				}
			}
		}
		for (int n = 0; n < _softVerticesSets.Count - 1; n++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet3 = _softVerticesSets[n];
			for (int num = n + 1; num < _softVerticesSets.Count; num++)
			{
				DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet4 = _softVerticesSets[num];
				if (!(dAZPhysicsMeshSoftVerticesSet3.jointCollider != null) || !(dAZPhysicsMeshSoftVerticesSet4.jointCollider != null))
				{
					continue;
				}
				Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet3.jointCollider, dAZPhysicsMeshSoftVerticesSet4.jointCollider);
				if (dAZPhysicsMeshSoftVerticesSet3.jointCollider2 != null)
				{
					Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet3.jointCollider2, dAZPhysicsMeshSoftVerticesSet4.jointCollider);
					if (dAZPhysicsMeshSoftVerticesSet4.jointCollider2 != null)
					{
						Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet3.jointCollider2, dAZPhysicsMeshSoftVerticesSet4.jointCollider2);
					}
				}
				if (dAZPhysicsMeshSoftVerticesSet4.jointCollider2 != null)
				{
					Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet3.jointCollider, dAZPhysicsMeshSoftVerticesSet4.jointCollider2);
				}
			}
		}
	}

	public void IgnoreOtherSoftGroupColliders(DAZPhysicsMeshSoftVerticesGroup otherGroup)
	{
		List<DAZPhysicsMeshSoftVerticesSet> list = otherGroup.softVerticesSets;
		for (int i = 0; i < list.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = list[i];
			for (int j = 0; j < _softVerticesSets.Count; j++)
			{
				DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet2 = _softVerticesSets[j];
				if (!(dAZPhysicsMeshSoftVerticesSet.jointCollider != null) || !(dAZPhysicsMeshSoftVerticesSet2.jointCollider != null))
				{
					continue;
				}
				Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet.jointCollider, dAZPhysicsMeshSoftVerticesSet2.jointCollider);
				if (dAZPhysicsMeshSoftVerticesSet.jointCollider2 != null)
				{
					Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet.jointCollider2, dAZPhysicsMeshSoftVerticesSet2.jointCollider);
					if (dAZPhysicsMeshSoftVerticesSet2.jointCollider2 != null)
					{
						Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet.jointCollider2, dAZPhysicsMeshSoftVerticesSet2.jointCollider2);
					}
				}
				if (dAZPhysicsMeshSoftVerticesSet2.jointCollider2 != null)
				{
					Physics.IgnoreCollision(dAZPhysicsMeshSoftVerticesSet.jointCollider, dAZPhysicsMeshSoftVerticesSet2.jointCollider2);
				}
			}
		}
	}

	private void SetInterpolation(DAZPhysicsMeshSoftVerticesSet ss)
	{
		if (_useInterpolation && _useSimulation && !useCustomInterpolation)
		{
			ss.jointRB.interpolation = RigidbodyInterpolation.Interpolate;
		}
		else
		{
			ss.jointRB.interpolation = RigidbodyInterpolation.None;
		}
	}

	private void SetJointLimits(ConfigurableJoint joint, DAZPhysicsMeshSoftVerticesSet ss, bool forceUniform = false)
	{
		if (_useUniformLimit || forceUniform)
		{
			if (lookAtOption == LookAtOption.Anchor)
			{
				if (_normalMovementType == MovementType.Lock)
				{
					joint.yMotion = ConfigurableJointMotion.Locked;
				}
				else if (_normalMovementType == MovementType.Free)
				{
					joint.yMotion = ConfigurableJointMotion.Free;
				}
				else
				{
					joint.yMotion = ConfigurableJointMotion.Limited;
				}
				if (_tangentMovementType == MovementType.Lock)
				{
					joint.zMotion = ConfigurableJointMotion.Locked;
				}
				else if (_tangentMovementType == MovementType.Free)
				{
					joint.zMotion = ConfigurableJointMotion.Free;
				}
				else
				{
					joint.zMotion = ConfigurableJointMotion.Limited;
				}
			}
			else
			{
				if (_normalMovementType == MovementType.Lock)
				{
					joint.zMotion = ConfigurableJointMotion.Locked;
				}
				else if (_normalMovementType == MovementType.Free)
				{
					joint.zMotion = ConfigurableJointMotion.Free;
				}
				else
				{
					joint.zMotion = ConfigurableJointMotion.Limited;
				}
				if (_tangentMovementType == MovementType.Lock)
				{
					joint.yMotion = ConfigurableJointMotion.Locked;
				}
				else if (_tangentMovementType == MovementType.Free)
				{
					joint.yMotion = ConfigurableJointMotion.Free;
				}
				else
				{
					joint.yMotion = ConfigurableJointMotion.Limited;
				}
			}
			if (_tangent2MovementType == MovementType.Lock)
			{
				joint.xMotion = ConfigurableJointMotion.Locked;
			}
			else if (_tangent2MovementType == MovementType.Free)
			{
				joint.xMotion = ConfigurableJointMotion.Free;
			}
			else
			{
				joint.xMotion = ConfigurableJointMotion.Limited;
			}
			SoftJointLimit linearLimit = joint.linearLimit;
			linearLimit.limit = _normalDistanceLimit * ss.limitMultiplier;
			joint.linearLimit = linearLimit;
		}
		else
		{
			if (lookAtOption == LookAtOption.Anchor)
			{
				if (_normalMovementType == MovementType.Lock)
				{
					joint.yMotion = ConfigurableJointMotion.Locked;
				}
				else
				{
					joint.yMotion = ConfigurableJointMotion.Free;
				}
				if (_tangentMovementType == MovementType.Lock)
				{
					joint.zMotion = ConfigurableJointMotion.Locked;
				}
				else
				{
					joint.zMotion = ConfigurableJointMotion.Free;
				}
			}
			else
			{
				if (_normalMovementType == MovementType.Lock)
				{
					joint.zMotion = ConfigurableJointMotion.Locked;
				}
				else
				{
					joint.zMotion = ConfigurableJointMotion.Free;
				}
				if (_tangentMovementType == MovementType.Lock)
				{
					joint.yMotion = ConfigurableJointMotion.Locked;
				}
				else
				{
					joint.yMotion = ConfigurableJointMotion.Free;
				}
			}
			if (_tangent2MovementType == MovementType.Lock)
			{
				joint.xMotion = ConfigurableJointMotion.Locked;
			}
			else
			{
				joint.xMotion = ConfigurableJointMotion.Free;
			}
		}
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;
	}

	private void SetJointDrive(ConfigurableJoint joint, DAZPhysicsMeshSoftVerticesSet ss)
	{
		JointDrive zDrive = default(JointDrive);
		zDrive.positionSpring = _jointSpringNormal * ss.springMultiplier;
		zDrive.positionDamper = _jointDamperNormal;
		zDrive.maximumForce = _jointSpringMaxForce;
		JointDrive xDrive = default(JointDrive);
		xDrive.positionSpring = _jointSpringTangent * ss.springMultiplier;
		xDrive.positionDamper = _jointDamperTangent;
		xDrive.maximumForce = _jointSpringMaxForce;
		JointDrive yDrive = default(JointDrive);
		yDrive.positionSpring = _jointSpringTangent2 * ss.springMultiplier;
		yDrive.positionDamper = _jointDamperTangent2;
		yDrive.maximumForce = _jointSpringMaxForce;
		joint.xDrive = xDrive;
		joint.yDrive = yDrive;
		joint.zDrive = zDrive;
	}

	private void SetColliders(DAZPhysicsMeshSoftVerticesSet ss)
	{
		int direction = 0;
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		if (lookAtOption != 0)
		{
			switch (_colliderOrient)
			{
			case ColliderOrient.Normal:
				direction = 2;
				break;
			case ColliderOrient.Tangent:
				direction = 1;
				break;
			case ColliderOrient.Tangent2:
				direction = 0;
				break;
			}
			vector.x = _colliderTangent2Offset;
			vector.y = _colliderTangentOffset;
			vector.z = _colliderNormalOffset;
			vector2.x = _secondColliderTangent2Offset;
			vector2.y = _secondColliderTangentOffset;
			vector2.z = _secondColliderNormalOffset;
		}
		else
		{
			direction = (int)_colliderOrient;
			vector.x = _colliderTangent2Offset;
			vector.z = _colliderTangentOffset;
			vector.y = _colliderNormalOffset;
			vector2.x = _secondColliderTangent2Offset;
			vector2.z = _secondColliderTangentOffset;
			vector2.y = _secondColliderNormalOffset;
		}
		switch (colliderType)
		{
		case ColliderType.Capsule:
		{
			CapsuleCollider capsuleCollider = ss.jointCollider as CapsuleCollider;
			capsuleCollider.radius = _colliderRadius * ss.sizeMultiplier;
			capsuleCollider.height = _colliderLength * ss.sizeMultiplier;
			capsuleCollider.direction = direction;
			capsuleCollider.center = vector * ss.sizeMultiplier;
			if (_useSecondCollider && ss.jointCollider2 != null)
			{
				capsuleCollider = ss.jointCollider2 as CapsuleCollider;
				capsuleCollider.radius = _secondColliderRadius * ss.sizeMultiplier;
				capsuleCollider.height = _secondColliderLength * ss.sizeMultiplier;
				capsuleCollider.direction = direction;
				capsuleCollider.center = vector2 * ss.sizeMultiplier;
			}
			break;
		}
		case ColliderType.Sphere:
		{
			SphereCollider sphereCollider = ss.jointCollider as SphereCollider;
			sphereCollider.radius = _colliderRadius * ss.sizeMultiplier;
			sphereCollider.center = vector * ss.sizeMultiplier;
			if (_useSecondCollider && ss.jointCollider2 != null)
			{
				sphereCollider = ss.jointCollider2 as SphereCollider;
				sphereCollider.radius = _secondColliderRadius * ss.sizeMultiplier;
				sphereCollider.center = vector2 * ss.sizeMultiplier;
			}
			break;
		}
		case ColliderType.Box:
		{
			BoxCollider boxCollider = ss.jointCollider as BoxCollider;
			float num = _colliderRadius * 2f * ss.sizeMultiplier;
			boxCollider.size = new Vector3(num, num, num);
			boxCollider.center = vector * ss.sizeMultiplier;
			if (_useSecondCollider && ss.jointCollider2 != null)
			{
				boxCollider = ss.jointCollider2 as BoxCollider;
				num = _secondColliderRadius * 2f * ss.sizeMultiplier;
				boxCollider.size = new Vector3(num, num, num);
				boxCollider.center = vector2 * ss.sizeMultiplier;
			}
			break;
		}
		}
		if (_colliderMaterial != null)
		{
			ss.jointCollider.sharedMaterial = _colliderMaterial;
			if (_useSecondCollider)
			{
				ss.jointCollider2.sharedMaterial = _colliderMaterial;
			}
		}
	}

	private void SetMass(DAZPhysicsMeshSoftVerticesSet ss)
	{
		ss.jointRB.mass = _jointMass;
	}

	private void CreateJoints(Transform transform)
	{
		if (_normalReference != null)
		{
			_startingNormalReferencePosition = _normalReference.position;
			Transform transform2;
			if (name != null && name != string.Empty)
			{
				GameObject gameObject = new GameObject("PhysicsMesh" + name);
				transform2 = gameObject.transform;
				transform2.SetParent(transform);
				transform2.position = transform.position;
				transform2.rotation = transform.rotation;
			}
			else
			{
				transform2 = transform;
			}
			Vector3 vector;
			if (_normalReference != null)
			{
				DAZBone component = _normalReference.GetComponent<DAZBone>();
				vector = ((!(component != null)) ? transform2.position : component.importWorldPosition);
			}
			else
			{
				vector = Vector3.zero;
			}
			if (embedJoints)
			{
				GameObject gameObject2 = new GameObject("PhysicsMeshKRB" + name);
				embedRB = gameObject2.AddComponent<Rigidbody>();
				embedRB.isKinematic = true;
				embedTransform = embedRB.transform;
				embedTransform.SetParent(transform2);
				embedTransform.position = _normalReference.position;
				embedTransform.rotation = _normalReference.rotation;
			}
			else
			{
				embedRB = null;
				embedTransform = null;
			}
			bool flag = lookAtOption == LookAtOption.VertexNormal || lookAtOption == LookAtOption.VertexNormalRefUp || lookAtOption == LookAtOption.VertexNormalAnchorUp;
			for (int i = 0; i < _softVerticesSets.Count; i++)
			{
				DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
				Vector3[] visibleMorphedUVVertices = _skin.dazMesh.visibleMorphedUVVertices;
				Vector3[] morphedUVNormals = _skin.dazMesh.morphedUVNormals;
				Vector3 vector2 = (dAZPhysicsMeshSoftVerticesSet.initialTargetPosition = ((!centerBetweenTargetAndAnchor) ? visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.targetVertex] : ((visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.targetVertex] + visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f)));
				if (!embedJoints)
				{
					GameObject gameObject3 = new GameObject("PhysicsMeshKRB" + name + i);
					dAZPhysicsMeshSoftVerticesSet.kinematicTransform = gameObject3.transform;
					dAZPhysicsMeshSoftVerticesSet.kinematicTransform.SetParent(transform2);
					dAZPhysicsMeshSoftVerticesSet.kinematicRB = gameObject3.AddComponent<Rigidbody>();
					dAZPhysicsMeshSoftVerticesSet.kinematicRB.isKinematic = true;
					dAZPhysicsMeshSoftVerticesSet.kinematicTransform.position = vector2;
				}
				_skin.postSkinVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] = true;
				_skin.postSkinVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] = true;
				if (flag)
				{
					_skin.postSkinNormalVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] = true;
					if (centerBetweenTargetAndAnchor)
					{
						_skin.postSkinNormalVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] = true;
					}
				}
				_skin.postSkinVertsChanged = true;
				Quaternion rotation = ((lookAtOption == LookAtOption.NormalReference || dAZPhysicsMeshSoftVerticesSet.forceLookAtReference) ? Quaternion.LookRotation(vector - vector2, visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.targetVertex]) : ((lookAtOption == LookAtOption.VertexNormal) ? ((!centerBetweenTargetAndAnchor) ? Quaternion.LookRotation(-morphedUVNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex]) : Quaternion.LookRotation((-morphedUVNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex] - morphedUVNormals[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f)) : ((lookAtOption == LookAtOption.VertexNormalRefUp) ? ((!centerBetweenTargetAndAnchor) ? Quaternion.LookRotation(-morphedUVNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex], vector - vector2) : Quaternion.LookRotation((-morphedUVNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex] - morphedUVNormals[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f, vector - vector2)) : ((lookAtOption != LookAtOption.VertexNormalAnchorUp) ? Quaternion.LookRotation(visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.targetVertex], vector - vector2) : ((!centerBetweenTargetAndAnchor) ? Quaternion.LookRotation(-morphedUVNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex], visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.targetVertex]) : Quaternion.LookRotation((-morphedUVNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex] - morphedUVNormals[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f, visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.targetVertex]))))));
				if (!embedJoints)
				{
					dAZPhysicsMeshSoftVerticesSet.kinematicTransform.rotation = rotation;
				}
				if (_useSimulation && !_useUniformLimit && !embedJoints)
				{
					GameObject gameObject4 = new GameObject("JointTracker");
					dAZPhysicsMeshSoftVerticesSet.jointTrackerTransform = gameObject4.transform;
					dAZPhysicsMeshSoftVerticesSet.jointTrackerTransform.SetParent(dAZPhysicsMeshSoftVerticesSet.kinematicTransform);
					dAZPhysicsMeshSoftVerticesSet.jointTrackerTransform.localPosition = Vector3.zero;
					dAZPhysicsMeshSoftVerticesSet.jointTrackerTransform.localRotation = Quaternion.identity;
				}
				GameObject gameObject5 = new GameObject("PhysicsMeshJoint" + name + i);
				dAZPhysicsMeshSoftVerticesSet.jointTransform = gameObject5.transform;
				if (_useSimulation)
				{
					if (embedJoints)
					{
						dAZPhysicsMeshSoftVerticesSet.jointTransform.SetParent(transform2);
					}
					else
					{
						dAZPhysicsMeshSoftVerticesSet.jointTransform.SetParent(transform2);
					}
				}
				else if (embedJoints)
				{
					dAZPhysicsMeshSoftVerticesSet.jointTransform.SetParent(embedTransform);
				}
				else
				{
					dAZPhysicsMeshSoftVerticesSet.jointTransform.SetParent(dAZPhysicsMeshSoftVerticesSet.kinematicTransform);
				}
				dAZPhysicsMeshSoftVerticesSet.jointTransform.position = vector2;
				dAZPhysicsMeshSoftVerticesSet.jointTransform.rotation = rotation;
				dAZPhysicsMeshSoftVerticesSet.jointRB = gameObject5.AddComponent<Rigidbody>();
				dAZPhysicsMeshSoftVerticesSet.jointRB.useGravity = false;
				dAZPhysicsMeshSoftVerticesSet.jointRB.drag = 0f;
				dAZPhysicsMeshSoftVerticesSet.jointRB.angularDrag = 0f;
				dAZPhysicsMeshSoftVerticesSet.jointRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
				dAZPhysicsMeshSoftVerticesSet.jointRB.isKinematic = false;
				dAZPhysicsMeshSoftVerticesSet.jointRB.detectCollisions = false;
				dAZPhysicsMeshSoftVerticesSet.joint = gameObject5.AddComponent<ConfigurableJoint>();
				if (embedJoints)
				{
					dAZPhysicsMeshSoftVerticesSet.joint.connectedBody = embedRB;
					dAZPhysicsMeshSoftVerticesSet.joint.autoConfigureConnectedAnchor = false;
					dAZPhysicsMeshSoftVerticesSet.joint.anchor = Vector3.zero;
					dAZPhysicsMeshSoftVerticesSet.joint.connectedAnchor = _normalReference.InverseTransformPoint(vector2);
				}
				else
				{
					dAZPhysicsMeshSoftVerticesSet.joint.connectedBody = dAZPhysicsMeshSoftVerticesSet.kinematicRB;
					dAZPhysicsMeshSoftVerticesSet.joint.autoConfigureConnectedAnchor = false;
					dAZPhysicsMeshSoftVerticesSet.joint.anchor = Vector3.zero;
					dAZPhysicsMeshSoftVerticesSet.joint.connectedAnchor = Vector3.zero;
				}
				SetJointLimits(dAZPhysicsMeshSoftVerticesSet.joint, dAZPhysicsMeshSoftVerticesSet);
				SetJointDrive(dAZPhysicsMeshSoftVerticesSet.joint, dAZPhysicsMeshSoftVerticesSet);
				SetInterpolation(dAZPhysicsMeshSoftVerticesSet);
				switch (colliderType)
				{
				case ColliderType.Capsule:
				{
					CapsuleCollider capsuleCollider = (CapsuleCollider)(dAZPhysicsMeshSoftVerticesSet.jointCollider = gameObject5.AddComponent<CapsuleCollider>());
					if (_useSecondCollider)
					{
						capsuleCollider = (CapsuleCollider)(dAZPhysicsMeshSoftVerticesSet.jointCollider2 = gameObject5.AddComponent<CapsuleCollider>());
					}
					break;
				}
				case ColliderType.Sphere:
				{
					SphereCollider sphereCollider = (SphereCollider)(dAZPhysicsMeshSoftVerticesSet.jointCollider = gameObject5.AddComponent<SphereCollider>());
					if (_useSecondCollider)
					{
						sphereCollider = (SphereCollider)(dAZPhysicsMeshSoftVerticesSet.jointCollider2 = gameObject5.AddComponent<SphereCollider>());
					}
					break;
				}
				case ColliderType.Box:
				{
					BoxCollider boxCollider = gameObject5.AddComponent<BoxCollider>();
					if (_useSecondCollider)
					{
						boxCollider = (BoxCollider)(dAZPhysicsMeshSoftVerticesSet.jointCollider2 = gameObject5.AddComponent<BoxCollider>());
					}
					break;
				}
				}
				if (_colliderLayer != null && _colliderLayer != string.Empty)
				{
					gameObject5.layer = LayerMask.NameToLayer(_colliderLayer);
				}
				SetColliders(dAZPhysicsMeshSoftVerticesSet);
				SetMass(dAZPhysicsMeshSoftVerticesSet);
				dAZPhysicsMeshSoftVerticesSet.jointRB.centerOfMass = Vector3.zero;
				if (i == 0 && controller != null)
				{
					ConfigurableJoint configurableJoint = gameObject5.AddComponent<ConfigurableJoint>();
					configurableJoint.rotationDriveMode = RotationDriveMode.Slerp;
					configurableJoint.autoConfigureConnectedAnchor = false;
					configurableJoint.connectedAnchor = Vector3.zero;
					Rigidbody component2 = controller.GetComponent<Rigidbody>();
					controller.transform.position = gameObject5.transform.position;
					controller.transform.rotation = gameObject5.transform.rotation;
					if (component2 != null)
					{
						configurableJoint.connectedBody = component2;
					}
					controller.followWhenOffRB = dAZPhysicsMeshSoftVerticesSet.jointRB;
				}
			}
			InitColliders();
		}
		else
		{
			Debug.LogError("Can't create joints without up reference set");
		}
	}

	public void AdjustInitialTargetPositions()
	{
		Vector3[] visibleMorphedUVVertices = _skin.dazMesh.visibleMorphedUVVertices;
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			Vector3 vector = (dAZPhysicsMeshSoftVerticesSet.initialTargetPosition = ((!centerBetweenTargetAndAnchor) ? visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.targetVertex] : ((visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.targetVertex] + visibleMorphedUVVertices[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f)));
		}
	}

	public void AdjustLinkJointDistances()
	{
		if (!useLinkJoints)
		{
			return;
		}
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			for (int j = 0; j < dAZPhysicsMeshSoftVerticesSet.links.Count; j++)
			{
				DAZPhysicsMeshSoftVerticesSet setByID = GetSetByID(dAZPhysicsMeshSoftVerticesSet.links[j]);
				float magnitude = (dAZPhysicsMeshSoftVerticesSet.initialTargetPosition - setByID.initialTargetPosition).magnitude;
				SpringJoint springJoint = dAZPhysicsMeshSoftVerticesSet.linkJoints[j];
				springJoint.minDistance = magnitude;
				springJoint.maxDistance = magnitude;
			}
		}
	}

	public void ResetJoints()
	{
		if (!wasInit)
		{
			return;
		}
		_appliedBackForce.x = 0f;
		_appliedBackForce.y = 0f;
		_appliedBackForce.z = 0f;
		_bufferedBackForce.x = 0f;
		_bufferedBackForce.y = 0f;
		_bufferedBackForce.z = 0f;
		if (embedJoints)
		{
			embedTransform.position = _normalReference.position;
			embedTransform.rotation = _normalReference.rotation;
		}
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			Vector3 vector = ((!centerBetweenTargetAndAnchor) ? _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] : ((_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] + _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f));
			if (embedJoints)
			{
				dAZPhysicsMeshSoftVerticesSet.joint.connectedAnchor = _normalReference.InverseTransformPoint(vector);
			}
			else
			{
				dAZPhysicsMeshSoftVerticesSet.kinematicTransform.position = vector;
			}
			dAZPhysicsMeshSoftVerticesSet.jointTransform.position = vector;
			if (_useSimulation)
			{
				dAZPhysicsMeshSoftVerticesSet.lastPosition = dAZPhysicsMeshSoftVerticesSet.jointTransform.position;
				if (!_useUniformLimit && !embedJoints)
				{
					dAZPhysicsMeshSoftVerticesSet.jointTrackerTransform.position = dAZPhysicsMeshSoftVerticesSet.kinematicTransform.position;
				}
			}
			if (embedJoints)
			{
				continue;
			}
			Quaternion identity = Quaternion.identity;
			if (lookAtOption == LookAtOption.NormalReference || dAZPhysicsMeshSoftVerticesSet.forceLookAtReference)
			{
				identity.SetLookRotation(_normalReference.position - vector, _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex]);
			}
			else if (lookAtOption == LookAtOption.VertexNormal)
			{
				if (centerBetweenTargetAndAnchor)
				{
					identity.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f);
				}
				else
				{
					identity.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex]);
				}
			}
			else if (lookAtOption == LookAtOption.VertexNormalRefUp)
			{
				if (centerBetweenTargetAndAnchor)
				{
					identity.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f, _normalReference.position - vector);
				}
				else
				{
					identity.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex], _normalReference.position - vector);
				}
			}
			else if (lookAtOption == LookAtOption.VertexNormalAnchorUp)
			{
				if (centerBetweenTargetAndAnchor)
				{
					identity.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f, _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex]);
				}
				else
				{
					identity.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex], _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex]);
				}
			}
			else
			{
				identity.SetLookRotation(_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex], normalReference.position - vector);
			}
			dAZPhysicsMeshSoftVerticesSet.kinematicTransform.rotation = identity;
			dAZPhysicsMeshSoftVerticesSet.jointTransform.rotation = identity;
		}
	}

	public void PrepareUpdateJointsThreaded()
	{
		if (_normalReference != null)
		{
			_normalReferencePosition = _normalReference.position;
		}
	}

	public void UpdateJointTargetsThreaded()
	{
		if (embedJoints)
		{
			return;
		}
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			Vector3 vector = ((!centerBetweenTargetAndAnchor) ? _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] : ((_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] + _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f));
			dAZPhysicsMeshSoftVerticesSet.lastJointTargetPosition = dAZPhysicsMeshSoftVerticesSet.jointTargetPosition;
			dAZPhysicsMeshSoftVerticesSet.jointTargetPosition = vector;
			Quaternion identity = Quaternion.identity;
			if (lookAtOption == LookAtOption.NormalReference || dAZPhysicsMeshSoftVerticesSet.forceLookAtReference)
			{
				identity.SetLookRotation(_normalReferencePosition - vector, _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex]);
			}
			else if (lookAtOption == LookAtOption.VertexNormal)
			{
				if (centerBetweenTargetAndAnchor)
				{
					identity.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f);
				}
				else
				{
					identity.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex]);
				}
			}
			else if (lookAtOption == LookAtOption.VertexNormalRefUp)
			{
				if (centerBetweenTargetAndAnchor)
				{
					identity.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f, _normalReferencePosition - vector);
				}
				else
				{
					identity.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex], _normalReferencePosition - vector);
				}
			}
			else if (lookAtOption == LookAtOption.VertexNormalAnchorUp)
			{
				if (centerBetweenTargetAndAnchor)
				{
					identity.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f, _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex]);
				}
				else
				{
					identity.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet.targetVertex], _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - vector);
				}
			}
			else
			{
				identity.SetLookRotation(_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex], _normalReferencePosition - vector);
			}
			dAZPhysicsMeshSoftVerticesSet.jointTargetLookAt = identity;
		}
	}

	public void UpdateJoints()
	{
		if (!wasInit || !(_normalReference != null) || !_on)
		{
			return;
		}
		Vector3 vector = default(Vector3);
		vector.x = 0f;
		vector.y = 0f;
		vector.z = 0f;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		if (useJointBackForce && _jointBackForce > 0f)
		{
			if (_jointBackForceThresholdDistance == 0f)
			{
				num2 = 1E+20f;
				num3 = 1E+20f;
			}
			else
			{
				num2 = 1f / _jointBackForceThresholdDistance;
				num3 = num2 * num2;
			}
			num = _jointBackForceThresholdDistance * _jointBackForceThresholdDistance;
		}
		if (pauseSimulation)
		{
			ResetJoints();
			return;
		}
		if (_useSimulation)
		{
			if (embedJoints)
			{
				embedRB.MovePosition(_normalReference.position);
				embedRB.MoveRotation(_normalReference.rotation);
				for (int i = 0; i < _softVerticesSets.Count; i++)
				{
					DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
					Vector3 vector2 = ((!centerBetweenTargetAndAnchor) ? _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] : ((_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] + _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f));
					dAZPhysicsMeshSoftVerticesSet.joint.connectedAnchor = _normalReference.InverseTransformPoint(vector2);
					dAZPhysicsMeshSoftVerticesSet.lastPosition = dAZPhysicsMeshSoftVerticesSet.jointRB.position;
					if (_clampVelocity && dAZPhysicsMeshSoftVerticesSet.jointRB.velocity.sqrMagnitude > _maxSimulationVelocitySqr)
					{
						dAZPhysicsMeshSoftVerticesSet.jointRB.velocity = dAZPhysicsMeshSoftVerticesSet.jointRB.velocity.normalized * _maxSimulationVelocity;
					}
					if (useJointBackForce && _jointBackForce > 0f)
					{
						Vector3 vector3 = dAZPhysicsMeshSoftVerticesSet.jointTransform.position - vector2;
						float num4 = Mathf.Abs(vector3.x) + Mathf.Abs(vector3.y) + Mathf.Abs(vector3.z);
						if (num4 > _jointBackForceThresholdDistance)
						{
							float num5 = Mathf.Clamp01((num4 - _jointBackForceThresholdDistance) * num2);
							vector += vector3 * num5;
						}
					}
				}
			}
			else if (useThreading)
			{
				for (int j = 0; j < _softVerticesSets.Count; j++)
				{
					DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet2 = _softVerticesSets[j];
					dAZPhysicsMeshSoftVerticesSet2.kinematicRB.MovePosition(dAZPhysicsMeshSoftVerticesSet2.jointTargetPosition);
					dAZPhysicsMeshSoftVerticesSet2.kinematicRB.MoveRotation(dAZPhysicsMeshSoftVerticesSet2.jointTargetLookAt);
					if (!_useUniformLimit)
					{
						dAZPhysicsMeshSoftVerticesSet2.jointTrackerTransform.position = dAZPhysicsMeshSoftVerticesSet2.jointTransform.position;
						Vector3 localPosition = dAZPhysicsMeshSoftVerticesSet2.jointTrackerTransform.localPosition;
						bool flag = false;
						if (_normalMovementType == MovementType.Limit)
						{
							float num6 = _normalDistanceLimit * dAZPhysicsMeshSoftVerticesSet2.limitMultiplier;
							float num7 = _normalNegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet2.limitMultiplier;
							if (lookAtOption != 0)
							{
								if (localPosition.z > num6)
								{
									localPosition.z = num6;
									flag = true;
								}
								else if (localPosition.z < 0f - num7)
								{
									localPosition.z = 0f - num7;
									flag = true;
								}
							}
							else if (localPosition.y > num6)
							{
								localPosition.y = num6;
								flag = true;
							}
							else if (localPosition.y < 0f - num7)
							{
								localPosition.y = 0f - num7;
								flag = true;
							}
						}
						if (_tangentMovementType == MovementType.Limit)
						{
							float num8 = _tangentDistanceLimit * dAZPhysicsMeshSoftVerticesSet2.limitMultiplier;
							float num9 = _tangentNegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet2.limitMultiplier;
							if (lookAtOption != 0)
							{
								if (localPosition.y > num8)
								{
									localPosition.y = num8;
									flag = true;
								}
								else if (localPosition.y < 0f - num9)
								{
									localPosition.y = 0f - num9;
									flag = true;
								}
							}
							else if (localPosition.z > num8)
							{
								localPosition.z = num8;
								flag = true;
							}
							else if (localPosition.z < 0f - num9)
							{
								localPosition.z = 0f - num9;
								flag = true;
							}
						}
						if (_tangent2MovementType == MovementType.Limit)
						{
							float num10 = _tangent2DistanceLimit * dAZPhysicsMeshSoftVerticesSet2.limitMultiplier;
							float num11 = _tangent2NegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet2.limitMultiplier;
							if (localPosition.x > num10)
							{
								localPosition.x = num10;
								flag = true;
							}
							else if (localPosition.x < 0f - num11)
							{
								localPosition.x = 0f - num11;
								flag = true;
							}
						}
						if (flag)
						{
							dAZPhysicsMeshSoftVerticesSet2.jointTrackerTransform.localPosition = localPosition;
							dAZPhysicsMeshSoftVerticesSet2.jointRB.position = dAZPhysicsMeshSoftVerticesSet2.jointTrackerTransform.position;
						}
					}
					dAZPhysicsMeshSoftVerticesSet2.lastPosition = dAZPhysicsMeshSoftVerticesSet2.jointRB.position;
					if (useJointBackForce && _jointBackForce > 0f)
					{
						Vector3 vector4 = dAZPhysicsMeshSoftVerticesSet2.jointTransform.position - dAZPhysicsMeshSoftVerticesSet2.lastJointTargetPosition;
						if (vector4.x > _jointBackForceThresholdDistance)
						{
							vector4.x -= _jointBackForceThresholdDistance;
						}
						else if (vector4.x < 0f - _jointBackForceThresholdDistance)
						{
							vector4.x += _jointBackForceThresholdDistance;
						}
						else
						{
							vector4.x = 0f;
						}
						if (vector4.y > _jointBackForceThresholdDistance)
						{
							vector4.y -= _jointBackForceThresholdDistance;
						}
						else if (vector4.y < 0f - _jointBackForceThresholdDistance)
						{
							vector4.y += _jointBackForceThresholdDistance;
						}
						else
						{
							vector4.y = 0f;
						}
						if (vector4.z > _jointBackForceThresholdDistance)
						{
							vector4.z -= _jointBackForceThresholdDistance;
						}
						else if (vector4.z < 0f - _jointBackForceThresholdDistance)
						{
							vector4.z += _jointBackForceThresholdDistance;
						}
						else
						{
							vector4.z = 0f;
						}
						vector += vector4;
					}
				}
			}
			else
			{
				for (int k = 0; k < _softVerticesSets.Count; k++)
				{
					DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet3 = _softVerticesSets[k];
					Vector3 vector5 = ((!centerBetweenTargetAndAnchor) ? _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.targetVertex] : ((_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.targetVertex] + _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.anchorVertex]) * 0.5f));
					dAZPhysicsMeshSoftVerticesSet3.kinematicRB.MovePosition(vector5);
					Quaternion identity = Quaternion.identity;
					if (lookAtOption == LookAtOption.NormalReference || dAZPhysicsMeshSoftVerticesSet3.forceLookAtReference)
					{
						identity.SetLookRotation(_normalReference.position - vector5, _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.targetVertex]);
					}
					else if (lookAtOption == LookAtOption.VertexNormal)
					{
						if (centerBetweenTargetAndAnchor)
						{
							identity.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet3.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet3.anchorVertex]) * 0.5f);
						}
						else
						{
							identity.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet3.targetVertex]);
						}
					}
					else if (lookAtOption == LookAtOption.VertexNormalRefUp)
					{
						if (centerBetweenTargetAndAnchor)
						{
							identity.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet3.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet3.anchorVertex]) * 0.5f, _normalReference.position - vector5);
						}
						else
						{
							identity.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet3.targetVertex], _normalReference.position - vector5);
						}
					}
					else if (lookAtOption == LookAtOption.VertexNormalAnchorUp)
					{
						if (centerBetweenTargetAndAnchor)
						{
							identity.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet3.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet3.anchorVertex]) * 0.5f, _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.targetVertex]);
						}
						else
						{
							identity.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet3.targetVertex], _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.targetVertex]);
						}
					}
					else
					{
						identity.SetLookRotation(_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet3.targetVertex], normalReference.position - vector5);
					}
					dAZPhysicsMeshSoftVerticesSet3.kinematicRB.MoveRotation(identity);
					if (!_useUniformLimit)
					{
						dAZPhysicsMeshSoftVerticesSet3.jointTrackerTransform.position = dAZPhysicsMeshSoftVerticesSet3.jointTransform.position;
						Vector3 localPosition2 = dAZPhysicsMeshSoftVerticesSet3.jointTrackerTransform.localPosition;
						bool flag2 = false;
						if (_normalMovementType == MovementType.Limit)
						{
							float num12 = _normalDistanceLimit * dAZPhysicsMeshSoftVerticesSet3.limitMultiplier;
							float num13 = _normalNegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet3.limitMultiplier;
							if (lookAtOption != 0)
							{
								if (localPosition2.z > num12)
								{
									localPosition2.z = num12;
									flag2 = true;
								}
								else if (localPosition2.z < 0f - num13)
								{
									localPosition2.z = 0f - num13;
									flag2 = true;
								}
							}
							else if (localPosition2.y > num12)
							{
								localPosition2.y = num12;
								flag2 = true;
							}
							else if (localPosition2.y < 0f - num13)
							{
								localPosition2.y = 0f - num13;
								flag2 = true;
							}
						}
						if (_tangentMovementType == MovementType.Limit)
						{
							float num14 = _tangentDistanceLimit * dAZPhysicsMeshSoftVerticesSet3.limitMultiplier;
							float num15 = _tangentNegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet3.limitMultiplier;
							if (lookAtOption != 0)
							{
								if (localPosition2.y > num14)
								{
									localPosition2.y = num14;
									flag2 = true;
								}
								else if (localPosition2.y < 0f - num15)
								{
									localPosition2.y = 0f - num15;
									flag2 = true;
								}
							}
							else if (localPosition2.z > num14)
							{
								localPosition2.z = num14;
								flag2 = true;
							}
							else if (localPosition2.z < 0f - num15)
							{
								localPosition2.z = 0f - num15;
								flag2 = true;
							}
						}
						if (_tangent2MovementType == MovementType.Limit)
						{
							float num16 = _tangent2DistanceLimit * dAZPhysicsMeshSoftVerticesSet3.limitMultiplier;
							float num17 = _tangent2NegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet3.limitMultiplier;
							if (localPosition2.x > num16)
							{
								localPosition2.x = num16;
								flag2 = true;
							}
							else if (localPosition2.x < 0f - num17)
							{
								localPosition2.x = 0f - num17;
								flag2 = true;
							}
						}
						if (flag2)
						{
							dAZPhysicsMeshSoftVerticesSet3.jointTrackerTransform.localPosition = localPosition2;
							dAZPhysicsMeshSoftVerticesSet3.jointRB.position = dAZPhysicsMeshSoftVerticesSet3.jointTrackerTransform.position;
						}
					}
					dAZPhysicsMeshSoftVerticesSet3.lastPosition = dAZPhysicsMeshSoftVerticesSet3.jointRB.position;
					if (_clampVelocity && dAZPhysicsMeshSoftVerticesSet3.jointRB.velocity.sqrMagnitude > _maxSimulationVelocitySqr)
					{
						dAZPhysicsMeshSoftVerticesSet3.jointRB.velocity = dAZPhysicsMeshSoftVerticesSet3.jointRB.velocity.normalized * _maxSimulationVelocity;
					}
					if (useJointBackForce && _jointBackForce > 0f)
					{
						Vector3 vector6 = dAZPhysicsMeshSoftVerticesSet3.jointTransform.position - vector5;
						float num18 = Mathf.Abs(vector6.x) + Mathf.Abs(vector6.y) + Mathf.Abs(vector6.z);
						if (num18 > _jointBackForceThresholdDistance)
						{
							float num19 = Mathf.Clamp01((num18 - _jointBackForceThresholdDistance) * num2);
							vector += vector6 * num19;
						}
					}
				}
			}
		}
		else if (useThreading)
		{
			for (int l = 0; l < _softVerticesSets.Count; l++)
			{
				DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet4 = _softVerticesSets[l];
				dAZPhysicsMeshSoftVerticesSet4.kinematicTransform.SetPositionAndRotation(dAZPhysicsMeshSoftVerticesSet4.jointTargetPosition, dAZPhysicsMeshSoftVerticesSet4.jointTargetLookAt);
				if (!_useUniformLimit)
				{
					Vector3 localPosition3 = dAZPhysicsMeshSoftVerticesSet4.jointTransform.localPosition;
					bool flag3 = false;
					if (_normalMovementType == MovementType.Limit)
					{
						float num20 = _normalDistanceLimit * dAZPhysicsMeshSoftVerticesSet4.limitMultiplier;
						float num21 = _normalNegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet4.limitMultiplier;
						if (lookAtOption != 0)
						{
							if (localPosition3.z > num20)
							{
								localPosition3.z = num20;
								flag3 = true;
							}
							else if (localPosition3.z < 0f - num21)
							{
								localPosition3.z = 0f - num21;
								flag3 = true;
							}
						}
						else if (localPosition3.y > num20)
						{
							localPosition3.y = num20;
							flag3 = true;
						}
						else if (localPosition3.y < 0f - num21)
						{
							localPosition3.y = 0f - num21;
							flag3 = true;
						}
					}
					if (_tangentMovementType == MovementType.Limit)
					{
						float num22 = _tangentDistanceLimit * dAZPhysicsMeshSoftVerticesSet4.limitMultiplier;
						float num23 = _tangentNegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet4.limitMultiplier;
						if (lookAtOption != 0)
						{
							if (localPosition3.y > num22)
							{
								localPosition3.y = num22;
								flag3 = true;
							}
							else if (localPosition3.y < 0f - num23)
							{
								localPosition3.y = 0f - num23;
								flag3 = true;
							}
						}
						else if (localPosition3.z > num22)
						{
							localPosition3.z = num22;
							flag3 = true;
						}
						else if (localPosition3.z < 0f - num23)
						{
							localPosition3.z = 0f - num23;
							flag3 = true;
						}
					}
					if (_tangent2MovementType == MovementType.Limit)
					{
						float num24 = _tangent2DistanceLimit * dAZPhysicsMeshSoftVerticesSet4.limitMultiplier;
						float num25 = _tangent2NegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet4.limitMultiplier;
						if (localPosition3.x > num24)
						{
							localPosition3.x = num24;
							flag3 = true;
						}
						else if (localPosition3.x < 0f - num25)
						{
							localPosition3.x = 0f - num25;
							flag3 = true;
						}
					}
					if (flag3)
					{
						dAZPhysicsMeshSoftVerticesSet4.jointTransform.localPosition = localPosition3;
					}
				}
				if (useJointBackForce && _jointBackForce > 0f)
				{
					Vector3 vector7 = dAZPhysicsMeshSoftVerticesSet4.jointTransform.position - dAZPhysicsMeshSoftVerticesSet4.jointTargetPosition;
					float num26 = Mathf.Abs(vector7.x) + Mathf.Abs(vector7.y) + Mathf.Abs(vector7.z);
					if (num26 > _jointBackForceThresholdDistance)
					{
						float num27 = Mathf.Clamp01((num26 - _jointBackForceThresholdDistance) * num2);
						vector += vector7 * num27;
					}
				}
			}
		}
		else
		{
			for (int m = 0; m < _softVerticesSets.Count; m++)
			{
				DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet5 = _softVerticesSets[m];
				Vector3 vector8 = ((!centerBetweenTargetAndAnchor) ? _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.targetVertex] : ((_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.targetVertex] + _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.anchorVertex]) * 0.5f));
				dAZPhysicsMeshSoftVerticesSet5.kinematicRB.MovePosition(vector8);
				Quaternion identity2 = Quaternion.identity;
				if (lookAtOption == LookAtOption.NormalReference || dAZPhysicsMeshSoftVerticesSet5.forceLookAtReference)
				{
					identity2.SetLookRotation(_normalReference.position - vector8, _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.targetVertex]);
				}
				else if (lookAtOption == LookAtOption.VertexNormal)
				{
					if (centerBetweenTargetAndAnchor)
					{
						identity2.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet5.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet5.anchorVertex]) * 0.5f);
					}
					else
					{
						identity2.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet5.targetVertex]);
					}
				}
				else if (lookAtOption == LookAtOption.VertexNormalRefUp)
				{
					if (centerBetweenTargetAndAnchor)
					{
						identity2.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet5.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet5.anchorVertex]) * 0.5f, _normalReference.position - vector8);
					}
					else
					{
						identity2.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet5.targetVertex], _normalReference.position - vector8);
					}
				}
				else if (lookAtOption == LookAtOption.VertexNormalAnchorUp)
				{
					if (centerBetweenTargetAndAnchor)
					{
						identity2.SetLookRotation((-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet5.targetVertex] - _skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet5.anchorVertex]) * 0.5f, _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.targetVertex]);
					}
					else
					{
						identity2.SetLookRotation(-_skin.postSkinNormals[dAZPhysicsMeshSoftVerticesSet5.targetVertex], _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.targetVertex]);
					}
				}
				else
				{
					identity2.SetLookRotation(_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.anchorVertex] - _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet5.targetVertex], normalReference.position - vector8);
				}
				dAZPhysicsMeshSoftVerticesSet5.kinematicTransform.SetPositionAndRotation(vector8, identity2);
				if (!_useUniformLimit)
				{
					Vector3 localPosition4 = dAZPhysicsMeshSoftVerticesSet5.jointTransform.localPosition;
					bool flag4 = false;
					if (_normalMovementType == MovementType.Limit)
					{
						float num28 = _normalDistanceLimit * dAZPhysicsMeshSoftVerticesSet5.limitMultiplier;
						float num29 = _normalNegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet5.limitMultiplier;
						if (lookAtOption != 0)
						{
							if (localPosition4.z > num28)
							{
								localPosition4.z = num28;
								flag4 = true;
							}
							else if (localPosition4.z < 0f - num29)
							{
								localPosition4.z = 0f - num29;
								flag4 = true;
							}
						}
						else if (localPosition4.y > num28)
						{
							localPosition4.y = num28;
							flag4 = true;
						}
						else if (localPosition4.y < 0f - num29)
						{
							localPosition4.y = 0f - num29;
							flag4 = true;
						}
					}
					if (_tangentMovementType == MovementType.Limit)
					{
						float num30 = _tangentDistanceLimit * dAZPhysicsMeshSoftVerticesSet5.limitMultiplier;
						float num31 = _tangentNegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet5.limitMultiplier;
						if (lookAtOption != 0)
						{
							if (localPosition4.y > num30)
							{
								localPosition4.y = num30;
								flag4 = true;
							}
							else if (localPosition4.y < 0f - num31)
							{
								localPosition4.y = 0f - num31;
								flag4 = true;
							}
						}
						else if (localPosition4.z > num30)
						{
							localPosition4.z = num30;
							flag4 = true;
						}
						else if (localPosition4.z < 0f - num31)
						{
							localPosition4.z = 0f - num31;
							flag4 = true;
						}
					}
					if (_tangent2MovementType == MovementType.Limit)
					{
						float num32 = _tangent2DistanceLimit * dAZPhysicsMeshSoftVerticesSet5.limitMultiplier;
						float num33 = _tangent2NegativeDistanceLimit * dAZPhysicsMeshSoftVerticesSet5.limitMultiplier;
						if (localPosition4.x > num32)
						{
							localPosition4.x = num32;
							flag4 = true;
						}
						else if (localPosition4.x < 0f - num33)
						{
							localPosition4.x = 0f - num33;
							flag4 = true;
						}
					}
					if (flag4)
					{
						dAZPhysicsMeshSoftVerticesSet5.jointTransform.localPosition = localPosition4;
					}
				}
				if (useJointBackForce && _jointBackForce > 0f)
				{
					Vector3 vector9 = dAZPhysicsMeshSoftVerticesSet5.jointTransform.position - vector8;
					float num34 = Mathf.Abs(vector9.x) + Mathf.Abs(vector9.y) + Mathf.Abs(vector9.z);
					if (num34 > _jointBackForceThresholdDistance)
					{
						float num35 = Mathf.Clamp01((num34 - _jointBackForceThresholdDistance) * num2);
						vector += vector9 * num35;
					}
				}
			}
		}
		if (useJointBackForce && _jointBackForce > 0f && backForceRigidbody != null)
		{
			float num36 = ((!(TimeControl.singleton != null) || !TimeControl.singleton.compensateFixedTimestep) ? 1f : (1f / Time.timeScale));
			Vector3 vector10 = vector * _jointBackForce * num36;
			_appliedBackForce = Vector3.ClampMagnitude(vector10, _jointBackForceMaxForce);
		}
	}

	public void ResetAdjustJoints()
	{
		if (backForceAdjustJoints != null)
		{
			if (backForceAdjustJointsUseJoint2)
			{
				backForceAdjustJoints.additionalJoint2RotationX = 0f;
				backForceAdjustJoints.additionalJoint2RotationY = 0f;
				backForceAdjustJoints.additionalJoint2RotationZ = 0f;
			}
			else
			{
				backForceAdjustJoints.additionalJoint1RotationX = 0f;
				backForceAdjustJoints.additionalJoint1RotationY = 0f;
				backForceAdjustJoints.additionalJoint1RotationZ = 0f;
			}
		}
	}

	public void ApplyBackForce()
	{
		if (!useJointBackForce || !(_jointBackForce > 0f) || !(backForceRigidbody != null))
		{
			return;
		}
		float num = Time.fixedDeltaTime * 90f;
		_bufferedBackForce = Vector3.Lerp(_bufferedBackForce, _appliedBackForce, backForceResponse * num);
		if (backForceAdjustJoints != null)
		{
			Vector3 vector = backForceRigidbody.transform.InverseTransformVector(_bufferedBackForce);
			if (backForceAdjustJointsUseJoint2)
			{
				backForceAdjustJoints.additionalJoint2RotationX += vector.y;
				backForceAdjustJoints.additionalJoint2RotationX = Mathf.Clamp(backForceAdjustJoints.additionalJoint2RotationX, 0f - backForceAdjustJointsMaxAngle, backForceAdjustJointsMaxAngle);
				backForceAdjustJoints.additionalJoint2RotationY -= vector.x;
				backForceAdjustJoints.additionalJoint2RotationY = Mathf.Clamp(backForceAdjustJoints.additionalJoint2RotationY, 0f - backForceAdjustJointsMaxAngle, backForceAdjustJointsMaxAngle);
			}
			else
			{
				backForceAdjustJoints.additionalJoint1RotationX += vector.y;
				backForceAdjustJoints.additionalJoint1RotationX = Mathf.Clamp(backForceAdjustJoints.additionalJoint1RotationX, 0f - backForceAdjustJointsMaxAngle, backForceAdjustJointsMaxAngle);
				backForceAdjustJoints.additionalJoint1RotationY -= vector.x;
				backForceAdjustJoints.additionalJoint1RotationY = Mathf.Clamp(backForceAdjustJoints.additionalJoint1RotationY, 0f - backForceAdjustJointsMaxAngle, backForceAdjustJointsMaxAngle);
			}
			backForceAdjustJoints.SyncTargetRotation();
		}
		else
		{
			backForceRigidbody.AddForce(_bufferedBackForce, ForceMode.Force);
		}
	}

	public void PrepareMorphVerticesThreaded(float interpFactor, bool updateRB)
	{
		if (!wasInit || !(_normalReference != null) || !_on)
		{
			return;
		}
		bool flag = _useSimulation && _useInterpolation && useCustomInterpolation;
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			if (dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length > 0 && _influenceType == InfluenceType.DistanceAlongMoveVector)
			{
				if (lookAtOption == LookAtOption.Anchor)
				{
					dAZPhysicsMeshSoftVerticesSet.primaryMove = dAZPhysicsMeshSoftVerticesSet.kinematicTransform.forward;
				}
				else
				{
					dAZPhysicsMeshSoftVerticesSet.primaryMove = dAZPhysicsMeshSoftVerticesSet.kinematicTransform.up;
				}
			}
			Vector3 vector = ((!centerBetweenTargetAndAnchor) ? _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] : ((_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] + _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f));
			if (flag)
			{
				Vector3 vector2 = Vector3.Lerp(dAZPhysicsMeshSoftVerticesSet.lastPosition, dAZPhysicsMeshSoftVerticesSet.jointRB.position, interpFactor);
				dAZPhysicsMeshSoftVerticesSet.move = vector2 - vector;
			}
			else
			{
				dAZPhysicsMeshSoftVerticesSet.move = dAZPhysicsMeshSoftVerticesSet.jointTransform.position - vector;
			}
		}
	}

	public void MorphVerticesThreaded()
	{
		if (!wasInit || !_on)
		{
			return;
		}
		float num = 1f / _maxInfluenceDistance;
		bool flag = lookAtOption == LookAtOption.VertexNormal || lookAtOption == LookAtOption.VertexNormalRefUp || lookAtOption == LookAtOption.VertexNormalAnchorUp;
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			Vector3 vector = ((!centerBetweenTargetAndAnchor) ? _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] : ((_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] + _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f));
			if (dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length > 0 && _influenceType != InfluenceType.HardCopy)
			{
				if (_influenceType == InfluenceType.DistanceAlongMoveVector)
				{
					for (int j = 0; j < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; j++)
					{
						Vector3 rhs = _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.influenceVertices[j]] - vector;
						Vector3 vector2 = dAZPhysicsMeshSoftVerticesSet.primaryMove * Vector3.Dot(dAZPhysicsMeshSoftVerticesSet.primaryMove, rhs);
						dAZPhysicsMeshSoftVerticesSet.influenceVerticesDistances[j] = vector2.magnitude;
					}
				}
				else
				{
					for (int k = 0; k < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; k++)
					{
						Vector3 vector3 = _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.influenceVertices[k]] - vector;
						dAZPhysicsMeshSoftVerticesSet.influenceVerticesDistances[k] = vector3.magnitude;
					}
				}
			}
			if (!_skin.postSkinVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex])
			{
				_skin.postSkinVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] = true;
				_skin.postSkinVertsChanged = true;
			}
			if (flag && !_skin.postSkinNormalVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex])
			{
				_skin.postSkinNormalVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] = true;
				_skin.postSkinVertsChanged = true;
			}
			if (!_skin.postSkinVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex])
			{
				_skin.postSkinVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] = true;
				_skin.postSkinVertsChanged = true;
			}
			ref Vector3 reference = ref _skin.postSkinMorphs[dAZPhysicsMeshSoftVerticesSet.targetVertex];
			reference = dAZPhysicsMeshSoftVerticesSet.move;
			if (_influenceType == InfluenceType.HardCopy)
			{
				if (autoInfluenceAnchor)
				{
					ref Vector3 reference2 = ref _skin.postSkinMorphs[dAZPhysicsMeshSoftVerticesSet.anchorVertex];
					reference2 = dAZPhysicsMeshSoftVerticesSet.move;
				}
				for (int l = 0; l < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; l++)
				{
					int num2 = dAZPhysicsMeshSoftVerticesSet.influenceVertices[l];
					if (!_skin.postSkinVerts[num2])
					{
						_skin.postSkinVerts[num2] = true;
						_skin.postSkinVertsChanged = true;
					}
					ref Vector3 reference3 = ref _skin.postSkinMorphs[num2];
					reference3 = dAZPhysicsMeshSoftVerticesSet.move;
				}
				continue;
			}
			for (int m = 0; m < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; m++)
			{
				int num3 = dAZPhysicsMeshSoftVerticesSet.influenceVertices[m];
				float num4 = dAZPhysicsMeshSoftVerticesSet.influenceVerticesDistances[m];
				float f = Mathf.Min(1f, num4 * num);
				float num5 = 1f - Mathf.Pow(f, falloffPower);
				num5 = ((!(weightBias > 0f)) ? ((1f + weightBias) * num5) : ((1f - weightBias) * num5 + weightBias));
				dAZPhysicsMeshSoftVerticesSet.influenceVerticesWeights[m] = num5;
				if (!_skin.postSkinVerts[num3])
				{
					_skin.postSkinVerts[num3] = true;
					_skin.postSkinVertsChanged = true;
				}
				ref Vector3 reference4 = ref _skin.postSkinMorphs[num3];
				reference4 = dAZPhysicsMeshSoftVerticesSet.move * num5 * weightMultiplier;
			}
		}
	}

	public void MorphVertices(float interpFactor, bool updateRB)
	{
		if (!wasInit || !(_normalReference != null) || !_on)
		{
			return;
		}
		float num = 1f / _maxInfluenceDistance;
		bool flag = lookAtOption == LookAtOption.VertexNormal || lookAtOption == LookAtOption.VertexNormalRefUp || lookAtOption == LookAtOption.VertexNormalAnchorUp;
		bool flag2 = _useSimulation && _useInterpolation && useCustomInterpolation;
		for (int i = 0; i < _softVerticesSets.Count; i++)
		{
			DAZPhysicsMeshSoftVerticesSet dAZPhysicsMeshSoftVerticesSet = _softVerticesSets[i];
			Vector3 vector = ((!centerBetweenTargetAndAnchor) ? _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] : ((_skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] + _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex]) * 0.5f));
			if (dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length > 0 && _influenceType != InfluenceType.HardCopy)
			{
				if (_influenceType == InfluenceType.DistanceAlongMoveVector)
				{
					Vector3 vector2 = ((lookAtOption != 0) ? dAZPhysicsMeshSoftVerticesSet.kinematicTransform.up : dAZPhysicsMeshSoftVerticesSet.kinematicTransform.forward);
					for (int j = 0; j < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; j++)
					{
						Vector3 rhs = _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.influenceVertices[j]] - vector;
						Vector3 vector3 = vector2 * Vector3.Dot(vector2, rhs);
						dAZPhysicsMeshSoftVerticesSet.influenceVerticesDistances[j] = vector3.magnitude;
					}
				}
				else
				{
					for (int k = 0; k < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; k++)
					{
						Vector3 vector4 = _skin.rawSkinnedVerts[dAZPhysicsMeshSoftVerticesSet.influenceVertices[k]] - vector;
						dAZPhysicsMeshSoftVerticesSet.influenceVerticesDistances[k] = vector4.magnitude;
					}
				}
			}
			Vector3 vector6;
			if (flag2)
			{
				Vector3 vector5 = Vector3.Lerp(dAZPhysicsMeshSoftVerticesSet.lastPosition, dAZPhysicsMeshSoftVerticesSet.jointRB.position, interpFactor);
				vector6 = vector5 - vector;
			}
			else
			{
				vector6 = dAZPhysicsMeshSoftVerticesSet.jointTransform.position - vector;
			}
			if (!_skin.postSkinVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex])
			{
				_skin.postSkinVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] = true;
				_skin.postSkinVertsChanged = true;
			}
			if (flag)
			{
				if (!_skin.postSkinNormalVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex])
				{
					_skin.postSkinNormalVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] = true;
					_skin.postSkinVertsChanged = true;
				}
				if (centerBetweenTargetAndAnchor && !_skin.postSkinNormalVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex])
				{
					_skin.postSkinNormalVerts[dAZPhysicsMeshSoftVerticesSet.anchorVertex] = true;
				}
			}
			if (!_skin.postSkinVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex])
			{
				_skin.postSkinVerts[dAZPhysicsMeshSoftVerticesSet.targetVertex] = true;
				_skin.postSkinVertsChanged = true;
			}
			_skin.postSkinMorphs[dAZPhysicsMeshSoftVerticesSet.targetVertex] = vector6;
			if (_influenceType == InfluenceType.HardCopy)
			{
				if (autoInfluenceAnchor)
				{
					_skin.postSkinMorphs[dAZPhysicsMeshSoftVerticesSet.anchorVertex] = vector6;
				}
				for (int l = 0; l < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; l++)
				{
					int num2 = dAZPhysicsMeshSoftVerticesSet.influenceVertices[l];
					if (!_skin.postSkinVerts[num2])
					{
						_skin.postSkinVerts[num2] = true;
						_skin.postSkinVertsChanged = true;
					}
					_skin.postSkinMorphs[num2] = vector6;
				}
				continue;
			}
			for (int m = 0; m < dAZPhysicsMeshSoftVerticesSet.influenceVertices.Length; m++)
			{
				int num3 = dAZPhysicsMeshSoftVerticesSet.influenceVertices[m];
				float num4 = dAZPhysicsMeshSoftVerticesSet.influenceVerticesDistances[m];
				float f = Mathf.Min(1f, num4 * num);
				float num5 = 1f - Mathf.Pow(f, falloffPower);
				num5 = ((!(weightBias > 0f)) ? ((1f + weightBias) * num5) : ((1f - weightBias) * num5 + weightBias));
				dAZPhysicsMeshSoftVerticesSet.influenceVerticesWeights[m] = num5;
				if (!_skin.postSkinVerts[num3])
				{
					_skin.postSkinVerts[num3] = true;
					_skin.postSkinVertsChanged = true;
				}
				ref Vector3 reference = ref _skin.postSkinMorphs[num3];
				reference = vector6 * num5 * weightMultiplier;
			}
		}
	}
}
