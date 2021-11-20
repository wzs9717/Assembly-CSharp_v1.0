using UnityEngine;

public class FollowRotationSelective : MonoBehaviour
{
	public Transform follow;

	public Mesh mesh;

	public Material material;

	public Quaternion2Angles.RotationOrder rotationOrder;

	public float xrot;

	public float yrot;

	public float zrot;

	private Vector3[] meshVerts;

	private Matrix4x4 bindpose;

	private int numVertices;

	private Vector3[] vertices;

	private Mesh drawMesh;

	private void Start()
	{
		if (mesh != null)
		{
			bindpose = follow.localToWorldMatrix;
			drawMesh = new Mesh();
			meshVerts = mesh.vertices;
			numVertices = meshVerts.Length;
			vertices = new Vector3[numVertices];
			for (int i = 0; i < numVertices; i++)
			{
				ref Vector3 reference = ref vertices[i];
				reference = bindpose.MultiplyPoint3x4(meshVerts[i]);
			}
			drawMesh.vertices = vertices;
			drawMesh.normals = mesh.normals;
			drawMesh.uv = mesh.uv;
			drawMesh.triangles = mesh.triangles;
		}
	}

	private void SkinMesh()
	{
		Matrix4x4 localToWorldMatrix = follow.localToWorldMatrix;
		for (int i = 0; i < numVertices; i++)
		{
			ref Vector3 reference = ref vertices[i];
			reference = localToWorldMatrix.MultiplyPoint3x4(meshVerts[i]);
		}
		drawMesh.vertices = vertices;
	}

	private void DrawMesh()
	{
		if (mesh != null && material != null)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				Graphics.DrawMesh(drawMesh, identity, material, 0, null, i, null, castShadows: false, receiveShadows: false);
			}
		}
	}

	private void Update()
	{
		Vector3 angles = Quaternion2Angles.GetAngles(follow.localRotation, rotationOrder);
		xrot = angles.x * 57.29578f;
		yrot = angles.y * 57.29578f;
		zrot = angles.z * 57.29578f;
		SkinMesh();
		DrawMesh();
	}
}
