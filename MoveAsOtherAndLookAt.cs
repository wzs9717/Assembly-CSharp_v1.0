using UnityEngine;

public class MoveAsOtherAndLookAt : MonoBehaviour
{
	public enum LookAtTime
	{
		LookAtFirst,
		LookAtBeforeMove,
		LookAtAfterMove,
		LookAtBeforeAndAfterMove
	}

	public LookAtTime lookAtTime;

	public Transform MoveAsTransform;

	public FreeControllerV3 MoveAsFreeControllerV3;

	public Transform LookAtTransform;

	public bool MoveToLookHeight;

	public float LeftRightOffsetBeforeMove;

	public float LeftRightOffsetRelativeToDistanceBeforeMove;

	public float UpDownOffsetBeforeMove;

	public float MoveTowardsLookAtDistance;

	public float LeftRightOffsetAfterMove;

	public float UpDownOffsetAfterMove;

	public bool MoveSpecificDistanceFromLookAt;

	public bool MoveAsEnabled = true;

	public bool debug;

	private void Start()
	{
	}

	private void LateUpdate()
	{
		if (!LookAtTransform)
		{
			return;
		}
		if (MoveAsEnabled && ((bool)MoveAsTransform || (bool)MoveAsFreeControllerV3))
		{
			Vector3 vector = ((!MoveAsFreeControllerV3) ? MoveAsTransform.position : MoveAsFreeControllerV3.selectedPosition);
			if (MoveToLookHeight)
			{
				vector.y = LookAtTransform.position.y;
			}
			if (debug)
			{
				MyDebug.DrawWireCube(vector, 0.1f, Color.blue);
			}
			if (lookAtTime == LookAtTime.LookAtFirst)
			{
				base.transform.position = vector;
				base.transform.LookAt(LookAtTransform);
			}
			Vector3 vector2 = LookAtTransform.position - vector;
			float magnitude = vector2.magnitude;
			vector2.Normalize();
			Vector3 vector3 = Vector3.Cross(Vector3.up, vector2);
			vector3.Normalize();
			vector = vector + Vector3.up * UpDownOffsetBeforeMove + vector3 * LeftRightOffsetBeforeMove + vector3 * magnitude * LeftRightOffsetRelativeToDistanceBeforeMove;
			if (debug)
			{
				MyDebug.DrawWireCube(vector, 0.1f, Color.red);
			}
			if (lookAtTime == LookAtTime.LookAtBeforeMove || lookAtTime == LookAtTime.LookAtBeforeAndAfterMove)
			{
				vector2 = LookAtTransform.position - vector;
				vector2.Normalize();
				vector3 = Vector3.Cross(Vector3.up, vector2);
				vector3.Normalize();
				base.transform.position = vector;
				base.transform.LookAt(LookAtTransform);
			}
			if (MoveSpecificDistanceFromLookAt)
			{
				float num = Vector3.Distance(LookAtTransform.position, vector);
				vector = vector + vector2 * (num - MoveTowardsLookAtDistance) + Vector3.up * UpDownOffsetAfterMove + vector3 * LeftRightOffsetAfterMove;
			}
			else
			{
				vector = vector + vector2 * MoveTowardsLookAtDistance + Vector3.up * UpDownOffsetAfterMove + vector3 * LeftRightOffsetAfterMove;
			}
			base.transform.position = vector;
			if (lookAtTime == LookAtTime.LookAtAfterMove || lookAtTime == LookAtTime.LookAtBeforeAndAfterMove)
			{
				base.transform.LookAt(LookAtTransform);
			}
		}
		else
		{
			base.transform.LookAt(LookAtTransform);
		}
	}
}
