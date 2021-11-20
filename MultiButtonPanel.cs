using UnityEngine;
using UnityEngine.UI;

public class MultiButtonPanel : MonoBehaviour
{
	public delegate void ButtonCallback(string name);

	public Text headerText;

	public Button button1;

	public Text button1Text;

	public Button button2;

	public Text button2Text;

	public Button button3;

	public Text button3Text;

	[SerializeField]
	protected bool _showButton1;

	[SerializeField]
	protected bool _showButton2;

	[SerializeField]
	protected bool _showButton3;

	public ButtonCallback buttonCallback;

	public bool showButton1
	{
		get
		{
			return _showButton1;
		}
		set
		{
			_showButton1 = value;
			if (button1 != null)
			{
				button1.gameObject.SetActive(_showButton1);
			}
		}
	}

	public bool showButton2
	{
		get
		{
			return _showButton2;
		}
		set
		{
			_showButton2 = value;
			if (button2 != null)
			{
				button2.gameObject.SetActive(_showButton2);
			}
		}
	}

	public bool showButton3
	{
		get
		{
			return _showButton3;
		}
		set
		{
			_showButton3 = value;
			if (button3 != null)
			{
				button3.gameObject.SetActive(_showButton3);
			}
		}
	}

	protected void Button1Click()
	{
		if (button1Text != null && buttonCallback != null)
		{
			buttonCallback(button1Text.text);
		}
	}

	protected void Button2Click()
	{
		if (button2Text != null && buttonCallback != null)
		{
			buttonCallback(button2Text.text);
		}
	}

	protected void Button3Click()
	{
		if (button3Text != null && buttonCallback != null)
		{
			buttonCallback(button3Text.text);
		}
	}

	public void SetButton1Text(string t)
	{
		if (button1Text != null)
		{
			button1Text.text = t;
		}
	}

	public void SetButton2Text(string t)
	{
		if (button2Text != null)
		{
			button2Text.text = t;
		}
	}

	public void SetButton3Text(string t)
	{
		if (button3Text != null)
		{
			button3Text.text = t;
		}
	}

	private void OnEnable()
	{
		showButton1 = _showButton1;
		if (button1 != null)
		{
			button1.onClick.AddListener(Button1Click);
		}
		showButton2 = _showButton2;
		if (button2 != null)
		{
			button2.onClick.AddListener(Button2Click);
		}
		showButton3 = _showButton3;
		if (button3 != null)
		{
			button3.onClick.AddListener(Button3Click);
		}
	}

	private void OnDisable()
	{
		if (button1 != null)
		{
			button1.onClick.RemoveListener(Button1Click);
		}
		if (button2 != null)
		{
			button2.onClick.RemoveListener(Button2Click);
		}
		if (button3 != null)
		{
			button3.onClick.RemoveListener(Button3Click);
		}
	}
}
