using UnityEngine;

public class CollisionDebugs : MonoBehaviour
{
	[SerializeField]
	private bool _allOn;

	public bool allOn
	{
		get
		{
			return _allOn;
		}
		set
		{
			if (_allOn != value)
			{
				_allOn = value;
				CollisionDebug[] componentsInChildren = GetComponentsInChildren<CollisionDebug>(includeInactive: true);
				CollisionDebug[] array = componentsInChildren;
				foreach (CollisionDebug collisionDebug in array)
				{
					collisionDebug.on = _allOn;
				}
			}
		}
	}
}
