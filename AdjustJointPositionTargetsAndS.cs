public class AdjustJointPositionTargetsAndSetDAZMorph : AdjustJointPositionTargets
{
	protected SetDAZMorph setDAZMorph;

	protected void Init()
	{
		setDAZMorph = GetComponent<SetDAZMorph>();
	}

	protected override void Adjust()
	{
		base.Adjust();
		if (setDAZMorph == null)
		{
			Init();
		}
		if (on && setDAZMorph != null)
		{
			setDAZMorph.morphPercent = _percent;
		}
	}

	public void OnEnable()
	{
		Adjust();
	}
}
