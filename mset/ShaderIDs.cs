using UnityEngine;

namespace mset
{
	public class ShaderIDs
	{
		public int specCubeIBL = -1;

		public int skyCubeIBL = -1;

		public int skyMatrix = -1;

		public int invSkyMatrix = -1;

		public int skySize = -1;

		public int skyMin = -1;

		public int skyMax = -1;

		public int[] SH;

		public int exposureIBL = -1;

		public int exposureLM = -1;

		public int oldExposureIBL = -1;

		public int blendWeightIBL = -1;

		private bool _valid;

		public bool valid => _valid;

		public ShaderIDs()
		{
			SH = new int[9];
		}

		public void Link()
		{
			Link(string.Empty);
		}

		public void Link(string postfix)
		{
			specCubeIBL = Shader.PropertyToID("_SpecCubeIBL" + postfix);
			skyCubeIBL = Shader.PropertyToID("_SkyCubeIBL" + postfix);
			skyMatrix = Shader.PropertyToID("_SkyMatrix" + postfix);
			invSkyMatrix = Shader.PropertyToID("_InvSkyMatrix" + postfix);
			skyMin = Shader.PropertyToID("_SkyMin" + postfix);
			skyMax = Shader.PropertyToID("_SkyMax" + postfix);
			exposureIBL = Shader.PropertyToID("_ExposureIBL" + postfix);
			exposureLM = Shader.PropertyToID("_ExposureLM" + postfix);
			for (int i = 0; i < 9; i++)
			{
				SH[i] = Shader.PropertyToID("_SH" + i + postfix);
			}
			blendWeightIBL = Shader.PropertyToID("_BlendWeightIBL");
			_valid = true;
		}
	}
}
