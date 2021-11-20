using UnityEngine;
using UnityEngine.UI;

public class DAZSkinControl : MonoBehaviour
{
	protected DAZSkinV2 _skin;

	public Toggle useEarlyFinishToggle;

	[SerializeField]
	protected bool _useEarlyFinish;

	public DAZSkinV2 skin
	{
		get
		{
			return _skin;
		}
		set
		{
			if (_skin != value)
			{
				_skin = value;
				SyncUseEarlyFinish();
			}
		}
	}

	public bool useEarlyFinish
	{
		get
		{
			return _useEarlyFinish;
		}
		set
		{
			if (_useEarlyFinish != value)
			{
				_useEarlyFinish = value;
				if (useEarlyFinishToggle != null)
				{
					useEarlyFinishToggle.isOn = value;
				}
				SyncUseEarlyFinish();
			}
		}
	}

	protected void SyncUseEarlyFinish()
	{
		if (skin != null)
		{
			skin.useEarlyFinish = _useEarlyFinish;
		}
	}

	protected void InitUI()
	{
		if (useEarlyFinishToggle != null)
		{
			useEarlyFinishToggle.onValueChanged.AddListener(delegate
			{
				useEarlyFinish = useEarlyFinishToggle.isOn;
			});
			useEarlyFinishToggle.isOn = _useEarlyFinish;
			SyncUseEarlyFinish();
		}
	}

	private void Start()
	{
		InitUI();
	}
}
