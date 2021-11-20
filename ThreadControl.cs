using UnityEngine;

public class ThreadControl : MonoBehaviour
{
	public delegate void OnNumSubThreadsChanged(int numThreads);

	public static ThreadControl singleton;

	public OnNumSubThreadsChanged onNumSubThreadsChangedHandlers;

	[SerializeField]
	private int _numSubThreads;

	public int numSubThreads
	{
		get
		{
			return _numSubThreads;
		}
		set
		{
			if (_numSubThreads != value)
			{
				_numSubThreads = value;
				if (onNumSubThreadsChangedHandlers != null)
				{
					onNumSubThreadsChangedHandlers(_numSubThreads);
				}
			}
		}
	}

	public float numSubThreadsFloat
	{
		get
		{
			return _numSubThreads;
		}
		set
		{
			numSubThreads = (int)value;
		}
	}

	private void Awake()
	{
		singleton = this;
	}
}
