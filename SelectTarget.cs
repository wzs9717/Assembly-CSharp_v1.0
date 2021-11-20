using UnityEngine;

[ExecuteInEditMode]
public class SelectTarget : MonoBehaviour
{
	public static bool useGlobalAlpha;

	public static float globalAlpha = 0.5f;

	public string selectionName;

	public Color normalColor = new Color(0f, 1f, 0f, 0.5f);

	public Color highlightColor = new Color(1f, 1f, 0f, 0.5f);

	public Material material;

	public float meshScale = 0.5f;

	public float highlightedScale = 1f;

	public float unhighlightedScale = 1f;

	public Mesh mesh;

	public bool highlighted;

	private Material localMaterial;

	private void Awake()
	{
		if (material != null)
		{
			localMaterial = Object.Instantiate(material);
		}
	}

	private void Update()
	{
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		float num = meshScale;
		num = ((!highlighted) ? (num * unhighlightedScale) : (num * highlightedScale));
		Vector3 s = new Vector3(num, num, num);
		Matrix4x4 identity = Matrix4x4.identity;
		identity.SetTRS(Vector3.zero, Quaternion.identity, s);
		Matrix4x4 matrix = localToWorldMatrix * identity;
		if (useGlobalAlpha)
		{
			highlightColor.a = globalAlpha;
			normalColor.a = globalAlpha;
		}
		if (mesh != null && localMaterial != null)
		{
			if (highlighted)
			{
				localMaterial.color = highlightColor;
			}
			else
			{
				localMaterial.color = normalColor;
			}
			Graphics.DrawMesh(mesh, matrix, localMaterial, base.gameObject.layer, null, 0, null, castShadows: false, receiveShadows: false);
		}
	}
}
