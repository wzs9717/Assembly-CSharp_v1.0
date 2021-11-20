using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour, ISelectHandler, IDeselectHandler, IEventSystemHandler
{
	public delegate void OnOpenPopup();

	public delegate void OnValueChange(string value);

	public Color normalColor = Color.white;

	public Color normalBackgroundColor = Color.white;

	public Color selectColor = Color.blue;

	public Button topButton;

	public RectTransform popupPanel;

	public Image backgroundImage;

	public RectTransform popupButtonPrefab;

	[SerializeField]
	private RectTransform[] _popupTransforms;

	[SerializeField]
	private Button[] _popupButtons;

	[SerializeField]
	private int _numPopupValues;

	[SerializeField]
	private string[] _popupValues;

	public bool useDifferentDisplayValues;

	[SerializeField]
	private string[] _displayPopupValues;

	public string startingValue;

	public float topBottomBuffer = 5f;

	private bool _visible;

	private bool _makeInvisibleNextFrame;

	public OnOpenPopup onOpenPopupHandlers;

	public OnValueChange onValueChangeHandlers;

	[SerializeField]
	private string _currentValue = string.Empty;

	protected int _currentValueIndex = -1;

	protected bool _currentValueChanged;

	public int numPopupValues
	{
		get
		{
			return _numPopupValues;
		}
		set
		{
			if (_numPopupValues == value)
			{
				return;
			}
			ClearPanel();
			int num = _numPopupValues;
			_numPopupValues = value;
			string[] array = new string[_numPopupValues];
			string[] array2 = new string[_numPopupValues];
			for (int i = 0; i < _numPopupValues; i++)
			{
				if (i < num)
				{
					if (_displayPopupValues != null && i < _displayPopupValues.Length)
					{
						if (_displayPopupValues[i] == null)
						{
							array2[i] = string.Empty;
						}
						else
						{
							array2[i] = _displayPopupValues[i];
						}
					}
					if (_popupValues[i] == null)
					{
						array[i] = string.Empty;
					}
					else
					{
						array[i] = _popupValues[i];
					}
				}
				else
				{
					array[i] = string.Empty;
				}
			}
			_popupValues = array;
			_displayPopupValues = array2;
			CreatePanelButtons();
		}
	}

	public string[] popupValues => _popupValues;

	public string[] displayPopupValues => _displayPopupValues;

	public bool visible
	{
		get
		{
			return _visible;
		}
		set
		{
			if (_visible == value)
			{
				return;
			}
			_visible = value;
			if (_visible)
			{
				_makeInvisibleNextFrame = false;
				if (onOpenPopupHandlers != null)
				{
					onOpenPopupHandlers();
				}
				if (popupPanel != null)
				{
					popupPanel.gameObject.SetActive(value: true);
				}
			}
			else
			{
				_makeInvisibleNextFrame = true;
			}
		}
	}

	public string currentValue
	{
		get
		{
			return _currentValue;
		}
		set
		{
			if (!(_currentValue != value) && _currentValueIndex != -1)
			{
				return;
			}
			_currentValue = value;
			_currentValueChanged = true;
			_currentValueIndex = -1;
			for (int i = 0; i < _popupValues.Length; i++)
			{
				if (_currentValue == _popupValues[i])
				{
					_currentValueIndex = i;
					break;
				}
			}
			if (topButton != null && _currentValueIndex != -1)
			{
				Text[] componentsInChildren = topButton.GetComponentsInChildren<Text>(includeInactive: true);
				if (componentsInChildren != null)
				{
					if (useDifferentDisplayValues)
					{
						componentsInChildren[0].text = _displayPopupValues[_currentValueIndex];
					}
					else
					{
						componentsInChildren[0].text = _popupValues[_currentValueIndex];
					}
				}
			}
			if (onValueChangeHandlers != null)
			{
				onValueChangeHandlers(_currentValue);
			}
		}
	}

	public string currentValueNoCallback
	{
		get
		{
			return _currentValue;
		}
		set
		{
			if (!(_currentValue != value) && _currentValueIndex != -1)
			{
				return;
			}
			_currentValue = value;
			_currentValueIndex = -1;
			for (int i = 0; i < _popupValues.Length; i++)
			{
				if (_currentValue == _popupValues[i])
				{
					_currentValueIndex = i;
					break;
				}
			}
			if (topButton != null && _currentValueIndex != -1)
			{
				Text[] componentsInChildren = topButton.GetComponentsInChildren<Text>(includeInactive: true);
				if (componentsInChildren != null)
				{
					if (useDifferentDisplayValues)
					{
						componentsInChildren[0].text = _displayPopupValues[_currentValueIndex];
					}
					else
					{
						componentsInChildren[0].text = _popupValues[_currentValueIndex];
					}
				}
			}
			HighlightCurrentValue();
		}
	}

	public void setPopupValue(int index, string text)
	{
		if (index >= 0 && index < _numPopupValues)
		{
			_popupValues[index] = text;
			SetPanelButtonText(index);
			_currentValueChanged = true;
		}
	}

	public void setDisplayPopupValue(int index, string text)
	{
		if (index >= 0 && index < _numPopupValues)
		{
			_displayPopupValues[index] = text;
			SetPanelButtonText(index);
			_currentValueChanged = true;
		}
	}

	protected void HighlightCurrentValue()
	{
		if (_popupButtons == null || _popupButtons.Length != _numPopupValues)
		{
			return;
		}
		for (int i = 0; i < _numPopupValues; i++)
		{
			if (_popupButtons[i] != null)
			{
				ColorBlock colors = _popupButtons[i].colors;
				if (_popupValues[i] == currentValue)
				{
					colors.normalColor = selectColor;
				}
				else
				{
					colors.normalColor = normalColor;
				}
				_popupButtons[i].colors = colors;
			}
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (backgroundImage != null)
		{
			backgroundImage.color = selectColor;
		}
		else if (topButton != null)
		{
			ColorBlock colors = topButton.colors;
			colors.normalColor = selectColor;
			topButton.colors = colors;
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if (backgroundImage != null)
		{
			backgroundImage.color = normalBackgroundColor;
		}
		else if (topButton != null)
		{
			ColorBlock colors = topButton.colors;
			colors.normalColor = normalColor;
			topButton.colors = colors;
		}
	}

	public void SetPreviousValue()
	{
		if (_popupValues != null && _currentValueIndex > 0)
		{
			currentValue = _popupValues[_currentValueIndex - 1];
		}
	}

	public void SetNextValue()
	{
		if (_popupValues != null && _currentValueIndex < _popupValues.Length - 1)
		{
			currentValue = _popupValues[_currentValueIndex + 1];
		}
	}

	public void Toggle()
	{
		visible = !visible;
	}

	private void ClearPanel()
	{
		if (popupPanel != null)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (RectTransform item in popupPanel)
			{
				if (!popupButtonPrefab || !(popupButtonPrefab.gameObject == item.gameObject))
				{
					list.Add(item.gameObject);
				}
			}
			foreach (GameObject item2 in list)
			{
				Object.DestroyImmediate(item2);
			}
		}
		_popupTransforms = null;
		_popupButtons = null;
	}

	private void SetPanelButtonText(int index)
	{
		string text = _popupValues[index];
		RectTransform rectTransform = _popupTransforms[index];
		if (!(rectTransform != null))
		{
			return;
		}
		Button component = rectTransform.GetComponent<Button>();
		if (component != null)
		{
			_popupButtons[index] = component;
			string popupValueCopy = string.Copy(text);
			component.onClick.RemoveAllListeners();
			component.onClick.AddListener(delegate
			{
				currentValue = popupValueCopy;
			});
		}
		Text[] componentsInChildren = component.GetComponentsInChildren<Text>(includeInactive: true);
		if (componentsInChildren != null)
		{
			if (useDifferentDisplayValues)
			{
				componentsInChildren[0].text = _displayPopupValues[index];
			}
			else
			{
				componentsInChildren[0].text = text;
			}
		}
	}

	private void CreatePanelButtons()
	{
		if (_popupValues == null || !(popupButtonPrefab != null))
		{
			return;
		}
		_popupTransforms = new RectTransform[_numPopupValues];
		_popupButtons = new Button[_numPopupValues];
		int num = 1;
		float num2 = 2f * topBottomBuffer;
		for (int i = 0; i < _numPopupValues; i++)
		{
			RectTransform rectTransform = Object.Instantiate(popupButtonPrefab);
			_popupTransforms[i] = rectTransform;
			rectTransform.gameObject.SetActive(value: true);
			SetPanelButtonText(i);
			float height = rectTransform.rect.height;
			num2 += height;
			rectTransform.SetParent(popupPanel, worldPositionStays: false);
			Vector2 zero = Vector2.zero;
			zero.y = (float)num * (0f - height) + height * 0.5f - topBottomBuffer;
			rectTransform.anchoredPosition = zero;
			num++;
		}
		if (topButton != null)
		{
			RectTransform component = topButton.GetComponent<RectTransform>();
			if (component != null)
			{
				num2 += component.rect.height;
			}
		}
		RectTransform component2 = GetComponent<RectTransform>();
		if (component2 != null)
		{
			Vector2 sizeDelta = component2.sizeDelta;
			sizeDelta.y = num2;
			component2.sizeDelta = sizeDelta;
		}
		HighlightCurrentValue();
	}

	private void TestDelegate(string test)
	{
		Debug.Log("TestDelegate called with " + test);
	}

	private void Start()
	{
		popupPanel.gameObject.SetActive(value: false);
		ClearPanel();
		CreatePanelButtons();
	}

	private void Update()
	{
		if (_makeInvisibleNextFrame)
		{
			_makeInvisibleNextFrame = false;
			if (popupPanel != null)
			{
				popupPanel.gameObject.SetActive(value: false);
			}
			if (base.gameObject.activeInHierarchy && _currentValueChanged)
			{
				LookInputModule.SelectGameObject(base.gameObject);
			}
		}
		if (_currentValueChanged)
		{
			HighlightCurrentValue();
		}
		_currentValueChanged = false;
	}
}
