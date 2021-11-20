using System.Threading;

public class DAZMorphTaskInfo
{
	public DAZMorphTaskType taskType;

	public int threadIndex;

	public string name;

	public AutoResetEvent resetEvent;

	public Thread thread;

	public volatile bool working;

	public volatile bool kill;

	public int index1;

	public int index2;
}
