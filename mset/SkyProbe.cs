using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace mset
{
	public class SkyProbe
	{
		public RenderTexture cubeRT;

		public int maxExponent = 512;

		public Vector4 exposures = Vector4.one;

		public bool generateMipChain = true;

		public bool highestMipIsMirror = true;

		public float convolutionScale = 1f;

		public RenderingPath renderPath = RenderingPath.Forward;

		private static int sampleCount = 128;

		private static Vector4[] randomValues;

		public SkyProbe()
		{
			buildRandomValueTable();
		}

		public static void buildRandomValueTable()
		{
			if (randomValues == null)
			{
				float num = sampleCount;
				randomValues = new Vector4[sampleCount];
				float[] array = new float[sampleCount];
				for (int i = 0; i < sampleCount; i++)
				{
					randomValues[i] = default(Vector4);
					array[i] = (randomValues[i].x = (float)(i + 1) / num);
				}
				int num2 = sampleCount;
				for (int j = 0; j < sampleCount; j++)
				{
					int num3 = UnityEngine.Random.Range(0, num2 - 1);
					float num4 = array[num3];
					array[num3] = array[--num2];
					randomValues[j].y = num4;
					randomValues[j].z = Mathf.Cos((float)Math.PI * 2f * num4);
					randomValues[j].w = Mathf.Sin((float)Math.PI * 2f * num4);
				}
			}
		}

		public static void bindRandomValueTable(Material mat, string paramName, int inputFaceSize)
		{
			for (int i = 0; i < sampleCount; i++)
			{
				mat.SetVector(paramName + i, randomValues[i]);
			}
			float f = (float)(inputFaceSize * inputFaceSize) / (float)sampleCount;
			f = 0.5f * Mathf.Log(f, 2f) + 0.5f;
			mat.SetFloat("_ImportantLog", f);
		}

		public static void buildRandomValueCode()
		{
		}

		public void blur(Cubemap targetCube, Texture sourceCube, bool dstRGBM, bool srcRGBM, bool linear)
		{
			if (!(sourceCube == null) && !(targetCube == null))
			{
				GameObject gameObject = new GameObject("_temp_probe");
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				gameObject.SetActive(value: true);
				Camera camera = gameObject.AddComponent<Camera>();
				camera.renderingPath = renderPath;
				camera.useOcclusionCulling = false;
				Material material = new Material(Shader.Find("Hidden/Marmoset/RGBM Cube"));
				Matrix4x4 identity = Matrix4x4.identity;
				int num = maxExponent;
				bool flag = generateMipChain;
				maxExponent = 8 * num;
				generateMipChain = false;
				convolve_internal(targetCube, sourceCube, dstRGBM, srcRGBM, linear, camera, material, identity);
				convolve_internal(targetCube, targetCube, dstRGBM, dstRGBM, linear, camera, material, identity);
				maxExponent = num;
				generateMipChain = flag;
				SkyManager skyManager = SkyManager.Get();
				if ((bool)skyManager)
				{
					skyManager.GlobalSky = skyManager.GlobalSky;
				}
				UnityEngine.Object.DestroyImmediate(material);
				UnityEngine.Object.DestroyImmediate(gameObject);
			}
		}

		public void convolve(Cubemap targetCube, Texture sourceCube, bool dstRGBM, bool srcRGBM, bool linear)
		{
			if (!(targetCube == null))
			{
				GameObject gameObject = new GameObject("_temp_probe");
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				gameObject.SetActive(value: true);
				Camera camera = gameObject.AddComponent<Camera>();
				camera.renderingPath = renderPath;
				camera.useOcclusionCulling = false;
				Material material = new Material(Shader.Find("Hidden/Marmoset/RGBM Cube"));
				Matrix4x4 identity = Matrix4x4.identity;
				copy_internal(targetCube, sourceCube, dstRGBM, srcRGBM, linear, camera, material, identity);
				int num = maxExponent;
				maxExponent = 2 * num;
				convolve_internal(targetCube, sourceCube, dstRGBM, srcRGBM, linear, camera, material, identity);
				maxExponent = 8 * num;
				convolve_internal(targetCube, targetCube, dstRGBM, dstRGBM, linear, camera, material, identity);
				maxExponent = num;
				SkyManager skyManager = SkyManager.Get();
				if ((bool)skyManager)
				{
					skyManager.GlobalSky = skyManager.GlobalSky;
				}
				UnityEngine.Object.DestroyImmediate(material);
				UnityEngine.Object.DestroyImmediate(gameObject);
			}
		}

		public bool capture(Texture targetCube, Vector3 position, Quaternion rotation, bool HDR, bool linear, bool convolve)
		{
			if (targetCube == null)
			{
				return false;
			}
			bool flag = false;
			if (cubeRT == null)
			{
				flag = true;
				cubeRT = RenderTexture.GetTemporary(targetCube.width, targetCube.width, 24, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
				cubeRT.Release();
				cubeRT.dimension = TextureDimension.Cube;
				cubeRT.useMipMap = true;
				cubeRT.autoGenerateMips = true;
				cubeRT.Create();
				if (!cubeRT.IsCreated() && !cubeRT.Create())
				{
					cubeRT = RenderTexture.GetTemporary(targetCube.width, targetCube.width, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
					cubeRT.Release();
					cubeRT.dimension = TextureDimension.Cube;
					cubeRT.useMipMap = true;
					cubeRT.autoGenerateMips = true;
					cubeRT.Create();
				}
			}
			if (!cubeRT.IsCreated() && !cubeRT.Create())
			{
				return false;
			}
			GameObject gameObject = null;
			Camera camera = null;
			gameObject = new GameObject("_temp_probe");
			camera = gameObject.AddComponent<Camera>();
			SkyManager skyManager = SkyManager.Get();
			if ((bool)skyManager && (bool)skyManager.ProbeCamera)
			{
				camera.CopyFrom(skyManager.ProbeCamera);
			}
			else if ((bool)Camera.main)
			{
				camera.CopyFrom(Camera.main);
			}
			camera.renderingPath = renderPath;
			camera.useOcclusionCulling = false;
			camera.allowHDR = true;
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.SetActive(value: true);
			gameObject.transform.position = position;
			Shader.SetGlobalVector("_UniformOcclusion", exposures);
			camera.RenderToCubemap(cubeRT);
			Shader.SetGlobalVector("_UniformOcclusion", Vector4.one);
			Matrix4x4 identity = Matrix4x4.identity;
			identity.SetTRS(position, rotation, Vector3.one);
			Material material = new Material(Shader.Find("Hidden/Marmoset/RGBM Cube"));
			bool dstRGBM = HDR;
			bool srcRGBM = false;
			copy_internal(targetCube, cubeRT, dstRGBM, srcRGBM, linear, camera, material, identity);
			if (convolve)
			{
				convolve_internal(targetCube, cubeRT, HDR, srcRGBM: false, linear, camera, material, identity);
			}
			if ((bool)skyManager)
			{
				skyManager.GlobalSky = skyManager.GlobalSky;
			}
			UnityEngine.Object.DestroyImmediate(material);
			UnityEngine.Object.DestroyImmediate(gameObject);
			if (flag)
			{
				UnityEngine.Object.DestroyImmediate(cubeRT);
			}
			return true;
		}

		private static void toggleKeywordPair(string on, string off, bool yes)
		{
			if (yes)
			{
				Shader.EnableKeyword(on);
				Shader.DisableKeyword(off);
			}
			else
			{
				Shader.EnableKeyword(off);
				Shader.DisableKeyword(on);
			}
		}

		private static void toggleKeywordPair(Material mat, string on, string off, bool yes)
		{
			if (yes)
			{
				mat.EnableKeyword(on);
				mat.DisableKeyword(off);
			}
			else
			{
				mat.EnableKeyword(off);
				mat.DisableKeyword(on);
			}
		}

		private void copy_internal(Texture dstCube, Texture srcCube, bool dstRGBM, bool srcRGBM, bool linear, Camera cam, Material skyMat, Matrix4x4 matrix)
		{
			bool allowHDR = cam.allowHDR;
			CameraClearFlags clearFlags = cam.clearFlags;
			int cullingMask = cam.cullingMask;
			cam.clearFlags = CameraClearFlags.Skybox;
			cam.cullingMask = 0;
			cam.allowHDR = !dstRGBM;
			skyMat.name = "Internal HDR to RGBM Skybox";
			skyMat.shader = Shader.Find("Hidden/Marmoset/RGBM Cube");
			toggleKeywordPair("MARMO_RGBM_INPUT_ON", "MARMO_RGBM_INPUT_OFF", srcRGBM);
			toggleKeywordPair("MARMO_RGBM_OUTPUT_ON", "MARMO_RGBM_OUTPUT_OFF", dstRGBM);
			skyMat.SetMatrix("_SkyMatrix", matrix);
			skyMat.SetTexture("_CubeHDR", srcCube);
			Material skybox = RenderSettings.skybox;
			RenderSettings.skybox = skyMat;
			RenderTexture renderTexture = dstCube as RenderTexture;
			Cubemap cubemap = dstCube as Cubemap;
			if ((bool)renderTexture)
			{
				cam.RenderToCubemap(renderTexture);
			}
			else if ((bool)cubemap)
			{
				cam.RenderToCubemap(cubemap);
			}
			cam.allowHDR = allowHDR;
			cam.clearFlags = clearFlags;
			cam.cullingMask = cullingMask;
			RenderSettings.skybox = skybox;
		}

		private void convolve_internal(Texture dstTex, Texture srcCube, bool dstRGBM, bool srcRGBM, bool linear, Camera cam, Material skyMat, Matrix4x4 matrix)
		{
			bool allowHDR = cam.allowHDR;
			CameraClearFlags clearFlags = cam.clearFlags;
			int cullingMask = cam.cullingMask;
			cam.clearFlags = CameraClearFlags.Skybox;
			cam.cullingMask = 0;
			cam.allowHDR = !dstRGBM;
			skyMat.name = "Internal Convolve Skybox";
			skyMat.shader = Shader.Find("Hidden/Marmoset/RGBM Convolve");
			toggleKeywordPair("MARMO_RGBM_INPUT_ON", "MARMO_RGBM_INPUT_OFF", srcRGBM);
			toggleKeywordPair("MARMO_RGBM_OUTPUT_ON", "MARMO_RGBM_OUTPUT_OFF", dstRGBM);
			skyMat.SetMatrix("_SkyMatrix", matrix);
			skyMat.SetTexture("_CubeHDR", srcCube);
			bindRandomValueTable(skyMat, "_PhongRands", srcCube.width);
			Material skybox = RenderSettings.skybox;
			RenderSettings.skybox = skyMat;
			Cubemap cubemap = dstTex as Cubemap;
			RenderTexture renderTexture = dstTex as RenderTexture;
			if ((bool)cubemap)
			{
				if (generateMipChain)
				{
					int num = QPow.Log2i(cubemap.width) - 1;
					for (int i = (highestMipIsMirror ? 1 : 0); i < num; i++)
					{
						int ext = 1 << num - i;
						float value = QPow.clampedDownShift(maxExponent, (!highestMipIsMirror) ? i : (i - 1), 1);
						skyMat.SetFloat("_SpecularExp", value);
						skyMat.SetFloat("_SpecularScale", convolutionScale);
						Cubemap cubemap2 = new Cubemap(ext, cubemap.format, mipmap: false);
						cam.RenderToCubemap(cubemap2);
						for (int j = 0; j < 6; j++)
						{
							CubemapFace face = (CubemapFace)j;
							cubemap.SetPixels(cubemap2.GetPixels(face), face, i);
						}
						UnityEngine.Object.DestroyImmediate(cubemap2);
					}
					cubemap.Apply(updateMipmaps: false);
				}
				else
				{
					skyMat.SetFloat("_SpecularExp", maxExponent);
					skyMat.SetFloat("_SpecularScale", convolutionScale);
					cam.RenderToCubemap(cubemap);
				}
			}
			else if ((bool)renderTexture)
			{
				skyMat.SetFloat("_SpecularExp", maxExponent);
				skyMat.SetFloat("_SpecularScale", convolutionScale);
				cam.RenderToCubemap(renderTexture);
			}
			cam.clearFlags = clearFlags;
			cam.cullingMask = cullingMask;
			cam.allowHDR = allowHDR;
			RenderSettings.skybox = skybox;
		}
	}
}
