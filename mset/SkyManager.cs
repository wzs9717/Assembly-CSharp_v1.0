using System.Collections.Generic;
using UnityEngine;

namespace mset
{
	[ExecuteInEditMode]
	public class SkyManager : MonoBehaviour
	{
		private static SkyManager _Instance;

		public bool LinearSpace = true;

		[SerializeField]
		private bool _BlendingSupport = true;

		[SerializeField]
		private bool _ProjectionSupport = true;

		public bool GameAutoApply = true;

		public bool EditorAutoApply = true;

		public bool AutoMaterial;

		public int IgnoredLayerMask;

		public int[] _IgnoredLayers;

		public int _IgnoredLayerCount;

		[SerializeField]
		private Sky _GlobalSky;

		[SerializeField]
		private SkyBlender GlobalBlender = new SkyBlender();

		private Sky nextSky;

		private float nextBlendTime;

		private float nextSkipTime;

		public float LocalBlendTime = 0.25f;

		public float GlobalBlendTime = 0.25f;

		private Light[] prevLights;

		private Light[] nextLights;

		private float[] prevIntensities;

		private float[] nextIntensities;

		private Material _SkyboxMaterial;

		[SerializeField]
		private bool _ShowSkybox = true;

		public Camera ProbeCamera;

		private HashSet<Renderer> staticRenderers = new HashSet<Renderer>();

		private HashSet<Renderer> dynamicRenderers = new HashSet<Renderer>();

		private HashSet<Renderer> globalSkyChildren = new HashSet<Renderer>();

		private HashSet<SkyApplicator> skyApplicators = new HashSet<SkyApplicator>();

		private float seekTimer;

		private float lastTimestamp = -1f;

		private int renderCheckIterator;

		private bool firstFrame = true;

		public Sky[] SkiesToProbe;

		public int ProbeExponent = 512;

		public Vector4 ProbeExposures = Vector4.one;

		public bool ProbeWithCubeRT = true;

		public bool ProbeOnlyStatic;

		public bool BlendingSupport
		{
			get
			{
				return _BlendingSupport;
			}
			set
			{
				_BlendingSupport = value;
				Sky.EnableBlendingSupport(value);
				if (!value)
				{
					Sky.EnableTerrainBlending(enable: false);
				}
			}
		}

		public bool ProjectionSupport
		{
			get
			{
				return _ProjectionSupport;
			}
			set
			{
				_ProjectionSupport = value;
				Sky.EnableProjectionSupport(value);
			}
		}

		public Sky GlobalSky
		{
			get
			{
				return _GlobalSky;
			}
			set
			{
				BlendToGlobalSky(value, 0f);
			}
		}

		private Material SkyboxMaterial
		{
			get
			{
				if (_SkyboxMaterial == null)
				{
					_SkyboxMaterial = Resources.Load<Material>("skyboxMat");
					if (!_SkyboxMaterial)
					{
						Debug.LogError("Failed to find skyboxMat material in Resources folder!");
					}
				}
				return _SkyboxMaterial;
			}
		}

		public bool ShowSkybox
		{
			get
			{
				return _ShowSkybox;
			}
			set
			{
				if (value)
				{
					if ((bool)SkyboxMaterial && RenderSettings.skybox != SkyboxMaterial)
					{
						RenderSettings.skybox = SkyboxMaterial;
					}
				}
				else if (RenderSettings.skybox != null && (RenderSettings.skybox == _SkyboxMaterial || RenderSettings.skybox.name == "Internal IBL Skybox"))
				{
					RenderSettings.skybox = null;
				}
				_ShowSkybox = value;
			}
		}

		public static SkyManager Get()
		{
			if (_Instance == null)
			{
				_Instance = Object.FindObjectOfType<SkyManager>();
			}
			return _Instance;
		}

		public void BlendToGlobalSky(Sky next)
		{
			BlendToGlobalSky(next, GlobalBlendTime, 0f);
		}

