using UnityEngine;
using UnityEngine.SceneManagement;

namespace mset
{
	public class Sky : MonoBehaviour
	{
		[SerializeField]
		private Texture specularCube;

		[SerializeField]
		private Texture skyboxCube;

		public bool IsProbe;

		[SerializeField]
		private Bounds dimensions = new Bounds(Vector3.zero, Vector3.one);

		private bool _dirty;

		[SerializeField]
		private float masterIntensity = 1f;

		[SerializeField]
		private float skyIntensity = 1f;

		[SerializeField]
		private float specIntensity = 1f;

		[SerializeField]
		private float diffIntensity = 1f;

		[SerializeField]
		private float camExposure = 1f;

		[SerializeField]
		private float specIntensityLM = 1f;

		[SerializeField]
		private float diffIntensityLM = 1f;

		[SerializeField]
		private bool hdrSky = true;

		[SerializeField]
		private bool hdrSpec = true;

		[SerializeField]
		private bool linearSpace = true;

		[SerializeField]
		private bool autoDetectColorSpace = true;

		[SerializeField]
		private bool hasDimensions;

		public SHEncoding SH = new SHEncoding();

		public SHEncodingFile CustomSH;

		private Matrix4x4 skyMatrix = Matrix4x4.identity;

		private Matrix4x4 invMatrix = Matrix4x4.identity;

		private Vector4 exposures = Vector4.one;

		private Vector4 exposuresLM = Vector4.one;

		private Vector4 skyMin = -Vector4.one;

		private Vector4 skyMax = Vector4.one;

		private ShaderIDs[] blendIDs = new ShaderIDs[2]
		{
			new ShaderIDs(),
			new ShaderIDs()
		};

		private static MaterialPropertyBlock propBlock;

		[SerializeField]
		private Cubemap _blackCube;

		[SerializeField]
		private Material _SkyboxMaterial;

		private static bool internalProjectionSupport;

		private static bool internalBlendingSupport;

		private Material projMaterial;

		public Texture SpecularCube
		{
			get
			{
				return specularCube;
			}
			set
			{
				specularCube = value;
			}
		}

		public Texture SkyboxCube
		{
			get
			{
				return skyboxCube;
			}
			set
			{
				skyboxCube = value;
			}
		}

		public Bounds Dimensions
		{
			get
			{
				return dimensions;
			}
			set
			{
				_dirty = true;
				dimensions = value;
			}
		}

		public bool Dirty
		{
			get
			{
				return _dirty;
			}
			set
			{
				_dirty = value;
			}
		}

		public float MasterIntensity
		{
			get
			{
				return masterIntensity;
			}
			set
			{
				_dirty = true;
				masterIntensity = value;
			}
		}

		public float SkyIntensity
		{
			get
			{
				return skyIntensity;
			}
			set
			{
				_dirty = true;
				skyIntensity = value;
			}
		}

		public float SpecIntensity
		{
			get
			{
				return specIntensity;
			}
			set
			{
				_dirty = true;
				specIntensity = value;
			}
		}

		public float DiffIntensity
		{
			get
			{
				return diffIntensity;
			}
			set
			{
				_dirty = true;
				diffIntensity = value;
			}
		}

		public float CamExposure
		{
			get
			{
				return camExposure;
			}
			set
			{
				_dirty = true;
				camExposure = value;
			}
		}

		public float SpecIntensityLM
		{
			get
			{
				return specIntensityLM;
			}
			set
			{
				_dirty = true;
				specIntensityLM = value;
			}
		}

		public float DiffIntensityLM
		{
			get
			{
				return diffIntensityLM;
			}
			set
			{
				_dirty = true;
				diffIntensityLM = value;
			}
		}

		public bool HDRSky
		{
			get
			{
				return hdrSky;
			}
			set
			{
				_dirty = true;
				hdrSky = value;
			}
		}

		public bool HDRSpec
		{
			get
			{
				return hdrSpec;
			}
			set
			{
				_dirty = true;
				hdrSpec = value;
			}
		}

		public bool LinearSpace
		{
			get
			{
				return linearSpace;
			}
			set
			{
				_dirty = true;
				linearSpace = value;
			}
		}

		public bool AutoDetectColorSpace
		{
			get
			{
				return autoDetectColorSpace;
			}
			set
			{
				_dirty = true;
				autoDetectColorSpace = value;
			}
		}

		public bool HasDimensions
		{
			get
			{
				return hasDimensions;
			}
			set
			{
				_dirty = true;
				hasDimensions = value;
			}
		}

