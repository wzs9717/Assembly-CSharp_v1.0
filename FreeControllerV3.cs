using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class FreeControllerV3 : JSONStorable
{
	public enum SelectLinkState
	{
		PositionAndRotation,
		Position,
		Rotation
	}

	public enum PositionState
	{
		On,
		Off,
		Following,
		Hold,
		Lock,
		ParentLink,
		PhysicsLink
	}

	public enum RotationState
	{
		On,
		Off,
		Following,
		Hold,
		Lock,
		LookAt,
		ParentLink,
		PhysicsLink
	}

	public enum MoveAxisnames
	{
		X,
		Y,
		Z,
		CameraRight,
		CameraUp,
		CameraForward,
		CameraRightNoY,
		CameraForwardNoY,
		None
	}

	public enum RotateAxisnames
	{
		X,
		Y,
		Z,
		NegX,
		NegY,
		NegZ,
		WorldY,
		None
	}

	public enum DrawAxisnames
	{
		X,
		Y,
		Z,
		NegX,
		NegY,
		NegZ
	}

	public enum ControlMode
	{
		Off,
		Position,
		Rotation
	}

	public static float targetAlpha = 1f;

	public Atom containingAtom;

	public UIPopup linkToSelectionPopup;

	public UIPopup linkToAtomSelectionPopup;

	private Rigidbody _linkToRB;

	private Transform _linkToConnector;

	private ConfigurableJoint _linkToJoint;

	protected string linkToAtomUID;

	private PositionState preLinkPositionState;

	private RotationState preLinkRotationState;

	public Button selectLinkToFromSceneButton;

	public Button selectAlignToFromSceneButton;

	public PositionState startingPositionState;

	public RotationState startingRotationState;

	public bool stateCanBeModified = true;

	public ToggleGroupValue positionToggleGroup;

	private PositionState _currentPositionState;

	public ToggleGroupValue rotationToggleGroup;

	private RotationState _currentRotationState;

	public Toggle xLockToggle;

	[SerializeField]
	private bool _xLock;

	public Toggle yLockToggle;

	[SerializeField]
	private bool _yLock;

	public Toggle zLockToggle;

	[SerializeField]
	private bool _zLock;

	public Toggle xRotLockToggle;

	[SerializeField]
	private bool _xRotLock;

	public Toggle yRotLockToggle;

	[SerializeField]
	private bool _yRotLock;

	public Toggle zRotLockToggle;

	[SerializeField]
	private bool _zRotLock;

	public SetTextFromFloat xPositionText;

	public Button xPositionMinus1Button;

	public Button xPositionMinusPoint1Button;

	public Button xPositionMinusPoint01Button;

	public Button xPosition0Button;

	public Button xPositionPlusPoint01Button;

	public Button xPositionPlusPoint1Button;

	public Button xPositionPlus1Button;

	public Button xPositionSnapPoint1Button;

	public SetTextFromFloat yPositionText;

	public Button yPositionMinus1Button;

	public Button yPositionMinusPoint1Button;

	public Button yPositionMinusPoint01Button;

	public Button yPosition0Button;

	public Button yPositionPlusPoint01Button;

	public Button yPositionPlusPoint1Button;

	public Button yPositionPlus1Button;

	public Button yPositionSnapPoint1Button;

	public SetTextFromFloat zPositionText;

	public Button zPositionMinus1Button;

	public Button zPositionMinusPoint1Button;

	public Button zPositionMinusPoint01Button;

	public Button zPosition0Button;

	public Button zPositionPlusPoint01Button;

	public Button zPositionPlusPoint1Button;

	public Button zPositionPlus1Button;

	public Button zPositionSnapPoint1Button;

	public SetTextFromFloat xRotationText;

	public Button xRotationMinus45Button;

	public Button xRotationMinus5Button;

	public Button xRotationMinusPoint5Button;

	public Button xRotation0Button;

	public Button xRotationPlusPoint5Button;

	public Button xRotationPlus5Button;

	public Button xRotationPlus45Button;

	public Button xRotationSnap1Button;

	public SetTextFromFloat yRotationText;

	public Button yRotationMinus45Button;

	public Button yRotationMinus5Button;

	public Button yRotationMinusPoint5Button;

	public Button yRotation0Button;

	public Button yRotationPlusPoint5Button;

	public Button yRotationPlus5Button;

	public Button yRotationPlus45Button;

	public Button yRotationSnap1Button;

	public SetTextFromFloat zRotationText;

	public Button zRotationMinus45Button;

	public Button zRotationMinus5Button;

	public Button zRotationMinusPoint5Button;

	public Button zRotation0Button;

	public Button zRotationPlusPoint5Button;

	public Button zRotationPlus5Button;

	public Button zRotationPlus45Button;

	public Button zRotationSnap1Button;

	public Toggle interactableInPlayModeToggle;

	private bool _startingInteractableInPlayMode;

	[SerializeField]
	private bool _interactableInPlayMode = true;

	public Toggle useGravityOnRBWhenOffToggle;

	private bool _startingUseGravityOnRBWhenOff;

	[SerializeField]
	private bool _useGravityOnRBWhenOff = true;

	public Toggle physicsEnabledToggle;

	private bool _startingPhysicsEnabled;

	public Toggle collisionEnabledToggle;

	private bool _startingCollsionEnabled;

	private Rigidbody _followWhenOffRB;

	private Rigidbody kinematicRB;

	private ConfigurableJoint connectedJoint;

	private ConfigurableJoint naturalJoint;

	public bool useForceWhenOff = true;

	public float distanceHolder;

	public float forceFactor = 10000f;

	public float torqueFactor = 2000f;

	public Slider RBMassSlider;

	[SerializeField]
	private float _RBLockPositionSpring = 250000f;

	[SerializeField]
	private float _RBLockPositionDamper = 250f;

	[SerializeField]
	public float _RBLockPositionMaxForce = 1E+08f;

	public Slider holdPositionSpringSlider;

	[SerializeField]
	private float _RBHoldPositionSpring = 1000f;

	public Slider holdPositionDamperSlider;

	[SerializeField]
	private float _RBHoldPositionDamper = 50f;

	public Slider holdPositionMaxForceSlider;

	[SerializeField]
	private float _RBHoldPositionMaxForce = 10000f;

	public Slider linkPositionSpringSlider;

	[SerializeField]
	private float _RBLinkPositionSpring = 250000f;

	public Slider linkPositionDamperSlider;

	[SerializeField]
	private float _RBLinkPositionDamper = 250f;

	public Slider linkPositionMaxForceSlider;

	[SerializeField]
	private float _RBLinkPositionMaxForce = 1E+08f;

	[SerializeField]
	private float _RBLockRotationSpring = 250000f;

	[SerializeField]
	private float _RBLockRotationDamper = 250f;

	[SerializeField]
	public float _RBLockRotationMaxForce = 1E+08f;

	public Slider holdRotationSpringSlider;

	[SerializeField]
	private float _RBHoldRotationSpring = 1000f;

	public Slider holdRotationDamperSlider;

	[SerializeField]
	private float _RBHoldRotationDamper = 50f;

	public Slider holdRotationMaxForceSlider;

	[SerializeField]
	private float _RBHoldRotationMaxForce = 10000f;

	public Slider linkRotationSpringSlider;

	[SerializeField]
	private float _RBLinkRotationSpring = 250000f;

	public Slider linkRotationDamperSlider;

	[SerializeField]
	private float _RBLinkRotationDamper = 250f;

	public Slider linkRotationMaxForceSlider;

	[SerializeField]
	private float _RBLinkRotationMaxForce = 1E+08f;

	public Slider jointRotationDriveSpringSlider;

	[SerializeField]
	private float _jointRotationDriveSpring;

	public Slider jointRotationDriveDamperSlider;

	[SerializeField]
	private float _jointRotationDriveDamper;

	public Slider jointRotationDriveMaxForceSlider;

	[SerializeField]
	private float _jointRotationDriveMaxForce;

	public Slider jointRotationDriveXTargetSlider;

	private float _jointRotationDriveXTargetMin;

	private float _jointRotationDriveXTargetMax;

	[SerializeField]
	private float _jointRotationDriveXTarget;

	public Slider jointRotationDriveYTargetSlider;

	private float _jointRotationDriveYTargetMin;

	private float _jointRotationDriveYTargetMax;

	[SerializeField]
	private float _jointRotationDriveYTarget;

	public Slider jointRotationDriveZTargetSlider;

	private float _jointRotationDriveZTargetMin;

	private float _jointRotationDriveZTargetMax;

	[SerializeField]
	private float _jointRotationDriveZTarget;

	public Text UIDText;

	public Text UIDTextAlt;

	public Transform[] UITransforms;

	public Transform[] UITransformsPlayMode;

	public bool GUIalwaysVisibleWhenSelected;

	public bool useContainedMeshRenderers = true;

	private bool _hidden;

	private bool _guihidden = true;

	public float unhighlightedScale = 0.5f;

	public float highlightedScale = 0.5f;

	public float selectedScale = 1f;

	private bool _highlighted;

	private Vector3 _selectedPosition;

	private bool _selected;

	public Color onColor = new Color(0f, 1f, 0f, 0.5f);

	public Color offColor = new Color(1f, 0f, 0f, 0.5f);

	public Color followingColor = new Color(1f, 0f, 1f, 0.5f);

	public Color holdColor = new Color(1f, 0.5f, 0f, 0.5f);

	public Color lockColor = new Color(0.5f, 0.25f, 0f, 0.5f);

	public Color lookAtColor = new Color(0f, 1f, 1f, 0.5f);

	public Color highlightColor = new Color(1f, 1f, 0f, 0.5f);

	public Color selectedColor = new Color(0f, 0f, 1f, 0.5f);

	public Color overlayColor = new Color(1f, 1f, 1f, 0.5f);

	private Color _currentPositionColor;

	private Color _currentRotationColor;

	public Material material;

	public Material linkLineMaterial;

	private LineDrawer linkLineDrawer;

	private Material positionMaterialLocal;

	private Material rotationMaterialLocal;

	private Material materialOverlay;

	public float meshScale = 0.5f;

	private Mesh _currentPositionMesh;

	private Mesh _currentRotationMesh;

	public Mesh onPositionMesh;

	public Mesh offPositionMesh;

	public Mesh followingPositionMesh;

	public Mesh holdPositionMesh;

	public Mesh lockPositionMesh;

	public Mesh onRotationMesh;

	public Mesh offRotationMesh;

	public Mesh followingRotationMesh;

	public Mesh holdRotationMesh;

	public Mesh lockRotationMesh;

	public Mesh lookAtRotationMesh;

	public Mesh moveModeOverlayMesh;

	public Mesh rotateModeOverlayMesh;

	public bool debug;

	public Transform control;

	public Transform follow;

	public Transform followWhenOff;

	public Transform lookAt;

	public Transform alsoMoveWhenInactive;

	public MoveAxisnames MoveAxis1 = MoveAxisnames.CameraRightNoY;

	public MoveAxisnames MoveAxis2 = MoveAxisnames.CameraForwardNoY;

	public MoveAxisnames MoveAxis3 = MoveAxisnames.Y;

	public RotateAxisnames RotateAxis1 = RotateAxisnames.Z;

	public RotateAxisnames RotateAxis2;

	public RotateAxisnames RotateAxis3 = RotateAxisnames.Y;

	public DrawAxisnames MeshForwardAxis = DrawAxisnames.Y;

	public DrawAxisnames MeshUpAxis = DrawAxisnames.Z;

	public DrawAxisnames DrawForwardAxis = DrawAxisnames.Z;

	public DrawAxisnames DrawUpAxis = DrawAxisnames.Y;

	public float moveFactor = 1f;

	public float rotateFactor = 60f;

	private bool _moveEnabled = true;

	private bool _moveForceEnabled;

	private bool _rotationEnabled = true;

	private bool _rotationForceEnabled;

	private Vector3 appliedForce;

	private Vector3 appliedTorque;

	private ControlMode _controlMode = ControlMode.Position;

	public Vector3 startingPosition;

	public Quaternion startingRotation;

	private Vector3 initialLocalPosition;

	private Quaternion initialLocalRotation;

	private MeshRenderer[] mrs;

	private bool wasInit;

	public Rigidbody linkToRB
	{
		get
		{
			return _linkToRB;
		}
		set
		{
			if (!(_linkToRB != value))
			{
				return;
			}
			if (_linkToConnector != null)
			{
				UnityEngine.Object.DestroyImmediate(_linkToConnector.gameObject);
				_linkToConnector = null;
			}
			if (_linkToJoint != null)
			{
				UnityEngine.Object.DestroyImmediate(_linkToJoint);
				_linkToJoint = null;
			}
			_linkToRB = value;
			if (_linkToRB != null)
			{
				if (_followWhenOffRB != null && _linkToRB != null)
				{
					GameObject gameObject = _followWhenOffRB.gameObject;
					_linkToJoint = gameObject.AddComponent<ConfigurableJoint>();
					_linkToJoint.connectedBody = _linkToRB;
					_linkToJoint.xMotion = ConfigurableJointMotion.Free;
					_linkToJoint.yMotion = ConfigurableJointMotion.Free;
					_linkToJoint.zMotion = ConfigurableJointMotion.Free;
					_linkToJoint.angularXMotion = ConfigurableJointMotion.Free;
					_linkToJoint.angularYMotion = ConfigurableJointMotion.Free;
					_linkToJoint.angularZMotion = ConfigurableJointMotion.Free;
					_linkToJoint.rotationDriveMode = RotationDriveMode.Slerp;
					SetLinkedJointSprings();
				}
				GameObject gameObject2 = new GameObject();
				_linkToConnector = gameObject2.transform;
				_linkToConnector.position = base.transform.position;
				_linkToConnector.rotation = base.transform.rotation;
				_linkToConnector.SetParent(_linkToRB.transform);
			}
		}
	}

	public PositionState currentPositionState
	{
		get
		{
			return _currentPositionState;
		}
		set
		{
			if (!stateCanBeModified)
			{
				return;
			}
			_currentPositionState = value;
			switch (_currentPositionState)
			{
			case PositionState.On:
			case PositionState.Following:
			case PositionState.Hold:
			case PositionState.Lock:
			case PositionState.ParentLink:
			case PositionState.PhysicsLink:
				if (_followWhenOffRB != null)
				{
					_followWhenOffRB.useGravity = _useGravityOnRBWhenOff;
				}
				break;
			case PositionState.Off:
				if (_followWhenOffRB != null)
				{
					_followWhenOffRB.useGravity = _useGravityOnRBWhenOff;
				}
				break;
			}
			switch (_currentPositionState)
			{
			case PositionState.On:
				_moveEnabled = true;
				_moveForceEnabled = false;
				break;
			case PositionState.Off:
			case PositionState.Following:
			case PositionState.Hold:
				_moveEnabled = useForceWhenOff;
				_moveForceEnabled = useForceWhenOff;
				break;
			case PositionState.ParentLink:
			case PositionState.PhysicsLink:
				_moveEnabled = useForceWhenOff;
				_moveForceEnabled = useForceWhenOff;
				if (_linkToConnector != null)
				{
					_linkToConnector.position = base.transform.position;
				}
				if (_linkToJoint != null)
				{
					_linkToJoint.connectedBody = null;
					_linkToJoint.connectedBody = _linkToRB;
				}
				break;
			case PositionState.Lock:
				_moveEnabled = false;
				_moveForceEnabled = false;
				if (_followWhenOffRB != null)
				{
					control.position = _followWhenOffRB.position;
				}
				break;
			}
			if (positionToggleGroup != null)
			{
				positionToggleGroup.activeToggleName = _currentPositionState.ToString();
			}
			SetLinkedJointSprings();
			SetJointSprings();
			StateChanged();
		}
	}

	public RotationState currentRotationState
	{
		get
		{
			return _currentRotationState;
		}
		set
		{
			if (!stateCanBeModified)
			{
				return;
			}
			_currentRotationState = value;
			switch (_currentRotationState)
			{
			case RotationState.On:
				_rotationEnabled = true;
				_rotationForceEnabled = false;
				break;
			case RotationState.Off:
			case RotationState.Following:
			case RotationState.Hold:
			case RotationState.LookAt:
				_rotationEnabled = useForceWhenOff;
				_rotationForceEnabled = useForceWhenOff;
				break;
			case RotationState.ParentLink:
			case RotationState.PhysicsLink:
				_rotationEnabled = useForceWhenOff;
				_rotationForceEnabled = useForceWhenOff;
				if (_linkToConnector != null)
				{
					_linkToConnector.rotation = base.transform.rotation;
				}
				if (_linkToJoint != null)
				{
					_linkToJoint.connectedBody = null;
					_linkToJoint.connectedBody = _linkToRB;
				}
				break;
			case RotationState.Lock:
				_rotationEnabled = false;
				_rotationForceEnabled = false;
				if (_followWhenOffRB != null)
				{
					control.rotation = _followWhenOffRB.rotation;
				}
				break;
			}
			if (rotationToggleGroup != null)
			{
				rotationToggleGroup.activeToggleName = _currentRotationState.ToString();
			}
			SetLinkedJointSprings();
			SetJointSprings();
			StateChanged();
		}
	}

	public bool isPositionOn => _currentPositionState == PositionState.On || _currentPositionState == PositionState.Following || _currentPositionState == PositionState.Hold || _currentPositionState == PositionState.ParentLink || _currentPositionState == PositionState.PhysicsLink;

	public bool isRotationOn => _currentRotationState == RotationState.On || _currentRotationState == RotationState.Following || _currentRotationState == RotationState.Hold || _currentRotationState == RotationState.LookAt || _currentRotationState == RotationState.ParentLink || _currentPositionState == PositionState.PhysicsLink;

	public bool xLock
	{
		get
		{
			return _xLock;
		}
		set
		{
			if (_xLock != value)
			{
				_xLock = value;
				if (xLockToggle != null)
				{
					xLockToggle.isOn = value;
				}
			}
		}
	}

	public bool yLock
	{
		get
		{
			return _yLock;
		}
		set
		{
			if (_yLock != value)
			{
				_yLock = value;
				if (yLockToggle != null)
				{
					yLockToggle.isOn = value;
				}
			}
		}
	}

	public bool zLock
	{
		get
		{
			return _zLock;
		}
		set
		{
			if (_zLock != value)
			{
				_zLock = value;
				if (zLockToggle != null)
				{
					zLockToggle.isOn = value;
				}
			}
		}
	}

	public bool xRotLock
	{
		get
		{
			return _xRotLock;
		}
		set
		{
			if (_xRotLock != value)
			{
				_xRotLock = value;
				if (xRotLockToggle != null)
				{
					xRotLockToggle.isOn = value;
				}
			}
		}
	}

	public bool yRotLock
	{
		get
		{
			return _yRotLock;
		}
		set
		{
			if (_yRotLock != value)
			{
				_yRotLock = value;
				if (yRotLockToggle != null)
				{
					yRotLockToggle.isOn = value;
				}
			}
		}
	}

	public bool zRotLock
	{
		get
		{
			return _zRotLock;
		}
		set
		{
			if (_zRotLock != value)
			{
				_zRotLock = value;
				if (zRotLockToggle != null)
				{
					zRotLockToggle.isOn = value;
				}
			}
		}
	}

	public bool interactableInPlayMode
	{
		get
		{
			return _interactableInPlayMode;
		}
		set
		{
			if (_interactableInPlayMode != value)
			{
				_interactableInPlayMode = value;
				if (interactableInPlayModeToggle != null)
				{
					interactableInPlayModeToggle.isOn = value;
				}
			}
		}
	}

	public bool useGravityOnRBWhenOff
	{
		get
		{
			return _useGravityOnRBWhenOff;
		}
		set
		{
			if (_useGravityOnRBWhenOff != value)
			{
				_useGravityOnRBWhenOff = value;
				if (useGravityOnRBWhenOffToggle != null)
				{
					useGravityOnRBWhenOffToggle.isOn = value;
				}
				if (_followWhenOffRB != null && _currentPositionState == PositionState.Off)
				{
					_followWhenOffRB.useGravity = _useGravityOnRBWhenOff;
				}
			}
		}
	}

	public bool physicsEnabled
	{
		get
		{
			if (_followWhenOffRB != null)
			{
				return !_followWhenOffRB.isKinematic;
			}
			return false;
		}
		set
		{
			if (_followWhenOffRB != null && _followWhenOffRB.isKinematic == value)
			{
				_followWhenOffRB.isKinematic = !value;
				MeshCollider component = _followWhenOffRB.GetComponent<MeshCollider>();
				if (component != null)
				{
					component.convex = value;
				}
				if (physicsEnabledToggle != null)
				{
					physicsEnabledToggle.isOn = value;
				}
			}
		}
	}

	public bool collisionEnabled
	{
		get
		{
			if (_followWhenOffRB != null)
			{
				return _followWhenOffRB.detectCollisions;
			}
			return false;
		}
		set
		{
			if (_followWhenOffRB != null && _followWhenOffRB.detectCollisions != value)
			{
				_followWhenOffRB.detectCollisions = value;
				if (collisionEnabledToggle != null)
				{
					collisionEnabledToggle.isOn = value;
				}
			}
		}
	}

	public Rigidbody followWhenOffRB
	{
		get
		{
			return _followWhenOffRB;
		}
		set
		{
			if (!(_followWhenOffRB != value))
			{
				return;
			}
			_followWhenOffRB = value;
			followWhenOff = _followWhenOffRB.transform;
			ConfigurableJoint[] components = followWhenOff.GetComponents<ConfigurableJoint>();
			ConfigurableJoint[] array = components;
			foreach (ConfigurableJoint configurableJoint in array)
			{
				if (configurableJoint.connectedBody == kinematicRB)
				{
					connectedJoint = configurableJoint;
					SetJointSprings();
				}
			}
		}
	}

	public float RBMass
	{
		get
		{
			if (_followWhenOffRB != null)
			{
				return _followWhenOffRB.mass;
			}
			return 0f;
		}
		set
		{
			if (_followWhenOffRB != null)
			{
				_followWhenOffRB.mass = value;
				if (RBMassSlider != null)
				{
					RBMassSlider.value = value;
				}
				SetJointSprings();
			}
		}
	}

	public float RBLockPositionSpring
	{
		get
		{
			return _RBLockPositionSpring;
		}
		set
		{
			if (_RBLockPositionSpring != value)
			{
				_RBLockPositionSpring = value;
				SetJointSprings();
			}
		}
	}

	public float RBLockPositionDamper
	{
		get
		{
			return _RBLockPositionDamper;
		}
		set
		{
			if (_RBLockPositionDamper != value)
			{
				_RBLockPositionDamper = value;
				SetJointSprings();
			}
		}
	}

	public float RBLockPositionMaxForce
	{
		get
		{
			return _RBLockPositionMaxForce;
		}
		set
		{
			if (_RBLockPositionMaxForce != value)
			{
				_RBLockPositionMaxForce = value;
				SetJointSprings();
			}
		}
	}

	public float RBHoldPositionSpring
	{
		get
		{
			return _RBHoldPositionSpring;
		}
		set
		{
			if (_RBHoldPositionSpring != value)
			{
				_RBHoldPositionSpring = value;
				if (holdPositionSpringSlider != null)
				{
					holdPositionSpringSlider.value = value;
				}
				SetJointSprings();
			}
		}
	}

	public float RBHoldPositionDamper
	{
		get
		{
			return _RBHoldPositionDamper;
		}
		set
		{
			if (_RBHoldPositionDamper != value)
			{
				_RBHoldPositionDamper = value;
				if (holdPositionDamperSlider != null)
				{
					holdPositionDamperSlider.value = value;
				}
				SetJointSprings();
			}
		}
	}

	public float RBHoldPositionMaxForce
	{
		get
		{
			return _RBHoldPositionMaxForce;
		}
		set
		{
			if (_RBHoldPositionMaxForce != value)
			{
				_RBHoldPositionMaxForce = value;
				if (holdPositionMaxForceSlider != null)
				{
					holdPositionMaxForceSlider.value = value;
				}
				SetJointSprings();
			}
		}
	}

	public float RBLinkPositionSpring
	{
		get
		{
			return _RBLinkPositionSpring;
		}
		set
		{
			if (_RBLinkPositionSpring != value)
			{
				_RBLinkPositionSpring = value;
				if (linkPositionSpringSlider != null)
				{
					linkPositionSpringSlider.value = value;
				}
				SetLinkedJointSprings();
			}
		}
	}

	public float RBLinkPositionDamper
	{
		get
		{
			return _RBLinkPositionDamper;
		}
		set
		{
			if (_RBLinkPositionDamper != value)
			{
				_RBLinkPositionDamper = value;
				if (linkPositionDamperSlider != null)
				{
					linkPositionDamperSlider.value = value;
				}
				SetLinkedJointSprings();
			}
		}
	}

	public float RBLinkPositionMaxForce
	{
		get
		{
			return _RBLinkPositionMaxForce;
		}
		set
		{
			if (_RBLinkPositionMaxForce != value)
			{
				_RBLinkPositionMaxForce = value;
				if (linkPositionMaxForceSlider != null)
				{
					linkPositionMaxForceSlider.value = value;
				}
				SetLinkedJointSprings();
			}
		}
	}

	public float RBLockRotationSpring
	{
		get
		{
			return _RBLockRotationSpring;
		}
		set
		{
			if (_RBLockRotationSpring != value)
			{
				_RBLockRotationSpring = value;
				SetJointSprings();
			}
		}
	}

	public float RBLockRotationDamper
	{
		get
		{
			return _RBLockRotationDamper;
		}
		set
		{
			if (_RBLockRotationDamper != value)
			{
				_RBLockRotationDamper = value;
				SetJointSprings();
			}
		}
	}

	public float RBLockRotationMaxForce
	{
		get
		{
			return _RBLockRotationMaxForce;
		}
		set
		{
			if (_RBLockRotationMaxForce != value)
			{
				_RBLockRotationMaxForce = value;
				SetJointSprings();
			}
		}
	}

	public float RBHoldRotationSpring
	{
		get
		{
			return _RBHoldRotationSpring;
		}
		set
		{
			if (_RBHoldRotationSpring != value)
			{
				_RBHoldRotationSpring = value;
				if (holdRotationSpringSlider != null)
				{
					holdRotationSpringSlider.value = value;
				}
				SetJointSprings();
			}
		}
	}

	public float RBHoldRotationDamper
	{
		get
		{
			return _RBHoldRotationDamper;
		}
		set
		{
			if (_RBHoldRotationDamper != value)
			{
				_RBHoldRotationDamper = value;
				if (holdRotationDamperSlider != null)
				{
					holdRotationDamperSlider.value = value;
				}
				SetJointSprings();
			}
		}
	}

	public float RBHoldRotationMaxForce
	{
		get
		{
			return _RBHoldRotationMaxForce;
		}
		set
		{
			if (_RBHoldRotationMaxForce != value)
			{
				_RBHoldRotationMaxForce = value;
				if (holdRotationMaxForceSlider != null)
				{
					holdRotationMaxForceSlider.value = value;
				}
				SetJointSprings();
			}
		}
	}

	public float RBLinkRotationSpring
	{
		get
		{
			return _RBLinkRotationSpring;
		}
		set
		{
			if (_RBLinkRotationSpring != value)
			{
				_RBLinkRotationSpring = value;
				if (linkRotationSpringSlider != null)
				{
					linkRotationSpringSlider.value = value;
				}
				SetLinkedJointSprings();
			}
		}
	}

	public float RBLinkRotationDamper
	{
		get
		{
			return _RBLinkRotationDamper;
		}
		set
		{
			if (_RBLinkRotationDamper != value)
			{
				_RBLinkRotationDamper = value;
				if (linkRotationDamperSlider != null)
				{
					linkRotationDamperSlider.value = value;
				}
				SetLinkedJointSprings();
			}
		}
	}

	public float RBLinkRotationMaxForce
	{
		get
		{
			return _RBLinkRotationMaxForce;
		}
		set
		{
			if (_RBLinkRotationMaxForce != value)
			{
				_RBLinkRotationMaxForce = value;
				if (linkRotationMaxForceSlider != null)
				{
					linkRotationMaxForceSlider.value = value;
				}
				SetLinkedJointSprings();
			}
		}
	}

	public float jointRotationDriveSpring
	{
		get
		{
			return _jointRotationDriveSpring;
		}
		set
		{
			if (_jointRotationDriveSpring != value)
			{
				_jointRotationDriveSpring = value;
				if (jointRotationDriveSpringSlider != null)
				{
					jointRotationDriveSpringSlider.value = value;
				}
				SetNaturalJointDrive();
			}
		}
	}

	public float jointRotationDriveDamper
	{
		get
		{
			return _jointRotationDriveDamper;
		}
		set
		{
			if (_jointRotationDriveDamper != value)
			{
				_jointRotationDriveDamper = value;
				if (jointRotationDriveDamperSlider != null)
				{
					jointRotationDriveDamperSlider.value = value;
				}
				SetNaturalJointDrive();
			}
		}
	}

	public float jointRotationDriveMaxForce
	{
		get
		{
			return _jointRotationDriveMaxForce;
		}
		set
		{
			if (_jointRotationDriveMaxForce != value)
			{
				_jointRotationDriveMaxForce = value;
				if (jointRotationDriveMaxForceSlider != null)
				{
					jointRotationDriveMaxForceSlider.value = value;
				}
				SetNaturalJointDrive();
			}
		}
	}

	public float jointRotationDriveXTarget
	{
		get
		{
			return _jointRotationDriveXTarget;
		}
		set
		{
			if (_jointRotationDriveXTarget != value)
			{
				_jointRotationDriveXTarget = value;
				if (jointRotationDriveXTargetSlider != null)
				{
					jointRotationDriveXTargetSlider.value = value;
				}
				SetNaturalJointDrive();
			}
		}
	}

	public float jointRotationDriveYTarget
	{
		get
		{
			return _jointRotationDriveYTarget;
		}
		set
		{
			if (_jointRotationDriveYTarget != value)
			{
				_jointRotationDriveYTarget = value;
				if (jointRotationDriveYTargetSlider != null)
				{
					jointRotationDriveYTargetSlider.value = value;
				}
				SetNaturalJointDrive();
			}
		}
	}

	public float jointRotationDriveZTarget
	{
		get
		{
			return _jointRotationDriveZTarget;
		}
		set
		{
			if (_jointRotationDriveZTarget != value)
			{
				_jointRotationDriveZTarget = value;
				if (jointRotationDriveZTargetSlider != null)
				{
					jointRotationDriveZTargetSlider.value = value;
				}
				SetNaturalJointDrive();
			}
		}
	}

	public bool hidden
	{
		get
		{
			return _hidden;
		}
		set
		{
			_hidden = value;
			if (_hidden)
			{
				if (mrs != null)
				{
					MeshRenderer[] array = mrs;
					foreach (MeshRenderer meshRenderer in array)
					{
						meshRenderer.enabled = false;
					}
				}
			}
			else if (mrs != null)
			{
				MeshRenderer[] array2 = mrs;
				foreach (MeshRenderer meshRenderer2 in array2)
				{
					meshRenderer2.enabled = true;
				}
			}
		}
	}

	public bool guihidden
	{
		get
		{
			return _guihidden;
		}
		set
		{
			_guihidden = value;
			if (_guihidden)
			{
				if (!GUIalwaysVisibleWhenSelected || !_selected)
				{
					HideGUI();
				}
			}
			else if (_selected)
			{
				ShowGUI();
			}
		}
	}

	public bool highlighted
	{
		get
		{
			return _highlighted;
		}
		set
		{
			_highlighted = value;
			SetColor();
		}
	}

	public Vector3 selectedPosition => _selectedPosition;

	public bool selected
	{
		get
		{
			return _selected;
		}
		set
		{
			_selected = value;
			if (_selected)
			{
				if (!_guihidden || GUIalwaysVisibleWhenSelected)
				{
					ShowGUI();
				}
				_selectedPosition = control.position;
			}
			else
			{
				HideGUI();
			}
			SetColor();
		}
	}

	public Color currentPositionColor => _currentPositionColor;

	public Color currentRotationolor => _currentRotationColor;

	public ControlMode controlMode
	{
		get
		{
			return _controlMode;
		}
		set
		{
			switch (value)
			{
			case ControlMode.Position:
				if (_moveEnabled || _moveForceEnabled)
				{
					_controlMode = value;
				}
				break;
			case ControlMode.Rotation:
				if (_rotationEnabled || _rotationForceEnabled)
				{
					_controlMode = value;
				}
				break;
			default:
				_controlMode = ControlMode.Off;
				break;
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			needsStore = true;
			Vector3 position = base.transform.position;
			jSON["position"]["x"].AsFloat = position.x;
			jSON["position"]["y"].AsFloat = position.y;
			jSON["position"]["z"].AsFloat = position.z;
			Vector3 eulerAngles = base.transform.eulerAngles;
			jSON["rotation"]["x"].AsFloat = eulerAngles.x;
			jSON["rotation"]["y"].AsFloat = eulerAngles.y;
			jSON["rotation"]["z"].AsFloat = eulerAngles.z;
			jSON["positionState"] = currentPositionState.ToString();
			jSON["rotationState"] = currentRotationState.ToString();
			if (interactableInPlayModeToggle != null)
			{
				jSON["interactableInPlayMode"].AsBool = interactableInPlayMode;
			}
			if (collisionEnabledToggle != null)
			{
				jSON["collisionEnabled"].AsBool = collisionEnabled;
			}
			if (physicsEnabledToggle != null)
			{
				jSON["physicsEnabled"].AsBool = physicsEnabled;
			}
			if (useGravityOnRBWhenOffToggle != null)
			{
				jSON["useGravityWhenOff"].AsBool = _useGravityOnRBWhenOff;
			}
			if (RBMassSlider != null)
			{
				SliderControl component = RBMassSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != RBMass)
				{
					jSON["mass"].AsFloat = RBMass;
				}
			}
			if (_linkToRB != null && linkToAtomUID != null)
			{
				jSON["linkTo"] = linkToAtomUID + ":" + _linkToRB.name;
			}
			if (holdPositionSpringSlider != null)
			{
				SliderControl component2 = holdPositionSpringSlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != RBHoldPositionSpring)
				{
					jSON["holdPositionSpring"].AsFloat = RBHoldPositionSpring;
				}
			}
			if (holdPositionDamperSlider != null)
			{
				SliderControl component3 = holdPositionDamperSlider.GetComponent<SliderControl>();
				if (component3 == null || component3.defaultValue != RBHoldPositionDamper)
				{
					jSON["holdPositionDamper"].AsFloat = RBHoldPositionDamper;
				}
			}
			if (holdPositionMaxForceSlider != null)
			{
				SliderControl component4 = holdPositionMaxForceSlider.GetComponent<SliderControl>();
				if (component4 == null || component4.defaultValue != RBHoldPositionMaxForce)
				{
					jSON["holdPositionMaxForce"].AsFloat = RBHoldPositionMaxForce;
				}
			}
			if (holdRotationSpringSlider != null)
			{
				SliderControl component5 = holdRotationSpringSlider.GetComponent<SliderControl>();
				if (component5 == null || component5.defaultValue != RBHoldRotationSpring)
				{
					jSON["holdRotationSpring"].AsFloat = RBHoldRotationSpring;
				}
			}
			if (holdRotationDamperSlider != null)
			{
				SliderControl component6 = holdRotationDamperSlider.GetComponent<SliderControl>();
				if (component6 == null || component6.defaultValue != RBHoldRotationDamper)
				{
					jSON["holdRotationDamper"].AsFloat = RBHoldRotationDamper;
				}
			}
			if (holdRotationMaxForceSlider != null)
			{
				SliderControl component7 = holdRotationMaxForceSlider.GetComponent<SliderControl>();
				if (component7 == null || component7.defaultValue != RBHoldRotationMaxForce)
				{
					jSON["holdRotationMaxForce"].AsFloat = RBHoldRotationMaxForce;
				}
			}
			if (linkPositionSpringSlider != null)
			{
				SliderControl component8 = linkPositionSpringSlider.GetComponent<SliderControl>();
				if (component8 == null || component8.defaultValue != RBLinkPositionSpring)
				{
					jSON["linkPositionSpring"].AsFloat = RBLinkPositionSpring;
				}
			}
			if (linkPositionDamperSlider != null)
			{
				SliderControl component9 = linkPositionDamperSlider.GetComponent<SliderControl>();
				if (component9 == null || component9.defaultValue != RBLinkPositionDamper)
				{
					jSON["linkPositionDamper"].AsFloat = RBLinkPositionDamper;
				}
			}
			if (linkPositionMaxForceSlider != null)
			{
				SliderControl component10 = linkPositionMaxForceSlider.GetComponent<SliderControl>();
				if (component10 == null || component10.defaultValue != RBLinkPositionMaxForce)
				{
					jSON["linkPositionMaxForce"].AsFloat = RBLinkPositionMaxForce;
				}
			}
			if (linkRotationSpringSlider != null)
			{
				SliderControl component11 = linkRotationSpringSlider.GetComponent<SliderControl>();
				if (component11 == null || component11.defaultValue != RBLinkRotationSpring)
				{
					jSON["linkRotationSpring"].AsFloat = RBLinkRotationSpring;
				}
			}
			if (linkRotationDamperSlider != null)
			{
				SliderControl component12 = linkRotationDamperSlider.GetComponent<SliderControl>();
				if (component12 == null || component12.defaultValue != RBLinkRotationDamper)
				{
					jSON["linkRotationDamper"].AsFloat = RBLinkRotationDamper;
				}
			}
			if (linkRotationMaxForceSlider != null)
			{
				SliderControl component13 = linkRotationMaxForceSlider.GetComponent<SliderControl>();
				if (component13 == null || component13.defaultValue != RBLinkRotationMaxForce)
				{
					jSON["linkRotationMaxForce"].AsFloat = RBLinkRotationMaxForce;
				}
			}
			if (jointRotationDriveSpringSlider != null)
			{
				SliderControl component14 = jointRotationDriveSpringSlider.GetComponent<SliderControl>();
				if (component14 == null || component14.defaultValue != jointRotationDriveSpring)
				{
					jSON["jointDriveSpring"].AsFloat = jointRotationDriveSpring;
				}
			}
			if (jointRotationDriveDamperSlider != null)
			{
				SliderControl component15 = jointRotationDriveDamperSlider.GetComponent<SliderControl>();
				if (component15 == null || component15.defaultValue != jointRotationDriveDamper)
				{
					jSON["jointDriveDamper"].AsFloat = jointRotationDriveDamper;
				}
			}
			if (jointRotationDriveMaxForceSlider != null)
			{
				SliderControl component16 = jointRotationDriveMaxForceSlider.GetComponent<SliderControl>();
				if (component16 == null || component16.defaultValue != jointRotationDriveMaxForce)
				{
					jSON["jointDriveMaxForce"].AsFloat = jointRotationDriveMaxForce;
				}
			}
			if (jointRotationDriveXTargetSlider != null)
			{
				SliderControl component17 = jointRotationDriveXTargetSlider.GetComponent<SliderControl>();
				if (component17 == null || component17.defaultValue != jointRotationDriveXTarget)
				{
					jSON["jointDriveXTarget"].AsFloat = jointRotationDriveXTarget;
				}
			}
			if (jointRotationDriveYTargetSlider != null)
			{
				SliderControl component18 = jointRotationDriveYTargetSlider.GetComponent<SliderControl>();
				if (component18 == null || component18.defaultValue != jointRotationDriveYTarget)
				{
					jSON["jointDriveYTarget"].AsFloat = jointRotationDriveYTarget;
				}
			}
			if (jointRotationDriveZTargetSlider != null)
			{
				SliderControl component19 = jointRotationDriveZTargetSlider.GetComponent<SliderControl>();
				if (component19 == null || component19.defaultValue != jointRotationDriveZTarget)
				{
					jSON["jointDriveZTarget"].AsFloat = jointRotationDriveZTarget;
				}
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		Init();
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (!restorePhysical)
		{
			return;
		}
		if (jc["position"] != null)
		{
			Vector3 position = base.transform.position;
			if (jc["position"]["x"] != null)
			{
				position.x = jc["position"]["x"].AsFloat;
			}
			if (jc["position"]["y"] != null)
			{
				position.y = jc["position"]["y"].AsFloat;
			}
			if (jc["position"]["z"] != null)
			{
				position.z = jc["position"]["z"].AsFloat;
			}
			base.transform.position = position;
			if (control != null)
			{
				control.position = position;
			}
		}
		else
		{
			base.transform.position = startingPosition;
			if (control != null)
			{
				control.position = startingPosition;
			}
		}
		if (jc["rotation"] != null)
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			if (jc["rotation"]["x"] != null)
			{
				eulerAngles.x = jc["rotation"]["x"].AsFloat;
			}
			if (jc["rotation"]["y"] != null)
			{
				eulerAngles.y = jc["rotation"]["y"].AsFloat;
			}
			if (jc["rotation"]["z"] != null)
			{
				eulerAngles.z = jc["rotation"]["z"].AsFloat;
			}
			base.transform.eulerAngles = eulerAngles;
			if (control != null)
			{
				control.eulerAngles = eulerAngles;
			}
		}
		else
		{
			base.transform.rotation = startingRotation;
			if (control != null)
			{
				control.rotation = startingRotation;
			}
		}
		if (jc["positionState"] != null)
		{
			SetPositionStateFromString(jc["positionState"]);
		}
		else
		{
			currentPositionState = startingPositionState;
		}
		if (jc["rotationState"] != null)
		{
			SetRotationStateFromString(jc["rotationState"]);
		}
		else
		{
			currentRotationState = startingRotationState;
		}
		if (interactableInPlayModeToggle != null)
		{
			if (jc["interactableInPlayMode"] != null)
			{
				interactableInPlayMode = jc["interactableInPlayMode"].AsBool;
			}
			else
			{
				interactableInPlayMode = _startingInteractableInPlayMode;
			}
		}
		if (collisionEnabledToggle != null)
		{
			if (jc["collisionEnabled"] != null)
			{
				collisionEnabled = jc["collisionEnabled"].AsBool;
			}
			else
			{
				collisionEnabled = _startingCollsionEnabled;
			}
		}
		if (physicsEnabledToggle != null)
		{
			if (jc["physicsEnabled"] != null)
			{
				physicsEnabled = jc["physicsEnabled"].AsBool;
			}
			else
			{
				physicsEnabled = _startingPhysicsEnabled;
			}
		}
		if (useGravityOnRBWhenOffToggle != null)
		{
			if (jc["useGravityWhenOff"] != null)
			{
				useGravityOnRBWhenOff = jc["useGravityWhenOff"].AsBool;
			}
			else
			{
				useGravityOnRBWhenOff = _startingUseGravityOnRBWhenOff;
			}
		}
		if (jc["mass"] != null)
		{
			RBMass = jc["mass"].AsFloat;
		}
		else if (RBMassSlider != null)
		{
			SliderControl component = RBMassSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				RBMass = component.defaultValue;
			}
		}
		if (jc["holdPositionSpring"] != null)
		{
			RBHoldPositionSpring = jc["holdPositionSpring"].AsFloat;
		}
		else if (holdPositionSpringSlider != null)
		{
			SliderControl component2 = holdPositionSpringSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				RBHoldPositionSpring = component2.defaultValue;
			}
		}
		if (jc["holdPositionDamper"] != null)
		{
			RBHoldPositionDamper = jc["holdPositionDamper"].AsFloat;
		}
		else if (holdPositionDamperSlider != null)
		{
			SliderControl component3 = holdPositionDamperSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				RBHoldPositionDamper = component3.defaultValue;
			}
		}
		if (jc["holdPositionMaxForce"] != null)
		{
			RBHoldPositionMaxForce = jc["holdPositionMaxForce"].AsFloat;
		}
		else if (holdPositionMaxForceSlider != null)
		{
			SliderControl component4 = holdPositionMaxForceSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				RBHoldPositionMaxForce = component4.defaultValue;
			}
		}
		if (jc["holdRotationSpring"] != null)
		{
			RBHoldRotationSpring = jc["holdRotationSpring"].AsFloat;
		}
		else if (holdRotationSpringSlider != null)
		{
			SliderControl component5 = holdRotationSpringSlider.GetComponent<SliderControl>();
			if (component5 != null)
			{
				RBHoldRotationSpring = component5.defaultValue;
			}
		}
		if (jc["holdRotationDamper"] != null)
		{
			RBHoldRotationDamper = jc["holdRotationDamper"].AsFloat;
		}
		else if (holdRotationDamperSlider != null)
		{
			SliderControl component6 = holdRotationDamperSlider.GetComponent<SliderControl>();
			if (component6 != null)
			{
				RBHoldRotationDamper = component6.defaultValue;
			}
		}
		if (jc["holdRotationMaxForce"] != null)
		{
			RBHoldRotationMaxForce = jc["holdRotationMaxForce"].AsFloat;
		}
		else if (holdRotationMaxForceSlider != null)
		{
			SliderControl component7 = holdRotationMaxForceSlider.GetComponent<SliderControl>();
			if (component7 != null)
			{
				RBHoldRotationMaxForce = component7.defaultValue;
			}
		}
		if (jc["linkPositionSpring"] != null)
		{
			RBLinkPositionSpring = jc["linkPositionSpring"].AsFloat;
		}
		else if (linkPositionSpringSlider != null)
		{
			SliderControl component8 = linkPositionSpringSlider.GetComponent<SliderControl>();
			if (component8 != null)
			{
				RBLinkPositionSpring = component8.defaultValue;
			}
		}
		if (jc["linkPositionDamper"] != null)
		{
			RBLinkPositionDamper = jc["linkPositionDamper"].AsFloat;
		}
		else if (linkPositionDamperSlider != null)
		{
			SliderControl component9 = linkPositionDamperSlider.GetComponent<SliderControl>();
			if (component9 != null)
			{
				RBLinkPositionDamper = component9.defaultValue;
			}
		}
		if (jc["linkPositionMaxForce"] != null)
		{
			RBLinkPositionMaxForce = jc["linkPositionMaxForce"].AsFloat;
		}
		else if (linkPositionMaxForceSlider != null)
		{
			SliderControl component10 = linkPositionMaxForceSlider.GetComponent<SliderControl>();
			if (component10 != null)
			{
				RBLinkPositionMaxForce = component10.defaultValue;
			}
		}
		if (jc["linkRotationSpring"] != null)
		{
			RBLinkRotationSpring = jc["linkRotationSpring"].AsFloat;
		}
		else if (linkRotationSpringSlider != null)
		{
			SliderControl component11 = linkRotationSpringSlider.GetComponent<SliderControl>();
			if (component11 != null)
			{
				RBLinkRotationSpring = component11.defaultValue;
			}
		}
		if (jc["linkRotationDamper"] != null)
		{
			RBLinkRotationDamper = jc["linkRotationDamper"].AsFloat;
		}
		else if (linkRotationDamperSlider != null)
		{
			SliderControl component12 = linkRotationDamperSlider.GetComponent<SliderControl>();
			if (component12 != null)
			{
				RBLinkRotationDamper = component12.defaultValue;
			}
		}
		if (jc["linkRotationMaxForce"] != null)
		{
			RBLinkRotationMaxForce = jc["linkRotationMaxForce"].AsFloat;
		}
		else if (linkRotationMaxForceSlider != null)
		{
			SliderControl component13 = linkRotationMaxForceSlider.GetComponent<SliderControl>();
			if (component13 != null)
			{
				RBLinkRotationMaxForce = component13.defaultValue;
			}
		}
		if (jc["jointDriveSpring"] != null)
		{
			jointRotationDriveSpring = jc["jointDriveSpring"].AsFloat;
		}
		else if (jointRotationDriveSpringSlider != null)
		{
			SliderControl component14 = jointRotationDriveSpringSlider.GetComponent<SliderControl>();
			if (component14 != null)
			{
				jointRotationDriveSpring = component14.defaultValue;
			}
		}
		if (jc["jointDriveDamper"] != null)
		{
			jointRotationDriveDamper = jc["jointDriveDamper"].AsFloat;
		}
		else if (jointRotationDriveDamperSlider != null)
		{
			SliderControl component15 = jointRotationDriveDamperSlider.GetComponent<SliderControl>();
			if (component15 != null)
			{
				jointRotationDriveDamper = component15.defaultValue;
			}
		}
		if (jc["jointDriveMaxForce"] != null)
		{
			jointRotationDriveMaxForce = jc["jointDriveMaxForce"].AsFloat;
		}
		else if (jointRotationDriveMaxForceSlider != null)
		{
			SliderControl component16 = jointRotationDriveMaxForceSlider.GetComponent<SliderControl>();
			if (component16 != null)
			{
				jointRotationDriveMaxForce = component16.defaultValue;
			}
		}
		if (jc["jointDriveXTarget"] != null)
		{
			jointRotationDriveXTarget = jc["jointDriveXTarget"].AsFloat;
		}
		else if (jointRotationDriveXTargetSlider != null)
		{
			SliderControl component17 = jointRotationDriveXTargetSlider.GetComponent<SliderControl>();
			if (component17 != null)
			{
				jointRotationDriveXTarget = component17.defaultValue;
			}
		}
		if (jc["jointDriveYTarget"] != null)
		{
			jointRotationDriveYTarget = jc["jointDriveYTarget"].AsFloat;
		}
		else if (jointRotationDriveYTargetSlider != null)
		{
			SliderControl component18 = jointRotationDriveYTargetSlider.GetComponent<SliderControl>();
			if (component18 != null)
			{
				jointRotationDriveYTarget = component18.defaultValue;
			}
		}
		if (jc["jointDriveZTarget"] != null)
		{
			jointRotationDriveZTarget = jc["jointDriveZTarget"].AsFloat;
		}
		else if (jointRotationDriveZTargetSlider != null)
		{
			SliderControl component19 = jointRotationDriveZTargetSlider.GetComponent<SliderControl>();
			if (component19 != null)
			{
				jointRotationDriveZTarget = component19.defaultValue;
			}
		}
	}

	public override void LateRestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.LateRestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (!restorePhysical)
		{
			return;
		}
		if (jc["linkTo"] != null)
		{
			if (SuperController.singleton != null)
			{
				Rigidbody rb = SuperController.singleton.RigidbodyNameToRigidbody(jc["linkTo"]);
				SelectLinkToRigidbody(rb);
			}
		}
		else
		{
			SelectLinkToRigidbody(null);
		}
		if (jc["positionState"] != null)
		{
			SetPositionStateFromString(jc["positionState"]);
		}
		else
		{
			currentPositionState = startingPositionState;
		}
		if (jc["rotationState"] != null)
		{
			SetRotationStateFromString(jc["rotationState"]);
		}
		else
		{
			currentRotationState = startingRotationState;
		}
	}

	public void MovePlayerTo()
	{
	}

	public void MovePlayerToAndControl()
	{
	}

	protected virtual void SetLinkToAtomNames()
	{
		if (!(linkToAtomSelectionPopup != null) || !(SuperController.singleton != null))
		{
			return;
		}
		List<string> atomUIDsWithRigidbodies = SuperController.singleton.GetAtomUIDsWithRigidbodies();
		if (atomUIDsWithRigidbodies == null)
		{
			linkToAtomSelectionPopup.numPopupValues = 1;
			linkToAtomSelectionPopup.setPopupValue(0, "None");
			return;
		}
		linkToAtomSelectionPopup.numPopupValues = atomUIDsWithRigidbodies.Count + 1;
		linkToAtomSelectionPopup.setPopupValue(0, "None");
		for (int i = 0; i < atomUIDsWithRigidbodies.Count; i++)
		{
			linkToAtomSelectionPopup.setPopupValue(i + 1, atomUIDsWithRigidbodies[i]);
		}
	}

	protected void onLinkToRigidbodyNamesChanged(List<string> rbNames)
	{
		if (!(linkToSelectionPopup != null))
		{
			return;
		}
		if (rbNames == null)
		{
			linkToSelectionPopup.numPopupValues = 1;
			linkToSelectionPopup.setPopupValue(0, "None");
			return;
		}
		linkToSelectionPopup.numPopupValues = rbNames.Count + 1;
		linkToSelectionPopup.setPopupValue(0, "None");
		for (int i = 0; i < rbNames.Count; i++)
		{
			linkToSelectionPopup.setPopupValue(i + 1, rbNames[i]);
		}
	}

	public virtual void SetLinkToAtom(string atomUID)
	{
		if (SuperController.singleton != null)
		{
			Atom atomByUid = SuperController.singleton.GetAtomByUid(atomUID);
			if (atomByUid != null)
			{
				linkToAtomUID = atomUID;
				List<string> rigidbodyNamesInAtom = SuperController.singleton.GetRigidbodyNamesInAtom(linkToAtomUID);
				onLinkToRigidbodyNamesChanged(rigidbodyNamesInAtom);
				linkToSelectionPopup.currentValue = "None";
			}
		}
	}

	public void SetLinkToRigidbody(string rigidbodyName)
	{
		if (SuperController.singleton != null)
		{
			Rigidbody rigidbody2 = (linkToRB = SuperController.singleton.RigidbodyNameToRigidbody(rigidbodyName));
		}
	}

	public virtual void SetLinkToRigidbodyObject(string objectName)
	{
		if (linkToAtomUID != null)
		{
			SetLinkToRigidbody(linkToAtomUID + ":" + objectName);
		}
	}

	public void SelectLinkToRigidbody(Rigidbody rb)
	{
		SelectLinkToRigidbody(rb, SelectLinkState.PositionAndRotation);
	}

	public void SelectLinkToRigidbody(Rigidbody rb, SelectLinkState linkState, bool usePhysicalLink = false)
	{
		if (linkToAtomSelectionPopup != null)
		{
			if (rb != null)
			{
				Atom atom = null;
				FreeControllerV3 component = rb.GetComponent<FreeControllerV3>();
				if (component != null)
				{
					atom = component.containingAtom;
				}
				else
				{
					ForceReceiver component2 = rb.GetComponent<ForceReceiver>();
					if (component2 != null)
					{
						atom = component2.containingAtom;
					}
				}
				if (atom != null)
				{
					linkToAtomUID = atom.uid;
					linkToAtomSelectionPopup.currentValue = atom.uid;
				}
			}
			else
			{
				linkToAtomSelectionPopup.currentValue = "None";
			}
		}
		if (linkToSelectionPopup != null)
		{
			if (rb != null)
			{
				linkToSelectionPopup.currentValueNoCallback = rb.name;
			}
			else
			{
				linkToSelectionPopup.currentValueNoCallback = "None";
			}
		}
		if (rb != null)
		{
			linkToRB = rb;
			if (_currentPositionState != PositionState.ParentLink && _currentPositionState != PositionState.PhysicsLink && (linkState == SelectLinkState.Position || linkState == SelectLinkState.PositionAndRotation))
			{
				preLinkPositionState = currentPositionState;
				if (usePhysicalLink)
				{
					currentPositionState = PositionState.PhysicsLink;
				}
				else
				{
					currentPositionState = PositionState.ParentLink;
				}
			}
			if (_currentRotationState != RotationState.ParentLink && _currentRotationState != RotationState.PhysicsLink && (linkState == SelectLinkState.Rotation || linkState == SelectLinkState.PositionAndRotation))
			{
				preLinkRotationState = currentRotationState;
				if (usePhysicalLink)
				{
					currentRotationState = RotationState.PhysicsLink;
				}
				else
				{
					currentRotationState = RotationState.ParentLink;
				}
			}
		}
		else
		{
			linkToRB = null;
			if (_currentPositionState == PositionState.ParentLink || _currentPositionState == PositionState.PhysicsLink)
			{
				currentPositionState = preLinkPositionState;
			}
			if (_currentRotationState == RotationState.ParentLink || _currentRotationState == RotationState.PhysicsLink)
			{
				currentRotationState = preLinkRotationState;
			}
		}
	}

	public void SelectLinkToRigidbodyFromScene()
	{
		SuperController.singleton.SelectModeRigidbody(SelectLinkToRigidbody);
	}

	public void SelectAlignToRigidbody(Rigidbody rb)
	{
		control.position = rb.transform.position;
		control.rotation = rb.transform.rotation;
	}

	public void SelectAlignToRigidbodyFromScene()
	{
		SuperController.singleton.SelectModeRigidbody(SelectAlignToRigidbody);
	}

	private void SetLinkedJointSprings()
	{
		if (_linkToJoint != null)
		{
			JointDrive xDrive = _linkToJoint.xDrive;
			if (_currentPositionState == PositionState.PhysicsLink)
			{
				xDrive.positionSpring = _RBLinkPositionSpring;
				xDrive.positionDamper = _RBLinkPositionDamper;
				xDrive.maximumForce = _RBLinkPositionMaxForce;
			}
			else
			{
				xDrive.positionSpring = 0f;
				xDrive.positionDamper = 0f;
				xDrive.maximumForce = 0f;
			}
			_linkToJoint.xDrive = xDrive;
			_linkToJoint.yDrive = xDrive;
			_linkToJoint.zDrive = xDrive;
			xDrive = _linkToJoint.slerpDrive;
			if (_currentRotationState == RotationState.PhysicsLink)
			{
				xDrive.positionSpring = _RBLinkRotationSpring;
				xDrive.positionDamper = _RBLinkRotationDamper;
				xDrive.maximumForce = _RBLinkRotationMaxForce;
			}
			else
			{
				xDrive.positionSpring = 0f;
				xDrive.positionDamper = 0f;
				xDrive.maximumForce = 0f;
			}
			_linkToJoint.slerpDrive = xDrive;
			_linkToJoint.angularXDrive = xDrive;
			_linkToJoint.angularYZDrive = xDrive;
		}
	}

	private void SetJointSprings()
	{
		if (connectedJoint != null)
		{
			JointDrive xDrive = connectedJoint.xDrive;
			switch (_currentPositionState)
			{
			case PositionState.On:
			case PositionState.Following:
			case PositionState.Hold:
			case PositionState.ParentLink:
			case PositionState.PhysicsLink:
				xDrive.positionSpring = _RBHoldPositionSpring;
				xDrive.positionDamper = _RBHoldPositionDamper;
				xDrive.maximumForce = _RBHoldPositionMaxForce;
				break;
			case PositionState.Lock:
				xDrive.positionSpring = _RBLockPositionSpring;
				xDrive.positionDamper = _RBLockPositionDamper;
				xDrive.maximumForce = _RBLockPositionMaxForce;
				break;
			case PositionState.Off:
				xDrive.positionSpring = 0f;
				xDrive.positionDamper = 0f;
				xDrive.maximumForce = 0f;
				break;
			}
			connectedJoint.xDrive = xDrive;
			connectedJoint.yDrive = xDrive;
			connectedJoint.zDrive = xDrive;
			xDrive = connectedJoint.slerpDrive;
			switch (_currentRotationState)
			{
			case RotationState.On:
			case RotationState.Following:
			case RotationState.Hold:
			case RotationState.LookAt:
			case RotationState.ParentLink:
			case RotationState.PhysicsLink:
				xDrive.positionSpring = _RBHoldRotationSpring;
				xDrive.positionDamper = _RBHoldRotationDamper;
				xDrive.maximumForce = _RBHoldRotationMaxForce;
				break;
			case RotationState.Lock:
				xDrive.positionSpring = _RBLockRotationSpring;
				xDrive.positionDamper = _RBLockRotationDamper;
				xDrive.maximumForce = _RBLockRotationMaxForce;
				break;
			case RotationState.Off:
				xDrive.positionSpring = 0f;
				xDrive.positionDamper = 0f;
				xDrive.maximumForce = 0f;
				break;
			}
			connectedJoint.slerpDrive = xDrive;
			connectedJoint.angularXDrive = xDrive;
			connectedJoint.angularYZDrive = xDrive;
		}
	}

	private void SetNaturalJointDrive()
	{
		if (naturalJoint != null)
		{
			JointDrive slerpDrive = naturalJoint.slerpDrive;
			slerpDrive.positionSpring = _jointRotationDriveSpring;
			slerpDrive.positionDamper = _jointRotationDriveDamper;
			slerpDrive.maximumForce = _jointRotationDriveMaxForce;
			naturalJoint.slerpDrive = slerpDrive;
			Quaternion targetRotation = Quaternion.Euler(_jointRotationDriveXTarget, _jointRotationDriveYTarget, _jointRotationDriveZTarget);
			naturalJoint.targetRotation = targetRotation;
		}
	}

	public void XPositionSnapPoint1()
	{
		Vector3 position = control.position;
		position.x *= 10f;
		position.x = Mathf.Round(position.x);
		position.x /= 10f;
		control.position = position;
	}

	public void YPositionSnapPoint1()
	{
		Vector3 position = control.position;
		position.y *= 10f;
		position.y = Mathf.Round(position.y);
		position.y /= 10f;
		control.position = position;
	}

	public void ZPositionSnapPoint1()
	{
		Vector3 position = control.position;
		position.z *= 10f;
		position.z = Mathf.Round(position.z);
		position.z /= 10f;
		control.position = position;
	}

	public void XRotationSnap1()
	{
		Vector3 eulerAngles = control.eulerAngles;
		eulerAngles.x = Mathf.Round(eulerAngles.x);
		control.eulerAngles = eulerAngles;
	}

	public void YRotationSnap1()
	{
		Vector3 eulerAngles = control.eulerAngles;
		eulerAngles.y = Mathf.Round(eulerAngles.y);
		control.eulerAngles = eulerAngles;
	}

	public void ZRotationSnap1()
	{
		Vector3 eulerAngles = control.eulerAngles;
		eulerAngles.z = Mathf.Round(eulerAngles.z);
		control.eulerAngles = eulerAngles;
	}

	public void XRotation0()
	{
		Vector3 eulerAngles = control.eulerAngles;
		eulerAngles.x = 0f;
		control.eulerAngles = eulerAngles;
	}

	public void YRotation0()
	{
		Vector3 eulerAngles = control.eulerAngles;
		eulerAngles.y = 0f;
		control.eulerAngles = eulerAngles;
	}

	public void ZRotation0()
	{
		Vector3 eulerAngles = control.eulerAngles;
		eulerAngles.z = 0f;
		control.eulerAngles = eulerAngles;
	}

	public void XRotationAdd(float a)
	{
		Vector3 eulerAngles = control.eulerAngles;
		eulerAngles.x += a;
		control.eulerAngles = eulerAngles;
	}

	public void YRotationAdd(float a)
	{
		Vector3 eulerAngles = control.eulerAngles;
		eulerAngles.y += a;
		control.eulerAngles = eulerAngles;
	}

	public void ZRotationAdd(float a)
	{
		Vector3 eulerAngles = control.eulerAngles;
		eulerAngles.z += a;
		control.eulerAngles = eulerAngles;
	}

	public void NextControlMode()
	{
		if (_controlMode == ControlMode.Off)
		{
			controlMode = ControlMode.Position;
		}
		else if (_controlMode == ControlMode.Position)
		{
			controlMode = ControlMode.Rotation;
		}
		else
		{
			controlMode = ControlMode.Position;
		}
	}

	private void Awake()
	{
		if (control == null)
		{
			control = base.transform;
		}
		if (useContainedMeshRenderers)
		{
			mrs = GetComponentsInChildren<MeshRenderer>();
		}
		if ((bool)material)
		{
			positionMaterialLocal = UnityEngine.Object.Instantiate(material);
			rotationMaterialLocal = UnityEngine.Object.Instantiate(material);
			materialOverlay = UnityEngine.Object.Instantiate(material);
		}
		if ((bool)followWhenOff)
		{
			_followWhenOffRB = followWhenOff.GetComponent<Rigidbody>();
		}
		kinematicRB = GetComponent<Rigidbody>();
		if (!(kinematicRB != null) || !followWhenOff)
		{
			return;
		}
		ConfigurableJoint[] components = followWhenOff.GetComponents<ConfigurableJoint>();
		ConfigurableJoint[] array = components;
		foreach (ConfigurableJoint configurableJoint in array)
		{
			if (configurableJoint.connectedBody == kinematicRB)
			{
				connectedJoint = configurableJoint;
				SetJointSprings();
				continue;
			}
			naturalJoint = configurableJoint;
			JointDrive slerpDrive = naturalJoint.slerpDrive;
			_jointRotationDriveSpring = slerpDrive.positionSpring;
			_jointRotationDriveDamper = slerpDrive.positionDamper;
			_jointRotationDriveMaxForce = slerpDrive.maximumForce;
			Vector3 eulerAngles = naturalJoint.targetRotation.eulerAngles;
			if (eulerAngles.x > 180f)
			{
				eulerAngles.x -= 360f;
			}
			else if (eulerAngles.x < -180f)
			{
				eulerAngles.x += 360f;
			}
			if (eulerAngles.y > 180f)
			{
				eulerAngles.y -= 360f;
			}
			else if (eulerAngles.y < -180f)
			{
				eulerAngles.y += 360f;
			}
			if (eulerAngles.z > 180f)
			{
				eulerAngles.z -= 360f;
			}
			else if (eulerAngles.z < -180f)
			{
				eulerAngles.z += 360f;
			}
			_jointRotationDriveXTarget = eulerAngles.x;
			_jointRotationDriveYTarget = eulerAngles.y;
			_jointRotationDriveZTarget = eulerAngles.z;
			if (naturalJoint.lowAngularXLimit.limit < naturalJoint.highAngularXLimit.limit)
			{
				_jointRotationDriveXTargetMin = naturalJoint.lowAngularXLimit.limit;
				_jointRotationDriveXTargetMax = naturalJoint.highAngularXLimit.limit;
			}
			else
			{
				_jointRotationDriveXTargetMin = naturalJoint.highAngularXLimit.limit;
				_jointRotationDriveXTargetMax = naturalJoint.lowAngularXLimit.limit;
			}
			_jointRotationDriveYTargetMin = 0f - naturalJoint.angularYLimit.limit;
			_jointRotationDriveYTargetMax = naturalJoint.angularYLimit.limit;
			_jointRotationDriveZTargetMin = 0f - naturalJoint.angularZLimit.limit;
			_jointRotationDriveZTargetMax = naturalJoint.angularZLimit.limit;
		}
	}

	private void InitUI()
	{
		if ((bool)linkLineMaterial)
		{
			linkLineDrawer = new LineDrawer(linkLineMaterial);
		}
		if (UIDText != null)
		{
			if (containingAtom != null)
			{
				UIDText.text = containingAtom.uid + ":" + base.name;
			}
			else
			{
				UIDText.text = base.name;
			}
			if (UIDTextAlt != null)
			{
				UIDTextAlt.text = UIDText.text;
			}
		}
		if (positionToggleGroup != null)
		{
		}
		if (rotationToggleGroup != null)
		{
		}
		if (holdPositionSpringSlider != null)
		{
			holdPositionSpringSlider.value = _RBHoldPositionSpring;
			SliderControl component = holdPositionSpringSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				component.defaultValue = _RBHoldPositionSpring;
			}
		}
		if (holdPositionDamperSlider != null)
		{
			holdPositionDamperSlider.value = _RBHoldPositionDamper;
			SliderControl component2 = holdPositionDamperSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				component2.defaultValue = _RBHoldPositionDamper;
			}
		}
		if (holdPositionMaxForceSlider != null)
		{
			holdPositionMaxForceSlider.value = _RBHoldPositionMaxForce;
			SliderControl component3 = holdPositionMaxForceSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				component3.defaultValue = _RBHoldPositionMaxForce;
			}
		}
		if (holdRotationSpringSlider != null)
		{
			holdRotationSpringSlider.value = _RBHoldRotationSpring;
			SliderControl component4 = holdRotationSpringSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				component4.defaultValue = _RBHoldRotationSpring;
			}
		}
		if (holdRotationDamperSlider != null)
		{
			holdRotationDamperSlider.value = _RBHoldRotationDamper;
			SliderControl component5 = holdRotationDamperSlider.GetComponent<SliderControl>();
			if (component5 != null)
			{
				component5.defaultValue = _RBHoldRotationDamper;
			}
		}
		if (holdRotationMaxForceSlider != null)
		{
			holdRotationMaxForceSlider.value = _RBHoldRotationMaxForce;
			SliderControl component6 = holdRotationMaxForceSlider.GetComponent<SliderControl>();
			if (component6 != null)
			{
				component6.defaultValue = _RBHoldRotationMaxForce;
			}
		}
		if (linkPositionSpringSlider != null)
		{
			linkPositionSpringSlider.value = _RBLinkPositionSpring;
			SliderControl component7 = linkPositionSpringSlider.GetComponent<SliderControl>();
			if (component7 != null)
			{
				component7.defaultValue = _RBLinkPositionSpring;
			}
		}
		if (linkPositionDamperSlider != null)
		{
			linkPositionDamperSlider.value = _RBLinkPositionDamper;
			SliderControl component8 = linkPositionDamperSlider.GetComponent<SliderControl>();
			if (component8 != null)
			{
				component8.defaultValue = _RBLinkPositionDamper;
			}
		}
		if (linkPositionMaxForceSlider != null)
		{
			linkPositionMaxForceSlider.value = _RBLinkPositionMaxForce;
			SliderControl component9 = linkPositionMaxForceSlider.GetComponent<SliderControl>();
			if (component9 != null)
			{
				component9.defaultValue = _RBLinkPositionMaxForce;
			}
		}
		if (linkRotationSpringSlider != null)
		{
			linkRotationSpringSlider.value = _RBLinkRotationSpring;
			SliderControl component10 = linkRotationSpringSlider.GetComponent<SliderControl>();
			if (component10 != null)
			{
				component10.defaultValue = _RBLinkRotationSpring;
			}
		}
		if (linkRotationDamperSlider != null)
		{
			linkRotationDamperSlider.value = _RBLinkRotationDamper;
			SliderControl component11 = linkRotationDamperSlider.GetComponent<SliderControl>();
			if (component11 != null)
			{
				component11.defaultValue = _RBLinkRotationDamper;
			}
		}
		if (linkRotationMaxForceSlider != null)
		{
			linkRotationMaxForceSlider.value = _RBLinkRotationMaxForce;
			SliderControl component12 = linkRotationMaxForceSlider.GetComponent<SliderControl>();
			if (component12 != null)
			{
				component12.defaultValue = _RBLinkRotationMaxForce;
			}
		}
		if (jointRotationDriveSpringSlider != null)
		{
			jointRotationDriveSpringSlider.value = _jointRotationDriveSpring;
			SliderControl component13 = jointRotationDriveSpringSlider.GetComponent<SliderControl>();
			if (component13 != null)
			{
				component13.defaultValue = _jointRotationDriveSpring;
			}
		}
		if (jointRotationDriveDamperSlider != null)
		{
			jointRotationDriveDamperSlider.value = _jointRotationDriveDamper;
			SliderControl component14 = jointRotationDriveDamperSlider.GetComponent<SliderControl>();
			if (component14 != null)
			{
				component14.defaultValue = _jointRotationDriveDamper;
			}
		}
		if (jointRotationDriveMaxForceSlider != null)
		{
			jointRotationDriveMaxForceSlider.value = _jointRotationDriveMaxForce;
			SliderControl component15 = jointRotationDriveMaxForceSlider.GetComponent<SliderControl>();
			if (component15 != null)
			{
				component15.defaultValue = _jointRotationDriveMaxForce;
			}
		}
		if (jointRotationDriveXTargetSlider != null)
		{
			jointRotationDriveXTargetSlider.minValue = _jointRotationDriveXTargetMin;
			jointRotationDriveXTargetSlider.maxValue = _jointRotationDriveXTargetMax;
			jointRotationDriveXTargetSlider.value = _jointRotationDriveXTarget;
			SliderControl component16 = jointRotationDriveXTargetSlider.GetComponent<SliderControl>();
			if (component16 != null)
			{
				component16.defaultValue = _jointRotationDriveXTarget;
			}
		}
		if (jointRotationDriveYTargetSlider != null)
		{
			jointRotationDriveYTargetSlider.minValue = _jointRotationDriveYTargetMin;
			jointRotationDriveYTargetSlider.maxValue = _jointRotationDriveYTargetMax;
			jointRotationDriveYTargetSlider.value = _jointRotationDriveYTarget;
			SliderControl component17 = jointRotationDriveYTargetSlider.GetComponent<SliderControl>();
			if (component17 != null)
			{
				component17.defaultValue = _jointRotationDriveYTarget;
			}
		}
		if (jointRotationDriveZTargetSlider != null)
		{
			jointRotationDriveZTargetSlider.minValue = _jointRotationDriveZTargetMin;
			jointRotationDriveZTargetSlider.maxValue = _jointRotationDriveZTargetMax;
			jointRotationDriveZTargetSlider.value = _jointRotationDriveZTarget;
			SliderControl component18 = jointRotationDriveZTargetSlider.GetComponent<SliderControl>();
			if (component18 != null)
			{
				component18.defaultValue = _jointRotationDriveZTarget;
			}
		}
		if (useGravityOnRBWhenOffToggle != null)
		{
			useGravityOnRBWhenOffToggle.isOn = useGravityOnRBWhenOff;
		}
		if (RBMassSlider != null)
		{
			RBMassSlider.value = RBMass;
			SliderControl component19 = RBMassSlider.GetComponent<SliderControl>();
			if (component19 != null)
			{
				component19.defaultValue = RBMass;
			}
		}
		if (physicsEnabledToggle != null)
		{
			physicsEnabledToggle.isOn = physicsEnabled;
		}
		if (collisionEnabledToggle != null)
		{
			collisionEnabled = collisionEnabledToggle.isOn;
		}
		if (interactableInPlayModeToggle != null)
		{
			interactableInPlayModeToggle.isOn = interactableInPlayMode;
		}
		if (xLockToggle != null)
		{
			xLockToggle.isOn = xLock;
		}
		if (yLockToggle != null)
		{
			yLockToggle.isOn = yLock;
		}
		if (zLockToggle != null)
		{
			zLockToggle.isOn = zLock;
		}
		if (xRotLockToggle != null)
		{
			xRotLockToggle.isOn = xRotLock;
		}
		if (yRotLockToggle != null)
		{
			yRotLockToggle.isOn = yRotLock;
		}
		if (zRotLockToggle != null)
		{
			zRotLockToggle.isOn = zLock;
		}
	}

	private void InitUIHandlers()
	{
		if (positionToggleGroup != null)
		{
			ToggleGroupValue toggleGroupValue = positionToggleGroup;
			toggleGroupValue.onToggleChangedHandlers = (ToggleGroupValue.OnToggleChanged)Delegate.Combine(toggleGroupValue.onToggleChangedHandlers, new ToggleGroupValue.OnToggleChanged(SetPositionStateFromString));
		}
		if (rotationToggleGroup != null)
		{
			ToggleGroupValue toggleGroupValue2 = rotationToggleGroup;
			toggleGroupValue2.onToggleChangedHandlers = (ToggleGroupValue.OnToggleChanged)Delegate.Combine(toggleGroupValue2.onToggleChangedHandlers, new ToggleGroupValue.OnToggleChanged(SetRotationStateFromString));
		}
		if (holdPositionSpringSlider != null)
		{
			holdPositionSpringSlider.onValueChanged.AddListener(delegate
			{
				RBHoldPositionSpring = holdPositionSpringSlider.value;
			});
		}
		if (holdPositionDamperSlider != null)
		{
			holdPositionDamperSlider.onValueChanged.AddListener(delegate
			{
				RBHoldPositionDamper = holdPositionDamperSlider.value;
			});
		}
		if (holdPositionMaxForceSlider != null)
		{
			holdPositionMaxForceSlider.onValueChanged.AddListener(delegate
			{
				RBHoldPositionMaxForce = holdPositionMaxForceSlider.value;
			});
		}
		if (holdRotationSpringSlider != null)
		{
			holdRotationSpringSlider.onValueChanged.AddListener(delegate
			{
				RBHoldRotationSpring = holdRotationSpringSlider.value;
			});
		}
		if (holdRotationDamperSlider != null)
		{
			holdRotationDamperSlider.onValueChanged.AddListener(delegate
			{
				RBHoldRotationDamper = holdRotationDamperSlider.value;
			});
		}
		if (holdRotationMaxForceSlider != null)
		{
			holdRotationMaxForceSlider.onValueChanged.AddListener(delegate
			{
				RBHoldRotationMaxForce = holdRotationMaxForceSlider.value;
			});
		}
		if (linkPositionSpringSlider != null)
		{
			linkPositionSpringSlider.onValueChanged.AddListener(delegate
			{
				RBLinkPositionSpring = linkPositionSpringSlider.value;
			});
		}
		if (linkPositionDamperSlider != null)
		{
			linkPositionDamperSlider.onValueChanged.AddListener(delegate
			{
				RBLinkPositionDamper = linkPositionDamperSlider.value;
			});
		}
		if (linkPositionMaxForceSlider != null)
		{
			linkPositionMaxForceSlider.onValueChanged.AddListener(delegate
			{
				RBLinkPositionMaxForce = linkPositionMaxForceSlider.value;
			});
		}
		if (linkRotationSpringSlider != null)
		{
			linkRotationSpringSlider.onValueChanged.AddListener(delegate
			{
				RBLinkRotationSpring = linkRotationSpringSlider.value;
			});
		}
		if (linkRotationDamperSlider != null)
		{
			linkRotationDamperSlider.onValueChanged.AddListener(delegate
			{
				RBLinkRotationDamper = linkRotationDamperSlider.value;
			});
		}
		if (linkRotationMaxForceSlider != null)
		{
			linkRotationMaxForceSlider.onValueChanged.AddListener(delegate
			{
				RBLinkRotationMaxForce = linkRotationMaxForceSlider.value;
			});
		}
		if (linkToAtomSelectionPopup != null)
		{
			UIPopup uIPopup = linkToAtomSelectionPopup;
			uIPopup.onOpenPopupHandlers = (UIPopup.OnOpenPopup)Delegate.Combine(uIPopup.onOpenPopupHandlers, new UIPopup.OnOpenPopup(SetLinkToAtomNames));
			UIPopup uIPopup2 = linkToAtomSelectionPopup;
			uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetLinkToAtom));
		}
		if (linkToSelectionPopup != null)
		{
			UIPopup uIPopup3 = linkToSelectionPopup;
			uIPopup3.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup3.onValueChangeHandlers, new UIPopup.OnValueChange(SetLinkToRigidbodyObject));
		}
		if (jointRotationDriveSpringSlider != null)
		{
			jointRotationDriveSpringSlider.onValueChanged.AddListener(delegate
			{
				jointRotationDriveSpring = jointRotationDriveSpringSlider.value;
			});
		}
		if (jointRotationDriveDamperSlider != null)
		{
			jointRotationDriveDamperSlider.onValueChanged.AddListener(delegate
			{
				jointRotationDriveDamper = jointRotationDriveDamperSlider.value;
			});
		}
		if (jointRotationDriveMaxForceSlider != null)
		{
			jointRotationDriveMaxForceSlider.onValueChanged.AddListener(delegate
			{
				jointRotationDriveMaxForce = jointRotationDriveMaxForceSlider.value;
			});
		}
		if (jointRotationDriveXTargetSlider != null)
		{
			jointRotationDriveXTargetSlider.onValueChanged.AddListener(delegate
			{
				jointRotationDriveXTarget = jointRotationDriveXTargetSlider.value;
			});
		}
		if (jointRotationDriveYTargetSlider != null)
		{
			jointRotationDriveYTargetSlider.onValueChanged.AddListener(delegate
			{
				jointRotationDriveYTarget = jointRotationDriveYTargetSlider.value;
			});
		}
		if (jointRotationDriveZTargetSlider != null)
		{
			jointRotationDriveZTargetSlider.onValueChanged.AddListener(delegate
			{
				jointRotationDriveZTarget = jointRotationDriveZTargetSlider.value;
			});
		}
		if (useGravityOnRBWhenOffToggle != null)
		{
			useGravityOnRBWhenOffToggle.onValueChanged.AddListener(delegate
			{
				useGravityOnRBWhenOff = useGravityOnRBWhenOffToggle.isOn;
			});
		}
		if (RBMassSlider != null)
		{
			RBMassSlider.onValueChanged.AddListener(delegate
			{
				RBMass = RBMassSlider.value;
			});
		}
		if (physicsEnabledToggle != null)
		{
			physicsEnabledToggle.onValueChanged.AddListener(delegate
			{
				physicsEnabled = physicsEnabledToggle.isOn;
			});
		}
		if (collisionEnabledToggle != null)
		{
			collisionEnabledToggle.onValueChanged.AddListener(delegate
			{
				collisionEnabled = collisionEnabledToggle.isOn;
			});
		}
		if (selectLinkToFromSceneButton != null)
		{
			selectLinkToFromSceneButton.onClick.AddListener(SelectLinkToRigidbodyFromScene);
		}
		if (selectAlignToFromSceneButton != null)
		{
			selectAlignToFromSceneButton.onClick.AddListener(SelectAlignToRigidbodyFromScene);
		}
		if (interactableInPlayModeToggle != null)
		{
			interactableInPlayModeToggle.onValueChanged.AddListener(delegate
			{
				interactableInPlayMode = interactableInPlayModeToggle.isOn;
			});
		}
		if (xLockToggle != null)
		{
			xLockToggle.onValueChanged.AddListener(delegate
			{
				xLock = xLockToggle.isOn;
			});
		}
		if (yLockToggle != null)
		{
			yLockToggle.onValueChanged.AddListener(delegate
			{
				yLock = yLockToggle.isOn;
			});
		}
		if (zLockToggle != null)
		{
			zLockToggle.onValueChanged.AddListener(delegate
			{
				zLock = zLockToggle.isOn;
			});
		}
		if (xRotLockToggle != null)
		{
			xRotLockToggle.onValueChanged.AddListener(delegate
			{
				xRotLock = xRotLockToggle.isOn;
			});
		}
		if (yRotLockToggle != null)
		{
			yRotLockToggle.onValueChanged.AddListener(delegate
			{
				yRotLock = yRotLockToggle.isOn;
			});
		}
		if (zRotLockToggle != null)
		{
			zRotLockToggle.onValueChanged.AddListener(delegate
			{
				zRotLock = zRotLockToggle.isOn;
			});
		}
		if (xPositionMinus1Button != null)
		{
			xPositionMinus1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(-1f, 0f, 0f);
			});
		}
		if (xPositionMinusPoint1Button != null)
		{
			xPositionMinusPoint1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(-0.1f, 0f, 0f);
			});
		}
		if (xPositionMinusPoint01Button != null)
		{
			xPositionMinusPoint01Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(-0.01f, 0f, 0f);
			});
		}
		if (xPositionPlusPoint01Button != null)
		{
			xPositionPlusPoint01Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0.01f, 0f, 0f);
			});
		}
		if (xPositionPlusPoint1Button != null)
		{
			xPositionPlusPoint1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0.1f, 0f, 0f);
			});
		}
		if (xPositionPlus1Button != null)
		{
			xPositionPlus1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(1f, 0f, 0f);
			});
		}
		if (yPositionMinus1Button != null)
		{
			yPositionMinus1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, -1f, 0f);
			});
		}
		if (yPositionMinusPoint1Button != null)
		{
			yPositionMinusPoint1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, -0.1f, 0f);
			});
		}
		if (yPositionMinusPoint01Button != null)
		{
			yPositionMinusPoint01Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, -0.01f, 0f);
			});
		}
		if (yPositionPlusPoint01Button != null)
		{
			yPositionPlusPoint01Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, 0.01f, 0f);
			});
		}
		if (yPositionPlusPoint1Button != null)
		{
			yPositionPlusPoint1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, 0.1f, 0f);
			});
		}
		if (yPositionPlus1Button != null)
		{
			yPositionPlus1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, 1f, 0f);
			});
		}
		if (zPositionMinus1Button != null)
		{
			zPositionMinus1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, 0f, -1f);
			});
		}
		if (zPositionMinusPoint1Button != null)
		{
			zPositionMinusPoint1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, 0f, -0.1f);
			});
		}
		if (zPositionMinusPoint01Button != null)
		{
			zPositionMinusPoint01Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, 0f, -0.01f);
			});
		}
		if (zPositionPlusPoint01Button != null)
		{
			zPositionPlusPoint01Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, 0f, 0.01f);
			});
		}
		if (zPositionPlusPoint1Button != null)
		{
			zPositionPlusPoint1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, 0f, 0.1f);
			});
		}
		if (zPositionPlus1Button != null)
		{
			zPositionPlus1Button.onClick.AddListener(delegate
			{
				MoveAbsoluteNoForce(0f, 0f, 1f);
			});
		}
		if (xRotationMinus45Button != null)
		{
			xRotationMinus45Button.onClick.AddListener(delegate
			{
				XRotationAdd(-45f);
			});
		}
		if (xRotationMinus5Button != null)
		{
			xRotationMinus5Button.onClick.AddListener(delegate
			{
				XRotationAdd(-5f);
			});
		}
		if (xRotationMinusPoint5Button != null)
		{
			xRotationMinusPoint5Button.onClick.AddListener(delegate
			{
				XRotationAdd(-0.5f);
			});
		}
		if (xRotationPlusPoint5Button != null)
		{
			xRotationPlusPoint5Button.onClick.AddListener(delegate
			{
				XRotationAdd(0.5f);
			});
		}
		if (xRotationPlus5Button != null)
		{
			xRotationPlus5Button.onClick.AddListener(delegate
			{
				XRotationAdd(5f);
			});
		}
		if (xRotationPlus45Button != null)
		{
			xRotationPlus45Button.onClick.AddListener(delegate
			{
				XRotationAdd(45f);
			});
		}
		if (yRotationMinus45Button != null)
		{
			yRotationMinus45Button.onClick.AddListener(delegate
			{
				YRotationAdd(-45f);
			});
		}
		if (yRotationMinus5Button != null)
		{
			yRotationMinus5Button.onClick.AddListener(delegate
			{
				YRotationAdd(-5f);
			});
		}
		if (yRotationMinusPoint5Button != null)
		{
			yRotationMinusPoint5Button.onClick.AddListener(delegate
			{
				YRotationAdd(-0.5f);
			});
		}
		if (yRotationPlusPoint5Button != null)
		{
			yRotationPlusPoint5Button.onClick.AddListener(delegate
			{
				YRotationAdd(0.5f);
			});
		}
		if (yRotationPlus5Button != null)
		{
			yRotationPlus5Button.onClick.AddListener(delegate
			{
				YRotationAdd(5f);
			});
		}
		if (yRotationPlus45Button != null)
		{
			yRotationPlus45Button.onClick.AddListener(delegate
			{
				YRotationAdd(45f);
			});
		}
		if (zRotationMinus45Button != null)
		{
			zRotationMinus45Button.onClick.AddListener(delegate
			{
				ZRotationAdd(-45f);
			});
		}
		if (zRotationMinus5Button != null)
		{
			zRotationMinus5Button.onClick.AddListener(delegate
			{
				ZRotationAdd(-5f);
			});
		}
		if (zRotationMinusPoint5Button != null)
		{
			zRotationMinusPoint5Button.onClick.AddListener(delegate
			{
				ZRotationAdd(-0.5f);
			});
		}
		if (zRotationPlusPoint5Button != null)
		{
			zRotationPlusPoint5Button.onClick.AddListener(delegate
			{
				ZRotationAdd(0.5f);
			});
		}
		if (zRotationPlus5Button != null)
		{
			zRotationPlus5Button.onClick.AddListener(delegate
			{
				ZRotationAdd(5f);
			});
		}
		if (zRotationPlus45Button != null)
		{
			zRotationPlus45Button.onClick.AddListener(delegate
			{
				ZRotationAdd(45f);
			});
		}
		if (xPosition0Button != null)
		{
			xPosition0Button.onClick.AddListener(delegate
			{
				MoveTo(0f, control.position.y, control.position.z);
			});
		}
		if (yPosition0Button != null)
		{
			yPosition0Button.onClick.AddListener(delegate
			{
				MoveTo(control.position.x, 0f, control.position.z);
			});
		}
		if (zPosition0Button != null)
		{
			zPosition0Button.onClick.AddListener(delegate
			{
				MoveTo(control.position.x, control.position.y, 0f);
			});
		}
		if (xRotation0Button != null)
		{
			xRotation0Button.onClick.AddListener(XRotation0);
		}
		if (yRotation0Button != null)
		{
			yRotation0Button.onClick.AddListener(YRotation0);
		}
		if (zRotation0Button != null)
		{
			zRotation0Button.onClick.AddListener(ZRotation0);
		}
		if (xPositionSnapPoint1Button != null)
		{
			xPositionSnapPoint1Button.onClick.AddListener(XPositionSnapPoint1);
		}
		if (yPositionSnapPoint1Button != null)
		{
			yPositionSnapPoint1Button.onClick.AddListener(YPositionSnapPoint1);
		}
		if (zPositionSnapPoint1Button != null)
		{
			zPositionSnapPoint1Button.onClick.AddListener(ZPositionSnapPoint1);
		}
		if (xRotationSnap1Button != null)
		{
			xRotationSnap1Button.onClick.AddListener(XRotationSnap1);
		}
		if (yRotationSnap1Button != null)
		{
			yRotationSnap1Button.onClick.AddListener(YRotationSnap1);
		}
		if (zRotationSnap1Button != null)
		{
			zRotationSnap1Button.onClick.AddListener(ZRotationSnap1);
		}
	}

	private Vector3 GetVectorFromAxis(DrawAxisnames axis)
	{
		Vector3 zero = Vector3.zero;
		switch (axis)
		{
		case DrawAxisnames.X:
			zero.x = 1f;
			break;
		case DrawAxisnames.Y:
			zero.y = 1f;
			break;
		case DrawAxisnames.Z:
			zero.z = 1f;
			break;
		case DrawAxisnames.NegX:
			zero.x = -1f;
			break;
		case DrawAxisnames.NegY:
			zero.y = -1f;
			break;
		case DrawAxisnames.NegZ:
			zero.z = -1f;
			break;
		}
		return zero;
	}

	private void ApplyForce()
	{
		if ((bool)_followWhenOffRB)
		{
			if (_moveForceEnabled)
			{
				_followWhenOffRB.AddForce(appliedForce * Time.fixedDeltaTime, ForceMode.Force);
			}
			if (_rotationForceEnabled)
			{
				_followWhenOffRB.AddRelativeTorque(appliedTorque * Time.fixedDeltaTime, ForceMode.Force);
			}
		}
	}

	private void UpdateTransform(bool updateGUI)
	{
		if (_currentPositionState == PositionState.Off)
		{
			if ((bool)followWhenOff)
			{
				control.position = followWhenOff.position;
			}
		}
		else if (_currentPositionState == PositionState.Following)
		{
			if ((bool)follow)
			{
				Vector3 position = control.position;
				if (!xLock)
				{
					position.x = follow.position.x;
				}
				if (!yLock)
				{
					position.y = follow.position.y;
				}
				if (!zLock)
				{
					position.z = follow.position.z;
				}
				control.position = position;
			}
		}
		else if ((_currentPositionState == PositionState.ParentLink || _currentPositionState == PositionState.PhysicsLink) && (bool)_linkToConnector)
		{
			Vector3 position2 = control.position;
			if (!xLock)
			{
				position2.x = _linkToConnector.position.x;
			}
			if (!yLock)
			{
				position2.y = _linkToConnector.position.y;
			}
			if (!zLock)
			{
				position2.z = _linkToConnector.position.z;
			}
			control.position = position2;
		}
		if (_currentRotationState == RotationState.Off)
		{
			if ((bool)followWhenOff)
			{
				control.rotation = followWhenOff.rotation;
			}
		}
		else if (_currentRotationState == RotationState.Following)
		{
			if ((bool)follow)
			{
				Vector3 eulerAngles = control.eulerAngles;
				Vector3 eulerAngles2 = follow.eulerAngles;
				if (!xRotLock)
				{
					eulerAngles.x = eulerAngles2.x;
				}
				if (!yRotLock)
				{
					eulerAngles.y = eulerAngles2.y;
				}
				if (!zRotLock)
				{
					eulerAngles.z = eulerAngles2.z;
				}
				control.eulerAngles = eulerAngles;
			}
		}
		else if (_currentRotationState == RotationState.LookAt)
		{
			control.LookAt(lookAt.position);
		}
		else if ((_currentRotationState == RotationState.ParentLink || _currentPositionState == PositionState.PhysicsLink) && (bool)_linkToConnector)
		{
			Vector3 eulerAngles3 = control.eulerAngles;
			Vector3 eulerAngles4 = _linkToConnector.eulerAngles;
			if (!xRotLock)
			{
				eulerAngles3.x = eulerAngles4.x;
			}
			if (!yRotLock)
			{
				eulerAngles3.y = eulerAngles4.y;
			}
			if (!zRotLock)
			{
				eulerAngles3.z = eulerAngles4.z;
			}
			control.eulerAngles = eulerAngles3;
		}
		if (control != base.transform)
		{
			base.transform.position = control.position;
			base.transform.rotation = control.rotation;
		}
		if (alsoMoveWhenInactive != null && !alsoMoveWhenInactive.gameObject.activeInHierarchy)
		{
			alsoMoveWhenInactive.position = control.position;
			alsoMoveWhenInactive.rotation = control.rotation;
		}
		if (_followWhenOffRB != null)
		{
			if (!_followWhenOffRB.gameObject.activeInHierarchy)
			{
				_followWhenOffRB.position = control.position;
				_followWhenOffRB.transform.position = control.position;
				_followWhenOffRB.rotation = control.rotation;
				_followWhenOffRB.transform.rotation = control.rotation;
			}
			else if (_followWhenOffRB.isKinematic)
			{
				if (_currentPositionState != PositionState.Off)
				{
					_followWhenOffRB.position = control.position;
				}
				if (_currentRotationState != RotationState.Off)
				{
					_followWhenOffRB.rotation = control.rotation;
				}
			}
		}
		if (updateGUI && !_guihidden)
		{
			if (xPositionText != null)
			{
				xPositionText.floatVal = control.position.x;
			}
			if (yPositionText != null)
			{
				yPositionText.floatVal = control.position.y;
			}
			if (zPositionText != null)
			{
				zPositionText.floatVal = control.position.z;
			}
			Vector3 eulerAngles5 = control.eulerAngles;
			if (xRotationText != null)
			{
				xRotationText.floatVal = eulerAngles5.x;
			}
			if (yRotationText != null)
			{
				yRotationText.floatVal = eulerAngles5.y;
			}
			if (zRotationText != null)
			{
				zRotationText.floatVal = eulerAngles5.z;
			}
		}
	}

	public void ShowGUI()
	{
		bool flag = false;
		if (SuperController.singleton != null && SuperController.singleton.gameMode == SuperController.GameMode.Play)
		{
			flag = true;
		}
		if (flag)
		{
			Transform[] uITransforms = UITransforms;
			foreach (Transform transform in uITransforms)
			{
				if (transform != null)
				{
					transform.gameObject.SetActive(value: false);
				}
			}
			Transform[] uITransformsPlayMode = UITransformsPlayMode;
			foreach (Transform transform2 in uITransformsPlayMode)
			{
				if (transform2 != null)
				{
					transform2.gameObject.SetActive(value: true);
				}
			}
			return;
		}
		Transform[] uITransforms2 = UITransforms;
		foreach (Transform transform3 in uITransforms2)
		{
			if (transform3 != null)
			{
				transform3.gameObject.SetActive(value: true);
			}
		}
		Transform[] uITransformsPlayMode2 = UITransformsPlayMode;
		foreach (Transform transform4 in uITransformsPlayMode2)
		{
			if (transform4 != null)
			{
				transform4.gameObject.SetActive(value: false);
			}
		}
	}

	public void HideGUI()
	{
		bool flag = false;
		if (SuperController.singleton != null && SuperController.singleton.gameMode == SuperController.GameMode.Play)
		{
			flag = true;
		}
		Transform[] uITransforms = UITransforms;
		foreach (Transform transform in uITransforms)
		{
			if (!(transform != null))
			{
				continue;
			}
			if (flag)
			{
				transform.gameObject.SetActive(value: false);
				continue;
			}
			UIVisibility component = transform.GetComponent<UIVisibility>();
			if (component != null)
			{
				if (!component.keepVisible)
				{
					transform.gameObject.SetActive(value: false);
				}
			}
			else
			{
				transform.gameObject.SetActive(value: false);
			}
		}
		Transform[] uITransformsPlayMode = UITransformsPlayMode;
		foreach (Transform transform2 in uITransformsPlayMode)
		{
			if (!(transform2 != null))
			{
				continue;
			}
			if (!flag)
			{
				transform2.gameObject.SetActive(value: false);
				continue;
			}
			UIVisibility component2 = transform2.GetComponent<UIVisibility>();
			if (component2 != null)
			{
				if (!component2.keepVisible)
				{
					transform2.gameObject.SetActive(value: false);
				}
			}
			else
			{
				transform2.gameObject.SetActive(value: false);
			}
		}
	}

	private void SetColor()
	{
		if (_selected)
		{
			_currentPositionColor = selectedColor;
			_currentRotationColor = selectedColor;
		}
		else if (_highlighted)
		{
			_currentPositionColor = highlightColor;
			_currentRotationColor = highlightColor;
		}
		else
		{
			switch (_currentPositionState)
			{
			case PositionState.On:
				_currentPositionColor = onColor;
				break;
			case PositionState.Off:
				_currentPositionColor = offColor;
				break;
			case PositionState.Following:
			case PositionState.ParentLink:
			case PositionState.PhysicsLink:
				_currentPositionColor = followingColor;
				break;
			case PositionState.Hold:
				_currentPositionColor = holdColor;
				break;
			case PositionState.Lock:
				_currentPositionColor = lockColor;
				break;
			}
			switch (_currentRotationState)
			{
			case RotationState.On:
				_currentRotationColor = onColor;
				break;
			case RotationState.Off:
				_currentRotationColor = offColor;
				break;
			case RotationState.Following:
			case RotationState.ParentLink:
			case RotationState.PhysicsLink:
				_currentRotationColor = followingColor;
				break;
			case RotationState.Hold:
				_currentRotationColor = holdColor;
				break;
			case RotationState.Lock:
				_currentRotationColor = lockColor;
				break;
			case RotationState.LookAt:
				_currentRotationColor = lookAtColor;
				break;
			}
		}
		if (mrs != null)
		{
			MeshRenderer[] array = mrs;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.material.color = _currentPositionColor;
			}
		}
	}

	private void SetMesh()
	{
		switch (_currentPositionState)
		{
		case PositionState.Off:
			_currentPositionMesh = offPositionMesh;
			break;
		case PositionState.On:
			_currentPositionMesh = onPositionMesh;
			break;
		case PositionState.Following:
		case PositionState.ParentLink:
		case PositionState.PhysicsLink:
			_currentPositionMesh = followingPositionMesh;
			break;
		case PositionState.Hold:
			_currentPositionMesh = holdPositionMesh;
			break;
		case PositionState.Lock:
			_currentPositionMesh = lockPositionMesh;
			break;
		}
		switch (_currentRotationState)
		{
		case RotationState.Off:
			_currentRotationMesh = offRotationMesh;
			break;
		case RotationState.On:
			_currentRotationMesh = onRotationMesh;
			break;
		case RotationState.Following:
		case RotationState.ParentLink:
		case RotationState.PhysicsLink:
			_currentRotationMesh = followingRotationMesh;
			break;
		case RotationState.Hold:
			_currentRotationMesh = holdRotationMesh;
			break;
		case RotationState.Lock:
			_currentRotationMesh = lockRotationMesh;
			break;
		case RotationState.LookAt:
			_currentRotationMesh = lookAtRotationMesh;
			break;
		}
	}

	private void StateChanged()
	{
		SetMesh();
		SetColor();
	}

	public void SetPositionStateFromString(string state)
	{
		try
		{
			PositionState positionState2 = (currentPositionState = (PositionState)Enum.Parse(typeof(PositionState), state));
		}
		catch (ArgumentException)
		{
			Debug.LogError("State " + state + " is not a valid position state");
		}
	}

	public void SetRotationStateFromString(string state)
	{
		try
		{
			RotationState rotationState2 = (currentRotationState = (RotationState)Enum.Parse(typeof(RotationState), state));
		}
		catch (ArgumentException)
		{
			Debug.LogError("State " + state + " is not a valid rotation state");
		}
	}

	public void ResetControl()
	{
		control.localPosition = initialLocalPosition;
		control.localRotation = initialLocalRotation;
	}

	public void Move(Vector3 direction)
	{
		if (_moveForceEnabled && (bool)_followWhenOffRB && useForceWhenOff)
		{
			appliedForce += direction * forceFactor;
		}
		else if (_moveEnabled)
		{
			Vector3 translation = direction * moveFactor * Time.deltaTime / Time.timeScale;
			control.Translate(translation, Space.World);
			if (connectedJoint != null)
			{
				_followWhenOffRB.WakeUp();
			}
		}
	}

	public void MoveAbsoluteNoForce(Vector3 direction)
	{
		if (!_moveForceEnabled && _moveEnabled)
		{
			control.Translate(direction, Space.World);
		}
	}

	public void MoveAbsoluteNoForce(float x, float y, float z)
	{
		Vector3 direction = default(Vector3);
		direction.x = x;
		direction.y = y;
		direction.z = z;
		MoveAbsoluteNoForce(direction);
	}

	public void MoveTo(Vector3 pos)
	{
		if (!_moveForceEnabled && _moveEnabled)
		{
			control.position = pos;
		}
	}

	public void MoveTo(float x, float y, float z)
	{
		Vector3 pos = default(Vector3);
		pos.x = x;
		pos.y = y;
		pos.z = z;
		MoveTo(pos);
	}

	public void RotateX(float val)
	{
		if (_rotationForceEnabled && (bool)_followWhenOffRB && useForceWhenOff)
		{
			appliedTorque.x = val * torqueFactor;
		}
		else if (_rotationEnabled)
		{
			control.Rotate(new Vector3(val * rotateFactor * Time.deltaTime / Time.timeScale, 0f, 0f));
			if (connectedJoint != null)
			{
				_followWhenOffRB.WakeUp();
			}
		}
	}

	public void RotateY(float val)
	{
		if (_rotationForceEnabled && (bool)_followWhenOffRB && useForceWhenOff)
		{
			appliedTorque.y = val * torqueFactor;
		}
		else if (_rotationEnabled)
		{
			control.Rotate(new Vector3(0f, val * rotateFactor * Time.deltaTime / Time.timeScale, 0f));
			if (connectedJoint != null)
			{
				_followWhenOffRB.WakeUp();
			}
		}
	}

	public void RotateZ(float val)
	{
		if (_rotationForceEnabled && (bool)_followWhenOffRB && useForceWhenOff)
		{
			appliedTorque.z = val * torqueFactor;
		}
		else if (_rotationEnabled)
		{
			control.Rotate(new Vector3(0f, 0f, val * rotateFactor * Time.deltaTime / Time.timeScale));
			if (connectedJoint != null)
			{
				_followWhenOffRB.WakeUp();
			}
		}
	}

	public void RotateWorldX(float val, bool absolute = false)
	{
		if (!_rotationForceEnabled && _rotationEnabled)
		{
			float num = val;
			if (!absolute)
			{
				num *= rotateFactor * Time.deltaTime / Time.timeScale;
			}
			control.Rotate(num, 0f, 0f, Space.World);
		}
	}

	public void RotateWorldY(float val, bool absolute = false)
	{
		if (!_rotationForceEnabled && _rotationEnabled)
		{
			float num = val;
			if (!absolute)
			{
				num *= rotateFactor * Time.deltaTime / Time.timeScale;
			}
			control.Rotate(0f, num, 0f, Space.World);
		}
	}

	public void RotateWorldZ(float val, bool absolute = false)
	{
		if (!_rotationForceEnabled && _rotationEnabled)
		{
			float num = val;
			if (!absolute)
			{
				num *= rotateFactor * Time.deltaTime / Time.timeScale;
			}
			control.Rotate(0f, 0f, num, Space.World);
		}
	}

	public void ResetAppliedForces()
	{
		appliedForce.x = 0f;
		appliedForce.y = 0f;
		appliedForce.z = 0f;
		appliedTorque.x = 0f;
		appliedTorque.y = 0f;
		appliedTorque.z = 0f;
	}

	public void MoveAxis(MoveAxisnames man, float val)
	{
		switch (man)
		{
		case MoveAxisnames.X:
			Move(new Vector3(val, 0f, 0f));
			break;
		case MoveAxisnames.Y:
			Move(new Vector3(0f, val, 0f));
			break;
		case MoveAxisnames.Z:
			Move(new Vector3(0f, 0f, val));
			break;
		case MoveAxisnames.CameraRight:
		{
			Vector3 direction5 = Camera.main.transform.right * val;
			Move(direction5);
			break;
		}
		case MoveAxisnames.CameraRightNoY:
		{
			Vector3 direction4 = Camera.main.transform.right * val;
			direction4.y = 0f;
			Move(direction4);
			break;
		}
		case MoveAxisnames.CameraForward:
		{
			Vector3 direction3 = Camera.main.transform.forward * val;
			Move(direction3);
			break;
		}
		case MoveAxisnames.CameraForwardNoY:
		{
			Vector3 direction2 = Camera.main.transform.forward * val;
			direction2.y = 0f;
			Move(direction2);
			break;
		}
		case MoveAxisnames.CameraUp:
		{
			Vector3 direction = Camera.main.transform.up * val;
			Move(direction);
			break;
		}
		}
	}

	public void RotateAxis(RotateAxisnames ran, float val)
	{
		switch (ran)
		{
		case RotateAxisnames.X:
			RotateX(val);
			break;
		case RotateAxisnames.NegX:
			RotateX(0f - val);
			break;
		case RotateAxisnames.Y:
			RotateY(val);
			break;
		case RotateAxisnames.NegY:
			RotateY(0f - val);
			break;
		case RotateAxisnames.Z:
			RotateZ(val);
			break;
		case RotateAxisnames.NegZ:
			RotateZ(0f - val);
			break;
		case RotateAxisnames.WorldY:
			RotateWorldY(val);
			break;
		}
	}

	public void ControlAxis1(float val)
	{
		if (controlMode == ControlMode.Rotation)
		{
			RotateAxis(RotateAxis1, val);
		}
		else if (controlMode == ControlMode.Position)
		{
			MoveAxis(MoveAxis1, val);
		}
	}

	public void ControlAxis2(float val)
	{
		if (controlMode == ControlMode.Rotation)
		{
			RotateAxis(RotateAxis2, val);
		}
		else if (controlMode == ControlMode.Position)
		{
			MoveAxis(MoveAxis2, val);
		}
	}

	public void ControlAxis3(float val)
	{
		if (controlMode == ControlMode.Rotation)
		{
			RotateAxis(RotateAxis3, val);
		}
		else if (controlMode == ControlMode.Position)
		{
			MoveAxis(MoveAxis3, val);
		}
	}

	private void Init()
	{
		if (!wasInit)
		{
			if (control == null)
			{
				control = base.transform;
			}
			wasInit = true;
			startingPosition = base.transform.position;
			startingRotation = base.transform.rotation;
			_startingInteractableInPlayMode = _interactableInPlayMode;
			_startingCollsionEnabled = collisionEnabled;
			_startingPhysicsEnabled = physicsEnabled;
			_startingUseGravityOnRBWhenOff = _useGravityOnRBWhenOff;
			initialLocalPosition = control.localPosition;
			initialLocalRotation = control.localRotation;
			bool flag = stateCanBeModified;
			stateCanBeModified = true;
			currentPositionState = startingPositionState;
			currentRotationState = startingRotationState;
			stateCanBeModified = flag;
		}
	}

	private void Start()
	{
		Init();
		InitUIHandlers();
		InitUI();
		HideGUI();
	}

	private void FixedUpdate()
	{
		if (_followWhenOffRB != null && _followWhenOffRB.isKinematic)
		{
			UpdateTransform(updateGUI: false);
		}
		ApplyForce();
	}

	private void Update()
	{
		UpdateTransform(updateGUI: true);
		if (((bool)_currentPositionMesh || (bool)_currentRotationMesh) && !hidden)
		{
			Matrix4x4 localToWorldMatrix = control.localToWorldMatrix;
			Vector3 vectorFromAxis = GetVectorFromAxis(MeshForwardAxis);
			Vector3 vectorFromAxis2 = GetVectorFromAxis(MeshUpAxis);
			Quaternion quaternion = Quaternion.LookRotation(vectorFromAxis, vectorFromAxis2);
			Vector3 vectorFromAxis3 = GetVectorFromAxis(DrawForwardAxis);
			Vector3 vectorFromAxis4 = GetVectorFromAxis(DrawUpAxis);
			Quaternion quaternion2 = Quaternion.LookRotation(vectorFromAxis3, vectorFromAxis4);
			Quaternion q = quaternion2 * quaternion;
			float num = meshScale;
			num = (_selected ? (num * selectedScale) : ((!_highlighted) ? (num * unhighlightedScale) : (num * highlightedScale)));
			Vector3 s = new Vector3(num, num, num);
			Matrix4x4 identity = Matrix4x4.identity;
			identity.SetTRS(Vector3.zero, q, s);
			Matrix4x4 matrix = localToWorldMatrix * identity;
			if ((bool)_currentPositionMesh)
			{
				positionMaterialLocal.SetFloat("_Alpha", targetAlpha);
				positionMaterialLocal.color = _currentPositionColor;
				Graphics.DrawMesh(_currentPositionMesh, matrix, positionMaterialLocal, base.gameObject.layer, null, 0, null, castShadows: false, receiveShadows: false);
			}
			if ((bool)_currentRotationMesh)
			{
				rotationMaterialLocal.SetFloat("_Alpha", targetAlpha);
				rotationMaterialLocal.color = _currentRotationColor;
				Graphics.DrawMesh(_currentRotationMesh, matrix, rotationMaterialLocal, base.gameObject.layer, null, 0, null, castShadows: false, receiveShadows: false);
			}
			if (_selected)
			{
				materialOverlay.SetFloat("_Alpha", targetAlpha);
				materialOverlay.color = overlayColor;
				if (_controlMode == ControlMode.Position)
				{
					if ((bool)moveModeOverlayMesh)
					{
						Graphics.DrawMesh(moveModeOverlayMesh, matrix, materialOverlay, base.gameObject.layer, null, 0, null, castShadows: false, receiveShadows: false);
					}
				}
				else if (_controlMode == ControlMode.Rotation && (bool)rotateModeOverlayMesh)
				{
					Graphics.DrawMesh(rotateModeOverlayMesh, matrix, materialOverlay, base.gameObject.layer, null, 0, null, castShadows: false, receiveShadows: false);
				}
			}
		}
		if (linkLineDrawer != null && _linkToRB != null && !_hidden)
		{
			ForceReceiver component = _linkToRB.GetComponent<ForceReceiver>();
			if (component == null || !component.skipUIDrawing)
			{
				linkLineMaterial.SetFloat("_Alpha", targetAlpha);
				linkLineDrawer.SetLinePoints(base.transform.position, _linkToRB.transform.position);
				linkLineDrawer.Draw(base.gameObject.layer);
			}
		}
	}
}
