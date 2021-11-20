public class SyncForceProducer : ForceProducer
{
	public ForceProducer syncProducer;

	public void SetSyncProducer(ForceProducer pdcr)
	{
		if (wasInit)
		{
			syncProducer = pdcr;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (syncProducer != null)
		{
			SetTargetForce(syncProducer.targetForceMagnitude);
		}
	}

	public void SetSyncForceProducerFromPopupList()
	{
	}
}
