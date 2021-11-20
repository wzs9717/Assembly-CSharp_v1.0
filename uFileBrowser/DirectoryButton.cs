using UnityEngine;
using UnityEngine.UI;

namespace uFileBrowser
{
	public class DirectoryButton : MonoBehaviour
	{
		public Text label;

		[HideInInspector]
		public string text;

		[HideInInspector]
		public string fullPath;

		[HideInInspector]
		public int id;

		private FileBrowser browser;

		public void OnClick()
		{
			if ((bool)browser)
			{
				browser.OnDirectoryClick(id);
			}
		}

		public void Set(FileBrowser b, string txt, string path, int i)
		{
			browser = b;
			text = txt;
			fullPath = path;
			id = i;
			label.text = text;
		}
	}
}
