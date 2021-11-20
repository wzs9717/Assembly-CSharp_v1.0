using UnityEngine;

public class AdjustMaterialFromLight : MonoBehaviour
{
	public Material material;

	public Light lightComponent;

	public float intensityFactor = 1f;

	private void Start()
	{
	}

	private void Update()
	{
		if ((bool)material && (bool)lightComponent)
		{
			Vector4 vector = default(Vector4);
			vector.x = Mathf.Clamp01(lightComponent.color.r * lightComponent.intensity * intensityFactor);
			vector.y = Mathf.Clamp01(lightComponent.color.g * lightComponent.intensity * intensityFactor);
			vector.z = Mathf.Clamp01(lightComponent.color.b * lightComponent.intensity * intensityFactor);
			vector.w = 1f;
			if (material.HasProperty("_GlowColor"))
			{
				material.SetColor("_GlowColor", vector);
			}
			else
			{
				material.color = vector;
			}
		}
	}
}
