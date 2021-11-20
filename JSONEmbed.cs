using System.IO;
using UnityEngine;

public class JSONEmbed : MonoBehaviour
{
	public string loadPath;

	public string jsonStore;

	public string imgPath;

	public Sprite sprite;

	public void Load()
	{
		if (loadPath != string.Empty)
		{
			StreamReader streamReader = new StreamReader(loadPath);
			jsonStore = streamReader.ReadToEnd();
			streamReader.Close();
		}
	}

	public void LoadImage()
	{
		byte[] array = File.ReadAllBytes(imgPath);
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(array);
		Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
		sprite = Sprite.Create(texture2D, rect, new Vector2(0f, 0f));
	}
}
