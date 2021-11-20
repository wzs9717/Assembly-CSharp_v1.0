using System.Collections.Generic;
using UnityEngine;

public class DAZPhysicsMeshSelection : MonoBehaviour
{
	public enum SelectionMode
	{
		Target,
		Anchor,
		Influenced
	}

	public Transform meshTransform;

	public DAZMesh mesh;

	[SerializeField]
	private DAZSkinV2 _skin;

	public SelectionMode selectionMode;

	public string selectionName;

	[SerializeField]
	protected int _subMeshSelection = -1;

	[SerializeField]
	protected int _targetVertex = -1;

	[SerializeField]
	protected int _anchorVertex = -1;

	[SerializeField]
	protected List<int> _influenceVertices;

	protected Dictionary<int, bool> influenceVerticesDict;

	public int[] influenceVerticesArray;

	public float[] influenceVerticesWeights;

	public float maxInfluenceDistance = 0.02f;

	public bool showSelection;

	public bool showBackfaces;

	public bool showLinkLines;

	public float handleSize = 0.0005f;

	[SerializeField]
	protected bool _changed;

	public string colliderLayer = "SkinMorph";

	public PhysicMaterial jointMaterial;

	private Transform kinematicTransform;

	private Rigidbody kinematicRB;

	private Transform jointTransform;

	private Rigidbody jointRB;

	private ConfigurableJoint joint;

	private SphereCollider jointCollider;

	[SerializeField]
	private float _jointMass = 0.01f;

	[SerializeField]
	private float _colliderRadius = 0.002f;

	[SerializeField]
	private float _weightBias;

	[SerializeField]
	private bool _usePrimaryLimit = true;

	[SerializeField]
	private bool _useSecondaryLimit = true;

	[SerializeField]
	private float _hardLimit = 0.01f;

	private bool wasInit;

