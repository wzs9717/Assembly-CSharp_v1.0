using UnityEngine;

public class MoveAndRotateAs : MonoBehaviour
{
	public enum UpdateTime
	{
		Normal,
		Late,
		Fixed,
		Batch
	}

	public Transform MoveAndRotateAsTransform;

	public Vector3 Offset;

	public bool MoveAsEnabled = true;

	public bool RotateAsEnabled = true;

	public bool useRigidbody;

	public UpdateTime updateTime;

	public void DoUpdate()
	{
		if (!MoveAndRotateAsTransform)
		{
			return;
		}
		if (MoveAsEnabled)
		{
			if (useRigidbody)
			{
				Rigidbody component = base.transform.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.MovePosition(MoveAndRotateAsTransform.position + Offset);
				}
			}
			else
			{
				base.transform.position = MoveAndRotateAsTransform.position + Offset;
			}
		}
		if (!RotateAsEnabled)
		{
			return;
		}
		if (useRigidbody)
		{
			Rigidbody component2 = base.transform.GetComponent<Rigidbody>();
			if (component2 != null)
			{
				component2.MoveRotation(MoveAndRotateAsTransform.rotation);
			}
		}
		else
		{
			base.transform.rotation = MoveAndRotateAsTransform.rotation;
		}
	}

	private void FixedUpdate()
	{
		if (updateTime == UpdateTime.Fixed)
		{
			DoUpdate();
		}
	}

	private void Update()
	{
		if (updateTime == UpdateTime.Normal)
		{
			DoUpdate();
		}
	}

	private void LateUpdate()
	{
		if (updateTime == UpdateTime.Late)
		{
			DoUpdate();
		}
	}
}
