using mset;
using UnityEngine;

public class SkySwab : MonoBehaviour
{
	public Sky targetSky;

	public SkyManager manager;

	private Vector3 scale = new Vector3(1f, 1.01f, 1f);

	private Quaternion baseRot = Quaternion.identity;

	public Vector3 bigScale = new Vector3(1.2f, 1.21f, 1.2f);

	public Vector3 hoverScale = new Vector3(1f, 1f, 1f);

	public Vector3 littleScale = new Vector3(0.75f, 0.76f, 0.75f);

	private void Start()
	{
		baseRot = base.transform.localRotation;
		scale = littleScale;
		manager = SkyManager.Get();
		if (!manager)
		{
			Debug.LogError("Failed to find SkyManager in scene. You'll probably want one of those.");
		}
	}

	private void OnMouseDown()
	{
		if ((bool)targetSky)
		{
			manager.BlendToGlobalSky(targetSky);
		}
	}

	private void OnMouseOver()
	{
		scale = hoverScale;
	}

	private void OnMouseExit()
	{
		scale = littleScale;
	}

	private void Update()
	{
		if (manager.GlobalSky == targetSky)
		{
			base.transform.Rotate(0f, 200f * Time.deltaTime, 0f);
			base.transform.localScale = bigScale;
		}
		else
		{
			base.transform.localRotation = baseRot;
			base.transform.localScale = scale;
		}
	}
}