	public DAZSkinV2 skin
	{
		get
		{
			return _skin;
		}
		set
		{
			_skin = value;
			if (_skin != null)
			{
				mesh = _skin.dazMesh;
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

	public int targetVertex
	{
		get
		{
			return _targetVertex;
		}
		set
		{
			if (_targetVertex != value)
			{
				_changed = true;
				_targetVertex = value;
				if (IsVertexInfluenced(_targetVertex))
				{
					DeselectInfluencedVertex(_targetVertex);
				}
			}
		}
	}

	public int anchorVertex
	{
		get
		{
			return _anchorVertex;
		}
		set
		{
			if (_anchorVertex != value)
			{
				_changed = true;
				_anchorVertex = value;
			}
		}
	}

	public List<int> influenceVertices => _influenceVertices;

	public bool changed => _changed;

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
				if (jointRB != null)
				{
					jointRB.mass = _jointMass;
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
				if (jointCollider != null)
				{
					jointCollider.radius = value;
				}
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

	public bool usePrimaryLimit
	{
		get
		{
			return _usePrimaryLimit;
		}
		set
		{
			if (_usePrimaryLimit == value)
			{
				return;
			}
			_usePrimaryLimit = value;
			if (joint != null)
			{
				if (_usePrimaryLimit)
				{
					joint.zMotion = ConfigurableJointMotion.Limited;
				}
				else
				{
					joint.zMotion = ConfigurableJointMotion.Free;
				}
			}
		}
	}

	public bool useSecondaryLimit
	{
		get
		{
			return _useSecondaryLimit;
		}
		set
		{
			if (_useSecondaryLimit == value)
			{
				return;
			}
			_useSecondaryLimit = value;
			if (joint != null)
			{
				if (_useSecondaryLimit)
				{
					joint.xMotion = ConfigurableJointMotion.Limited;
					joint.yMotion = ConfigurableJointMotion.Limited;
				}
				else
				{
					joint.xMotion = ConfigurableJointMotion.Free;
					joint.yMotion = ConfigurableJointMotion.Free;
				}
			}
		}
	}

	public float hardLimit
	{
		get
		{
			return _hardLimit;
		}
		set
		{
			if (_hardLimit != value)
			{
				_hardLimit = value;
				if (joint != null)
				{
					SoftJointLimit linearLimit = joint.linearLimit;
					linearLimit.limit = _hardLimit;
					joint.linearLimit = linearLimit;
				}
			}
		}
	}

	public void clearChanged()
	{
		_changed = false;
	}

	protected void InitList(bool force = false)
	{
		if (_influenceVertices == null || force)
		{
			_influenceVertices = new List<int>();
		}
	}

	protected void InitDict(bool force = false)
	{
		InitList(force);
		if (influenceVerticesDict != null && !force)
		{
			return;
		}
		influenceVerticesDict = new Dictionary<int, bool>();
		foreach (int influenceVertex in influenceVertices)
		{
			influenceVerticesDict.Add(influenceVertex, value: true);
		}
	}

	public bool IsVertexInfluenced(int vid)
	{
		InitDict();
		return influenceVerticesDict.ContainsKey(vid);
	}

	public void SelectInfluencedVertex(int vid)
	{
		InitDict();
		if (vid >= 0 && vid <= mesh.numUVVertices && !influenceVerticesDict.ContainsKey(vid) && vid != _targetVertex)
		{
			_changed = true;
			_influenceVertices.Add(vid);
			influenceVerticesDict.Add(vid, value: true);
		}
	}

	public void DeselectInfluencedVertex(int vid)
	{
		InitDict();
		if (influenceVerticesDict.ContainsKey(vid))
		{
			_changed = true;
			_influenceVertices.Remove(vid);
			influenceVerticesDict.Remove(vid);
		}
	}

	public void ToggleInfluencedVertexSelection(int vid)
	{
		InitDict();
		_changed = true;
		if (influenceVerticesDict.ContainsKey(vid))
		{
			_influenceVertices.Remove(vid);
			influenceVerticesDict.Remove(vid);
		}
		else if (vid != _targetVertex)
		{
			_influenceVertices.Add(vid);
			influenceVerticesDict.Add(vid, value: true);
		}
	}

	public void ClearInfluencedSelection()
	{
		_changed = true;
		InitDict(force: true);
	}

	private void Init()
	{
		if (!wasInit && skin != null && skin.rawSkinnedVerts != null)
		{
			wasInit = true;
			CreateJoint();
			InitWeights();
		}
	}

	private void InitWeights()
	{
		influenceVerticesArray = new int[influenceVertices.Count];
		influenceVerticesWeights = new float[influenceVertices.Count];
		int num = 0;
		foreach (int influenceVertex in influenceVertices)
		{
			influenceVerticesArray[num] = influenceVertex;
			float num2 = Mathf.Max(0f, 1f - (skin.rawSkinnedVerts[influenceVertex] - skin.rawSkinnedVerts[targetVertex]).magnitude / maxInfluenceDistance);
			influenceVerticesWeights[num] = num2;
			num++;
		}
	}

	private void CreateJoint()
	{
		GameObject gameObject = new GameObject("PhysicsMeshKRB");
		kinematicTransform = gameObject.transform;
		kinematicTransform.SetParent(base.transform);
		kinematicRB = gameObject.AddComponent<Rigidbody>();
		kinematicRB.isKinematic = true;
		kinematicTransform.position = skin.rawSkinnedVerts[targetVertex];
		kinematicTransform.LookAt(skin.rawSkinnedVerts[anchorVertex]);
		GameObject gameObject2 = new GameObject("PhysicsMeshJoint");
		jointTransform = gameObject2.transform;
		jointTransform.SetParent(kinematicTransform);
		jointTransform.localPosition = Vector3.zero;
		jointTransform.localRotation = Quaternion.identity;
		jointRB = gameObject2.AddComponent<Rigidbody>();
		jointRB.mass = _jointMass;
		jointRB.useGravity = false;
		jointRB.drag = 0f;
		jointRB.angularDrag = 0f;
		jointRB.interpolation = RigidbodyInterpolation.None;
		jointRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
		jointRB.isKinematic = false;
		jointRB.constraints = RigidbodyConstraints.FreezeRotation;
		joint = gameObject2.AddComponent<ConfigurableJoint>();
		if (_usePrimaryLimit)
		{
			joint.zMotion = ConfigurableJointMotion.Limited;
		}
		else
		{
			joint.zMotion = ConfigurableJointMotion.Free;
		}
		if (_useSecondaryLimit)
		{
			joint.xMotion = ConfigurableJointMotion.Limited;
			joint.yMotion = ConfigurableJointMotion.Limited;
		}
		else
		{
			joint.xMotion = ConfigurableJointMotion.Free;
			joint.yMotion = ConfigurableJointMotion.Free;
		}
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;
		JointDrive zDrive = default(JointDrive);
		zDrive.positionSpring = 4f;
		zDrive.positionDamper = 0.4f;
		zDrive.maximumForce = 100000f;
		JointDrive jointDrive = default(JointDrive);
		jointDrive.positionSpring = 40f;
		jointDrive.positionDamper = 4f;
		jointDrive.maximumForce = 100000f;
		joint.xDrive = jointDrive;
		joint.yDrive = jointDrive;
		joint.zDrive = zDrive;
		SoftJointLimit linearLimit = joint.linearLimit;
		linearLimit.limit = _hardLimit;
		joint.linearLimit = linearLimit;
		joint.connectedBody = kinematicRB;
		jointCollider = gameObject2.AddComponent<SphereCollider>();
		jointCollider.radius = _colliderRadius;
		if (jointMaterial != null)
		{
			jointCollider.sharedMaterial = jointMaterial;
		}
		gameObject2.layer = LayerMask.NameToLayer(colliderLayer);
	}

	private void UpdateJoint()
	{
		if (wasInit)
		{
			kinematicTransform.position = skin.rawSkinnedVerts[targetVertex];
			kinematicTransform.LookAt(skin.rawSkinnedVerts[anchorVertex]);
			Vector3 localPosition = jointTransform.localPosition;
			if (localPosition.z < 0f)
			{
				localPosition.z = 0f;
				jointTransform.localPosition = localPosition;
			}
		}
	}

	private void MorphVertices()
	{
		if (wasInit)
		{
			Vector3 vector = jointRB.position - skin.rawSkinnedVerts[targetVertex];
			skin.postSkinMorphs[targetVertex] = vector;
			for (int i = 0; i < influenceVerticesArray.Length; i++)
			{
				int num = influenceVerticesArray[i];
				float num2 = influenceVerticesWeights[i];
				num2 = ((!(weightBias > 0f)) ? ((1f + weightBias) * num2) : ((1f - weightBias) * num2 + weightBias));
				ref Vector3 reference = ref skin.postSkinMorphs[num];
				reference = vector * num2 * 1.1f;
			}
		}
	}

	private void FixedUpdate()
	{
		Init();
		UpdateJoint();
		MorphVertices();
	}
}
