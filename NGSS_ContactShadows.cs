using UnityEngine;
using UnityEngine.Rendering;

[ImageEffectAllowedInSceneView]
[ExecuteInEditMode]
public class NGSS_ContactShadows : MonoBehaviour
{
	public Light mainDirectionalLight;

	[Range(0f, 3f)]
	public float shadowsSoftness = 1f;

	[Range(1f, 4f)]
	public float shadowsDistance = 2f;

	[Range(0.1f, 4f)]
	public float shadowsFade = 1f;

	[Range(0f, 1f)]
	public float rayWidth = 0.1f;

	[Range(16f, 128f)]
	public int raySamples = 64;

	private CommandBuffer blendShadowsCB;

	private CommandBuffer computeShadowsCB;

	private bool isInitialized;

	private Camera _mCamera;

	private Material _mMaterial;

	private Camera mCamera
	{
		get
		{
			if (_mCamera == null)
			{
				_mCamera = GetComponent<Camera>();
				if (_mCamera == null)
				{
					_mCamera = Camera.main;
				}
				if (_mCamera == null)
				{
					Debug.LogError("NGSS Error: No MainCamera found, please provide one.", this);
				}
				else
				{
					_mCamera.depthTextureMode |= DepthTextureMode.Depth;
				}
			}
			return _mCamera;
		}
	}

	private Material mMaterial
	{
		get
		{
			if (_mMaterial == null)
			{
				_mMaterial = new Material(Shader.Find("Hidden/NGSS_ContactShadows"));
			}
			return _mMaterial;
		}
	}

	private void AddCommandBuffers()
	{
		computeShadowsCB = new CommandBuffer
		{
			name = "NGSS ContactShadows: Compute"
		};
		blendShadowsCB = new CommandBuffer
		{
			name = "NGSS ContactShadows: Mix"
		};
		bool flag = mCamera.renderingPath == RenderingPath.Forward;
		if ((bool)mCamera)
		{
			CommandBuffer[] commandBuffers = mCamera.GetCommandBuffers(flag ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting);
			foreach (CommandBuffer commandBuffer in commandBuffers)
			{
				if (commandBuffer.name == computeShadowsCB.name)
				{
					return;
				}
			}
			mCamera.AddCommandBuffer(flag ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting, computeShadowsCB);
		}
		if (!mainDirectionalLight)
		{
			return;
		}
		CommandBuffer[] commandBuffers2 = mainDirectionalLight.GetCommandBuffers(LightEvent.AfterScreenspaceMask);
		foreach (CommandBuffer commandBuffer2 in commandBuffers2)
		{
			if (commandBuffer2.name == blendShadowsCB.name)
			{
				return;
			}
		}
		mainDirectionalLight.AddCommandBuffer(LightEvent.AfterScreenspaceMask, blendShadowsCB);
	}

	private void RemoveCommandBuffers()
	{
		_mMaterial = null;
		bool flag = mCamera.renderingPath == RenderingPath.Forward;
		if ((bool)mCamera)
		{
			mCamera.RemoveCommandBuffer(flag ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting, computeShadowsCB);
		}
		if ((bool)mainDirectionalLight)
		{
			mainDirectionalLight.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, blendShadowsCB);
		}
		isInitialized = false;
	}

	private void Init()
	{
		if (!isInitialized)
		{
			if (mCamera.renderingPath != RenderingPath.Forward && mCamera.renderingPath != RenderingPath.DeferredShading)
			{
				Debug.LogWarning("Please set your camera rendering path to either Forward or Defferred and re-add this component to your main camera again.", this);
				base.enabled = false;
				Object.DestroyImmediate(this);
				return;
			}
			AddCommandBuffers();
			int num = Shader.PropertyToID("NGSS_ContactShadowRT");
			int num2 = Shader.PropertyToID("NGSS_DepthSourceRT");
			computeShadowsCB.GetTemporaryRT(num, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
			computeShadowsCB.GetTemporaryRT(num2, -1, -1, 0, FilterMode.Point, RenderTextureFormat.RFloat);
			computeShadowsCB.Blit(num, num2, mMaterial, 0);
			computeShadowsCB.Blit(num2, num, mMaterial, 1);
			computeShadowsCB.Blit(num, num2, mMaterial, 2);
			blendShadowsCB.Blit(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CurrentActive, mMaterial, 3);
			computeShadowsCB.SetGlobalTexture("NGSS_ContactShadowsTexture", num2);
			isInitialized = true;
		}
	}

	private void OnEnable()
	{
		if ((bool)mainDirectionalLight)
		{
			Init();
		}
	}

	private void OnDisable()
	{
		if (isInitialized)
		{
			RemoveCommandBuffers();
		}
	}

	private void OnApplicationQuit()
	{
		if (isInitialized)
		{
			RemoveCommandBuffers();
		}
	}

	private void OnPreRender()
	{
		if ((bool)mainDirectionalLight)
		{
			Init();
		}
		if (isInitialized && !(mainDirectionalLight == null))
		{
			mMaterial.SetVector("LightDir", mCamera.transform.InverseTransformDirection(mainDirectionalLight.transform.forward));
			mMaterial.SetFloat("ShadowsSoftness", shadowsSoftness);
			mMaterial.SetFloat("ShadowsDistance", shadowsDistance);
			mMaterial.SetFloat("ShadowsFade", shadowsFade);
			mMaterial.SetFloat("RayWidth", rayWidth);
			mMaterial.SetInt("RaySamples", raySamples);
		}
	}
}