		private Cubemap blackCube
		{
			get
			{
				if (_blackCube == null)
				{
					_blackCube = Resources.Load<Cubemap>("blackCube");
				}
				return _blackCube;
			}
		}

		private Material SkyboxMaterial
		{
			get
			{
				if (_SkyboxMaterial == null)
				{
					_SkyboxMaterial = Resources.Load<Material>("skyboxMat");
				}
				return _SkyboxMaterial;
			}
		}

		private static Material[] getTargetMaterials(Renderer target)
		{
			SkyAnchor component = target.gameObject.GetComponent<SkyAnchor>();
			if (component != null)
			{
				return component.materials;
			}
			return target.sharedMaterials;
		}

		public void Apply()
		{
			Apply(0);
		}

		public void Apply(int blendIndex)
		{
			ShaderIDs bids = blendIDs[blendIndex];
			ApplyGlobally(bids);
		}

		public void Apply(Renderer target)
		{
			Apply(target, 0);
		}

		public void Apply(Renderer target, int blendIndex)
		{
			if ((bool)target && base.enabled && base.gameObject.activeInHierarchy)
			{
				ApplyFast(target, blendIndex);
			}
		}

		public void ApplyFast(Renderer target, int blendIndex)
		{
			if (propBlock == null)
			{
				propBlock = new MaterialPropertyBlock();
			}
			target.GetPropertyBlock(propBlock);
			ApplyToBlock(ref propBlock, blendIDs[blendIndex]);
			target.SetPropertyBlock(propBlock);
		}

		public void Apply(Material target)
		{
			Apply(target, 0);
		}

		public void Apply(Material target, int blendIndex)
		{
			if ((bool)target && base.enabled && base.gameObject.activeInHierarchy)
			{
				ApplyToMaterial(target, blendIDs[blendIndex]);
			}
		}

		private void ApplyToBlock(ref MaterialPropertyBlock block, ShaderIDs bids)
		{
			block.SetVector(bids.exposureIBL, exposures);
			block.SetVector(bids.exposureLM, exposuresLM);
			block.SetMatrix(bids.skyMatrix, skyMatrix);
			block.SetMatrix(bids.invSkyMatrix, invMatrix);
			block.SetVector(bids.skyMin, skyMin);
			block.SetVector(bids.skyMax, skyMax);
			if ((bool)specularCube)
			{
				block.SetTexture(bids.specCubeIBL, specularCube);
			}
			else
			{
				block.SetTexture(bids.specCubeIBL, blackCube);
			}
			block.SetVector(bids.SH[0], SH.cBuffer[0]);
			block.SetVector(bids.SH[1], SH.cBuffer[1]);
			block.SetVector(bids.SH[2], SH.cBuffer[2]);
			block.SetVector(bids.SH[3], SH.cBuffer[3]);
			block.SetVector(bids.SH[4], SH.cBuffer[4]);
			block.SetVector(bids.SH[5], SH.cBuffer[5]);
			block.SetVector(bids.SH[6], SH.cBuffer[6]);
			block.SetVector(bids.SH[7], SH.cBuffer[7]);
			block.SetVector(bids.SH[8], SH.cBuffer[8]);
		}

		private void ApplyToMaterial(Material mat, ShaderIDs bids)
		{
			mat.SetVector(bids.exposureIBL, exposures);
			mat.SetVector(bids.exposureLM, exposuresLM);
			mat.SetMatrix(bids.skyMatrix, skyMatrix);
			mat.SetMatrix(bids.invSkyMatrix, invMatrix);
			mat.SetVector(bids.skyMin, skyMin);
			mat.SetVector(bids.skyMax, skyMax);
			if ((bool)specularCube)
			{
				mat.SetTexture(bids.specCubeIBL, specularCube);
			}
			else
			{
				mat.SetTexture(bids.specCubeIBL, blackCube);
			}
			if ((bool)skyboxCube)
			{
				mat.SetTexture(bids.skyCubeIBL, skyboxCube);
			}
			for (int i = 0; i < 9; i++)
			{
				mat.SetVector(bids.SH[i], SH.cBuffer[i]);
			}
		}

		private void ApplySkyTransform(ShaderIDs bids)
		{
			Shader.SetGlobalMatrix(bids.skyMatrix, skyMatrix);
			Shader.SetGlobalMatrix(bids.invSkyMatrix, invMatrix);
			Shader.SetGlobalVector(bids.skyMin, skyMin);
			Shader.SetGlobalVector(bids.skyMax, skyMax);
		}

