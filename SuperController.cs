using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using uFileBrowser;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using Valve.VR;

public class SuperController : MonoBehaviour
{
	public enum GameMode
	{
		Edit,
		Play
	}

	public enum ActiveUI
	{
		None,
		MainMenu,
		MainMenuOnly,
		SceneOptions,
		UserPreferences,
		SelectedOptions,
		MultiButtonPanel,
		EmbeddedScenePanel
	}

	private enum SelectMode
	{
		Off,
		FilteredTargets,
		Targets,
		Controller,
		ForceReceiver,
		ForceProducer,
		Rigidbody,
		Atom
	}

	public delegate void SelectControllerCallback(FreeControllerV3 fc);

	public delegate void SelectForceProducerCallback(ForceProducerV2 fp);

	public delegate void SelectForceReceiverCallback(ForceReceiver fr);

	public delegate void SelectRigidbodyCallback(Rigidbody rb);

	public delegate void SelectAtomCallback(Atom a);

	public delegate void OnForceReceiverNamesChanged(string[] receiverNames);

	public delegate void OnForceProducerNamesChanged(string[] producerNames);

	public delegate void OnFreeControllerNamesChanged(string[] controllerNames);

	public delegate void OnRigidbodyNamesChanged(string[] rigidbodyNames);

	public delegate void OnAtomUIDsChanged(List<string> atomUIDs);

	public delegate void OnAtomUIDsWithForceReceiversChanged(List<string> atomUIDs);

	public delegate void OnAtomUIDsWithForceProducersChanged(List<string> atomUIDs);

	public delegate void OnAtomUIDsWithFreeControllersChanged(List<string> atomUIDs);

	public delegate void OnAtomUIDsWithRigidbodiesChanged(List<string> atomUIDs);

	private static SuperController _singleton;

	public Camera screenshotCamera;

	public string savesDir = "Saves/";

	public bool enableStartScene = true;

	public JSONEmbed startJSONEmbedScene;

	public bool disableUI;

	public bool disableNavigation;

	private bool onStartScene;

	public string startSceneName = "start.json";

	public string newSceneName = "default.json";

	public string[] editorSceneList;

	protected int saveCount = -1;

	protected string savingName;

	public JSONNode loadJson;

	public Transform atomContainer;

	protected string loadedName;

	public FileBrowser fileBrowserUI;

	public MultiButtonPanel multiButtonPanel;

	protected bool loadMerge;

	protected bool isLoading;

	public string buttonToggleMainHUD = "ButtonStart";

	public Transform mainHUD;

	public Transform mainMenuUI;

	public Transform sceneOptionsUI;

	public bool useAltUI;

	public Transform sceneControlUI;

	public Transform sceneControlUIAlt;

	public Transform userPreferencesUI;

	public Transform embeddedSceneUI;

	public float targetAlpha;

	public Material rayLineMaterialRight;

	public Material rayLineMaterialLeft;

	private LineDrawer rayLineDrawerRight;

	private LineDrawer rayLineDrawerLeft;

	[SerializeField]
	private GameMode _gameMode;

	public Toggle editModeToggle;

	public Toggle playModeToggle;

	public Toggle helpToggle;

	public Toggle helpToggleAlt;

	public Transform helpOverlayOVR;

	public Transform helpOverlayVive;

	public bool helpOnAtStart;

	private ActiveUI _lastActiveUI;

	private ActiveUI _activeUI = ActiveUI.SelectedOptions;

	private bool _freezeAnimation;

	public LayerMask targetColliderMask;

	public Transform selectPrefab;

	public LayerMask selectColliderMask;

	private FreeControllerV3 selectedController;

	public Text selectedControllerNameDisplay;

	private SelectMode selectMode;

	public bool useLookSelect;

	public Camera lookCamera;

	public string buttonToggleTargets = "ButtonBack";

	private bool targetsOnWithButton;

	public string buttonSelect = "ButtonA";

	public string buttonUnselect = "ButtonB";

	public string buttonToggleRotateMode = "ButtonY";

	public string buttonCycleSelection = "ButtonX";

	public JoystickControl.Axis navigationForwardAxis = JoystickControl.Axis.LeftStickY;

	public bool invertNavigationForwardAxis;

	public JoystickControl.Axis navigationSideAxis = JoystickControl.Axis.LeftStickX;

	public bool invertNavigationSideAxis;

	public JoystickControl.Axis navigationTurnAxis = JoystickControl.Axis.RightStickX;

	public bool invertNavigationTurnAxis;

	public JoystickControl.Axis navigationUpAxis = JoystickControl.Axis.RightStickY;

	public bool invertNavigationUpAxis;

	public bool navigationForwardAxisEnabled = true;

	public bool navigationSideAxisEnabled = true;

	public bool navigationTurnAxisEnabled = true;

	public bool navigationUpAxisEnabled = true;

	private bool _navigationForwardAxisEnabled = true;

	private bool _navigationSideAxisEnabled = true;

	private bool _navigationTurnAxisEnabled = true;

	private bool _navigationUpAxisEnabled = true;

	public JoystickControl.Axis axis1 = JoystickControl.Axis.RightStickX;

	public JoystickControl.Axis axis2 = JoystickControl.Axis.RightStickY;

	public JoystickControl.Axis axis3 = JoystickControl.Axis.Triggers;

	public JoystickControl.Axis tabAxis = JoystickControl.Axis.DPadX;

	public bool invertAxis1 = true;

	public bool invertAxis2 = true;

	public bool invertAxis3 = true;

	public Transform selectionHUDTransform;

	private SelectionHUD selectionHUD;

	private List<FreeControllerV3> highlightedControllersLook;

	private List<SelectTarget> highlightedSelectTargetsLook;

	public CameraTarget centerCameraTarget;

	public Camera leftControllerCamera;

	public Camera rightControllerCamera;

	public Transform rightSelectionHUDTransform;

	public Transform leftSelectionHUDTransform;

	private SelectionHUD rightSelectionHUD;

	private SelectionHUD leftSelectionHUD;

	private List<FreeControllerV3> highlightedControllersLeft;

	private List<FreeControllerV3> highlightedControllersRight;

	private FreeControllerV3 rightGrabbedController;

	private FreeControllerV3 leftGrabbedController;

	private FreeControllerV3 rightFullGrabbedController;

	private FreeControllerV3 leftFullGrabbedController;

	private HandControl leftHandControl;

	private HandControl rightHandControl;

	private bool leftTriggerOn;

	private bool rightTriggerOn;

	private List<SelectTarget> highlightedSelectTargetsLeft;

	private List<SelectTarget> highlightedSelectTargetsRight;

	public Transform ViveRig;

	public Camera ViveCenterCamera;

	public SteamVR_TrackedObject viveObjectLeft;

	public SteamVR_TrackedObject viveObjectRight;

	public Transform OVRRig;

	public Camera OVRCenterCamera;

	public Transform touchObjectLeft;

	public Transform touchObjectRight;

	public float maxAngularVelocity = 20f;

	public float maxDepenetrationVelocity = 100f;

	private List<FreeControllerV3> allControllers;

	private List<AnimationPattern> allAnimationPatterns;

	private List<AnimationStep> allAnimationSteps;

	private List<Animator> allAnimators;

	private List<Canvas> allCanvases;

	public Slider worldScaleSlider;

	private float _worldScale = 1f;

	public Slider controllerScaleSlider;

	private float _controllerScale = 1f;

	private List<Transform> selectionInstances;

	private SelectControllerCallback selectControllerCallback;

	private SelectForceProducerCallback selectForceProducerCallback;

	private SelectForceReceiverCallback selectForceReceiverCallback;

	private SelectRigidbodyCallback selectRigidbodyCallback;

	private SelectAtomCallback selectAtomCallback;

	public Transform mainHUDAttachPoint;

	private Vector3 mainHUDAttachPointStartingPosition;

	private Quaternion mainHUDAttachPointStartingRotation;

	public bool showMainHUDOnStart;

	private bool _mainHUDVisible;

	private bool _pointerModeLeft;

	private bool _pointerModeRight;

	private bool GUIhit;

	private bool GUIhitLeft;

	private bool GUIhitRight;

	private bool leftGUIInteract;

	private bool rightGUIInteract;

	private bool tabAxisOn;

	private Dictionary<string, bool> uids;

	private Dictionary<string, Atom> atoms;

	private Dictionary<string, Atom> startingAtoms;

	private List<string> atomUIDs;

	private List<string> atomUIDsWithForceReceivers;

	private List<string> atomUIDsWithForceProducers;

	private List<string> atomUIDsWithFreeControllers;

	private List<string> atomUIDsWithRigidbodies;

	private Dictionary<string, ForceReceiver> frMap;

	private Dictionary<string, ForceProducerV2> fpMap;

	private Dictionary<string, FreeControllerV3> fcMap;

	private Dictionary<string, Rigidbody> rbMap;

	private int maxUID = 1000;

	public OnForceReceiverNamesChanged onForceReceiverNamesChangedHandlers;

	private string[] _forceReceiverNames;

	public OnForceProducerNamesChanged onForceProducerNamesChangedHandlers;

	private string[] _forceProducerNames;

	public OnFreeControllerNamesChanged onFreeControllerNamesChangedHandlers;

	private string[] _freeControllerNames;

	public OnRigidbodyNamesChanged onRigidbodyNamesChangedHandlers;

	private string[] _rigidbodyNames;

	public Atom[] atomPrefabs;

	public Atom[] indirectAtomPrefabs;

	protected Dictionary<string, Atom> atomPrefabByType;

	public UIPopup atomPrefabPopup;

	public OnAtomUIDsChanged onAtomUIDsChangedHandlers;

	public OnAtomUIDsWithForceReceiversChanged onAtomUIDsWithForceReceiversChangedHandlers;

	public OnAtomUIDsWithForceProducersChanged onAtomUIDsWithForceProducersChangedHandlers;

	public OnAtomUIDsWithFreeControllersChanged onAtomUIDsWithFreeControllersChangedHandlers;

	public OnAtomUIDsWithRigidbodiesChanged onAtomUIDsWithRigidbodiesChangedHandlers;

	public Transform navigationPlayArea;

	public Transform regularPlayArea;

	public Transform navigationRig;

	public Transform navigationPlayer;

	public Transform navigationCamera;

	public CubicBezierCurve navigationCurve;

	public float navigationDistance = 100f;

	public bool useLookForNavigation = true;

	public LayerMask navigationColliderMask;

	private Quaternion startDoubleTouchRotation;

	public Transform environmentColliders;

	public Slider environmentColliderHeightSlider;

	public Slider environmentColliderHeightSliderAlt;

	private float _environmentColliderHeight;

	private Ray castRay;

	private bool _useInterpolation = true;

	public static SuperController singleton => _singleton;

	public GameMode gameMode
	{
		get
		{
			return _gameMode;
		}
		set
		{
			if (_gameMode == value)
			{
				return;
			}
			_gameMode = value;
			if (_gameMode == GameMode.Edit)
			{
				if (editModeToggle != null)
				{
					editModeToggle.isOn = true;
				}
				if (playModeToggle != null)
				{
					playModeToggle.isOn = false;
				}
				if (_activeUI == ActiveUI.SelectedOptions)
				{
					activeUI = ActiveUI.SelectedOptions;
				}
			}
			else
			{
				if (editModeToggle != null)
				{
					editModeToggle.isOn = false;
				}
				if (playModeToggle != null)
				{
					playModeToggle.isOn = true;
				}
				if (_activeUI == ActiveUI.SelectedOptions)
				{
					activeUI = ActiveUI.SelectedOptions;
				}
			}
		}
	}

	public ActiveUI lastActiveUI => _lastActiveUI;