		public void BlendToGlobalSky(Sky next, float blendTime)
		{
			BlendToGlobalSky(next, blendTime, 0f);
		}

		public void BlendToGlobalSky(Sky next, float blendTime, float skipTime)
		{
			if (next != null)
			{
				nextSky = next;
				nextBlendTime = blendTime;
				nextSkipTime = skipTime;
			}
			_GlobalSky = nextSky;
		}

		private void ResetLightBlend()
		{
			if (nextLights != null)
			{
				for (int i = 0; i < nextLights.Length; i++)
				{
					nextLights[i].intensity = nextIntensities[i];
					nextLights[i].enabled = true;
				}
				nextLights = null;
				nextIntensities = null;
			}
			if (prevLights != null)
			{
				for (int j = 0; j < prevLights.Length; j++)
				{
					prevLights[j].intensity = prevIntensities[j];
					prevLights[j].enabled = false;
				}
				prevLights = null;
				prevIntensities = null;
			}
		}

		private void StartLightBlend(Sky prev, Sky next)
		{
			prevLights = null;
			prevIntensities = null;
			if ((bool)prev)
			{
				prevLights = prev.GetComponentsInChildren<Light>();
				if (prevLights != null && prevLights.Length > 0)
				{
					prevIntensities = new float[prevLights.Length];
					for (int i = 0; i < prevLights.Length; i++)
					{
						prevLights[i].enabled = true;
						prevIntensities[i] = prevLights[i].intensity;
					}
				}
			}
			nextLights = null;
			nextIntensities = null;
			if (!next)
			{
				return;
			}
			nextLights = next.GetComponentsInChildren<Light>();
			if (nextLights != null && nextLights.Length > 0)
			{
				nextIntensities = new float[nextLights.Length];
				for (int j = 0; j < nextLights.Length; j++)
				{
					nextIntensities[j] = nextLights[j].intensity;
					nextLights[j].enabled = true;
					nextLights[j].intensity = 0f;
				}
			}
		}

		private void UpdateLightBlend()
		{
			if (GlobalBlender.IsBlending)
			{
				float blendWeight = GlobalBlender.BlendWeight;
				float num = 1f - blendWeight;
				for (int i = 0; i < prevLights.Length; i++)
				{
					prevLights[i].intensity = num * prevIntensities[i];
				}
				for (int j = 0; j < nextLights.Length; j++)
				{
					nextLights[j].intensity = blendWeight * nextIntensities[j];
				}
			}
			else
			{
				ResetLightBlend();
			}
		}

		private void HandleGlobalSkyChange()
		{
			if (nextSky != null)
			{
				ResetLightBlend();
				if (BlendingSupport && nextBlendTime > 0f)
				{
					Sky currentSky = GlobalBlender.CurrentSky;
					GlobalBlender.BlendTime = nextBlendTime;
					GlobalBlender.BlendToSky(nextSky);
					Sky[] array = Object.FindObjectsOfType<Sky>();
					Sky[] array2 = array;
					foreach (Sky sky in array2)
					{
						sky.ToggleChildLights(enable: false);
					}
					GlobalBlender.SkipTime(nextSkipTime);
					StartLightBlend(currentSky, nextSky);
				}
				else
				{
					GlobalBlender.SnapToSky(nextSky);
					nextSky.Apply(0);
					nextSky.Apply(1);
					Sky[] array3 = Object.FindObjectsOfType<Sky>();
					Sky[] array4 = array3;
					foreach (Sky sky2 in array4)
					{
						sky2.ToggleChildLights(enable: false);
					}
					nextSky.ToggleChildLights(enable: true);
				}
				_GlobalSky = nextSky;
				nextSky = null;
				if (!Application.isPlaying)
				{
					EditorApplySkies(forceApply: true);
				}
			}
			UpdateLightBlend();
		}

