using System;
using UnityEngine;
using UnityEngine.VR;

public class OVROverlay : MonoBehaviour
{
	public enum OverlayShape
	{
		Quad = 0,
		Cylinder = 1,
		Cubemap = 2,
		OffcenterCubemap = 4
	}

	public enum OverlayType
	{
		None,
		Underlay,
		Overlay,
		OverlayShowLod
	}

	private const int maxInstances = 15;

	internal static OVROverlay[] instances = new OVROverlay[15];

	public OverlayType currentOverlayType = OverlayType.Overlay;

	public OverlayShape currentOverlayShape;

	public Texture[] textures = new Texture[2];

	private Texture[] cachedTextures = new Texture[2];

	private IntPtr[] texNativePtrs = new IntPtr[2]
	{
		IntPtr.Zero,
		IntPtr.Zero
	};

	private int layerIndex = -1;

	private Renderer rend;

	public void OverrideOverlayTextureInfo(Texture srcTexture, IntPtr nativePtr, VRNode node)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		int num = (((int)node == 1) ? 1 : 0);
		textures[num] = srcTexture;
		cachedTextures[num] = srcTexture;
		texNativePtrs[num] = nativePtr;
	}

	private void Awake()
	{
		Debug.Log("Overlay Awake");
		rend = GetComponent<Renderer>();
		for (int i = 0; i < 2; i++)
		{
			if (rend != null && textures[i] == null)
			{
				textures[i] = rend.material.mainTexture;
			}
			if (textures[i] != null)
			{
				cachedTextures[i] = textures[i];
				ref IntPtr reference = ref texNativePtrs[i];
				reference = textures[i].GetNativeTexturePtr();
			}
		}
	}

	private void OnEnable()
	{
		if (!OVRManager.isHmdPresent)
		{
			base.enabled = false;
			return;
		}
		OnDisable();
		for (int i = 0; i < 15; i++)
		{
			if (instances[i] == null || instances[i] == this)
			{
				layerIndex = i;
				instances[i] = this;
				break;
			}
		}
	}

	private void OnDisable()
	{
		if (layerIndex != -1)
		{
			OVRPlugin.SetOverlayQuad(onTop: true, headLocked: false, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, OVRPose.identity.ToPosef(), Vector3.one.ToVector3f(), layerIndex);
			instances[layerIndex] = null;
		}
		layerIndex = -1;
	}

	private void OnRenderObject()
	{
		if (!Camera.current.CompareTag("MainCamera") || Camera.current.cameraType != CameraType.Game || layerIndex == -1 || currentOverlayType == OverlayType.None)
		{
			return;
		}
		if (currentOverlayShape == OverlayShape.Cylinder || currentOverlayShape == OverlayShape.OffcenterCubemap)
		{
			Debug.LogWarning(string.Concat("Overlay shape ", currentOverlayShape, " is not supported on current platform"));
		}
		for (int i = 0; i < 2; i++)
		{
			if (i >= textures.Length)
			{
				continue;
			}
			if (textures[i] != cachedTextures[i])
			{
				cachedTextures[i] = textures[i];
				if (cachedTextures[i] != null)
				{
					ref IntPtr reference = ref texNativePtrs[i];
					reference = cachedTextures[i].GetNativeTexturePtr();
				}
			}
			if (currentOverlayShape == OverlayShape.Cubemap && textures[i] != null && textures[i].GetType() != typeof(Cubemap))
			{
				Debug.LogError("Need Cubemap texture for cube map overlay");
				return;
			}
		}
		if (cachedTextures[0] == null || texNativePtrs[0] == IntPtr.Zero)
		{
			return;
		}
		bool onTop = currentOverlayType == OverlayType.Overlay;
		bool flag = false;
		Transform parent = base.transform;
		while (parent != null && !flag)
		{
			flag |= parent == Camera.current.transform;
			parent = parent.parent;
		}
		OVRPose oVRPose = ((!flag) ? base.transform.ToTrackingSpacePose() : base.transform.ToHeadSpacePose());
		Vector3 lossyScale = base.transform.lossyScale;
		for (int j = 0; j < 3; j++)
		{
			lossyScale[j] /= Camera.current.transform.lossyScale[j];
		}
		if (currentOverlayShape == OverlayShape.Cubemap)
		{
			oVRPose.position = Camera.current.transform.position;
		}
		if (currentOverlayShape == OverlayShape.OffcenterCubemap)
		{
			oVRPose.position = base.transform.position;
			if (oVRPose.position.magnitude > 1f)
			{
				Debug.LogWarning("your cube map center offset's magnitude is greater than 1, which will cause some cube map pixel always invisible .");
			}
		}
		if (currentOverlayShape == OverlayShape.Cylinder)
		{
			float num = lossyScale.x / lossyScale.z / (float)Math.PI * 180f;
			if (num > 180f)
			{
				Debug.LogError("Cylinder overlay's arc angle has to be below 180 degree, current arc angle is " + num + " degree.");
				return;
			}
		}
		bool flag2 = OVRPlugin.SetOverlayQuad(onTop, flag, texNativePtrs[0], texNativePtrs[1], IntPtr.Zero, oVRPose.flipZ().ToPosef(), lossyScale.ToVector3f(), layerIndex, (OVRPlugin.OverlayShape)currentOverlayShape);
		if ((bool)rend)
		{
			rend.enabled = !flag2;
		}
	}
}