		private void ApplyGlobally(ShaderIDs bids)
		{
			Shader.SetGlobalMatrix(bids.skyMatrix, skyMatrix);
			Shader.SetGlobalMatrix(bids.invSkyMatrix, invMatrix);
			Shader.SetGlobalVector(bids.skyMin, skyMin);
			Shader.SetGlobalVector(bids.skyMax, skyMax);
			Shader.SetGlobalVector(bids.exposureIBL, exposures);
			Shader.SetGlobalVector(bids.exposureLM, exposuresLM);
			Shader.SetGlobalFloat("_EmissionLM", 1f);
			Shader.SetGlobalVector("_UniformOcclusion", Vector4.one);
			if ((bool)specularCube)
			{
				Shader.SetGlobalTexture(bids.specCubeIBL, specularCube);
			}
			else
			{
				Shader.SetGlobalTexture(bids.specCubeIBL, blackCube);
			}
			if ((bool)skyboxCube)
			{
				Shader.SetGlobalTexture(bids.skyCubeIBL, skyboxCube);
			}
			for (int i = 0; i < 9; i++)
			{
				Shader.SetGlobalVector(bids.SH[i], SH.cBuffer[i]);
			}
		}

		public static void ScrubGlobalKeywords()
		{
			Shader.DisableKeyword("MARMO_SKY_BLEND_ON");
			Shader.DisableKeyword("MARMO_SKY_BLEND_OFF");
			Shader.DisableKeyword("MARMO_BOX_PROJECTION_ON");
			Shader.DisableKeyword("MARMO_BOX_PROJECTION_OFF");
			Shader.DisableKeyword("MARMO_TERRAIN_BLEND_ON");
			Shader.DisableKeyword("MARMO_TERRAIN_BLEND_OFF");
		}

		public static void ScrubKeywords(Material[] materials)
		{
			foreach (Material material in materials)
			{
				if (material != null)
				{
					material.DisableKeyword("MARMO_SKY_BLEND_ON");
					material.DisableKeyword("MARMO_SKY_BLEND_OFF");
					material.DisableKeyword("MARMO_BOX_PROJECTION_ON");
					material.DisableKeyword("MARMO_BOX_PROJECTION_OFF");
					material.DisableKeyword("MARMO_TERRAIN_BLEND_ON");
					material.DisableKeyword("MARMO_TERRAIN_BLEND_OFF");
				}
			}
		}

		public static void EnableProjectionSupport(bool enable)
		{
			if (enable)
			{
				Shader.DisableKeyword("MARMO_BOX_PROJECTION_OFF");
			}
			else
			{
				Shader.EnableKeyword("MARMO_BOX_PROJECTION_OFF");
			}
			internalProjectionSupport = enable;
		}

		public static void EnableGlobalProjection(bool enable)
		{
			if (internalProjectionSupport)
			{
				if (enable)
				{
					Shader.EnableKeyword("MARMO_BOX_PROJECTION_ON");
				}
				else
				{
					Shader.DisableKeyword("MARMO_BOX_PROJECTION_ON");
				}
			}
		}

		public static void EnableProjection(Renderer target, Material[] mats, bool enable)
		{
			if (!internalProjectionSupport || mats == null)
			{
				return;
			}
			if (enable)
			{
				foreach (Material material in mats)
				{
					if ((bool)material)
					{
						material.EnableKeyword("MARMO_BOX_PROJECTION_ON");
						material.DisableKeyword("MARMO_BOX_PROJECTION_OFF");
					}
				}
				return;
			}
			foreach (Material material2 in mats)
			{
				if ((bool)material2)
				{
					material2.DisableKeyword("MARMO_BOX_PROJECTION_ON");
					material2.EnableKeyword("MARMO_BOX_PROJECTION_OFF");
				}
			}
		}

		public static void EnableProjection(Material mat, bool enable)
		{
			if (internalProjectionSupport)
			{
				if (enable)
				{
					mat.EnableKeyword("MARMO_BOX_PROJECTION_ON");
					mat.DisableKeyword("MARMO_BOX_PROJECTION_OFF");
				}
				else
				{
					mat.DisableKeyword("MARMO_BOX_PROJECTION_ON");
					mat.EnableKeyword("MARMO_BOX_PROJECTION_OFF");
				}
			}
		}

