using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
	public Text statusText;

	public Button startButton;

	public Button startButtonAlt;

	public Slider fullProgressSlider;

	public Slider individualProgressSlider;

	public string[] preloadScenes;

	public string sceneCheckFile;

	public string sceneCheckFilePathPrefix;

	public string sceneName;

	public bool loadOnStart;

	public bool activateWhenLoaded = true;

	public bool loadAsync = true;

	protected bool isLoading;

	protected bool isLoadingMainScene;

	protected AsyncOperation async;

	public Material progressMaterial;

	public string progressMaterialFieldName;

	protected float progressTarget;

	protected float progress;

	public float progressMaxSpeed = 0.005f;

	public RectTransform contentLevelParent;

	public Transform contentLevelTogglePrefab;

	public ToggleGroup contentLevelToggleGroup;

	public float contentLevelToggleStartY = 100f;

	public float contentLevelToggleBuffer = 5f;

	public Transform singleSceneBanner;

	public Transform multiSceneBanner;

	public Transform keyIssueBanner;

	protected Dictionary<string, string> keyNameToKeyVal;

	protected string firstSceneName;

	protected bool singleScene;

	public Text keyInputField;

	public Text keyEntryStatus;

	public void AddKey()
	{
		if (sceneCheckFile == null || !(sceneCheckFile != string.Empty) || !(keyInputField != null))
		{
			return;
		}
		JSONClass jSONClass;
		if (File.Exists(sceneCheckFile))
		{
			StreamReader streamReader = new StreamReader(sceneCheckFile);
			string aJSON = streamReader.ReadToEnd();
			streamReader.Close();
			JSONNode jSONNode = JSON.Parse(aJSON);
			if (jSONNode == null)
			{
				jSONClass = new JSONClass();
			}
			else
			{
				jSONClass = jSONNode.AsObject;
				if (jSONClass == null)
				{
					jSONClass = new JSONClass();
				}
			}
		}
		else
		{
			jSONClass = new JSONClass();
		}
		string text = keyInputField.text;
		int buildIndexByScenePath = SceneUtility.GetBuildIndexByScenePath(sceneCheckFilePathPrefix + "/" + text);
		if (buildIndexByScenePath != -1)
		{
			if (keyEntryStatus != null)
			{
				keyEntryStatus.text = "Key accepted";
			}
			jSONClass[text].AsBool = true;
			string value = jSONClass.ToString(string.Empty);
			StreamWriter streamWriter = new StreamWriter(sceneCheckFile);
			streamWriter.Write(value);
			streamWriter.Close();
			GenerateContentLevelToggles();
		}
		else if (keyEntryStatus != null)
		{
			keyEntryStatus.text = "Invalid key";
		}
	}

	protected void GenerateContentLevelToggles()
	{
		if (sceneCheckFile != null && sceneCheckFile != string.Empty && contentLevelParent != null && contentLevelTogglePrefab != null)
		{
			foreach (Transform item in contentLevelParent)
			{
				if (item.name != "Label")
				{
					Object.Destroy(item.gameObject);
				}
			}
			contentLevelParent.gameObject.SetActive(value: false);
			keyNameToKeyVal = new Dictionary<string, string>();
			if (File.Exists(sceneCheckFile))
			{
				StreamReader streamReader = new StreamReader(sceneCheckFile);
				string aJSON = streamReader.ReadToEnd();
				streamReader.Close();
				JSONNode jSONNode = JSON.Parse(aJSON);
				if (jSONNode != null)
				{
					JSONClass asObject = jSONNode.AsObject;
					if (asObject != null)
					{
						float num = contentLevelToggleStartY;
						int num2 = 0;
						Toggle toggle = null;
						firstSceneName = null;
						List<string> list = new List<string>(asObject.Keys);
						list.Sort(delegate(string s1, string s2)
						{
							char c = s1[0];
							char c2 = s2[0];
							if (c == c2)
							{
								return 0;
							}
							switch (c)
							{
							case 'F':
								return -1;
							case 'T':
								if (c2 == 'F')
								{
									return 1;
								}
								return -1;
							case 'E':
								if (c2 == 'F' || c2 == 'T')
								{
									return 1;
								}
								return -1;
							default:
								return 1;
							}
						});
						Vector2 anchoredPosition = default(Vector2);
						foreach (string item2 in list)
						{
							string text = item2[0] switch
							{
								'F' => "Free", 
								'T' => "Teaser", 
								'E' => "Entertainer", 
								'C' => "Creator", 
								_ => "Unknown", 
							};
							if (text != null && item2 != null && text != "Unknown")
							{
								if (firstSceneName == null)
								{
									firstSceneName = item2;
								}
								num2++;
								keyNameToKeyVal.Add(text, item2);
								anchoredPosition.x = 0f;
								anchoredPosition.y = num;
								Transform transform2 = Object.Instantiate(contentLevelTogglePrefab);
								transform2.SetParent(contentLevelParent, worldPositionStays: false);
								RectTransform component = transform2.GetComponent<RectTransform>();
								component.anchoredPosition = anchoredPosition;
								num += component.sizeDelta.y + contentLevelToggleBuffer;
								Toggle componentInChildren = transform2.GetComponentInChildren<Toggle>();
								if (componentInChildren != null)
								{
									toggle = componentInChildren;
									componentInChildren.isOn = false;
									componentInChildren.group = contentLevelToggleGroup;
								}
								Text componentInChildren2 = transform2.GetComponentInChildren<Text>();
								if (componentInChildren2 != null)
								{
									componentInChildren2.text = text;
								}
							}
							else
							{
								Debug.LogError("Invalid key file");
								if (keyIssueBanner != null)
								{
									keyIssueBanner.gameObject.SetActive(value: true);
								}
							}
						}
						if (toggle != null)
						{
							toggle.isOn = true;
						}
						if (num2 > 0)
						{
							float y = num + contentLevelToggleBuffer;
							Vector2 sizeDelta = contentLevelParent.sizeDelta;
							sizeDelta.y = y;
							contentLevelParent.sizeDelta = sizeDelta;
						}
						if (num2 == 1)
						{
							singleScene = true;
							if (singleSceneBanner != null)
							{
								singleSceneBanner.gameObject.SetActive(value: true);
							}
							if (multiSceneBanner != null)
							{
								multiSceneBanner.gameObject.SetActive(value: false);
							}
						}
						else if (num2 >= 1)
						{
							singleScene = false;
							contentLevelParent.gameObject.SetActive(value: true);
							if (singleSceneBanner != null)
							{
								singleSceneBanner.gameObject.SetActive(value: false);
							}
							if (multiSceneBanner != null)
							{
								multiSceneBanner.gameObject.SetActive(value: true);
							}
						}
						else
						{
							Debug.LogError("No valid scenes found in keys file");
							if (keyIssueBanner != null)
							{
								keyIssueBanner.gameObject.SetActive(value: true);
							}
						}
					}
					else
					{
						Debug.LogError("Invalid key file");
						if (keyIssueBanner != null)
						{
							keyIssueBanner.gameObject.SetActive(value: true);
						}
					}
				}
				else
				{
					Debug.LogError("Invalid key file");
					if (keyIssueBanner != null)
					{
						keyIssueBanner.gameObject.SetActive(value: true);
					}
				}
			}
			else
			{
				Debug.LogError("Key file missing");
				if (keyIssueBanner != null)
				{
					keyIssueBanner.gameObject.SetActive(value: true);
				}
			}
		}
		else if (contentLevelParent != null)
		{
			contentLevelParent.gameObject.SetActive(value: false);
		}
	}

	protected string GetLoadSceneName()
	{
		if (sceneCheckFile != null && sceneCheckFile != string.Empty)
		{
			if (singleScene)
			{
				return firstSceneName;
			}
			if (contentLevelToggleGroup != null)
			{
				foreach (Toggle item in contentLevelToggleGroup.ActiveToggles())
				{
					Text componentInChildren = item.GetComponentInChildren<Text>();
					if (componentInChildren != null)
					{
						string text = componentInChildren.text;
						if (keyNameToKeyVal.TryGetValue(text, out var value))
						{
							return sceneCheckFilePathPrefix + "/" + value;
						}
					}
				}
			}
			return null;
		}
		return sceneName;
	}

	private IEnumerator LoadMainScene()
	{
		if (startButton != null)
		{
			startButton.interactable = false;
		}
		if (startButtonAlt != null)
		{
			startButtonAlt.interactable = false;
		}
		string loadSceneName = GetLoadSceneName();
		if (loadSceneName != null)
		{
			async = SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Single);
			yield return async;
			yield break;
		}
		Debug.LogError("Load scene name was null. Check key file");
		if (keyIssueBanner != null)
		{
			keyIssueBanner.gameObject.SetActive(value: true);
		}
	}

	private IEnumerator LoadMergeScenesAsync()
	{
		isLoading = true;
		yield return null;
		if (fullProgressSlider != null)
		{
			fullProgressSlider.gameObject.SetActive(value: true);
			fullProgressSlider.value = 0f;
		}
		if (individualProgressSlider != null)
		{
			individualProgressSlider.gameObject.SetActive(value: true);
			individualProgressSlider.value = 0f;
		}
		if (progressMaterial != null && progressMaterial.HasProperty(progressMaterialFieldName))
		{
			progressMaterial.SetFloat(progressMaterialFieldName, 0f);
		}
		int fullLength = preloadScenes.Length;
		for (int i = 0; i < preloadScenes.Length; i++)
		{
			async = SceneManager.LoadSceneAsync(preloadScenes[i], LoadSceneMode.Additive);
			yield return async;
			progressTarget = (float)(i + 1) / (float)fullLength;
		}
		isLoading = false;
	}

	protected void LoadScene()
	{
		if (startButton != null)
		{
			startButton.interactable = false;
		}
		if (startButtonAlt != null)
		{
			startButtonAlt.interactable = false;
		}
		if (loadAsync)
		{
			StartCoroutine(LoadMergeScenesAsync());
			return;
		}
		for (int i = 0; i < preloadScenes.Length; i++)
		{
			SceneManager.LoadScene(preloadScenes[i], LoadSceneMode.Additive);
		}
		string loadSceneName = GetLoadSceneName();
		if (loadSceneName != null)
		{
			SceneManager.LoadScene(loadSceneName, LoadSceneMode.Single);
		}
	}

	protected void ActivateScene()
	{
		StartCoroutine(LoadMainScene());
	}

	private void Start()
	{
		if (loadOnStart)
		{
			if (startButton != null)
			{
				startButton.gameObject.SetActive(value: true);
				startButton.onClick.AddListener(ActivateScene);
				startButton.interactable = false;
			}
			if (startButtonAlt != null)
			{
				startButtonAlt.gameObject.SetActive(value: true);
				startButtonAlt.onClick.AddListener(ActivateScene);
				startButtonAlt.interactable = false;
			}
			LoadScene();
		}
		else
		{
			if (startButton != null)
			{
				startButton.gameObject.SetActive(value: true);
				startButton.onClick.AddListener(LoadScene);
			}
			if (startButtonAlt != null)
			{
				startButtonAlt.gameObject.SetActive(value: true);
				startButtonAlt.onClick.AddListener(LoadScene);
			}
		}
		GenerateContentLevelToggles();
	}

	private void Update()
	{
		if (isLoading && individualProgressSlider != null)
		{
			individualProgressSlider.value = async.progress * 100f;
		}
		if (progressTarget > progress)
		{
			progress += progressMaxSpeed;
			if (progress > progressTarget)
			{
				progress = progressTarget;
			}
		}
		if (progress == 1f)
		{
			progress = progressTarget;
			if (activateWhenLoaded)
			{
				ActivateScene();
			}
			else
			{
				if (startButton != null)
				{
					startButton.interactable = true;
				}
				if (startButtonAlt != null)
				{
					startButtonAlt.interactable = true;
				}
			}
		}
		if (fullProgressSlider != null)
		{
			fullProgressSlider.value = progress * 100f;
		}
		if (progressMaterial != null && progressMaterial.HasProperty(progressMaterialFieldName))
		{
			progressMaterial.SetFloat(progressMaterialFieldName, progress);
		}
	}
}
