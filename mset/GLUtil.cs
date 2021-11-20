using UnityEngine;

namespace mset
{
	public class GLUtil
	{
		private static Vector3 prevStripVertex = Vector3.zero;

		public static void StripFirstVertex(Vector3 v)
		{
			prevStripVertex = v;
		}

		public static void StripFirstVertex3(float x, float y, float z)
		{
			prevStripVertex.Set(x, y, z);
		}

		public static void StripVertex3(float x, float y, float z)
		{
			GL.Vertex(prevStripVertex);
			GL.Vertex3(x, y, z);
			prevStripVertex.Set(x, y, z);
		}

		public static void StripVertex(Vector3 v)
		{
			GL.Vertex(prevStripVertex);
			GL.Vertex(v);
			prevStripVertex = v;
		}

		public static void DrawCube(Vector3 pos, Vector3 radius)
		{
			Vector3 vector = pos - radius;
			Vector3 vector2 = pos + radius;
			GL.Begin(7);
			GL.Vertex3(vector.x, vector.y, vector.z);
			GL.Vertex3(vector2.x, vector.y, vector.z);
			GL.Vertex3(vector2.x, vector.y, vector2.z);
			GL.Vertex3(vector.x, vector.y, vector2.z);
			GL.Vertex3(vector2.x, vector2.y, vector.z);
			GL.Vertex3(vector.x, vector2.y, vector.z);
			GL.Vertex3(vector.x, vector2.y, vector2.z);
			GL.Vertex3(vector2.x, vector2.y, vector2.z);
			GL.Vertex3(vector2.x, vector.y, vector.z);
			GL.Vertex3(vector2.x, vector2.y, vector.z);
			GL.Vertex3(vector2.x, vector2.y, vector2.z);
			GL.Vertex3(vector2.x, vector.y, vector2.z);
			GL.Vertex3(vector.x, vector2.y, vector.z);
			GL.Vertex3(vector.x, vector.y, vector.z);
			GL.Vertex3(vector.x, vector.y, vector2.z);
			GL.Vertex3(vector.x, vector2.y, vector2.z);
			GL.Vertex3(vector2.x, vector2.y, vector2.z);
			GL.Vertex3(vector.x, vector2.y, vector2.z);
			GL.Vertex3(vector.x, vector.y, vector2.z);
			GL.Vertex3(vector2.x, vector.y, vector2.z);
			GL.Vertex3(vector.x, vector2.y, vector.z);
			GL.Vertex3(vector2.x, vector2.y, vector.z);
			GL.Vertex3(vector2.x, vector.y, vector.z);
			GL.Vertex3(vector.x, vector.y, vector.z);
			GL.End();
		}

		public static void DrawWireCube(Vector3 pos, Vector3 radius)
		{
			Vector3 vector = pos - radius;
			Vector3 vector2 = pos + radius;
			GL.Begin(1);
			StripFirstVertex3(vector.x, vector.y, vector.z);
			StripVertex3(vector2.x, vector.y, vector.z);
			StripVertex3(vector2.x, vector.y, vector2.z);
			StripVertex3(vector.x, vector.y, vector2.z);
			StripFirstVertex3(vector2.x, vector2.y, vector.z);
			StripVertex3(vector.x, vector2.y, vector.z);
			StripVertex3(vector.x, vector2.y, vector2.z);
			StripVertex3(vector2.x, vector2.y, vector2.z);
			StripFirstVertex3(vector2.x, vector.y, vector.z);
			StripVertex3(vector2.x, vector2.y, vector.z);
			StripVertex3(vector2.x, vector2.y, vector2.z);
			StripVertex3(vector2.x, vector.y, vector2.z);
			StripFirstVertex3(vector.x, vector2.y, vector.z);
			StripVertex3(vector.x, vector.y, vector.z);
			StripVertex3(vector.x, vector.y, vector2.z);
			StripVertex3(vector.x, vector2.y, vector2.z);
			StripFirstVertex3(vector2.x, vector2.y, vector2.z);
			StripVertex3(vector.x, vector2.y, vector2.z);
			StripVertex3(vector.x, vector.y, vector2.z);
			StripVertex3(vector2.x, vector.y, vector2.z);
			StripFirstVertex3(vector.x, vector2.y, vector.z);
			StripVertex3(vector2.x, vector2.y, vector.z);
			StripVertex3(vector2.x, vector.y, vector.z);
			StripVertex3(vector.x, vector.y, vector.z);
			GL.End();
		}
	}
}