		private void Start()
		{
			Sky.ScrubGlobalKeywords();
			_SkyboxMaterial = SkyboxMaterial;
			ShowSkybox = _ShowSkybox;
			BlendingSupport = _BlendingSupport;
			ProjectionSupport = _ProjectionSupport;
			if (_GlobalSky == null)
			{
				_GlobalSky = base.gameObject.GetComponent<Sky>();
			}
			if (_GlobalSky == null)
			{
				_GlobalSky = Object.FindObjectOfType<Sky>();
			}
			GlobalBlender.SnapToSky(_GlobalSky);
		}

		public void RegisterApplicator(SkyApplicator app)
		{
			skyApplicators.Add(app);
			foreach (Renderer dynamicRenderer in dynamicRenderers)
			{
				app.RendererInside(dynamicRenderer);
			}
			foreach (Renderer staticRenderer in staticRenderers)
			{
				app.RendererInside(staticRenderer);
			}
		}

		public void UnregisterApplicator(SkyApplicator app, HashSet<Renderer> renderersToClear)
		{
			skyApplicators.Remove(app);
			foreach (Renderer item in renderersToClear)
			{
				if (_GlobalSky != null)
				{
					_GlobalSky.Apply(item, 0);
				}
			}
		}

		public void UnregisterRenderer(Renderer rend)
		{
			if (!dynamicRenderers.Remove(rend))
			{
				staticRenderers.Remove(rend);
			}
		}

		public void RegisterNewRenderer(Renderer rend)
		{
			if (!rend.gameObject.activeInHierarchy)
			{
				return;
			}
			int num = 1 << rend.gameObject.layer;
			if ((IgnoredLayerMask & num) != 0)
			{
				return;
			}
			if (rend.gameObject.isStatic)
			{
				if (!staticRenderers.Contains(rend))
				{
					staticRenderers.Add(rend);
					ApplyCorrectSky(rend);
				}
			}
			else if (!dynamicRenderers.Contains(rend))
			{
				dynamicRenderers.Add(rend);
				if (rend.GetComponent<SkyAnchor>() == null)
				{
					rend.gameObject.AddComponent(typeof(SkyAnchor));
				}
			}
		}

		public void SeekNewRenderers()
		{
			Renderer[] array = Object.FindObjectsOfType<MeshRenderer>();
			for (int i = 0; i < array.Length; i++)
			{
				RegisterNewRenderer(array[i]);
			}
			array = Object.FindObjectsOfType<SkinnedMeshRenderer>();
			for (int j = 0; j < array.Length; j++)
			{
				RegisterNewRenderer(array[j]);
			}
		}

		public void ApplyCorrectSky(Renderer rend)
		{
			bool flag = false;
			SkyAnchor component = rend.GetComponent<SkyAnchor>();
			if ((bool)component && component.BindType == SkyAnchor.AnchorBindType.TargetSky)
			{
				component.Apply();
				flag = true;
			}
			foreach (SkyApplicator skyApplicator in skyApplicators)
			{
				if (flag)
				{
					skyApplicator.RemoveRenderer(rend);
				}
				else if (skyApplicator.RendererInside(rend))
				{
					flag = true;
				}
			}
			if (!flag && _GlobalSky != null)
			{
				if (component != null)
				{
					if (component.CurrentApplicator != null)
					{
						component.CurrentApplicator.RemoveRenderer(rend);
						component.CurrentApplicator = null;
					}
					component.BlendToGlobalSky(_GlobalSky);
				}
				if (!globalSkyChildren.Contains(rend))
				{
					globalSkyChildren.Add(rend);
				}
			}
			if ((flag || _GlobalSky == null) && globalSkyChildren.Contains(rend))
			{
				globalSkyChildren.Remove(rend);
			}
		}

