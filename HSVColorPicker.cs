using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HSVColorPicker : TwoDPicker
{
	public delegate void OnColorChanged(Color color);

	public Slider hueSlider;

	public Image hueImage;

	public float defaultHue;

	[SerializeField]
	private float _hue;

	[SerializeField]
	private int _hueInt;

	public float defaultSaturation;

	public float defaultCvalue;

	public Slider redSlider;

	public Slider greenSlider;

	public Slider blueSlider;

	public Image colorSample;

	public Transform colorObject;

	[SerializeField]
	private float _red;

	[SerializeField]
	private float _green;

	[SerializeField]
	private float _blue;

	public Color currentColor;

	public HSVColor currentHSVColor;

	private static bool cacheBuildTriggered;

	private static Dictionary<int, Sprite> cachedSprites;

	public OnColorChanged onColorChangedHandlers;

	private bool wasInit;

	public float hue
	{
		get
		{
			return _hue;
		}
		set
		{
			if (_hue != value)
			{
				_hue = value;
				_hue = Mathf.Clamp01(_hue);
				_hueInt = Mathf.FloorToInt(_hue * 255f);
				RegenerateSVImage();
				RecalcRGB();
				if (hueSlider != null)
				{
					hueSlider.value = _hue;
				}
			}
		}
	}

	public float saturation
	{
		get
		{
			return _xVal;
		}
		set
		{
			if (base.xVal != value)
			{
				base.xVal = value;
				RecalcRGB();
			}
		}
	}

	public float cvalue
	{
		get
		{
			return _yVal;
		}
		set
		{
			if (base.yVal != value)
			{
				base.yVal = value;
				RecalcRGB();
			}
		}
	}

	public float red
	{
		get
		{
			return _red;
		}
		set
		{
			if (_red != value)
			{
				_red = value;
				_red = Mathf.Clamp01(_red);
				RecalcHSV();
			}
		}
	}

	public float green
	{
		get
		{
			return _green;
		}
		set
		{
			if (_green != value)
			{
				_green = value;
				_green = Mathf.Clamp01(_green);
				RecalcHSV();
			}
		}
	}

	public float blue
	{
		get
		{
			return _blue;
		}
		set
		{
			if (_blue != value)
			{
				_blue = value;
				_blue = Mathf.Clamp01(_blue);
				RecalcHSV();
			}
		}
	}

	private void SetCurrentColorFromRGB()
	{
		currentColor = new Color(_red, _green, _blue);
		if (colorSample != null)
		{
			colorSample.color = currentColor;
		}
		if (colorObject != null)
		{
			Component[] components = colorObject.GetComponents<Component>();
			foreach (Component component in components)
			{
				Type type = component.GetType();
				type.GetProperty("color")?.SetValue(component, currentColor, null);
			}
		}
		if (onColorChangedHandlers != null)
		{
			onColorChangedHandlers(currentColor);
		}
	}

	private void SetRGBSliders()
	{
		if (redSlider != null)
		{
			redSlider.value = _red * 255f;
		}
		if (greenSlider != null)
		{
			greenSlider.value = _green * 255f;
		}
		if (blueSlider != null)
		{
			blueSlider.value = _blue * 255f;
		}
	}

	private void RecalcRGB()
	{
		Init();
		currentHSVColor.H = hue;
		currentHSVColor.S = saturation;
		currentHSVColor.V = cvalue;
		Color color = HSVToRGB(hue, saturation, cvalue);
		_red = color.r;
		_green = color.g;
		_blue = color.b;
		SetRGBSliders();
		SetCurrentColorFromRGB();
	}

	private void RecalcHSV()
	{
		HSVColor hSVColor = RGBToHSV(_red, _green, _blue);
		hue = hSVColor.H;
		saturation = hSVColor.S;
		cvalue = hSVColor.V;
	}

	public static HSVColor RGBToHSV(float r, float g, float b)
	{
		HSVColor hSVColor = new HSVColor();
		float a = Mathf.Min(r, b);
		a = Mathf.Min(a, g);
		float a2 = Mathf.Max(r, b);
		a2 = (hSVColor.V = Mathf.Max(a2, g));
		float num = a2 - a;
		if (a2 != 0f)
		{
			hSVColor.S = num / a2;
			if (num == 0f)
			{
				hSVColor.H = 0f;
			}
			else if (r == a2)
			{
				hSVColor.H = (g - b) / num;
			}
			else if (g == a2)
			{
				hSVColor.H = 2f + (b - r) / num;
			}
			else
			{
				hSVColor.H = 4f + (r - g) / num;
			}
			hSVColor.H /= 6f;
			if (hSVColor.H < 0f)
			{
				hSVColor.H += 1f;
			}
			return hSVColor;
		}
		hSVColor.S = 0f;
		hSVColor.H = 0f;
		return hSVColor;
	}

	public static Color HSVToRGB(float H, float S, float V)
	{
		Color white = Color.white;
		if (S == 0f)
		{
			white.r = V;
			white.g = V;
			white.b = V;
		}
		else if (V == 0f)
		{
			white.r = 0f;
			white.g = 0f;
			white.b = 0f;
		}
		else
		{
			white.r = 0f;
			white.g = 0f;
			white.b = 0f;
			float num = H * 6f;
			int num2 = (int)Mathf.Floor(num);
			float num3 = num - (float)num2;
			float num4 = V * (1f - S);
			float num5 = V * (1f - S * num3);
			float num6 = V * (1f - S * (1f - num3));
			switch (num2)
			{
			case -1:
				white.r = V;
				white.g = num4;
				white.b = num5;
				break;
			case 0:
				white.r = V;
				white.g = num6;
				white.b = num4;
				break;
			case 1:
				white.r = num5;
				white.g = V;
				white.b = num4;
				break;
			case 2:
				white.r = num4;
				white.g = V;
				white.b = num6;
				break;
			case 3:
				white.r = num4;
				white.g = num5;
				white.b = V;
				break;
			case 4:
				white.r = num6;
				white.g = num4;
				white.b = V;
				break;
			case 5:
				white.r = V;
				white.g = num4;
				white.b = num5;
				break;
			case 6:
				white.r = V;
				white.g = num6;
				white.b = num4;
				break;
			}
			white.r = Mathf.Clamp(white.r, 0f, 1f);
			white.g = Mathf.Clamp(white.g, 0f, 1f);
			white.b = Mathf.Clamp(white.b, 0f, 1f);
		}
		return white;
	}

	public void RegenerateSVImage()
	{
		if (cachedSprites == null)
		{
			cachedSprites = new Dictionary<int, Sprite>();
		}
		if (!cachedSprites.TryGetValue(_hueInt, out var value))
		{
			Texture2D texture2D = new Texture2D(256, 256);
			for (int i = 0; i < 256; i++)
			{
				for (int j = 0; j < 256; j++)
				{
					texture2D.SetPixel(i, j, HSVToRGB(_hue, (float)i / 255f, (float)j / 255f));
				}
			}
			texture2D.Apply();
			Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
			value = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f));
			cachedSprites.Add(_hueInt, value);
		}
		Image component = GetComponent<Image>();
		if (component != null)
		{
			component.sprite = value;
			component.color = Color.white;
			component.type = Image.Type.Simple;
		}
	}

	private void RegenerateHueImage()
	{
		if (hueImage != null)
		{
			Texture2D texture2D = new Texture2D(1, 256);
			for (int i = 0; i < 256; i++)
			{
				texture2D.SetPixel(0, i, HSVToRGB((float)i / 255f, 1f, 1f));
			}
			texture2D.Apply();
			Rect rect = new Rect(0f, 0f, 1f, texture2D.height);
			Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f));
			hueImage.sprite = sprite;
			hueImage.color = Color.white;
			hueImage.type = Image.Type.Simple;
		}
	}

	private IEnumerator GenerateAllCachedImages()
	{
		int saveHueInt = _hueInt;
		float saveHue = _hue;
		for (int i = 0; i < 256; i++)
		{
			_hueInt = i;
			_hue = (float)_hueInt / 255f;
			RegenerateSVImage();
			yield return null;
		}
		_hueInt = saveHueInt;
		_hue = saveHue;
	}

	private void RegenerateImages()
	{
		RegenerateSVImage();
		RegenerateHueImage();
	}

	protected override void SetSelectorPositionFromXYVal()
	{
		base.SetSelectorPositionFromXYVal();
		RecalcRGB();
	}

	private void Init()
	{
		if (!wasInit || currentHSVColor == null)
		{
			currentHSVColor = new HSVColor();
			defaultHue = hue;
			defaultSaturation = saturation;
			defaultCvalue = cvalue;
			wasInit = true;
		}
	}

	public void Reset()
	{
		Init();
		hue = defaultHue;
		saturation = defaultSaturation;
		cvalue = defaultCvalue;
		RecalcRGB();
	}

	private void OnEnable()
	{
		Init();
		RegenerateImages();
	}

	private void Start()
	{
		Init();
		if (!cacheBuildTriggered)
		{
			cacheBuildTriggered = true;
			GenerateAllCachedImages();
		}
		RegenerateImages();
	}
}