		public static void EnableBlendingSupport(bool enable)
		{
			if (enable)
			{
				Shader.DisableKeyword("MARMO_SKY_BLEND_OFF");
			}
			else
			{
				Shader.EnableKeyword("MARMO_SKY_BLEND_OFF");
			}
			internalBlendingSupport = enable;
		}

		public static void EnableTerrainBlending(bool enable)
		{
			if (internalBlendingSupport)
			{
				if (enable)
				{
					Shader.EnableKeyword("MARMO_TERRAIN_BLEND_ON");
					Shader.DisableKeyword("MARMO_TERRAIN_BLEND_OFF");
				}
				else
				{
					Shader.DisableKeyword("MARMO_TERRAIN_BLEND_ON");
					Shader.EnableKeyword("MARMO_TERRAIN_BLEND_OFF");
				}
			}
		}

		public static void EnableGlobalBlending(bool enable)
		{
			if (internalBlendingSupport)
			{
				if (enable)
				{
					Shader.EnableKeyword("MARMO_SKY_BLEND_ON");
				}
				else
				{
					Shader.DisableKeyword("MARMO_SKY_BLEND_ON");
				}
			}
		}

		public static void EnableBlending(Renderer target, Material[] mats, bool enable)
		{
			if (!internalBlendingSupport || mats == null)
			{
				return;
			}
			if (enable)
			{
				foreach (Material material in mats)
				{
					if ((bool)material)
					{
						material.EnableKeyword("MARMO_SKY_BLEND_ON");
						material.DisableKeyword("MARMO_SKY_BLEND_OFF");
					}
				}
				return;
			}
			foreach (Material material2 in mats)
			{
				if ((bool)material2)
				{
					material2.DisableKeyword("MARMO_SKY_BLEND_ON");
					material2.EnableKeyword("MARMO_SKY_BLEND_OFF");
				}
			}
		}

		public static void EnableBlending(Material mat, bool enable)
		{
			if (internalBlendingSupport)
			{
				if (enable)
				{
					mat.EnableKeyword("MARMO_SKY_BLEND_ON");
					mat.DisableKeyword("MARMO_SKY_BLEND_OFF");
				}
				else
				{
					mat.DisableKeyword("MARMO_SKY_BLEND_ON");
					mat.EnableKeyword("MARMO_SKY_BLEND_OFF");
				}
			}
		}

		public static void SetBlendWeight(float weight)
		{
			Shader.SetGlobalFloat("_BlendWeightIBL", weight);
		}

		public static void SetBlendWeight(Renderer target, float weight)
		{
			if (propBlock == null)
			{
				propBlock = new MaterialPropertyBlock();
			}
			target.GetPropertyBlock(propBlock);
			propBlock.SetFloat("_BlendWeightIBL", weight);
			target.SetPropertyBlock(propBlock);
		}

		public static void SetBlendWeight(Material mat, float weight)
		{
			mat.SetFloat("_BlendWeightIBL", weight);
		}

		public static void SetUniformOcclusion(Renderer target, float diffuse, float specular)
		{
			if (target != null)
			{
				Vector4 one = Vector4.one;
				one.x = diffuse;
				one.y = specular;
				Material[] targetMaterials = getTargetMaterials(target);
				Material[] array = targetMaterials;
				foreach (Material material in array)
				{
					material.SetVector("_UniformOcclusion", one);
				}
			}
		}

		public void SetCustomExposure(float diffInt, float specInt, float skyInt, float camExpo)
		{
			SetCustomExposure(null, diffInt, specInt, skyInt, camExpo);
		}

		public void SetCustomExposure(Renderer target, float diffInt, float specInt, float skyInt, float camExpo)
		{
			Vector4 result = Vector4.one;
			ComputeExposureVector(ref result, diffInt, specInt, skyInt, camExpo);
			if (target == null)
			{
				Shader.SetGlobalVector(blendIDs[0].exposureIBL, result);
				return;
			}
			Material[] targetMaterials = getTargetMaterials(target);
			Material[] array = targetMaterials;
			foreach (Material material in array)
			{
				material.SetVector(blendIDs[0].exposureIBL, result);
			}
		}

		public void ToggleChildLights(bool enable)
		{
			Light[] componentsInChildren = GetComponentsInChildren<Light>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = enable;
			}
		}

