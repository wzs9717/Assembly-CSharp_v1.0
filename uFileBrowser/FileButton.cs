using UnityEngine;
using UnityEngine.UI;

namespace uFileBrowser
{
	public class FileButton : MonoBehaviour
	{
		public Button button;

		public Image buttonImage;

		public Image fileIcon;

		public Text label;

		public Sprite selectedSprite;

		[HideInInspector]
		public string text;

		[HideInInspector]
		public string fullPath;

		[HideInInspector]
		public bool isDir;

		[HideInInspector]
		public int id;

		private FileBrowser browser;

		public void Select()
		{
			button.transition = Selectable.Transition.None;
			buttonImage.overrideSprite = selectedSprite;
		}

		public void Unselect()
		{
			button.transition = Selectable.Transition.SpriteSwap;
			buttonImage.overrideSprite = null;
		}

		public void OnClick()
		{
			if ((bool)browser)
			{
				browser.OnFileClick(id);
			}
		}

		public void Set(FileBrowser b, string txt, string path, bool dir, int i)
		{
			browser = b;
			text = txt;
			fullPath = path;
			isDir = dir;
			id = i;
			label.text = text;
			if (isDir)
			{
				fileIcon.sprite = b.folderIcon;
			}
			else
			{
				fileIcon.sprite = b.GetFileIcon(txt);
			}
		}
	}
}
