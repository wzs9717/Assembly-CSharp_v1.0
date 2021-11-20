using UnityEngine;

public class PlayerTransform : MonoBehaviour
{
	public static Transform player;

	private void Start()
	{
		player = base.transform;
	}
}
