using UnityEngine;

public class DelayedMeshRenderer : MonoBehaviour
{
	protected MeshFilter meshFilter;

	protected MeshRenderer meshRenderer;

	protected Matrix4x4 lastMatrix;

	private void Start()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		if (meshRenderer != null)
		{
			meshRenderer.enabled = false;
		}
		lastMatrix = base.transform.localToWorldMatrix;
	}

	private void Update()
	{
		if (meshFilter != null && meshRenderer != null)
		{
			Mesh mesh = meshFilter.mesh;
			for (int i = 0; i < meshRenderer.materials.Length; i++)
			{
				Material material = meshRenderer.materials[i];
				if (material != null)
				{
					Graphics.DrawMesh(mesh, lastMatrix, material, base.gameObject.layer, null, i, null, meshRenderer.shadowCastingMode, meshRenderer.receiveShadows);
				}
			}
		}
		lastMatrix = base.transform.localToWorldMatrix;
	}
}