		public void EditorUpdate(bool forceApply)
		{
			Sky.EnableGlobalProjection(enable: true);
			Sky.EnableBlendingSupport(enable: false);
			Sky.EnableTerrainBlending(enable: false);
			if ((bool)_GlobalSky)
			{
				_GlobalSky.Apply(0);
				_GlobalSky.Apply(1);
				if ((bool)SkyboxMaterial)
				{
					_GlobalSky.Apply(SkyboxMaterial, 0);
					_GlobalSky.Apply(SkyboxMaterial, 1);
				}
				_GlobalSky.Dirty = false;
			}
			HandleGlobalSkyChange();
			if (EditorAutoApply)
			{
				EditorApplySkies(forceApply);
			}
		}

		private void EditorApplySkies(bool forceApply)
		{
			Shader.SetGlobalVector("_UniformOcclusion", Vector4.one);
			SkyApplicator[] apps = Object.FindObjectsOfType<SkyApplicator>();
			object[] renderers = Object.FindObjectsOfType<MeshRenderer>();
			EditorApplyToList(renderers, apps, forceApply);
			renderers = Object.FindObjectsOfType<SkinnedMeshRenderer>();
			EditorApplyToList(renderers, apps, forceApply);
		}

		private void EditorApplyToList(object[] renderers, SkyApplicator[] apps, bool forceApply)
		{
			foreach (object obj in renderers)
			{
				Renderer renderer = (Renderer)obj;
				int num = 1 << renderer.gameObject.layer;
				if ((IgnoredLayerMask & num) != 0 || !renderer.gameObject.activeInHierarchy)
				{
					continue;
				}
				if (forceApply)
				{
					MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
					materialPropertyBlock.Clear();
					renderer.SetPropertyBlock(materialPropertyBlock);
				}
				SkyAnchor skyAnchor = renderer.gameObject.GetComponent<SkyAnchor>();
				if ((bool)skyAnchor && !skyAnchor.enabled)
				{
					skyAnchor = null;
				}
				bool flag = renderer.transform.hasChanged || ((bool)skyAnchor && skyAnchor.HasChanged);
				bool flag2 = false;
				if ((bool)skyAnchor && skyAnchor.BindType == SkyAnchor.AnchorBindType.TargetSky)
				{
					skyAnchor.Apply();
					flag2 = true;
				}
				if (GameAutoApply && !flag2)
				{
					foreach (SkyApplicator skyApplicator in apps)
					{
						if (skyApplicator.gameObject.activeInHierarchy)
						{
							if ((bool)skyApplicator.TargetSky && (forceApply || skyApplicator.HasChanged || skyApplicator.TargetSky.Dirty || flag))
							{
								flag2 |= skyApplicator.ApplyInside(renderer);
								skyApplicator.TargetSky.Dirty = false;
							}
							skyApplicator.HasChanged = false;
						}
					}
				}
				if (!flag2 && (bool)_GlobalSky && (forceApply || _GlobalSky.Dirty || flag))
				{
					_GlobalSky.Apply(renderer, 0);
				}
				renderer.transform.hasChanged = false;
				if ((bool)skyAnchor)
				{
					skyAnchor.HasChanged = false;
				}
			}
			if (forceApply && (bool)_GlobalSky)
			{
				_GlobalSky.Apply(0);
				if ((bool)_SkyboxMaterial)
				{
					_GlobalSky.Apply(_SkyboxMaterial, 0);
				}
				_GlobalSky.Dirty = false;
			}
		}

		public void LateUpdate()
		{
			if (firstFrame && (bool)_GlobalSky)
			{
				firstFrame = false;
				_GlobalSky.Apply(0);
				_GlobalSky.Apply(1);
				if ((bool)_SkyboxMaterial)
				{
					_GlobalSky.Apply(_SkyboxMaterial, 0);
					_GlobalSky.Apply(_SkyboxMaterial, 1);
				}
			}
			float num = 0f;
			if (lastTimestamp > 0f)
			{
				num = Time.realtimeSinceStartup - lastTimestamp;
			}
			lastTimestamp = Time.realtimeSinceStartup;
			seekTimer -= num;
			HandleGlobalSkyChange();
			GameApplySkies(forceApply: false);
		}