	public ActiveUI activeUI
	{
		get
		{
			return _activeUI;
		}
		set
		{
			_lastActiveUI = _activeUI;
			_activeUI = value;
			switch (_activeUI)
			{
			case ActiveUI.None:
				if (mainMenuUI != null)
				{
					mainMenuUI.gameObject.SetActive(value: false);
				}
				if (sceneOptionsUI != null)
				{
					sceneOptionsUI.gameObject.SetActive(value: false);
				}
				if (userPreferencesUI != null)
				{
					userPreferencesUI.gameObject.SetActive(value: false);
				}
				if (selectedController != null)
				{
					selectedController.guihidden = true;
				}
				if (multiButtonPanel != null)
				{
					multiButtonPanel.gameObject.SetActive(value: false);
				}
				if (useAltUI)
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: false);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: true);
					}
				}
				else
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: true);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: false);
					}
				}
				if (embeddedSceneUI != null)
				{
					embeddedSceneUI.gameObject.SetActive(value: false);
				}
				break;
			case ActiveUI.MainMenu:
				if (mainMenuUI != null)
				{
					mainMenuUI.gameObject.SetActive(value: true);
				}
				if (sceneOptionsUI != null)
				{
					sceneOptionsUI.gameObject.SetActive(value: false);
				}
				if (userPreferencesUI != null)
				{
					userPreferencesUI.gameObject.SetActive(value: false);
				}
				if (selectedController != null)
				{
					selectedController.guihidden = true;
				}
				if (multiButtonPanel != null)
				{
					multiButtonPanel.gameObject.SetActive(value: false);
				}
				if (useAltUI)
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: false);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: true);
					}
				}
				else
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: true);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: false);
					}
				}
				if (embeddedSceneUI != null)
				{
					embeddedSceneUI.gameObject.SetActive(value: false);
				}
				break;
			case ActiveUI.MainMenuOnly:
				if (mainMenuUI != null)
				{
					mainMenuUI.gameObject.SetActive(value: true);
				}
				if (sceneOptionsUI != null)
				{
					sceneOptionsUI.gameObject.SetActive(value: false);
				}
				if (userPreferencesUI != null)
				{
					userPreferencesUI.gameObject.SetActive(value: false);
				}
				if (selectedController != null)
				{
					selectedController.guihidden = true;
				}
				if (multiButtonPanel != null)
				{
					multiButtonPanel.gameObject.SetActive(value: false);
				}
				if (sceneControlUI != null)
				{
					sceneControlUI.gameObject.SetActive(value: false);
				}
				if (sceneControlUIAlt != null)
				{
					sceneControlUIAlt.gameObject.SetActive(value: false);
				}
				if (embeddedSceneUI != null)
				{
					embeddedSceneUI.gameObject.SetActive(value: false);
				}
				break;
			case ActiveUI.SceneOptions:
				if (mainMenuUI != null)
				{
					mainMenuUI.gameObject.SetActive(value: false);
				}
				if (sceneOptionsUI != null)
				{
					sceneOptionsUI.gameObject.SetActive(value: true);
				}
				if (userPreferencesUI != null)
				{
					userPreferencesUI.gameObject.SetActive(value: false);
				}
				if (selectedController != null)
				{
					selectedController.guihidden = true;
				}
				if (multiButtonPanel != null)
				{
					multiButtonPanel.gameObject.SetActive(value: false);
				}
				if (useAltUI)
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: false);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: true);
					}
				}
				else
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: true);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: false);
					}
				}
				if (embeddedSceneUI != null)
				{
					embeddedSceneUI.gameObject.SetActive(value: false);
				}
				break;
			case ActiveUI.UserPreferences:
				if (mainMenuUI != null)
				{
					mainMenuUI.gameObject.SetActive(value: false);
				}
				if (sceneOptionsUI != null)
				{
					sceneOptionsUI.gameObject.SetActive(value: false);
				}
				if (userPreferencesUI != null)
				{
					userPreferencesUI.gameObject.SetActive(value: true);
				}
				if (selectedController != null)
				{
					selectedController.guihidden = true;
				}
				if (multiButtonPanel != null)
				{
					multiButtonPanel.gameObject.SetActive(value: false);
				}
				if (useAltUI)
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: false);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: true);
					}
				}
				else
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: true);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: false);
					}
				}
				if (embeddedSceneUI != null)
				{
					embeddedSceneUI.gameObject.SetActive(value: false);
				}
				break;
			case ActiveUI.SelectedOptions:
				if (mainMenuUI != null)
				{
					mainMenuUI.gameObject.SetActive(value: false);
				}
				if (sceneOptionsUI != null)
				{
					sceneOptionsUI.gameObject.SetActive(value: false);
				}
				if (userPreferencesUI != null)
				{
					userPreferencesUI.gameObject.SetActive(value: false);
				}
				if (selectedController != null && _mainHUDVisible)
				{
					selectedController.guihidden = false;
				}
				if (multiButtonPanel != null)
				{
					multiButtonPanel.gameObject.SetActive(value: false);
				}
				if (useAltUI)
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: false);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: true);
					}
				}
				else
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: true);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: false);
					}
				}
				if (embeddedSceneUI != null)
				{
					embeddedSceneUI.gameObject.SetActive(value: false);
				}
				break;
			case ActiveUI.MultiButtonPanel:
				if (mainMenuUI != null)
				{
					mainMenuUI.gameObject.SetActive(value: false);
				}
				if (sceneOptionsUI != null)
				{
					sceneOptionsUI.gameObject.SetActive(value: false);
				}
				if (userPreferencesUI != null)
				{
					userPreferencesUI.gameObject.SetActive(value: false);
				}
				if (selectedController != null)
				{
					selectedController.guihidden = true;
				}
				if (multiButtonPanel != null)
				{
					multiButtonPanel.gameObject.SetActive(value: true);
				}
				if (sceneControlUI != null)
				{
					sceneControlUI.gameObject.SetActive(value: false);
				}
				if (sceneControlUIAlt != null)
				{
					sceneControlUIAlt.gameObject.SetActive(value: false);
				}
				if (embeddedSceneUI != null)
				{
					embeddedSceneUI.gameObject.SetActive(value: false);
				}
				break;
			case ActiveUI.EmbeddedScenePanel:
				if (mainMenuUI != null)
				{
					mainMenuUI.gameObject.SetActive(value: false);
				}
				if (sceneOptionsUI != null)
				{
					sceneOptionsUI.gameObject.SetActive(value: false);
				}
				if (userPreferencesUI != null)
				{
					userPreferencesUI.gameObject.SetActive(value: false);
				}
				if (selectedController != null)
				{
					selectedController.guihidden = true;
				}
				if (multiButtonPanel != null)
				{
					multiButtonPanel.gameObject.SetActive(value: false);
				}
				if (useAltUI)
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: false);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: true);
					}
				}
				else
				{
					if (sceneControlUI != null)
					{
						sceneControlUI.gameObject.SetActive(value: true);
					}
					if (sceneControlUIAlt != null)
					{
						sceneControlUIAlt.gameObject.SetActive(value: false);
					}
				}
				if (embeddedSceneUI != null)
				{
					embeddedSceneUI.gameObject.SetActive(value: true);
				}
				break;
			}
		}
	}

	public bool freezeAnimation => _freezeAnimation;

	public float worldScale
	{
		get
		{
			return _worldScale;
		}
		set
		{
			if (_worldScale != value)
			{
				_worldScale = value;
				if (worldScaleSlider != null)
				{
					worldScaleSlider.value = _worldScale;
				}
				Vector3 localScale = new Vector3(_worldScale, _worldScale, _worldScale);
				base.transform.localScale = localScale;
			}
		}
	}

	public float controllerScale
	{
		get
		{
			return _controllerScale;
		}
		set
		{
			if (_controllerScale != value)
			{
				_controllerScale = value;
				if (controllerScaleSlider != null)
				{
					controllerScaleSlider.value = _controllerScale;
				}
			}
		}
	}

	public SteamVR_Controller.Device viveControllerLeft
	{
		get
		{
			if (viveObjectLeft != null)
			{
				if (viveObjectLeft.index != SteamVR_TrackedObject.EIndex.None)
				{
					return SteamVR_Controller.Input((int)viveObjectLeft.index);
				}
				return null;
			}
			return null;
		}
	}

	public SteamVR_Controller.Device viveControllerRight
	{
		get
		{
			if (viveObjectRight != null)
			{
				if (viveObjectRight.index != SteamVR_TrackedObject.EIndex.None)
				{
					return SteamVR_Controller.Input((int)viveObjectRight.index);
				}
				return null;
			}
			return null;
		}
	}

	private Transform motionControllerLeft
	{
		get
		{
			if (OVRManager.isHmdPresent)
			{
				return touchObjectLeft;
			}
			return viveObjectLeft.transform;
		}
	}

	private Transform motionControllerRight
	{
		get
		{
			if (OVRManager.isHmdPresent)
			{
				return touchObjectRight;
			}
			return viveObjectRight.transform;
		}
	}

	public string[] forceReceiverNames => _forceReceiverNames;

	public string[] forceProducerNames => _forceProducerNames;

	public string[] freeControllerNames => _freeControllerNames;

	public string[] rigidbodyNames => _rigidbodyNames;

	public float environmentColliderHeight
	{
		get
		{
			return _environmentColliderHeight;
		}
		set
		{
			if (_environmentColliderHeight != value)
			{
				float adj = value - _environmentColliderHeight;
				HUDAnchor.AdjustAnchorHeights(adj);
				_environmentColliderHeight = value;
				if (environmentColliders != null)
				{
					Vector3 position = environmentColliders.position;
					position.y = _environmentColliderHeight;
					environmentColliders.position = position;
				}
				if (navigationRig != null)
				{
					Vector3 position2 = navigationRig.position;
					position2.y = _environmentColliderHeight;
					navigationRig.position = position2;
				}
				if (environmentColliderHeightSlider != null)
				{
					environmentColliderHeightSlider.value = _environmentColliderHeight;
				}
				if (environmentColliderHeightSliderAlt != null)
				{
					environmentColliderHeightSliderAlt.value = _environmentColliderHeight;
				}
			}
		}
	}

	private bool useInterpolation
	{
		get
		{
			return _useInterpolation;
		}
		set
		{
			if (_useInterpolation == value)
			{
				return;
			}
			_useInterpolation = value;
			foreach (Atom value2 in atoms.Values)
			{
				value2.useRigidbodyInterpolation = _useInterpolation;
			}
		}
	}

	public void StartScene()
	{
		if (startJSONEmbedScene != null)
		{
			LoadFromJSONEmbed(startJSONEmbedScene);
			return;
		}
		Load(savesDir + startSceneName);
		loadedName = string.Empty;
	}

	public void NewScene()
	{
		LoadForEdit(savesDir + newSceneName);
		loadedName = string.Empty;
	}

	public void ClearScene()
	{
		if (Application.isPlaying)
		{
			if (!isLoading)
			{
				loadMerge = false;
				onStartScene = false;
				gameMode = GameMode.Play;
				StartCoroutine(LoadCo(clearOnly: true));
			}
			else
			{
				Debug.LogWarning("Already loading file " + loadedName + ". Can't clear until complete");
			}
		}
	}

	protected void ProcessSave()
	{
		if (saveCount == 0 && screenshotCamera != null)
		{
			RenderTexture targetTexture = screenshotCamera.targetTexture;
			if (targetTexture != null)
			{
				Texture2D texture2D = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, mipmap: false);
				RenderTexture.active = targetTexture;
				texture2D.ReadPixels(new Rect(0f, 0f, targetTexture.width, targetTexture.height), 0, 0);
				texture2D.Apply();
				byte[] bytes = texture2D.EncodeToJPG();
				string text = savingName.Replace(".json", ".jpg");
				File.WriteAllBytes(text, bytes);
				if (fileBrowserUI != null)
				{
					fileBrowserUI.ClearCacheImage(text);
				}
			}
			screenshotCamera.enabled = false;
			saveCount--;
		}
		else if (saveCount > 0)
		{
			saveCount--;
		}
	}

	public void SaveSceneDialog()
	{
		if (multiButtonPanel != null)
		{
			multiButtonPanel.SetButton1Text("Save New");
			multiButtonPanel.showButton1 = true;
			if (loadedName != null && loadedName != string.Empty)
			{
				multiButtonPanel.SetButton2Text("Overwrite Current");
				multiButtonPanel.showButton2 = true;
			}
			else
			{
				multiButtonPanel.showButton2 = false;
			}
			multiButtonPanel.SetButton3Text("Cancel");
			multiButtonPanel.showButton3 = true;
			multiButtonPanel.buttonCallback = SaveConfirm;
			activeUI = ActiveUI.MultiButtonPanel;
		}
	}

	public void SaveConfirm(string option)
	{
		if (_lastActiveUI == ActiveUI.MultiButtonPanel)
		{
			activeUI = ActiveUI.None;
		}
		else
		{
			activeUI = _lastActiveUI;
		}
		multiButtonPanel.gameObject.SetActive(value: false);
		multiButtonPanel.buttonCallback = null;
		if (option == "Save New")
		{
			loadedName = string.Concat(str2: ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString(), str0: savesDir, str1: "scene/", str3: ".json");
			Save(loadedName);
		}
		else if (loadedName != null && loadedName != string.Empty && option == "Overwrite Current")
		{
			Save(loadedName);
		}
	}

	public void Save(string saveName = "Saves/scene/savefile.json", Atom specificAtom = null, bool includePhysical = true, bool includeAppearance = true)
	{
		Debug.Log("Save " + saveName);
		int num = saveName.LastIndexOf('/');
		if (num >= 0)
		{
			string path = saveName.Substring(0, num);
			Directory.CreateDirectory(path);
		}
		JSONClass jSONClass = new JSONClass();
		if (specificAtom == null)
		{
			if (worldScaleSlider != null)
			{
				SliderControl component = worldScaleSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != worldScale)
				{
					jSONClass["worldScale"].AsFloat = worldScale;
				}
			}
			if (environmentColliderHeightSlider != null)
			{
				SliderControl component2 = environmentColliderHeightSlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != _environmentColliderHeight)
				{
					jSONClass["environmentHeight"].AsFloat = _environmentColliderHeight;
				}
			}
			JSONArray jSONArray = (JSONArray)(jSONClass["atoms"] = new JSONArray());
			foreach (Atom value2 in atoms.Values)
			{
				if (value2.on)
				{
					value2.Store(jSONArray);
				}
			}
		}
		else
		{
			JSONArray jSONArray2 = (JSONArray)(jSONClass["atoms"] = new JSONArray());
			specificAtom.Store(jSONArray2, includePhysical, includeAppearance);
		}
		string value = jSONClass.ToString(string.Empty);
		StreamWriter streamWriter = new StreamWriter(saveName);
		streamWriter.Write(value);
		streamWriter.Close();
		savingName = saveName;
		saveCount = 2;
		if (screenshotCamera != null)
		{
			screenshotCamera.enabled = true;
			screenshotCamera.fieldOfView = 60f;
		}
	}

	public void LoadMergeSceneDialog()
	{
		fileBrowserUI.defaultPath = savesDir + "scene";
		loadMerge = true;
		activeUI = ActiveUI.None;
		fileBrowserUI.Show(Load);
	}

	public void LoadSceneDialog()
	{
		fileBrowserUI.defaultPath = savesDir + "scene";
		loadMerge = false;
		activeUI = ActiveUI.None;
		fileBrowserUI.Show(Load);
	}

	protected IEnumerator LoadCo(bool clearOnly = false)
	{
		JSONArray jatoms = loadJson["atoms"].AsArray;
		if (!loadMerge)
		{
			JSONClass jc = new JSONClass();
			ClearSelection();
			foreach (Atom value in atoms.Values)
			{
				value.PreRestore();
			}
			if (startingAtoms != null)
			{
				List<Atom> list = new List<Atom>();
				foreach (Atom value2 in atoms.Values)
				{
					if (!startingAtoms.ContainsKey(value2.uid))
					{
						list.Add(value2);
					}
					else
					{
						value2.on = true;
					}
				}
				foreach (Atom item in list)
				{
					RemoveAtom(item);
				}
			}
			yield return null;
			foreach (Atom value3 in atoms.Values)
			{
				value3.ClearParentAtom();
			}
			foreach (Atom value4 in atoms.Values)
			{
				value4.RestoreTransform(jc);
			}
			foreach (Atom value5 in atoms.Values)
			{
				value5.RestoreParentAtom(jc);
			}
			foreach (Atom value6 in atoms.Values)
			{
				value6.Restore(jc);
			}
			foreach (Atom value7 in atoms.Values)
			{
				value7.LateRestore(jc);
			}
			foreach (Atom value8 in atoms.Values)
			{
				value8.PostRestore();
			}
			if (worldScaleSlider != null)
			{
				SliderControl component = worldScaleSlider.GetComponent<SliderControl>();
				if (component != null)
				{
					worldScale = component.defaultValue;
				}
			}
			if (environmentColliderHeightSlider != null)
			{
				SliderControl component2 = environmentColliderHeightSlider.GetComponent<SliderControl>();
				if (component2 != null)
				{
					environmentColliderHeight = component2.defaultValue;
				}
			}
		}
		if (!clearOnly)
		{
			foreach (Atom value9 in atoms.Values)
			{
				value9.PreRestore();
			}
			IEnumerator enumerator11 = jatoms.GetEnumerator();
			try
			{
				while (enumerator11.MoveNext())
				{
					JSONClass jSONClass = (JSONClass)enumerator11.Current;
					string text = jSONClass["id"];
					string text2 = jSONClass["type"];
					Atom atomByUid = GetAtomByUid(text);
					if (atomByUid == null)
					{
						AddAtomByType(text2, text);
						atomByUid = GetAtomByUid(text);
					}
					else if (atomByUid.type != text2)
					{
						Debug.LogError("Atom " + atomByUid.name + " already exists, but uses different type " + atomByUid.type + " compared to requested " + text2);
					}
					if (atomByUid != null)
					{
						atomByUid.on = true;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				IDisposable disposable2 = (disposable = enumerator11 as IDisposable);
				if (disposable != null)
				{
					disposable2.Dispose();
				}
			}
			yield return null;
			IEnumerator enumerator12 = jatoms.GetEnumerator();
			try
			{
				while (enumerator12.MoveNext())
				{
					JSONClass jSONClass2 = (JSONClass)enumerator12.Current;
					string text3 = jSONClass2["id"];
					string text4 = jSONClass2["type"];
					Atom atomByUid2 = GetAtomByUid(text3);
					if (atomByUid2 != null)
					{
						atomByUid2.RestoreTransform(jSONClass2);
					}
					else
					{
						Debug.LogError("Failed to find atom " + text3 + " of type " + text4);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				IDisposable disposable3 = (disposable = enumerator12 as IDisposable);
				if (disposable != null)
				{
					disposable3.Dispose();
				}
			}
			IEnumerator enumerator13 = jatoms.GetEnumerator();
			try
			{
				while (enumerator13.MoveNext())
				{
					JSONClass jSONClass3 = (JSONClass)enumerator13.Current;
					string uid = jSONClass3["id"];
					Atom atomByUid3 = GetAtomByUid(uid);
					if (atomByUid3 != null)
					{
						atomByUid3.RestoreParentAtom(jSONClass3);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				IDisposable disposable4 = (disposable = enumerator13 as IDisposable);
				if (disposable != null)
				{
					disposable4.Dispose();
				}
			}
			IEnumerator enumerator14 = jatoms.GetEnumerator();
			try
			{
				while (enumerator14.MoveNext())
				{
					JSONClass jSONClass4 = (JSONClass)enumerator14.Current;
					string text5 = jSONClass4["id"];
					Atom atomByUid4 = GetAtomByUid(text5);
					if (atomByUid4 != null)
					{
						atomByUid4.Restore(jSONClass4);
					}
					else
					{
						Debug.LogError("Could not find atom by uid " + text5);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				IDisposable disposable5 = (disposable = enumerator14 as IDisposable);
				if (disposable != null)
				{
					disposable5.Dispose();
				}
			}
			IEnumerator enumerator15 = jatoms.GetEnumerator();
			try
			{
				while (enumerator15.MoveNext())
				{
					JSONClass jSONClass5 = (JSONClass)enumerator15.Current;
					string text6 = jSONClass5["id"];
					Atom atomByUid5 = GetAtomByUid(text6);
					if (atomByUid5 != null)
					{
						atomByUid5.LateRestore(jSONClass5);
					}
					else
					{
						Debug.LogError("Could not find atom by uid " + text6);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				IDisposable disposable6 = (disposable = enumerator15 as IDisposable);
				if (disposable != null)
				{
					disposable6.Dispose();
				}
			}
			foreach (Atom value10 in atoms.Values)
			{
				value10.PostRestore();
			}
			if (loadJson["worldScale"] != null)
			{
				worldScale = loadJson["worldScale"].AsFloat;
			}
			else if (worldScaleSlider != null)
			{
				SliderControl component3 = worldScaleSlider.GetComponent<SliderControl>();
				if (component3 != null)
				{
					worldScale = component3.defaultValue;
				}
			}
			if (loadJson["environmentHeight"] != null)
			{
				environmentColliderHeight = loadJson["environmentHeight"].AsFloat;
			}
			else if (environmentColliderHeightSlider != null)
			{
				SliderControl component4 = environmentColliderHeightSlider.GetComponent<SliderControl>();
				if (component4 != null)
				{
					environmentColliderHeight = component4.defaultValue;
				}
			}
		}
		activeUI = ActiveUI.None;
		if (!showMainHUDOnStart || disableUI)
		{
			HideMainHUD();
		}
		if (mainHUDAttachPoint != null)
		{
			mainHUDAttachPoint.localPosition = mainHUDAttachPointStartingPosition;
			mainHUDAttachPoint.localRotation = mainHUDAttachPointStartingRotation;
		}
		isLoading = false;
	}

	public void Load(string saveName = "savefile")
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (!isLoading)
		{
			if (saveName != null && saveName != string.Empty && File.Exists(saveName))
			{
				onStartScene = false;
				gameMode = GameMode.Play;
				Debug.Log("Load " + saveName);
				StreamReader streamReader = new StreamReader(saveName);
				string aJSON = streamReader.ReadToEnd();
				streamReader.Close();
				loadJson = JSON.Parse(aJSON);
				loadedName = saveName;
				isLoading = true;
				StartCoroutine(LoadCo());
			}
		}
		else
		{
			Debug.LogWarning("Already loading file " + loadedName + ". Can't load another until complete");
		}
	}

	public void LoadFromJSONEmbed(JSONEmbed je)
	{
		if (Application.isPlaying)
		{
			onStartScene = false;
			loadMerge = false;
			gameMode = GameMode.Play;
			loadJson = JSON.Parse(je.jsonStore);
			loadedName = je.name;
			isLoading = true;
			StartCoroutine(LoadCo());
		}
	}

	public void LoadForEdit(string saveName = "savefile")
	{
		Load(saveName);
		gameMode = GameMode.Edit;
	}

	public void Quit()
	{
		Application.Quit();
	}

	private void SetHelpOverlay(bool on)
	{
		if (helpToggle != null)
		{
			helpToggle.isOn = on;
		}
		if (helpToggleAlt != null)
		{
			helpToggleAlt.isOn = on;
		}
		if (on)
		{
			if (OVRManager.isHmdPresent)
			{
				if (helpOverlayOVR != null)
				{
					helpOverlayOVR.gameObject.SetActive(value: true);
				}
			}
			else if (helpOverlayVive != null)
			{
				helpOverlayVive.gameObject.SetActive(value: true);
			}
		}
		else
		{
			if (helpOverlayOVR != null)
			{
				helpOverlayOVR.gameObject.SetActive(value: false);
			}
			if (helpOverlayVive != null)
			{
				helpOverlayVive.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetActiveUI(string uiName)
	{
		try
		{
			activeUI = (ActiveUI)Enum.Parse(typeof(ActiveUI), uiName);
		}
		catch (ArgumentException)
		{
			Debug.LogError("Attempted to set UI to " + uiName + " which is not a valid UI name");
		}
	}

	private void InitUI()
	{
		if (mainHUDAttachPoint != null)
		{
			mainHUDAttachPointStartingPosition = mainHUDAttachPoint.localPosition;
			mainHUDAttachPointStartingRotation = mainHUDAttachPoint.localRotation;
		}
		activeUI = ActiveUI.SelectedOptions;
		if (selectionHUDTransform != null)
		{
			selectionHUD = selectionHUDTransform.GetComponent<SelectionHUD>();
			SetSelectionHUDHeader(selectionHUD, "Highlighted Controllers");
		}
		if (rightSelectionHUDTransform != null)
		{
			rightSelectionHUD = rightSelectionHUDTransform.GetComponent<SelectionHUD>();
			SetSelectionHUDHeader(rightSelectionHUD, string.Empty);
		}
		if (leftSelectionHUDTransform != null)
		{
			leftSelectionHUD = leftSelectionHUDTransform.GetComponent<SelectionHUD>();
			SetSelectionHUDHeader(leftSelectionHUD, string.Empty);
		}
		if (worldScaleSlider != null)
		{
			worldScaleSlider.value = _worldScale;
			worldScaleSlider.onValueChanged.AddListener(delegate
			{
				worldScale = worldScaleSlider.value;
			});
			SliderControl component = worldScaleSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				component.defaultValue = _worldScale;
			}
		}
		if (controllerScaleSlider != null)
		{
			controllerScaleSlider.value = _controllerScale;
			controllerScaleSlider.onValueChanged.AddListener(delegate
			{
				controllerScale = controllerScaleSlider.value;
			});
			SliderControl component2 = controllerScaleSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				component2.defaultValue = _controllerScale;
			}
		}
		if (environmentColliderHeightSlider != null)
		{
			environmentColliderHeightSlider.value = _environmentColliderHeight;
			environmentColliderHeightSlider.onValueChanged.AddListener(delegate
			{
				environmentColliderHeight = environmentColliderHeightSlider.value;
			});
			SliderControl component3 = environmentColliderHeightSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				component3.defaultValue = _environmentColliderHeight;
			}
		}
		if (environmentColliderHeightSliderAlt != null)
		{
			environmentColliderHeightSliderAlt.value = _environmentColliderHeight;
			environmentColliderHeightSliderAlt.onValueChanged.AddListener(delegate
			{
				environmentColliderHeight = environmentColliderHeightSliderAlt.value;
			});
			SliderControl component4 = environmentColliderHeightSliderAlt.GetComponent<SliderControl>();
			if (component4 != null)
			{
				component4.defaultValue = _environmentColliderHeight;
			}
		}
		if (editModeToggle != null)
		{
			editModeToggle.isOn = _gameMode == GameMode.Edit;
			editModeToggle.onValueChanged.AddListener(delegate
			{
				if (editModeToggle.isOn)
				{
					gameMode = GameMode.Edit;
				}
			});
		}
		if (playModeToggle != null)
		{
			playModeToggle.isOn = _gameMode == GameMode.Play;
			playModeToggle.onValueChanged.AddListener(delegate
			{
				if (playModeToggle.isOn)
				{
					gameMode = GameMode.Play;
				}
			});
		}
		if (selectedControllerNameDisplay != null)
		{
			selectedControllerNameDisplay.text = string.Empty;
		}
		if (rayLineMaterialLeft != null)
		{
			rayLineDrawerLeft = new LineDrawer(rayLineMaterialLeft);
		}
		if (rayLineMaterialRight != null)
		{
			rayLineDrawerRight = new LineDrawer(rayLineMaterialRight);
		}
		if (helpToggle != null)
		{
			helpToggle.onValueChanged.AddListener(delegate
			{
				SetHelpOverlay(helpToggle.isOn);
			});
		}
		if (helpToggleAlt != null)
		{
			helpToggleAlt.onValueChanged.AddListener(delegate
			{
				SetHelpOverlay(helpToggleAlt.isOn);
			});
		}
		if (helpOnAtStart)
		{
			SetHelpOverlay(on: true);
		}
		if (showMainHUDOnStart && !disableUI)
		{
			ShowMainHUD();
		}
		else
		{
			HideMainHUD();
		}
	}

	public void ClearSelection()
	{
		if (selectedController != null)
		{
			if (selectedControllerNameDisplay != null)
			{
				selectedControllerNameDisplay.text = string.Empty;
			}
			selectedController.selected = false;
			selectedController.hidden = true;
			selectedController.guihidden = true;
			selectedController = null;
			_navigationForwardAxisEnabled = navigationForwardAxisEnabled;
			_navigationSideAxisEnabled = navigationSideAxisEnabled;
			_navigationTurnAxisEnabled = navigationTurnAxisEnabled;
			_navigationUpAxisEnabled = navigationUpAxisEnabled;
		}
		if (LookInputModule.singleton != null)
		{
			LookInputModule.singleton.ClearSelection();
		}
		SyncVisibility();
	}

	private void SyncVisibility()
	{
		switch (selectMode)
		{
		case SelectMode.Targets:
			foreach (FreeControllerV3 allController in allControllers)
			{
				if (gameMode == GameMode.Edit || allController.interactableInPlayMode)
				{
					allController.hidden = false;
				}
				else
				{
					allController.hidden = true;
				}
			}
			break;
		case SelectMode.FilteredTargets:
			foreach (FreeControllerV3 allController2 in allControllers)
			{
				if (selectedController == null || selectedController != allController2)
				{
					allController2.hidden = true;
				}
				else if (gameMode == GameMode.Edit || allController2.interactableInPlayMode)
				{
					allController2.hidden = false;
				}
				else
				{
					allController2.hidden = true;
				}
			}
			break;
		default:
			foreach (FreeControllerV3 allController3 in allControllers)
			{
				allController3.hidden = true;
			}
			break;
		}
		Atom atom = null;
		if (selectedController != null)
		{
			atom = selectedController.containingAtom;
		}
		if (selectMode == SelectMode.FilteredTargets && atom != null)
		{
			FreeControllerV3[] freeControllers = atom.freeControllers;
			foreach (FreeControllerV3 freeControllerV in freeControllers)
			{
				if (gameMode == GameMode.Edit || freeControllerV.interactableInPlayMode)
				{
					freeControllerV.hidden = false;
				}
				else
				{
					freeControllerV.hidden = true;
				}
			}
		}
		foreach (ForceProducerV2 value in fpMap.Values)
		{
			value.drawLines = false;
			if (atom != null && atom == value.containingAtom && (selectMode == SelectMode.Targets || selectMode == SelectMode.FilteredTargets))
			{
				value.drawLines = true;
			}
		}
		if (allAnimationPatterns != null)
		{
			foreach (AnimationPattern allAnimationPattern in allAnimationPatterns)
			{
				allAnimationPattern.draw = selectMode == SelectMode.Targets;
				if (!(atom != null) || !(atom == allAnimationPattern.containingAtom) || (selectMode != SelectMode.Targets && selectMode != SelectMode.FilteredTargets))
				{
					continue;
				}
				allAnimationPattern.draw = true;
				AnimationStep[] steps = allAnimationPattern.steps;
				foreach (AnimationStep animationStep in steps)
				{
					if (!(animationStep.containingAtom != null) || animationStep.containingAtom.freeControllers == null)
					{
						continue;
					}
					FreeControllerV3[] freeControllers2 = animationStep.containingAtom.freeControllers;
					foreach (FreeControllerV3 freeControllerV2 in freeControllers2)
					{
						if (gameMode == GameMode.Edit || freeControllerV2.interactableInPlayMode)
						{
							freeControllerV2.hidden = false;
						}
						else
						{
							freeControllerV2.hidden = true;
						}
					}
				}
			}
		}
		if (allAnimationSteps == null)
		{
			return;
		}
		foreach (AnimationStep allAnimationStep in allAnimationSteps)
		{
			if (!(allAnimationStep.animationParent != null) || !(atom != null) || !(atom == allAnimationStep.containingAtom) || (selectMode != SelectMode.Targets && selectMode != SelectMode.FilteredTargets))
			{
				continue;
			}
			allAnimationStep.animationParent.draw = true;
			if (allAnimationStep.animationParent.containingAtom != null && allAnimationStep.animationParent.containingAtom.freeControllers != null)
			{
				FreeControllerV3[] freeControllers3 = allAnimationStep.animationParent.containingAtom.freeControllers;
				foreach (FreeControllerV3 freeControllerV3 in freeControllers3)
				{
					if (gameMode == GameMode.Edit || freeControllerV3.interactableInPlayMode)
					{
						freeControllerV3.hidden = false;
					}
					else
					{
						freeControllerV3.hidden = true;
					}
				}
			}
			AnimationStep[] steps2 = allAnimationStep.animationParent.steps;
			foreach (AnimationStep animationStep2 in steps2)
			{
				if (!(animationStep2.containingAtom != null) || animationStep2.containingAtom.freeControllers == null)
				{
					continue;
				}
				FreeControllerV3[] freeControllers4 = animationStep2.containingAtom.freeControllers;
				foreach (FreeControllerV3 freeControllerV4 in freeControllers4)
				{
					if (gameMode == GameMode.Edit || freeControllerV4.interactableInPlayMode)
					{
						freeControllerV4.hidden = false;
					}
					else
					{
						freeControllerV4.hidden = true;
					}
				}
			}
		}
	}

	protected void SetSelectionHUDHeader(SelectionHUD sh, string txt)
	{
		if (sh != null && sh.headerText != null)
		{
			sh.headerText.text = txt;
		}
	}

	private void ResetSelectionInstances()
	{
		if (selectionInstances != null)
		{
			foreach (Transform selectionInstance in selectionInstances)
			{
				UnityEngine.Object.Destroy(selectionInstance.gameObject);
			}
		}
		selectionInstances = new List<Transform>();
		highlightedSelectTargetsLook = null;
		highlightedSelectTargetsLeft = null;
		highlightedSelectTargetsRight = null;
	}

	public void SelectModeControllers(SelectControllerCallback scc)
	{
		if (selectMode != SelectMode.Controller)
		{
			ResetSelectionInstances();
			ClearSelection();
			selectMode = SelectMode.Controller;
			if (selectPrefab != null)
			{
				foreach (FreeControllerV3 allController in allControllers)
				{
					Transform transform = UnityEngine.Object.Instantiate(selectPrefab);
					SelectTarget component = transform.GetComponent<SelectTarget>();
					if (component != null)
					{
						component.selectionName = allController.containingAtom.uid + ":" + allController.name;
					}
					transform.parent = allController.transform;
					transform.position = allController.transform.position;
					selectionInstances.Add(transform);
				}
			}
			SetSelectionHUDHeader(selectionHUD, "Select Controller");
			if (selectionHUD != null)
			{
				selectionHUD.gameObject.SetActive(value: true);
			}
			selectControllerCallback = scc;
		}
		SyncVisibility();
	}

	public void SelectModeForceProducers(SelectForceProducerCallback sfpc)
	{
		if (selectMode != SelectMode.ForceProducer)
		{
			ResetSelectionInstances();
			ClearSelection();
			selectMode = SelectMode.ForceProducer;
			if (selectPrefab != null)
			{
				foreach (string key in fpMap.Keys)
				{
					ForceProducerV2 forceProducerV = ProducerNameToForceProducer(key);
					if (forceProducerV != null)
					{
						Transform transform = UnityEngine.Object.Instantiate(selectPrefab);
						SelectTarget component = transform.GetComponent<SelectTarget>();
						if (component != null)
						{
							component.selectionName = key;
						}
						transform.parent = forceProducerV.transform;
						transform.position = forceProducerV.transform.position;
						selectionInstances.Add(transform);
					}
				}
			}
			SetSelectionHUDHeader(selectionHUD, "Select Force Producer");
			if (selectionHUD != null)
			{
				selectionHUD.gameObject.SetActive(value: true);
			}
			selectForceProducerCallback = sfpc;
		}
		SyncVisibility();
	}

	public void SelectModeForceReceivers(SelectForceReceiverCallback sfrc)
	{
		if (selectMode != SelectMode.ForceReceiver)
		{
			ResetSelectionInstances();
			ClearSelection();
			selectMode = SelectMode.ForceReceiver;
			if (selectPrefab != null)
			{
				foreach (string key in frMap.Keys)
				{
					ForceReceiver forceReceiver = ReceiverNameToForceReceiver(key);
					if (forceReceiver != null && !forceReceiver.skipUIDrawing)
					{
						Transform transform = UnityEngine.Object.Instantiate(selectPrefab);
						SelectTarget component = transform.GetComponent<SelectTarget>();
						if (component != null)
						{
							component.selectionName = key;
						}
						transform.parent = forceReceiver.transform;
						transform.position = forceReceiver.transform.position;
						selectionInstances.Add(transform);
					}
				}
			}
			SetSelectionHUDHeader(selectionHUD, "Select Force Receiver");
			if (selectionHUD != null)
			{
				selectionHUD.gameObject.SetActive(value: true);
			}
			selectForceReceiverCallback = sfrc;
		}
		SyncVisibility();
	}

	public void SelectModeRigidbody(SelectRigidbodyCallback srbc)
	{
		if (selectMode != SelectMode.Rigidbody)
		{
			ResetSelectionInstances();
			ClearSelection();
			selectMode = SelectMode.Rigidbody;
			if (selectPrefab != null)
			{
				foreach (string key in rbMap.Keys)
				{
					Rigidbody rigidbody = RigidbodyNameToRigidbody(key);
					if (!(rigidbody != null))
					{
						continue;
					}
					ForceReceiver component = rigidbody.GetComponent<ForceReceiver>();
					if (component == null || !component.skipUIDrawing)
					{
						Transform transform = UnityEngine.Object.Instantiate(selectPrefab);
						SelectTarget component2 = transform.GetComponent<SelectTarget>();
						if (component2 != null)
						{
							component2.selectionName = key;
						}
						transform.parent = rigidbody.transform;
						transform.position = rigidbody.transform.position;
						selectionInstances.Add(transform);
					}
				}
			}
			SetSelectionHUDHeader(selectionHUD, "Select Physics Object");
			if (selectionHUD != null)
			{
				selectionHUD.gameObject.SetActive(value: true);
			}
			selectRigidbodyCallback = srbc;
		}
		SyncVisibility();
	}

	public void SelectModeAtom(SelectAtomCallback sac)
	{
		if (selectMode != SelectMode.Atom)
		{
			ResetSelectionInstances();
			ClearSelection();
			selectMode = SelectMode.Atom;
			if (selectPrefab != null)
			{
				foreach (string atomUID in atomUIDs)
				{
					Atom atomByUid = GetAtomByUid(atomUID);
					if (atomByUid != null)
					{
						Transform transform = UnityEngine.Object.Instantiate(selectPrefab);
						SelectTarget component = transform.GetComponent<SelectTarget>();
						if (component != null)
						{
							component.selectionName = atomUID;
						}
						if (atomByUid.childAtomContainer != null)
						{
							transform.parent = atomByUid.childAtomContainer;
							transform.position = atomByUid.childAtomContainer.position;
						}
						else
						{
							transform.parent = atomByUid.transform;
							transform.position = atomByUid.transform.position;
						}
						selectionInstances.Add(transform);
					}
				}
			}
			SetSelectionHUDHeader(selectionHUD, "Select Atom");
			if (selectionHUD != null)
			{
				selectionHUD.gameObject.SetActive(value: true);
			}
			selectAtomCallback = sac;
		}
		SyncVisibility();
	}

	private void SelectModeOff()
	{
		if (selectMode != 0)
		{
			ResetSelectionInstances();
			selectMode = SelectMode.Off;
			SetSelectionHUDHeader(selectionHUD, "Highlighted Controllers");
			selectControllerCallback = null;
			selectForceProducerCallback = null;
			selectForceReceiverCallback = null;
			selectRigidbodyCallback = null;
			selectAtomCallback = null;
			if (selectionHUD != null)
			{
				selectionHUD.gameObject.SetActive(value: false);
			}
		}
		SyncVisibility();
	}

	private void SelectModeTargets()
	{
		if (selectMode != SelectMode.Targets)
		{
			ResetSelectionInstances();
			selectMode = SelectMode.Targets;
			SetSelectionHUDHeader(selectionHUD, "Highlighted Controllers");
			selectControllerCallback = null;
			selectForceProducerCallback = null;
			selectForceReceiverCallback = null;
			selectRigidbodyCallback = null;
			selectAtomCallback = null;
			if (selectionHUD != null)
			{
				selectionHUD.gameObject.SetActive(value: false);
			}
		}
		SyncVisibility();
	}

	private void SelectModeFiltered()
	{
		if (selectMode != SelectMode.FilteredTargets)
		{
			ResetSelectionInstances();
			selectMode = SelectMode.FilteredTargets;
			SetSelectionHUDHeader(selectionHUD, "Highlighted Controllers");
			selectControllerCallback = null;
			selectForceProducerCallback = null;
			selectForceReceiverCallback = null;
			selectRigidbodyCallback = null;
			selectAtomCallback = null;
			if (selectionHUD != null)
			{
				selectionHUD.gameObject.SetActive(value: false);
			}
		}
		SyncVisibility();
	}

	public void SetFreezeAnimation(bool freeze)
	{
		if (freeze)
		{
			FreezeAnimation();
		}
		else
		{
			UnfreezeAnimation();
		}
	}

	public void FreezeAnimation()
	{
		_freezeAnimation = true;
		if (allAnimators == null)
		{
			return;
		}
		foreach (Animator allAnimator in allAnimators)
		{
			allAnimator.enabled = false;
		}
	}

	public void UnfreezeAnimation()
	{
		_freezeAnimation = false;
		if (allAnimators == null)
		{
			return;
		}
		foreach (Animator allAnimator in allAnimators)
		{
			allAnimator.enabled = true;
		}
	}

	public void ShowMainHUD(bool setAnchors = true)
	{
		_mainHUDVisible = true;
		if (mainHUD != null)
		{
			mainHUD.gameObject.SetActive(value: true);
		}
		activeUI = _activeUI;
		if (setAnchors)
		{
			HUDAnchor.SetAnchorsToReference();
		}
	}

	public void HideMainHUD()
	{
		_mainHUDVisible = false;
		if (mainHUD != null)
		{
			mainHUD.gameObject.SetActive(value: false);
		}
		if (selectedController != null)
		{
			selectedController.guihidden = true;
		}
	}

	public void MoveMainHUD(Transform t)
	{
		if (mainHUDAttachPoint != null && t != null)
		{
			mainHUDAttachPoint.position = t.position;
			mainHUDAttachPoint.rotation = t.rotation;
		}
	}

	public void ToggleMainHUD()
	{
		if (mainHUD != null)
		{
			if (_mainHUDVisible)
			{
				HideMainHUD();
			}
			else
			{
				ShowMainHUD();
			}
		}
	}

	public void ToggleRotationMode()
	{
		if (selectedController != null)
		{
			selectedController.NextControlMode();
		}
	}

	public void TogglePointerModeLeft()
	{
		_pointerModeLeft = !_pointerModeLeft;
	}

	public void TogglePointerModeRight()
	{
		_pointerModeRight = !_pointerModeRight;
	}

	private void UnhighlightControllers(List<FreeControllerV3> highlightList)
	{
		foreach (FreeControllerV3 highlight in highlightList)
		{
			highlight.highlighted = false;
		}
		highlightList.Clear();
	}

	private void InitTargets()
	{
		if (selectionHUD != null)
		{
			selectionHUD.gameObject.SetActive(value: false);
		}
		SyncVisibility();
	}

	private void PrepControllers()
	{
		foreach (FreeControllerV3 allController in allControllers)
		{
			allController.ResetAppliedForces();
		}
	}

	private bool GetMenuShow()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Touch);
		}
		if (viveControllerLeft != null)
		{
			return viveControllerLeft.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu);
		}
		return false;
	}

	private bool GetMenuMove()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.Get(OVRInput.Button.Start, OVRInput.Controller.Touch);
		}
		if (viveControllerLeft != null)
		{
			return viveControllerLeft.GetPress(EVRButtonId.k_EButton_ApplicationMenu);
		}
		return false;
	}

	private bool GetNavigateShow()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.Touch);
		}
		if (viveControllerRight != null)
		{
			return viveControllerRight.GetPress(EVRButtonId.k_EButton_ApplicationMenu);
		}
		return false;
	}

	private bool GetNavigateStart()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Touch.Two, OVRInput.Controller.Touch);
		}
		if (viveControllerRight != null)
		{
			return viveControllerRight.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu);
		}
		return false;
	}

	private bool GetNavigateFinish()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Touch);
		}
		if (viveControllerRight != null)
		{
			return viveControllerRight.GetPressUp(EVRButtonId.k_EButton_ApplicationMenu);
		}
		return false;
	}

	private void HideLeftController()
	{
		if (OVRManager.isHmdPresent)
		{
			if (touchObjectLeft != null)
			{
				MeshRenderer[] componentsInChildren = touchObjectLeft.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				MeshRenderer[] array = componentsInChildren;
				foreach (MeshRenderer meshRenderer in array)
				{
					meshRenderer.enabled = false;
				}
			}
		}
		else
		{
			ViveTrackedControllers.singleton.HideLeftController();
		}
	}

	private void HideRightController()
	{
		if (OVRManager.isHmdPresent)
		{
			if (touchObjectRight != null)
			{
				MeshRenderer[] componentsInChildren = touchObjectRight.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				MeshRenderer[] array = componentsInChildren;
				foreach (MeshRenderer meshRenderer in array)
				{
					meshRenderer.enabled = false;
				}
			}
		}
		else
		{
			ViveTrackedControllers.singleton.HideRightController();
		}
	}

	private void ShowLeftController()
	{
		if (OVRManager.isHmdPresent)
		{
			if (touchObjectLeft != null)
			{
				MeshRenderer[] componentsInChildren = touchObjectLeft.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				MeshRenderer[] array = componentsInChildren;
				foreach (MeshRenderer meshRenderer in array)
				{
					meshRenderer.enabled = true;
				}
			}
		}
		else
		{
			ViveTrackedControllers.singleton.ShowLeftController();
		}
	}

	private void ShowRightController()
	{
		if (OVRManager.isHmdPresent)
		{
			if (touchObjectRight != null)
			{
				MeshRenderer[] componentsInChildren = touchObjectRight.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				MeshRenderer[] array = componentsInChildren;
				foreach (MeshRenderer meshRenderer in array)
				{
					meshRenderer.enabled = true;
				}
			}
		}
		else
		{
			ViveTrackedControllers.singleton.ShowRightController();
		}
	}

	private void ProcessGUIInteract()
	{
		if (OVRManager.isHmdPresent)
		{
			bool down = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Touch);
			if (GUIhitRight && down)
			{
				rightGUIInteract = true;
			}
			bool down2 = OVRInput.GetDown(OVRInput.Button.Three, OVRInput.Controller.Touch);
			if (GUIhitLeft && down2)
			{
				leftGUIInteract = true;
			}
			if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.Touch) && rightGUIInteract)
			{
				rightGUIInteract = false;
			}
			if (OVRInput.GetUp(OVRInput.Button.Three, OVRInput.Controller.Touch) && leftGUIInteract)
			{
				leftGUIInteract = false;
			}
		}
		else
		{
			bool rightTouchedThisFrame = ViveTrackedControllers.singleton.rightTouchedThisFrame;
			if (GUIhitRight && rightTouchedThisFrame)
			{
				rightGUIInteract = true;
			}
			bool leftTouchedThisFrame = ViveTrackedControllers.singleton.leftTouchedThisFrame;
			if (GUIhitLeft && leftTouchedThisFrame)
			{
				leftGUIInteract = true;
			}
			if (ViveTrackedControllers.singleton.rightUntouchedThisFrame && rightGUIInteract)
			{
				rightGUIInteract = false;
			}
			if (ViveTrackedControllers.singleton.leftUntouchedThisFrame && leftGUIInteract)
			{
				leftGUIInteract = false;
			}
		}
	}

	private bool GetTargetShow()
	{
		if (OVRManager.isHmdPresent)
		{
			bool flag = !rightGUIInteract && OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.Touch);
			bool flag2 = !leftGUIInteract && OVRInput.Get(OVRInput.Touch.Three, OVRInput.Controller.Touch);
			return flag || flag2;
		}
		bool flag3 = !rightGUIInteract && ViveTrackedControllers.singleton.rightTouching;
		bool flag4 = !leftGUIInteract && ViveTrackedControllers.singleton.leftTouching;
		return flag3 || flag4;
	}

	private bool GetLeftSelect()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Button.Three, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton.leftTouchpadPressedThisFrame;
	}

	private bool GetRightSelect()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton.rightTouchpadPressedThisFrame;
	}

	private float GetLeftTriggerVal()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton.leftTriggerValue;
	}

	private float GetRightTriggerVal()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton.rightTriggerValue;
	}

	private bool GetLeftQuickGrab()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton.leftTriggerFullClickThisFrame;
	}

	private bool GetRightQuickGrab()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton.rightTriggerFullClickThisFrame;
	}

	private bool GetLeftQuickRelease()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton.leftTriggerFullUnclickThisFrame;
	}

	private bool GetRightQuickRelease()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton.rightTriggerFullUnclickThisFrame;
	}

	private bool GetLeftGrabToggle()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton.leftGrippedThisFrame;
	}

	private bool GetRightGrabToggle()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton.rightGrippedThisFrame;
	}

	private void ProcessTargetSelectionDoRaycast(SelectionHUD sh, Ray ray, List<FreeControllerV3> hitsList, bool doHighlight = true)
	{
		RaycastHit[] array = Physics.RaycastAll(ray, 50f, targetColliderMask);
		if (array != null && array.Length > 0)
		{
			Dictionary<FreeControllerV3, bool> dictionary = new Dictionary<FreeControllerV3, bool>();
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				FreeControllerV3 freeControllerV = raycastHit.transform.GetComponent<FreeControllerV3>();
				if (freeControllerV == null)
				{
					FreeControllerV3Link component = raycastHit.transform.GetComponent<FreeControllerV3Link>();
					if (component != null)
					{
						freeControllerV = component.linkedController;
					}
				}
				if (freeControllerV != null && !freeControllerV.hidden && !dictionary.ContainsKey(freeControllerV) && (gameMode == GameMode.Edit || freeControllerV.interactableInPlayMode))
				{
					dictionary.Add(freeControllerV, value: true);
					if (!hitsList.Contains(freeControllerV))
					{
						hitsList.Add(freeControllerV);
					}
				}
			}
			FreeControllerV3[] array3 = hitsList.ToArray();
			FreeControllerV3[] array4 = array3;
			foreach (FreeControllerV3 freeControllerV2 in array4)
			{
				if (!dictionary.ContainsKey(freeControllerV2))
				{
					freeControllerV2.highlighted = false;
					hitsList.Remove(freeControllerV2);
				}
			}
			if (doHighlight)
			{
				for (int k = 0; k < hitsList.Count; k++)
				{
					FreeControllerV3 freeControllerV3 = hitsList[k];
					if (k == 0)
					{
						freeControllerV3.highlighted = true;
					}
					else
					{
						freeControllerV3.highlighted = false;
					}
				}
			}
			if (sh != null)
			{
				sh.ClearSelections();
				if (hitsList.Count > 0)
				{
					int num = 0;
					foreach (FreeControllerV3 hits in hitsList)
					{
						string text = hits.containingAtom.uid + ":" + hits.name;
						sh.SetSelection(num, hits.transform, text);
						num++;
					}
				}
			}
		}
		else
		{
			if (doHighlight)
			{
				foreach (FreeControllerV3 hits2 in hitsList)
				{
					hits2.highlighted = false;
				}
			}
			hitsList.Clear();
		}
		if (sh != null)
		{
			if (hitsList.Count > 0)
			{
				sh.gameObject.SetActive(value: true);
				sh.transform.position = hitsList[0].transform.position;
				Vector3 localScale = default(Vector3);
				localScale.z = (localScale.y = (localScale.x = (sh.transform.position - lookCamera.transform.position).magnitude));
				sh.transform.localScale = localScale;
				sh.transform.LookAt(lookCamera.transform.position, lookCamera.transform.up);
			}
			else
			{
				sh.gameObject.SetActive(value: false);
			}
		}
	}

	private void ProcessTargetSelectionDoSelect(List<FreeControllerV3> highlightedControllers)
	{
		if (highlightedControllers.Count > 0)
		{
			FreeControllerV3 freeControllerV = highlightedControllers[0];
			highlightedControllers.RemoveAt(0);
			highlightedControllers.Add(freeControllerV);
			if (selectedController != null && selectedController == freeControllerV)
			{
				ClearSelection();
				return;
			}
			if (selectedController != null)
			{
				ClearSelection();
			}
			if (navigationForwardAxis == axis1 || navigationForwardAxis == axis2 || navigationForwardAxis == axis3)
			{
				_navigationForwardAxisEnabled = false;
			}
			if (navigationSideAxis == axis1 || navigationSideAxis == axis2 || navigationSideAxis == axis3)
			{
				_navigationSideAxisEnabled = false;
			}
			if (navigationTurnAxis == axis1 || navigationTurnAxis == axis2 || navigationTurnAxis == axis3)
			{
				_navigationTurnAxisEnabled = false;
			}
			if (navigationUpAxis == axis1 || navigationUpAxis == axis2 || navigationUpAxis == axis3)
			{
				_navigationUpAxisEnabled = false;
			}
			freeControllerV.selected = true;
			selectedController = freeControllerV;
			if (selectedController != null && selectedControllerNameDisplay != null)
			{
				if (selectedController.containingAtom != null)
				{
					selectedControllerNameDisplay.text = selectedController.containingAtom.name + ":" + selectedController.name;
				}
				else
				{
					selectedControllerNameDisplay.text = selectedController.name;
				}
			}
			activeUI = ActiveUI.SelectedOptions;
			if (selectedController != null)
			{
				ShowMainHUD();
			}
		}
		else
		{
			ClearSelection();
		}
	}

	private void ProcessTargetSelectionCycleSelect(List<FreeControllerV3> highlightedControllers)
	{
		FreeControllerV3 item = highlightedControllers[0];
		highlightedControllers.RemoveAt(0);
		highlightedControllers.Add(item);
	}

	private void ProcessTargetSelectionCycleBackwardsSelect(List<FreeControllerV3> highlightedControllers)
	{
		int index = highlightedControllers.Count - 1;
		FreeControllerV3 item = highlightedControllers[index];
		highlightedControllers.RemoveAt(index);
		highlightedControllers.Insert(0, item);
	}

	private void ProcessControllerTargetHighlight(SelectionHUD sh, Transform processFrom, bool ptrMode, LineDrawer rayLineDrawer, List<FreeControllerV3> highlightedControllers, bool uihit, FreeControllerV3 excludeController)
	{
		List<FreeControllerV3> list = new List<FreeControllerV3>();
		if (!uihit)
		{
			Collider[] array = Physics.OverlapSphere(processFrom.position, 0.01f * _worldScale, targetColliderMask);
			if (array.Length > 0)
			{
				bool flag = false;
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					FreeControllerV3 component = collider.GetComponent<FreeControllerV3>();
					if (component != null && (gameMode == GameMode.Edit || component.interactableInPlayMode) && (!(excludeController != null) || !(component == excludeController)))
					{
						flag = true;
						if (!list.Contains(component))
						{
							list.Add(component);
						}
						float num = (component.distanceHolder = Vector3.SqrMagnitude(processFrom.position - collider.transform.position));
					}
				}
				if (!flag)
				{
					Collider[] array3 = array;
					foreach (Collider collider2 in array3)
					{
						FreeControllerV3 freeControllerV = null;
						FreeControllerV3Link component2 = collider2.GetComponent<FreeControllerV3Link>();
						if (component2 != null)
						{
							freeControllerV = component2.linkedController;
						}
						if (freeControllerV != null && (gameMode == GameMode.Edit || freeControllerV.interactableInPlayMode) && (!(excludeController != null) || !(freeControllerV == excludeController)))
						{
							if (!list.Contains(freeControllerV))
							{
								list.Add(freeControllerV);
							}
							float num2 = (freeControllerV.distanceHolder = Vector3.SqrMagnitude(processFrom.position - collider2.transform.position));
						}
					}
				}
			}
		}
		if (sh != null)
		{
			sh.ClearSelections();
			if (list.Count > 0)
			{
				highlightedControllers.Clear();
				List<FreeControllerV3> list2 = new List<FreeControllerV3>();
				list.Sort((FreeControllerV3 c1, FreeControllerV3 c2) => c1.distanceHolder.CompareTo(c2.distanceHolder));
				sh.gameObject.SetActive(value: true);
				sh.useDrawFromPosition = true;
				sh.drawFrom = processFrom.position;
				int num3 = 0;
				foreach (FreeControllerV3 item in list)
				{
					if (!list2.Contains(item))
					{
						if (num3 == 0)
						{
							highlightedControllers.Add(item);
						}
						string text = item.containingAtom.uid + ":" + item.name;
						sh.SetSelection(num3, item.transform, text);
						num3++;
						list2.Add(item);
					}
				}
				sh.transform.position = processFrom.position;
				Vector3 localScale = default(Vector3);
				localScale.z = (localScale.y = (localScale.x = (sh.transform.position - lookCamera.transform.position).magnitude));
				sh.transform.localScale = localScale;
				sh.transform.LookAt(lookCamera.transform.position, lookCamera.transform.up);
			}
			else if (!ptrMode)
			{
				highlightedControllers.Clear();
				sh.gameObject.SetActive(value: false);
			}
		}
		if (list.Count == 0 && ptrMode)
		{
			castRay.origin = processFrom.position;
			castRay.direction = processFrom.forward;
			if (rayLineDrawer != null)
			{
				rayLineDrawer.SetLinePoints(processFrom.position, processFrom.position + 50f * processFrom.forward);
				rayLineDrawer.Draw(base.gameObject.layer);
			}
			sh.useDrawFromPosition = true;
			sh.drawFrom = processFrom.position;
			if (!uihit)
			{
				ProcessTargetSelectionDoRaycast(sh, castRay, highlightedControllers, doHighlight: false);
			}
		}
	}

	private void ProcessTargetSelectionDoGrabRight(Transform rightControl)
	{
		if ((bool)rightGrabbedController)
		{
			rightGrabbedController.SelectLinkToRigidbody(null);
			rightGrabbedController = null;
		}
		if (highlightedControllersRight.Count <= 0)
		{
			return;
		}
		Rigidbody component = rightControl.GetComponent<Rigidbody>();
		FreeControllerV3 item = highlightedControllersRight[0];
		highlightedControllersRight.RemoveAt(0);
		highlightedControllersRight.Add(item);
		rightGrabbedController = item;
		if (leftFullGrabbedController == rightGrabbedController)
		{
			leftFullGrabbedController = null;
			leftHandControl = null;
		}
		if (leftGrabbedController == rightGrabbedController)
		{
			leftGrabbedController = null;
		}
		if (!(component != null))
		{
			return;
		}
		if (rightFullGrabbedController != null)
		{
			Rigidbody followWhenOffRB = rightFullGrabbedController.followWhenOffRB;
			if (followWhenOffRB != null)
			{
				rightGrabbedController.SelectLinkToRigidbody(followWhenOffRB, FreeControllerV3.SelectLinkState.PositionAndRotation, usePhysicalLink: true);
			}
			else
			{
				rightGrabbedController.SelectLinkToRigidbody(component);
			}
		}
		else
		{
			rightGrabbedController.SelectLinkToRigidbody(component);
		}
	}

	private void ProcessTargetSelectionDoFullGrabRight(Transform rightControl)
	{
		if (highlightedControllersRight.Count > 0)
		{
			Rigidbody component = rightControl.GetComponent<Rigidbody>();
			FreeControllerV3 item = highlightedControllersRight[0];
			highlightedControllersRight.RemoveAt(0);
			highlightedControllersRight.Add(item);
			rightFullGrabbedController = item;
			if (leftFullGrabbedController == rightFullGrabbedController)
			{
				leftFullGrabbedController = null;
				leftHandControl = null;
			}
			if (rightGrabbedController == rightFullGrabbedController)
			{
				rightGrabbedController = null;
			}
			if (component != null)
			{
				rightFullGrabbedController.SelectLinkToRigidbody(component);
			}
			rightHandControl = rightFullGrabbedController.GetComponent<HandControl>();
		}
		else if (rightGrabbedController != null)
		{
			rightFullGrabbedController = rightGrabbedController;
			rightHandControl = rightFullGrabbedController.GetComponent<HandControl>();
			rightGrabbedController = null;
		}
	}

	private void ProcessTargetSelectionDoGrabLeft(Transform leftControl)
	{
		if ((bool)leftGrabbedController)
		{
			leftGrabbedController.SelectLinkToRigidbody(null);
			leftGrabbedController = null;
		}
		if (highlightedControllersLeft.Count <= 0)
		{
			return;
		}
		Rigidbody component = leftControl.GetComponent<Rigidbody>();
		FreeControllerV3 item = highlightedControllersLeft[0];
		highlightedControllersLeft.RemoveAt(0);
		highlightedControllersLeft.Add(item);
		leftGrabbedController = item;
		if (rightFullGrabbedController == leftGrabbedController)
		{
			rightFullGrabbedController = null;
			rightHandControl = null;
		}
		if (rightGrabbedController == leftGrabbedController)
		{
			rightGrabbedController = null;
		}
		if (!(component != null))
		{
			return;
		}
		if (leftFullGrabbedController != null)
		{
			Rigidbody followWhenOffRB = leftFullGrabbedController.followWhenOffRB;
			if (followWhenOffRB != null)
			{
				leftGrabbedController.SelectLinkToRigidbody(followWhenOffRB, FreeControllerV3.SelectLinkState.PositionAndRotation, usePhysicalLink: true);
			}
			else
			{
				leftGrabbedController.SelectLinkToRigidbody(component);
			}
		}
		else
		{
			leftGrabbedController.SelectLinkToRigidbody(component);
		}
	}

	private void ProcessTargetSelectionDoFullGrabLeft(Transform leftControl)
	{
		if (highlightedControllersLeft.Count > 0)
		{
			Rigidbody component = leftControl.GetComponent<Rigidbody>();
			FreeControllerV3 item = highlightedControllersLeft[0];
			highlightedControllersLeft.RemoveAt(0);
			highlightedControllersLeft.Add(item);
			leftFullGrabbedController = item;
			if (rightFullGrabbedController == leftFullGrabbedController)
			{
				rightFullGrabbedController = null;
				rightHandControl = null;
			}
			if (leftGrabbedController == leftFullGrabbedController)
			{
				leftGrabbedController = null;
			}
			if (component != null)
			{
				leftFullGrabbedController.SelectLinkToRigidbody(component);
			}
			leftHandControl = leftFullGrabbedController.GetComponent<HandControl>();
		}
		else if (leftGrabbedController != null)
		{
			leftFullGrabbedController = leftGrabbedController;
			leftGrabbedController = null;
			leftHandControl = leftFullGrabbedController.GetComponent<HandControl>();
		}
	}

	private void ProcessMotionControllerTargetSelection()
	{
		if (highlightedControllersLeft == null)
		{
			highlightedControllersLeft = new List<FreeControllerV3>();
		}
		if (highlightedControllersRight == null)
		{
			highlightedControllersRight = new List<FreeControllerV3>();
		}
		ProcessControllerTargetHighlight(leftSelectionHUD, motionControllerLeft, _pointerModeLeft, rayLineDrawerLeft, highlightedControllersLeft, GUIhitLeft, leftFullGrabbedController);
		if (leftGrabbedController != null || leftFullGrabbedController != null)
		{
			HideLeftController();
			leftSelectionHUD.gameObject.SetActive(value: false);
		}
		else
		{
			ShowLeftController();
		}
		ProcessControllerTargetHighlight(rightSelectionHUD, motionControllerRight, _pointerModeRight, rayLineDrawerRight, highlightedControllersRight, GUIhitRight, rightFullGrabbedController);
		if (rightGrabbedController != null || rightFullGrabbedController != null)
		{
			HideRightController();
			rightSelectionHUD.gameObject.SetActive(value: false);
		}
		else
		{
			ShowRightController();
		}
		ProcessGUIInteract();
		bool targetShow = GetTargetShow();
		if (selectMode != SelectMode.Targets && (targetShow || targetsOnWithButton))
		{
			SelectModeTargets();
			_pointerModeLeft = true;
			_pointerModeRight = true;
		}
		if (selectMode != 0 && !targetShow && !targetsOnWithButton)
		{
			SelectModeOff();
			_pointerModeLeft = false;
			_pointerModeRight = false;
		}
		if (!GUIhitRight)
		{
			if (GetRightSelect())
			{
				ProcessTargetSelectionDoSelect(highlightedControllersRight);
			}
			if (GetRightQuickGrab())
			{
				ProcessTargetSelectionDoGrabRight(motionControllerRight);
			}
			if (GetRightGrabToggle())
			{
				if ((bool)rightFullGrabbedController)
				{
					rightFullGrabbedController.SelectLinkToRigidbody(null);
					rightFullGrabbedController = null;
				}
				else
				{
					ProcessTargetSelectionDoFullGrabRight(motionControllerRight);
				}
			}
			if (rightHandControl != null)
			{
				float rightTriggerVal = GetRightTriggerVal();
				if (rightTriggerVal > 0.01f || rightTriggerOn)
				{
					rightTriggerOn = rightTriggerVal != 0f;
					rightHandControl.thumbGrasp = rightTriggerVal * 0.3f;
					rightHandControl.fingerGrasp = rightTriggerVal * 0.3f;
				}
			}
			if (highlightedControllersRight.Count > 1)
			{
				int rightTouchScroll = ViveTrackedControllers.singleton.GetRightTouchScroll();
				if (rightTouchScroll > 0)
				{
					ProcessTargetSelectionCycleBackwardsSelect(highlightedControllersRight);
				}
				if (rightTouchScroll < 0)
				{
					ProcessTargetSelectionCycleSelect(highlightedControllersRight);
				}
			}
		}
		if (!GUIhitLeft)
		{
			if (GetLeftSelect())
			{
				ProcessTargetSelectionDoSelect(highlightedControllersLeft);
			}
			if (GetLeftQuickGrab())
			{
				ProcessTargetSelectionDoGrabLeft(motionControllerLeft);
			}
			if (GetLeftGrabToggle())
			{
				if ((bool)leftFullGrabbedController)
				{
					leftFullGrabbedController.SelectLinkToRigidbody(null);
					leftFullGrabbedController = null;
					leftHandControl = null;
				}
				else
				{
					ProcessTargetSelectionDoFullGrabLeft(motionControllerLeft);
				}
			}
			if (leftHandControl != null)
			{
				float leftTriggerVal = GetLeftTriggerVal();
				if (leftTriggerVal > 0.01f || leftTriggerOn)
				{
					leftTriggerOn = leftTriggerVal != 0f;
					leftHandControl.thumbGrasp = leftTriggerVal * 0.3f;
					leftHandControl.fingerGrasp = leftTriggerVal * 0.3f;
				}
			}
			if (highlightedControllersLeft.Count > 1)
			{
				int leftTouchScroll = ViveTrackedControllers.singleton.GetLeftTouchScroll();
				if (leftTouchScroll > 0)
				{
					ProcessTargetSelectionCycleBackwardsSelect(highlightedControllersLeft);
				}
				if (leftTouchScroll < 0)
				{
					ProcessTargetSelectionCycleSelect(highlightedControllersLeft);
				}
			}
		}
		if (GetRightQuickRelease() && (bool)rightGrabbedController)
		{
			rightGrabbedController.SelectLinkToRigidbody(null);
			rightGrabbedController = null;
		}
		if (GetLeftQuickRelease() && (bool)leftGrabbedController)
		{
			leftGrabbedController.SelectLinkToRigidbody(null);
			leftGrabbedController = null;
		}
	}

	private void ProcessCommonTargetSelection()
	{
		if (highlightedControllersLook == null)
		{
			highlightedControllersLook = new List<FreeControllerV3>();
		}
		if (!(lookCamera != null) || !useLookSelect)
		{
			return;
		}
		if (GUIhit)
		{
			UnhighlightControllers(highlightedControllersLook);
			if (selectionHUD != null)
			{
				selectionHUD.ClearSelections();
				selectionHUD.gameObject.SetActive(value: false);
			}
		}
		else if (selectMode != 0)
		{
			Transform transform = lookCamera.transform;
			castRay.origin = transform.position;
			castRay.direction = transform.forward;
			ProcessTargetSelectionDoRaycast(selectionHUD, castRay, highlightedControllersLook);
		}
	}

	private void ProcessControllerTargetSelection()
	{
		if (!useLookSelect)
		{
			return;
		}
		if (highlightedControllersLook == null)
		{
			highlightedControllersLook = new List<FreeControllerV3>();
		}
		if (buttonToggleTargets != null && JoystickControl.GetButtonDown(buttonToggleTargets))
		{
			targetsOnWithButton = !targetsOnWithButton;
		}
		if (!GUIhit)
		{
			if (buttonSelect != null && buttonSelect != string.Empty && JoystickControl.GetButtonDown(buttonSelect))
			{
				ProcessTargetSelectionDoSelect(highlightedControllersLook);
			}
			if (buttonCycleSelection != null && buttonCycleSelection != string.Empty && JoystickControl.GetButtonDown(buttonCycleSelection) && highlightedControllersLook.Count > 0)
			{
				ProcessTargetSelectionCycleSelect(highlightedControllersLook);
			}
		}
		if (buttonUnselect != null && buttonUnselect != string.Empty && JoystickControl.GetButtonDown(buttonUnselect))
		{
			ClearSelection();
		}
	}

	private void ProcessTargetControl()
	{
		if (selectedController != null)
		{
			if (axis1 != 0 && (!(LookInputModule.singleton != null) || !LookInputModule.singleton.controlAxisUsed || LookInputModule.singleton.controlAxis != axis1))
			{
				float num = JoystickControl.GetAxis(axis1);
				if (num > 0.01f || num < -0.01f)
				{
					if (invertAxis1)
					{
						num = 0f - num;
					}
					selectedController.ControlAxis1(num * _controllerScale);
				}
			}
			if (axis2 != 0 && (!(LookInputModule.singleton != null) || !LookInputModule.singleton.controlAxisUsed || LookInputModule.singleton.controlAxis != axis2))
			{
				float num2 = JoystickControl.GetAxis(axis2);
				if (num2 > 0.01f || num2 < -0.01f)
				{
					if (invertAxis2)
					{
						num2 = 0f - num2;
					}
					selectedController.ControlAxis2(num2 * _controllerScale);
				}
			}
			if (axis3 != 0 && (!(LookInputModule.singleton != null) || !LookInputModule.singleton.controlAxisUsed || LookInputModule.singleton.controlAxis != axis3))
			{
				float num3 = JoystickControl.GetAxis(axis3);
				if (num3 > 0.01f || num3 < -0.01f)
				{
					if (invertAxis3)
					{
						num3 = 0f - num3;
					}
					selectedController.ControlAxis3(num3 * _controllerScale);
				}
			}
		}
		if (buttonToggleRotateMode != null && buttonToggleRotateMode != string.Empty && JoystickControl.GetButtonDown(buttonToggleRotateMode))
		{
			ToggleRotationMode();
		}
	}

	private void AssignUICamera(Camera c)
	{
		if (c != null)
		{
			LookInputModule.singleton.referenceCamera = c;
			{
				foreach (Canvas allCanvase in allCanvases)
				{
					if (allCanvase.renderMode == RenderMode.WorldSpace)
					{
						allCanvase.worldCamera = c;
					}
				}
				return;
			}
		}
		Debug.LogError("Tried to call AssignUICamera with a null camera");
	}

	private void ProcessUI()
	{
		if (!disableUI)
		{
			if (GetMenuShow())
			{
				ShowMainHUD();
			}
			if (GetMenuMove())
			{
				MoveMainHUD(motionControllerLeft);
			}
		}
		if (!(LookInputModule.singleton != null))
		{
			return;
		}
		if (useLookSelect)
		{
			AssignUICamera(lookCamera);
			LookInputModule.singleton.ProcessMain();
			GUIhit = LookInputModule.singleton.guiRaycastHit;
		}
		else if (leftControllerCamera != null)
		{
			AssignUICamera(leftControllerCamera);
			LookInputModule.singleton.ProcessMain();
			GUIhitLeft = LookInputModule.singleton.guiRaycastHit;
			if (rightControllerCamera != null)
			{
				AssignUICamera(rightControllerCamera);
				LookInputModule.singleton.ProcessRight();
				GUIhitRight = LookInputModule.singleton.guiRaycastHit;
			}
			else
			{
				Debug.LogError("Right controller camera is null while processing UI");
			}
			AssignUICamera(leftControllerCamera);
		}
	}

	private void ProcessSelectDoRaycast(SelectionHUD sh, Ray ray, List<SelectTarget> hitsList, bool doHighlight = true)
	{
		RaycastHit[] array = Physics.RaycastAll(ray, 50f, selectColliderMask);
		if (array != null && array.Length > 0)
		{
			Dictionary<SelectTarget, bool> dictionary = new Dictionary<SelectTarget, bool>();
			RaycastHit[] array2 = array;
			foreach (RaycastHit raycastHit in array2)
			{
				SelectTarget component = raycastHit.transform.GetComponent<SelectTarget>();
				if (component != null && !dictionary.ContainsKey(component))
				{
					dictionary.Add(component, value: true);
					if (!hitsList.Contains(component))
					{
						hitsList.Add(component);
					}
				}
			}
			SelectTarget[] array3 = hitsList.ToArray();
			SelectTarget[] array4 = array3;
			foreach (SelectTarget selectTarget in array4)
			{
				if (!dictionary.ContainsKey(selectTarget))
				{
					selectTarget.highlighted = false;
					hitsList.Remove(selectTarget);
				}
			}
			if (doHighlight)
			{
				for (int k = 0; k < hitsList.Count; k++)
				{
					SelectTarget selectTarget2 = hitsList[k];
					if (k == 0)
					{
						selectTarget2.highlighted = true;
					}
					else
					{
						selectTarget2.highlighted = false;
					}
				}
			}
			if (sh != null)
			{
				sh.ClearSelections();
				if (hitsList.Count > 0)
				{
					int num = 0;
					foreach (SelectTarget hits in hitsList)
					{
						sh.SetSelection(num, hits.transform, hits.selectionName);
						num++;
					}
				}
			}
		}
		else
		{
			if (doHighlight)
			{
				foreach (SelectTarget hits2 in hitsList)
				{
					hits2.highlighted = false;
				}
			}
			hitsList.Clear();
		}
		if (sh != null)
		{
			if (hitsList.Count > 0)
			{
				sh.gameObject.SetActive(value: true);
				sh.transform.position = hitsList[0].transform.position;
				Vector3 localScale = default(Vector3);
				localScale.z = (localScale.y = (localScale.x = (sh.transform.position - lookCamera.transform.position).magnitude));
				sh.transform.localScale = localScale;
				sh.transform.LookAt(lookCamera.transform.position);
			}
			else
			{
				sh.gameObject.SetActive(value: false);
			}
		}
	}

	private void ProcessSelectDoSelect(List<SelectTarget> highlightedSelectTargets)
	{
		SelectTarget selectTarget = highlightedSelectTargets[0];
		switch (selectMode)
		{
		case SelectMode.Controller:
		{
			if (fcMap.TryGetValue(selectTarget.selectionName, out var value2))
			{
				selectControllerCallback(value2);
				SelectModeOff();
			}
			break;
		}
		case SelectMode.ForceProducer:
		{
			if (fpMap.TryGetValue(selectTarget.selectionName, out var value4))
			{
				selectForceProducerCallback(value4);
				SelectModeOff();
			}
			break;
		}
		case SelectMode.ForceReceiver:
		{
			if (frMap.TryGetValue(selectTarget.selectionName, out var value5))
			{
				selectForceReceiverCallback(value5);
				SelectModeOff();
			}
			break;
		}
		case SelectMode.Rigidbody:
		{
			if (rbMap.TryGetValue(selectTarget.selectionName, out var value3))
			{
				selectRigidbodyCallback(value3);
				SelectModeOff();
			}
			break;
		}
		case SelectMode.Atom:
		{
			if (atoms.TryGetValue(selectTarget.selectionName, out var value))
			{
				selectAtomCallback(value);
				SelectModeOff();
			}
			break;
		}
		}
	}

	private void ProcessSelectCycleSelect(List<SelectTarget> highlightedSelectTargets)
	{
		SelectTarget item = highlightedSelectTargets[0];
		highlightedSelectTargets.RemoveAt(0);
		highlightedSelectTargets.Add(item);
	}

	private void ProcessSelectCycleBackwardsSelect(List<SelectTarget> highlightedSelectTargets)
	{
		int index = highlightedSelectTargets.Count - 1;
		SelectTarget item = highlightedSelectTargets[index];
		highlightedSelectTargets.RemoveAt(index);
		highlightedSelectTargets.Insert(0, item);
	}

	private void ProcessSelectTargetHighlight(SelectionHUD sh, Transform processFrom, bool isLeft)
	{
		castRay.origin = processFrom.position;
		castRay.direction = processFrom.forward;
		if (isLeft)
		{
			if (rayLineDrawerLeft != null)
			{
				rayLineDrawerLeft.SetLinePoints(processFrom.position, processFrom.position + 50f * processFrom.forward);
				rayLineDrawerLeft.Draw(base.gameObject.layer);
			}
			ProcessSelectDoRaycast(sh, castRay, highlightedSelectTargetsLeft, doHighlight: false);
			sh.useDrawFromPosition = true;
			sh.drawFrom = processFrom.position;
		}
		else
		{
			if (rayLineDrawerRight != null)
			{
				rayLineDrawerRight.SetLinePoints(processFrom.position, processFrom.position + 50f * processFrom.forward);
				rayLineDrawerRight.Draw(base.gameObject.layer);
			}
			ProcessSelectDoRaycast(sh, castRay, highlightedSelectTargetsRight, doHighlight: false);
			sh.useDrawFromPosition = true;
			sh.drawFrom = processFrom.position;
		}
	}

	private void ProcessViveControllerSelect()
	{
		if (highlightedSelectTargetsLeft == null)
		{
			highlightedSelectTargetsLeft = new List<SelectTarget>();
		}
		if (highlightedSelectTargetsRight == null)
		{
			highlightedSelectTargetsRight = new List<SelectTarget>();
		}
		if (viveObjectLeft != null && !GUIhitLeft)
		{
			ProcessSelectTargetHighlight(leftSelectionHUD, viveObjectLeft.transform, isLeft: true);
		}
		if (viveObjectRight != null && !GUIhitRight)
		{
			ProcessSelectTargetHighlight(rightSelectionHUD, viveObjectRight.transform, isLeft: false);
		}
		if (ViveTrackedControllers.singleton.leftTouchpadPressedThisFrame && highlightedSelectTargetsLeft.Count > 0)
		{
			ProcessSelectDoSelect(highlightedSelectTargetsLeft);
		}
		if (ViveTrackedControllers.singleton.rightTouchpadPressedThisFrame && highlightedSelectTargetsRight.Count > 0)
		{
			ProcessSelectDoSelect(highlightedSelectTargetsRight);
		}
		if (highlightedSelectTargetsLeft != null && highlightedSelectTargetsLeft.Count > 0)
		{
			int leftTouchScroll = ViveTrackedControllers.singleton.GetLeftTouchScroll();
			if (leftTouchScroll > 0)
			{
				ProcessSelectCycleBackwardsSelect(highlightedSelectTargetsLeft);
			}
			if (leftTouchScroll < 0)
			{
				ProcessSelectCycleSelect(highlightedSelectTargetsLeft);
			}
		}
		if (highlightedSelectTargetsRight != null && highlightedSelectTargetsRight.Count > 0)
		{
			int rightTouchScroll = ViveTrackedControllers.singleton.GetRightTouchScroll();
			if (rightTouchScroll > 0)
			{
				ProcessSelectCycleBackwardsSelect(highlightedSelectTargetsRight);
			}
			if (rightTouchScroll < 0)
			{
				ProcessSelectCycleSelect(highlightedSelectTargetsRight);
			}
		}
		if (ViveTrackedControllers.singleton.leftGrippedThisFrame || ViveTrackedControllers.singleton.rightGrippedThisFrame)
		{
			SelectModeOff();
		}
	}

	private void ProcessSelect()
	{
		if (highlightedSelectTargetsLook == null)
		{
			highlightedSelectTargetsLook = new List<SelectTarget>();
		}
		if (useLookSelect && lookCamera != null && !GUIhit)
		{
			Transform transform = lookCamera.transform;
			castRay.origin = transform.position;
			castRay.direction = transform.forward;
			ProcessSelectDoRaycast(selectionHUD, castRay, highlightedSelectTargetsLook);
			if (buttonSelect != null && buttonSelect != string.Empty && JoystickControl.GetButtonDown(buttonSelect) && highlightedSelectTargetsLook.Count > 0)
			{
				ProcessSelectDoSelect(highlightedSelectTargetsLook);
			}
			if (buttonCycleSelection != null && buttonCycleSelection != string.Empty && JoystickControl.GetButtonDown(buttonCycleSelection) && highlightedSelectTargetsLook.Count > 0)
			{
				ProcessSelectCycleSelect(highlightedSelectTargetsLook);
			}
			if (buttonUnselect != null && buttonUnselect != string.Empty && JoystickControl.GetButtonDown(buttonUnselect))
			{
				SelectModeOff();
			}
		}
	}

	private void ProcessTabControl()
	{
		if (_mainHUDVisible)
		{
			if (!GUIhit || tabAxis == JoystickControl.Axis.None)
			{
				return;
			}
			float axis = JoystickControl.GetAxis(tabAxis);
			if (axis > 0.01f || axis < -0.01f)
			{
				if (!tabAxisOn && UITabSelector.activeTabSelector != null)
				{
					if (axis > 0.01f)
					{
						UITabSelector.activeTabSelector.SelectNextTab();
					}
					else
					{
						UITabSelector.activeTabSelector.SelectPreviousTab();
					}
				}
				tabAxisOn = true;
			}
			else
			{
				tabAxisOn = false;
			}
		}
		else
		{
			UITabSelector.activeTabSelector = null;
		}
	}

	private string CreateUID(string name)
	{
		if (!uids.ContainsKey(name))
		{
			uids.Add(name, value: true);
			return name;
		}
		for (int i = 2; i < maxUID; i++)
		{
			string text = name + "#" + i;
			if (!uids.ContainsKey(text))
			{
				uids.Add(text, value: true);
				return text;
			}
		}
		Debug.LogError("Exceeded UID limit of " + maxUID + " for " + name);
		return null;
	}

	private void SyncForceReceiverNames()
	{
		_forceReceiverNames = new string[frMap.Keys.Count];
		frMap.Keys.CopyTo(_forceReceiverNames, 0);
		if (onForceReceiverNamesChangedHandlers != null)
		{
			onForceReceiverNamesChangedHandlers(_forceReceiverNames);
		}
	}

	public List<string> GetForceReceiverNamesInAtom(string atomUID)
	{
		List<string> list = new List<string>();
		if (atoms.TryGetValue(atomUID, out var value))
		{
			ForceReceiver[] forceReceivers = value.forceReceivers;
			foreach (ForceReceiver forceReceiver in forceReceivers)
			{
				list.Add(forceReceiver.name);
			}
		}
		return list;
	}

	public ForceReceiver ReceiverNameToForceReceiver(string receiverName)
	{
		if (frMap != null && frMap.TryGetValue(receiverName, out var value))
		{
			return value;
		}
		return null;
	}

	private void SyncForceProducerNames()
	{
		_forceProducerNames = new string[fpMap.Keys.Count];
		fpMap.Keys.CopyTo(_forceProducerNames, 0);
		if (onForceProducerNamesChangedHandlers != null)
		{
			onForceProducerNamesChangedHandlers(_forceProducerNames);
		}
	}

	public List<string> GetForceProducerNamesInAtom(string atomUID)
	{
		List<string> list = new List<string>();
		if (atoms.TryGetValue(atomUID, out var value))
		{
			ForceProducerV2[] forceProducers = value.forceProducers;
			foreach (ForceProducerV2 forceProducerV in forceProducers)
			{
				list.Add(forceProducerV.name);
			}
		}
		return list;
	}

	public ForceProducerV2 ProducerNameToForceProducer(string producerName)
	{
		if (fpMap != null && fpMap.TryGetValue(producerName, out var value))
		{
			return value;
		}
		return null;
	}

	private void SyncFreeControllerNames()
	{
		_freeControllerNames = new string[fcMap.Keys.Count];
		fcMap.Keys.CopyTo(_freeControllerNames, 0);
		if (onFreeControllerNamesChangedHandlers != null)
		{
			onFreeControllerNamesChangedHandlers(_freeControllerNames);
		}
	}

	public List<string> GetFreeControllerNamesInAtom(string atomUID)
	{
		List<string> list = new List<string>();
		if (atoms.TryGetValue(atomUID, out var value))
		{
			FreeControllerV3[] freeControllers = value.freeControllers;
			foreach (FreeControllerV3 freeControllerV in freeControllers)
			{
				list.Add(freeControllerV.name);
			}
		}
		return list;
	}

	public FreeControllerV3 FreeControllerNameToFreeController(string controllerName)
	{
		if (fcMap != null && fcMap.TryGetValue(controllerName, out var value))
		{
			return value;
		}
		return null;
	}

	private void SyncRigidbodyNames()
	{
		_rigidbodyNames = new string[rbMap.Keys.Count];
		rbMap.Keys.CopyTo(_rigidbodyNames, 0);
		if (onRigidbodyNamesChangedHandlers != null)
		{
			onRigidbodyNamesChangedHandlers(_rigidbodyNames);
		}
	}

	public List<string> GetRigidbodyNamesInAtom(string atomUID)
	{
		List<string> list = new List<string>();
		if (atoms.TryGetValue(atomUID, out var value))
		{
			Rigidbody[] linkableRigidbodies = value.linkableRigidbodies;
			foreach (Rigidbody rigidbody in linkableRigidbodies)
			{
				list.Add(rigidbody.name);
			}
		}
		return list;
	}

	public Rigidbody RigidbodyNameToRigidbody(string rigidbodyName)
	{
		if (rbMap != null && rbMap.TryGetValue(rigidbodyName, out var value))
		{
			return value;
		}
		return null;
	}

	public List<string> GetAtomUIDs()
	{
		return atomUIDs;
	}

	public List<string> GetAtomUIDsWithForceReceivers()
	{
		return atomUIDsWithForceReceivers;
	}

	public List<string> GetAtomUIDsWithForceProducers()
	{
		return atomUIDsWithForceProducers;
	}

	public List<string> GetAtomUIDsWithFreeControllers()
	{
		return atomUIDsWithFreeControllers;
	}

	public List<string> GetAtomUIDsWithRigidbodies()
	{
		return atomUIDsWithRigidbodies;
	}

	public Atom GetAtomByUid(string uid)
	{
		Atom value = null;
		atoms.TryGetValue(uid, out value);
		return value;
	}

	public void AddAtomByPopupValue()
	{
		if (atomPrefabPopup != null)
		{
			AddAtomByType(atomPrefabPopup.currentValue);
		}
	}

	public Transform AddAtomByType(string type, string useuid = null)
	{
		if (type != null)
		{
			if (atomPrefabByType.TryGetValue(type, out var value))
			{
				return AddAtom(value, useuid);
			}
			Debug.LogError("Atom type " + type + " does not exist. Cannot add");
			return null;
		}
		Debug.LogError("Atom type is null for " + useuid + ". Cannot add");
		return null;
	}

	public Transform AddAtom(Atom atom, string useuid = null)
	{
		string text = ((useuid == null) ? CreateUID(atom.name) : CreateUID(useuid));
		if (text != null)
		{
			Transform transform = UnityEngine.Object.Instantiate(atom.transform);
			transform.SetParent(atomContainer, worldPositionStays: true);
			Atom component = transform.GetComponent<Atom>();
			component.uid = text;
			component.name = text;
			InitAtom(component);
			component.collisionEnabled = false;
			if (onAtomUIDsChangedHandlers != null)
			{
				onAtomUIDsChangedHandlers(atomUIDs);
			}
			if (onAtomUIDsWithForceReceiversChangedHandlers != null && component.forceReceivers.Length > 0)
			{
				onAtomUIDsWithForceReceiversChangedHandlers(atomUIDsWithForceReceivers);
			}
			if (onAtomUIDsWithForceProducersChangedHandlers != null && component.forceProducers.Length > 0)
			{
				onAtomUIDsWithForceProducersChangedHandlers(atomUIDsWithForceProducers);
			}
			if (onAtomUIDsWithFreeControllersChangedHandlers != null && component.freeControllers.Length > 0)
			{
				onAtomUIDsWithFreeControllersChangedHandlers(atomUIDsWithFreeControllers);
			}
			if (onAtomUIDsWithRigidbodiesChangedHandlers != null && component.linkableRigidbodies.Length > 0)
			{
				onAtomUIDsWithRigidbodiesChangedHandlers(atomUIDsWithRigidbodies);
			}
			SyncVisibility();
			SyncForceReceiverNames();
			SyncForceProducerNames();
			SyncFreeControllerNames();
			SyncRigidbodyNames();
			return transform;
		}
		return null;
	}

	public void RemoveAtom(Atom atom)
	{
		foreach (string atomUID in atomUIDs)
		{
			Atom atomByUid = GetAtomByUid(atomUID);
			if (atomByUid != null && atomByUid.parentAtom == atom)
			{
				atomByUid.parentAtom = null;
			}
		}
		if (atom.parentAtom != null)
		{
			atom.parentAtom = null;
		}
		atoms.Remove(atom.uid);
		atomUIDs.Remove(atom.uid);
		uids.Remove(atom.uid);
		if (onAtomUIDsChangedHandlers != null)
		{
			onAtomUIDsChangedHandlers(atomUIDs);
		}
		if (atomUIDsWithForceReceivers.Remove(atom.uid) && onAtomUIDsWithForceReceiversChangedHandlers != null)
		{
			onAtomUIDsWithForceReceiversChangedHandlers(atomUIDsWithForceReceivers);
		}
		if (atomUIDsWithForceProducers.Remove(atom.uid) && onAtomUIDsWithForceProducersChangedHandlers != null)
		{
			onAtomUIDsWithForceProducersChangedHandlers(atomUIDsWithForceProducers);
		}
		if (atomUIDsWithFreeControllers.Remove(atom.uid) && onAtomUIDsWithFreeControllersChangedHandlers != null)
		{
			onAtomUIDsWithFreeControllersChangedHandlers(atomUIDsWithFreeControllers);
		}
		if (atomUIDsWithRigidbodies.Remove(atom.uid) && onAtomUIDsWithRigidbodiesChangedHandlers != null)
		{
			onAtomUIDsWithRigidbodiesChangedHandlers(atomUIDsWithRigidbodies);
		}
		FreeControllerV3[] freeControllers = atom.freeControllers;
		foreach (FreeControllerV3 freeControllerV in freeControllers)
		{
			allControllers.Remove(freeControllerV);
			string key = atom.uid + ":" + freeControllerV.name;
			fcMap.Remove(key);
		}
		ForceProducerV2[] forceProducers = atom.forceProducers;
		foreach (ForceProducerV2 forceProducerV in forceProducers)
		{
			string key2 = atom.uid + ":" + forceProducerV.name;
			fpMap.Remove(key2);
		}
		ForceReceiver[] forceReceivers = atom.forceReceivers;
		foreach (ForceReceiver forceReceiver in forceReceivers)
		{
			string key3 = atom.uid + ":" + forceReceiver.name;
			frMap.Remove(key3);
		}
		Rigidbody[] linkableRigidbodies = atom.linkableRigidbodies;
		foreach (Rigidbody rigidbody in linkableRigidbodies)
		{
			string key4 = atom.uid + ":" + rigidbody.name;
			rbMap.Remove(key4);
		}
		AnimationPattern[] animationPatterns = atom.animationPatterns;
		foreach (AnimationPattern item in animationPatterns)
		{
			allAnimationPatterns.Remove(item);
		}
		AnimationStep[] animationSteps = atom.animationSteps;
		foreach (AnimationStep item2 in animationSteps)
		{
			allAnimationSteps.Remove(item2);
		}
		Animator[] animators = atom.animators;
		foreach (Animator item3 in animators)
		{
			allAnimators.Remove(item3);
		}
		Canvas[] canvases = atom.canvases;
		foreach (Canvas item4 in canvases)
		{
			allCanvases.Remove(item4);
		}
		SyncForceReceiverNames();
		SyncForceProducerNames();
		SyncFreeControllerNames();
		UnityEngine.Object.DestroyImmediate(atom.gameObject);
	}

	private void InitAtom(Atom atom)
	{
		atoms.Add(atom.uid, atom);
		atomUIDs.Add(atom.uid);
		bool flag = false;
		FreeControllerV3[] freeControllers = atom.freeControllers;
		foreach (FreeControllerV3 freeControllerV in freeControllers)
		{
			flag = true;
			allControllers.Add(freeControllerV);
			string key = atom.uid + ":" + freeControllerV.name;
			fcMap.Add(key, freeControllerV);
		}
		if (flag)
		{
			atomUIDsWithFreeControllers.Add(atom.uid);
		}
		bool flag2 = false;
		ForceProducerV2[] forceProducers = atom.forceProducers;
		foreach (ForceProducerV2 forceProducerV in forceProducers)
		{
			flag2 = true;
			string key2 = atom.uid + ":" + forceProducerV.name;
			fpMap.Add(key2, forceProducerV);
		}
		if (flag2)
		{
			atomUIDsWithForceProducers.Add(atom.uid);
		}
		bool flag3 = false;
		ForceReceiver[] forceReceivers = atom.forceReceivers;
		foreach (ForceReceiver forceReceiver in forceReceivers)
		{
			flag3 = true;
			string key3 = atom.uid + ":" + forceReceiver.name;
			frMap.Add(key3, forceReceiver);
		}
		if (flag3)
		{
			atomUIDsWithForceReceivers.Add(atom.uid);
		}
		bool flag4 = false;
		Rigidbody[] rigidbodies = atom.rigidbodies;
		foreach (Rigidbody rigidbody in rigidbodies)
		{
			rigidbody.maxAngularVelocity = maxAngularVelocity;
			rigidbody.maxDepenetrationVelocity = maxDepenetrationVelocity;
		}
		Rigidbody[] linkableRigidbodies = atom.linkableRigidbodies;
		foreach (Rigidbody rigidbody2 in linkableRigidbodies)
		{
			flag4 = true;
			string key4 = atom.uid + ":" + rigidbody2.name;
			rbMap.Add(key4, rigidbody2);
		}
		if (flag4)
		{
			atomUIDsWithRigidbodies.Add(atom.uid);
		}
		AnimationPattern[] animationPatterns = atom.animationPatterns;
		foreach (AnimationPattern item in animationPatterns)
		{
			allAnimationPatterns.Add(item);
		}
		AnimationStep[] animationSteps = atom.animationSteps;
		foreach (AnimationStep item2 in animationSteps)
		{
			allAnimationSteps.Add(item2);
		}
		Animator[] animators = atom.animators;
		foreach (Animator item3 in animators)
		{
			allAnimators.Add(item3);
		}
		Canvas[] canvases = atom.canvases;
		foreach (Canvas item4 in canvases)
		{
			allCanvases.Add(item4);
		}
		atom.useRigidbodyInterpolation = _useInterpolation;
	}

	private void InitAtoms()
	{
		atomPrefabByType = new Dictionary<string, Atom>();
		List<string> list = new List<string>();
		Atom[] array = atomPrefabs;
		foreach (Atom atom in array)
		{
			if (atom != null)
			{
				string type = atom.type;
				if (!atomPrefabByType.ContainsKey(type))
				{
					list.Add(type);
					atomPrefabByType.Add(type, atom);
					continue;
				}
				Debug.LogError("Atom " + atom.name + " uses type " + type + " that is already used");
			}
		}
		Atom[] array2 = indirectAtomPrefabs;
		foreach (Atom atom2 in array2)
		{
			if (atom2 != null)
			{
				string type2 = atom2.type;
				if (!atomPrefabByType.ContainsKey(type2))
				{
					atomPrefabByType.Add(type2, atom2);
					continue;
				}
				Debug.LogError("Atom " + atom2.name + " uses type " + type2 + " that is already used");
			}
		}
		List<Atom> list2 = new List<Atom>();
		Atom[] array3 = atomPrefabs;
		foreach (Atom atom3 in array3)
		{
			if (atom3 != null)
			{
				list2.Add(atom3);
			}
		}
		list2.Sort((Atom x, Atom y) => (x.category == y.category) ? x.type.CompareTo(y.type) : x.category.CompareTo(y.category));
		if (atomPrefabPopup != null)
		{
			atomPrefabPopup.numPopupValues = list2.Count;
			for (int l = 0; l < list2.Count; l++)
			{
				atomPrefabPopup.setPopupValue(l, list2[l].type);
			}
		}
		atoms = new Dictionary<string, Atom>();
		atomUIDs = new List<string>();
		atomUIDsWithForceReceivers = new List<string>();
		atomUIDsWithForceProducers = new List<string>();
		atomUIDsWithFreeControllers = new List<string>();
		atomUIDsWithRigidbodies = new List<string>();
		uids = new Dictionary<string, bool>();
		frMap = new Dictionary<string, ForceReceiver>();
		fpMap = new Dictionary<string, ForceProducerV2>();
		fcMap = new Dictionary<string, FreeControllerV3>();
		rbMap = new Dictionary<string, Rigidbody>();
		allControllers = new List<FreeControllerV3>();
		allAnimationPatterns = new List<AnimationPattern>();
		allAnimationSteps = new List<AnimationStep>();
		allAnimators = new List<Animator>();
		allCanvases = new List<Canvas>();
		Atom[] componentsInChildren = atomContainer.GetComponentsInChildren<Atom>();
		startingAtoms = new Dictionary<string, Atom>();
		Atom[] array4 = componentsInChildren;
		foreach (Atom atom4 in array4)
		{
			string text = CreateUID(atom4.name);
			if (text != null)
			{
				atom4.uid = text;
				InitAtom(atom4);
				startingAtoms.Add(text, atom4);
			}
		}
		if (onAtomUIDsChangedHandlers != null)
		{
			onAtomUIDsChangedHandlers(atomUIDs);
		}
		if (onAtomUIDsWithForceReceiversChangedHandlers != null)
		{
			onAtomUIDsWithForceReceiversChangedHandlers(atomUIDsWithForceReceivers);
		}
		if (onAtomUIDsWithForceProducersChangedHandlers != null)
		{
			onAtomUIDsWithForceProducersChangedHandlers(atomUIDsWithForceProducers);
		}
		if (onAtomUIDsWithFreeControllersChangedHandlers != null)
		{
			onAtomUIDsWithFreeControllersChangedHandlers(atomUIDsWithFreeControllers);
		}
		if (onAtomUIDsWithRigidbodiesChangedHandlers != null)
		{
			onAtomUIDsWithRigidbodiesChangedHandlers(atomUIDsWithRigidbodies);
		}
		SyncForceReceiverNames();
		SyncForceProducerNames();
		SyncFreeControllerNames();
		SyncRigidbodyNames();
	}

	public void environmentColliderHeightAdjust(float val)
	{
		environmentColliderHeight += val;
	}

	private void ProcessMotionControllerNavigation()
	{
		if (!(navigationPlayArea != null))
		{
			return;
		}
		MeshRenderer meshRenderer = null;
		if (regularPlayArea != null)
		{
			meshRenderer = regularPlayArea.GetComponent<MeshRenderer>();
		}
		if (meshRenderer != null)
		{
			meshRenderer.enabled = false;
		}
		MeshRenderer component = navigationPlayArea.GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.enabled = false;
		}
		MeshRenderer meshRenderer2 = null;
		MeshRenderer meshRenderer3 = null;
		if (lookCamera != null)
		{
			if (navigationPlayer != null)
			{
				meshRenderer2 = navigationPlayer.GetComponent<MeshRenderer>();
				if (meshRenderer2 != null)
				{
					meshRenderer2.enabled = false;
				}
			}
			if (navigationCamera != null)
			{
				meshRenderer3 = navigationCamera.GetComponentInChildren<MeshRenderer>();
				if (meshRenderer3 != null)
				{
					meshRenderer3.enabled = false;
				}
			}
		}
		if (navigationCurve != null)
		{
			navigationCurve.draw = false;
		}
		if (disableNavigation)
		{
			return;
		}
		if (GetNavigateShow())
		{
			if (GetNavigateStart() && navigationPlayArea != null && navigationRig != null)
			{
				startDoubleTouchRotation = navigationRig.rotation;
				navigationPlayArea.rotation = navigationRig.rotation;
			}
			if (meshRenderer != null)
			{
				meshRenderer.enabled = true;
			}
			if (component != null)
			{
				component.enabled = true;
			}
			if (meshRenderer2 != null)
			{
				meshRenderer2.enabled = true;
			}
			if (meshRenderer3 != null)
			{
				meshRenderer3.enabled = true;
			}
			if (useLookForNavigation && lookCamera != null)
			{
				castRay.origin = lookCamera.transform.position;
				castRay.direction = lookCamera.transform.forward;
			}
			else
			{
				castRay.origin = motionControllerRight.position;
				castRay.direction = motionControllerRight.forward;
			}
			RaycastHit[] array = Physics.RaycastAll(castRay, navigationDistance, navigationColliderMask);
			if (array != null && array.Length > 0)
			{
				int num = -1;
				float num2 = navigationDistance;
				for (int i = 0; i < array.Length; i++)
				{
					float magnitude = (array[i].point - castRay.origin).magnitude;
					if (magnitude < num2)
					{
						num = i;
						num2 = magnitude;
					}
				}
				if (lookCamera != null && navigationPlayArea != null)
				{
					if (navigationPlayer != null)
					{
						Vector3 localPosition = lookCamera.transform.localPosition;
						localPosition.y = 0f;
						navigationPlayer.localPosition = localPosition;
					}
					if (navigationCamera != null)
					{
						navigationCamera.localRotation = lookCamera.transform.localRotation;
					}
					Vector3 vector = navigationPlayer.position - navigationPlayArea.position;
					navigationPlayArea.rotation = startDoubleTouchRotation;
					navigationPlayArea.position = array[num].point - vector;
					Vector3 vector2 = Quaternion2Angles.GetAngles(Quaternion.Inverse(lookCamera.transform.rotation) * motionControllerRight.rotation, Quaternion2Angles.RotationOrder.ZXY) * 57.29578f;
					navigationPlayArea.Rotate(Vector3.up, (0f - vector2.z) * 2f);
				}
				if (navigationCurve != null && navigationCurve.points != null && navigationCurve.points.Length == 3)
				{
					if (useLookForNavigation)
					{
						navigationCurve.points[0].transform.position = lookCamera.transform.position;
					}
					else
					{
						navigationCurve.points[0].transform.position = motionControllerRight.position;
					}
					if (navigationPlayer != null)
					{
						navigationCurve.points[2].transform.position = navigationPlayer.position;
					}
					else
					{
						navigationCurve.points[2].transform.position = array[num].point;
					}
					Vector3 position = (navigationCurve.points[0].transform.position + navigationCurve.points[2].transform.position) * 0.5f;
					position.y += 1f * navigationCurve.transform.lossyScale.y;
					navigationCurve.points[1].transform.position = position;
					navigationCurve.draw = true;
				}
			}
		}
		if (GetNavigateFinish() && navigationRig != null)
		{
			navigationRig.position = navigationPlayArea.position;
			navigationRig.rotation = navigationPlayArea.rotation;
		}
	}

	private void ProcessControllerNavigation()
	{
		if (!(navigationRig != null) || !(lookCamera != null) || disableNavigation)
		{
			return;
		}
		if (_navigationForwardAxisEnabled && navigationForwardAxis != 0 && (!(LookInputModule.singleton != null) || !LookInputModule.singleton.controlAxisUsed || LookInputModule.singleton.controlAxis != navigationForwardAxis))
		{
			float num = JoystickControl.GetAxis(navigationForwardAxis);
			if (num > 0.01f || num < -0.01f)
			{
				Vector3 forward = lookCamera.transform.forward;
				forward.y = 0f;
				forward.Normalize();
				if (invertNavigationForwardAxis)
				{
					num = 0f - num;
				}
				Vector3 position = navigationRig.position;
				position += forward * (num * 0.5f * Time.deltaTime / Time.timeScale);
				navigationRig.position = position;
			}
		}
		if (_navigationSideAxisEnabled && navigationSideAxis != 0 && (!(LookInputModule.singleton != null) || !LookInputModule.singleton.controlAxisUsed || LookInputModule.singleton.controlAxis != navigationSideAxis))
		{
			float num2 = JoystickControl.GetAxis(navigationSideAxis);
			if (num2 > 0.01f || num2 < -0.01f)
			{
				Vector3 right = lookCamera.transform.right;
				right.y = 0f;
				right.Normalize();
				if (invertNavigationSideAxis)
				{
					num2 = 0f - num2;
				}
				Vector3 position2 = navigationRig.position;
				position2 += right * (num2 * 0.5f * Time.deltaTime / Time.timeScale);
				navigationRig.position = position2;
			}
		}
		if (_navigationUpAxisEnabled && navigationUpAxis != 0 && (!(LookInputModule.singleton != null) || !LookInputModule.singleton.controlAxisUsed || LookInputModule.singleton.controlAxis != navigationUpAxis))
		{
			float num3 = JoystickControl.GetAxis(navigationUpAxis);
			if (num3 > 0.01f || num3 < -0.01f)
			{
				if (invertNavigationUpAxis)
				{
					num3 = 0f - num3;
				}
				environmentColliderHeight += num3 * 0.5f * Time.deltaTime / Time.timeScale;
			}
		}
		if (!_navigationTurnAxisEnabled || navigationTurnAxis == JoystickControl.Axis.None || (LookInputModule.singleton != null && LookInputModule.singleton.controlAxisUsed && LookInputModule.singleton.controlAxis == navigationTurnAxis))
		{
			return;
		}
		float num4 = JoystickControl.GetAxis(navigationTurnAxis);
		if (num4 > 0.01f || num4 < -0.01f)
		{
			if (invertNavigationTurnAxis)
			{
				num4 = 0f - num4;
			}
			navigationRig.RotateAround(lookCamera.transform.position, Vector3.up, num4 * 50f * Time.deltaTime / Time.timeScale);
		}
	}

	private void ProcessTimeScale()
	{
		if (Time.timeScale < 1f)
		{
			useInterpolation = true;
		}
		else
		{
			useInterpolation = true;
		}
	}

	protected void DetermineVRRig()
	{
		string loadedDeviceName = VRSettings.get_loadedDeviceName();
		Debug.Log("VR device active is " + VRSettings.get_isDeviceActive());
		Debug.Log("Loaded VR device is " + loadedDeviceName);
		if (loadedDeviceName == "Oculus")
		{
			if (OVRRig != null)
			{
				OVRRig.gameObject.SetActive(value: true);
			}
			if (ViveRig != null)
			{
				ViveRig.gameObject.SetActive(value: false);
			}
			if (OVRCenterCamera != null && centerCameraTarget != null)
			{
				centerCameraTarget.transform.SetParent(OVRCenterCamera.transform);
				centerCameraTarget.transform.localPosition = Vector3.zero;
				centerCameraTarget.transform.localRotation = Quaternion.identity;
				centerCameraTarget.FindCamera();
			}
			if (touchObjectLeft != null)
			{
				Camera component = touchObjectLeft.GetComponent<Camera>();
				if (component != null)
				{
					leftControllerCamera = component;
				}
			}
			if (touchObjectRight != null)
			{
				Camera component2 = touchObjectRight.GetComponent<Camera>();
				if (component2 != null)
				{
					rightControllerCamera = component2;
				}
			}
			return;
		}
		if (OVRRig != null)
		{
			OVRRig.gameObject.SetActive(value: false);
		}
		if (ViveRig != null)
		{
			ViveRig.gameObject.SetActive(value: true);
		}
		if (ViveCenterCamera != null && centerCameraTarget != null)
		{
			centerCameraTarget.transform.SetParent(ViveCenterCamera.transform);
			centerCameraTarget.transform.localPosition = Vector3.zero;
			centerCameraTarget.transform.localRotation = Quaternion.identity;
			centerCameraTarget.FindCamera();
		}
		if (viveObjectLeft != null)
		{
			Camera component3 = viveObjectLeft.GetComponent<Camera>();
			if (component3 != null)
			{
				leftControllerCamera = component3;
			}
			else
			{
				Debug.LogError("Could not find camera on left controller");
			}
		}
		if (viveObjectRight != null)
		{
			Camera component4 = viveObjectRight.GetComponent<Camera>();
			if (component4 != null)
			{
				rightControllerCamera = component4;
			}
			else
			{
				Debug.LogError("Could not find camera on right controller");
			}
		}
	}

	private void Awake()
	{
		_singleton = this;
		DetermineVRRig();
	}

	private IEnumerator DelayStart()
	{
		onStartScene = true;
		yield return null;
		yield return null;
		yield return null;
		StartScene();
	}

	private void Start()
	{
		castRay = default(Ray);
		_navigationForwardAxisEnabled = navigationForwardAxisEnabled;
		_navigationSideAxisEnabled = navigationSideAxisEnabled;
		_navigationUpAxisEnabled = navigationUpAxisEnabled;
		_navigationTurnAxisEnabled = navigationTurnAxisEnabled;
		InitUI();
		InitAtoms();
		InitTargets();
		if (enableStartScene)
		{
			StartCoroutine(DelayStart());
		}
	}

	private void Update()
	{
		if (Time.unscaledDeltaTime > 0.015f)
		{
			DebugHUD.Alert1();
		}
		if (CameraTarget.centerTarget != null && CameraTarget.centerTarget.targetCamera != null)
		{
			lookCamera = CameraTarget.centerTarget.targetCamera;
		}
		PrepControllers();
		ProcessTimeScale();
		ProcessMotionControllerNavigation();
		ProcessControllerNavigation();
		if (!onStartScene)
		{
			ProcessUI();
			if (selectMode == SelectMode.Off || selectMode == SelectMode.Targets || selectMode == SelectMode.FilteredTargets)
			{
				ProcessCommonTargetSelection();
				ProcessMotionControllerTargetSelection();
			}
			else
			{
				ProcessViveControllerSelect();
			}
			ProcessTabControl();
			ProcessSave();
		}
	}
}
