using UnityEngine;

public class AdjustColliderFromMorphedDAZMesh : MonoBehaviour
{
	public DAZMesh mesh;

	public bool adjustPrimaryAxis;

	public int primaryAxisVertexIndex1;

	public int primaryAxisVertexIndex2;

	public int secondaryAxisVertexIndex1;

	public bool adjustSecondaryAxis;


	public int secondaryAxisVertexIndex2;

	protected CapsuleCollider capsuleCollider;

	private void Awake()
	{
		capsuleCollider = GetComponent<CapsuleCollider>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (mesh != null)
		{
			if (adjustPrimaryAxis)
			{
			}
			if (!adjustSecondaryAxis)
			{
			}
		}
	}
}
