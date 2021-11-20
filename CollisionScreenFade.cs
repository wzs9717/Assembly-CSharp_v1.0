using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CollisionScreenFade : MonoBehaviour
{
	public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 0f);

	public Shader fadeShader;

	public float fadeAlpha = 0.2f;

	public float fadeOutRate = 3f;

	public float fadeInRate = 3f;

	public bool useBlur = true;

	public float maxBlurSpread = 1f;

	private Material fadeMaterial;

	private bool isFading;

	private bool isFadingOut;

	private Blur blurEffect;

	private void Awake()
	{
		fadeMaterial = ((!(fadeShader != null)) ? new Material(Shader.Find("Transparent/Diffuse")) : new Material(fadeShader));
		blurEffect = GetComponent<Blur>();
	}

	private void OnDestroy()
	{
		if (fadeMaterial != null)
		{
			Object.Destroy(fadeMaterial);
		}
	}

	private void OnCollisionStay()
	{
		isFadingOut = true;
	}

	private void OnCollisionExit()
	{
		isFadingOut = false;
	}

	private void FixedUpdate()
	{
		if (isFadingOut)
		{
			float num = fadeOutRate * Time.fixedDeltaTime / Time.timeScale;
			fadeColor.a += num * fadeAlpha;
			fadeColor.a = Mathf.Clamp(fadeColor.a, 0f, fadeAlpha);
			fadeMaterial.color = fadeColor;
			if (useBlur && blurEffect != null)
			{
				blurEffect.enabled = true;
				blurEffect.blurSpread += num * maxBlurSpread;
				blurEffect.blurSpread = Mathf.Clamp(blurEffect.blurSpread, 0f, maxBlurSpread);
			}
			isFading = true;
		}
		else if (fadeColor.a != 0f)
		{
			float num2 = fadeInRate * Time.fixedDeltaTime / Time.timeScale;
			fadeColor.a -= num2 * fadeAlpha;
			fadeColor.a = Mathf.Clamp01(fadeColor.a);
			fadeMaterial.color = fadeColor;
			if (useBlur && blurEffect != null)
			{
				blurEffect.enabled = true;
				blurEffect.blurSpread -= num2 * maxBlurSpread;
				blurEffect.blurSpread = Mathf.Clamp(blurEffect.blurSpread, 0f, maxBlurSpread);
			}
			isFading = true;
		}
		else
		{
			if (useBlur && blurEffect != null)
			{
				blurEffect.blurSpread = 0f;
				blurEffect.enabled = false;
			}
			isFading = false;
		}
	}

	private void OnPostRender()
	{
		if (isFading)
		{
			fadeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(fadeMaterial.color);
			GL.Begin(7);
			GL.Vertex3(0f, 0f, -12f);
			GL.Vertex3(0f, 1f, -12f);
			GL.Vertex3(1f, 1f, -12f);
			GL.Vertex3(1f, 0f, -12f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