		public void GameApplySkies(bool forceApply)
		{
			GlobalBlender.ApplyToTerrain();
			GlobalBlender.Apply();
			if ((bool)_SkyboxMaterial)
			{
				GlobalBlender.Apply(_SkyboxMaterial);
			}
			if (GameAutoApply || forceApply)
			{
				if (seekTimer <= 0f || forceApply)
				{
					SeekNewRenderers();
					seekTimer = 0.5f;
				}
				List<SkyApplicator> list = new List<SkyApplicator>();
				foreach (SkyApplicator skyApplicator in skyApplicators)
				{
					if (skyApplicator == null || skyApplicator.gameObject == null)
					{
						list.Add(skyApplicator);
					}
				}
				foreach (SkyApplicator item in list)
				{
					skyApplicators.Remove(item);
				}
				if (GlobalBlender.IsBlending || GlobalBlender.CurrentSky.Dirty || GlobalBlender.WasBlending(Time.deltaTime))
				{
					foreach (Renderer globalSkyChild in globalSkyChildren)
					{
						if ((bool)globalSkyChild)
						{
							SkyAnchor component = globalSkyChild.GetComponent<SkyAnchor>();
							if (component != null)
							{
								GlobalBlender.Apply(globalSkyChild, component.materials);
							}
						}
					}
				}
				int num = 0;
				int num2 = 0;
				List<Renderer> list2 = new List<Renderer>();
				foreach (Renderer dynamicRenderer in dynamicRenderers)
				{
					num2++;
					if (!forceApply && num2 < renderCheckIterator)
					{
						continue;
					}
					if (dynamicRenderer == null || dynamicRenderer.gameObject == null)
					{
						list2.Add(dynamicRenderer);
					}
					else
					{
						if (!dynamicRenderer.gameObject.activeInHierarchy)
						{
							continue;
						}
						renderCheckIterator++;
						if (!forceApply && num > 50)
						{
							num = 0;
							renderCheckIterator--;
							break;
						}
						SkyAnchor component2 = dynamicRenderer.GetComponent<SkyAnchor>();
						if (component2.HasChanged)
						{
							num++;
							component2.HasChanged = false;
							if (AutoMaterial)
							{
								component2.UpdateMaterials();
							}
							ApplyCorrectSky(dynamicRenderer);
						}
					}
				}
				foreach (Renderer item2 in list2)
				{
					dynamicRenderers.Remove(item2);
				}
				if (renderCheckIterator >= dynamicRenderers.Count)
				{
					renderCheckIterator = 0;
				}
			}
			_GlobalSky.Dirty = false;
		}

		public void ProbeSkies(GameObject[] objects, Sky[] skies, bool probeAll, bool probeIBL)
		{
			int num = 0;
			List<Sky> list = new List<Sky>();
			string text = string.Empty;
			if (skies != null)
			{
				foreach (Sky sky in skies)
				{
					if ((bool)sky)
					{
						if (probeAll || sky.IsProbe)
						{
							list.Add(sky);
							continue;
						}
						num++;
						text = text + sky.name + "\n";
					}
				}
			}
			if (objects != null)
			{
				foreach (GameObject gameObject in objects)
				{
					Sky component = gameObject.GetComponent<Sky>();
					if ((bool)component)
					{
						if (probeAll || component.IsProbe)
						{
							list.Add(component);
							continue;
						}
						num++;
						text = text + component.name + "\n";
					}
				}
			}
			if (num > 0)
			{
			}
			if (list.Count > 0)
			{
				ProbeExposures = ((!probeIBL) ? Vector4.zero : Vector4.one);
				SkiesToProbe = new Sky[list.Count];
				for (int k = 0; k < list.Count; k++)
				{
					SkiesToProbe[k] = list[k];
				}
			}
		}
	}
}
