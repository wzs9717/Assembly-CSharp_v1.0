using mset;
using UnityEngine;

public class ProbeDemo : MonoBehaviour
{
	private bool spinning = true;

	public float guiAlpha = 0.8f;

	private float helpAlpha = 0.25f;

	public Transform mesh;

	public Texture2D helpTex;

	private Color helpColor = new Color(1f, 1f, 1f, 0f);

	private float targetExposure = 1f;

	private Vector3 angularVel = new Vector3(0f, 6f, 0f);

	private void Start()
	{
	}

	private void Update()
	{
		SkyManager skyManager = SkyManager.Get();
		if ((bool)skyManager && (bool)skyManager.GlobalSky)
		{
			Sky globalSky = skyManager.GlobalSky;
			if (Input.GetKeyDown(KeyCode.S))
			{
				spinning = !spinning;
			}
			if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadEquals))
			{
				targetExposure = Mathf.Min(targetExposure + 0.2f, 2f);
			}
			if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				targetExposure = Mathf.Max(0.05f, targetExposure - 0.2f);
			}
			if (Mathf.Abs(globalSky.CamExposure - targetExposure) > 0.01f)
			{
				globalSky.CamExposure = 0.95f * globalSky.CamExposure + 0.05f * targetExposure;
			}
			else
			{
				globalSky.CamExposure = targetExposure;
			}
		}
	}

	private void FixedUpdate()
	{
		if (spinning && (bool)mesh)
		{
			mesh.transform.Rotate(angularVel * Time.fixedDeltaTime);
		}
	}

	private void OnGUI()
	{
		Rect pixelRect = GetComponent<Camera>().pixelRect;
		helpColor.a = guiAlpha;
		if ((bool)helpTex)
		{
			pixelRect.width = 0.75f * (float)helpTex.width;
			pixelRect.height = 0.75f * (float)helpTex.height;
			pixelRect.y = -50f;
			pixelRect.x = (float)GetComponent<Camera>().pixelWidth - pixelRect.width;
			Rect rect = pixelRect;
			rect.x += 0.5f * rect.width;
			rect.width *= 0.5f;
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.y = (float)GetComponent<Camera>().pixelHeight - mousePosition.y;
			if (rect.Contains(mousePosition))
			{
				helpAlpha = Mathf.Lerp(helpAlpha, 1f, 0.01f);
			}
			else
			{
				helpAlpha = Mathf.Lerp(helpAlpha, 0.25f, 0.01f);
			}
			helpColor.a = helpAlpha * guiAlpha;
			GUI.color = helpColor;
			GUI.DrawTexture(pixelRect, helpTex);
		}
	}
}
