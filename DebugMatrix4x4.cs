using UnityEngine;

public class DebugMatrix4x4 : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		string text = string.Empty;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				text = text + localToWorldMatrix[i, j].ToString("F4") + ":";
			}
			text += "\n";
		}
		Debug.Log("Matrix is\n" + text);
		Debug.Log("Quaternion is \n" + base.transform.localRotation.ToString("F2"));
	}
}
