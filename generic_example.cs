using uFileBrowser;
using UnityEngine;
using UnityEngine.UI;

public class generic_example : MonoBehaviour
{
	public FileBrowser browser;

	public Text file;

	public void ShowButtonClick()
	{
		browser.Show(BrowserClosed);
	}

	public void BrowserClosed(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			Debug.Log("No file selected.");
		}
		else
		{
			file.text = "You selected:\n" + path;
		}
	}
}
