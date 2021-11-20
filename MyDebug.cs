using UnityEngine;

public static class MyDebug
{
	public static void DrawWireCube(Vector3 position, float size, Color color)
	{
		Vector3 vector = default(Vector3);
		vector.x = position.x + size;
		vector.y = position.y + size;
		vector.z = position.z + size;
		Vector3 vector2 = default(Vector3);
		vector2.x = position.x - size;
		vector2.y = position.y + size;
		vector2.z = position.z + size;
		Vector3 vector3 = default(Vector3);
		vector3.x = position.x - size;
		vector3.y = position.y - size;
		vector3.z = position.z + size;
		Vector3 vector4 = default(Vector3);
		vector4.x = position.x + size;
		vector4.y = position.y - size;
		vector4.z = position.z + size;
		Vector3 vector5 = default(Vector3);
		vector5.x = position.x + size;
		vector5.y = position.y + size;
		vector5.z = position.z - size;
		Vector3 vector6 = default(Vector3);
		vector6.x = position.x - size;
		vector6.y = position.y + size;
		vector6.z = position.z - size;
		Vector3 vector7 = default(Vector3);
		vector7.x = position.x - size;
		vector7.y = position.y - size;
		vector7.z = position.z - size;
		Vector3 vector8 = default(Vector3);
		vector8.x = position.x + size;
		vector8.y = position.y - size;
		vector8.z = position.z - size;
		Debug.DrawLine(vector, vector2, color);
		Debug.DrawLine(vector2, vector3, color);
		Debug.DrawLine(vector3, vector4, color);
		Debug.DrawLine(vector4, vector, color);
		Debug.DrawLine(vector, vector5, color);
		Debug.DrawLine(vector2, vector6, color);
		Debug.DrawLine(vector2, vector6, color);
		Debug.DrawLine(vector3, vector7, color);
		Debug.DrawLine(vector4, vector8, color);
		Debug.DrawLine(vector5, vector6, color);
		Debug.DrawLine(vector6, vector7, color);
		Debug.DrawLine(vector7, vector8, color);
		Debug.DrawLine(vector8, vector5, color);
	}
}