		private void UpdateSkySize()
		{
			if (HasDimensions)
			{
				skyMin = Dimensions.center - Dimensions.extents;
				skyMax = Dimensions.center + Dimensions.extents;
				Vector3 localScale = base.transform.localScale;
				skyMin.x *= localScale.x;
				skyMin.y *= localScale.y;
				skyMin.z *= localScale.z;
				skyMax.x *= localScale.x;
				skyMax.y *= localScale.y;
				skyMax.z *= localScale.z;
			}
			else
			{
				skyMax = Vector4.one * 100000f;
				skyMin = Vector4.one * -100000f;
			}
		}

		private void UpdateSkyTransform()
		{
			skyMatrix.SetTRS(base.transform.position, base.transform.rotation, Vector3.one);
			invMatrix = skyMatrix.inverse;
		}

		private void ComputeExposureVector(ref Vector4 result, float diffInt, float specInt, float skyInt, float camExpo)
		{
			result.x = masterIntensity * diffInt;
			result.y = masterIntensity * specInt;
			result.z = masterIntensity * skyInt * camExpo;
			result.w = camExpo;
			float num = 6f;
			if (linearSpace)
			{
				num = Mathf.Pow(num, 2.2f);
			}
			if (!hdrSpec)
			{
				result.y /= num;
			}
			if (!hdrSky)
			{
				result.z /= num;
			}
		}

		private void UpdateExposures()
		{
			ComputeExposureVector(ref exposures, diffIntensity, specIntensity, skyIntensity, camExposure);
			exposuresLM.x = diffIntensityLM;
			exposuresLM.y = specIntensityLM;
		}

		private void UpdatePropertyIDs()
		{
			blendIDs[0].Link();
			blendIDs[1].Link("1");
		}

		public void Awake()
		{
			UpdatePropertyIDs();
			propBlock = new MaterialPropertyBlock();
		}

		private void Reset()
		{
			skyMatrix = (invMatrix = Matrix4x4.identity);
			exposures = Vector4.one;
			exposuresLM = Vector4.one;
			specularCube = (skyboxCube = null);
			masterIntensity = (skyIntensity = (specIntensity = (diffIntensity = 1f)));
			hdrSky = (hdrSpec = false);
		}

		private void OnEnable()
		{
			if (SH == null)
			{
				SH = new SHEncoding();
			}
			if (CustomSH != null)
			{
				SH.copyFrom(CustomSH.SH);
			}
			SH.copyToBuffer();
			SceneManager.sceneLoaded += SceneStart;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= SceneStart;
		}

		private void SceneStart(Scene scene, LoadSceneMode mode)
		{
			UpdateExposures();
			UpdateSkyTransform();
			UpdateSkySize();
		}

		private void Start()
		{
			UpdateExposures();
			UpdateSkyTransform();
			UpdateSkySize();
		}

		private void Update()
		{
			if (base.transform.hasChanged)
			{
				Dirty = true;
				UpdateSkyTransform();
				UpdateSkySize();
				base.transform.hasChanged = false;
			}
			UpdateExposures();
		}

		private void OnDestroy()
		{
			SH = null;
			_blackCube = null;
			specularCube = null;
			skyboxCube = null;
		}

		public void DrawProjectionCube(Vector3 center, Vector3 radius)
		{
			if (projMaterial == null)
			{
				projMaterial = Resources.Load<Material>("projectionMat");
				if (!projMaterial)
				{
					Debug.LogError("Failed to find projectionMat material in Resources folder!");
				}
			}
			Vector4 one = Vector4.one;
			one.z = CamExposure;
			one *= masterIntensity;
			ShaderIDs shaderIDs = blendIDs[0];
			projMaterial.color = new Color(0.7f, 0.7f, 0.7f, 1f);
			projMaterial.SetVector(shaderIDs.skyMin, -Dimensions.extents);
			projMaterial.SetVector(shaderIDs.skyMax, Dimensions.extents);
			projMaterial.SetVector(shaderIDs.exposureIBL, one);
			projMaterial.SetTexture(shaderIDs.skyCubeIBL, specularCube);
			projMaterial.SetMatrix(shaderIDs.skyMatrix, skyMatrix);
			projMaterial.SetMatrix(shaderIDs.invSkyMatrix, invMatrix);
			projMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(base.transform.localToWorldMatrix);
			GLUtil.DrawCube(center, -radius);
			GL.End();
			GL.PopMatrix();
		}

		private void OnTriggerEnter(Collider other)
		{
			if ((bool)other.GetComponent<Renderer>())
			{
				Apply(other.GetComponent<Renderer>(), 0);
			}
		}

		private void OnPostRender()
		{
		}
	}
}
