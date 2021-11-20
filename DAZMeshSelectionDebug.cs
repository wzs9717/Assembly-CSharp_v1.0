using UnityEngine;

[ExecuteInEditMode]
public class DAZMeshSelectionDebug : MonoBehaviour
{
	public bool on;

	public float boxSize = 0.01f;

	public float vectorSize = 0.01f;

	public Color vertexColor = Color.blue;

	public Color normalColor = Color.red;

	public Color tangentColor = Color.yellow;

	public bool debugVertex = true;

	public bool debugNormal = true;

	public bool debugTangent = true;

	public DAZMeshSelection dms;

	private void OnEnable()
	{
		dms = GetComponent<DAZMeshSelection>();
	}

	private void debug(int vertId)
	{
		Vector3[] array;
		Vector3[] array2;
		Vector4[] array3;
		if (dms.mesh.drawMorphedUVMappedMesh || dms.mesh.drawInEditorWhenNotPlaying)
		{
			array = dms.mesh.morphedUVVertices;
			array2 = dms.mesh.morphedUVNormals;
			array3 = dms.mesh.morphedUVTangents;
		}
		else if (dms.mesh.drawUVMappedMesh)
		{
			array = dms.mesh.UVVertices;
			array2 = dms.mesh.UVNormals;
			array3 = dms.mesh.UVTangents;
		}
		else
		{
			array = dms.mesh.baseVertices;
			array2 = dms.mesh.baseNormals;
			array3 = null;
		}
		if (debugVertex && vertId < array.Length)
		{
			MyDebug.DrawWireCube(array[vertId], boxSize, vertexColor);
		}
		if (debugNormal && vertId < array.Length)
		{
			Debug.DrawLine(array[vertId], array[vertId] + array2[vertId] * vectorSize, normalColor);
		}
		if (debugTangent && array3 != null && vertId < array.Length)
		{
			Vector3 vector = default(Vector3);
			vector.x = array3[vertId].x;
			vector.y = array3[vertId].y;
			vector.z = array3[vertId].z;
			Debug.DrawLine(array[vertId], array[vertId] + vector * vectorSize, tangentColor);
		}
	}

	private void Update()
	{
		if (!(dms != null) || !on || !Application.isEditor)
		{
			return;
		}
		foreach (int selectedVertex in dms.selectedVertices)
		{
			debug(selectedVertex);
		}
	}
}
