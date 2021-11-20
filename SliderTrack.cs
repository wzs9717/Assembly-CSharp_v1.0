using UnityEngine;
using UnityEngine.UI;

public class SliderTrack : MonoBehaviour
{
	public Slider master;

	protected Slider slave;

	private void Update()
	{
		if (master != null && slave != null)
		{
			slave.value = master.value;
		}
	}

	private void Start()
	{
		slave = GetComponent<Slider>();
	}
}
