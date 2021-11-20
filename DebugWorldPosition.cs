using UnityEngine;

[ExecuteInEditMode]
public class DebugWorldPosition : MonoBehaviour
{
	public Vector3 worldPosition;

	public Vector3 startingMatrixWorldPosition;

	private void Update()
	{
		worldPosition = base.transform.position;
	}

	private void Start()
	{
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		startingMatrixWorldPosition.x = localToWorldMatrix.m03;
		startingMatrixWorldPosition.y = localToWorldMatrix.m13;
		startingMatrixWorldPosition.z = localToWorldMatrix.m23;
	}
}
