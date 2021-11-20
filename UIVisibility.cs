using UnityEngine;

public class UIVisibility : MonoBehaviour
{
	[SerializeField]
	private bool _keepVisible;

	public bool keepVisible
	{
		get
		{
			return _keepVisible;
		}
		set
		{
			_keepVisible = value;
		}
	}
}
