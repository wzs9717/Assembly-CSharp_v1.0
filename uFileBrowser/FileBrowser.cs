using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace uFileBrowser
{
	public class FileBrowser : MonoBehaviour
	{
		public class FileBrowserTaskInfo
		{
			public string name;

			public AutoResetEvent resetEvent;

			public Thread thread;

			public volatile bool working;

			public volatile bool kill;

			public int index1;

			public int index2;
		}

		private class QueuedImage
		{
			public string imgPath;

			public Image imageToLoadInto;

			public Texture2D tex;

			public bool jpgRead;

			public byte[] jpg;
		}

		public string defaultPath = string.Empty;

		public bool selectDirectory;

		public bool showFiles;

		public bool canCancel = true;

		public bool selectOnClick = true;

		public string fileFormat = string.Empty;

		public GameObject overlay;

		public GameObject window;

		public GameObject fileButtonPrefab;

		public GameObject directoryButtonPrefab;

		public RectTransform fileContent;

		public RectTransform dirContent;

		public InputField currentPathField;

		public InputField searchField;

		public Button searchCancelButton;

		public Button cancelButton;

		public Button selectButton;

		public Text selectButtonText;

		public Sprite folderIcon;

		public Sprite defaultIcon;

		public List<FileIcon> fileIcons = new List<FileIcon>();

		[SerializeField]
		[HideInInspector]
		private string currentPath;

		[SerializeField]
		[HideInInspector]
		private string search;

		[SerializeField]
		[HideInInspector]
		private string slash;

		[SerializeField]
		[HideInInspector]
		private List<string> drives;

		private List<FileButton> fileButtons;

		private List<DirectoryButton> dirButtons;

		private int selected = -1;

		private FileBrowserCallback callback;

		protected FileBrowserTaskInfo fileBrowserTask;

		protected bool _threadsRunning;

		private Dictionary<string, Sprite> imageCache;

		private Queue<QueuedImage> queuedImages;

		public string SelectedPath
		{
			get
			{
				if (selected > -1)
				{
					return fileButtons[selected].fullPath;
				}
				return null;
			}
		}

		private void Update()
		{
			StartThreads();
			if (!fileBrowserTask.working)
			{
				ProcessImageQueue();
				fileBrowserTask.working = true;
				fileBrowserTask.resetEvent.Set();
			}
		}

		protected void OnApplicationQuit()
		{
			if (Application.isPlaying)
			{
				StopThreads();
			}
		}

		private void Awake()
		{
			imageCache = new Dictionary<string, Sprite>();
			queuedImages = new Queue<QueuedImage>();
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			slash = directorySeparatorChar.ToString();
			if (Application.platform == RuntimePlatform.Android)
			{
				if (string.IsNullOrEmpty(defaultPath))
				{
					defaultPath = "/storage";
				}
			}
			else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			{
				drives = new List<string>(Directory.GetLogicalDrives());
			}
			else if ((Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) && string.IsNullOrEmpty(defaultPath))
			{
				defaultPath = "/home/";
			}
		}

		private void Start()
		{
		}

		public void Show(FileBrowserCallback callback)
		{
			GotoDirectory(defaultPath);
			UpdateUI();
			this.callback = callback;
			if ((bool)overlay)
			{
				overlay.SetActive(value: true);
			}
			window.SetActive(value: true);
		}

		public void Hide()
		{
			if (selected > -1)
			{
				fileButtons[selected].Unselect();
			}
			currentPath = string.Empty;
			selected = -1;
			search = string.Empty;
			if ((bool)overlay)
			{
				overlay.SetActive(value: false);
			}
			window.SetActive(value: false);
		}

		public void UpdateUI()
		{
			if ((bool)cancelButton)
			{
				cancelButton.gameObject.SetActive(canCancel);
			}
			if (currentPathField != null)
			{
				currentPathField.text = currentPath;
			}
			if (searchField != null)
			{
				searchField.text = search;
			}
		}

		public void OnFileClick(int i)
		{
			if (i >= fileButtons.Count)
			{
				Debug.LogError("uFileBrowser: Button index is bigger than array, something went wrong.");
			}
			else if (fileButtons[i].isDir)
			{
				if (!selectDirectory)
				{
					GotoDirectory(fileButtons[i].fullPath);
				}
				else
				{
					SelectFile(i);
				}
			}
			else
			{
				SelectFile(i);
			}
		}

		public void OnDirectoryClick(int i)
		{
			if (i >= dirButtons.Count)
			{
				Debug.LogError("uFileBrowser: Button index is bigger than array, something went wrong.");
			}
			else
			{
				GotoDirectory(dirButtons[i].fullPath);
			}
		}

		private void GotoDirectory(string path)
		{
			if (path == currentPath && path != string.Empty)
			{
				return;
			}
			if (string.IsNullOrEmpty(path))
			{
				if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
				{
					currentPath = string.Empty;
				}
				else if (Application.platform == RuntimePlatform.Android)
				{
					currentPath = "/storage";
				}
				else
				{
					currentPath = "/home/";
				}
			}
			else
			{
				if (!Directory.Exists(path))
				{
					Debug.LogError("uFileBrowser: Directory doesn't exist:\n" + path);
					return;
				}
				currentPath = path;
			}
			if ((bool)currentPathField)
			{
				currentPathField.text = currentPath;
			}
			selected = -1;
			UpdateFileList();
			UpdateDirectoryList();
		}

		private void SelectFile(int i)
		{
			if (i >= fileButtons.Count)
			{
				Debug.LogError("uFileBrowser: Selection index bigger than array.");
			}
			else if (i == selected && selectDirectory && fileButtons[i].isDir)
			{
				GotoDirectory(fileButtons[i].fullPath);
			}
			else if (fileButtons[i].isDir || !selectDirectory)
			{
				if (selected != -1)
				{
					fileButtons[selected].Unselect();
				}
				selected = i;
				fileButtons[i].Select();
				if (selectOnClick)
				{
					SelectButtonClicked();
				}
			}
		}

		private void UpdateFileList()
		{
			if (fileButtons == null)
			{
				fileButtons = new List<FileButton>();
			}
			else
			{
				for (int i = 0; i < fileButtons.Count; i++)
				{
					UnityEngine.Object.Destroy(fileButtons[i].gameObject);
				}
				fileButtons.Clear();
			}
			if ((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) && string.IsNullOrEmpty(currentPath))
			{
				for (int j = 0; j < drives.Count; j++)
				{
					CreateFileButton(drives[j], drives[j], dir: true, j);
				}
				return;
			}
			List<string> list = new List<string>();
			if ((selectDirectory && showFiles) || !selectDirectory)
			{
				try
				{
					list = new List<string>(Directory.GetFiles(currentPath));
					DirectoryInfo directoryInfo = new DirectoryInfo(currentPath);
					FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
					Array.Sort(fileSystemInfos, (FileSystemInfo a, FileSystemInfo b) => b.LastWriteTime.CompareTo(a.LastWriteTime));
					list = new List<string>();
					FileSystemInfo[] array = fileSystemInfos;
					foreach (FileSystemInfo fileSystemInfo in array)
					{
						list.Add(fileSystemInfo.FullName);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("uFileBrowser: " + ex.Message);
				}
				FilterFormat(list);
			}
			List<string> list2 = new List<string>();
			try
			{
				list2 = new List<string>(Directory.GetDirectories(currentPath));
			}
			catch (Exception ex2)
			{
				Debug.LogError("uFileBrowser: " + ex2.Message);
			}
			for (int l = 0; l < list2.Count; l++)
			{
				string text = list2[l].Substring(list2[l].LastIndexOf(slash) + 1);
				CreateFileButton(text, list2[l], dir: true, fileButtons.Count);
			}
			for (int m = 0; m < list.Count; m++)
			{
				string text2 = list[m].Substring(list[m].LastIndexOf(slash) + 1);
				CreateFileButton(text2, list[m], dir: false, fileButtons.Count);
			}
		}

		private void UpdateDirectoryList()
		{
			if (!directoryButtonPrefab)
			{
				return;
			}
			if (dirButtons == null)
			{
				dirButtons = new List<DirectoryButton>();
			}
			else
			{
				for (int i = 0; i < dirButtons.Count; i++)
				{
					UnityEngine.Object.Destroy(dirButtons[i].gameObject);
				}
				dirButtons.Clear();
			}
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			{
				CreateDirectoryButton("My Computer", string.Empty, 0);
			}
			else
			{
				CreateDirectoryButton(slash, slash, 0);
			}
			if (string.IsNullOrEmpty(currentPath))
			{
				return;
			}
			string[] array = currentPath.Split(slash[0]);
			for (int j = 0; j < array.Length; j++)
			{
				if (!string.IsNullOrEmpty(array[j]))
				{
					string text = currentPath.Substring(0, currentPath.LastIndexOf(array[j]));
					CreateDirectoryButton(array[j] + slash, text + array[j] + slash, dirButtons.Count);
				}
			}
		}

		private void FilterFormat(List<string> files)
		{
			if (string.IsNullOrEmpty(fileFormat))
			{
				return;
			}
			string[] array = fileFormat.Split('|');
			for (int i = 0; i < files.Count; i++)
			{
				bool flag = true;
				string text = string.Empty;
				if (files[i].Contains("."))
				{
					text = files[i].Substring(files[i].LastIndexOf('.') + 1).ToLowerInvariant();
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (text == array[j].Trim().ToLowerInvariant())
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					files.RemoveAt(i);
					i--;
				}
			}
		}

		private void FilterList()
		{
			if (string.IsNullOrEmpty(search))
			{
				for (int i = 0; i < fileButtons.Count; i++)
				{
					fileButtons[i].gameObject.SetActive(value: true);
				}
				return;
			}
			for (int j = 0; j < fileButtons.Count; j++)
			{
				if (!fileButtons[j].text.ToLowerInvariant().Contains(search))
				{
					fileButtons[j].gameObject.SetActive(value: false);
				}
				else
				{
					fileButtons[j].gameObject.SetActive(value: true);
				}
			}
		}

		protected void MTTask(object info)
		{
			FileBrowserTaskInfo fileBrowserTaskInfo = (FileBrowserTaskInfo)info;
			while (_threadsRunning)
			{
				fileBrowserTaskInfo.resetEvent.WaitOne(-1, exitContext: true);
				if (fileBrowserTaskInfo.kill)
				{
					break;
				}
				ProcessImageQueueThreaded();
				fileBrowserTaskInfo.working = false;
			}
		}

		protected void StopThreads()
		{
			_threadsRunning = false;
			if (fileBrowserTask != null)
			{
				fileBrowserTask.kill = true;
				fileBrowserTask.resetEvent.Set();
				while (fileBrowserTask.thread.IsAlive)
				{
				}
				fileBrowserTask = null;
			}
		}

		protected void StartThreads()
		{
			if (!_threadsRunning)
			{
				_threadsRunning = true;
				fileBrowserTask = new FileBrowserTaskInfo();
				fileBrowserTask.name = "FileBrowserTask";
				fileBrowserTask.resetEvent = new AutoResetEvent(initialState: false);
				fileBrowserTask.thread = new Thread(MTTask);
				fileBrowserTask.thread.Priority = System.Threading.ThreadPriority.BelowNormal;
				fileBrowserTask.thread.Start(fileBrowserTask);
			}
		}

		public void ClearCacheImage(string imgPath)
		{
			if (imageCache != null && imageCache.ContainsKey(imgPath))
			{
				imageCache.Remove(imgPath);
			}
		}

		protected void ProcessImageQueueThreaded()
		{
			if (queuedImages != null && queuedImages.Count > 0)
			{
				QueuedImage queuedImage = queuedImages.Peek();
				if (!queuedImage.jpgRead)
				{
					queuedImage.jpg = File.ReadAllBytes(queuedImage.imgPath);
					queuedImage.jpgRead = true;
				}
			}
		}

		private void CreateSprite(QueuedImage qi)
		{
			Sprite sprite = Sprite.Create(rect: new Rect(0f, 0f, qi.tex.width, qi.tex.height), texture: qi.tex, pivot: new Vector2(0f, 0f));
			imageCache.Add(qi.imgPath, sprite);
			qi.imageToLoadInto.sprite = sprite;
		}

		private void ProcessImageQueue()
		{
			if (queuedImages != null && queuedImages.Count > 0)
			{
				QueuedImage queuedImage = queuedImages.Peek();
				if (queuedImage.jpgRead)
				{
					queuedImages.Dequeue();
					queuedImage.tex.LoadImage(queuedImage.jpg);
					CreateSprite(queuedImage);
				}
			}
		}

		private IEnumerator ProcessImageQueueIE()
		{
			while (true)
			{
				if (queuedImages != null && queuedImages.Count > 0)
				{
					QueuedImage qi = queuedImages.Dequeue();
					WWW www = new WWW("file://" + qi.imgPath);
					yield return www;
					qi.tex = www.texture;
					CreateSprite(qi);
				}
				else
				{
					yield return null;
				}
			}
		}

		private void CreateFileButton(string text, string path, bool dir, int i)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(fileButtonPrefab, Vector3.zero, Quaternion.identity);
			gameObject.GetComponent<RectTransform>().SetParent(fileContent, worldPositionStays: false);
			FileButton component = gameObject.GetComponent<FileButton>();
			component.Set(this, text, path, dir, i);
			Transform transform = gameObject.transform.Find("Image");
			if (transform != null)
			{
				Image component2 = transform.GetComponent<Image>();
				if (component2 != null)
				{
					string text2 = path.Replace(".json", ".jpg");
					Sprite value = null;
					if (imageCache.TryGetValue(text2, out value))
					{
						component2.sprite = value;
					}
					else if (File.Exists(text2))
					{
						QueuedImage queuedImage = new QueuedImage();
						queuedImage.imgPath = text2;
						queuedImage.imageToLoadInto = component2;
						queuedImage.tex = new Texture2D(2, 2);
						queuedImages.Enqueue(queuedImage);
					}
				}
			}
			fileButtons.Add(component);
		}

		private void CreateDirectoryButton(string text, string path, int i)
		{
			if (dirContent != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(directoryButtonPrefab, Vector3.zero, Quaternion.identity);
				gameObject.GetComponent<RectTransform>().SetParent(dirContent, worldPositionStays: false);
				DirectoryButton component = gameObject.GetComponent<DirectoryButton>();
				component.Set(this, text, path, i);
				dirButtons.Add(component);
			}
		}

		public void PathFieldEndEdit()
		{
			if (currentPathField != null)
			{
				if (Directory.Exists(currentPathField.text))
				{
					GotoDirectory(currentPathField.text);
				}
				else
				{
					currentPathField.text = currentPath;
				}
			}
		}

		public void SearchChanged()
		{
			if ((bool)searchField)
			{
				search = searchField.text.Trim();
				FilterList();
			}
		}

		public void SearchCancelClick()
		{
			search = string.Empty;
			searchField.text = string.Empty;
			FilterList();
		}

		public void SelectButtonClicked()
		{
			if (selected > -1 && ((fileButtons[selected].isDir && selectDirectory) || (!fileButtons[selected].isDir && !selectDirectory)))
			{
				callback(fileButtons[selected].fullPath);
				Hide();
			}
		}

		public void CancelButtonClicked()
		{
			if (canCancel)
			{
				if (callback != null)
				{
					callback(string.Empty);
				}
				Hide();
			}
		}

		public Sprite GetFileIcon(string path)
		{
			string empty = string.Empty;
			if (path.Contains("."))
			{
				empty = path.Substring(path.LastIndexOf('.') + 1);
				for (int i = 0; i < fileIcons.Count; i++)
				{
					if (fileIcons[i].extension == empty)
					{
						return fileIcons[i].icon;
					}
				}
				return defaultIcon;
			}
			return defaultIcon;
		}
	}
}
