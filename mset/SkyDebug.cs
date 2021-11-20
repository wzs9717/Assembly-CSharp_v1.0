using UnityEngine;

namespace mset
{
	public class SkyDebug : MonoBehaviour
	{
		public bool printConstantly = true;

		public bool printOnce;

		public bool printToGUI = true;

		public bool printToConsole;

		public bool printInEditor = true;

		public bool printDetails;

		public string debugString = string.Empty;

		private MaterialPropertyBlock block;

		private GUIStyle debugStyle;

		private static int debugCounter;

		private int debugID;

		private void Start()
		{
			debugID = debugCounter;
			debugCounter++;
		}

		private void LateUpdate()
		{
			bool flag = printOnce || printConstantly;
			if ((bool)GetComponent<Renderer>() && flag)
			{
				printOnce = false;
				debugString = GetDebugString();
				if (printToConsole)
				{
					Debug.Log(debugString);
				}
			}
		}

		public string GetDebugString()
		{
			string text = "<b>SkyDebug Info - " + base.name + "</b>\n";
			Material material = null;
			material = ((!Application.isPlaying) ? GetComponent<Renderer>().sharedMaterial : GetComponent<Renderer>().material);
			text = text + material.shader.name + "\n";
			string text2 = text;
			text = text2 + "is supported: " + material.shader.isSupported + "\n";
			ShaderIDs[] array = new ShaderIDs[2]
			{
				new ShaderIDs(),
				new ShaderIDs()
			};
			array[0].Link();
			array[1].Link("1");
			text += "\n<b>Anchor</b>\n";
			SkyAnchor component = GetComponent<SkyAnchor>();
			if (component != null)
			{
				text = text + "Curr. sky: " + component.CurrentSky.name + "\n";
				text = text + "Prev. sky: " + component.PreviousSky.name + "\n";
			}
			else
			{
				text += "none\n";
			}
			text += "\n<b>Property Block</b>\n";
			if (block == null)
			{
				block = new MaterialPropertyBlock();
			}
			block.Clear();
			GetComponent<Renderer>().GetPropertyBlock(block);
			for (int i = 0; i < 2; i++)
			{
				text = text + "Renderer Property block - blend ID " + i;
				if (printDetails)
				{
					text = text + "\nexposureIBL  " + block.GetVector(array[i].exposureIBL);
					text = text + "\nexposureLM   " + block.GetVector(array[i].exposureLM);
					text = text + "\nskyMin       " + block.GetVector(array[i].skyMin);
					text = text + "\nskyMax       " + block.GetVector(array[i].skyMax);
					text += "\ndiffuse SH\n";
					for (int j = 0; j < 4; j++)
					{
						text = string.Concat(text, block.GetVector(array[i].SH[j]), "\n");
					}
					text += "...\n";
				}
				Texture texture = block.GetTexture(array[i].specCubeIBL);
				Texture texture2 = block.GetTexture(array[i].skyCubeIBL);
				text += "\nspecCubeIBL  ";
				text = ((!texture) ? (text + "none") : (text + texture.name));
				text += "\nskyCubeIBL   ";
				text = ((!texture2) ? (text + "none") : (text + texture2.name));
				if (printDetails)
				{
					text = text + "\nskyMatrix\n" + block.GetMatrix(array[i].skyMatrix);
					text = text + "\ninvSkyMatrix\n" + block.GetMatrix(array[i].invSkyMatrix);
				}
				if (i == 0)
				{
					text = text + "\nblendWeightIBL " + block.GetFloat(array[i].blendWeightIBL);
				}
				text += "\n\n";
			}
			return text;
		}

		private void OnDrawGizmosSelected()
		{
			bool flag = printOnce || printConstantly;
			if ((bool)GetComponent<Renderer>() && printInEditor && printToConsole && flag)
			{
				printOnce = false;
				string message = GetDebugString();
				Debug.Log(message);
			}
		}

		private void OnGUI()
		{
			if (printToGUI)
			{
				Rect position = Rect.MinMaxRect(3f, 3f, 360f, 1024f);
				if ((bool)Camera.main)
				{
					position.yMax = Camera.main.pixelHeight;
				}
				position.xMin += (float)debugID * position.width;
				GUI.color = Color.white;
				if (debugStyle == null)
				{
					debugStyle = new GUIStyle();
					debugStyle.richText = true;
				}
				string text = "<color=\"#000\">";
				string text2 = "</color>";
				GUI.TextArea(position, text + debugString + text2, debugStyle);
				text = "<color=\"#FFF\">";
				position.xMin -= 1f;
				position.yMin -= 2f;
				GUI.TextArea(position, text + debugString + text2, debugStyle);
			}
		}
	}
}
