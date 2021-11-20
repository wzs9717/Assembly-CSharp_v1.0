using UnityEngine;

public class LineDrawer
{
	public Material material;

	private Mesh mesh;

	private Vector3[] meshVerts;

	private int numVertices;

	private int numLines = 1;

	private Matrix4x4 drawMatrix;

	public LineDrawer(Material m)
	{
		material = m;
		numLines = 1;
		_LineDrawer();
	}

	public LineDrawer(int numberOfLines, Material m)
	{
		material = m;
		numLines = numberOfLines;
		_LineDrawer();
	}

	private void _LineDrawer()
	{
		mesh = new Mesh();
		numVertices = numLines * 2;
		drawMatrix = Matrix4x4.identity;
		meshVerts = new Vector3[numVertices];
		int[] array = new int[numVertices];
		Vector2[] array2 = new Vector2[numVertices];
		Vector3[] normals = new Vector3[numVertices];
		for (int i = 0; i < numVertices; i++)
		{
			array[i] = i;
			array2[i].x = 0f;
			array2[i].y = 0f;
		}
		mesh.vertices = meshVerts;
		mesh.uv = array2;
		mesh.normals = normals;
		mesh.SetIndices(array, MeshTopology.Lines, 0);
	}

	public void SetLinePoints(int lineIndex, Vector3 point1, Vector3 point2)
	{
		int num = lineIndex * 2;
		meshVerts[num] = point1;
		meshVerts[num + 1] = point2;
	}

	public void SetLinePoints(Vector3 point1, Vector3 point2)
	{
		SetLinePoints(0, point1, point2);
	}

	public void Draw(int layer = 0)
	{
		mesh.vertices = meshVerts;
		mesh.RecalculateBounds();
		if ((bool)material)
		{
			Graphics.DrawMesh(mesh, drawMatrix, material, layer, null, 0, null, castShadows: false, receiveShadows: false);
		}
	}
}
