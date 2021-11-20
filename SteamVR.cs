using System;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VR;
using Valve.VR;

public class SteamVR : IDisposable
{
	private static bool _enabled = true;

	private static SteamVR _instance;

	public static bool[] connected = new bool[16];

	public ETextureType textureType;

	public static bool active => _instance != null;

	public static bool enabled
	{
		get
		{
			if (!VRSettings.get_enabled())
			{
				enabled = false;
			}
			return _enabled;
		}
		set
		{
			_enabled = value;
			if (!_enabled)
			{
				SafeDispose();
			}
		}
	}

	public static SteamVR instance
	{
		get
		{
			if (!enabled)
			{
				return null;
			}
			if (_instance == null)
			{
				_instance = CreateInstance();
				if (_instance == null)
				{
					_enabled = false;
				}
			}
			return _instance;
		}
	}

	public static bool usingNativeSupport => VRDevice.GetNativePtr() != IntPtr.Zero;

	public CVRSystem hmd { get; private set; }

	public CVRCompositor compositor { get; private set; }

	public CVROverlay overlay { get; private set; }

	public static bool initializing { get; private set; }

	public static bool calibrating { get; private set; }

	public static bool outOfRange { get; private set; }

	public float sceneWidth { get; private set; }

	public float sceneHeight { get; private set; }

	public float aspect { get; private set; }

	public float fieldOfView { get; private set; }

	public Vector2 tanHalfFov { get; private set; }

	public VRTextureBounds_t[] textureBounds { get; private set; }

	public SteamVR_Utils.RigidTransform[] eyes { get; private set; }

	public string hmd_TrackingSystemName => GetStringProperty(ETrackedDeviceProperty.Prop_TrackingSystemName_String);

