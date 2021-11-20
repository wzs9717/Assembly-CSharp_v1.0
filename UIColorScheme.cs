using UnityEngine;
using UnityEngine.UI;

public class UIColorScheme : MonoBehaviour
{
	public Color imageColor = new Color(1f, 1f, 1f, 1f);

	public Color buttonNormalColor = Color.white;

	public Color buttonHighlightedColor = new Color(0.7f, 0.7f, 0.7f, 1f);

	public Color buttonPressedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	public Color buttonDisabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	public Color buttonImageColor = new Color(0.7f, 0.7f, 0.7f, 1f);

	public float buttonColorMultiplier = 1f;

	public Color toggleNormalColor = Color.white;

	public Color toggleHighlightedColor = new Color(0.7f, 0.7f, 0.7f, 1f);

	public Color togglePressedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	public Color toggleDisabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	public float toggleColorMultiplier = 1f;

	public Color sliderNormalColor = Color.white;

	public Color sliderHighlightedColor = new Color(0.7f, 0.7f, 0.7f, 1f);

	public Color sliderPressedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	public Color sliderDisabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	public Color sliderImageColor = new Color(0f, 0f, 0f, 0.5f);

	public float sliderColorMultiplier = 1f;

	public Color textColor = new Color(0.2f, 0.2f, 0.2f, 1f);

	public void UpdateColors()
	{
		Image[] componentsInChildren = GetComponentsInChildren<Image>(includeInactive: true);
		foreach (Image image in componentsInChildren)
		{
			if (0 == 0)
			{
				image.color = imageColor;
				image.SetAllDirty();
			}
		}
		Button[] componentsInChildren2 = GetComponentsInChildren<Button>(includeInactive: true);
		foreach (Button button in componentsInChildren2)
		{
			bool flag = false;
			bool flag2 = false;
			if (!flag)
			{
				ColorBlock colors = button.colors;
				colors.normalColor = buttonNormalColor;
				colors.highlightedColor = buttonHighlightedColor;
				colors.pressedColor = buttonPressedColor;
				colors.disabledColor = buttonDisabledColor;
				colors.colorMultiplier = buttonColorMultiplier;
				button.colors = colors;
			}
			Image component = button.GetComponent<Image>();
			if (!flag2 && component != null)
			{
				component.color = buttonImageColor;
				component.SetAllDirty();
			}
		}
		Toggle[] componentsInChildren3 = GetComponentsInChildren<Toggle>(includeInactive: true);
		foreach (Toggle toggle in componentsInChildren3)
		{
			if (0 == 0)
			{
				ColorBlock colors2 = toggle.colors;
				colors2.normalColor = toggleNormalColor;
				colors2.highlightedColor = toggleHighlightedColor;
				colors2.pressedColor = togglePressedColor;
				colors2.disabledColor = toggleDisabledColor;
				colors2.colorMultiplier = toggleColorMultiplier;
				toggle.colors = colors2;
			}
		}
		Slider[] componentsInChildren4 = GetComponentsInChildren<Slider>(includeInactive: true);
		foreach (Slider slider in componentsInChildren4)
		{
			bool flag3 = false;
			bool flag4 = false;
			if (!flag3)
			{
				ColorBlock colors3 = slider.colors;
				colors3.normalColor = toggleNormalColor;
				colors3.highlightedColor = toggleHighlightedColor;
				colors3.pressedColor = togglePressedColor;
				colors3.disabledColor = toggleDisabledColor;
				colors3.colorMultiplier = toggleColorMultiplier;
				slider.colors = colors3;
			}
			Image component2 = slider.GetComponent<Image>();
			if (!flag4 && component2 != null)
			{
				component2.color = sliderImageColor;
				component2.SetAllDirty();
			}
		}
		Text[] componentsInChildren5 = GetComponentsInChildren<Text>(includeInactive: true);
		foreach (Text text in componentsInChildren5)
		{
			if (0 == 0)
			{
				text.color = textColor;
			}
		}
	}
}
