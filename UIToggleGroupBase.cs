using UnityEngine;

public class UIToggleGroupBase : MonoBehaviour
{
	private static int _currentBase;

	private static int _baseIncrement = 20;

	private int _baseNumber;

	public int baseNumber => _baseNumber;

	private void Start()
	{
		_baseNumber = _currentBase;
		_currentBase += _baseIncrement;
	}
}