	public string hmd_ModelNumber => GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String);

	public string hmd_SerialNumber => GetStringProperty(ETrackedDeviceProperty.Prop_SerialNumber_String);

	public float hmd_SecondsFromVsyncToPhotons => GetFloatProperty(ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float);

	public float hmd_DisplayFrequency => GetFloatProperty(ETrackedDeviceProperty.Prop_DisplayFrequency_Float);

	private SteamVR()
	{
		hmd = OpenVR.System;
		Debug.Log("Connected to " + hmd_TrackingSystemName + ":" + hmd_SerialNumber);
		compositor = OpenVR.Compositor;
		overlay = OpenVR.Overlay;
		uint pnWidth = 0u;
		uint pnHeight = 0u;
		hmd.GetRecommendedRenderTargetSize(ref pnWidth, ref pnHeight);
		sceneWidth = pnWidth;
		sceneHeight = pnHeight;
		float pfLeft = 0f;
		float pfRight = 0f;
		float pfTop = 0f;
		float pfBottom = 0f;
		hmd.GetProjectionRaw(EVREye.Eye_Left, ref pfLeft, ref pfRight, ref pfTop, ref pfBottom);
		float pfLeft2 = 0f;
		float pfRight2 = 0f;
		float pfTop2 = 0f;
		float pfBottom2 = 0f;
		hmd.GetProjectionRaw(EVREye.Eye_Right, ref pfLeft2, ref pfRight2, ref pfTop2, ref pfBottom2);
		tanHalfFov = new Vector2(Mathf.Max(0f - pfLeft, pfRight, 0f - pfLeft2, pfRight2), Mathf.Max(0f - pfTop, pfBottom, 0f - pfTop2, pfBottom2));
		textureBounds = new VRTextureBounds_t[2];
		textureBounds[0].uMin = 0.5f + 0.5f * pfLeft / tanHalfFov.x;
		textureBounds[0].uMax = 0.5f + 0.5f * pfRight / tanHalfFov.x;
		textureBounds[0].vMin = 0.5f - 0.5f * pfBottom / tanHalfFov.y;
		textureBounds[0].vMax = 0.5f - 0.5f * pfTop / tanHalfFov.y;
		textureBounds[1].uMin = 0.5f + 0.5f * pfLeft2 / tanHalfFov.x;
		textureBounds[1].uMax = 0.5f + 0.5f * pfRight2 / tanHalfFov.x;
		textureBounds[1].vMin = 0.5f - 0.5f * pfBottom2 / tanHalfFov.y;
		textureBounds[1].vMax = 0.5f - 0.5f * pfTop2 / tanHalfFov.y;
		sceneWidth /= Mathf.Max(textureBounds[0].uMax - textureBounds[0].uMin, textureBounds[1].uMax - textureBounds[1].uMin);
		sceneHeight /= Mathf.Max(textureBounds[0].vMax - textureBounds[0].vMin, textureBounds[1].vMax - textureBounds[1].vMin);
		aspect = tanHalfFov.x / tanHalfFov.y;
		fieldOfView = 2f * Mathf.Atan(tanHalfFov.y) * 57.29578f;
		eyes = new SteamVR_Utils.RigidTransform[2]
		{
			new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Left)),
			new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Right))
		};
		switch (SystemInfo.graphicsDeviceType)
		{
		case GraphicsDeviceType.OpenGLES2:
		case GraphicsDeviceType.OpenGLES3:
		case GraphicsDeviceType.OpenGLCore:
			textureType = ETextureType.OpenGL;
			break;
		case GraphicsDeviceType.Vulkan:
			textureType = ETextureType.Vulkan;
			break;
		default:
			textureType = ETextureType.DirectX;
			break;
		}
		SteamVR_Events.Initializing.Listen(OnInitializing);
		SteamVR_Events.Calibrating.Listen(OnCalibrating);
		SteamVR_Events.OutOfRange.Listen(OnOutOfRange);
		SteamVR_Events.DeviceConnected.Listen(OnDeviceConnected);
		SteamVR_Events.NewPoses.Listen(OnNewPoses);
	}

	private static SteamVR CreateInstance()
	{
		try
		{
			EVRInitError peError = EVRInitError.None;
			if (!usingNativeSupport)
			{
				Debug.Log("OpenVR initialization failed.  Ensure 'Virtual Reality Supported' is checked in Player Settings, and OpenVR is added to the list of Virtual Reality SDKs.");
				return null;
			}
			OpenVR.GetGenericInterface("IVRCompositor_020", ref peError);
			if (peError != 0)
			{
				ReportError(peError);
				return null;
			}
			OpenVR.GetGenericInterface("IVROverlay_014", ref peError);
			if (peError != 0)
			{
				ReportError(peError);
				return null;
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return null;
		}
		return new SteamVR();
	}

	private static void ReportError(EVRInitError error)
	{
		switch (error)
		{
		case EVRInitError.None:
			break;
		case EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime:
			Debug.Log("SteamVR Initialization Failed!  Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
			break;
		case EVRInitError.Init_VRClientDLLNotFound:
			Debug.Log("SteamVR drivers not found!  They can be installed via Steam under Library > Tools.  Visit http://steampowered.com to install Steam.");
			break;
		case EVRInitError.Driver_RuntimeOutOfDate:
			Debug.Log("SteamVR Initialization Failed!  Make sure device's runtime is up to date.");
			break;
		default:
			Debug.Log(OpenVR.GetStringForHmdError(error));
			break;
		}
	}

	public string GetTrackedDeviceString(uint deviceId)
	{
		ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
		uint stringTrackedDeviceProperty = hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, null, 0u, ref pError);
		if (stringTrackedDeviceProperty > 1)
		{
			StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
			hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, stringBuilder, stringTrackedDeviceProperty, ref pError);
			return stringBuilder.ToString();
		}
		return null;
	}

	private string GetStringProperty(ETrackedDeviceProperty prop)
	{
		ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
		uint stringTrackedDeviceProperty = hmd.GetStringTrackedDeviceProperty(0u, prop, null, 0u, ref pError);
		if (stringTrackedDeviceProperty > 1)
		{
			StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
			hmd.GetStringTrackedDeviceProperty(0u, prop, stringBuilder, stringTrackedDeviceProperty, ref pError);
			return stringBuilder.ToString();
		}
		return (pError == ETrackedPropertyError.TrackedProp_Success) ? "<unknown>" : pError.ToString();
	}

	private float GetFloatProperty(ETrackedDeviceProperty prop)
	{
		ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
		return hmd.GetFloatTrackedDeviceProperty(0u, prop, ref pError);
	}

	private void OnInitializing(bool initializing)
	{
		SteamVR.initializing = initializing;
	}

	private void OnCalibrating(bool calibrating)
	{
		SteamVR.calibrating = calibrating;
	}

	private void OnOutOfRange(bool outOfRange)
	{
		SteamVR.outOfRange = outOfRange;
	}

	private void OnDeviceConnected(int i, bool connected)
	{
		SteamVR.connected[i] = connected;
	}

	private void OnNewPoses(TrackedDevicePose_t[] poses)
	{
		ref SteamVR_Utils.RigidTransform reference = ref eyes[0];
		reference = new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Left));
		ref SteamVR_Utils.RigidTransform reference2 = ref eyes[1];
		reference2 = new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Right));
		for (int i = 0; i < poses.Length; i++)
		{
			bool bDeviceIsConnected = poses[i].bDeviceIsConnected;
			if (bDeviceIsConnected != connected[i])
			{
				SteamVR_Events.DeviceConnected.Send(i, bDeviceIsConnected);
			}
		}
		if ((long)poses.Length > 0L)
		{
			ETrackingResult eTrackingResult = poses[0].eTrackingResult;
			bool flag = eTrackingResult == ETrackingResult.Uninitialized;
			if (flag != initializing)
			{
				SteamVR_Events.Initializing.Send(flag);
			}
			bool flag2 = eTrackingResult == ETrackingResult.Calibrating_InProgress || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
			if (flag2 != calibrating)
			{
				SteamVR_Events.Calibrating.Send(flag2);
			}
			bool flag3 = eTrackingResult == ETrackingResult.Running_OutOfRange || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
			if (flag3 != outOfRange)
			{
				SteamVR_Events.OutOfRange.Send(flag3);
			}
		}
	}

	~SteamVR()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		SteamVR_Events.Initializing.Remove(OnInitializing);
		SteamVR_Events.Calibrating.Remove(OnCalibrating);
		SteamVR_Events.OutOfRange.Remove(OnOutOfRange);
		SteamVR_Events.DeviceConnected.Remove(OnDeviceConnected);
		SteamVR_Events.NewPoses.Remove(OnNewPoses);
		_instance = null;
	}

	public static void SafeDispose()
	{
		if (_instance != null)
		{
			_instance.Dispose();
		}
	}
}
